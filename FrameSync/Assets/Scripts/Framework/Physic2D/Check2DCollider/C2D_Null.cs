using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework
{
    public class C2D_Null : Check2DCollider
    {
        public static C2D_Null NULL = new C2D_Null();
        public override FP forwardLen
        {
            get
            {
                return 0;
            }
        }

        public override bool CheckCircle(TSVector2 sCenter, FP nRadius)
        {
            return false;
        }

        public override bool CheckCollider(Check2DCollider collider)
        {
            return false;
        }

        public override bool CheckLine(TSVector2 sOrgPos, TSVector2 sOffset, out TSVector2 sCrossPoint)
        {
            sCrossPoint = sOrgPos;
            return false;
        }

        public override bool CheckPos(TSVector2 sPosition)
        {
            return false;
        }

        public override bool CheckRect(TSVector2 sCenter, TSVector2 sDir, FP nHalfWidth, FP nHalfHeight)
        {
            return false;
        }
    }
}
