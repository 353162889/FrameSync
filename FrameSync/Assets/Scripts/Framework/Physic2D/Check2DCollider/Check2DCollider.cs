using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework
{
    abstract public class Check2DCollider : IPoolable
    {
        public TSVector2 center { get; set; }
        public TSVector2 forward { get; set; }
        public Check2DCollider()
            :this(TSVector2.zero,TSVector2.up)
        {
        }
        public Check2DCollider(TSVector2 center,TSVector2 forward)
        {
            this.center = center;
            this.forward = forward;
        }

        abstract public FP forwardLen { get; }
       
        abstract public bool CheckLine(TSVector2 sOrgPos, TSVector2 sOffset, out TSVector2 sCrossPoint);

        abstract public bool CheckPos(TSVector2 sPosition);

        abstract public bool CheckCircle(TSVector2 sCenter, FP nRadius);

        abstract public bool CheckRect(TSVector2 sCenter, TSVector2 sDir, FP nHalfWidth, FP nHalfHeight);

        abstract public bool CheckCollider(Check2DCollider collider);

        public virtual void Reset()
        {
            this.center = TSVector2.zero;
            this.forward = TSVector2.up;
        }
    }
}
