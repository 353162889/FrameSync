using UnityEngine;
using System.Collections;
using NodeEditor;
using Framework;
using BTCore;

namespace Game
{
    public class UnitForbidActionData
    {
        [NEProperty("触发时间", true)]
        public FP time;
        [NEProperty("禁止类型")]
        public UnitForbidType eForbidType;
    }

    [AINode(typeof(UnitForbidActionData))]
    [NENodeDesc("AI禁止unit行为,AI结束，该禁止行为立即结束")]
    public class UnitForbidAction : BaseTimeLineAIAction
    {
        private UnitForbidActionData m_cForbidData; 
        public override FP time
        {
            get
            {
                return m_cForbidData.time;
            }
        }


        protected override void OnInitData(object data)
        {
            base.OnInitData(data);
            m_cForbidData = data as UnitForbidActionData;
        }


        public override BTActionResult OnRun(AIBlackBoard blackBoard)
        {
            var unit = (Unit)blackBoard.host.agent;
            if (unit != null)
            {
                unit.Forbid(m_cForbidData.eForbidType, UnitForbidFromType.AI);
            }
            return BTActionResult.Ready;
        }
    }
}