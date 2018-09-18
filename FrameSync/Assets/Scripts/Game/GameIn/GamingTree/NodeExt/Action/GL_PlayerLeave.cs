using UnityEngine;
using System.Collections;
using Game;
using Framework;
using NodeEditor;
using BTCore;
using System.Collections.Generic;

namespace Game
{
    public class GL_PlayerLeaveData
    {
        [NEProperty("触发时间", true)]
        public FP time;
        [NEPropertyBtn("打开路径编辑器", "NENodeFuncExt", "ShowPathEditorWindow")]
        [NEProperty("移动路径")]
        public TSVector[] points;
    }
    [GamingNode(typeof(GL_PlayerLeaveData))]
    [NENodeDesc("战斗结束，玩家们离开")]
    public class GL_PlayerLeave : BaseTimeLineGamingAction
    {
        private GL_PlayerLeaveData m_cLeaveData;
        public override FP time
        {
            get
            {
                return m_cLeaveData.time;
            }
        }

        protected override void OnInitData(object data)
        {
            base.OnInitData(data);
            m_cLeaveData = data as GL_PlayerLeaveData;
        }

        protected override void OnEnter(GamingBlackBoard blackBoard)
        {
            var lstPlayer = PvpPlayerMgr.Instance.lstPlayer;
            for (int i = 0; i < lstPlayer.Count; i++)
            {
                var player = lstPlayer[i];
                var unit = player.unit;
                if (unit == null) continue;
                unit.Forbid(UnitForbidType.ForbidPlayerMove,UnitForbidFromType.Game);
                unit.Forbid(UnitForbidType.ForbidPlayerForward, UnitForbidFromType.Game);
                unit.Forbid(UnitForbidType.ForbidPlayerSkill, UnitForbidFromType.Game);
                if (m_cLeaveData.points.Length > 1)
                {
                    var lst = ResetObjectPool<List<TSVector>>.Instance.GetObject();
                    var firstPoint = m_cLeaveData.points[0];
                    for (int j = 1; j < m_cLeaveData.points.Length; j++)
                    {
                        lst.Add(unit.curPosition + m_cLeaveData.points[j] - firstPoint);
                    }
                    unit.Move(lst, MoveFromType.Game);
                    ResetObjectPool<List<TSVector>>.Instance.SaveObject(lst);
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
            return BTActionResult.Ready;
        }
    }
}
