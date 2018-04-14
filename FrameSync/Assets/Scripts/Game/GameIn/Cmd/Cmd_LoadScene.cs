using Framework;
using GameData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game
{
    public class Cmd_LoadScene : CommandBase
    {
        private int m_nSceneId;
        public int sceneId { get { return m_nSceneId; } }
        private AsyncOperation m_cAsyncOperation;
        public override void Execute(ICommandContext context)
        {
            base.Execute(context);
            GameInContext gameInContext = context as GameInContext;
            m_nSceneId = gameInContext.sceneId;
            var resScene = ResCfgSys.Instance.GetCfg<ResScene>(m_nSceneId);
            m_cAsyncOperation = SceneManager.LoadSceneAsync(resScene.name);
        }

        public override void OnUpdate()
        {
            if(m_cAsyncOperation.isDone)
            {
                this.OnExecuteDone(CmdExecuteState.Success);
            }
        }

        public override void OnDestroy()
        {
            if(m_cAsyncOperation != null)
            {
                m_cAsyncOperation = null;
            }
            base.OnDestroy();
        }

    }
}
