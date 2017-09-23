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
    }
}