using UnityEngine;
using System.Collections.Generic;

namespace OcTreeProjector
{
    public class OTMesh
    {
        public Mesh mesh { get { return m_Mesh; } }

        public Bounds bounds { get { return m_Bounds; } }

        public List<Vector3> m_VertexList;
        public List<Vector2> m_UVList;

        public List<int> m_Indexes;

        public Mesh m_Mesh;

        private int m_Index;
        
        private Matrix4x4 m_WorldToProjector;

        private Bounds m_Bounds;

        private volatile bool m_IsMeshRebuilt;

        private volatile bool m_IsUpdatedMatrix;
        

        public OTMesh()
        {
            m_VertexList = new List<Vector3>();
            m_Indexes = new List<int>();
            m_UVList = new List<Vector2>();
            m_Mesh = new Mesh();
            m_Mesh.MarkDynamic();
        }

        public void SetMatrix(Matrix4x4 matrix, Bounds bounds)
        {
            if (m_IsUpdatedMatrix)
                return;
            m_IsUpdatedMatrix = true;
            m_WorldToProjector = matrix;
            m_Bounds = bounds;
        }

        public void PreBuildMesh()
        {
            m_Index = 0;
            m_VertexList.Clear();
            m_UVList.Clear();
            m_Indexes.Clear();
        }

        public void PostBuildMesh()
        {
            m_IsMeshRebuilt = true;
            m_IsUpdatedMatrix = false;
        }

        public void RefreshMesh()
        {
            if (m_IsMeshRebuilt)
            {
                m_Mesh.Clear();
                m_Mesh.SetVertices(m_VertexList);
                m_Mesh.SetUVs(0, m_UVList);
                m_Mesh.SetTriangles(m_Indexes, 0);
                m_IsMeshRebuilt = false;
            }
        }

        public void DrawMesh(Material material, LayerMask layer)
        {
            Graphics.DrawMesh(m_Mesh, Matrix4x4.identity, material, layer);
        }

        public void AddTriangle(OTMeshTriangle triangle)
        {
            m_VertexList.Add(triangle.vertex0);
            m_VertexList.Add(triangle.vertex1);
            m_VertexList.Add(triangle.vertex2);

            Vector3 pj0 = m_WorldToProjector.MultiplyPoint(triangle.vertex0);
            Vector3 pj1 = m_WorldToProjector.MultiplyPoint(triangle.vertex1);
            Vector3 pj2 = m_WorldToProjector.MultiplyPoint(triangle.vertex2);

            m_UVList.Add(new Vector2(pj0.x*0.5f+0.5f, pj0.y*0.5f+0.5f));
            m_UVList.Add(new Vector2(pj1.x*0.5f+0.5f, pj1.y*0.5f+0.5f));
            m_UVList.Add(new Vector2(pj2.x*0.5f+0.5f, pj2.y*0.5f+0.5f));

            m_Indexes.Add(m_Index + 0);
            m_Indexes.Add(m_Index + 1);
            m_Indexes.Add(m_Index + 2);
            m_Index += 3;
        }

        public void Release()
        {
            if (m_Mesh)
                Object.Destroy(m_Mesh);
        }
    }
}