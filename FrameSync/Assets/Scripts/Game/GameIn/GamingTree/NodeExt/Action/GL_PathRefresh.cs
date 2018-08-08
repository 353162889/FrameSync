using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework;
using NodeEditor;
using BTCore;

namespace Game
{
    public class GL_PathRefreshData
    {
        [NEProperty("触发时间", true)]
        public FP time;
        [NEProperty("配置ID")]
        public int configId;
        [NEProperty("刷新对象类型")]
        public UnitType unitType;
        [NEProperty("刷新对象阵营")]
        public CampType campType;
        [NEProperty("移动结束时销毁")]
        public bool pathEndDestory;
        [NEPropertyBtn("打开路径编辑器", "NENodeFuncExt", "ShowPathEditorWindow")]
        [NEProperty("移动路径")]
        public TSVector[] points;
    }
    [GamingNode(typeof(GL_PathRefreshData))]
    [NENodeDesc("刷新对象，并在路径上移动")]
    public class GL_PathRefresh : BaseTimeLineGamingAction
    {
        private GL_PathRefreshData m_cRefreshData;
        public override FP time
        {
            get
            {
                return m_cRefreshData.time;
            }
        }

        private bool m_moveEnd;
        private Unit m_cUnit;

        protected override void OnInitData(object data)
        {
            base.OnInitData(data);
            m_cRefreshData = data as GL_PathRefreshData;
        }

        protected override void OnEnter(GamingBlackBoard blackBoard)
        {
            m_moveEnd = false;
            if (m_cRefreshData.points == null || m_cRefreshData.points.Length == 0)
            {
                m_moveEnd = true;
            }
            if (!m_moveEnd)
            {
                TSVector bornPos = m_cRefreshData.points[0];
                TSVector bornForward = TSVector.forward;
                if(m_cRefreshData.points.Length > 1)
                {
                    var forward = m_cRefreshData.points[1] - m_cRefreshData.points[0];
                    if(forward != TSVector.zero)
                    {
                        forward.Normalize();
                        bornForward = forward;
                    }
                }
                m_cUnit = BattleScene.Instance.CreateUnit(m_cRefreshData.configId, (int)m_cRefreshData.campType, m_cRefreshData.unitType, bornPos, bornForward);
                m_cUnit.OnUnitDie += OnUnitDie;
                if (m_cRefreshData.points.Length > 1)
                {
                    var lst = ResetObjectPool<List<TSVector>>.Instance.GetObject();
                    var firstPoint = m_cRefreshData.points[0];
                    for (int i = 1; i < m_cRefreshData.points.Length; i++)
                    {
                        lst.Add(m_cUnit.curPosition + m_cRefreshData.points[i] - firstPoint);
                    }
                    m_cUnit.Move(lst);
                    m_cUnit.OnUnitMoveStop += OnOneMoveStop;
                    ResetObjectPool<List<TSVector>>.Instance.SaveObject(lst);
                }
            }
            
            base.OnEnter(blackBoard);
        }

        private void OnUnitDie(Unit unit, DamageInfo damageInfo)
        {
            m_moveEnd = true;
        }

        private void OnOneMoveStop(TSVector position, TSVector forward)
        {
            m_moveEnd = true;
        }

        public override BTActionResult OnRun(GamingBlackBoard blackBoard)
        {
            if (m_moveEnd)
            {
                return BTActionResult.Ready;
            }
            else
            {
                if (m_cUnit != null && !m_cUnit.isMoving) return BTActionResult.Ready;
                return BTActionResult.Running;
            }
        }

        public override void OnExit(GamingBlackBoard blackBoard)
        {
            if(m_cUnit != null)
            {
                m_cUnit.OnUnitDie -= OnUnitDie;
                m_cUnit.OnUnitMoveStop -= OnOneMoveStop;
                if(m_cRefreshData.pathEndDestory)
                {
                    BattleScene.Instance.DestroyUnit(m_cUnit);
                    m_cUnit = null;
                }
            }
            base.OnExit(blackBoard);
        }

    }
}
