using BTCore;
using Framework;
using NodeEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
{
    public class RemoteTreePool : Singleton<RemoteTreePool>
    {
        private Dictionary<int, Queue<RemoteTree>> m_dicPool = new Dictionary<int, Queue<RemoteTree>>();

        public RemoteTree GetRemoteTree(int configId)
        {
            Queue<RemoteTree> queue = null;
            if(m_dicPool.TryGetValue(configId, out queue))
            {
                if (queue.Count > 0)
                {
                    return queue.Dequeue();
                }
            }
            NEData neData = RemoteCfgSys.Instance.GetSkillData(configId);
            RemoteTree remoteTree = CreateNode(neData) as RemoteTree;
            return remoteTree;
        }

        public void SaveRemoteTree(int configId,RemoteTree remoteTree)
        {
            Queue<RemoteTree> queue = null;
            if (!m_dicPool.TryGetValue(configId, out queue))
            {
                queue = new Queue<RemoteTree>();
                m_dicPool.Add(configId, queue);
            }
            remoteTree.Clear();
            queue.Enqueue(remoteTree);
        }

        public void Clear()
        {
            m_dicPool.Clear();
        }

        public static BTNode CreateNode(NEData neData)
        {
            Type neDataType = neData.data.GetType();
            int index = Remote.lstRemoteNodeDataType.IndexOf(neDataType);
            if (index == -1)
            {
                CLog.LogError("can not find remoteNeDataType=" + neDataType + " mapping nodeType");
                return null;
            }
            Type neNodeType = Remote.lstRemoteNodeType[index];
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
