using UnityEngine;
using System.Collections.Generic;

namespace OcTreeProjector
{
    public class OTMesh
    {
        public Mesh mesh { get { return m_Mesh; } }

        //public Matrix4x4 worldToLocal;
        //public Matrix4x4 localToProjector;

        public List<Vector3> m_VertexList;
        //public List<Vector2> m_UVList;

        public List<int> m_Indexes;

        public Mesh m_Mesh;

        private int m_Index;

        private volatile bool m_IsMeshRebuilt;

        //private MaterialPropertyBlock m_PropertyBlock;
        //private object m_Lock;
        

        public OTMesh()
        {
            m_VertexList = new List<Vector3>();
            m_Indexes = new List<int>();
            //m_UVList = new List<Vector2>();
            m_Mesh = new Mesh();
            m_Mesh.MarkDynamic();
            //m_PropertyBlock = new MaterialPropertyBlock();
            //m_Lock = new object();
        }

        public void PreBuildMesh()
        {
            //lock (m_Lock)
            {
                m_Index = 0;
                m_VertexList.Clear();
                //m_UVList.Clear();
                m_Indexes.Clear();
            }
        }

        public void PostBuildMesh()
        {
            //lock (m_Lock)
            {
                m_IsMeshRebuilt = true;
            }
        }

        public bool BuildMesh()
        {
            //lock (m_Lock)
            {
                if (m_IsMeshRebuilt)
                {
                    m_Mesh.Clear();
                    m_Mesh.SetVertices(m_VertexList);
                    //m_Mesh.SetUVs(0, m_UVList);
                    m_Mesh.SetTriangles(m_Indexes, 0);
                    m_IsMeshRebuilt = false;
                    return true;
                }
            }
            return false;
        }

        //public void DrawMesh(Material material, Matrix4x4 worldToProjector, LayerMask layer)
        //{
        //    //MaterialPropertyBlock bl;bl.SetMatrix()
        //    m_PropertyBlock.SetMatrix("internal_Projector", worldToProjector);
        //    Graphics.DrawMesh(m_Mesh, Matrix4x4.identity, material, layer, Camera.current, 0, m_PropertyBlock);
        //}

        public void AddTriangle(OTMeshTriangle triangle)
        {
            //lock (m_Lock)
            {
                m_VertexList.Add(triangle.vertex0);
                m_VertexList.Add(triangle.vertex1);
                m_VertexList.Add(triangle.vertex2);

                m_Indexes.Add(m_Index + 0);
                m_Indexes.Add(m_Index + 1);
                m_Indexes.Add(m_Index + 2);
                m_Index += 3;
            }
        }

        public void Release()
        {
            if (m_Mesh)
                Object.Destroy(m_Mesh);
        }
    }
}