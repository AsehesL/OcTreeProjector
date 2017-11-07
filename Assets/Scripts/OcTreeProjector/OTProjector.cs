using UnityEngine;
using System.Threading;
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

    public string ocTreeName;

    public bool debug;

    private Vector3 m_Position;
    private Quaternion m_Rotation;
    private float m_OrthographicSize;
    private float m_Near;
    private float m_Far;
    private float m_Aspect;
    private bool m_Orthographic;
    private float m_FieldOfView;

    private string m_OcTreeName;

    private bool m_IsInitialized;

    private MeshOcTree m_OcTree;

    private Bounds m_Bounds;

    private MeshOcTreeTriggerHandle m_Handle;

    private OTMesh m_Mesh;

    private WaitCallback m_BuildMeshCallBack;

    private Matrix4x4 m_WorldToProjector;

    void Start()
    {
        if (string.IsNullOrEmpty(ocTreeName))
        {
            return;
        }
        m_OcTree = Resources.Load<MeshOcTree>(ocTreeName);
        if (m_OcTree == null)
            return;
        m_OcTreeName = ocTreeName;

        m_Handle = OcTreeTriggerHandle;
        m_BuildMeshCallBack = BuildProjectorMesh;

        m_Mesh = new OTMesh();

        m_IsInitialized = true;
    }

    void OnDestroy()
    {
        if (m_Mesh != null)
            m_Mesh.Release();
        m_Mesh = null;
        m_OcTree = null;
        m_Handle = null;
        m_BuildMeshCallBack = null;
    }

    void Update()
    {
        if (!m_IsInitialized)
            return;
        m_Mesh.RefreshMesh();
        m_Mesh.DrawMesh(material, gameObject.layer);
    }


    void LateUpdate()
    {
        bool rebuildMesh = false;

        if (m_OcTreeName != ocTreeName)
        {
            m_OcTreeName = ocTreeName;
            if (!ReLoadOcTree())
            {
                m_IsInitialized = false;
                return;
            }
            else
            {
                m_IsInitialized = true;
            }
            rebuildMesh = true;
        }

        if (!m_IsInitialized)
            return;
       
        if (m_Orthographic != orthographic)
        {
            m_Orthographic = orthographic;
            ReBuildBounds();
            rebuildMesh = true;
        }
        else
        {
            if (m_Orthographic &&
                (m_Position != transform.position || m_Rotation != transform.rotation ||
                 m_OrthographicSize != orthographicSize || m_Near != near ||
                 m_Far != far || m_Aspect != aspect))
            {
                ReBuildBounds();
                rebuildMesh = true;
            }
            else if (!m_Orthographic &&
                     (m_Position != transform.position || m_Rotation != transform.rotation ||
                      m_FieldOfView != fieldOfView || m_Near != near ||
                      m_Far != far || m_Aspect != aspect))
            {
                ReBuildBounds();
                rebuildMesh = true;
            }
        }
        if (rebuildMesh)
        {
            m_Mesh.SetMatrix(m_WorldToProjector, m_Bounds);
            ThreadPool.QueueUserWorkItem(m_BuildMeshCallBack, m_Mesh);
        }


    }

    bool ReLoadOcTree()
    {
        if (string.IsNullOrEmpty(m_OcTreeName))
            return false;
        m_OcTree = Resources.Load<MeshOcTree>(m_OcTreeName);
        if (m_OcTree == null)
            return false;
        return true;
    }

    void ReBuildBounds()
    {
        m_Aspect = aspect;
        m_Near = near;
        m_Far = far;
        m_OrthographicSize = orthographicSize;
        m_Rotation = transform.rotation;
        m_Position = transform.position;
        m_FieldOfView = fieldOfView;
        m_Orthographic = orthographic;

        Matrix4x4 proj = default(Matrix4x4);
        if (m_Orthographic)
        {
            proj = Matrix4x4.Ortho(-m_Aspect*m_OrthographicSize, m_Aspect*m_OrthographicSize,
                -m_OrthographicSize, m_OrthographicSize, m_Near, m_Far);
            m_WorldToProjector = proj*transform.worldToLocalMatrix;
        }
        else
        {
            proj = Matrix4x4.Perspective(m_FieldOfView, m_Aspect, m_Near, m_Far);
            m_WorldToProjector = proj * transform.worldToLocalMatrix;
        }

        if (m_Orthographic)
            m_Bounds = OTProjectorUtils.OrthoBounds(transform.position, transform.rotation, m_OrthographicSize, m_Aspect,
                m_Near, m_Far);
        else
            m_Bounds = OTProjectorUtils.PerspectiveBounds(transform.position, transform.rotation, m_FieldOfView,
                m_Aspect, m_Near, m_Far);

    }

    void BuildProjectorMesh(object state)
    {
        if (state == null)
            return;
        if (m_OcTree == null)
            return;
        OTMesh mesh = (OTMesh) state;
        mesh.PreBuildMesh();

        m_OcTree.Trigger(mesh.bounds, mesh, m_Handle);
        mesh.PostBuildMesh();

    }

    void OcTreeTriggerHandle(OTMesh mesh, OcTreeProjector.OTMeshTriangle triangle)
    {
        mesh.AddTriangle(triangle);
    }

    void OnDrawGizmosSelected()
    {
        this.DrawProjectorGizmos();
        if (debug)
        {
            this.m_Bounds.DrawBounds(Color.black);
            if (this.m_OcTree)
                m_OcTree.DrawTree(0, 0.1f);
        }
    }
}