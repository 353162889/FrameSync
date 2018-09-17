using Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Game
{
    public class GameCollider : IPoolable
    {
        private GameColliderItem m_cColliderItem;

        public List<Check2DCollider> lstCollider { get { return m_lstColliders; } }
        private List<Check2DCollider> m_lstColliders = new List<Check2DCollider>();

        public TSVector center { get { return m_sCenter; } }
        private TSVector m_sCenter;
        private TSVector2 m_sCenter2;

        public bool enable { get { return m_bEnable; } }
        private bool m_bEnable;

        static GameCollider()
        {
            ObjectPool<C2D_Circle>.Instance.Init(100);
            ObjectPool<C2D_Rect>.Instance.Init(100);
        }

        public void Init(TSVector position, string path)
        {
            m_cColliderItem = GameColliderCfgSys.Instance.GetColliderItem(path);
            m_lstColliders.Clear();
            m_bEnable = true;


            if (m_cColliderItem != null && m_cColliderItem.mLstData.Count > 0)
            {
                for (int i = 0; i < m_cColliderItem.mLstData.Count; i++)
                {
                    var c = Create2DCollider(m_cColliderItem.mLstData[i]);
                    m_lstColliders.Add(c);
                }
            }
            Reculate(position);
        }

        public void SetEnable(bool enable)
        {
            m_bEnable = enable;
        }

        private void Reculate(TSVector position)
        {
            if(m_lstColliders.Count > 0)
            {
                m_sCenter = TSVector.zero;
                for (int i = 0; i < m_lstColliders.Count; i++)
                {
                    TSVector2 center2 = m_lstColliders[i].center;
                    m_sCenter = m_sCenter + new TSVector(center2.x, 0, center2.y);
                }
                if (m_lstColliders.Count > 1)
                {
                    m_sCenter /= m_lstColliders.Count;
                }
            }
            else
            {
                m_sCenter = position;
            }
            m_sCenter2.x = m_sCenter.x;
            m_sCenter2.y = m_sCenter.z;
        }

        public bool CheckCircle(TSVector sCenter, FP nRadius,out TSVector sCrossPoint,bool ignoreEnable = false)
        {
            sCrossPoint = sCenter;
            if (!ignoreEnable && !m_bEnable) return false;
            TSVector2 sCenter2 = new TSVector2(sCenter.x,sCenter.z);
            TSVector2 sCrossPoint2 = sCenter2;
            if (m_lstColliders.Count > 0)
            {
                for (int i = m_lstColliders.Count - 1; i > -1; i--)
                {
                    var collider2 = m_lstColliders[i];
                    if (m_lstColliders[i].CheckCircle(sCenter2, nRadius))
                    {
                        collider2.CheckLine(sCenter2, collider2.center - sCenter2, out sCrossPoint2);
                        sCrossPoint.x = sCrossPoint2.x;
                        sCrossPoint.z = sCrossPoint2.y;
                        return true;
                    }
                }
            }
            else
            {
                if(TSCheck2D.CheckCicleAndPos(sCenter2,nRadius, m_sCenter2))
                {
                    sCrossPoint = m_sCenter;
                    return true;
                }
            }
            return false;
        }

        public bool CheckCircle(TSVector sCenter, FP nRadius, bool ignoreEnable = false)
        {
            if (!ignoreEnable && !m_bEnable) return false;
            TSVector2 sCenter2 = new TSVector2(sCenter.x, sCenter.z);
            TSVector2 sCrossPoint2 = sCenter2;
            if (m_lstColliders.Count > 0)
            {
                for (int i = m_lstColliders.Count - 1; i > -1; i--)
                {
                    var collider2 = m_lstColliders[i];
                    if (m_lstColliders[i].CheckCircle(sCenter2, nRadius))
                    {
                        return true;
                    }
                }
            }
            else
            {
                if (TSCheck2D.CheckCicleAndPos(sCenter2, nRadius, m_sCenter2))
                {
                    return true;
                }
            }
            return false;
        }

        public bool CheckCollider(GameCollider otherCollider,out TSVector sCrossPoint, bool ignoreEnable = false)
        {
            sCrossPoint = otherCollider.center;
            if (!ignoreEnable && !m_bEnable) return false;
            TSVector2 sCenter2 = new TSVector2(otherCollider.center.x, otherCollider.center.z);
            TSVector2 sCrossPoint2 = sCenter2;
            if (m_lstColliders.Count > 0)
            {
                for (int i = m_lstColliders.Count - 1; i > -1; i--)
                {
                    var collider2 = m_lstColliders[i];
                    var lstOtherCollders = otherCollider.lstCollider;
                    for (int j = 0; j < lstOtherCollders.Count; j++)
                    {
                        if (collider2.CheckCollider(lstOtherCollders[j]))
                        {
                            collider2.CheckLine(sCenter2, collider2.center - sCenter2, out sCrossPoint2);
                            sCrossPoint.x = sCrossPoint2.x;
                            sCrossPoint.z = sCrossPoint2.y;
                            return true;
                        }
                    }
                }
            }
            else
            {
                if(otherCollider.CheckPos(m_sCenter, out sCrossPoint))
                {
                    return true;
                }
            }
            return false;
        }

        public bool CheckCollider(GameCollider otherCollider, bool ignoreEnable = false)
        {
            if ((!ignoreEnable && !m_bEnable) || !otherCollider.enable) return false;
            TSVector2 sCenter2 = new TSVector2(otherCollider.center.x, otherCollider.center.z);
            TSVector2 sCrossPoint2 = sCenter2;
            if (m_lstColliders.Count > 0)
            {
                for (int i = m_lstColliders.Count - 1; i > -1; i--)
                {
                    var collider2 = m_lstColliders[i];
                    var lstOtherCollders = otherCollider.lstCollider;
                    for (int j = 0; j < lstOtherCollders.Count; j++)
                    {
                        if (collider2.CheckCollider(lstOtherCollders[j]))
                        {
                            return true;
                        }
                    }
                }
            }
            else
            {
                if(otherCollider.CheckPos(m_sCenter))
                {
                    return true;
                }
            }
            return false;
        }

        public bool CheckLine(TSVector sOrgPos, TSVector sOffset, out TSVector sCrossPoint, bool ignoreEnable = false)
        {
            sCrossPoint = sOrgPos;
            if (!ignoreEnable && !m_bEnable) return false;
            TSVector2 sOrgPos2 = new TSVector2(sOrgPos.x,sOrgPos.z);
            TSVector2 sOffset2 = new TSVector2(sOffset.x,sOffset.z);
            TSVector2 sCrossPoint2 = new TSVector2(sCrossPoint.x,sCrossPoint.z);
            for (int i = m_lstColliders.Count - 1; i > -1; i--)
            {
                if (m_lstColliders[i].CheckLine(sOrgPos2, sOffset2, out sCrossPoint2))
                {
                    sCrossPoint.x = sCrossPoint2.x;
                    sCrossPoint.z = sCrossPoint2.y;
                    return true;
                }
            }
            return false;
        }

        public bool CheckPos(TSVector sPosition,out TSVector sCrossPoint, bool ignoreEnable = false)
        {
            sCrossPoint = sPosition;
            if (!ignoreEnable && !m_bEnable) return false;
            TSVector2 sPosition2 = new TSVector2(sPosition.x,sPosition.z);
            for (int i = m_lstColliders.Count - 1; i > -1; i--)
            {
                var collider2 = m_lstColliders[i];
                if (collider2.CheckPos(sPosition2))
                {
                    return true;
                }
            }
            return false;
        }

        public bool CheckPos(TSVector sPosition, bool ignoreEnable = false)
        {
            if (!ignoreEnable && !m_bEnable) return false;
            TSVector2 sPosition2 = new TSVector2(sPosition.x, sPosition.z);
            for (int i = m_lstColliders.Count - 1; i > -1; i--)
            {
                var collider2 = m_lstColliders[i];
                if (collider2.CheckPos(sPosition2))
                {
                    return true;
                }
            }
            return false;
        }

        public bool CheckRect(TSVector sCenter, TSVector sDir, FP nHalfWidth, FP nHalfHeight,out TSVector sCrossPoint, bool ignoreEnable = false)
        {
            sCrossPoint = sCenter;
            if (!ignoreEnable && !m_bEnable) return false;
            TSVector2 sCenter2 = new TSVector2(sCenter.x, sCenter.z);
            TSVector2 sDir2 = new TSVector2(sDir.x, sDir.z);
            TSVector2 sCrossPoint2 = new TSVector2(sCrossPoint.x, sCrossPoint.z);
            if (m_lstColliders.Count > 0)
            {
                for (int i = m_lstColliders.Count - 1; i > -1; i--)
                {
                    var collider2 = m_lstColliders[i];
                    if (collider2.CheckRect(sCenter2, sDir2, nHalfWidth, nHalfHeight))
                    {
                        collider2.CheckLine(sCenter2, collider2.center - sCenter2, out sCrossPoint2);
                        sCrossPoint.x = sCrossPoint2.x;
                        sCrossPoint.z = sCrossPoint2.y;
                        return true;
                    }
                }
            }
            else
            {
                if(TSCheck2D.CheckRectangleAndPos(sCenter2,sDir2,nHalfWidth,nHalfHeight,m_sCenter2))
                {
                    return true;
                }
            }
            return false;
        }

        public bool CheckRect(TSVector sCenter, TSVector sDir, FP nHalfWidth, FP nHalfHeight, bool ignoreEnable = false)
        {
            if (!ignoreEnable && !m_bEnable) return false;
            TSVector2 sCenter2 = new TSVector2(sCenter.x, sCenter.z);
            TSVector2 sDir2 = new TSVector2(sDir.x, sDir.z);
            if (m_lstColliders.Count > 0)
            {
                for (int i = m_lstColliders.Count - 1; i > -1; i--)
                {
                    var collider2 = m_lstColliders[i];
                    if (collider2.CheckRect(sCenter2, sDir2, nHalfWidth, nHalfHeight))
                    {
                        return true;
                    }
                }
            }
            else
            {
                if (TSCheck2D.CheckRectangleAndPos(sCenter2, sDir2, nHalfWidth, nHalfHeight, m_sCenter2))
                {
                    return true;
                }
            }
            return false;
        }

        private Check2DCollider Create2DCollider(BaseGameColliderData data)
        {
            Check2DCollider collider = null;
            if(data.gameColliderType == GameColliderType.RectCollider)
            {
                C2D_Rect rect = ObjectPool<C2D_Rect>.Instance.GetObject();
                RectColliderData rectData = (RectColliderData)data;
                rect.halfWidth = rectData.halfWidth;
                rect.halfHeight = rectData.halfHeight;
                collider = rect;
            }
            else if(data.gameColliderType == GameColliderType.CircleCollider)
            {
                C2D_Circle circle = ObjectPool<C2D_Circle>.Instance.GetObject();
                CircleColliderData circleData = (CircleColliderData)data;
                circle.radius = circleData.radius;
                collider = circle;
            }
            return collider;
        }

        //碰撞框测试
        //private List<GameObject> m_lstObject = new List<GameObject>();
        public void Update(TSVector curPosition,TSVector curForward)
        {
            //if (m_lstObject.Count < m_lstColliders.Count)
            //{
            //    for (int i = 0; i < m_lstColliders.Count; i++)
            //    {
            //        if (m_lstColliders[i] is C2D_Rect)
            //        {
            //            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //            RectColliderData data = (RectColliderData)m_cColliderItem.mLstData[i];
            //            go.transform.localScale = new Vector3(data.halfWidth.AsFloat() * 2f, 1, data.halfHeight.AsFloat() * 2f);
            //            go.name = "RectCollider";
            //            m_lstObject.Add(go);
            //        }
            //        else if (m_lstColliders[i] is C2D_Circle)
            //        {
            //            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            //            CircleColliderData data = (CircleColliderData)m_cColliderItem.mLstData[i];
            //            go.transform.localScale = new Vector3(data.radius.AsFloat() * 2f, data.radius.AsFloat() * 2f, data.radius.AsFloat() * 2f);
            //            go.name = "CircleCollider";
            //            m_lstObject.Add(go);
            //        }
            //    }
            //}
            for (int i = 0; i < m_lstColliders.Count; i++)
            {
                var collider = m_lstColliders[i];
                BaseGameColliderData colliderData = m_cColliderItem.mLstData[i];
                UpdateCollider2(collider, curPosition, curForward, colliderData);
                //m_lstObject[i].transform.position = new Vector3(collider.center.x.AsFloat(), 0, collider.center.y.AsFloat());
                //m_lstObject[i].transform.forward = new Vector3(collider.forward.x.AsFloat(), 0, collider.forward.y.AsFloat());
            }
            Reculate(curPosition);
        }

        private void UpdateCollider2(Check2DCollider collider, TSVector curPosition,TSVector curForward,BaseGameColliderData colliderData)
        {
            if (colliderData.center == TSVector.zero && colliderData.forward == TSVector.forward)
            {
                collider.center = new TSVector2(curPosition.x, curPosition.z);
                collider.forward = new TSVector2(curForward.x, curForward.z);
            }
            else
            {
                FP nAngle = TSVector.Angle(TSVector.forward, curForward);
                if (curForward.x < 0)
                {
                    nAngle = 360 - nAngle;
                }
                TSQuaternion sQuat = TSQuaternion.AngleAxis(nAngle, TSVector.up);
                TSVector center = curPosition + sQuat * colliderData.center;
                TSVector forward = sQuat * colliderData.forward;
                collider.center = new TSVector2(center.x, center.z);
                collider.forward = new TSVector2(forward.x, forward.z);
            }
        }

        public void Reset()
        {
            m_cColliderItem = null;
            for (int i = 0; i < m_lstColliders.Count; i++)
            {
                var collider = m_lstColliders[i];
                if (collider is C2D_Rect)
                {
                    ObjectPool<C2D_Rect>.Instance.SaveObject((C2D_Rect)collider);
                }
                else if (collider is C2D_Circle)
                {
                    ObjectPool<C2D_Circle>.Instance.SaveObject((C2D_Circle)collider);
                }
            }
            m_lstColliders.Clear();
        }
    }
}
