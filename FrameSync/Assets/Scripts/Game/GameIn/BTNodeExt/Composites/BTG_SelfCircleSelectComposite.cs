using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BTCore;
using Framework;
using NodeEditor;

namespace Game
{
    public class BTG_SelfCircleSelectCompositeData : BTG_BaseSelectAgentObjCompositeData
    {
        [NEProperty("挂点(不填使用当前位置)")]
        public string hangPoint;
        [NEProperty("半径(为0的话不检测碰撞)")]
        public FP radius;
    }
    [BTGameNode(typeof(BTG_SelfCircleSelectCompositeData))]
    public class BTG_SelfCircleSelectComposite : BTG_BaseSelectAgentObjComposite
    {
        private BTG_SelfCircleSelectCompositeData m_cSelfCircleSelectData;
        protected override void OnInitData(object data)
        {
            base.OnInitData(data);
            m_cSelfCircleSelectData = data as BTG_SelfCircleSelectCompositeData;
        }


        protected override void OnSelectChild(AgentObjectBlackBoard blackBoard, List<AgentObject> lst, ref List<SelectAgentObjInfo> result)
        {
            if (lst.Count == 0 || m_cSelfCircleSelectData.radius <= 0) return;
            AgentObject host = blackBoard.host;
            TSVector curPosition = host.curPosition;
            TSVector curForward = host.curForward;
            if (!string.IsNullOrEmpty(m_cSelfCircleSelectData.hangPoint))
            {
                blackBoard.host.GetHangPoint(m_cSelfCircleSelectData.hangPoint, host.curPosition, host.curForward, out curPosition, out curForward);
            }
            //检测lastPosition到curPosition这条之间碰撞到的代理
            for (int i = 0; i < lst.Count; i++)
            {
                var agentObj = lst[i];
                if (agentObj.gameCollider != null)
                {
                    if (agentObj.gameCollider.CheckCircle(host.curPosition, m_cSelfCircleSelectData.radius))
                    {
                        SelectAgentObjInfo info = new SelectAgentObjInfo();
                        info.agentObj = agentObj;
                        info.hitPoint = agentObj.curPosition;
                        info.hitDirect = host.curForward;
                        result.Add(info);
                    }
                }
            }
        }
    }
}
