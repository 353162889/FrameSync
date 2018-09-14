using UnityEngine;
using NodeEditor;
using Framework;
using BTCore;

namespace Game
{

    public class GL_WaitStageClearData
    {
        [NEProperty("触发时间")]
        public FP time;
        [NEProperty("当前阵容")]
        public CampType camp;
    }

    [GamingNode(typeof(GL_WaitStageClearData))]
    [NENodeDesc("等待一段时间")]
    public class GL_WaitStageClear : BaseTimeLineGamingAction
    {
        private GL_WaitStageClearData m_cWaitStageClearData;

        public override FP time
        {
            get
            {
                return m_cWaitStageClearData.time;
            }
        }

        protected override void OnInitData(object data)
        {
            base.OnInitData(data);
            m_cWaitStageClearData = data as GL_WaitStageClearData;
        }


        protected override BTActionResult OnRun(GamingBlackBoard blackBoard)
        {
            AgentObjField field = BattleScene.Instance.GetField((int)m_cWaitStageClearData.camp, AgentObjectType.Unit);
            if (field.lstFriend.Count > 0) return BTActionResult.Running;
            return BTActionResult.Ready;
        }
      
    }
}