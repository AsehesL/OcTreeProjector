using UnityEngine;
using System.Collections.Generic;

namespace OcTreeProjector
{
    public class OTMesh
    {
        public Mesh mesh { get { return m_Mesh; } }

        public Matrix4x4 worldToLocal;

        public List<Vector3> m_VertexList;

        public List<int> m_Indexes;

        public Mesh m_Mesh;

        private int m_Index;

        private volatile bool m_IsMeshRebuilt;
        

        public OTMesh()
        {
            m_VertexList = new List<Vector3>();
            m_Indexes = new List<int>();
            m_Mesh = new Mesh();
            m_Mesh.MarkDynamic();
        }

        public void PreBuildMesh()
        {
            m_Index = 0;
            m_VertexList.Clear();
            m_Indexes.Clear();
        }

        public void PostBuildMesh()
        {
            m_IsMeshRebuilt = true;
        }

        public bool BuildMesh()
        {
            if (m_IsMeshRebuilt)
            {
                m_Mesh.Clear();
                m_Mesh.SetVertices(m_VertexList);
                m_Mesh.SetTriangles(m_Indexes, 0);
                m_IsMeshRebuilt = false;
                return true;
            }
            return false;
        }

        public void AddTriangle(OTMeshTriangle triangle)
        {
            m_VertexList.Add(worldToLocal.MultiplyPoint(triangle.vertex0));
            m_VertexList.Add(worldToLocal.MultiplyPoint(triangle.vertex1));
            m_VertexList.Add(worldToLocal.MultiplyPoint(triangle.vertex2));
            m_Indexes.Add(m_Index + 0);
            m_Indexes.Add(m_Index + 1);
            m_Indexes.Add(m_Index + 2);
            m_Index += 3;
        }
    }
}