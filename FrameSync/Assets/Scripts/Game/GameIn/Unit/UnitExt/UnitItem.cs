using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework;

namespace Game
{
    public class UnitItem : Unit
    {
        public FP radius { get { return 2; } }
        protected override void SubInit()
        {
            m_sPrefab = "Prefab/Item/Item_1";
            InitAttr();
            InitMove();
            InitView();
            InitAI();
            this.moveSpeed = 10;
        }

        public void HitUnit(Unit unit)
        {

        }
    }
}
