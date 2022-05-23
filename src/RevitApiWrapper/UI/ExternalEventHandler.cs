using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitApiWrapper.UI
{
    internal class ExternalEventHandler : IExternalEventHandler
    {
        private Func<UIApplication, object> _func;
        public Func<UIApplication, object> Func
        {
            get => _func;
            set => _func = value ??throw new ArgumentNullException();
        }


        public event EventHandler<object> EventCompleted;

        public Exception Exception { get; private set; }


        public void Execute(UIApplication app)
        {
            object result = null;
            try
            {
                result = _func.Invoke(app);
            }
            catch (Exception e)
            {
                Exception = e;
            }
            EventCompleted?.Invoke(this, result);
        }

        public string GetName()
        {
            return nameof(ExternalEventHandler);
        }
    }
}
