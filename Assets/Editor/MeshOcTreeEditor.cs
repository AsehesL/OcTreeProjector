using System.Collections.Generic;
using OcTreeProjector;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MeshOcTree))]
public class MeshOcTreeEditor : Editor
{
    private MeshOcTree m_Target;

    [MenuItem("Assets/Create/OcTreeProjector/OcTree")]
    static void CreateTree()
    {
        MeshOcTree tree = MeshOcTree.CreateInstance<MeshOcTree>();
        ProjectWindowUtil.CreateAsset(tree, "New Tree.asset");
    }

    void OnEnable()
    {
        m_Target = (MeshOcTree) target;
    }

    public override void OnInspectorGUI()
    {
        if (m_Target.count == 0)
        {
            EditorGUILayout.HelpBox("空八叉树，请点击设置数据！", MessageType.Warning);
        }
        else
        {
            //base.OnInspectorGUI();
            GUILayout.Label("节点数：" + m_Target.count);
            GUILayout.Label("最大深度：" + m_Target.maxDepth);
            GUILayout.Label("中心坐标：" + m_Target.Bounds.center);
            GUILayout.Label("包围盒大小：" + m_Target.Bounds.size);
        }
        if (GUILayout.Button("重建OcTree"))
        {
            var wizard = ScriptableWizard.DisplayWizard<OcTreeCreateWizard>("从选中物体创建OcTree");
            wizard.tree = m_Target;
        }
    }
}

public class OcTreeCreateWizard : ScriptableWizard
{
    public MeshOcTree tree;

    private GameObject m_GameObject;
    private bool m_ContainChilds;

    void OnEnable()
    {
        maxSize = new Vector2(400, 100);
        minSize = new Vector2(400, 100);
    }

    void OnGUI()
    {
        m_GameObject = EditorGUILayout.ObjectField("根物体", m_GameObject, typeof (GameObject), true) as GameObject;
        m_ContainChilds = EditorGUILayout.Toggle("是否包含子物体", m_ContainChilds);

        GUILayout.Space(40);
        if (GUILayout.Button("创建"))
        {
            if (tree == null)
                return;
            BuildOcTree(tree);
            Close();
        }
    }

    private void BuildOcTree(MeshOcTree tree)
    {
        if (m_GameObject == null)
            return;
        MeshFilter[] mf = null;
        if (m_ContainChilds)
            mf = m_GameObject.GetComponentsInChildren<MeshFilter>();
        else
        {
            MeshFilter m = m_GameObject.GetComponent<MeshFilter>();
            if (m)
                mf = new MeshFilter[] {m};
            else
                return;
        }
        if (mf.Length == 0)
            return;

        float maxX = -Mathf.Infinity;
        float minX = Mathf.Infinity;
        float maxY = -Mathf.Infinity;
        float minY = Mathf.Infinity;
        float maxZ = -Mathf.Infinity;
        float minZ = Mathf.Infinity;

        List<OTMeshTriangle> triangles = new List<OTMeshTriangle>();

        for (int i = 0; i < mf.Length; i++)
        {
            if (mf[i].sharedMesh == null)
                continue;

            for (int j = 0; j < mf[i].sharedMesh.triangles.Length; j += 3)
            {
                Vector3 p1 =
                    mf[i].transform.localToWorldMatrix.MultiplyPoint(
                        mf[i].sharedMesh.vertices[mf[i].sharedMesh.triangles[j]]);
                Vector3 p2 =
                    mf[i].transform.localToWorldMatrix.MultiplyPoint(
                        mf[i].sharedMesh.vertices[mf[i].sharedMesh.triangles[j + 1]]);
                Vector3 p3 =
                    mf[i].transform.localToWorldMatrix.MultiplyPoint(
                        mf[i].sharedMesh.vertices[mf[i].sharedMesh.triangles[j + 2]]);

                maxX = Mathf.Max(maxX, p1.x, p2.x, p3.x);
                maxY = Mathf.Max(maxY, p1.y, p2.y, p3.y);
                maxZ = Mathf.Max(maxZ, p1.z, p2.z, p3.z);

                minX = Mathf.Min(minX, p1.x, p2.x, p3.x);
                minY = Mathf.Min(minY, p1.y, p2.y, p3.y);
                minZ = Mathf.Min(minZ, p1.z, p2.z, p3.z);

                OTMeshTriangle triangle = new OTMeshTriangle(p1, p2, p3);

                triangles.Add(triangle);
            }
        }

        Vector3 size = new Vector3(maxX - minX, maxY - minY, maxZ - minZ);
        Vector3 center = new Vector3(minX, minY, minZ) + size/2;
       
        tree.Build(center, size*1.1f, 5);
        for (int i = 0; i < triangles.Count; i++)
        {
            tree.Add(triangles[i]);
        }

        string path = AssetDatabase.GetAssetPath(tree);
        AssetDatabase.SaveAssets();
        AssetImporter importer = AssetImporter.GetAtPath(path);
        importer.SaveAndReimport();
    }

}