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

    private MaterialPropertyBlock m_PropertyBlock;

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

        m_PropertyBlock = new MaterialPropertyBlock();
        m_MeshRenderer.SetPropertyBlock(m_PropertyBlock);

        m_Mesh = new OTMesh();
        m_MeshFilter.sharedMesh = m_Mesh.mesh;

        m_IsInitialized = true;
    }

    void OnDestroy()
    {
        if (m_Mesh != null)
            m_Mesh.Release();
        if (m_MeshRenderer)
            Destroy(m_MeshRenderer.gameObject);
        m_Mesh = null;
        m_OcTree = null;
    }

    void OnEnable()
    {
        if (m_MeshRenderer)
            m_MeshRenderer.gameObject.SetActive(true);
    }

    void OnDisable()
    {
        if (m_MeshRenderer)
            m_MeshRenderer.gameObject.SetActive(false);
    }

    void Update()
    {
        if (m_Mesh.BuildMesh())
        {
        }
        Matrix4x4 mat = Matrix4x4.Ortho(-m_OrthographicSize * m_Aspect, m_OrthographicSize * m_Aspect,
           -m_OrthographicSize, m_OrthographicSize, m_Near, m_Far);
        //m_Mesh.DrawMesh(material, mat * transform.worldToLocalMatrix, gameObject.layer);
        m_PropertyBlock.SetMatrix("internal_Projector", mat * transform.worldToLocalMatrix);
        m_MeshRenderer.SetPropertyBlock(m_PropertyBlock);
    }


    void LateUpdate()
    {
        if (!m_IsInitialized)
            return;
        
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
             //m_Mesh.localToProjector = Matrix4x4.Ortho(-m_OrthographicSize * m_Aspect, m_OrthographicSize * m_Aspect,
            //-m_OrthographicSize, m_OrthographicSize, m_Near, m_Far);
                //m_Mesh.worldToLocal = transform.worldToLocalMatrix;
            }
            ThreadPool.QueueUserWorkItem(m_BuildMeshCallBack, m_Mesh);
            //BuildProjectorMesh(m_Mesh);
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
        
    }

    void BuildProjectorMesh(object state)
    {
        if (state == null)
            return;
        if (m_OcTree == null)
            return;
        OTMesh mesh = (OTMesh) state;
        mesh.PreBuildMesh();
        //lock (m_Mesh)
        //{
        //    mesh.PreBuildMesh();
            m_OcTree.Trigger(m_Bounds, mesh, m_Handle);
            mesh.PostBuildMesh();
        //}

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
