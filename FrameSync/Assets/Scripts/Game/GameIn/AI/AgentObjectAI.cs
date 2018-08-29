using BTCore;
using Framework;
using NodeEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
{
    public class AgentObjectAI
    {
        public static NETreeComposeType AIComposeType = new NETreeComposeType(typeof(AITree), new List<Type> { typeof(AINodeAttribute), typeof(BTNodeAttribute), typeof(BTGameNodeAttribute) }, "Assets/ResourceEx/Config/AI", "AI", "bytes", "AI");
        private static bool m_bInit = false;
        private static List<Type> m_lstAINodeType;
        public static List<Type> lstAINodeType { get { return m_lstAINodeType; } }
        private static List<Type> m_lstAINodeDataType;
        public static List<Type> lstAINodeDataType { get { return m_lstAINodeDataType; } }
        private static Type[] m_arrAINodeDataType;
        public static Type[] arrAINodeDataType { get { return m_arrAINodeDataType; } }

        public static void Init()
        {
            if (m_bInit) return;
            m_bInit = true;
            NEUtil.LoadTreeComposeTypes(AIComposeType, out m_lstAINodeType, out m_lstAINodeDataType);
            m_arrAINodeDataType = m_lstAINodeDataType.ToArray();
        }

        public AgentObject host { get { return m_cHost; } }
        private AgentObject m_cHost;
        private AITree m_cAITree;
        private string m_sAIPath;
        private AIBlackBoard m_cBlackBoard;
        private bool m_bStart;
        public bool start { get { return m_bStart; } }
        static AgentObjectAI() { Init(); }

        public void Init(AgentObject host,string aiPath)
        {
            if (!string.IsNullOrEmpty(m_sAIPath) && m_cAITree != null)
            {
                AITreePool.Instance.SaveAITree(m_sAIPath, m_cAITree);
                m_sAIPath = null;
            }
            m_sAIPath = aiPath;
            m_cAITree = AITreePool.Instance.GetAITree(m_sAIPath);

            m_cHost = host;
            m_cBlackBoard = new AIBlackBoard(this);
            m_bStart = false;
        }

        public void Start()
        {
            m_bStart = true;
        }

        public void Stop()
        {
            m_bStart = false;
            if (m_cAITree != null)
            {
                m_cAITree.Clear();
            }
        }

        public void Update(FP deltaTime)
        {
            if(m_bStart && m_cAITree != null)
            {
                m_cBlackBoard.deltaTime = deltaTime;
                BTResult result = m_cAITree.OnTick(m_cBlackBoard);
                if(result != BTResult.Running)
                {
                    Stop();
                }
            }
        }

        public void Clear()
        {
            if (!string.IsNullOrEmpty(m_sAIPath) && m_cAITree != null)
            {
                AITreePool.Instance.SaveAITree(m_sAIPath, m_cAITree);
            }
            if (m_cBlackBoard != null)
            {
                m_cBlackBoard.Clear();
            }
            m_sAIPath = null;
            m_cAITree = null;
            m_cHost = null;
        }
    }
}
