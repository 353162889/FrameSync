using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework
{
    public class NetFrameData
    {
        private Dictionary<int, List<NetRecvData>> m_dicData;
        public int recvFrameIndex {
            get { return m_nRecvFrameIndex; }
        }
        private int m_nRecvFrameIndex;

        private int m_nCurLeaveRecvCount;
        private Queue<List<NetRecvData>> m_cPool;
        public NetFrameData()
        {
            m_dicData = new Dictionary<int, List<NetRecvData>>();
            m_nRecvFrameIndex = 0;
            m_nCurLeaveRecvCount = 0;
            m_cPool = new Queue<List<NetRecvData>>();
        }

        public bool AddRecvData(NetRecvData recvData)
        {
            if(recvData.recvOpcode == 0)
            {
                m_nCurLeaveRecvCount = recvData.len;
                m_nRecvFrameIndex = (int)recvData.data;
                if(m_nCurLeaveRecvCount > 0)
                {
                    List<NetRecvData> queue = GetList();
                    m_dicData.Add(m_nRecvFrameIndex, queue);
                }
                else
                {
                    m_nRecvFrameIndex++;
                }
                return true;
            }
            else
            {
                if(m_nCurLeaveRecvCount > 0)
                {
                    try
                    {
                        m_dicData[m_nRecvFrameIndex].Add(recvData);
                        m_nCurLeaveRecvCount--;
                        if (m_nCurLeaveRecvCount == 0)
                        {
                            m_nRecvFrameIndex++;
                        }
                    }
                    catch(Exception e)
                    {
                        CLog.LogError("接收帧消息顺序发生错误，当前包将丢失,opcode=" + recvData.recvOpcode+"\n"+ e.Message);
                    }
                    return true;
                }
            }
            return false;
        }

        public List<NetRecvData> GetFrameData(int frameIndex)
        {
            List<NetRecvData> list = null;
            m_dicData.TryGetValue(frameIndex, out list);
            return list;
        }

        public void RemoveFrameData(int frameIndex)
        {
            var lst = GetFrameData(frameIndex);
            if(lst != null)
            {
                m_dicData.Remove(frameIndex);
                SaveList(lst);
            }
        }

        private List<NetRecvData> GetList()
        {
            if(m_cPool.Count <= 0)
            {
                return new List<NetRecvData>();
            }
            else
            {
                return m_cPool.Dequeue();
            }
        }

        private void SaveList(List<NetRecvData> queue)
        {
            queue.Clear();
            m_cPool.Enqueue(queue);
        }

        public void Clear()
        {
            m_dicData.Clear();
            m_nRecvFrameIndex = 0;
            m_nCurLeaveRecvCount = 0;
            m_cPool.Clear();
        }
    }
}
