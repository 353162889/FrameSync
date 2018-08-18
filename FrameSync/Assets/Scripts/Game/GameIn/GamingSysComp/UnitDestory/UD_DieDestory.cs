using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
{
    public class UD_DieDestory : UnitDestoryBase
    {
        protected override bool Check(Unit unit)
        {
            return unit.isDie;
        }
    }
}
