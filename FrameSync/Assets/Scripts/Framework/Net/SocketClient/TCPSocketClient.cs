using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Framework
{
    public class TCPSocketClient : SocketClient
    {
        private volatile Socket m_cSocket;
        private IAsyncResult m_cAsyncResult;

        public override int RecvNetData(Queue<NetRecvData> queue)
        {
            return 0;
        }

        public override int RecvNetData(Queue<NetRecvData> queue, int count)
        {
            return 0;
        }

        public override void SendNetData(NetSendData data)
        {
        }

        protected override bool BeginConnect(string ip, int port)
        {
            IPEndPoint point;
            if (!CheckEndToPoint(ip, port, out point)) return false;
            m_cSocket = new Socket(point.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            m_cAsyncResult = m_cSocket.BeginConnect(point, AsyncCallback, m_cSocket);
            return true;
        }
        private void AsyncCallback(IAsyncResult ar)
        {
            CLog.Log("Tcp AsyncCallback");
        }

        protected override bool IsSocketConnected()
        {
            if(m_cAsyncResult != null)
            {
                if(m_cAsyncResult.IsCompleted)
                {
                    OnConnectedSucc();
                    return true;
                }
            }
            return false;
        }

        private void OnConnectedSucc()
        {
            //初始化发送接收
        }

        public override void DisConnect()
        {
            try {
                if (m_cSocket != null)
                {
                    if (m_cAsyncResult != null)
                    {
                        m_cSocket.EndConnect(m_cAsyncResult);
                    }
                    if(Status == SocketClientStatus.Connected)
                    {
                        m_cSocket.Shutdown(SocketShutdown.Both);
                    }
                }
            }catch(Exception e)
            {
                CLog.LogError(e.Message+"\n"+e.StackTrace);
            }
            finally
            {
                try
                {
                    if (m_cSocket != null)
                    {
                        m_cSocket.Close();
                    }
                }
                catch (Exception e)
                {
                    CLog.LogError(e.Message + "\n" + e.StackTrace);
                }
                m_cSocket = null;
                m_cAsyncResult = null;
                base.DisConnect();
            }
        }

        private bool CheckEndToPoint(string ip,int port,out IPEndPoint point)
        {
            point = null;
            IPAddress ipAddress;
            if(IPAddress.TryParse(ip,out ipAddress))
            {
                point = new IPEndPoint(ipAddress, port);
                return true;
            }
            return false;
        }
    }
}
