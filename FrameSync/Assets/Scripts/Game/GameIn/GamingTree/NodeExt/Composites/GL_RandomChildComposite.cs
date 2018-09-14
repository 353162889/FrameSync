using UnityEngine;
using Game;
using NodeEditor;
using Framework;
using BTCore;

public class GL_RandomChildCompositeData
{
    [NEProperty("触发时间", true)]
    public FP time;
    [NEProperty("每个子节点执行一次后至少等待多久才能执行下一个", true)]
    public FP onChildExeTime;
}

[GamingNode(typeof(GL_RandomChildCompositeData))]
[NENodeDesc("随机执行一个子节点，然后结束")]
public class GL_RandomChildComposite : BaseTimeLineGamingComposite
{
    private GL_RandomChildCompositeData m_cRandomData;

    public override FP time
    {
        get
        {
            return m_cRandomData.time;
        }
    }

    private bool m_bIsEnd = true;
    private int m_nIdx = 0;
    private FP[] m_arrChildExeLeaveTime;

    protected override void OnInitData(object data)
    {
        base.OnInitData(data);
        m_cRandomData = data as GL_RandomChildCompositeData;
    }

    public override BTResult OnTick(GamingBlackBoard blackBoard)
    {
        if (m_lstChild.Count <= 0) return BTResult.Failure;
        if(m_arrChildExeLeaveTime == null)
        {
            m_arrChildExeLeaveTime = new FP[m_lstChild.Count];
            for (int i = 0; i < m_arrChildExeLeaveTime.Length; i++)
            {
                m_arrChildExeLeaveTime[i] = 0;
            }
        }
        else
        {
            for (int i = 0; i < m_arrChildExeLeaveTime.Length; i++)
            {
                m_arrChildExeLeaveTime[i] -= blackBoard.deltaTime;
            }
        }
        if (m_bIsEnd)
        {
            
            int totalCount = m_lstChild.Count;
            m_nIdx = GameInTool.Random(m_lstChild.Count);
            if (m_arrChildExeLeaveTime[m_nIdx] <= 0)
            {
                m_bIsEnd = false;
                m_arrChildExeLeaveTime[m_nIdx] = m_cRandomData.onChildExeTime;
            }
            else
            {
                return BTResult.Running;
            }
        }
        var result = m_lstChild[m_nIdx].OnTick(blackBoard);
        if(result != BTResult.Running)
        {
            m_bIsEnd = true;
        }
        return result;
    }

    public override void Clear()
    {
        if(m_arrChildExeLeaveTime !=null)
        {
            for (int i = 0; i < m_arrChildExeLeaveTime.Length; i++)
            {
                m_arrChildExeLeaveTime[i] = 0;
            }
        }
        base.Clear();
    }
}