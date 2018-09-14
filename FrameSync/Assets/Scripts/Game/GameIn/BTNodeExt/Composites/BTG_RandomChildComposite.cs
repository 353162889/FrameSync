using UnityEngine;
using Game;
using NodeEditor;
using Framework;
using BTCore;

public class BTG_RandomChildCompositeData
{
    [NEProperty("触发时间", true)]
    public FP time;
}

[BTGameNode(typeof(BTG_RandomChildCompositeData))]
[NENodeDesc("随机执行一个子节点，然后结束")]
public class BTG_RandomChildComposite : BaseTimeLineBTGameComposite
{
    private BTG_RandomChildCompositeData m_cRandomData;

    public override FP time
    {
        get
        {
            return m_cRandomData.time;
        }
    }

    private bool m_bIsEnd = true;
    private int m_nIdx = 0;

    protected override void OnInitData(object data)
    {
        base.OnInitData(data);
        m_cRandomData = data as BTG_RandomChildCompositeData;
    }

    public override BTResult OnTick(AgentObjectBlackBoard blackBoard)
    {
        if (m_lstChild.Count <= 0) return BTResult.Failure;
        if (m_bIsEnd)
        {
            m_bIsEnd = false;
            int totalCount = m_lstChild.Count;
            m_nIdx = GameInTool.Random(m_lstChild.Count);
            
        }
        var result = m_lstChild[m_nIdx].OnTick(blackBoard);
        if (result != BTResult.Running)
        {
            m_bIsEnd = true;
        }
        return result;
    }
}