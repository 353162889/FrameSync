using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Framework
{
    public class UIGraphicRaycaster : GraphicRaycaster
    {
        public int priority = 0;
        public override int sortOrderPriority
        {
            get
            {
                return priority;
            }
        }
    }

}