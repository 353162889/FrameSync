using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BTCore;
using NodeEditor;
using Framework;

namespace Game
{
    public class BTG_IsSelectDecoratorData
    {
    }

    [BTGameNode(typeof(BTG_IsSelectDecoratorData))]
    [NENodeDesc("装饰某个action操作是否击中目标对象")]
    public class BTG_IsSelectDecorator : BaseBTGameDecorator
    {
        private bool m_bIsSelect;
        private BTG_IsSelectDecoratorData m_cIsSelectData;
        protected override void OnInitData(object data)
        {
            base.OnInitData(data);
            m_cIsSelectData = data as BTG_IsSelectDecoratorData;
        }

        public override BTResult OnEnter(AgentObjectBlackBoard blackBoard)
        {
            GlobalEventDispatcher.Instance.AddEvent(GameEvent.BTNodeSelectCompositeSelectTarget, OnSelectTarget);
            m_bIsSelect = false;
            BTResult result = base.OnEnter(blackBoard);
            GlobalEventDispatcher.Instance.RemoveEvent(GameEvent.BTNodeSelectCompositeSelectTarget, OnSelectTarget);
            if (m_bIsSelect)
            {
                return BTResult.Success;
            }
            else
            {
                return BTResult.Failure;
            }
        }

        private void OnSelectTarget(object args)
        {
            m_bIsSelect = true;
        }
    }
}
