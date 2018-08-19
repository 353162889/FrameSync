using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework
{
    public class TSCheck2D
    {
        //NEW已测
        /// <summary>
        /// 检测一个点的xz平面投影是否处于2DAabb内
        /// </summary>
        /// <param name="sCenter"></param>
        /// <param name="nHalfWidth"></param>
        /// <param name="nHalfHeight"></param>
        /// <param name="sPos"></param>
        /// <returns></returns>
        public static bool CheckAabbAndPos(TSVector2 sCenter, FP nHalfWidth, FP nHalfHeight, TSVector2 sPos)
        {
            TSVector2 sOffset = sPos - sCenter;
            if (TSMath.Abs(sOffset.x) > nHalfWidth)
            {
                return false;
            }
            if (TSMath.Abs(sOffset.y) > nHalfHeight)
            {
                return false;
            }
            return true;
        }

        //New 已测
        /// <summary>
        /// 检测点是否在矩形中
        /// </summary>
        /// <param name="sCenter"></param>
        /// <param name="sDir"></param>
        /// <param name="nHalfWidth"></param>
        /// <param name="nHalfHeight"></param>
        /// <param name="sPos"></param>
        /// <returns></returns>
        public static bool CheckRectangleAndPos(TSVector2 sCenter,TSVector2 sDir,FP nHalfWidth,FP nHalfHeight,TSVector2 sPos)
        {
            //推导链接:http://jingyan.baidu.com/article/2c8c281dfbf3dd0009252a7b.html
            //假设对图片上任意点(x,y)，绕一个坐标点(cx,cy)逆时针旋转a角度后的新的坐标设为(x0, y0)，有公式：
            //x0= (x - cx)*cos(a) - (y - cy)*sin(a) + cx ;
            //y0= (x - cx)*sin(a) + (y - cy)*cos(a) + cy ;

            //注意，当sDir与x轴大于180度的时候计算的是逆时针旋转的角度，小于180顺时针
            FP nAngle = TSVector2.Angle(sDir, TSVector2.up);
            if (sDir.x < 0)
            {
                //当计算的角度是顺时针角度需要转换为逆时针角度
                nAngle = 360 - nAngle;
            }
            FP angle = nAngle * FP.Deg2Rad;
            FP nCos = TSMath.Cos(angle);
            FP nSin = TSMath.Sin(angle);
            FP deltaX = sPos.x - sCenter.x;
            FP deltaY = sPos.y - sCenter.y;
            FP nNewX = deltaX * nCos - deltaY * nSin + sCenter.x;
            FP nNewY = deltaX * nSin + deltaY * nCos + sCenter.y;

            sPos.x = nNewX;
            sPos.y = nNewY;
            return CheckAabbAndPos(sCenter, nHalfWidth, nHalfHeight, sPos);
        }

        //NEW 已测
        /// <summary>
        /// 检测一条射线的xz平面投影是否与2DAabb相交
        /// </summary>
        /// <param name="sCenter">AABB中心点</param>
        /// <param name="nHalfWidth">AABB宽度</param>
        /// <param name="nHalfHeight">AABB高度</param>
        /// <param name="sOrigin">射线原点</param>
        /// <param name="sDirection">射线方向</param>
        /// <param name="nMaxDistance">射线长度</param>
        /// <returns>返回负数代表无交点</returns>
        public static FP CheckAabbAndLine(TSVector2 sCenter, FP nHalfWidth, FP nHalfHeight, TSVector2 sOrigin, TSVector2 sDirection, FP nMaxDistance)
        {
            FP nMax = FP.MaxValue;
            FP nMin = FP.MinValue;
            //非线段直接忽略掉
            if (sDirection.x == 0 && sDirection.y == 0) return -1;
            sDirection.Normalize();

            if (0 == sDirection.x)
            {
                if ((sOrigin.x < sCenter.x - nHalfWidth) || (sOrigin.x > sCenter.x + nHalfWidth))
                {
                    return -1;
                }
            }
            else
            {
                //射线上任意一点p = p0 + t *dir ;
                //这里采用轴分离的思想slab方法
                FP invX = 1 / sDirection.x;
                FP halfLength = sDirection.x > 0 ? nHalfWidth : -nHalfWidth;
                FP t1 = (sCenter.x - halfLength - sOrigin.x) * invX;
                FP t2 = (sCenter.x + halfLength - sOrigin.x) * invX;

                if (t1 < 0 && t2 < 0) return -1;

                if (t1 > nMin) nMin = t1;
                if (t2 < nMax) nMax = t2;
            }

            if (0 == sDirection.y)
            {
                if ((sOrigin.y < sCenter.y - nHalfHeight) || (sOrigin.y > sCenter.y + nHalfHeight))
                {
                    return -1;
                }
            }
            else
            {
                FP invY = 1 / sDirection.y;
                FP halfLength = sDirection.y > 0 ? nHalfHeight : -nHalfHeight;
                FP t1 = (sCenter.y - halfLength - sOrigin.y) * invY;
                FP t2 = (sCenter.y + halfLength - sOrigin.y) * invY;
                if (t1 < 0 && t2 < 0) return -1;
                if (t1 > nMin) nMin = t1;
                if (t2 < nMax) nMax = t2;
            }
            FP len = nMin;
            if(len < 0)
            {
                len = nMax;
            }
            if(len > nMax || len < 0)
            {
                return -1;
            }
            if (len > nMaxDistance)
            {
                if (nMin < 0)
                {
                    return 0;
                }
                else
                {
                    return -1;
                }
            }
            return len;
        }
        //New 已测
        /// <summary>
        /// 检查矩形是否和线段相交
        /// </summary>
        /// <param name="sCenter"></param>
        /// <param name="sDir"></param>
        /// <param name="nHalfWidth"></param>
        /// <param name="nHalfHeight"></param>
        /// <param name="sOrgPos"></param>
        /// <param name="sOffset"></param>
        /// <returns></returns>
        public static bool CheckRectangleAndLine(TSVector2 sCenter, TSVector2 sDir, FP nHalfWidth, FP nHalfHeight, TSVector2 sOrgPos, ref TSVector2 sOffset)
        {
            FP nAngle = TSVector2.Angle(sDir, TSVector2.up);
            if (sDir.x < 0)
            {
                nAngle = 360 - nAngle;
            }
            TSVector2 sEndPos = sOrgPos + sOffset;
            FP angle = nAngle * FP.Deg2Rad;
            FP nCos = TSMath.Cos(angle);
            FP nSin = TSMath.Sin(angle);
            FP deltaX = sOrgPos.x - sCenter.x;
            FP deltaY = sOrgPos.y - sCenter.y;
            FP nNewX = deltaX * nCos - deltaY * nSin + sCenter.x;
            FP nNewY = deltaX * nSin + deltaY * nCos + sCenter.y;
            sOrgPos.x = nNewX;
            sOrgPos.y = nNewY;


            deltaX = sEndPos.x - sCenter.x;
            deltaY = sEndPos.y - sCenter.y;
            nNewX = deltaX * nCos - deltaY * nSin + sCenter.x;
            nNewY = deltaX * nSin + deltaY * nCos + sCenter.y;
            sEndPos.x = nNewX;
            sEndPos.y = nNewY;

            TSVector2 sNewDirection = sEndPos - sOrgPos;
            FP nDis = sOffset.magnitude;
            nDis = CheckAabbAndLine(sCenter, nHalfWidth, nHalfHeight, sOrgPos, sNewDirection, nDis);
            if (nDis < 0)
            {
                return false;
            }
            else
            {
                sOffset = sOffset.normalized * nDis;
                return true;
            }
        }

        //New 已测
        /// <summary>
        /// 计算点到经过两点的直线的距离 
        /// </summary>
        /// <param name="sPos"></param>
        /// <param name="sPos1"></param>
        /// <param name="sPos2"></param>
        /// <returns></returns>
        public static FP DistanceFromPointToLine(TSVector2 sPos, TSVector2 sPos1, TSVector2 sPos2)
        {
            TSVector2 line1 = sPos2 - sPos1;
            TSVector2 line2 = sPos - sPos1;
            if (line1 == TSVector2.zero) return line2.magnitude;
            return  TSMath.Abs(line2.x * line1.y - line2.y * line1.x) / line1.magnitude;
        }

        //New 已测
        /// <summary>
        /// 检测矩形与圆是否相交
        /// </summary>
        /// <param name="sCenter"></param>
        /// <param name="sDir"></param>
        /// <param name="nHalfWidth"></param>
        /// <param name="nHalfHeight"></param>
        /// <param name="sCircleCenter"></param>
        /// <param name="nRadius"></param>
        /// <returns></returns>
        public static bool CheckRectangleAndCircle(TSVector2 sCenter, TSVector2 sDir, FP nHalfWidth, FP nHalfHeight, TSVector2 sCircleCenter, FP nRadius)
        {
            sDir.Normalize();
            TSVector2 sUp = new TSVector2(-sDir.y, sDir.x);

            TSVector2 sTopPos = sCenter + sUp * nHalfHeight;
            TSVector2 sRightPos = sCenter + sDir * nHalfWidth;

            FP nHalfWidth2 = DistanceFromPointToLine(sCircleCenter, sCenter, sRightPos);
            FP nHalfHeight2 = DistanceFromPointToLine(sCircleCenter, sCenter, sTopPos);

            if (nHalfWidth2 > nHalfWidth + nRadius)
                return false;
            if (nHalfHeight2 > nHalfHeight + nRadius)
                return false;

            if (nHalfWidth2 <= nHalfWidth)
                return true;
            if (nHalfHeight2 <= nHalfHeight)
                return true;

            return (nHalfWidth2 - nHalfWidth) * (nHalfWidth2 - nHalfWidth) + (nHalfHeight2 - nHalfHeight) * (nHalfHeight2 - nHalfHeight) <= nRadius * nRadius;
        }

        //New 已测
        /// <summary>
        /// 检测圆与线段是否相交
        /// </summary>
        /// <param name="sOrgPos"></param>
        /// <param name="sOffset"></param>
        /// <param name="sCenter"></param>
        /// <param name="nRadius"></param>
        /// <param name="sCrossPoint"></param>
        /// <returns></returns>
        public static bool CheckCicleAndLine(TSVector2 sOrgPos, TSVector2 sOffset, TSVector2 sCenter, FP nRadius, out TSVector2 sCrossPoint)
        {
            //推导过程
            //http://blog.csdn.net/rabbit729/article/details/4285119

            sCrossPoint = sCenter;

            FP nDis = sOffset.magnitude;
            TSVector2 d = sOffset.normalized;
            TSVector2 e = sCenter - sOrgPos;
            FP a = (e.x * d.x + e.y * d.y);
            FP f = a * a + nRadius * nRadius - e.LengthSquared();

            if (f >= 0)
            {
                FP s = TSMath.Sqrt(f);
                FP t1 = a - s;
                FP t2 = a + s;
                if (TSMath.Abs(t1) > TSMath.Abs(t2))
                {
                    FP fTemp = t1;
                    t1 = t2;
                    t2 = fTemp;
                }
                //射线原点在圆外
                if ((t1 >= 0))
                {
                    //如果第二个点在圆内
                    if ((t1 - nDis) <= 0)
                    {
                        sCrossPoint.x = sOrgPos.x + t1 * d.x;
                        sCrossPoint.y = sOrgPos.y + t1 * d.y;
                        return true;
                    }
                    //两个点都在圆外
                    else
                    {
                        return false;
                    }
                }

                //这里说明射线原点在圆内
                if (t2 >= 0)
                {
                    //如果与圆有碰撞
                    if ((t2 - nDis) <= 0)
                    {
                        sCrossPoint.x = sOrgPos.x + t2 * d.x;
                        sCrossPoint.y = sOrgPos.y + t2 * d.y;
                        return true;
                    }
                    //如果两个点都在圆内
                    else
                    {
                        sCrossPoint = sOrgPos;
                        return true;
                    }
                }

            }
            return false;
        }

        /// <summary>
        /// 检测圆与线段是否相交
        /// </summary>
        /// <param name="sOrgPos"></param>
        /// <param name="sOffset"></param>
        /// <param name="sCenter"></param>
        /// <param name="nRadius"></param>
        /// <returns></returns>
        public static bool CheckCicleAndLine(TSVector2 sOrgPos, ref TSVector2 sOffset, TSVector2 sCenter, FP nRadius)
        {
            TSVector2 sCrossPoint;
            if (!CheckCicleAndLine(sOrgPos, sOffset, sCenter, nRadius, out sCrossPoint))
            {
                return false;
            }
            sOffset = sCrossPoint - sOrgPos;
            return true;
        }


        //New 已测
        /// <summary>
        /// 检测点是否在圆内
        /// </summary>
        /// <param name="sCenter"></param>
        /// <param name="nRadius"></param>
        /// <param name="sPostion"></param>
        /// <returns></returns>
        public static bool CheckCicleAndPos(TSVector2 sCenter, FP nRadius, TSVector2 sPostion)
        {
            FP lDistance = (sCenter - sPostion).LengthSquared();
            return lDistance <= nRadius * nRadius;
        }

        //New 已测
        /// <summary>
        /// 检测圆与圆是否相交
        /// </summary>
        /// <param name="sCenter"></param>
        /// <param name="nRadius"></param>
        /// <param name="sCenter2"></param>
        /// <param name="nRadius2"></param>
        /// <returns></returns>
        public static bool CheckCircleAndCircle(TSVector2 sCenter, FP nRadius, TSVector2 sCenter2, FP nRadius2)
        {
            FP nDeltaX = sCenter.x - sCenter2.x;
            FP nDeltaY = sCenter.y - sCenter2.y;
            FP nRadiusAdd = nRadius + nRadius2;
            return nDeltaX * nDeltaX + nDeltaY * nDeltaY <= nRadiusAdd * nRadiusAdd;
        }

        ///// <summary>
        ///// 检测点是否在扇形区域内
        ///// </summary>
        ///// <param name="sCenter"></param>
        ///// <param name="sForward"></param>
        ///// <param name="nRadius"></param>
        ///// <param name="nAngle"></param>
        ///// <param name="sPos"></param>
        ///// <returns></returns>
        //public static bool CheckSectorAndPos(TSVector2 sCenter, TSVector2 sForward, FP nRadius, FP nAngle, TSVector2 sPos)
        //{
        //    if (sPos == sCenter)
        //    {
        //        return true;
        //    }
        //    TSVector2 sDir = sPos - sCenter;
        //    FP nTempAngle = TSVector2.Angle(sDir, sForward);

        //    if (nTempAngle <= nAngle / 2)
        //    {
        //        if ((sCenter - sPos).LengthSquared() < nRadius * nRadius)
        //        {
        //            return true;
        //        }
        //    }

        //    return false;
        //}



        // /// <summary>
        // /// 检测圆与扇形是否相交
        // /// </summary>
        // /// <param name="sCircleCenter"></param>
        // /// <param name="nCircleRadius"></param>
        // /// <param name="sSectorCenter"></param>
        // /// <param name="sSectorForward"></param>
        // /// <param name="nSectorRadius"></param>
        // /// <param name="nSectorAngle"></param>
        // /// <returns></returns>
        // public static bool CheckCircleAndSector(TSVector2 sCircleCenter, FP nCircleRadius, TSVector2 sSectorCenter, TSVector2 sSectorForward, FP nSectorRadius, FP nSectorAngle)
        // {
        //     // 1. 如果扇形圆心和圆盘圆心的方向能分离，两形状不相交
        //     TSVector2 sDir = sCircleCenter - sSectorCenter;

        //     FP nSum = nCircleRadius + nSectorRadius;
        //     if (sDir.LengthSquared() > nSum * nSum)
        //     {
        //         return false;
        //     }

        //     // 2. 计算出扇形局部空间的 p
        //     sSectorForward.Normalize();
        //     FP px = TSVector2.Dot(sDir, sSectorForward);
        //     FP py = TSMath.Abs(-sDir.x * sSectorForward.y + sDir.y * sSectorForward.x);

        //     // 3. 如果 p_x > ||p|| cos theta，两形状相交
        //     FP nTheta = nSectorAngle / 2;
        //     if (px > sDir.magnitude * TSMath.Cos(nTheta))
        //         return true;

        //     // 4. 求左边线段与圆盘是否相交
        //     TSVector2 q = nSectorRadius * new TSVector2(TSMath.Cos(nTheta), TSMath.Sin(nTheta));
        //     TSVector2 p = new TSVector2(px, py);
        //     FP t = TSVector2.Dot(p, q) / q.LengthSquared();
        //     FP nDis = (p - (TSMath.Clamp(t, 0, 1) * q)).LengthSquared();
        //     return nDis <= nCircleRadius * nCircleRadius;
        // }

        // /// <summary>
        // /// 矩形与扇形是否相交
        // /// </summary>
        // /// <param name="sCenter"></param>
        // /// <param name="sDir"></param>
        // /// <param name="nHalfWidth"></param>
        // /// <param name="nHalfHeight"></param>
        // /// <param name="sSectorCenter"></param>
        // /// <param name="sSectorForward"></param>
        // /// <param name="nSectorRadius"></param>
        // /// <param name="nSectorAngles"></param>
        // /// <returns></returns>
        // public static bool CheckRectangleAndSector(TSVector2 sCenter, TSVector2 sDir, FP nHalfWidth, FP nHalfHeight,
        //TSVector2 sSectorCenter, TSVector2 sSectorForward, FP nSectorRadius, FP nSectorAngles)
        // {
        //     throw new Exception("需要实现矩形与扇形的检测");
        // }
    }
}
