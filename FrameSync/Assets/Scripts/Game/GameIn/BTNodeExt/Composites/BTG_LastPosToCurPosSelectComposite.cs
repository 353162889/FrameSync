using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
{
    public class BTG_LastPosToCurPosSelectCompositeData : BTG_BaseSelectAgentObjCompositeData
    {

    }
    [BTGameNode(typeof(BTG_LastPosToCurPosSelectCompositeData))]
    public class BTG_LastPosToCurPosSelectComposite : BTG_BaseSelectAgentObjComposite
    {
        private BTG_LastPosToCurPosSelectCompositeData m_cLastPosToCurPosData;
        protected override void OnInitData(object data)
        {
            base.OnInitData(data);
            m_cLastPosToCurPosData = data as BTG_LastPosToCurPosSelectCompositeData;
        }
    }
}
