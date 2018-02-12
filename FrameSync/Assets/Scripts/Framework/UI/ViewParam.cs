using UnityEngine;
using System.Collections;
using System;

namespace Framework
{
    public class ViewParam
    {
        public int intParam;
        public string strParam;
        public object objParam;
        public Action callbackParam;

        public void Copy(ViewParam param)
        {
            if (param == null)
            {
                this.intParam = -1;
                this.strParam = null;
                this.objParam = null;
                this.callbackParam = null;
            }
            else
            {
                this.intParam = param.intParam;
                this.strParam = param.strParam;
                this.objParam = param.objParam;
                this.callbackParam = param.callbackParam;
            }
        }
    }
}