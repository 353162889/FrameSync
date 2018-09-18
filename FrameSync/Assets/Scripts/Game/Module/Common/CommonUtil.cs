using UnityEngine;
using System.Collections;
using System;
using Framework;
using System.Collections.Generic;

namespace Game
{
    public class PopupViewOpenObject
    {
        public string title;
        public string content;
        public Action confirmCallback;
        public Action cancelCallback;
    }
    public static class CommonUtil
    {
        public static void ShowPopup(string content, string title = "提示",Action confirmCallback = null,Action cancelCallback = null)
        {
            ViewParam param = new ViewParam();
            var openObj = new PopupViewOpenObject();
            param.objParam = openObj;
            openObj.title = title;
            openObj.content = content;
            openObj.confirmCallback = confirmCallback;
            openObj.confirmCallback = cancelCallback;
            if(ViewSys.Instance.IsOpen("PopupTipsView"))
            {
                ViewSys.Instance.Close("PopupTipsView");
            }
            ViewSys.Instance.Open("PopupTipsView", param);
        }
    }

}