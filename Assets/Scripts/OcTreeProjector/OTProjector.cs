using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using OcTreeProjector;

/// <summary>
/// 八叉树投影器
/// </summary>
public class OTProjector : MonoBehaviour
{

    public float size;
    public float near;
    public float far;
    public float aspect;

    public Material material;

    private Bounds m_Bounds;

    private Vector3 m_Position;
    private Quaternion m_Rotation;
    private float m_Size;
    private float m_Near;
    private float m_Far;
    private float m_Aspect;

    private Mesh m_Mesh;

    private OTMesh m_MeshCache = new OTMesh();

    private MeshOcTreeTriggerHandle m_TriangleHandle;

    private Thread m_Thread;

    private bool m_RefreshCache;
    private bool m_RebuildMesh;

    void Start()
    {
        m_TriangleHandle = new MeshOcTreeTriggerHandle(SearchTriangle);
        m_Thread = new Thread(RefreshCache);
        m_Thread.Start();
    }

    void OnDestroy()
    {
        m_Thread.Abort();
    }

    void OnRenderObject()
    {
        if (material != null && m_Mesh != null)
        {
            material.SetPass(0);
            material.SetMatrix("internal_Projector", GetProjectorMatrix());
            Graphics.DrawMeshNow(m_Mesh, Matrix4x4.identity);
        }
    }

    void RefreshCache()
    {
        while (true)
        {
            if (m_RefreshCache)
            {
                m_RefreshCache = false;

                lock (m_MeshCache)
                {
                    m_MeshCache.vertexCache.Clear();
                    m_MeshCache.indexesCache.Clear();

                    m_MeshCache.currentIndex = 0;

                    OcTreeProjectorManager.Instance.OctTree.Trigger(m_Bounds, m_TriangleHandle);
                }

                m_RebuildMesh = true;
            }
            Thread.Sleep(10);
        }
    }

    void Update()
    {
        if (OcTreeProjectorManager.Instance == null)
            return;
        if (m_Position != transform.position || m_Rotation != transform.rotation || m_Size != size || m_Near != near ||
            m_Far != far || m_Aspect != aspect)
        {
            BuildBounds();

            m_RefreshCache = true;
        }

        if (m_RebuildMesh)
        {
            m_RebuildMesh = false;
            lock (m_MeshCache)
            {
                if (m_Mesh == null)
                    m_Mesh = new Mesh();
                m_Mesh.Clear();
                m_Mesh.SetVertices(m_MeshCache.vertexCache);
                m_Mesh.SetTriangles(m_MeshCache.indexesCache, 0);
            }
        }
    }

    void SearchTriangle(OTMeshTriangle triangle)
    {
        m_MeshCache.vertexCache.Add(triangle.vertex0);
        m_MeshCache.indexesCache.Add(0 + m_MeshCache.currentIndex * 3);

        m_MeshCache.vertexCache.Add(triangle.vertex1);
        m_MeshCache.indexesCache.Add(1 + m_MeshCache.currentIndex * 3);

        m_MeshCache.vertexCache.Add(triangle.vertex2);
        m_MeshCache.indexesCache.Add(2 + m_MeshCache.currentIndex * 3);

        m_MeshCache.currentIndex += 1;
    }

    void BuildBounds()
    {
        m_Aspect = aspect;
        m_Near = near;
        m_Far = far;
        m_Size = size;
        m_Position = transform.position;
        m_Rotation = transform.rotation;

        Vector3 p1 = m_Position + m_Rotation * new Vector3(-m_Size * m_Aspect, -m_Size, m_Near);
        Vector3 p2 = m_Position + m_Rotation * new Vector3(m_Size * m_Aspect, -m_Size, m_Near);
        Vector3 p3 = m_Position + m_Rotation * new Vector3(m_Size * m_Aspect, m_Size, m_Near);
        Vector3 p4 = m_Position + m_Rotation * new Vector3(-m_Size * m_Aspect, m_Size, m_Near);
        Vector3 p5 = m_Position + m_Rotation * new Vector3(-m_Size * m_Aspect, -m_Size, m_Far);
        Vector3 p6 = m_Position + m_Rotation * new Vector3(m_Size * m_Aspect, -m_Size, m_Far);
        Vector3 p7 = m_Position + m_Rotation * new Vector3(m_Size * m_Aspect, m_Size, m_Far);
        Vector3 p8 = m_Position + m_Rotation * new Vector3(-m_Size * m_Aspect, m_Size, m_Far);

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

        m_Bounds = new Bounds(ct, si);
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

    private Matrix4x4 GetProjectorMatrix()
    {
        Matrix4x4 mat = transform.worldToLocalMatrix;

        Matrix4x4 pj = new Matrix4x4();
        pj.m00 = 0.5f / (size * aspect);
        pj.m03 = 0.5f;
        pj.m11 = 0.5f / size;
        pj.m13 = 0.5f;
        pj.m22 = 2 / (far - near);
        pj.m23 = -2 * near / (far - near) - 1;
        pj.m33 = 1;

        return pj * mat;
    }

    void OnDrawGizmosSelected()
    {
        m_Bounds.DrawBounds(Color.black);
        Quaternion rot = transform.rotation;
        Vector3 p1 = transform.position + rot * new Vector3(-size * aspect, -size, near);
        Vector3 p2 = transform.position + rot * new Vector3(size * aspect, -size, near);
        Vector3 p3 = transform.position + rot * new Vector3(size * aspect, size, near);
        Vector3 p4 = transform.position + rot * new Vector3(-size * aspect, size, near);
        Vector3 p5 = transform.position + rot * new Vector3(-size * aspect, -size, far);
        Vector3 p6 = transform.position + rot * new Vector3(size * aspect, -size, far);
        Vector3 p7 = transform.position + rot * new Vector3(size * aspect, size, far);
        Vector3 p8 = transform.position + rot * new Vector3(-size * aspect, size, far);

        Gizmos.color = new Color(0.8f, 0.8f, 0.8f, 0.6f);

        Gizmos.DrawLine(p1, p2);
        Gizmos.DrawLine(p2, p3);
        Gizmos.DrawLine(p3, p4);
        Gizmos.DrawLine(p4, p1);

        Gizmos.DrawLine(p5, p6);
        Gizmos.DrawLine(p6, p7);
        Gizmos.DrawLine(p7, p8);
        Gizmos.DrawLine(p8, p5);

        Gizmos.DrawLine(p1, p5);
        Gizmos.DrawLine(p2, p6);
        Gizmos.DrawLine(p3, p7);
        Gizmos.DrawLine(p4, p8);
    }
}
