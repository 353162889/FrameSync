using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace NodeEditor
{
    public class NEData
    {
        public Vector2 editorPos;
        public object data;
        public List<NEData> lstChild;
    }
}
