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

    private MeshRenderer m_MeshRenderer;
    private MeshFilter m_MeshFilter;

    private OTMesh m_Mesh;

    private WaitCallback m_BuildMeshCallBack;

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

        m_MeshRenderer = new GameObject("MR").AddComponent<MeshRenderer>();
        m_MeshFilter = m_MeshRenderer.gameObject.AddComponent<MeshFilter>();
        //m_MeshRenderer.transform.SetParent(transform);
        m_MeshRenderer.transform.position = Vector3.zero;
        m_MeshRenderer.transform.rotation = Quaternion.identity;
        m_MeshRenderer.sharedMaterial = material;

        m_Mesh = new OTMesh();
        m_MeshFilter.sharedMesh = m_Mesh.mesh;

        m_IsInitialized = true;
    }

    void OnDestroy()
    {
        if (m_MeshFilter)
            Destroy(m_MeshFilter.gameObject);
    }

    void OnDisable()
    {
        if (m_MeshFilter)
            m_MeshFilter.gameObject.SetActive(false);
    }

    void OnEnable()
    {
        if (m_MeshFilter)
            m_MeshFilter.gameObject.SetActive(true);
    }

    void LateUpdate()
    {
        if (!m_IsInitialized)
            return;
        {
            if (m_Mesh.BuildMesh())
            {
                //m_MeshFilter.sharedMesh = m_Mesh.mesh;
            }
        }
        bool rebuildMesh = false;

        if (m_OcTreeName != ocTreeName)
        {
            m_OcTreeName = ocTreeName;
            ReLoadOcTree();
            rebuildMesh = true;
        }
        if (m_Orthographic != orthographic)
        {
            m_Orthographic = orthographic;
            ReBuildBounds();
            rebuildMesh = true;
        }
        else
        {
            if (m_Orthographic && (m_Position != transform.position||m_Rotation != transform.rotation || m_OrthographicSize != orthographicSize || m_Near != near ||
                 m_Far != far || m_Aspect != aspect))
            {
                ReBuildBounds();
                rebuildMesh = true;
            }else if (!m_Orthographic && (m_Position != transform.position||m_Rotation != transform.rotation || m_FieldOfView != fieldOfView || m_Near != near ||
                       m_Far != far || m_Aspect != aspect))
            {
                ReBuildBounds();
                rebuildMesh = true;
            }
        }
        if (rebuildMesh)
        {
            //lock (m_Mesh)
            {
                m_Mesh.worldToLocal = m_MeshRenderer.transform.worldToLocalMatrix;
            }
            ThreadPool.QueueUserWorkItem(m_BuildMeshCallBack, m_Mesh);
            //m_OcTree.Trigger(m_Bounds, )
        }
        //lock (m_Mesh)
        
    }

    void ReLoadOcTree()
    {
        
    }

    void ReBuildBounds()
    {
        m_Aspect = aspect;
        m_Near = near;
        m_Far = far;
        m_OrthographicSize = orthographicSize;
        m_Rotation = transform.rotation;
        m_Position = transform.position;

        Vector3 p1 = m_Position + m_Rotation * new Vector3(-m_OrthographicSize * m_Aspect, -m_OrthographicSize, m_Near);
        Vector3 p2 = m_Position + m_Rotation * new Vector3(m_OrthographicSize * m_Aspect, -m_OrthographicSize, m_Near);
        Vector3 p3 = m_Position + m_Rotation * new Vector3(m_OrthographicSize * m_Aspect, m_OrthographicSize, m_Near);
        Vector3 p4 = m_Position + m_Rotation * new Vector3(-m_OrthographicSize * m_Aspect, m_OrthographicSize, m_Near);
        Vector3 p5 = m_Position + m_Rotation * new Vector3(-m_OrthographicSize * m_Aspect, -m_OrthographicSize, m_Far);
        Vector3 p6 = m_Position + m_Rotation * new Vector3(m_OrthographicSize * m_Aspect, -m_OrthographicSize, m_Far);
        Vector3 p7 = m_Position + m_Rotation * new Vector3(m_OrthographicSize * m_Aspect, m_OrthographicSize, m_Far);
        Vector3 p8 = m_Position + m_Rotation * new Vector3(-m_OrthographicSize * m_Aspect, m_OrthographicSize, m_Far);

        Vector3 min = OTProjectorUtils.GetMinVector(p1, p2);
        min = OTProjectorUtils.GetMinVector(min, p3);
        min = OTProjectorUtils.GetMinVector(min, p4);
        min = OTProjectorUtils.GetMinVector(min, p5);
        min = OTProjectorUtils.GetMinVector(min, p6);
        min = OTProjectorUtils.GetMinVector(min, p7);
        min = OTProjectorUtils.GetMinVector(min, p8);
        Vector3 max = OTProjectorUtils.GetMaxVector(p1, p2);
        max = OTProjectorUtils.GetMaxVector(max, p3);
        max = OTProjectorUtils.GetMaxVector(max, p4);
        max = OTProjectorUtils.GetMaxVector(max, p5);
        max = OTProjectorUtils.GetMaxVector(max, p6);
        max = OTProjectorUtils.GetMaxVector(max, p7);
        max = OTProjectorUtils.GetMaxVector(max, p8);

        Vector3 si = max - min;
        Vector3 ct = min + si / 2;

        m_Bounds = new Bounds(ct, si);

        Matrix4x4 pjMatrix = Matrix4x4.Ortho(-m_OrthographicSize*m_Aspect, m_OrthographicSize*m_Aspect,
            -m_OrthographicSize, m_OrthographicSize, m_Near, m_Far);

        material.SetMatrix("internal_Projector", pjMatrix);
    }

    void BuildProjectorMesh(object state)
    {
        if (state == null)
            return;
        OTMesh mesh = (OTMesh) state;
        mesh.PreBuildMesh();
        //lock (m_Mesh)
        {
            mesh.PreBuildMesh();
            m_OcTree.Trigger(m_Bounds, mesh, m_Handle);
            mesh.PostBuildMesh();
        }

    }

    void OcTreeTriggerHandle(OTMesh mesh, OcTreeProjector.OTMeshTriangle triangle)
    {
         mesh.AddTriangle(triangle);
    }

    void OnDrawGizmos()
    {
        this.DrawProjectorGizmos();
    }
}
