using Framework;
using NodeEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BTCore
{
    public class BTDebugData
    {
        [NEProperty("触发时间")]
        public FP time;
        [NEProperty("描述")]
        public string desc;
    }

    [BTNode(typeof(BTDebugData))]
    [NENodeDesc("打印日志")]
    public class BTDebug : BTAction, IBTTimeLineNode
    {
        private BTDebugData m_cDebugData;

        public FP time
        {
            get
            {
                return m_cDebugData.time;
            }
        }

        protected override void OnInitData(object data)
        {
            base.OnInitData(data);
            m_cDebugData = data as BTDebugData;
        }

        public override BTActionResult OnRun(BTBlackBoard blackBoard)
        {
            CLog.LogColorArgs(CLogColor.Red,m_cDebugData.desc);
            return BTActionResult.Ready;
           
        }
    }
}
