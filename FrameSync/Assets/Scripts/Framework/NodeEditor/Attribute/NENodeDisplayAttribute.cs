using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NodeEditor
{
    public class NENodeDisplayAttribute : Attribute
    {
        public bool showInPoint { get; private set; }
        public bool showOutPoint { get; private set; }
        public bool showClose { get; private set; }
        public NENodeDisplayAttribute(bool showInPoint = true,bool showOutPoint = true,bool showClose = true)
        {
            this.showInPoint = showInPoint;
            this.showOutPoint = showOutPoint;
            this.showClose = showClose;
        }

    }
}
