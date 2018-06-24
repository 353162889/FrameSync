using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BTCore;
using Framework;
using NodeEditor;

namespace Game
{
    public class BTG_LastPosToCurPosSelectCompositeData : BTG_BaseSelectAgentObjCompositeData
    {
        [NEProperty("挂点(不填使用当前位置)")]
        public string hangPoint;
    }
    [BTGameNode(typeof(BTG_LastPosToCurPosSelectCompositeData))]
    public class BTG_LastPosToCurPosSelectComposite : BTG_BaseSelectAgentObjComposite
    {
        private BTG_LastPosToCurPosSelectCompositeData m_cLastPosToCurPosData;
        protected override void OnInitData(object data)
        {
            base.OnInitData(data);
            m_cLastPosToCurPosData = data as BTG_LastPosToCurPosSelectCompositeData;
        }


        protected override void OnSelectChild(AgentObjectBlackBoard blackBoard, List<AgentObject> lst, ref List<SelectAgentObjInfo> result)
        {
            AgentObject host = blackBoard.host;
            TSVector lastPosition = host.lastPosition;
            TSVector lastForward = host.lastForward;
            if(!string.IsNullOrEmpty(m_cLastPosToCurPosData.hangPoint))
            {
                blackBoard.host.GetHangPoint(m_cLastPosToCurPosData.hangPoint, host.lastPosition, host.lastForward, out lastPosition, out lastForward);
            }
            TSVector curPosition = host.curPosition;
            TSVector curForward = host.curForward;
            if (!string.IsNullOrEmpty(m_cLastPosToCurPosData.hangPoint))
            {
                blackBoard.host.GetHangPoint(m_cLastPosToCurPosData.hangPoint, host.curPosition, host.curForward, out curPosition, out curForward);
            }
            //检测lastPosition到curPosition这条之间碰撞到的代理
        }
    }
}
