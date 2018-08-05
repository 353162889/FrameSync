using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
{
    public struct ForbidInfo
    {
        public uint id;
        public uint forbidType;
        public byte forbidFromType;
    }

    public class StateForbid
    {
        private List<ForbidInfo> m_lstForbid;
        private uint m_nForbid;
        public StateForbid()
        {
            m_lstForbid = new List<ForbidInfo>();
            m_nForbid = 0;
        }
        public void Init()
        {
            m_lstForbid.Clear();
            m_nForbid = 0;
        }

        public uint Forbid(uint forbidType,byte forbidFromType)
        {
            ForbidInfo forbidInfo = new ForbidInfo();
            forbidInfo.id = GameInTool.GenerateForbidId();
            forbidInfo.forbidType = forbidType;
            forbidInfo.forbidFromType = forbidFromType;
            m_lstForbid.Add(forbidInfo);
            m_nForbid |= forbidType;
            return forbidInfo.id;
        }

        public void Resume(uint id)
        {
            uint forbidType = 0;
            for (int i = m_lstForbid.Count - 1; i > -1; i--)
            {
                var forbidInfo = m_lstForbid[i];
                if(forbidInfo.id == id)
                {
                    forbidType = forbidInfo.forbidType;
                    m_lstForbid.RemoveAt(i);
                    break;
                }
            }
            if (forbidType != 0)
            {
                bool hasOther = false;
                for (int i = 0; i < m_lstForbid.Count; i++)
                {
                    if(forbidType == m_lstForbid[i].forbidType)
                    {
                        hasOther = true;
                        break;
                    }
                }
                if(!hasOther)
                {
                    m_nForbid &= ~forbidType;
                }
            }
        }

        public void ResumeType(uint forbidType)
        {
            for (int i = m_lstForbid.Count - 1; i > -1; i--)
            {
                if (m_lstForbid[i].forbidType == forbidType)
                {
                    m_lstForbid.RemoveAt(i);
                }
            }
            m_nForbid &= ~forbidType;
        }

        public bool IsForbid(uint forbidType)
        {
            return (m_nForbid & forbidType) != 0;
        }

        public void Clear()
        {
            m_lstForbid.Clear();
            m_nForbid = 0;
        }
    }
}
