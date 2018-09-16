using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BTCore;
using Framework;
using NodeEditor;
using UnityEngine;

namespace Game
{
    public class BGT_SelfRectSelectCompositeData : BTG_BaseSelectAgentObjCompositeData
    {
        [NEProperty("挂点(不填使用当前位置)")]
        public string hangPoint;
        [NEProperty("宽(为0的话不检测碰撞)")]
        public FP width;
        [NEProperty("高(为0的话不检测碰撞)")]
        public FP height;
        [NEProperty("使用挂点方向")]
        public bool useHangPointDirect;
    }
    [BTGameNode(typeof(BGT_SelfRectSelectCompositeData))]
    public class BGT_SelfRectSelectComposite : BTG_BaseSelectAgentObjComposite
    {
        private BGT_SelfRectSelectCompositeData m_cSelfRectSelectData;
        protected override void OnInitData(object data)
        {
            base.OnInitData(data);
            m_cSelfRectSelectData = data as BGT_SelfRectSelectCompositeData;
        }


        protected override void OnSelectChild(AgentObjectBlackBoard blackBoard, List<AgentObject> lst, ref List<SelectAgentObjInfo> result)
        {
            if (lst.Count == 0 || m_cSelfRectSelectData.width <= 0 || m_cSelfRectSelectData.height <= 0) return;
            AgentObject host = blackBoard.host;
            TSVector curPosition = host.curPosition;
            TSVector curForward = host.curForward;
            if (!string.IsNullOrEmpty(m_cSelfRectSelectData.hangPoint))
            {
                blackBoard.host.GetHangPoint(m_cSelfRectSelectData.hangPoint, host.curPosition, host.curForward, out curPosition, out curForward);
            }
            if(!m_cSelfRectSelectData.useHangPointDirect)
            {
                curForward = host.curForward;
            }
            TSVector center = curPosition + curForward * m_cSelfRectSelectData.height / 2;
            //检测lastPosition到curPosition这条之间碰撞到的代理
            for (int i = 0; i < lst.Count; i++)
            {
                var agentObj = lst[i];
                if (agentObj.gameCollider != null)
                {
                    if (agentObj.gameCollider.CheckRect(center, curForward,m_cSelfRectSelectData.width / 2,m_cSelfRectSelectData.height / 2))
                    {
                        SelectAgentObjInfo info = new SelectAgentObjInfo();
                        info.agentObj = agentObj;
                        info.hitPoint = agentObj.curPosition;
                        info.hitDirect = host.curForward;
                        result.Add(info);
                    }
                    //Vector3 uCenter = center.ToUnityVector3();
                    //Vector3 uForward = curForward.ToUnityVector3();
                    //float nHalfWidth = (m_cSelfRectSelectData.width / 2).AsFloat();
                    //float nHalfHeight = (m_cSelfRectSelectData.height / 2).AsFloat();
                    //GizmosUtility.instance.BeginDrawGizmos(0f, () => {
                    //    UnityEngine.Gizmos.color = Color.red;
                    //    GizmosUtility.instance.DrawRect(uCenter, uForward, nHalfWidth, nHalfHeight);
                    //},false);
                }
            }
        }
    }
}
