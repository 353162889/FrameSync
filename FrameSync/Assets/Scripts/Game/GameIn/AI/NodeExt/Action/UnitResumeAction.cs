using UnityEngine;
using System.Collections;
using NodeEditor;
using Framework;
using BTCore;

namespace Game
{
    public class UnitResumeActionData
    {
        [NEProperty("触发时间", true)]
        public FP time;
        [NEProperty("还原禁止类型")]
        public UnitForbidType eForbidType;
    }

    [AINode(typeof(UnitResumeActionData))]
    [NENodeDesc("AI还原unit行为")]
    public class UnitResumeAction : BaseTimeLineAIAction
    {
        private UnitResumeActionData m_cResumeData;
        public override FP time
        {
            get
            {
                return m_cResumeData.time;
            }
        }


        protected override void OnInitData(object data)
        {
            base.OnInitData(data);
            m_cResumeData = data as UnitResumeActionData;
        }


        public override BTActionResult OnRun(AIBlackBoard blackBoard)
        {
            var unit = (Unit)blackBoard.host.agent;
            if (unit != null)
            {
                unit.Resume(m_cResumeData.eForbidType,UnitForbidFromType.AI);
            }
            return BTActionResult.Ready;
        }
    }
}