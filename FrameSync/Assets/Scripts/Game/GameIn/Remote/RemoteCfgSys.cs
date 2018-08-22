﻿using Framework;
using NodeEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
{
    public class RemoteCfgSys : Singleton<RemoteCfgSys>
    {
        private NEDataLoader m_cNEDataLoader;
        public void LoadResCfgs(Action onFinish)
        {
            m_cNEDataLoader = new NEDataLoader();
            List<string> files = new List<string>();
            files.Add("Config/Remote/1.bytes");
            files.Add("Config/Remote/2.bytes");
            Remote.Init();
            m_cNEDataLoader.Load(files, Remote.arrRemoteNodeDataType, onFinish);
        }

        public NEData GetSkillData(int remoteId)
        {
            NEData neData = m_cNEDataLoader.Get(remoteId);
            if (neData == null)
            {
                CLog.LogError("can not find remoteId = " + remoteId + " cfgs!");
            }
            return neData;
        }

        public void Clear()
        {
            if (m_cNEDataLoader != null)
            {
                m_cNEDataLoader.Clear();
                m_cNEDataLoader = null;
            }
        }
    }
}
