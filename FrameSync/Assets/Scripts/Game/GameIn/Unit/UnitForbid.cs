using Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
{
    //最多32个状态
    public enum UnitForbidType : uint
    {
        ForbidMove = 1 << 0,//禁止移动
        ForbidForward = 1 << 1,//禁止改变面向
        ForbidSkill = 1 << 2,//禁止释放技能
        ForbidSelect = 1 << 3,//禁止被选中
        ForbidPlayerMove = 1 << 4,//禁止玩家请求移动
    }

    public enum UnitForbidFromType
    {
        Game,       //来自于玩法限制
        Skill,      //技能限制
    }

    public partial class Unit
    {
        private StateForbid m_cForbid;
        public void InitForbid()
        {
            if(m_cForbid == null)
            {
                m_cForbid = new StateForbid();
            }
            m_cForbid.Init();
        }

        public void UpdateForbid(FP deltaTime) { }

        public void DieForbid(DamageInfo damageInfo)
        {
            if (m_cForbid != null)
            {
                m_cForbid.Clear();
            }
        }

        public void ResetForbid()
        {
            if (m_cForbid != null)
            {
                m_cForbid.Clear();
            }
        }

        public uint Forbid(UnitForbidType forbidType, UnitForbidFromType forbidFromType)
        {
            if (m_cForbid != null)
            {
                return m_cForbid.Forbid((uint)forbidType, (byte)forbidFromType);
            }
            return 0;
        }

        public void Resume(uint id)
        {
            if (m_cForbid != null)
            {
                m_cForbid.Resume(id);
            }
        }

        public void Resume(UnitForbidType forbidType)
        {
            if (m_cForbid != null)
            {
                m_cForbid.Resume((uint)forbidType);
            }
        }

        public bool IsForbid(UnitForbidType forbidType)
        {
            if (m_cForbid != null)
            {
                return m_cForbid.IsForbid((uint)forbidType);
            }
            return false;
        }
    }
}
