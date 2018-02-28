using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Framework
{
    public class SocketSender
    {
        private static int MaxSendDataSize = 8192;
        private Socket m_cSocket;
        private Queue<NetSendData> m_queueData;
        private volatile bool m_bLostConnect;
        private Thread m_cThread;
        private MemoryStream m_cStream;
        private ByteBuf m_cBuffer;
        private bool m_bStop = true;
        public SocketSender(Socket socket)
        {
            m_cSocket = socket;
            m_queueData = new Queue<NetSendData>();
            m_bLostConnect = false;
            m_cStream = new MemoryStream(MaxSendDataSize);
            m_cBuffer = new ByteBuf(MaxSendDataSize);
            m_cThread = new Thread(new ThreadStart(Run));
            m_cThread.IsBackground = true;
            m_cThread.Start();
            m_bStop = false;
        }

        private void Run()
        {
            while(!m_bStop)
            {
                if(m_queueData.Count == 0)
                {
                    Thread.Sleep(1);
                    continue;
                }
                NetSendData sendData;
                lock(m_queueData)
                {
                    sendData = m_queueData.Dequeue();
                }
                try
                {
                    m_cStream.Position = 0;
                    ProtoBuf.Serializer.NonGeneric.Serialize(m_cStream, sendData.data);
                    short len = (short)m_cStream.Position;
                    CLog.Log("send msg len ="+len);
                    m_cBuffer.SetIndex(0, 0);
                    m_cBuffer.WriteShortLE(sendData.sendOpcode);
                    m_cBuffer.WriteShortLE(len);
                    m_cBuffer.WriteBytes(m_cStream.GetBuffer(), 0, len);
                    int count = m_cSocket.Send(m_cBuffer.GetRaw(), 0, m_cBuffer.WriterIndex(),SocketFlags.None);
                    if(count == 0)
                    {
                        CLog.LogError("SocketSender send data count = 0");
                        m_bLostConnect = true;
                        break;
                    }
                }
                catch(Exception e)
                {
                    CLog.LogError("SocketSender exception:" + e.Message + "\n" + e.StackTrace);
                    m_bLostConnect = true;
                    break;
                }
            }
        }

        public void SendData(NetSendData data)
        {
            lock(m_queueData)
            {
                m_queueData.Enqueue(data);
            }
        }

        public bool Update()
        {
            return !m_bLostConnect;
        }

        public void Dispose()
        {
            m_bStop = true;
            if (m_cThread != null)
            {
                m_cThread.Abort();
                m_cThread = null;
            }
            m_bLostConnect = false;
            m_cSocket = null;
            m_cStream = null;
            m_cBuffer = null;
        }

    }
}
