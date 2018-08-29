using BTCore;
using Framework;
using NodeEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
{
    public class SK_TemplateCreateRemoteActionData
    {
        [NEProperty("触发时间", true)]
        public FP time;
        [NEProperty("从挂点位置创建")]
        public string hangPoint;
        [NEProperty("使用挂点方向")]
        public bool useHangPoint;
    }
    [SkillNode(typeof(SK_TemplateCreateRemoteActionData))]
    [NENodeDesc("从宿主挂点位置创建远程")]
    public class SK_TemplateCreateRemoteAction : BaseTimeLineSkillAction
    {
        private SK_TemplateCreateRemoteActionData m_cActionData;
        public override FP time { get { return m_cActionData.time; } }

        protected override void OnInitData(object data)
        {
            base.OnInitData(data);
            m_cActionData = data as SK_TemplateCreateRemoteActionData;
        }

        public override BTActionResult OnRun(SkillBlackBoard blackBoard)
        {
            Skill skill = blackBoard.skill;
            AgentObject host = skill.host;
            TSVector bornPosition = host.curPosition;
            TSVector bornForward = host.curForward;
            if (!string.IsNullOrEmpty(m_cActionData.hangPoint))
            {
                host.GetHangPoint(m_cActionData.hangPoint, out bornPosition, out bornForward);
            }
            TSVector targetForward = skill.targetForward;
            if (m_cActionData.useHangPoint)
            {
                targetForward = bornForward;
            }
            CLog.LogError("SK_TemplateCreateRemoteAction需要重写远程ID");
            var remote = BattleScene.Instance.CreateRemote(0, skill.host.campId, bornPosition, targetForward, skill.targetAgentId, skill.targetAgentType, skill.targetPosition, targetForward);
            //初始化远程属性
            remote.SetAttrValue((int)AttrType.Attack, host.GetAttrValue((int)AttrType.Attack));
            return BTActionResult.Ready;
        }
    }
}
