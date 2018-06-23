using Framework;
using NodeEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BTCore;

namespace Game
{
    public enum SelectAgentObjType
    {

    }
    public class BTG_BaseSelectAgentObjCompositeData
    {
        [NEProperty("触发时间", true)]
        public FP time;
        [NEProperty("持续时间")]
        public FP totalDuration;
        [NEProperty("每次间隔时间(<0:每帧)")]
        public FP oneDuration;
        [NEProperty("最大选择次数")]
        public int totalObjMaxCount;
        [NEProperty("最大单一对象选择次数")]
        public int oneObjMaxCount;
    }

    public struct SelectAgentObjInfo
    {
        public AgentObject agentObj;
        public TSVector hitPoint;
        public TSVector hitDirect;

        public void Reset()
        {
            agentObj = null;
            hitPoint = TSVector.zero;
            hitDirect = TSVector.forward;
        }
    }

    public abstract class BTG_BaseSelectAgentObjComposite : BaseTimeLineBTGameComposite
    {
        public struct SelectAgentObjCountInfo
        {
            public static readonly SelectAgentObjCountInfo empty = new SelectAgentObjCountInfo(0,AgentObjectType.Unit,0);
            public uint id;
            public AgentObjectType agentType;
            public int count;

            public SelectAgentObjCountInfo(uint id, AgentObjectType type,int count)
            {
                this.id = id;
                this.agentType = type;
                this.count = count;
            }
        }
        private BTG_BaseSelectAgentObjCompositeData m_cCompositeData;
        public override FP time { get { return m_cCompositeData.time; } }
        protected bool m_bIsEnd;
        protected FP m_sTime;
        protected FP m_sLastSelectTime;
        protected List<SelectAgentObjCountInfo> m_lstSelectInfo = new List<SelectAgentObjCountInfo>();
        protected int m_nMaxSelectCount;

        protected override void OnInitData(object data)
        {
            base.OnInitData(data);
            m_cCompositeData = data as BTG_BaseSelectAgentObjCompositeData;
        }

        public override BTResult OnTick(AgentObjectBlackBoard blackBoard)
        {
            if(m_bIsEnd)
            {
                m_sTime = 0;
                m_sLastSelectTime = 0;
                m_nMaxSelectCount = 0;
                m_bIsEnd = false;
                m_lstSelectInfo.Clear();
                OnEnter(blackBoard);
            }
            if(m_sLastSelectTime >= m_cCompositeData.oneDuration)
            {
                if (m_cCompositeData.oneDuration > 0)
                {
                    m_sLastSelectTime -= m_cCompositeData.oneDuration;
                }
                else
                {
                    m_sLastSelectTime = 0;
                }
                OnSelect(blackBoard);
            }
            
            if(m_sTime >= m_cCompositeData.totalDuration)
            {
                m_bIsEnd = true;
                m_lstSelectInfo.Clear();
                OnExit(blackBoard);
                return BTResult.Success;
            }
            m_sTime += blackBoard.deltaTime;
            m_sLastSelectTime += blackBoard.deltaTime;
           
            return BTResult.Running;
        }

        protected virtual void OnEnter(AgentObjectBlackBoard blackBoard)
        {

        }

        protected virtual List<SelectAgentObjInfo> OnSelectChild(AgentObjectBlackBoard blackBoard)
        {
            return null;
        }

        protected virtual void OnExit(AgentObjectBlackBoard blackBoard)
        {

        }

        private void OnSelect(AgentObjectBlackBoard blackBoard)
        {
            var lst = OnSelectChild(blackBoard);
            if (lst == null || lst.Count == 0) return;
            for (int i = 0; i < lst.Count; i++)
            {
                var selectObjInfo = lst[i];
                if(m_nMaxSelectCount < m_cCompositeData.totalObjMaxCount)
                {
                    SelectAgentObjCountInfo selectInfo = new SelectAgentObjCountInfo(selectObjInfo.agentObj.id, selectObjInfo.agentObj.agentType,0);
                    for (int j = 0; j < m_lstSelectInfo.Count; j++)
                    {
                        var info = m_lstSelectInfo[j];
                        if(info.id == selectObjInfo.agentObj.id && info.agentType == selectObjInfo.agentObj.agentType)
                        {
                            selectInfo = info;
                            break;
                        }
                    }
                    if(selectInfo.count < m_cCompositeData.oneObjMaxCount)
                    {
                        if (selectInfo.count == 0) m_lstSelectInfo.Add(selectInfo);
                        selectInfo.count++;
                        //通过选择器
                        ExecuteChilds(blackBoard, selectObjInfo);
                    }
                }
            }
        }

        private void ExecuteChilds(AgentObjectBlackBoard blackBoard, SelectAgentObjInfo selectObjInfo)
        {
            blackBoard.selectAgentObjInfo = selectObjInfo;
            int count = m_lstChild.Count;
            for (int i = 0; i < count; i++)
            {
                m_lstChild[i].OnTick(blackBoard);
            }
            blackBoard.selectAgentObjInfo.Reset();
        }

        public override void Clear()
        {
            m_bIsEnd = true;
            m_sTime = 0;
            m_sLastSelectTime = 0;
            base.Clear();
        }
    }
}
