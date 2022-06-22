#region Headers
/* ____________________________________________________________
*   DESCRIPTION: UiDocumentExtension
*   AUTHOR: Young
*   CREARETIME: 6/22/2022 8:35:55 PM 
*   CLRVERSION: 4.0.30319.42000
*  ____________________________________________________________
*/
#endregion

using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;

namespace RevitApiWrapper.UI
{
    public class ClassSelectionFilter<T> : ISelectionFilter where T : Element
    {
        public bool AllowElement(Element elem)
        {
            return elem is T;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return true;
        }
    }

    public class CategorySelectionFilter<T> : ISelectionFilter where T : Element
    {
        private readonly BuiltInCategory _builtInCategory;
        public CategorySelectionFilter(BuiltInCategory builtInCategory)
        {
            _builtInCategory = builtInCategory;
        }
        public bool AllowElement(Element elem)
        {
            return elem is T && (elem.Category?.Id?.IntegerValue ?? -1) == (int)_builtInCategory;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return true;
        }
    }
}
