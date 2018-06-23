using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Game
{
    public class HangPointView : MonoBehaviour
    {
        [SerializeField]
        private List<string> m_lstTransName = new List<string>();
        [SerializeField]
        private List<Transform> m_lstTrans = new List<Transform>();

        public Transform GetHangPoint(string name)
        {
            int index = m_lstTransName.IndexOf(name);
            if(index > -1)
            {
                return m_lstTrans[index];
            }
            return null;
        }
    }
}
