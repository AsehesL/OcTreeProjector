using UnityEngine;
using System.Collections;

namespace OcTreeProjector
{
    /// <summary>
    /// OcTreeProjector管理器
    /// </summary>
    internal class OcTreeProjectorManager : MonoBehaviour
    {
        internal static OcTreeProjectorManager Instance
        {
            get
            {
                if (instance == null)
                    instance = FindObjectOfType<OcTreeProjectorManager>();
                if (instance == null)
                {
                    instance = new GameObject("[OTProjectorManager]").AddComponent<OcTreeProjectorManager>();
                    instance.gameObject.hideFlags = HideFlags.HideInHierarchy;
                }
                return instance;
            }
        }

        private static OcTreeProjectorManager instance;

        public MeshOcTree OctTree { get { return m_OctTree; } }

        private MeshOcTree m_OctTree;

        void Awake()
        {
            m_OctTree = Resources.Load<MeshOcTree>("tree");
        }

        void OnDrawGizmosSelected()
        {
            if (m_OctTree != null)
            {
                m_OctTree.DrawTree(0, 0.1f);
            }
        }
    }
}