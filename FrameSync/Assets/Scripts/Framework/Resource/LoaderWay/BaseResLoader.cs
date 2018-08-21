using System;
using UnityEngine;

namespace Framework
{ 
    public class BaseResLoader : MonoBehaviour
    {
	    public event Action<Resource> OnResourceDone;


        protected ResourceFileUtil _resUtil;
        private void Awake()
        {
            _resUtil = gameObject.AddComponentOnce<ResourceFileUtil>();
        }

        protected virtual void OnDestroy()
        {
            _resUtil = null;
        }

        public virtual void Load(Resource res)
	    {
	    }

	    protected void OnDone(Resource res)
	    {
		    if (OnResourceDone != null)
		    {
			    OnResourceDone.Invoke (res);
		    }
	    }

        protected virtual string GetInResPath(Resource res)
        {
            return "";
        }
    }

}