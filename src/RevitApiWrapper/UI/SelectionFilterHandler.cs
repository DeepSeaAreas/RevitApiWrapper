using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace RevitApiWrapper.UI
{
    public class SelectionFilterHandler : ISelectionFilter
    {
        private Func<Element, bool> ElementFilter { get; }
        private Func<Reference, XYZ, bool> ReferenceFilter { get; }

        public SelectionFilterHandler(Func<Element, bool> elementFilter,
            Func<Reference, XYZ, bool> referenceFilter = null)
        {
            ElementFilter = elementFilter;
            ReferenceFilter = referenceFilter;
        }

        public bool AllowElement(Element elem)
        {
            return null == ElementFilter || ElementFilter(elem);
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return null == ReferenceFilter || ReferenceFilter(reference, position);
        }
    }
}