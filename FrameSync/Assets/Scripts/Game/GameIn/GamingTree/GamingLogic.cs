using BTCore;
using Framework;
using NodeEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
{
    public class GamingLogic
    {
        public static NETreeComposeType GamingComposeType = new NETreeComposeType(typeof(GamingTree), new List<Type> { typeof(GamingNodeAttribute), typeof(BTNodeAttribute) }, "Assets/ResourceEx/Config/Gaming", "Gaming", "bytes", "Gaming");
        private static bool m_bInit = false;
        private static List<Type> m_lstGamingNodeType;
        public static List<Type> lstGamingNodeType { get { return m_lstGamingNodeType; } }
        private static List<Type> m_lstGamingNodeDataType;
        public static List<Type> lstGamingNodeDataType { get { return m_lstGamingNodeDataType; } }
        private static Type[] m_arrGamingNodeDataType;
        public static Type[] arrGamingNodeDataType { get { return m_arrGamingNodeDataType; } }

        public static void Init()
        {
            if (m_bInit) return;
            m_bInit = true;
            NEUtil.LoadTreeComposeTypes(GamingComposeType, out m_lstGamingNodeType, out m_lstGamingNodeDataType);
            m_arrGamingNodeDataType = m_lstGamingNodeDataType.ToArray();
        }

        private GamingTree m_cGamingTree;
        private GamingData m_cGamingData;
        private GamingBlackBoard m_cBlackBoard;
        private bool m_bIsDo;
        static GamingLogic() { Init(); }

        public void Init(int configId)
        {
            m_cGamingTree = GamingTreePool.Instance.GetGamingTree(configId);
            m_cGamingData = m_cGamingTree.data as GamingData;
            m_cBlackBoard = new GamingBlackBoard();
            m_bIsDo = true;
        }

        public bool Update(FP deltaTime)
        {
            if (m_bIsDo && m_cGamingTree != null)
            {
                m_cBlackBoard.deltaTime = deltaTime;
                BTResult result = m_cGamingTree.OnTick(m_cBlackBoard);
                if (result != BTResult.Running)
                {
                    End();
                    return false;
                }
                return true;
            }
            return false;
        }

        public void End()
        {
            m_cGamingTree.Clear();
            m_bIsDo = false;
        }

        public void Clear()
        {
            End();
            if (m_cGamingData != null && m_cGamingTree != null)
            {
                GamingTreePool.Instance.SaveGamingTree(m_cGamingData.id, m_cGamingTree);
            }
            if (m_cBlackBoard != null)
            {
                m_cBlackBoard.Clear();
            }
            m_cGamingData = null;
            m_cGamingTree = null;
        }
    }
}
