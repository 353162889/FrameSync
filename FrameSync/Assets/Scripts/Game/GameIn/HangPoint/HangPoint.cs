using Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Game
{
    public class HangPoint : MonoBehaviour
    {

        private HangPointView m_cHangPointView;
        private HangPointItem m_cHangPointItem;

        public void InitHangView(HangPointView hangPointView)
        {
            m_cHangPointView = hangPointView;
        }

        public void Init(string path)
        {
            m_cHangPointItem = HangPointCfgSys.Instance.GetHangPointItem(path);
        }

        public Transform GetHangPoint(string name,TSVector curPosition,TSVector curForward,out TSVector position,out TSVector forward)
        {
            position = curPosition;
            forward = curForward;
            Transform transform = null;
            bool hasHangPoint = false;
            if (m_cHangPointItem != null)
            {
                var hangPointData = m_cHangPointItem.GetHangPointData(name);
                if (hangPointData != null)
                {
                    hasHangPoint = true;
                    FP nAngle = TSVector.Angle(TSVector.forward, curForward);
                    if (curForward.x < 0)
                    {
                        nAngle = 360 - nAngle;
                    }
                    //FP len = hangPointData.position.magnitude;
                    //FP sinAngle = TSMath.Sin(nAngle);
                    //FP cosAngle = TSMath.Cos(nAngle);
                    //FP rotateX = len * sinAngle;
                    //FP rotateZ = len * cosAngle;
                    //position.y = hangPointData.position.y;
                    //position.x = rotateX;
                    //position.z = rotateZ;

                    //TSVector cfgForward = hangPointData.forward;
                    //cfgForward.y = 0;
                    //nAngle = TSVector.Angle(TSVector.forward, cfgForward);
                    //sinAngle = TSMath.Sin(nAngle);
                    //cosAngle = TSMath.Cos(nAngle);

                    //这里会有性能问题
                    TSQuaternion sQuat = TSQuaternion.AngleAxis(nAngle, TSVector.up);
                    position = curPosition + sQuat * hangPointData.position;
                    forward = sQuat * hangPointData.forward;
                    forward.Normalize();
                }
            }
            if (m_cHangPointView != null)
            {
                transform = m_cHangPointView.GetHangPoint(name);
                hasHangPoint = true;
            }
            if (!hasHangPoint)
            {
                CLog.LogError("对象"+this.name+"找不到挂点名为:"+name+"的挂点信息");
            }
            return transform;
        }

        public void Clear()
        {
            if(m_cHangPointView != null)
            {
                //销毁当前对象身上挂点下的所有特效
                var lst = m_cHangPointView.GetAllHangPoint();
                for (int i = 0; i < lst.Count; i++)
                {
                    int count = lst[i].childCount;
                    for (int j = 0; j < count; j++)
                    {
                        var child = lst[i].GetChild(j);
                        SceneEffectPool.Instance.DestroyEffectGO(child.gameObject);
                    }
                }
            }
            m_cHangPointView = null;
            m_cHangPointItem = null;
        }
    }
}
