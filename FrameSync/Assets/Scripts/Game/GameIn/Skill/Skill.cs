using BTCore;
using Framework;
using NodeEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
{
    public class Skill
    {
        public static NETreeComposeType SkillComposeType = new NETreeComposeType(typeof(SkillTree), new List<Type> { typeof(SkillNodeAttribute), typeof(NENodeAttribute) }, "", "bytes", "技能");
        private static bool m_bInit = false;
        private static List<Type> m_lstSkillNodeType;
        public static List<Type> lstSkillNodeType { get { return m_lstSkillNodeType; } }
        private static List<Type> m_lstSkillNodeDataType;
        public static List<Type> lstSkillNodeDataType { get { return m_lstSkillNodeDataType; } }
        private static Type[] m_arrSkillNodeDataType;
        public static Type[] arrSkillNodeDataType { get { return m_arrSkillNodeDataType; } }

        public static void Init()
        {
            if (m_bInit) return;
            m_bInit = true;
            NEUtil.LoadTreeComposeTypes(SkillComposeType,out m_lstSkillNodeType,out m_lstSkillNodeDataType);
            m_arrSkillNodeDataType = m_lstSkillNodeDataType.ToArray();
        }

        public int skillId { get { return m_cSkillData.skillId; } }
        public SkillTargetType targetType { get { return m_cSkillData.skillTarget; } }
        public FP cd { get { return m_cSkillData.skillCD; } }
        public AgentObject host { get { return m_cHost; } }
        private AgentObject m_cHost;
        private SkillTree m_cSkillTree;
        private NEData m_cNEData;
        private SkillData m_cSkillData;
        public SkillData skillData { get { return m_cSkillData; } }
        private SkillBlackBoard m_cBlackBoard;
        private bool m_bIsDo;
        public bool isDo { get { return m_bIsDo; } }

        public AgentObject target { get { return m_cTarget; } }
        private AgentObject m_cTarget;
        public TSVector targetPosition { get { return m_sTargetPosition; } }
        private TSVector m_sTargetPosition;
        public TSVector targetForward { get { return m_sTargetForward; } }
        private TSVector m_sTargetForward;

        private FP m_sStartTime;

        public Skill(AgentObject host,NEData neData)
        {
            Init();
            m_cHost = host;
            m_cNEData = neData;
            m_cSkillData = m_cNEData.data as SkillData;
            m_cSkillTree = CreateNode(m_cNEData) as SkillTree;
            m_cSkillTree.Clear();
            m_cBlackBoard = new SkillBlackBoard(this);
            m_bIsDo = false;
            m_sStartTime = -1000;
        }

        public bool CanDo()
        {
            if (isDo) return false;
            if (FrameSyncSys.time - m_sStartTime < cd) return false;
            return true;
        }

        public void Do(uint targetAgentId,AgentObjectType targetAgentType,TSVector position,TSVector forward)
        {
            if (!CanDo()) return;
            m_cTarget = AgentObject.GetAgentObject(targetAgentId,targetAgentType);
            m_sTargetPosition = position;
            m_sTargetForward = forward;
            m_bIsDo = true;
            m_sStartTime = FrameSyncSys.time;
        }

        public void Break()
        {
            if (!m_bIsDo) return;
            End();
        }

        public void Update(FP deltaTime)
        {
            if(m_bIsDo)
            {
                m_cBlackBoard.deltaTime = deltaTime;
                BTResult result = m_cSkillTree.OnTick(m_cBlackBoard);
                if(result != BTResult.Running)
                {
                    End();
                }
            }
        }

        public void End()
        {
            m_cSkillTree.Clear();
            m_cBlackBoard.Clear();
            m_cTarget = null;
            m_sTargetPosition = TSVector.zero;
            m_sTargetForward = TSVector.zero;
            m_bIsDo = false;
        }

        public void Clear()
        {
            End();
            m_cHost = null;
        }

        public static BTNode CreateNode(NEData neData)
        {
            Type neDataType = neData.data.GetType();
            int index = m_lstSkillNodeDataType.IndexOf(neDataType);
            if (index == -1)
            {
                CLog.LogError("can not find skillNeDataType="+neDataType+" mapping nodeType");
                return null;
            }
            Type neNodeType = m_lstSkillNodeType[index];
            BTNode neNode = Activator.CreateInstance(neNodeType) as BTNode;
            neNode.data = neData.data;
            if(neData.lstChild != null)
            {
                for (int i = 0; i < neData.lstChild.Count; i++)
                {
                    BTNode childNode = CreateNode(neData.lstChild[i]);
                    neNode.AddChild(childNode);
                }
            }
            return neNode;
        }
    }
}
