﻿#region Headers
/* ____________________________________________________________
*   DESCRIPTION: MathExtension
*   AUTHOR: Young
*   CREARETIME: 5/19/2022 8:35:55 PM 
*   CLRVERSION: 4.0.30319.42000
*  ____________________________________________________________
*/
#endregion
using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitApiWrapper.DB
{
    public static class MathExtension
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static bool IsAlmostEqualZero(this double source, double tolerance = 1e-5)
        {
            return Math.Abs(source) <= tolerance;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static bool IsAlmostEqual(this double source, double target, double tolerance = 1e-5)
        {
            return Math.Abs(source - target) <= tolerance;
        }

        public static double MillimeterToFeet(this double number)
        {
#if R2019 || R2020
            return UnitUtils.ConvertToInternalUnits(number, DisplayUnitType.DUT_MILLIMETERS);
#else
            return UnitUtils.ConvertToInternalUnits(number, UnitTypeId.Millimeters);
#endif

        }

        public static double FeetToMillimeter(this double number)
        {
#if R2019 || R2020
            return UnitUtils.ConvertFromInternalUnits(number, DisplayUnitType.DUT_MILLIMETERS);
#else
            return UnitUtils.ConvertFromInternalUnits(number, UnitTypeId.Millimeters);
#endif

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static bool IsGreaterThan(this double source, double target, double tolerance = 1e-5)
        {
            return source - target > tolerance;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static bool IsGreaterThanOrEqualWith(this double source, double target, double tolerance = 1e-5)
        {
            return source - target >= -tolerance;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static bool IsLessThan(this double source, double target, double tolerance = 1e-5)
        {
            return !IsGreaterThanOrEqualWith(source, target, tolerance);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static bool IsLessThanOrEqualWith(this double source, double target, double tolerance = 1e-5)
        {
            return !IsGreaterThan(source, target, tolerance);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static double RadianToAngle(this double number)
        {
            return number * 180d / Math.PI;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static double AngelToRadian(this double number)
        {
            return number * Math.PI / 180d;
        }

        /// <summary>
        /// 通过向量对封闭路径进行偏移(扩大)
        /// </summary>
        /// <param name="array"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        public static CurveLoop OffsetPath(CurveLoop array, double k)
        {

            /*
             * 第二种方法：来源:https://blog.csdn.net/happy__888/article/details/315762
             * 此方法适用于平行直线结合形成的折线多边形的原位缩小与放大
             * 通过缩放前点与缩放后的点及两点形成的向量(a,b)可组成四边相同的菱形形状
             * 平行四边形面积：|a X b| = |a|*|b|*sin(θ) ,
             * 已知偏移的长度，及两条平行线段的垂线距离L，可以在菱形边做垂线获得一个直角三角形
             * 通过三角形定理，L/Lb = sin(Π - θ) 又 sin(Π - θ) = sin(θ)
             * 所以可以得到最终公式: Lb = L/|a X b|/|a|/|b|
             * 又最终点是起点(Ps) + 向量方向(a + b) * 向量长度(Lb)[因为之前求取向量将向量简化为单位向量，所以起点到中点的距离应该是菱形边会到未
             * 单位化之前的值 及 normal(Lb) / Lb = normal(LTargetPoint) / LTargetPoint]
             * 所以通过上述公式可以将最终点求出
             * 
             */

            var vertices = new List<XYZ>();
            var newVertices = new List<XYZ>();
            //因为Revit中顶点都是逆时针排序，只需要取出点即可
            foreach (var curve in array)
            {
                vertices.Add(curve.GetEndPoint(0));
            }
            //每个点遍历获取前一个点与后一个点，获取两个向量，此处位置的**向量方向会与缩放形式有关**
            for (int i = 0; i < vertices.Count; i++)
            {
                int iPrevious = -1;
                int iEnd = -1;
                if (i == 0)
                {
                    iPrevious = vertices.Count - 1;
                    iEnd = i + 1;
                }
                else if (i == vertices.Count - 1)
                {
                    iPrevious = i - 1;
                    iEnd = 0;
                }
                else
                {
                    iPrevious = i - 1;
                    iEnd = i + 1;
                }


                XYZ pPrevious = vertices[iPrevious];
                XYZ point = vertices[i];
                XYZ pEnd = vertices[iEnd];

                //normalize
                var v1 = (pPrevious - point).Normalize();
                var v2 = (pEnd - point).Normalize();
                var cross = (v1.X * v2.Y - v1.Y * v2.X);//叉积 , v1 , v2单位向量模为1
                var lb = 0.00;
                if (cross == 0)
                    continue;
                lb = k / cross;
                var tPoint = point + lb * (v1 + v2);
                newVertices.Add(tPoint);

            }


            //output 
            var loop = new CurveLoop();
            for (int i = 0; i < newVertices.Count; i++)
            {
                if (i == newVertices.Count - 1)
                {
                    var c = Line.CreateBound(newVertices[i], newVertices[0]);
                    loop.Append(c);
                }
                else
                {
                    var c = Line.CreateBound(newVertices[i], newVertices[i + 1]);
                    loop.Append(c);
                }
            }

            return loop;
        }
    }
}
