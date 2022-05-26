#region Headers
/* ____________________________________________________________
*   DESCRIPTION: DocumentExtension
*   AUTHOR: Young
*   CREARETIME: 5/19/2022 8:35:55 PM 
*   CLRVERSION: 4.0.30319.42000
*  ____________________________________________________________
*/
#endregion
using Autodesk.Revit.DB;
using System;

namespace RevitApiWrapper.DB
{
    public static class DocumentExtension
    {
        public static T GetElement<T>(this Document doc, ElementId id) where T : Element
        {
            if (doc is null)
            {
                throw new ArgumentNullException(nameof(doc));
            }
            if (ElementId.InvalidElementId == id)
            {
                throw new ArgumentNullException(nameof(id));
            }
            return doc.GetElement(id) as T;
        }

        public static T GetElement<T>(this Document doc, int id) where T : Element
        {
            if (doc is null)
            {
                throw new ArgumentNullException(nameof(doc));
            }
            if (ElementId.InvalidElementId.IntegerValue == id)
            {
                throw new ArgumentNullException(nameof(id));
            }
            return doc.GetElement(new ElementId(id)) as T;
        }

        public static T GetElement<T>(this Document doc, string guid) where T : Element
        {
            if (doc is null)
            {
                throw new ArgumentNullException(nameof(doc));
            }
            if (string.IsNullOrEmpty(guid))
            {
                throw new ArgumentNullException(nameof(guid));
            }
            return doc.GetElement(guid) as T;
        }

        public static T GetElement<T>(this Document doc, Reference reference) where T : Element
        {
            if (doc is null)
            {
                throw new ArgumentNullException(nameof(doc));
            }
            if (reference is null)
            {
                throw new ArgumentNullException(nameof(reference));
            }
            return doc.GetElement(reference) as T;
        }

        public static bool RunTransaction(this Document doc, string transactionName, Action action, IFailuresPreprocessor failuresPreprocessor = null)
        {
            if (doc == null)
            {
                throw new ArgumentNullException(nameof(doc));
            }
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }
            transactionName = string.IsNullOrEmpty(transactionName) ? Guid.NewGuid().ToString() : transactionName;
            using (var transaction = new Transaction(doc, transactionName))
            using (var handlerOptions = transaction.GetFailureHandlingOptions())
            {
                var transStatus = TransactionStatus.Uninitialized;
                handlerOptions.SetFailuresPreprocessor(failuresPreprocessor ?? new FailuresPreprocessor());
                handlerOptions.SetClearAfterRollback(true);
                handlerOptions.SetDelayedMiniWarnings(false);
                transaction.SetFailureHandlingOptions(handlerOptions);
                try
                {
                    transaction.Start();
                    action.Invoke();
                    transStatus = transaction.Commit();
                    return transStatus == TransactionStatus.Committed;
                }
                catch (Exception e)
                {
                    if (transStatus == TransactionStatus.Started)
                    {
                        transaction.RollBack();
                    }
                    return false;
                }
            }
        }
        internal class FailuresPreprocessor : IFailuresPreprocessor
        {
            public FailureProcessingResult PreprocessFailures(FailuresAccessor failuresAccessor)
            {
                failuresAccessor.DeleteAllWarnings();
                return FailureProcessingResult.Continue;
            }
        }
    }
}
