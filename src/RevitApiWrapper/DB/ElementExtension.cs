using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitApiWrapper.DB
{
    public static class ElementExtension
    {
        public static T GetLocation<T>(this Element element) where T : Location
        {
            if (element.Location is null)
            {
                throw new ArgumentNullException(nameof(element));
            }
            return element.Location as T;
        }
    }
}