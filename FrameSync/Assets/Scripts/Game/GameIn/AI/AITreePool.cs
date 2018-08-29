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
        private Dictionary<string, Queue<AITree>> m_dicPool = new Dictionary<string, Queue<AITree>>();

        public AITree GetAITree(string aiPath)
        {
            Queue<AITree> queue = null;
            if (m_dicPool.TryGetValue(aiPath, out queue))
            {
                if (queue.Count > 0)
                {
                    return queue.Dequeue();
                }
            }
            NEData neData = AICfgSys.Instance.GetAIData(aiPath);
            AITree aiTree = CreateNode(neData) as AITree;
            return aiTree;
        }

        public void SaveAITree(string aiPath, AITree aiTree)
        {
            Queue<AITree> queue = null;
            if (!m_dicPool.TryGetValue(aiPath, out queue))
            {
                queue = new Queue<AITree>();
                m_dicPool.Add(aiPath, queue);
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
            if (!neData.enable) return null;
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
