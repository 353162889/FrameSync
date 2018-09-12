using UnityEngine;
using System.Collections;
using Game;
using System.Collections.Generic;
using Framework;
using NodeEditor;

public enum PlayerSelectType
{
    Random,
    MinDistance,
    MinHp
}

public class BTG_PlayerSelectCompositeData : BTG_BaseSelectAgentObjCompositeData
{
    [NEProperty("选择玩家方式")]
    public PlayerSelectType eSelectType;
}
[BTGameNode(typeof(BTG_PlayerSelectCompositeData))]
[NENodeDesc("选择玩家单位代理")]
public class BTG_PlayerSelectComposite : BTG_BaseSelectAgentObjComposite
{
    private BTG_PlayerSelectCompositeData m_cPlayerSelectData;
    protected override void OnInitData(object data)
    {
        base.OnInitData(data);
        m_cPlayerSelectData = data as BTG_PlayerSelectCompositeData;
    }
    protected override void OnSelectChild(AgentObjectBlackBoard blackBoard, List<AgentObject> lst, ref List<SelectAgentObjInfo> result)
    {
        var lstPlayer = PvpPlayerMgr.Instance.lstPlayer;
        if(m_cPlayerSelectData.eSelectType == PlayerSelectType.Random)
        {
            PvpPlayer player = lstPlayer[GameInTool.Random(lstPlayer.Count)];
            if (player.unit != null && !player.unit.isDie)
            {
                SelectAgentObjInfo info = new SelectAgentObjInfo();
                info.agentObj = player.unit.agentObj;
                info.hitPoint = player.unit.curPosition;
                info.hitDirect = player.unit.curForward;
                result.Add(info);
            }
        }
        else if(m_cPlayerSelectData.eSelectType == PlayerSelectType.MinHp)
        {
            FP minHpSqr = FP.MaxValue;
            Unit unit = null;
            for (int i = 0; i < lstPlayer.Count; i++)
            {
                var player = lstPlayer[i];
                if (player.unit == null || player.unit.isDie) continue;
                if (player.unit.hp < minHpSqr)
                {
                    minHpSqr = player.unit.hp;
                    unit = player.unit;
                }
            }
            if(unit != null)
            {
                SelectAgentObjInfo info = new SelectAgentObjInfo();
                info.agentObj = unit.agentObj;
                info.hitPoint = unit.curPosition;
                info.hitDirect = unit.curForward;
                result.Add(info);
            }
        }
        else if(m_cPlayerSelectData.eSelectType == PlayerSelectType.MinDistance)
        {
            var curPos = blackBoard.host.curPosition;
            FP minDisSqr = FP.MaxValue;
            Unit unit = null;
            for (int i = 0; i < lstPlayer.Count; i++)
            {
                var player = lstPlayer[i];
                if (player.unit == null || player.unit.isDie) continue;
                FP disSqr = (player.unit.curPosition - curPos).sqrMagnitude;
                if (disSqr < minDisSqr)
                {
                    minDisSqr = disSqr;
                    unit = player.unit;
                }
            }
            if (unit != null)
            {
                SelectAgentObjInfo info = new SelectAgentObjInfo();
                info.agentObj = unit.agentObj;
                info.hitPoint = unit.curPosition;
                info.hitDirect = unit.curForward;
                result.Add(info);
            }
        }
    }
}
