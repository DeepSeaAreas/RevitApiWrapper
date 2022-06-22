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
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using RevitApiWrapper.DB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RevitApiWrapper.UI
{
    public static class UiDocumentExtension
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uIDocument"></param>
        /// <param name="statusPrompt"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public static IList<T> PickElementsByCalss<T>(this UIDocument uIDocument, string statusPrompt) where T : Element
        {
            if (string.IsNullOrEmpty(statusPrompt))
            {
                throw new ArgumentException($"'{nameof(statusPrompt)}' cannot be null or empty.", nameof(statusPrompt));
            }
            if (uIDocument is null)
            {
                throw new ArgumentNullException(nameof(uIDocument));
            }

            try
            {
                var pickRefers = uIDocument.Selection.PickObjects(ObjectType.Element, new ClassSelectionFilter<T>(), statusPrompt);
                return pickRefers.Select(uIDocument.Document.GetElement<T>).OfType<T>().ToArray();
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {

            }
            return default;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uIDocument"></param>
        /// <param name="statusPrompt"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public static T PickElementByClass<T>(this UIDocument uIDocument, string statusPrompt) where T : Element
        {
            if (string.IsNullOrEmpty(statusPrompt))
            {
                throw new ArgumentException($"'{nameof(statusPrompt)}' cannot be null or empty.", nameof(statusPrompt));
            }
            if (uIDocument is null)
            {
                throw new ArgumentNullException(nameof(uIDocument));
            }
            try
            {
                var pickRefer = uIDocument.Selection.PickObject(ObjectType.Element, new ClassSelectionFilter<T>(), statusPrompt);
                return uIDocument.Document.GetElement<T>(pickRefer);
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {

            }
            return default;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uIDocument"></param>
        /// <param name="builtInCategory"></param>
        /// <param name="statusPrompt"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static T PickElementByCategory<T>(this UIDocument uIDocument, BuiltInCategory builtInCategory, string statusPrompt) where T : Element
        {
            if (uIDocument is null)
            {
                throw new ArgumentNullException(nameof(uIDocument));
            }
            if (builtInCategory == BuiltInCategory.INVALID)
            {
                throw new ArgumentOutOfRangeException(nameof(builtInCategory));
            }
            try
            {
                var pickRefer = uIDocument.Selection.PickObject(ObjectType.Element, new CategorySelectionFilter<T>(builtInCategory), statusPrompt);
                return uIDocument.Document.GetElement<T>(pickRefer);
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {

            }
            return default;
        }

        public static IList<T> PickElementsByCategory<T>(this UIDocument uIDocument, BuiltInCategory builtInCategory, string statusPrompt) where T : Element
        {
            if (string.IsNullOrEmpty(statusPrompt))
            {
                throw new ArgumentException($"'{nameof(statusPrompt)}' cannot be null or empty.", nameof(statusPrompt));
            }
            if (builtInCategory == BuiltInCategory.INVALID)
            {
                throw new ArgumentOutOfRangeException(nameof(builtInCategory));
            }
            if (uIDocument is null)
            {
                throw new ArgumentNullException(nameof(uIDocument));
            }

            try
            {
                var pickRefers = uIDocument.Selection.PickObjects(ObjectType.Element, new CategorySelectionFilter<T>(builtInCategory), statusPrompt);
                return pickRefers.Select(uIDocument.Document.GetElement<T>).OfType<T>().ToArray();
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {

            }
            return default;
        }
    }
}
