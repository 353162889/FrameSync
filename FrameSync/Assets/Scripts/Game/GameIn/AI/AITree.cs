using BTCore;
using Framework;
using NodeEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
{
    public class AIData
    {
        [NEPropertyKey]
        [NEProperty("ID")]
        public int id;
        [NEProperty("描述")]
        public string aiDesc;
    }
    [BTNode(typeof(AIData))]
    [NENodeDisplay(false, true, false)]
    [NENodeName("AIRoot")]
    public class AITree : BTNode
    {
        private BTNode m_cChild;
        public override void AddChild(BTNode child)
        {
            if (m_cChild != null)
            {
                CLog.LogError("AITree has exist child node! add has override it");
            }
            m_cChild = child;
        }


        public override BTResult OnTick(BTBlackBoard blackBoard)
        {
            if (m_cChild != null)
            {
                return m_cChild.OnTick(blackBoard);
            }
            return base.OnTick(blackBoard);
        }

        public override void Clear()
        {
            if (m_cChild != null)
            {
                m_cChild.Clear();
            }
        }
    }
}
