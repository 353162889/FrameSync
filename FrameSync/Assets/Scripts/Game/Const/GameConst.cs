using Framework;
using GameData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
{
    public class GameConst : Singleton<GameConst>
    {
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
