using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitApiWrapper
{
    public class Test
    {
        public Test()
        {

#if RVT_21_RELEASE || RVT_22_RELEASE
             UnitUtils.Convert(15, UnitTypeId.Feet, UnitTypeId.Millimeters);
#endif

#if RVT_19_RELEASE || RVT_20_RELEASE
            UnitUtils.Convert(15, DisplayUnitType.DUT_DECIMAL_FEET, DisplayUnitType.DUT_MILLIMETERS);
#endif
        }
    }
}
