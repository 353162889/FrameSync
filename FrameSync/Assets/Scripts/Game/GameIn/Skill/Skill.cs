﻿using BTCore;
using Framework;
using GameData;
using NodeEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
{
    public enum SkillTargetType
    {
        None,
        Target,
        TargetPosition,
        TargetForward,
    }

    //技能类型
    public enum SkillType
    {
        Active,     //主动
        Passive,    //被动
    }

    public class Skill
    {
        public static NETreeComposeType SkillComposeType = new NETreeComposeType(typeof(SkillTree), new List<Type> { typeof(SkillNodeAttribute), typeof(BTNodeAttribute), typeof(BTGameNodeAttribute) }, "Assets/ResourceEx/Config/Skill", "skill", "bytes", "技能");
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

        public int skillId { get { return m_cResInfo.id; } }
        private SkillType m_eSkillType;
        public SkillType skillType { get { return m_eSkillType; } }
        private SkillTargetType m_eTargetType;
        public SkillTargetType targetType { get { return m_eTargetType; } }
        public FP cd { get { return m_cResInfo.cd; } }
        public AgentObject host { get { return m_cHost; } }
        private AgentObject m_cHost;
        private SkillTree m_cSkillTree;
        private NEData m_cNEData;
        private SkillData m_cSkillData;
        public SkillData skillData { get { return m_cSkillData; } }
        private ResSkill m_cResInfo;
        public ResSkill resInfo { get { return m_cResInfo; } }
        private SkillBlackBoard m_cBlackBoard;
        private bool m_bIsDo;
        public bool isDo { get { return m_bIsDo; } }

        public AgentObject target { get { return m_cTarget; } }
        private AgentObject m_cTarget;
        public uint targetAgentId { get { return m_cTarget == null ? 0 : m_cTarget.id; } }
        public AgentObjectType targetAgentType { get { return m_cTarget == null ? AgentObjectType.Unit : m_cTarget.agentType; } }
        public TSVector targetPosition { get { return m_sTargetPosition; } }
        private TSVector m_sTargetPosition;
        public TSVector targetForward { get { return m_sTargetForward; } }
        private TSVector m_sTargetForward;

        private FP m_sStartTime;
        private FP m_sEndTime;

        public Skill(int configId,NEData neData)
        {
            Init();
            m_cResInfo = ResCfgSys.Instance.GetCfg<ResSkill>(configId);
            if (m_cResInfo == null) CLog.LogError("没有配置ID="+configId+"的技能配置");
            m_cNEData = neData;
            m_cSkillData = m_cNEData.data as SkillData;
            m_cSkillTree = CreateNode(m_cNEData) as SkillTree;
            m_cSkillTree.Clear();
            m_eSkillType = (SkillType)Enum.Parse(typeof(SkillType), m_cResInfo.type);
            m_eTargetType = (SkillTargetType)Enum.Parse(typeof(SkillTargetType), m_cResInfo.target_type);
            m_bIsDo = false;
            m_sStartTime = -1000;
            m_sEndTime = -1000;
        }

        public void Init(AgentObject host)
        {
            m_bIsDo = false;
            m_sStartTime = -1000;
            m_sEndTime = -1000;
            m_cHost = host;
            if (m_cBlackBoard == null)
            {
                m_cBlackBoard = new SkillBlackBoard();
            }
            m_cBlackBoard.Init(this);
        }

        public bool CanDo()
        {
            if (isDo) return false;
            if (FrameSyncSys.time < m_sEndTime) return false;
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
            m_sEndTime = FrameSyncSys.time + cd;
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
            m_cTarget = null;
            m_sTargetPosition = TSVector.zero;
            m_sTargetForward = TSVector.zero;
            m_bIsDo = false;
        }

        public void Clear()
        {
            End();
            m_cBlackBoard.Clear();
            m_cHost = null;
        }

        public static BTNode CreateNode(NEData neData)
        {
            if (!neData.enable) return null;
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
                    if (childNode != null)
                    {
                        neNode.AddChild(childNode);
                    }
                }
            }
            return neNode;
        }
    }
}
