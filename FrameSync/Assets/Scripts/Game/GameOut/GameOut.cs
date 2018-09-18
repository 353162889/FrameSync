using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework;

namespace Game
{
    public class GameOut : StateBase
    {
        private CommandSequence m_cJoinSequence;
        protected override void OnEnter()
        {
            base.OnEnter();
            ViewSys.Instance.Open("LoadingView");
            m_cJoinSequence = new CommandSequence();
            var cmdLoadEmptyScene = new Cmd_LoadEmptyScene();
            m_cJoinSequence.AddSubCommand(cmdLoadEmptyScene);
            m_cJoinSequence.On_Done += OnJoinScene;
            m_cJoinSequence.Execute();
        }

        private void OnJoinScene(CommandBase obj)
        {
            m_cJoinSequence = null;
            ViewSys.Instance.Close("LoadingView");
            ViewSys.Instance.Open("LobbyView");
        }

        protected override void OnUpdate()
        {
            if(m_cJoinSequence != null)
            {
                m_cJoinSequence.OnUpdate();
            }
        }

        protected override void OnExit()
        {
            m_cJoinSequence = null;
            ViewSys.Instance.Close("LobbyView");
            base.OnExit();
        }
    }
}
