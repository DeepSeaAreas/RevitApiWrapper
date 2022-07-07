#region Headers
/* ____________________________________________________________
*   DESCRIPTION: GeometryExtension
*   AUTHOR: Young
*   CREARETIME: 6/19/2022 8:35:55 PM 
*   CLRVERSION: 4.0.30319.42000
*  ____________________________________________________________
*/
#endregion

using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace RevitApiWrapper.DB
{
    public static class GeometryExtension
    {
        #region Curve

        /// <summary>
        /// 
        /// </summary>
        /// <param name="curve"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static XYZ GetStartPoint(this Curve curve)
        {
            if (curve is null)
            {
                throw new ArgumentNullException(nameof(curve));
            }
            return curve.GetEndPoint(0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="curve"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static XYZ GetEndPoint(this Curve curve)
        {
            if (curve is null)
            {
                throw new ArgumentNullException(nameof(curve));
            }
            return curve.GetEndPoint(1);
        }

        public static XYZ GetMiddlePoint(this Curve curve)
        {
            if (curve is null)
            {
                throw new ArgumentNullException(nameof(curve));
            }
            return curve.Evaluate(0.5, true);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IList<XYZ> GetIntersectPoints(this Curve source, Curve target)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (target is null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            var result = source.Intersect(target, out var resultArray);
            if (result == SetComparisonResult.Overlap)
            {
                return resultArray.OfType<IntersectionResult>().Select(i => i.XYZPoint).ToArray();
            }
            return default;
        }

        public static IList<XYZ> GetIntersectPoints(this IList<Curve> curves)
        {
            if (curves is null)
            {
                throw new ArgumentNullException(nameof(curves));
            }
            var points = new List<XYZ>();
            for (int i = curves.Count - 1; i > 0; i--)
            {
                for (int j = i - 1; j >= 0; j--)
                {
                    if (curves[i].Intersect(curves[j], out var resultArray) == SetComparisonResult.Overlap)
                    {
                        for (int k = 0; k < resultArray.Size; k++)
                        {
                            points.Add(resultArray.get_Item(k).XYZPoint);
                        }
                    }
                }
            }
            return points;
        }

        /// <summary>
        /// 将首尾闭合的曲线进行首尾相连排序
        /// </summary>
        /// <param name="curves"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="Exception"></exception>
        public static void SortCurvesContiguous(this IList<Curve> curves)
        {
            var sixteenth = 1d / 12d / 16d;
            if (curves is null)
            {
                throw new ArgumentNullException(nameof(curves));
            }
            var curveCount = curves.Count;
            for (int i = 0; i < curveCount; i++)
            {
                var curve = curves[i];
                var endPoint = curve.GetEndPoint();

                var found = i + 1 >= curveCount;
                for (int j = i + 1; j < curveCount; ++j)
                {
                    var point = curves[j].GetStartPoint();

                    if (point.DistanceTo(endPoint).IsGreaterThan(sixteenth))
                    {
                        if (i + 1 != j)
                        {
                            var tempCurve = curves[i + 1];
                            curves[i + 1] = curves[j];
                            curves[j] = tempCurve;
                        }
                        found = true;
                        break;
                    }
                    point = curves[j].GetEndPoint();
                    if (point.DistanceTo(endPoint).IsGreaterThan(sixteenth))
                    {
                        if (i + 1 == j)
                        {
                            curves[i + 1] = curves[j].CreateReversedCurve();
                        }
                        else
                        {
                            var tempCurve = curves[i + 1];
                            curves[i + 1] = curves[j].CreateReversedCurve();
                            curves[j] = tempCurve;
                        }
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    throw new Exception("SortCurvesContiguous:" + " non-contiguous input curves");
                }
            }
        }

        private static Curve CreateReversedCurve(this Curve curve)
        {
            if (curve is null)
            {
                throw new ArgumentNullException(nameof(curve));
            }
            if (!(curve is Line || curve is Arc))
            {
                throw new NotImplementedException($"CreateReversedCurve for type {curve.GetType().Name}");
            }
            switch (curve)
            {
                case Line line:
                    return Line.CreateBound(line.GetEndPoint(), line.GetStartPoint());
                case Arc arc:
                    return Arc.Create(arc.GetEndPoint(), arc.GetStartPoint(), arc.Evaluate(0.5, true));
            }
            throw new Exception("CreateReversedCurve - Unreachable");
        }
        #endregion

        #region CurveLoop
        /// <summary>
        /// 通过向量对封闭路径进行偏移(扩大)
        /// </summary>
        /// <param name="curveLoop"></param>
        /// <param name="offset">扩大的值</param>
        /// <returns></returns>
        /// <remarks>
        /// 第二种方法：来源:https://blog.csdn.net/happy__888/article/details/315762 
        /// 此方法适用于平行直线结合形成的折线多边形的原位缩小与放大
        /// 通过缩放前点与缩放后的点及两点形成的向量(a,b)可组成四边相同的菱形形状
        /// 平行四边形面积：|a X b| = |a|*|b|*sin(θ) ,
        /// 已知偏移的长度，及两条平行线段的垂线距离L，可以在菱形边做垂线获得一个直角三角形
        /// 通过三角形定理，L/Lb = sin(Π - θ) 又 sin(Π - θ) = sin(θ)
        /// 所以可以得到最终公式: Lb = L/|a X b|/|a|/|b|
        /// 又最终点是起点(Ps) + 向量方向(a + b) * 向量长度(Lb)[因为之前求取向量将向量简化为单位向量，所以起点到中点的距离应该是菱形边会到未
        /// 单位化之前的值 及 normal(Lb) / Lb = normal(LTargetPoint) / LTargetPoint]
        /// 所以通过上述公式可以将最终点求出
        /// </remarks>
        public static CurveLoop OffsetPath(this CurveLoop curveLoop, double offset)
        {
            if (curveLoop is null)
            {
                throw new ArgumentNullException(nameof(curveLoop));
            }

            var vertices = new List<XYZ>();
            var newVertices = new List<XYZ>();
            //因为Revit中顶点都是逆时针排序，只需要取出点即可
            foreach (var curve in curveLoop)
            {
                vertices.Add(curve.GetEndPoint(0));
            }
            //每个点遍历获取前一个点与后一个点，获取两个向量，此处位置的**向量方向会与缩放形式有关**
            for (int i = 0; i < vertices.Count; i++)
            {
                int iPrevious;
                int iEnd;
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
                var pPrevious = vertices[iPrevious];
                var point = vertices[i];
                var pEnd = vertices[iEnd];

                //normalize
                var v1 = (pPrevious - point).Normalize();
                var v2 = (pEnd - point).Normalize();
                var cross = v1.X * v2.Y - v1.Y * v2.X;//叉积 , v1 , v2单位向量模为1
                if (cross.IsAlmostEqualZero())
                {
                    continue;
                }
                double lb = offset / cross;
                var tPoint = point + lb * (v1 + v2);
                newVertices.Add(tPoint);
            }
            //output 
            var loop = new CurveLoop();
            for (int i = 0; i < newVertices.Count; i++)
            {
                if (i == newVertices.Count - 1)
                {
                    loop.Append(Line.CreateBound(newVertices[i], newVertices[0]));
                }
                else
                {
                    loop.Append(Line.CreateBound(newVertices[i], newVertices[i + 1]));
                }
            }
            return loop;
        }
        /// <summary>
        /// 获取路径的点
        /// <remarks>
        /// 输入CurveLoop可以使用方法IsCounterclockwise判定为顺时针或逆时针
        /// </remarks>
        /// </summary>
        /// <param name="curveLoop"></param>
        /// <returns>CurveLoop点集</returns>
        public static List<XYZ> GetVertexesFromCurveLoop(this CurveLoop curveLoop)
        {
            List<XYZ> res = new List<XYZ>();
            var loop = curveLoop.GetEnumerator();
            while (loop.MoveNext())
                res.Add((loop.Current as Curve)?.GetStartPoint());
            return res;

        }
        #endregion
        #region XYZ
        /// <summary>
        /// 修改Z值
        /// </summary>
        /// <param name="point">宿主</param>
        /// <param name="elevation">目标值</param>
        /// <returns>修改后的Point值</returns>
        public static XYZ SetZ(this XYZ point , double elevation)
        {
            return new XYZ(point.X,point.Y, elevation);
        }

        #endregion
        #region UV
        /// <summary>
        /// 返回BoundingBoxUV的中心UV位置
        /// </summary>
        /// <param name="uv"></param>
        /// <returns></returns>
        public static UV GetMiddleUV(this BoundingBoxUV uv)
        {
            var maxUV = uv.Max;
            var minUV = uv.Min;
            var maxU = maxUV.U;
            var maxV = maxUV.V;
            var minU = minUV.U;
            var minV = minUV.V;
            return new UV((maxU + minU) / 2, (minV + maxV) / 2);
        }
        #endregion
    }
}
