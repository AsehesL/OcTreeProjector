using UnityEngine;
using System.Collections;

namespace OcTreeProjector
{
    /// <summary>
    /// 投影Mesh三角类
    /// </summary>
    [System.Serializable]
    public class OTMeshTriangle
    {
        public Vector3 vertex0;
        public Vector3 vertex1;
        public Vector3 vertex2;

        /// <summary>
        /// 三角形的AABB包围盒
        /// </summary>
        public Bounds bounds { get { return m_Bounds; } }

        [SerializeField] private Bounds m_Bounds;

        public OTMeshTriangle(Vector3 vertex0, Vector3 vertex1, Vector3 vertex2)
        {
            this.vertex0 = vertex0;
            this.vertex1 = vertex1;
            this.vertex2 = vertex2;

            float maxX = Mathf.Max(vertex0.x, vertex1.x, vertex2.x);
            float maxY = Mathf.Max(vertex0.y, vertex1.y, vertex2.y);
            float maxZ = Mathf.Max(vertex0.z, vertex1.z, vertex2.z);

            float minX = Mathf.Min(vertex0.x, vertex1.x, vertex2.x);
            float minY = Mathf.Min(vertex0.y, vertex1.y, vertex2.y);
            float minZ = Mathf.Min(vertex0.z, vertex1.z, vertex2.z);

            Vector3 si = new Vector3(maxX - minX, maxY - minY, maxZ - minZ);
            if (si.x <= 0)
                si.x = 0.1f;
            if (si.y <= 0)
                si.y = 0.1f;
            if (si.z <= 0)
                si.z = 0.1f;
            Vector3 ct = new Vector3(minX, minY, minZ) + si / 2;

            this.m_Bounds = new Bounds(ct, si);
        }

        public bool Contains(Vector3 position)
        {
            return bounds.Contains(position);
        }

        public bool Intersects(Bounds bounds)
        {
            return this.bounds.Intersects(bounds);
        }

        public void DrawArea()
        {
            bounds.DrawBounds(Color.black);

            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(vertex0, vertex1);
            Gizmos.DrawLine(vertex0, vertex2);
            Gizmos.DrawLine(vertex1, vertex2);
        }
    }
}