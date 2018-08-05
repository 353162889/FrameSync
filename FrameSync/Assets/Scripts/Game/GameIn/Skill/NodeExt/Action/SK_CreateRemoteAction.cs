using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BTCore;
using Framework;
using NodeEditor;

namespace Game
{
    public class SK_CreateRemoteActionData
    {
        [NEProperty("触发时间",true)]
        public FP time;
        [NEProperty("远程ID")]
        public int remoteId;
        [NEProperty("从挂点位置创建")]
        public string hangPoint;
        [NEProperty("使用挂点方向")]
        public bool useHangPoint;
    }
    [SkillNode(typeof(SK_CreateRemoteActionData))]
    [NENodeDesc("从宿主挂点位置创建远程")]
    public class SK_CreateRemoteAction : BaseTimeLineSkillAction
    {
        private SK_CreateRemoteActionData m_cActionData;
        public override FP time { get { return m_cActionData.time; } }

        protected override void OnInitData(object data)
        {
            base.OnInitData(data);
            m_cActionData = data as SK_CreateRemoteActionData;
        }

        public override BTActionResult OnRun(SkillBlackBoard blackBoard)
        {
            Skill skill = blackBoard.skill;
            AgentObject host = skill.host;
            TSVector bornPosition = host.curPosition;
            TSVector bornForward = host.curForward;
            if (!string.IsNullOrEmpty(m_cActionData.hangPoint))
            {
                host.GetHangPoint(m_cActionData.hangPoint,out bornPosition,out bornForward);
            }
            TSVector targetForward = skill.targetForward;
            if(m_cActionData.useHangPoint)
            {
                targetForward = bornForward;
            }
            var remote = BattleScene.Instance.CreateRemote(m_cActionData.remoteId,skill.host.campId, bornPosition, targetForward, skill.targetAgentId, skill.targetAgentType, skill.targetPosition, targetForward);
            //初始化远程属性
            remote.SetAttrValue((int)AttrType.Attack, host.GetAttrValue((int)AttrType.Attack));
            return BTActionResult.Ready;
        }
    }
}
