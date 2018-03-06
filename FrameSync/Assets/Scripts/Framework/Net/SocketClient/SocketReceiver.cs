using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Framework
{
    //一般包 2(code > 256) + 2(len)
    //帧包1(0) + 1(count) + 4(frameIndex)
    //接收前两字节的长度少于256，那么当前包是帧包（第一个字节是0,第二个字节是包含包的数量）
    public class SocketReceiver
    {
        private static int MaxRecvDataSize = 8192;
        private Socket m_cSocket;
        private Queue<NetRecvData> m_queueData;
        private volatile bool m_bLostConnect;
        private Thread m_cThread;
        private MemoryStream m_cStream;
        private byte[] m_cBuffer;
        private Dictionary<short, Type> m_dicOpcodeToType;
        private bool m_bStop = true;

        public SocketReceiver(Socket socket)
        {
            m_cSocket = socket;
            m_queueData = new Queue<NetRecvData>();
            m_bLostConnect = false;
            m_dicOpcodeToType = new Dictionary<short, Type>();
            LoadProtoTypes();
            m_cStream = new MemoryStream(MaxRecvDataSize);
            m_cBuffer = new byte[MaxRecvDataSize];
            m_cThread = new Thread(new ThreadStart(Run));
            m_cThread.IsBackground = true;
            m_cThread.Start();
            m_bStop = false;
        }

        private void LoadProtoTypes()
        {
            Type type = typeof(Proto.PacketOpcode);
            var values = Enum.GetNames(type);
            foreach (var item in values)
            {
                string fullName = "Proto."+ item + "_Data";
                Type t = Type.GetType(fullName);
                m_dicOpcodeToType.Add((short)(int)Enum.Parse(type,item), t);
            }
        }

        private void Run()
        {
            while (!m_bStop)
            {
                try
                {
                    int opcodeCount = m_cSocket.Receive(m_cBuffer, 0, 2, SocketFlags.None);
                    if (CheckReceiveZero(opcodeCount)) break;
                    short opcode = BitConverter.ToInt16(m_cBuffer, 0);
                    NetRecvData netData = new NetRecvData();
                    //如果是帧包
                    if(opcode < 256)
                    {
                        netData.recvOpcode = 0;
                        netData.len = opcode;
                        int frameIndexCount = m_cSocket.Receive(m_cBuffer, 0, 4, SocketFlags.None);
                        if (CheckReceiveZero(frameIndexCount)) break;
                        netData.data = BitConverter.ToInt32(m_cBuffer, 0);
                       // CLog.Log(string.Format("收到帧包frameCount={0},frameIndex={1},buff={2}",netData.len,netData.data,BitConverter.ToString(m_cBuffer,0,4)));
                    }
                    else
                    {
                        netData.recvOpcode = opcode;
                        //读取长度
                        int lenCount = m_cSocket.Receive(m_cBuffer, 0, 2, SocketFlags.None);
                        if (CheckReceiveZero(lenCount)) break;
                        netData.len = BitConverter.ToInt16(m_cBuffer, 0);
                        int dataLen = m_cSocket.Receive(m_cBuffer, 0, (int)netData.len, SocketFlags.None);
                        if (CheckReceiveZero(dataLen)) break;
                        //反序列化
                        try {
                            Type type = m_dicOpcodeToType[netData.recvOpcode];
                            if (ProtoBuf.Serializer.NonGeneric.CanSerialize(type))
                            {
                                m_cStream.Position = 0;
                                m_cStream.Write(m_cBuffer, 0, netData.len);
                                m_cStream.Position = 0;
                                netData.data = ProtoBuf.Serializer.NonGeneric.Deserialize(type, m_cStream);
                            }
                        }
                        catch(Exception ex)
                        {
                            CLog.LogError("SocketReceiver Deserialize fail!opcode=" + netData.recvOpcode);
                            continue;
                        }
                    }
                    lock (m_queueData)
                    {
                        m_queueData.Enqueue(netData);
                    }
                }
                catch(Exception e)
                {
                    //忽略WSACancelBlockingCall异常
                    if (!e.Message.Contains("WSACancelBlockingCall"))
                    {
                        CLog.LogError("SocketReceiver exception:" + e.Message + "\n" + e.StackTrace);
                    }
                    m_bLostConnect = true;
                    break;
                }
            }
        }

        public int RecvNetData(Queue<NetRecvData> queue)
        {
            int count = 0;
            lock(m_queueData)
            {
                while(m_queueData.Count > 0)
                {
                    count++;
                    queue.Enqueue(m_queueData.Dequeue());
                }
            }
            return count;
        }

        private bool CheckReceiveZero(int count)
        {
            if (count == 0)
            {
                CLog.LogError("SocketReceiver receive count = 0");
                m_bLostConnect = true;
                return true;
            }
            return false;
        }

        public bool Update()
        {
            return !m_bLostConnect;
        }

        public void Dispose()
        {
            m_bLostConnect = false;
            m_cSocket = null;
            m_cStream = null;
            m_cBuffer = null;
            m_bStop = true;
            if (m_cThread != null)
            {
                m_cThread.Abort();
                m_cThread = null;
            }
        }
    }
}
