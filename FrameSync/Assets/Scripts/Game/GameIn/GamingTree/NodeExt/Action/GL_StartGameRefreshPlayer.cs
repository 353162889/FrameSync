using UnityEngine;
using System.Collections;
using Game;
using Framework;
using NodeEditor;
using BTCore;
using System.Collections.Generic;

namespace Game
{
    public class GL_StartGameRefreshPlayerData
    {
        [NEProperty("触发时间", true)]
        public FP time;
        [NEPropertyBtn("打开路径编辑器", "NENodeFuncExt", "ShowPathEditorWindow")]
        [NEProperty("移动路径")]
        public TSVector[] points;
    }
    [GamingNode(typeof(GL_StartGameRefreshPlayerData))]
    [NENodeDesc("开始游戏刷新玩家并移动到指定位置")]
    public class GL_StartGameRefreshPlayer : BaseTimeLineGamingAction
    {
        private GL_StartGameRefreshPlayerData m_cRefreshData;
        public override FP time
        {
            get
            {
                return m_cRefreshData.time;
            }
        }

        private uint m_bForbidMoveId;
        private uint m_bForbidSkillId;
        private uint m_bForbidForwardId;

        protected override void OnInitData(object data)
        {
            base.OnInitData(data);
            m_cRefreshData = data as GL_StartGameRefreshPlayerData;
        }

        protected override void OnEnter(GamingBlackBoard blackBoard)
        {
            var lstPlayer = PvpPlayerMgr.Instance.lstPlayer;
            if (m_cRefreshData.points.Length > 0)
            {
                for (int i = 0; i < lstPlayer.Count; i++)
                {
                    var player = lstPlayer[i];
                    if (!player.initUnit)
                    {
                        var unit = player.CreatePlayerUnit(m_cRefreshData.points[0]);
                        player.SetBornPos(m_cRefreshData.points[m_cRefreshData.points.Length - 1]);
                        m_bForbidMoveId = unit.Forbid(UnitForbidType.ForbidPlayerMove, UnitForbidFromType.Game);
                        m_bForbidSkillId = unit.Forbid(UnitForbidType.ForbidPlayerSkill, UnitForbidFromType.Game);
                        m_bForbidForwardId = unit.Forbid(UnitForbidType.ForbidPlayerForward, UnitForbidFromType.Game);

                        if (m_cRefreshData.points.Length > 1)
                        {
                            var lst = ResetObjectPool<List<TSVector>>.Instance.GetObject();
                            var firstPoint = m_cRefreshData.points[0];
                            for (int j = 1; j < m_cRefreshData.points.Length; j++)
                            {
                                lst.Add(unit.curPosition + m_cRefreshData.points[j] - firstPoint);
                            }
                            unit.Move(lst,MoveFromType.Game);
                            ResetObjectPool<List<TSVector>>.Instance.SaveObject(lst);
                        }
                    }
                }
            }
        }

        protected override BTActionResult OnRun(GamingBlackBoard blackBoard)
        {
            var lstPlayer = PvpPlayerMgr.Instance.lstPlayer;
            for (int i = 0; i < lstPlayer.Count; i++)
            {
                var player = lstPlayer[i];
                if (player.unit != null && player.unit.isMoving) return BTActionResult.Running;
            }
            for (int i = 0; i < lstPlayer.Count; i++)
            {
                var player = lstPlayer[i];
                player.unit.Forbid(UnitForbidType.ForbidForward, UnitForbidFromType.Game);
                player.unit.Resume(m_bForbidMoveId);
                player.unit.Resume(m_bForbidSkillId);
                player.unit.Resume(m_bForbidForwardId);
            }
            return BTActionResult.Ready;
        }

        protected override void OnExit(GamingBlackBoard blackBoard)
        {
            m_bForbidMoveId = 0;
            m_bForbidSkillId = 0;
            m_bForbidForwardId = 0;
            base.OnExit(blackBoard);
        }
    }
}