using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework;
using NodeEditor;
using BTCore;

namespace Game
{
    public class GL_PathRandomRefreshData
    {
        [NEProperty("触发时间", true)]
        public FP time;
        [NEPropertyBtn("打开选择单位编辑器", "NENodeFuncExt", "ShowSelectMultiAirShipWindow")]
        [NEProperty("配置ID列表")]
        public int[] configIds;
        [NEProperty("刷新对象类型")]
        public UnitType unitType;
        [NEProperty("刷新对象阵营")]
        public CampType campType;
        [NEPropertyBtn("打开路径编辑器", "NENodeFuncExt", "ShowPathEditorWindow")]
        [NEProperty("移动路径")]
        public TSVector[] points;
        [NEProperty("刷新次数")]
        public int refreshTimes;
        [NEProperty("刷新间隔时间")]
        public FP refreshSpaceTime;
        [NEProperty("销毁时机")]
        public UnitDestoryType[] destoryTypes;
    }
    [GamingNode(typeof(GL_PathRandomRefreshData))]
    [NENodeDesc("随机刷新对象，并在路径上移动")]
    public class GL_PathRandomRefresh : BaseTimeLineGamingAction
    {
        private GL_PathRandomRefreshData m_cRefreshData;
        public override FP time
        {
            get
            {
                return m_cRefreshData.time;
            }
        }

        private int m_nRefreshTimes;
        private FP m_nRefreshSpaceTime;

        protected override void OnInitData(object data)
        {
            base.OnInitData(data);
            m_cRefreshData = data as GL_PathRandomRefreshData;
        }

        protected override void OnEnter(GamingBlackBoard blackBoard)
        {
            m_nRefreshTimes = m_cRefreshData.refreshTimes;
            m_nRefreshSpaceTime = m_cRefreshData.refreshSpaceTime;
        }

        private void Refresh()
        {
            if (m_cRefreshData.points.Length > 0 && m_cRefreshData.configIds != null && m_cRefreshData.configIds.Length > 0)
            {
                TSVector bornPos = m_cRefreshData.points[0];
                TSVector bornForward = TSVector.forward;
                if (m_cRefreshData.points.Length > 1)
                {
                    var forward = m_cRefreshData.points[1] - m_cRefreshData.points[0];
                    if (forward != TSVector.zero)
                    {
                        forward.Normalize();
                        bornForward = forward;
                    }
                }
                int index = GameInTool.Random(m_cRefreshData.configIds.Length);
                var unit = BattleScene.Instance.CreateUnit(m_cRefreshData.configIds[index], (int)m_cRefreshData.campType, m_cRefreshData.unitType, bornPos, bornForward);
                if (m_cRefreshData.points.Length > 1)
                {
                    var lst = ResetObjectPool<List<TSVector>>.Instance.GetObject();
                    var firstPoint = m_cRefreshData.points[0];
                    for (int i = 1; i < m_cRefreshData.points.Length; i++)
                    {
                        lst.Add(unit.curPosition + m_cRefreshData.points[i] - firstPoint);
                    }
                    unit.Move(lst,MoveFromType.Game);
                    ResetObjectPool<List<TSVector>>.Instance.SaveObject(lst);
                }
                unit.StartAI();
                if (m_cRefreshData.destoryTypes != null)
                {
                    for (int i = 0; i < m_cRefreshData.destoryTypes.Length; i++)
                    {
                        GlobalEventDispatcher.Instance.DispatchByParam(GameEvent.AddUnitDestory, m_cRefreshData.destoryTypes[i], unit);
                    }
                }
            }
        }

        public override BTActionResult OnRun(GamingBlackBoard blackBoard)
        {
            if (m_nRefreshTimes > 0)
            {
                if (m_nRefreshSpaceTime >= m_cRefreshData.refreshSpaceTime)
                {
                    m_nRefreshSpaceTime -= m_cRefreshData.refreshSpaceTime;
                    m_nRefreshTimes--;
                    Refresh();
                }
                m_nRefreshSpaceTime += blackBoard.deltaTime;
            }
            if (m_nRefreshSpaceTime > 0)
            {
                return BTActionResult.Running;
            }
            else
            {
                return BTActionResult.Ready;
            }
        }
    }
}

