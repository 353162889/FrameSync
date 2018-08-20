using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BTCore;
using Framework;
using NodeEditor;

namespace Game
{
    public class BTG_LastRectToCurRectSelectCompositeData : BTG_BaseSelectAgentObjCompositeData
    {
        [NEProperty("挂点(不填使用当前位置)")]
        public string hangPoint;
        [NEProperty("宽度(为0的话不检测碰撞)")]
        public FP width;
    }
    [BTGameNode(typeof(BTG_LastRectToCurRectSelectCompositeData))]
    public class BTG_LastRectToCurRectSelectComposite : BTG_BaseSelectAgentObjComposite
    {
        private BTG_LastRectToCurRectSelectCompositeData m_cLastRectToCurRectData;
        protected override void OnInitData(object data)
        {
            base.OnInitData(data);
            m_cLastRectToCurRectData = data as BTG_LastRectToCurRectSelectCompositeData;
        }


        protected override void OnSelectChild(AgentObjectBlackBoard blackBoard, List<AgentObject> lst, ref List<SelectAgentObjInfo> result)
        {
            if (lst.Count == 0 || m_cLastRectToCurRectData.width <= 0) return;
            AgentObject host = blackBoard.host;
            TSVector lastPosition = host.lastPosition;
            TSVector lastForward = host.lastForward;
            if (!string.IsNullOrEmpty(m_cLastRectToCurRectData.hangPoint))
            {
                host.GetHangPoint(m_cLastRectToCurRectData.hangPoint, host.lastPosition, host.lastForward, out lastPosition, out lastForward);
            }
            TSVector curPosition = host.curPosition;
            TSVector curForward = host.curForward;
            if (!string.IsNullOrEmpty(m_cLastRectToCurRectData.hangPoint))
            {
                host.GetHangPoint(m_cLastRectToCurRectData.hangPoint, host.curPosition, host.curForward, out curPosition, out curForward);
            }
            TSVector center = (curPosition + lastPosition) / 2;
            TSVector dir = curPosition - lastPosition;
            FP len = dir.magnitude;
            dir.Normalize();
            //检测lastPosition到curPosition这条之间碰撞到的代理
            for (int i = 0; i < lst.Count; i++)
            {
                var agentObj = lst[i];
                if (agentObj.gameCollider != null)
                {
                    TSVector hitPoint;
                    if (agentObj.gameCollider.CheckRect(center, dir, m_cLastRectToCurRectData.width / 2,len / 2,out hitPoint))
                    {
                        SelectAgentObjInfo info = new SelectAgentObjInfo();
                        info.agentObj = agentObj;
                        info.hitPoint = hitPoint;
                        TSVector direct = agentObj.gameCollider.center - curPosition;
                        if (direct.IsZero()) direct = curForward;
                        info.hitDirect = direct;
                        result.Add(info);
                    }
                }
            }
        }
    }
}

