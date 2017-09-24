using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace OcTreeProjector
{
    public class OTRealMesh
    {
        public readonly List<Vector3> vertexCache = new List<Vector3>();
        public readonly List<Vector2> uvCache = new List<Vector2>();
        public readonly List<int> indexesCache = new List<int>();
        public int currentIndex;
        private Mesh m_Mesh = new Mesh();
        public bool rebuildMesh = false;

        public Mesh GetMesh()
        {
            if (rebuildMesh)
            {
                m_Mesh.Clear();
                m_Mesh.SetVertices(vertexCache);
                m_Mesh.SetUVs(0, uvCache);
                m_Mesh.SetTriangles(indexesCache, 0);
                rebuildMesh = false;
            }
            return m_Mesh;
        }
    }
    public class OTMesh
    {
        public Bounds bounds;
        public Matrix4x4 projection;

        private MeshOcTree m_MeshOcTree;

        public OTRealMesh mesh;

        private MeshOcTreeTriggerHandle m_TriangleHandle;

        private bool m_RefreshCache = false;
        

        public OTMesh(MeshOcTree ocTree, OTRealMesh mesh)
        {
            this.m_MeshOcTree = ocTree;
            this.mesh = mesh;
            m_TriangleHandle = SearchTriangle;
        }

        public void RefreshMatrix(Matrix4x4 projection)
        {
            this.projection = projection;
            Matrix4x4 inv = projection.inverse;
            Vector3 p1 = new Vector3(-1, -1, -1);
            Vector3 p2 = new Vector3(-1, 1, -1);
            Vector3 p3 = new Vector3(1, 1, -1);
            Vector3 p4 = new Vector3(1, -1, -1);
            Vector3 p5 = new Vector3(-1, -1, 1);
            Vector3 p6 = new Vector3(-1, 1, 1);
            Vector3 p7 = new Vector3(1, 1, 1);
            Vector3 p8 = new Vector3(1, -1, 1);
            p1 = inv.MultiplyPoint(p1);
            p2 = inv.MultiplyPoint(p2);
            p3 = inv.MultiplyPoint(p3);
            p4 = inv.MultiplyPoint(p4);
            p5 = inv.MultiplyPoint(p5);
            p6 = inv.MultiplyPoint(p6);
            p7 = inv.MultiplyPoint(p7);
            p8 = inv.MultiplyPoint(p8);

            Vector3 min = GetMinVector(p1, p2);
            min = GetMinVector(min, p3);
            min = GetMinVector(min, p4);
            min = GetMinVector(min, p5);
            min = GetMinVector(min, p6);
            min = GetMinVector(min, p7);
            min = GetMinVector(min, p8);
            Vector3 max = GetMaxVector(p1, p2);
            max = GetMaxVector(max, p3);
            max = GetMaxVector(max, p4);
            max = GetMaxVector(max, p5);
            max = GetMaxVector(max, p6);
            max = GetMaxVector(max, p7);
            max = GetMaxVector(max, p8);

            Vector3 si = max - min;
            Vector3 ct = min + si / 2;

            bounds = new Bounds(ct, si);

            m_RefreshCache = true;
        }

        public void TriggerTest()
        {
            if (m_RefreshCache)
            {
                m_RefreshCache = false;

                m_MeshOcTree.Trigger(bounds, m_TriangleHandle);
                mesh.rebuildMesh = true;
                //m_RebuildMesh = true;
            }
        }

        private void SearchTriangle(OTMeshTriangle triangle)
        {
            Vector3 pj1, pj2, pj3;
            pj1 = projection.MultiplyPoint(triangle.vertex0);
            pj2 = projection.MultiplyPoint(triangle.vertex1);
            pj3 = projection.MultiplyPoint(triangle.vertex2);
            if (IsOutOfBounds(pj1) && IsOutOfBounds(pj2) && IsOutOfBounds(pj3))
                return;

            mesh.vertexCache.Add(triangle.vertex0);
            mesh.uvCache.Add(new Vector2(pj1.x*0.5f+0.5f, pj1.y*0.5f+0.5f));
            mesh.indexesCache.Add(0 + mesh.currentIndex * 3);

            mesh.vertexCache.Add(triangle.vertex1);
            mesh.uvCache.Add(new Vector2(pj2.x*0.5f+0.5f, pj2.y*0.5f+0.5f));
            mesh.indexesCache.Add(1 + mesh.currentIndex * 3);

            mesh.vertexCache.Add(triangle.vertex2);
            mesh.uvCache.Add(new Vector2(pj3.x*0.5f+0.5f, pj3.y*0.5f+0.5f));
            mesh.indexesCache.Add(2 + mesh.currentIndex * 3);

            mesh.currentIndex += 1;
        }

        public void Render(Material material)
        {
            material.SetPass(0);
            Graphics.DrawMeshNow(mesh.GetMesh(), Matrix4x4.identity);
        }

        private bool IsOutOfBounds(Vector3 pjpos)
        {
            if (pjpos.x < -1 || pjpos.x > 1 || pjpos.y < -1 || pjpos.y > 1 || pjpos.z < -1 || pjpos.z > 1)
                return true;
            return false;
        }

        private Vector3 GetMaxVector(Vector3 v1, Vector3 v2)
        {
            Vector3 p = v1;
            if (v2.x > p.x)
                p.x = v2.x;
            if (v2.y > p.y)
                p.y = v2.y;
            if (v2.z > p.z)
                p.z = v2.z;
            return p;
        }

        private Vector3 GetMinVector(Vector3 v1, Vector3 v2)
        {
            Vector3 p = v1;
            if (v2.x < p.x)
                p.x = v2.x;
            if (v2.y < p.y)
                p.y = v2.y;
            if (v2.z < p.z)
                p.z = v2.z;
            return p;
        }
    }
}