using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BTCore;
using Game;

namespace NodeEditor
{

  
    public class NEConfig
    {
        public static NETreeComposeType[] arrTreeComposeData = new NETreeComposeType[] {
        //技能数据
        Skill.SkillComposeType,
        Remote.RemoteComposeType,
        };
    }
}
