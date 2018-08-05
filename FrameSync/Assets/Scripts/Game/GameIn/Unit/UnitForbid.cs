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
            m_cForbid.Clear();
        }

        public void ResetForbid()
        {
            m_cForbid.Clear();
        }

        public uint Forbid(UnitForbidType forbidType, UnitForbidFromType forbidFromType)
        {
            return m_cForbid.Forbid((uint)forbidType, (byte)forbidFromType);
        }

        public void Resume(uint id)
        {
            m_cForbid.Resume(id);
        }

        public void Resume(UnitForbidType forbidType)
        {
            m_cForbid.Resume((uint)forbidType);
        }

        public bool IsForbid(UnitForbidType forbidType)
        {
            return m_cForbid.IsForbid((uint)forbidType);
        }
    }
}
