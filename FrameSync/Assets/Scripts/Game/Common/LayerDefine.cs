using Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
{
    public class LayerDefine
    {
        public const int Default = 0;
        public const int MoveBound = 8;

        public const int DefaultMask = 1 << Default;
        public const int MoveBoundMask = 1 << MoveBound;

        public const string StrDefault = "Default";
        public const string StrMoveBound = "MoveBound";

        private static Dictionary<string, int> map = InitLayerMaskDic();
        private static Dictionary<string, int> map_layer = InitLayerDic();
        private static Dictionary<string, int> InitLayerMaskDic()
        {
            Dictionary<string, int> m = new Dictionary<string, int>();
            m.Add(StrDefault, DefaultMask);
            m.Add(StrMoveBound, MoveBoundMask);
            return m;
        }

        private static Dictionary<string, int> InitLayerDic()
        {
            Dictionary<string, int> m = new Dictionary<string, int>();
            m.Add(StrDefault, Default);
            m.Add(StrMoveBound, MoveBound);
            return m;
        }
        public static int GetLayerMaskByName(string name)
        {
            int result = DefaultMask;
            if (map.TryGetValue(name, out result))
            {
                return result;
            }
            else
            {
                CLog.LogError("can not find layer name:" + name);
                return result;
            }
        }

        public static int GetLayerByName(string name)
        {
            int result = Default;
            if (map_layer.TryGetValue(name, out result))
            {
                return result;
            }
            else
            {
                CLog.LogError("can not find layer name:" + name);
                return result;
            }
        }
    }
}
