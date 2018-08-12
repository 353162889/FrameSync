using BTCore;
using Framework;
using NodeEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
{
    public class GL_PointRefreshData
    {
        [NEProperty("触发时间", true)]
        public FP time;
        [NEProperty("配置ID")]
        public int configId;
        [NEProperty("刷新对象类型")]
        public UnitType unitType;
        [NEProperty("刷新对象阵营")]
        public CampType campType;
        [NEPropertyBtn("打开路径编辑器", "NENodeFuncExt", "ShowPathEditorWindow")]
        [NEProperty("刷新点")]
        public TSVector point;
        [NEProperty("刷新方向")]
        public TSVector forward;
        [NEProperty("刷新次数")]
        public int refreshTimes;
        [NEProperty("刷新间隔时间")]
        public FP refreshSpaceTime;
    }
    [GamingNode(typeof(GL_PointRefreshData))]
    [NENodeDesc("在位置刷新对象")]
    public class GL_PointRefresh : BaseTimeLineGamingAction
    {
        private GL_PointRefreshData m_cRefreshData;
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
            m_cRefreshData = data as GL_PointRefreshData;
        }

        protected override void OnEnter(GamingBlackBoard blackBoard)
        {
            m_nRefreshTimes = m_cRefreshData.refreshTimes;
            m_nRefreshSpaceTime = m_cRefreshData.refreshSpaceTime;
        }

        public override BTActionResult OnRun(GamingBlackBoard blackBoard)
        {
            if (m_nRefreshTimes > 0)
            {
                if (m_nRefreshSpaceTime >= m_cRefreshData.refreshSpaceTime)
                {
                    m_nRefreshSpaceTime -= m_cRefreshData.refreshSpaceTime;
                    m_nRefreshTimes--;
                    var unit = BattleScene.Instance.CreateUnit(m_cRefreshData.configId, (int)m_cRefreshData.campType, m_cRefreshData.unitType, m_cRefreshData.point, m_cRefreshData.forward);
                    unit.StartAI();
                }
                m_nRefreshSpaceTime += blackBoard.deltaTime;
            }
            if(m_nRefreshSpaceTime > 0)
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
