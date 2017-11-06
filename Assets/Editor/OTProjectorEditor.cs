using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof (OTProjector))]
public class OTProjectorEditor : Editor
{

    private OTProjector m_Target;

    private string[] m_Trees;

    private int m_Index = 0;

    void OnEnable()
    {
        m_Target = (OTProjector) target;

        List<string> treeList = new List<string>();
        treeList.Add("None");

        DirectoryInfo resDir = new DirectoryInfo("Assets/Resources");
        int i = 0;
        if (resDir.Exists)
        {
            FileInfo[] treeFiles = resDir.GetFiles("*.asset", SearchOption.AllDirectories);
            foreach (var file in treeFiles)
            {
                string filename = file.FullName;
                filename = filename.Replace('\\', '/');
                filename = FileUtil.GetProjectRelativePath(filename);
                var obj = AssetDatabase.LoadAssetAtPath<OcTreeProjector.MeshOcTree>(filename);
                if (filename.EndsWith(".asset"))
                {
                    filename = filename.Replace(".asset", "");
                }
                if (filename.StartsWith("Assets/Resources/"))
                    filename = filename.Replace("Assets/Resources/","");
                if (obj)
                {
                    i++;
                    treeList.Add(filename);
                    if (m_Target.ocTreeName == filename)
                        m_Index = i;
                }
            }
        }
        m_Trees = treeList.ToArray();
    }

    public override void OnInspectorGUI()
    {
        Undo.RecordObject(target, "Projector Inspector");

        m_Target.near = Mathf.Max(0.01f, EditorGUILayout.FloatField("Near", m_Target.near));
        m_Target.far = Mathf.Max(m_Target.near + 0.01f, EditorGUILayout.FloatField("Far", m_Target.far));
        m_Target.orthographic = EditorGUILayout.Toggle("Orthographic", m_Target.orthographic);
        if (m_Target.orthographic)
        {
            m_Target.orthographicSize = Mathf.Max(0.01f,
                EditorGUILayout.FloatField("OrthographicSize", m_Target.orthographicSize));
        }
        else
        {
            m_Target.fieldOfView = Mathf.Clamp(EditorGUILayout.FloatField("FieldOfView", m_Target.fieldOfView), 0.01f,
                179.99f);

        }
        m_Target.aspect = Mathf.Max(0.01f, EditorGUILayout.FloatField("Aspect", m_Target.aspect));
        m_Target.material =
            EditorGUILayout.ObjectField("Material", m_Target.material, typeof (Material), false) as Material;

        EditorGUI.BeginChangeCheck();
        m_Index = EditorGUILayout.Popup("OcTree", m_Index, m_Trees);
        if (EditorGUI.EndChangeCheck())
        {
            m_Target.ocTreeName = m_Trees[m_Index];
        }
    }
}