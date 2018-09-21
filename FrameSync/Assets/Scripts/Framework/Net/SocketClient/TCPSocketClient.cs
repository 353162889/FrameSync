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
        private SocketSender m_cSender;
        private SocketReceiver m_cReceiver;

        public override int RecvNetData(Queue<NetRecvData> queue)
        {
            if(m_cReceiver != null)
            {
                m_cReceiver.RecvNetData(queue);
            }
            else
            {
                CLog.LogError("socket not connected,can not recv net Data!");
            }
            return 0;
        }

        public override void SendNetData(NetSendData data)
        {
            if(m_cSender != null)
            {
                m_cSender.SendData(data);
            }
            else
            {
                CLog.LogError("socket not connected,can not send net Data!");
            }
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

        protected override SocketClientStatus SocketConnectingStatus()
        {
            if(m_cAsyncResult != null)
            {
                if(m_cAsyncResult.IsCompleted)
                {

                    if (m_cAsyncResult != null)
                    {
                        try
                        {
                            m_cSocket.EndConnect(m_cAsyncResult);
                            m_cAsyncResult = null;
                        }
                        catch (Exception e)
                        {
                            m_cAsyncResult = null;
                            return SocketClientStatus.DisConnect;
                        }
                        OnConnectedSucc();
                        return SocketClientStatus.Connected;
                    }
                }
            }
            return SocketClientStatus.Connecting;
        }

        private void OnConnectedSucc()
        {
            //初始化发送接收
            m_cSender = new SocketSender(m_cSocket,m_cHeartBeatInfo);
            m_cReceiver = new SocketReceiver(m_cSocket,m_cHeartBeatInfo);
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
                if (m_cSender != null)
                {
                    m_cSender.Dispose();
                    m_cSender = null;
                }
                if (m_cReceiver != null)
                {
                    m_cReceiver.Dispose();
                    m_cReceiver = null;
                }
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

        public override void OnUpdate()
        {
            if(m_cSender != null)
            {
                if(!m_cSender.Update())
                {
                    this.LostConnect();
                }
            }
            if(m_cReceiver != null)
            {
                if(!m_cReceiver.Update())
                {
                    this.LostConnect();
                }
            }
            base.OnUpdate();
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
            CLog.LogError("ip="+ip+",port="+port+" can not arrived!(or format error)");
            return false;
        }
    }
}
