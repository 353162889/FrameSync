using Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
{
    public class GameColliderSet
    {
        public List<GameColliderItem> mLstColliderItem = new List<GameColliderItem>();
        public GameColliderItem GetColliderItem(string path)
        {
            if (mLstColliderItem != null)
            {
                for (int i = 0; i < mLstColliderItem.Count; i++)
                {
                    if (mLstColliderItem[i].path == path)
                    {
                        return mLstColliderItem[i];
                    }
                }
            }
            return null;
        }
    }

    public class GameColliderItem
    {
        public string path;
        public List<BaseGameColliderData> mLstData = new List<BaseGameColliderData>();
    }

    public enum GameColliderType
    {
        CircleCollider,
        RectCollider,
    }

    public class BaseGameColliderData
    {
        public string name;
        public GameColliderType gameColliderType;
    }

    public class CircleColliderData : BaseGameColliderData
    {
        public TSVector center;
        public TSVector forward;
        public FP radius;
    }

    public class RectColliderData : BaseGameColliderData
    {
        public TSVector center;
        public TSVector forward;
        public FP halfWidth;
        public FP halfHeight;
    }
}
