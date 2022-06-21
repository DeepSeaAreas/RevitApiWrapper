#region Headers
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
#if R2019 || R2020 || R2018
            return UnitUtils.ConvertToInternalUnits(number, DisplayUnitType.DUT_MILLIMETERS);
#else
            return UnitUtils.ConvertToInternalUnits(number, UnitTypeId.Millimeters);
#endif

        }

        public static double FeetToMillimeter(this double number)
        {
#if R2019 || R2020 || R2018
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


    }
}
