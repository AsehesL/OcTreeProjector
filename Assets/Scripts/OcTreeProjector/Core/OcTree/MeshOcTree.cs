using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace OcTreeProjector
{
    public delegate void MeshOcTreeTriggerHandle(OctProjectorMesh mesh, OTMeshTriangle triangle);

    /// <summary>
    /// 八叉树
    /// </summary>
    public class MeshOcTree : ScriptableObject
    {
        public int count { get { return m_Count; } }

        /// <summary>
        /// 最大深度
        /// </summary>
        public int maxDepth { get { return m_MaxDepth; } }

        /// <summary>
        /// 八叉树包围盒区域
        /// </summary>
        public Bounds Bounds
        {
            get
            {
                if (m_NodeLists != null && m_NodeLists.Count > 0)
                    return m_NodeLists[0].bounds;
                return default(Bounds);
            }
        }

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

        /// <summary>
        /// 构造八叉树
        /// </summary>
        /// <param name="center">八叉树中心坐标</param>
        /// <param name="size">八叉树区域大小</param>
        /// <param name="maxDepth">最大深度</param>
        public void Build(Vector3 center, Vector3 size, int maxDepth)
        {
            this.m_MaxDepth = maxDepth;
            this.m_NodeLists = new List<MeshOcTreeNode>();
            this.m_DataList = new List<OTMeshTriangle>();
            this.m_NodeIndexList = new List<int>();
            this.m_NodeLists.Add(new MeshOcTreeNode(new Bounds(center, size)));
        }

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="item"></param>
        public void Add(OTMeshTriangle item)
        {
            if (m_NodeLists != null && m_NodeLists.Count > 0)
            {
                MeshOcTreeNode node = m_NodeLists[0].Insert(item, 0, maxDepth, m_NodeLists);
                if (node != null)
                {
                    int index = m_NodeLists.IndexOf(node);
                    if (index >= 0)
                    {
                        m_DataList.Add(item);
                        m_NodeIndexList.Add(index);
                        m_Count++;
                    }
                }
            }
        }

        /// <summary>
        /// 清空八叉树
        /// </summary>
        public void Clear()
        {
            if (m_NodeLists != null && m_NodeLists.Count > 0)
                m_NodeLists[0].Clear(m_NodeLists);
            m_DataList.Clear();
            m_NodeIndexList.Clear();
            m_Count = 0;
        }

        /// <summary>
        /// 从树中移除对象
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(OTMeshTriangle item)
        {
            if (count <= 0)
                return false;
            int index = m_DataList.IndexOf(item);
            if (index >= 0)
            {
                int nodeIndex = m_NodeIndexList[index];
                if (m_NodeLists[nodeIndex].Remove(item))
                {
                    m_Count--;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 是否包含对象
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(OTMeshTriangle item)
        {
            if (m_NodeLists != null && m_NodeLists.Count > 0)
                return m_NodeLists[0].Contains(item, m_NodeLists);
            else
                return false;
        }

        public void Trigger(Bounds bounds, OctProjectorMesh mesh, MeshOcTreeTriggerHandle handle)
        {
            if (handle == null)
                return;
            if (m_NodeLists != null && m_NodeLists.Count > 0)
                m_NodeLists[0].Trigger(bounds, mesh, m_NodeLists, handle);
        }

        /// <summary>
        /// 绘制树（仅用于编辑器下查看）
        /// </summary>
        /// <param name="h"></param>
        /// <param name="deltaH"></param>
        public void DrawTree(float h, float deltaH)
        {
            if (m_NodeLists != null && m_NodeLists.Count > 0)
                m_NodeLists[0].DrawNode(h, deltaH, m_NodeLists);
        }

        public void DrawArea(float H, float S, float V)
        {
            if (m_NodeLists != null && m_NodeLists.Count > 0)
                m_NodeLists[0].DrawArea(H, S, V);
        }

        public void DrawArea(Color color)
        {
            if (m_NodeLists != null && m_NodeLists.Count > 0)
                m_NodeLists[0].DrawArea(color);
        }

        public static implicit operator bool(MeshOcTree tree)
        {
            return tree != null;
        }
    }
}