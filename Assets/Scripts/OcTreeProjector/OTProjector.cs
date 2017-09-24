using UnityEngine;
using System.Collections;
using OcTreeProjector;

public class OTProjector : MonoBehaviour
{
    public float near = 0.1f;
    public float far = 100;
    public float fieldOfView = 60;
    public float aspect = 1;
    public bool orthographic = false;
    public float orthographicSize = 10;
    public Material material;

    public MeshOcTree ocTree;

    private OTMesh m_MeshCache;

    private Vector3 m_Position;
    private Quaternion m_Rotation;
    private float m_OrthographicSize;
    private float m_Near;
    private float m_Far;
    private float m_Aspect;
    private bool m_Orthographic;
    private float m_FieldOfView;

    void Start()
    {
        if (ocTree == null)
            return;
        m_MeshCache = OcTreeProjectorManager.RegisterMesh(ocTree);
    }

    void OnRenderObject()
    {
        if (m_MeshCache == null)
            return;
        bool render = Camera.current == Camera.main;
#if UNITY_EDITOR
        render = render || (UnityEditor.SceneView.currentDrawingSceneView != null &&
                            Camera.current == UnityEditor.SceneView.currentDrawingSceneView.camera);
#endif
        if (render)
        {
            if (material != null)
            {
                lock (m_MeshCache)
                {
                    m_MeshCache.Render(material);
                }
            }
        }
    }

    void Update()
    {
        if (m_MeshCache == null)
            return;
        bool rebuildBounds = false;
        if (m_Orthographic != orthographic)
        {
            m_Orthographic = orthographic;
            rebuildBounds = true;
        }
        if (!rebuildBounds)
        {
            if (m_Position != transform.position || m_Rotation != transform.rotation ||
                     m_Near != near ||
                     m_Far != far || m_Aspect != aspect)
            {
                m_Position = transform.position;
                m_Rotation = transform.rotation;
                m_Near = near;
                m_Far = far;
                m_Aspect = aspect;
                rebuildBounds = true;
            }
        }
        if (!rebuildBounds)
        {
            if (orthographic)
            {
                if (m_OrthographicSize != orthographicSize)
                {
                    m_OrthographicSize = orthographicSize;
                    rebuildBounds = true;
                }
            }
            else
            {
                if (m_FieldOfView != fieldOfView)
                {
                    m_FieldOfView = fieldOfView;
                    rebuildBounds = true;
                }
            }
        }
        if (rebuildBounds)
        {
            Matrix4x4 mtx = default(Matrix4x4);
            if (orthographic)
            {
                mtx = Matrix4x4.Ortho(-orthographicSize * aspect, orthographicSize * aspect, orthographicSize, -orthographicSize,
                    -near, -far);
            }
            else
            {
                mtx = Matrix4x4.Perspective(fieldOfView, aspect, -near, -far);
            }
            mtx = mtx * transform.worldToLocalMatrix;
            lock (m_MeshCache)
            {
                m_MeshCache.RefreshMatrix(mtx);
            }
        }
    }


    void OnDrawGizmos()
    {
        Matrix4x4 mtx = default(Matrix4x4);
        if (orthographic)
        {
            mtx = Matrix4x4.Ortho(-orthographicSize*aspect, orthographicSize*aspect, orthographicSize, -orthographicSize,
                -near, -far).inverse;
        }
        else
        {
            mtx = Matrix4x4.Perspective(fieldOfView, aspect, -near, -far).inverse;
        }
        mtx = transform.localToWorldMatrix * mtx;
        Vector3 p1 = new Vector3(-1, -1, -1);
        Vector3 p2 = new Vector3(-1, 1, -1);
        Vector3 p3 = new Vector3(1, 1, -1);
        Vector3 p4 = new Vector3(1, -1, -1);
        Vector3 p5 = new Vector3(-1, -1, 1);
        Vector3 p6 = new Vector3(-1, 1, 1);
        Vector3 p7 = new Vector3(1, 1, 1);
        Vector3 p8 = new Vector3(1, -1, 1);
        p1 = mtx.MultiplyPoint(p1);
        p2 = mtx.MultiplyPoint(p2);
        p3 = mtx.MultiplyPoint(p3);
        p4 = mtx.MultiplyPoint(p4);
        p5 = mtx.MultiplyPoint(p5);
        p6 = mtx.MultiplyPoint(p6);
        p7 = mtx.MultiplyPoint(p7);
        p8 = mtx.MultiplyPoint(p8);

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
