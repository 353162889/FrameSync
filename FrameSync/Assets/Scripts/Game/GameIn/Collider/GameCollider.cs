using Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
{
    public class GameCollider
    {
        
        private GameColliderItem m_cColliderItem;
        public C2D_Combine collider { get { return m_cColider; } }
        private C2D_Combine m_cColider;

        static GameCollider()
        {
            ObjectPool<C2D_Circle>.Instance.Init(100);
            ObjectPool<C2D_Combine>.Instance.Init(100);
            ObjectPool<C2D_Rect>.Instance.Init(100);
        }

        public void Init(string path)
        {
            m_cColliderItem = GameColliderCfgSys.Instance.GetColliderItem(path);
            m_cColider = ObjectPool<C2D_Combine>.Instance.GetObject();
            if (m_cColliderItem != null && m_cColliderItem.mLstData.Count > 0)
            {
                for (int i = 0; i < m_cColliderItem.mLstData.Count; i++)
                {
                    var c = Create2DCollider(m_cColliderItem.mLstData[i]);
                    m_cColider.AddChild(c);
                }
            }
        }

        private Check2DCollider Create2DCollider(BaseGameColliderData data)
        {
            Check2DCollider collider = C2D_Null.NULL;
            if(data.gameColliderType == GameColliderType.RectCollider)
            {
                C2D_Rect rect = ObjectPool<C2D_Rect>.Instance.GetObject();
                RectColliderData rectData = (RectColliderData)data;
                collider = rect;
            }
            else if(data.gameColliderType == GameColliderType.CircleCollider)
            {
                C2D_Circle circle = ObjectPool<C2D_Circle>.Instance.GetObject();
                CircleColliderData circleData = (CircleColliderData)data;
                collider = circle;
            }
            return collider;
        }

        public void Update(TSVector curPosition,TSVector forward)
        {

        }

        public void Clear()
        {
            m_cColliderItem = null;
            if(m_cColider != null)
            {
                int count = m_cColider.lstColliders.Count;
                for (int i = 0; i < count; i++)
                {
                    var collider = m_cColider.lstColliders[i];
                    if(collider is C2D_Rect)
                    {
                        ObjectPool<C2D_Rect>.Instance.SaveObject((C2D_Rect)collider);
                    }
                    else if(collider is C2D_Circle)
                    {
                        ObjectPool<C2D_Circle>.Instance.SaveObject((C2D_Circle)collider);
                    }
                }
                ObjectPool<C2D_Combine>.Instance.SaveObject(m_cColider);
            }
            m_cColider = null;
        }
    }
}
