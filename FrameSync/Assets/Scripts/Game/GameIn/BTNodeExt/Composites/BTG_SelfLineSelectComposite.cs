using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BTCore;
using Framework;
using NodeEditor;

namespace Game
{
    public class BTG_SelfLineSelectCompositeData : BTG_BaseSelectAgentObjCompositeData
    {
        [NEProperty("挂点(不填使用当前位置)")]
        public string hangPoint;
        [NEProperty("长度(为0的话不检测碰撞)")]
        public FP length;
    }
    [BTGameNode(typeof(BTG_SelfLineSelectCompositeData))]
    public class BTG_SelfLineSelectComposite : BTG_BaseSelectAgentObjComposite
    {
        private BTG_SelfLineSelectCompositeData m_cSelfLineSelectData;
        private TSVector m_sOriginPosition;
        protected override void OnInitData(object data)
        {
            base.OnInitData(data);
            m_cSelfLineSelectData = data as BTG_SelfLineSelectCompositeData;
        }


        protected override void OnSelectChild(AgentObjectBlackBoard blackBoard, List<AgentObject> lst, ref List<SelectAgentObjInfo> result)
        {
            if (lst.Count == 0 || m_cSelfLineSelectData.length <= 0) return;
            AgentObject host = blackBoard.host;
            TSVector curPosition = host.curPosition;
            TSVector curForward = host.curForward;
            if (!string.IsNullOrEmpty(m_cSelfLineSelectData.hangPoint))
            {
                blackBoard.host.GetHangPoint(m_cSelfLineSelectData.hangPoint, host.curPosition, host.curForward, out curPosition, out curForward);
            }
            m_sOriginPosition = curPosition;
            //检测lastPosition到curPosition这条之间碰撞到的代理
            for (int i = 0; i < lst.Count; i++)
            {
                var agentObj = lst[i];
                if (agentObj.gameCollider != null)
                {
                    TSVector sCrossPoint;
                    if (agentObj.gameCollider.CheckLine(curPosition, curForward * m_cSelfLineSelectData.length, out sCrossPoint))
                    {
                        SelectAgentObjInfo info = new SelectAgentObjInfo();
                        info.agentObj = agentObj;
                        info.hitPoint = sCrossPoint;
                        info.hitDirect = curForward;
                        result.Add(info);
                    }

                    //Vector3 uCenter = curPosition.ToUnityVector3();
                    //Vector3 uForward = curForward.ToUnityVector3();
                    //float nHalfWidth = (m_cSelfRectSelectData.width / 2).AsFloat();
                    //float nHalfHeight = (m_cSelfRectSelectData.height / 2).AsFloat();
                    //GizmosUtility.instance.BeginDrawGizmos(0f, () =>
                    //{
                    //    UnityEngine.Gizmos.color = Color.red;
                    //    GizmosUtility.instance.DrawRect(uCenter, uForward, nHalfWidth, nHalfHeight);
                    //}, false);
                }
            }
            result.Sort(LengthSort);
        }
        protected int LengthSort(SelectAgentObjInfo a,SelectAgentObjInfo b)
        {
            FP aValue = (a.hitPoint - m_sOriginPosition).sqrMagnitude;
            FP bValue = (b.hitPoint - m_sOriginPosition).sqrMagnitude;
            if (aValue > bValue)
            {
                return 1;
            }
            else if(aValue == bValue)
            {
                return 0;
            }
            else
            {
                return -1;
            }
        }
    }
}
