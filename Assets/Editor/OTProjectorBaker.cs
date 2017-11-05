using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using OcTreeProjector;

namespace OcTreeProjector
{
    internal class OTProjectorBaker
    {

        [MenuItem("Test/CreateTree")]
        static void CreateTree()
        {
            if (Selection.activeGameObject == null)
                return;
            MeshFilter[] mf = Selection.activeGameObject.GetComponentsInChildren<MeshFilter>();

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
            MeshOcTree tree = MeshOcTree.CreateInstance<MeshOcTree>();
            tree.Build(center, size*1.1f, 5);
            for (int i = 0; i < triangles.Count; i++)
            {
                tree.Add(triangles[i]);
            }
            AssetDatabase.CreateAsset(tree, "Assets/Resources/tree.asset");
        }
    }
}