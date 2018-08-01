using BTCore;
using Framework;
using NodeEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
{
    public class AITreePool : Singleton<AITreePool>
    {
        private Dictionary<int, Queue<AITree>> m_dicPool = new Dictionary<int, Queue<AITree>>();

        public AITree GetAITree(int configId)
        {
            Queue<AITree> queue = null;
            if (m_dicPool.TryGetValue(configId, out queue))
            {
                if (queue.Count > 0)
                {
                    return queue.Dequeue();
                }
            }
            NEData neData = AICfgSys.Instance.GetAIData(configId);
            AITree aiTree = CreateNode(neData) as AITree;
            return aiTree;
        }

        public void SaveAITree(int configId, AITree aiTree)
        {
            Queue<AITree> queue = null;
            if (!m_dicPool.TryGetValue(configId, out queue))
            {
                queue = new Queue<AITree>();
                m_dicPool.Add(configId, queue);
            }
            aiTree.Clear();
            queue.Enqueue(aiTree);
        }

        public void Clear()
        {
            m_dicPool.Clear();
        }

        public static BTNode CreateNode(NEData neData)
        {
            Type neDataType = neData.data.GetType();
            int index = AgentObjectAI.lstAINodeDataType.IndexOf(neDataType);
            if (index == -1)
            {
                CLog.LogError("can not find aiNeDataType=" + neDataType + " mapping nodeType");
                return null;
            }
            Type neNodeType = AgentObjectAI.lstAINodeType[index];
            BTNode neNode = Activator.CreateInstance(neNodeType) as BTNode;
            neNode.data = neData.data;
            if (neData.lstChild != null)
            {
                for (int i = 0; i < neData.lstChild.Count; i++)
                {
                    BTNode childNode = CreateNode(neData.lstChild[i]);
                    neNode.AddChild(childNode);
                }
            }
            return neNode;
        }
    }
}
