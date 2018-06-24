using Framework;
using NodeEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
{

    public class GameColliderCfgSys : Singleton<GameColliderCfgSys>
    {
        public static string GameColliderPath = "Assets/ResourceEx/Config/GameCollider/GameCollider.bytes";
        public static Type[] RelateTypes;
        private GameColliderSet m_cGameColliderSet;
        private Action m_cCallback;
        private string m_sRootDir;

        static GameColliderCfgSys()
        {
            List<Type> types = new List<Type>();
            types.Add(typeof(GameColliderSet));
            types.Add(typeof(GameColliderItem));
            var arr = Enum.GetValues(typeof(GameColliderType));
            foreach (var item in arr)
            {
                string typeName = "Game." + item.ToString() + "Data";
                Type type = Type.GetType(typeName);
                types.Add(type);
            }
            RelateTypes = types.ToArray();
        }

        public void LoadResCfgs(Action onFinish)
        {
            m_cCallback = onFinish;
            m_sRootDir = ResourceSys.Instance.ResRootDir;
            if (!m_sRootDir.EndsWith("/")) m_sRootDir = m_sRootDir + "/";
            var path = GameColliderPath.Replace(m_sRootDir, "");
            ResourceSys.Instance.GetResource(path, OnLoadSucc, OnLoadSucc);
        }

        private void OnLoadSucc(Resource res)
        {
            var callback = m_cCallback;
            m_cCallback = null;
            if (res.isSucc)
            {
                byte[] bytes = res.GetBytes();
                m_cGameColliderSet = NEUtil.DeSerializerObjectFromBuff(bytes, typeof(GameColliderSet), RelateTypes) as GameColliderSet;
            }
            if (callback != null)
            {
                callback();
            }
        }
        //path，资源路径
        public GameColliderItem GetColliderItem(string path)
        {
            GameColliderItem colliderItem = null;
            if (m_cGameColliderSet != null)
            {
                path = m_sRootDir + path;
                colliderItem = m_cGameColliderSet.GetColliderItem(path);
            }
            return colliderItem;
        }

        public override void Dispose()
        {
            m_cGameColliderSet = null;
            base.Dispose();
        }
    }
}
