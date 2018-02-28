using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework
{
    public class FrameSyncSys : SingletonMonoBehaviour<FrameSyncSys>
    {
        public static int LOGIC_FRAME_DELTA = 50;  //逻辑帧设定为20帧/s
        public delegate void FrameSyncUpdateHandler();
        public event FrameSyncUpdateHandler OnFrameSyncUpdate;
        private bool m_bStartRun;
        private int m_nFrameIndex;
        protected override void Init()
        {
            m_bStartRun = false;
            m_nFrameIndex = 0;
        }

        public void StartRun()
        {
            m_bStartRun = true;
        }

        void Update()
        {
            if(m_bStartRun)
            {
                bool succ = NetSys.Instance.RunFrameData(NetChannelType.Game, m_nFrameIndex);
                if (succ)
                {
                    if (null != OnFrameSyncUpdate)
                    {
                        OnFrameSyncUpdate();
                    }

                    m_nFrameIndex++;
                }
            }
        }

        protected override void OnDestroy()
        {
            OnFrameSyncUpdate = null;
            base.OnDestroy();
        }
    }
}
