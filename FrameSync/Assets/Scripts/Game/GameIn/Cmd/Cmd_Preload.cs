using Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
{
    public class Cmd_Preload : CommandBase
    {
        private Dictionary<string, int> m_dic = new Dictionary<string, int>();
        public override void Execute(ICommandContext context)
        {
            base.Execute(context);
            m_dic.Clear();
            m_dic.Add(PathTool.GetBasePrefabPath("Prefab/Scene/Camera"),1);
            if (m_dic.Count == 0)
            {
                this.OnExecuteDone(CmdExecuteState.Success);
            }
            else
            {
                string[] keys = m_dic.Keys.ToArray<string>();
                int[] values = m_dic.Values.ToArray<int>();
                for (int i = 0; i < keys.Length; i++)
                {
                    SceneGOPool.Instance.CacheObject(keys[i], values[i], OnCallback);
                }
            }
        }

        private void OnCallback(string path)
        {
            m_dic.Remove(path);
            if(m_dic.Count == 0)
            {
                this.OnExecuteDone(CmdExecuteState.Success);
            }
        }

        public override void OnDestroy()
        {
            foreach (var item in m_dic)
            {
                SceneGOPool.Instance.RemoveCacheObject(item.Key, OnCallback);
            }
            base.OnDestroy();
        }
    }
}
