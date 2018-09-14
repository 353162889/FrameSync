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
        [NEPropertyBtn("打开选择单位列表编辑器", "NENodeFuncExt", "ShowSelectMultiAirShipWindow")]
        [NEProperty("配置ID列表")]
        public int[] configIds;
        [NEProperty("是否随机刷新配置ID")]
        public bool isRandomCfgIds;
        [NEProperty("刷新对象类型")]
        public UnitType unitType;
        [NEProperty("刷新对象阵营")]
        public CampType campType;
        [NEPropertyBtn("打开刷新点编辑器", "NENodeFuncExt", "ShowPointEditorWindow")]
        [NEProperty("刷新点列表")]
        public TSVector[] points;
        [NEProperty("是否随机刷新随机点")]
        public bool isRandomPoints;
        [NEPropertyBtn("打开路径编辑器", "NENodeFuncExt", "ShowPathEditorWindow")]
        [NEProperty("出场移动路径")]
        public TSVector[] joinScenePaths;
        [NEPropertyBtn("打开路径编辑器", "NENodeFuncExt", "ShowPathEditorWindow")]
        [NEProperty("离场移动路径")]
        public TSVector[] leaveScenePaths;
        [NEProperty("刷新次数")]
        public int refreshTimes;
        [NEProperty("刷新间隔时间")]
        public FP refreshSpaceTime;
        [NEProperty("销毁时机")]
        public UnitDestoryType[] destoryTypes;
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

        private int m_nRefreshTimes;
        private FP m_nRefreshSpaceTime;
        private int m_nCurCfgIdx;
        private int m_nCurPointIdx;

        protected override void OnInitData(object data)
        {
            base.OnInitData(data);
            m_cRefreshData = data as GL_PathRefreshData;
        }

        protected override void OnEnter(GamingBlackBoard blackBoard)
        {
            m_nRefreshTimes = m_cRefreshData.refreshTimes;
            m_nRefreshSpaceTime = m_cRefreshData.refreshSpaceTime;
            m_nCurCfgIdx = m_cRefreshData.isRandomCfgIds ? GameInTool.Random(m_cRefreshData.configIds.Length) : 0;
            m_nCurPointIdx = m_cRefreshData.isRandomPoints ? GameInTool.Random(m_cRefreshData.points.Length) : 0;
        }

        private void Refresh()
        {
            int refreshCfgId = m_cRefreshData.configIds[m_nCurCfgIdx];
            TSVector refreshPoint = m_cRefreshData.points[m_nCurPointIdx];
            TSVector[] refreshPath = m_cRefreshData.joinScenePaths;
            TSVector refreshForward = TSVector.forward;
            if (m_cRefreshData.joinScenePaths != null && m_cRefreshData.joinScenePaths.Length > 0)
            {
                if (m_cRefreshData.joinScenePaths.Length > 1)
                {
                    var forward = m_cRefreshData.joinScenePaths[1] - m_cRefreshData.joinScenePaths[0];
                    if (forward != TSVector.zero)
                    {
                        forward.Normalize();
                        refreshForward = forward;
                    }
                }
                else
                {
                    var forward = m_cRefreshData.joinScenePaths[0] - refreshPoint;
                    if (forward != TSVector.zero)
                    {
                        forward.Normalize();
                        refreshForward = forward;
                    }
                }
            }
            var unit = BattleScene.Instance.CreateUnit(refreshCfgId, (int)m_cRefreshData.campType, m_cRefreshData.unitType, refreshPoint, refreshForward);
           
            if(m_cRefreshData.joinScenePaths != null && m_cRefreshData.joinScenePaths.Length > 1)
            {
                unit.SetAIVariable(GameConst.AIJoinScenePathName, (SharedTSVectorArray)m_cRefreshData.joinScenePaths);
            }
            if(m_cRefreshData.leaveScenePaths != null && m_cRefreshData.leaveScenePaths.Length > 1)
            {
                unit.SetAIVariable(GameConst.AILeaveScenePathName, (SharedTSVectorArray)m_cRefreshData.leaveScenePaths);
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

        protected override BTActionResult OnRun(GamingBlackBoard blackBoard)
        {
            if (m_nRefreshTimes > 0)
            {
                if (m_nRefreshSpaceTime >= m_cRefreshData.refreshSpaceTime)
                {
                    m_nRefreshSpaceTime -= m_cRefreshData.refreshSpaceTime;
                    m_nRefreshTimes--;
                    Refresh();
                    m_nCurCfgIdx = m_cRefreshData.isRandomCfgIds ? GameInTool.Random(m_cRefreshData.configIds.Length) : (m_nCurCfgIdx + 1) % m_cRefreshData.configIds.Length;
                    m_nCurPointIdx = m_cRefreshData.isRandomPoints ? GameInTool.Random(m_cRefreshData.points.Length) : (m_nCurPointIdx + 1) % m_cRefreshData.points.Length;
                }
                m_nRefreshSpaceTime += blackBoard.deltaTime;
            }
            else
            {
                return BTActionResult.Ready;
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
