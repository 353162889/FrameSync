using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BTCore
{
    public class BTBlackBoard
    {
        protected List<BTSharedVariable> m_lstVariable;
        public BTBlackBoard()
        {
            m_lstVariable = new List<BTSharedVariable>();
        }

        public void SetVariable(string name,BTSharedVariable variable)
        {
            if(!m_lstVariable.Contains(variable))
            {
                variable.Name = name;
                m_lstVariable.Add(variable);
            }
        }

        public BTSharedVariable GetVariable(string name)
        {
            for (int i = 0; i < m_lstVariable.Count; i++)
            {
                if (m_lstVariable[i].Name == name) return m_lstVariable[i];
            }
            return null;
        }

        public void Clear()
        {
            m_lstVariable.Clear();
        }
    }
}
