using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
{
    public static class BattleInfo
    {
        public static long userId;//用户id
        public static int levelId;//关卡id
        public static int sceneId;//场景id
        public static bool standAlone;
        public static string ip;
        public static int port;
        public static int matchCount;//匹配人数

        public static void Clear()
        {
            userId = 0;
            levelId = 0;
            sceneId = 0;
            standAlone = true;
            ip = "";
            port = 0;
            matchCount = 1;
        }
    }
}
