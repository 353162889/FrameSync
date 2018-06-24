using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework
{
    public class C2D_Circle : Check2DCollider
    {
        public FP radius { get; set; }

        public override FP forwardLen
        {
            get
            {
                return radius;
            }
        }

        public C2D_Circle(TSVector2 center,TSVector2 forward, FP radius):base(center,forward)
        {
            this.radius = radius;
        }

        public override bool CheckCircle(TSVector2 sCenter, FP nRadius)
        {
            return TSCheck2D.CheckCircleAndCircle(this.center, radius, sCenter, nRadius);
        }

        public override bool CheckCollider(Check2DCollider collider)
        {
            if (collider == null) return false;
            return collider.CheckCircle(this.center, radius);
        }

        public override bool CheckLine(TSVector2 sOrgPos, TSVector2 sOffset, out TSVector2 sCrossPoint)
        {
            return TSCheck2D.CheckCicleAndLine(sOrgPos, sOffset, this.center, radius, out sCrossPoint);
        }

        public override bool CheckPos(TSVector2 sPosition)
        {
            return TSCheck2D.CheckCicleAndPos(this.center, radius, sPosition);
        }

        public override bool CheckRect(TSVector2 sCenter, TSVector2 sDir, FP nHalfWidth, FP nHalfHeight)
        {
            return TSCheck2D.CheckRectangleAndCircle(sCenter, sDir, nHalfWidth, nHalfHeight, this.center, radius);
        }
      
    }
}
