using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Game
{
    public class MoveBound : MonoBehaviour
    {
        public Transform left;
        public Transform right;
        public Transform up;
        public Transform bottom;

        void Awake()
        {
            InitTrans(left);
            InitTrans(right);
            InitTrans(up);
            InitTrans(bottom);
        }

        private void InitTrans(Transform t)
        {
            if (t != null)
            {
                t.gameObject.layer = LayerDefine.MoveBound;
                var collider = t.GetComponent<Collider>();
                if (collider != null)
                {
                    collider.isTrigger = true;
                }
            }
        }

        public void SetRect(Rect rect)
        {
            if(left != null)
            {
                left.position = new Vector3(rect.xMin,left.position.y,rect.center.y);
            }
            if(right != null)
            {
                right.position = new Vector3(rect.xMax,left.position.y,rect.center.y);
            }
            if(up != null)
            {
                up.position = new Vector3(rect.center.x,up.position.y,rect.yMax);
            }
            if(bottom != null)
            {
                bottom.position = new Vector3(rect.center.x,up.position.y,rect.yMin);
            }
        }
    }
}
