using BTCore;
using Framework;
using NodeEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
{

    public enum RemoteTargetType
    {
        Target,
        TargetPosition,
        TargetForward,
    }
    public class RemoteData
    {
    }

    [BTNode(typeof(RemoteData))]
    [NENodeDisplay(false, true, false)]
    [NENodeName("RemoteRoot")]
    public class RemoteTree : BTNode
    {
        private BTNode m_cChild;
        public override void AddChild(BTNode child)
        {
            if (m_cChild != null)
            {
                CLog.LogError("RemoteTree has exist child node! add has override it");
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
