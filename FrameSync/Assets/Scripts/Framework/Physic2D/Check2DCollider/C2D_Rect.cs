using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework
{
    public class C2D_Rect : Check2DCollider
    {
        public FP halfWidth { get; set; }
        public FP halfHeight { get; set; }
        
        public C2D_Rect(TSVector2 center,TSVector2 forward, FP nHalfWidth, FP nHalfHeight):base(center,forward)
        {
            halfWidth = nHalfWidth;
            halfHeight = nHalfHeight;
        }
        public override FP forwardLen
        {
            get
            {
                return halfHeight;
            }
        }

        public override bool CheckCircle(TSVector2 sCenter, FP nRadius)
        {
            return TSCheck2D.CheckRectangleAndCircle(this.center, this.forward, halfWidth, halfHeight, sCenter, nRadius);
        }

        public override bool CheckCollider(Check2DCollider collider)
        {
            return collider.CheckRect(this.center, this.forward, halfWidth, halfHeight);
        }

        public override bool CheckLine(TSVector2 sOrgPos, TSVector2 sOffset, out TSVector2 sCrossPoint)
        {
            sCrossPoint = TSVector2.zero;
            if(TSCheck2D.CheckRectangleAndLine(this.center, this.forward, halfWidth, halfHeight, sOrgPos, ref sOffset))
            {
                sCrossPoint = sOrgPos + sOffset;
                return true;
            }
            return false;
        }

        public override bool CheckPos(TSVector2 sPosition)
        {
            return TSCheck2D.CheckRectangleAndPos(this.center, this.forward, halfWidth, halfHeight, sPosition);
        }

        public override bool CheckRect(TSVector2 sCenter, TSVector2 sDir, FP nHalfWidth, FP nHalfHeight)
        {
            throw new Exception("需要实现矩形与矩形之间的碰撞");
        }
    }
}
