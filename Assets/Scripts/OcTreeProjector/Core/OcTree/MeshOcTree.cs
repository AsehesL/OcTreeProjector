using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace OcTreeProjector
{
    internal delegate void MeshOcTreeTriggerHandle(OTMeshTriangle triangle);

    /// <summary>
    /// 八叉树
    /// </summary>
    internal class MeshOcTree : ScriptableObject
    {
        public int count { get { return m_Count; } }

        /// <summary>
        /// 最大深度
        /// </summary>
        public int maxDepth { get { return m_MaxDepth; } }

        [SerializeField] private int m_Count;

        [SerializeField] private int m_MaxDepth;

        /// <summary>
        /// 节点列表
        /// </summary>
        [SerializeField]
        private List<MeshOcTreeNode> m_NodeLists;

        /// <summary>
        /// 数据列表
        /// </summary>
        [SerializeField]
        private List<OTMeshTriangle> m_DataList;
        /// <summary>
        /// 数据所在节点索引
        /// </summary>
        [SerializeField]
        private List<int> m_NodeIndexList;
    }
}