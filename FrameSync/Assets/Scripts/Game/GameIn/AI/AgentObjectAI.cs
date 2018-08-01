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
        public static NETreeComposeType AIComposeType = new NETreeComposeType(typeof(SkillTree), new List<Type> { typeof(AINodeAttribute), typeof(BTNodeAttribute), typeof(BTGameNodeAttribute) }, "Assets/ResourceEx/Config/AI", "AI", "bytes", "AI");
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
        private AIData m_cAIData;
        private AIBlackBoard m_cBlackBoard;
        public AgentObjectAI(AgentObject host, int configId)
        {
            Init();
            m_cHost = host;
            m_cAITree = AITreePool.Instance.GetAITree(configId);
            m_cAIData = m_cAITree.data as AIData;
            m_cBlackBoard = new AIBlackBoard(this);
        }

        public void Start()
        {

        }

        public void Stop()
        {

        }

        public void Update(FP deltaTime)
        {

        }

        public void Clear()
        {
            if (m_cAIData != null && m_cAITree != null)
            {
                AITreePool.Instance.SaveAITree(m_cAIData.id, m_cAITree);
            }
            m_cAIData = null;
            m_cAITree = null;
            m_cHost = null;
        }
    }
}
