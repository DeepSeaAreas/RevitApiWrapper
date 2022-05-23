using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Threading.Tasks;

namespace RevitApiWrapper.UI
{
    /// <summary>
    ///  reference:https://github.com/WhiteSharq/RevitTask
    /// </summary>
    public class ExternalEventHelper : IDisposable
    {
        private ExternalEventHandler _externalEventHandler;
        private TaskCompletionSource<object> _taskCompletionSource;
        private readonly ExternalEvent _externalEvent;

        public ExternalEventHelper()
        {
            _externalEventHandler = new ExternalEventHandler();
            _externalEventHandler.EventCompleted += OnEventCompleted;
            _externalEvent = ExternalEvent.Create(_externalEventHandler);
        }

        private void OnEventCompleted(object sender, object result)
        {
            if (_externalEventHandler.Exception is null)
            {
                _taskCompletionSource.SetResult(result);
            }
            else
            {
                _taskCompletionSource.TrySetException(_externalEventHandler.Exception);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        public Task<T> RunAsync<T>(Func<UIApplication, T> func) where T : Element
        {
            _taskCompletionSource = new TaskCompletionSource<object>();
            var task = Task.Run(async () => (T)await _taskCompletionSource.Task);
            _externalEventHandler.Func = (uiApp) => func.Invoke(uiApp);
            _externalEvent.Raise();
            return task;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public Task RunAsync(Action<UIApplication> action)
        {
            _taskCompletionSource = new TaskCompletionSource<object>();
            _externalEventHandler.Func = (uiApp) => { action.Invoke(uiApp); return new object(); };
            _externalEvent.Raise();

            return _taskCompletionSource.Task;
        }

        public void Dispose()
        {
            using (_externalEvent)
            {
                _taskCompletionSource = null;
                _externalEventHandler = null;
            }
        }
    }

}
