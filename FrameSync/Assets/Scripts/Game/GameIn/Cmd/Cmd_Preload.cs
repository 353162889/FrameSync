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
        private MultiResourceLoader m_cMultiLoader;
        public override void Execute(ICommandContext context)
        {
            base.Execute(context);
            m_cMultiLoader = new MultiResourceLoader(ResourceSys.Instance);
            m_dic.Clear();
            m_dic.Add(PathTool.GetBasePrefabPath("Prefab/Scene/Camera"),1);
            m_cMultiLoader.LoadList(m_dic.Keys.ToList(),OnComplete,OnProgress);
        }

        private void OnProgress(Resource res)
        {
            int count = m_dic[res.path];
            SceneGOPool.Instance.CacheObject(res, count);
        }

        private void OnComplete(MultiResourceLoader obj)
        {
            this.OnExecuteDone(CmdExecuteState.Success);
        }

        public override void OnDestroy()
        {
            if(m_cMultiLoader != null)
            {
                m_cMultiLoader.Clear();
                m_cMultiLoader = null;
            }
            base.OnDestroy();
        }
    }
}
