using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace OcTreeProjector
{
    [System.Serializable]
    internal class MeshOcTreeNode
    {

        public Bounds bounds { get { return m_Bounds; } }

        public bool HasTopLeftForwardChild { get { return m_ChildNodes[0] >= 0; } }
        public bool HasTopLeftBackChild { get { return m_ChildNodes[1] >= 0; } }
        public bool HasTopRightForwardChild { get { return m_ChildNodes[2] >= 0; } }
        public bool HasTopRightBackChild { get { return m_ChildNodes[3] >= 0; } }
        public bool HasBottomLeftForwardChild { get { return m_ChildNodes[4] >= 0; } }
        public bool HasBottomLeftBackChild { get { return m_ChildNodes[5] >= 0; } }
        public bool HasBottomRightForwardChild { get { return m_ChildNodes[6] >= 0; } }
        public bool HasBottomRightBackChild { get { return m_ChildNodes[7] >= 0; } }

        [SerializeField] private Bounds m_Bounds;

        [SerializeField]
        private List<OTMeshTriangle> m_ItemList;

        [SerializeField]
        private int[] m_ChildNodes = new int[] { -1, -1, -1, -1, -1, -1, -1, -1 };


        public MeshOcTreeNode(Bounds bounds)
        {
            this.m_Bounds = bounds;
            this.m_ItemList = new List<OTMeshTriangle>();
        }

        public MeshOcTreeNode Insert(OTMeshTriangle item, int depth, int maxDepth, List<MeshOcTreeNode> nodeList)
        {
            if (depth < maxDepth)
            {
                MeshOcTreeNode node = GetContainerNode(item, nodeList);
                if (node != null)
                    return node.Insert(item, depth + 1, maxDepth, nodeList);
            }
            m_ItemList.Add(item);
            return this;
        }

        public bool Remove(OTMeshTriangle item)
        {
            if (m_ItemList.Remove(item))
            {
                return true;
            }
            return false;
        }

        public void Clear(List<MeshOcTreeNode> nodeList)
        {
            for (int i = 0; i < m_ChildNodes.Length; i++)
            {
                if (m_ChildNodes[i] > 0)
                    nodeList[m_ChildNodes[i]].Clear(nodeList);
            }
            m_ItemList.Clear();
        }

        public bool Contains(OTMeshTriangle item, List<MeshOcTreeNode> nodeList)
        {
            for (int i = 0; i < m_ChildNodes.Length; i++)
            {
                if (m_ChildNodes[i] > 0)
                    if (nodeList[m_ChildNodes[i]].Contains(item, nodeList))
                        return true;
            }

            if (m_ItemList.Contains(item))
                return true;
            return false;
        }

        public void Trigger(Bounds bd, List<MeshOcTreeNode> nodeList, MeshOcTreeTriggerHandle handle)
        {
            if (handle == null)
                return;
            for (int i = 0; i < m_ChildNodes.Length; i++)
            {
                if (m_ChildNodes[i] > 0)
                    nodeList[m_ChildNodes[i]].Trigger(bd, nodeList, handle);
            }

            if (this.bounds.Intersects(bd))
            {
                for (int i = 0; i < m_ItemList.Count; i++)
                {
                    if (m_ItemList[i].Intersects(bd))
                        handle(m_ItemList[i]);
                }
            }
        }

        private MeshOcTreeNode GetContainerNode(OTMeshTriangle item, List<MeshOcTreeNode> nodeList)
        {
            Vector3 halfSize = bounds.size / 2;
            int result = -1;
            result = GetContainerNode(ref m_ChildNodes[0], bounds.center + new Vector3(-halfSize.x / 2, halfSize.y / 2, halfSize.z / 2),
                halfSize, item, nodeList);
            if (result > 0)
                return nodeList[result];

            result = GetContainerNode(ref m_ChildNodes[1], bounds.center + new Vector3(-halfSize.x / 2, halfSize.y / 2, -halfSize.z / 2),
                halfSize, item, nodeList);
            if (result > 0)
                return nodeList[result];

            result = GetContainerNode(ref m_ChildNodes[2], bounds.center + new Vector3(halfSize.x / 2, halfSize.y / 2, halfSize.z / 2),
                halfSize, item, nodeList);
            if (result > 0)
                return nodeList[result];

            result = GetContainerNode(ref m_ChildNodes[3], bounds.center + new Vector3(halfSize.x / 2, halfSize.y / 2, -halfSize.z / 2),
                halfSize, item, nodeList);
            if (result > 0)
                return nodeList[result];

            result = GetContainerNode(ref m_ChildNodes[4], bounds.center + new Vector3(-halfSize.x / 2, -halfSize.y / 2, halfSize.z / 2),
                halfSize, item, nodeList);
            if (result > 0)
                return nodeList[result];

            result = GetContainerNode(ref m_ChildNodes[5], bounds.center + new Vector3(-halfSize.x / 2, -halfSize.y / 2, -halfSize.z / 2),
                halfSize, item, nodeList);
            if (result > 0)
                return nodeList[result];

            result = GetContainerNode(ref m_ChildNodes[6], bounds.center + new Vector3(halfSize.x / 2, -halfSize.y / 2, halfSize.z / 2),
                halfSize, item, nodeList);
            if (result > 0)
                return nodeList[result];

            result = GetContainerNode(ref m_ChildNodes[7], bounds.center + new Vector3(halfSize.x / 2, -halfSize.y / 2, -halfSize.z / 2),
                halfSize, item, nodeList);
            if (result > 0)
                return nodeList[result];

            return null;
        }

        private int GetContainerNode(ref int node, Vector3 centerPos, Vector3 size, OTMeshTriangle item, List<MeshOcTreeNode> nodeList)
        {
            int result = -1;
            Bounds bd = item.bounds;
            if (node < 0)
            {
                Bounds bounds = new Bounds(centerPos, size);
                if (bounds.IsBoundsContainsAnotherBounds(bd))
                {
                    nodeList.Add(new MeshOcTreeNode(bounds));
                    node = nodeList.Count - 1;
                    result = node;
                }
            }
            else if (nodeList[node].bounds.IsBoundsContainsAnotherBounds(bd))
            {
                result = node;
            }
            return result;
        }

        public void DrawNode(float h, float deltaH, List<MeshOcTreeNode> nodeList)
        {
            for (int i = 0; i < m_ChildNodes.Length; i++)
            {
                if (m_ChildNodes[i] > 0)
                    nodeList[m_ChildNodes[i]].DrawNode(h + deltaH, deltaH, nodeList);
            }

            //if (flag < 8)
            DrawArea(h, 1, 1);
        }

        public void DrawArea(float H, float S, float V)
        {
            bounds.DrawBounds(H, S, V);

            if (m_ItemList != null)
            {
                for (int i = 0; i < m_ItemList.Count; i++)
                {
                    m_ItemList[i].DrawArea(0, 0, 0);
                }
            }
        }

        public void DrawArea(Color color)
        {
            bounds.DrawBounds(color);

            if (m_ItemList != null)
            {
                for (int i = 0; i < m_ItemList.Count; i++)
                {
                    m_ItemList[i].DrawArea(0, 0, 0);
                }
            }
        }
    }
}