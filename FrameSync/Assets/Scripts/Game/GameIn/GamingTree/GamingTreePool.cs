using BTCore;
using Framework;
using NodeEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
{
    public class GamingTreePool : Singleton<GamingTreePool>
    {
        private Dictionary<int, Queue<GamingTree>> m_dicPool = new Dictionary<int, Queue<GamingTree>>();

        public GamingTree GetGamingTree(int configId)
        {
            Queue<GamingTree> queue = null;
            if (m_dicPool.TryGetValue(configId, out queue))
            {
                if (queue.Count > 0)
                {
                    return queue.Dequeue();
                }
            }
            NEData neData = GamingCfgSys.Instance.GetGamingData(configId);
            GamingTree aiTree = CreateNode(neData) as GamingTree;
            return aiTree;
        }

        public void SaveGamingTree(int configId, GamingTree gamingTree)
        {
            Queue<GamingTree> queue = null;
            if (!m_dicPool.TryGetValue(configId, out queue))
            {
                queue = new Queue<GamingTree>();
                m_dicPool.Add(configId, queue);
            }
            gamingTree.Clear();
            queue.Enqueue(gamingTree);
        }

        public void Clear()
        {
            m_dicPool.Clear();
        }

        public static BTNode CreateNode(NEData neData)
        {
            if (!neData.enable) return null;
            Type neDataType = neData.data.GetType();
            int index = GamingLogic.lstGamingNodeDataType.IndexOf(neDataType);
            if (index == -1)
            {
                CLog.LogError("can not find gamingNeDataType=" + neDataType + " mapping nodeType");
                return null;
            }
            Type neNodeType = GamingLogic.lstGamingNodeType[index];
            BTNode neNode = Activator.CreateInstance(neNodeType) as BTNode;
            neNode.data = neData.data;
            if (neData.lstChild != null)
            {
                for (int i = 0; i < neData.lstChild.Count; i++)
                {
                    BTNode childNode = CreateNode(neData.lstChild[i]);
                    if (childNode != null)
                    {
                        neNode.AddChild(childNode);
                    }
                }
            }
            return neNode;
        }
    }
}
