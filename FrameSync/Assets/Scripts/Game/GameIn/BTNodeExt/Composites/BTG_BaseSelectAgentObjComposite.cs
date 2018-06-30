using Framework;
using NodeEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BTCore;

namespace Game
{
    public enum SelectAgentObjectType
    {
        Self,

        UnitFriend,
        UnitFriendOutSelf,
        UnitEnemy,

        RemoteFriend,
        RemoteFriendOutSelf,
        RemoteEnemy,

        UnitAndRemoteFriend,
        UnitAndRemoteFriendOutSelf,
        UnitAndRemoteEnemy,
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
        [NEProperty("选择的对象类型")]
        public SelectAgentObjectType selectType;
    }

    public struct SelectAgentObjInfo
    {
        public AgentObject agentObj;    //当前选择代理对象
        public TSVector hitPoint;       //当前选择点
        public TSVector hitDirect;      //当前选择方向
        public int agentObjCount;       //当前选择的代理数量

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

        private static Dictionary<int, Action<AgentObject,List<AgentObject>>> m_dicSelectFunc;

        private BTG_BaseSelectAgentObjCompositeData m_cCompositeData;
        public override FP time { get { return m_cCompositeData.time; } }
        protected bool m_bIsEnd;
        protected FP m_sTime;
        protected FP m_sLastSelectTime;
        protected List<SelectAgentObjCountInfo> m_lstSelectInfo = new List<SelectAgentObjCountInfo>();
        protected int m_nMaxSelectCount;

        #region 选择函数
        private static void SelectSelf(AgentObject host, List<AgentObject> lstAgentObj)
        {
            lstAgentObj.Add(host);
        }
        private static void SelectUnitFriend(AgentObject host, List<AgentObject> lstAgentObj)
        {
            var field = BattleScene.Instance.GetField(host.campId, AgentObjectType.Unit);
            for (int i = 0; i < field.lstFriend.Count; i++)
            {
                lstAgentObj.Add(field.lstFriend[i]);
            }
        }
        private static void SelectUnitFriendOutSelf(AgentObject host, List<AgentObject> lstAgentObj)
        {
            var field = BattleScene.Instance.GetField(host.campId, AgentObjectType.Unit);
            for (int i = 0; i < field.lstFriend.Count; i++)
            {
                if (field.lstFriend[i] == host) continue;
                lstAgentObj.Add(field.lstFriend[i]);
            }
        }
        private static void SelectUnitEnemy(AgentObject host, List<AgentObject> lstAgentObj)
        {
            var field = BattleScene.Instance.GetField(host.campId, AgentObjectType.Unit);
            for (int i = 0; i < field.lstEnemy.Count; i++)
            {
                lstAgentObj.Add(field.lstEnemy[i]);
            }
        }
        private static void SelectRemoteFriend(AgentObject host, List<AgentObject> lstAgentObj)
        {
            var field = BattleScene.Instance.GetField(host.campId, AgentObjectType.Remote);
            for (int i = 0; i < field.lstFriend.Count; i++)
            {
                lstAgentObj.Add(field.lstFriend[i]);
            }
        }
        private static void SelectRemoteFriendOutSelf(AgentObject host, List<AgentObject> lstAgentObj)
        {
            var field = BattleScene.Instance.GetField(host.campId, AgentObjectType.Remote);
            for (int i = 0; i < field.lstFriend.Count; i++)
            {
                if (field.lstFriend[i] == host) continue;
                lstAgentObj.Add(field.lstFriend[i]);
            }
        }
        private static void SelectRemoteEnemy(AgentObject host, List<AgentObject> lstAgentObj)
        {
            var field = BattleScene.Instance.GetField(host.campId, AgentObjectType.Remote);
            for (int i = 0; i < field.lstEnemy.Count; i++)
            {
                lstAgentObj.Add(field.lstEnemy[i]);
            }
        }
        private static void SelectUnitAndRemoteFriend(AgentObject host, List<AgentObject> lstAgentObj)
        {
            var field = BattleScene.Instance.GetField(host.campId, AgentObjectType.Unit);
            for (int i = 0; i < field.lstFriend.Count; i++)
            {
                lstAgentObj.Add(field.lstFriend[i]);
            }
            field = BattleScene.Instance.GetField(host.campId, AgentObjectType.Remote);
            for (int i = 0; i < field.lstFriend.Count; i++)
            {
                lstAgentObj.Add(field.lstFriend[i]);
            }
        }
        private static void SelectUnitAndRemoteFriendOutSelf(AgentObject host, List<AgentObject> lstAgentObj)
        {
            var field = BattleScene.Instance.GetField(host.campId, AgentObjectType.Unit);
            for (int i = 0; i < field.lstFriend.Count; i++)
            {
                if (field.lstFriend[i] == host) continue;
                lstAgentObj.Add(field.lstFriend[i]);
            }
            field = BattleScene.Instance.GetField(host.campId, AgentObjectType.Remote);
            for (int i = 0; i < field.lstFriend.Count; i++)
            {
                if (field.lstFriend[i] == host) continue;
                lstAgentObj.Add(field.lstFriend[i]);
            }
        }
        private static void SelectUnitAndRemoteEnemy(AgentObject host, List<AgentObject> lstAgentObj)
        {
            var field = BattleScene.Instance.GetField(host.campId, AgentObjectType.Unit);
            for (int i = 0; i < field.lstEnemy.Count; i++)
            {
                lstAgentObj.Add(field.lstEnemy[i]);
            }
            field = BattleScene.Instance.GetField(host.campId, AgentObjectType.Remote);
            for (int i = 0; i < field.lstEnemy.Count; i++)
            {
                lstAgentObj.Add(field.lstEnemy[i]);
            }
        }
        #endregion
        static BTG_BaseSelectAgentObjComposite()
        {
            ResetObjectPool<List<SelectAgentObjInfo>>.Instance.Init(5, (List<SelectAgentObjInfo> lst) => { lst.Clear(); });
            m_dicSelectFunc = new Dictionary<int, Action<AgentObject,List<AgentObject>>>();
            m_dicSelectFunc.Add((int)SelectAgentObjectType.Self, SelectSelf);
            m_dicSelectFunc.Add((int)SelectAgentObjectType.UnitFriend, SelectUnitFriend);
            m_dicSelectFunc.Add((int)SelectAgentObjectType.UnitFriendOutSelf, SelectUnitFriendOutSelf);
            m_dicSelectFunc.Add((int)SelectAgentObjectType.UnitEnemy, SelectUnitEnemy);
            m_dicSelectFunc.Add((int)SelectAgentObjectType.RemoteFriend, SelectRemoteFriend);
            m_dicSelectFunc.Add((int)SelectAgentObjectType.RemoteFriendOutSelf, SelectRemoteFriendOutSelf);
            m_dicSelectFunc.Add((int)SelectAgentObjectType.RemoteEnemy, SelectRemoteEnemy);
            m_dicSelectFunc.Add((int)SelectAgentObjectType.UnitAndRemoteFriend, SelectUnitAndRemoteFriend);
            m_dicSelectFunc.Add((int)SelectAgentObjectType.UnitAndRemoteFriendOutSelf, SelectUnitAndRemoteFriendOutSelf);
            m_dicSelectFunc.Add((int)SelectAgentObjectType.UnitAndRemoteEnemy, SelectUnitAndRemoteEnemy);
        }
       

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
                bool succ = OnSelect(blackBoard);
                if(!succ)
                {
                    m_bIsEnd = true;
                    return BTResult.Success;
                }
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

        protected virtual void OnSelectChild(AgentObjectBlackBoard blackBoard,List<AgentObject> lst,ref List<SelectAgentObjInfo> result)
        {
        }

        protected virtual void OnExit(AgentObjectBlackBoard blackBoard)
        {

        }

        private bool OnSelect(AgentObjectBlackBoard blackBoard)
        {
            var lstAgentObj = ResetObjectPool<List<AgentObject>>.Instance.GetObject();
            m_dicSelectFunc[(int)m_cCompositeData.selectType].Invoke(blackBoard.host, lstAgentObj);
            var lstSelectInfo = ResetObjectPool<List<SelectAgentObjInfo>>.Instance.GetObject();
            var lstSelectInfoResult = ResetObjectPool<List<SelectAgentObjInfo>>.Instance.GetObject();
            OnSelectChild(blackBoard, lstAgentObj,ref lstSelectInfo);
            ResetObjectPool<List<AgentObject>>.Instance.SaveObject(lstAgentObj);
            for (int i = 0; i < lstSelectInfo.Count; i++)
            {
                var selectObjInfo = lstSelectInfo[i];
                if (m_nMaxSelectCount < m_cCompositeData.totalObjMaxCount)
                {
                    SelectAgentObjCountInfo selectInfo = new SelectAgentObjCountInfo(selectObjInfo.agentObj.id, selectObjInfo.agentObj.agentType, 0);
                    for (int j = 0; j < m_lstSelectInfo.Count; j++)
                    {
                        var info = m_lstSelectInfo[j];
                        if (info.id == selectObjInfo.agentObj.id && info.agentType == selectObjInfo.agentObj.agentType)
                        {
                            selectInfo = info;
                            break;
                        }
                    }
                    if (selectInfo.count < m_cCompositeData.oneObjMaxCount)
                    {
                        if (selectInfo.count == 0) m_lstSelectInfo.Add(selectInfo);
                        selectInfo.count++;
                        //通过选择器
                        lstSelectInfoResult.Add(selectObjInfo);

                    }
                }
            }
            for (int i = 0; i < lstSelectInfoResult.Count; i++)
            {
                var selectObjInfo = lstSelectInfoResult[i];
                selectObjInfo.agentObjCount = lstSelectInfoResult.Count;
                ExecuteChilds(blackBoard, selectObjInfo);
            }
            m_nMaxSelectCount += lstSelectInfoResult.Count;
            if (m_nMaxSelectCount >= m_cCompositeData.oneObjMaxCount)
            {
                return false;
            }
            ResetObjectPool<List<SelectAgentObjInfo>>.Instance.SaveObject(lstSelectInfo);
            ResetObjectPool<List<SelectAgentObjInfo>>.Instance.SaveObject(lstSelectInfoResult);
            return true;
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
