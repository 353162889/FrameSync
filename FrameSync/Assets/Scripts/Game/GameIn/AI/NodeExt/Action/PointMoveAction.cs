using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework;
using NodeEditor;

namespace Game
{
    public class PointMoveActionData
    {
        [NEProperty("触发时间", true)]
        public FP time;
        [NEPropertyBtn("打开路径编辑器", "NENodeFuncExt", "ShowPathEditorWindow")]
        [NEProperty("移动路径")]
        public TSVector[] lstPoint;
    }

    [AINode(typeof(PointMoveActionData))]
    public class PointMoveAction : BaseTimeLineAIAction
    {
        private PointMoveActionData m_cMoveData;

        public override FP time
        {
            get
            {
                return m_cMoveData.time;
            }
        }

        protected override void OnInitData(object data)
        {
            base.OnInitData(data);
            m_cMoveData = data as PointMoveActionData;
        }


    }
}
