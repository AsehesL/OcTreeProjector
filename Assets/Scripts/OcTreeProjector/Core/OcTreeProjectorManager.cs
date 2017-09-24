using UnityEngine;
using System.Threading;
using System.Collections;
using System.Collections.Generic;

namespace OcTreeProjector
{
    /// <summary>
    /// OcTreeProjector管理器
    /// </summary>
    internal class OcTreeProjectorManager : MonoBehaviour
    {
        private static OcTreeProjectorManager Instance
        {
            get
            {
                if (instance == null)
                    instance = FindObjectOfType<OcTreeProjectorManager>();
                if (instance == null)
                {
                    instance = new GameObject("[OTProjectorManager]").AddComponent<OcTreeProjectorManager>();
                    //instance.gameObject.hideFlags = HideFlags.HideInHierarchy;
                }
                return instance;
            }
        }

        private static OcTreeProjectorManager instance;

        private List<OTMesh> m_MeshList = new List<OTMesh>();

        private Dictionary<MeshOcTree, OTRealMesh> m_Meshes = new Dictionary<MeshOcTree, OTRealMesh>();

        private Thread m_Thread;

        void Awake()
        {
            m_Thread = new Thread(Refresh);
            m_Thread.Start();
        }

        void OnDestroy()
        {
            m_Thread.Abort();
        }

        public static OTMesh RegisterMesh(MeshOcTree ocTree)
        {
            if (Instance == null)
                return null;
            if (ocTree == null)
                return null;
            OTRealMesh mesh = null;
            if (Instance.m_Meshes.ContainsKey(ocTree))
                mesh = Instance.m_Meshes[ocTree];
            else
            {
                mesh = new OTRealMesh();
                Instance.m_Meshes.Add(ocTree, mesh);
            }
            lock (Instance.m_MeshList)
            {
                OTMesh otmesh = new OTMesh(ocTree, mesh);
                if (!Instance.m_MeshList.Contains(otmesh))
                    Instance.m_MeshList.Add(otmesh);
                return otmesh;
            }
        }

        public static void UnregisterMesh(OTMesh mesh)
        {
            if (instance == null)
                return;
            lock (instance.m_MeshList)
            {
                instance.m_MeshList.Remove(mesh);
            }
        }

        private void Refresh()
        {
            while (true)
            {
                lock (m_MeshList)
                {
                    for (int i = 0; i < m_MeshList.Count; i++)
                    {
                        lock (m_MeshList[i])
                        {
                            m_MeshList[i].mesh.currentIndex = 0;
                            m_MeshList[i].TriggerTest();
                        }
                    }
                }
                Thread.Sleep(10);
            }
        }


//        public MeshOcTree OctTree { get { return m_OctTree; } }
//
//        private MeshOcTree m_OctTree;

//        void Awake()
//        {
//            m_OctTree = Resources.Load<MeshOcTree>("tree");
//        }
//
//        void OnDrawGizmosSelected()
//        {
//            if (m_OctTree != null)
//            {
//                m_OctTree.DrawTree(0, 0.1f);
//            }
//        }
    }
}