using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace RevitApiWrapper.Base
{
#if R2018 || R2019 || R2020

    /// <summary>
    /// 通过带单位的double类来解决不同单位转换的问题
    /// </summary>
    public class DoubleWithUnit
    {
        public double Value { get; set; }
        public DisplayUnitType Unit { get; private set; }

        public DoubleWithUnit(DisplayUnitType unit, double value = 0)
        {
            Value = UnitUtils.ConvertFromInternalUnits(value, unit);
            Unit = unit;
        }

        public DoubleWithUnit(DisplayUnitType unit, DisplayUnitType currentUnit, double value = 0)
        {
            Value = UnitUtils.Convert(value, currentUnit, unit);
            Unit = unit;
        }

        public double Get(DisplayUnitType unit)
        {
            return UnitUtils.Convert(Value, Unit, unit);
        }

        public void Set(double value, DisplayUnitType unit = DisplayUnitType.DUT_UNDEFINED)
        {
            if (unit is DisplayUnitType.DUT_UNDEFINED)
            {
                Value = UnitUtils.ConvertFromInternalUnits(value, Unit);
            }
            else
            {
                Value = UnitUtils.Convert(value, unit, Unit);
            }
        }

        public void SetUnit(DisplayUnitType unit)
        {
            Value = UnitUtils.Convert(Value, Unit, unit);
            Unit = unit;
        }

        public static bool operator ==(DoubleWithUnit value0, DoubleWithUnit value1)
        {
            value1.SetUnit(value0.Unit);
            if (Math.Abs(value0.Value - value1.Value) < 1e-5)
            {
                return true;
            }
            return false;
        }

        public static bool operator !=(DoubleWithUnit value0, DoubleWithUnit value1)
        {
            value1.SetUnit(value0.Unit);
            if (Math.Abs(value0.Value - value1.Value) >= 1e-5)
            {
                return true;
            }
            return false;
        }

        public override bool Equals(object value)
        {
            if (value is null && this is null)
            {
                return true;
            }
            else if (value is DoubleWithUnit value1)
            {
                value1.SetUnit(Unit);
                if (Math.Abs(Value - value1.Value) <= 1e-5)
                {
                    return true;
                }
                return false;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }

#else

    public class DoubleWithUnit
    {
        private Units units = new Units(UnitSystem.Metric);
        public double Value { get; set; }
        public ForgeTypeId Unit { get; private set; }

        public DoubleWithUnit(ForgeTypeId unit, double value = 0)
        {
            Value = UnitUtils.ConvertFromInternalUnits(value, unit);
            Unit = unit;
        }

        public DoubleWithUnit(ForgeTypeId unit,ForgeTypeId currentUnit, double value = 0)
        {
            Value = UnitUtils.Convert(value,currentUnit, unit);
            Unit = unit;
        }

        public double Get(ForgeTypeId unit)
        {
            return UnitUtils.Convert(Value, Unit, unit);
        }

        public void Set(double value, ForgeTypeId unit = null)
        {
            if (unit is null)
            {
                Value = UnitUtils.ConvertFromInternalUnits(value, Unit);
            }
            else
            {
                Value = UnitUtils.Convert(value, unit, Unit);
            }
        }

        public void SetUnit(ForgeTypeId unit)
        {
            Value = UnitUtils.Convert(Value, Unit, unit);
            Unit = unit;
        }

        public static bool operator ==(DoubleWithUnit value0, DoubleWithUnit value1)
        {
            value1.SetUnit(value0.Unit);
            if (Math.Abs(value0.Value - value1.Value) <= 1e-5)
            {
                return true;
            }
            return false;
        }

        public static bool operator !=(DoubleWithUnit value0, DoubleWithUnit value1)
        {
            value1.SetUnit(value0.Unit);
            if (Math.Abs(value0.Value - value1.Value) > 1e-5)
            {
                return true;
            }
            return false;
        }

        public override bool Equals(object value)
        {
            if (value is null && this is null)
            {
                return true;
            }
            else if (value is DoubleWithUnit value1)
            {
                value1.SetUnit(Unit);
                if (Math.Abs(Value - value1.Value) <= 1e-5)
                {
                    return true;
                }
                return false;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override string ToString()
        {
            return UnitFormatUtils.Format(units, Unit, Value, true);
        }
    }

#endif
}