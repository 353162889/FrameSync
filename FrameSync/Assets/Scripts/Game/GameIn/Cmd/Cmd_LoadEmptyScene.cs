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
    public class Cmd_LoadEmptyScene : CommandBase
    {
        private AsyncOperation m_cAsyncOperation;
        public override void Execute(ICommandContext context)
        {
            base.Execute(context);
            m_cAsyncOperation = SceneManager.LoadSceneAsync("Empty");
        }

        public override void OnUpdate()
        {
            if (m_cAsyncOperation.isDone)
            {
                ResourceSys.Instance.ReleaseUnUseRes();
                this.OnExecuteDone(CmdExecuteState.Success);
            }
        }

        public override void OnDestroy()
        {
            if (m_cAsyncOperation != null)
            {
                m_cAsyncOperation = null;
            }
            base.OnDestroy();
        }

    }
}
