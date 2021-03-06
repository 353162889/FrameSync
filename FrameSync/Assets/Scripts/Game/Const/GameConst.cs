﻿using Framework;
using GameData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
{
    public class GameConst : Singleton<GameConst>
    {
        #region AI部分
        public static string AIJoinScenePathName = "AIJoinScenePathName";//ai刷出时需要移动的路径
        public static string AILeaveScenePathName = "AILeaveScenePathName";//ai离场是需要移动的路径
        public static string AITargetName = "AITargetName";//ai目标
        #endregion

        #region 技能部分
        public static string SkillLaserEffectName = "SkillLaserEffectName";//激光特效的名称
        #endregion

        public int GetInt(string key)
        {
            var resConst = GetResConst(key);
            if(resConst != null)
            {
                return resConst.p_int;
            }
            return 0;
        }

        public string GetString(string key)
        {
            var resConst = GetResConst(key);
            if(resConst != null)
            {
                return resConst.p_string;
            }
            return "";
        }

        protected ResConst GetResConst(string key)
        {
            var resConst = ResCfgSys.Instance.GetCfg<ResConst>(key);
            if(resConst == null)
            {
                CLog.LogError("不存在key="+key+"的常量配置");
            }
            return resConst;
        }
    }
}
