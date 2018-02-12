using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Framework
{
    public class ResourceSys : MonoBehaviour
    {
        private static ResourceContainer uniqueInstance;

        public static ResourceContainer Instance
        {
            get
            {
                return uniqueInstance;
            }
        }

        protected virtual void Awake()
        {
            if (uniqueInstance == null)
            {
                uniqueInstance = gameObject.AddComponentOnce<ResourceContainer>();
                Exists = true;
                GameObject.DontDestroyOnLoad(this);
            }
        }

        protected virtual void OnDestroy()
        {
            if (uniqueInstance == this)
            {
                Exists = false;
                uniqueInstance = null;
            }
        }

        public static bool Exists
        {
            get;
            private set;
        }
    }
}
