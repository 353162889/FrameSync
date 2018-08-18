using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
{
    public class UD_StopMoveDestory : UnitDestoryBase
    {
        protected override bool Check(Unit unit)
        {
            return !unit.isMoving;
        }
    }
}
