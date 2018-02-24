using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework
{
    public enum SocketClientStatus
    {
        DisConnect = 1,
        Connecting = 2,
        Connected = 3
    }

    public abstract class SocketClient
    {
        //非手动断开连接时
        public event Action OnDisConnect;

        private Action<bool> m_connectCallback;

        private volatile bool m_bIsDisConnect = false;
        private volatile SocketClientStatus m_eStatus;
       
        public SocketClientStatus Status { get { return m_eStatus; }
            private set {
                if(m_eStatus != value)
                {
                    m_eStatus = value;
                    if(m_eStatus == SocketClientStatus.DisConnect)
                    {
                        m_bIsDisConnect = true;
                    }
                }
            } }

        protected float m_fStartConnectTime;
        public float TimeOut { get; set; }

        public SocketClient()
        {
            m_bIsDisConnect = false;
            m_eStatus = SocketClientStatus.DisConnect;
            m_fStartConnectTime = 0;
            TimeOut = 10f;
        }

        //开始连接
        public bool BeginConnect(string ip, int port, Action<bool> callback)
        {
            if (m_eStatus != SocketClientStatus.DisConnect)
            {
                CLog.LogError("socket client is not disConnect,can not beginConnect!");
                return false;
            }
            DisConnect();
            m_eStatus = SocketClientStatus.Connecting;
            m_fStartConnectTime = UnityEngine.Time.time;
            this.m_connectCallback = callback;
            bool succ = BeginConnect(ip, port);
            if (!succ)
            {
                if(null != this.m_connectCallback)
                {
                    this.m_connectCallback(false);
                    this.m_connectCallback = null;
                }
            }
            return succ;
        }

        //开始连接
        protected abstract bool BeginConnect(string ip, int port);
        //是否连接成功
        protected abstract bool IsSocketConnected();
        //发送网络数据（未序列化）
        public abstract void SendNetData(NetSendData data);
        //接收网络数据(已反序列化)
        public abstract int RecvNetData(Queue<NetRecvData> queue);
        //接受指定个数的网络数据
        public abstract int RecvNetData(Queue<NetRecvData> queue,int count);
        //用户断开连接，(外部)手动断开连接
        public virtual void DisConnect()
        {
            m_eStatus = SocketClientStatus.DisConnect;
            m_connectCallback = null;
            m_bIsDisConnect = false;
            m_fStartConnectTime = 0;
        }

        //系统断开连接
        protected void LostConnect()
        {
            Status = SocketClientStatus.DisConnect;
        }

        //主线程中
        public virtual void OnUpdate()
        {
            if(m_eStatus == SocketClientStatus.Connecting)
            {
                //连接超时（直接断开连接）
                if(UnityEngine.Time.time - m_fStartConnectTime > TimeOut)
                {
                    if(null != m_connectCallback)
                    {
                        m_connectCallback(false);
                        m_connectCallback = null;
                    }
                    DisConnect();
                }
                else
                {
                    if(IsSocketConnected())
                    {
                        m_eStatus = SocketClientStatus.Connected;
                        if (null != m_connectCallback)
                        {
                            m_connectCallback(true);
                            m_connectCallback = null;
                        }
                    }
                }
            }
            if(m_bIsDisConnect)
            {
                if (null != OnDisConnect)
                {
                    OnDisConnect();
                }
                DisConnect();
            }
        }

        //释放数据
        public virtual void Dispose()
        {
            DisConnect();
            OnDisConnect = null;
        }
    }
}
