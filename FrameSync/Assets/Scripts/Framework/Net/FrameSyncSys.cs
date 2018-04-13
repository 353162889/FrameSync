using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework
{
    public class FrameSyncSys : SingletonMonoBehaviour<FrameSyncSys>
    {
        private static FP OnFrameTime = FP.One / 20;
        public delegate void FrameSyncUpdateHandler(FP deltaTime);
        public event FrameSyncUpdateHandler OnFrameSyncUpdate;
        private bool m_bStartRun = false;
        private static int m_nFrameIndex;
        public static int frameIndex { get { return m_nFrameIndex; } }
        private static FP m_fpTime;
        public static FP time { get { return m_fpTime; } }

        public void StartRun()
        {
            m_bStartRun = true;
            m_nFrameIndex = 0;
            m_fpTime = 0;
            OnFrameSyncUpdate = null;
        }

        public void StopRun()
        {
            m_bStartRun = false;
            m_nFrameIndex = 0;
            m_fpTime = 0;
            OnFrameSyncUpdate = null;
        }

        void Update()
        {
            if(m_bStartRun)
            {
                bool succ = NetSys.Instance.RunFrameData(NetChannelType.Game, m_nFrameIndex);
                if (succ)
                {
                    m_nFrameIndex++;
                    m_fpTime = m_fpTime + OnFrameTime;
                    if (null != OnFrameSyncUpdate)
                    {
                        OnFrameSyncUpdate(OnFrameTime);
                    }
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
