using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BTCore;
using Framework;
using NodeEditor;

namespace Game
{
    public class BTG_SelfColliderSelectCompositeData : BTG_BaseSelectAgentObjCompositeData
    {
    }
    [BTGameNode(typeof(BTG_SelfColliderSelectCompositeData))]
    public class BTG_SelfColliderSelectComposite : BTG_BaseSelectAgentObjComposite
    {
        private BTG_SelfColliderSelectCompositeData m_cSelfColliderSelectData;
        protected override void OnInitData(object data)
        {
            base.OnInitData(data);
            m_cSelfColliderSelectData = data as BTG_SelfColliderSelectCompositeData;
        }


        protected override void OnSelectChild(AgentObjectBlackBoard blackBoard, List<AgentObject> lst, ref List<SelectAgentObjInfo> result)
        {
            if (lst.Count == 0) return;
            AgentObject host = blackBoard.host;
            //检测lastPosition到curPosition这条之间碰撞到的代理
            for (int i = 0; i < lst.Count; i++)
            {
                var agentObj = lst[i];
                if (agentObj.gameCollider != null)
                {
                    if (host.gameCollider.CheckCollider(agentObj.gameCollider))
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
