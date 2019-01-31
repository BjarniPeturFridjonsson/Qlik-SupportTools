using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using Eir.Common.Logging;

namespace FreyrViewer.Common
{
    public class ComponentDisposalHelper
    {
        private readonly Component _component;
        private readonly ConcurrentStack<Action> _disposedActions = new ConcurrentStack<Action>();

        public ComponentDisposalHelper(Component component)
        {
            _component = component;
            _component.Disposed += Component_Disposed;
        }

        private void Component_Disposed(object sender, EventArgs e)
        {
            Log.To.Main.Add($"Performing {_disposedActions.Count} dispose actions for component {_component}", LogLevel.Verbose);
            _component.Disposed -= Component_Disposed;
            Action action;
            while (_disposedActions.TryPop(out action))
            {
                try
                {
                    action();
                }
                catch (Exception ex)
                {
                    Log.To.Main.AddException("Error when performing dispose actions", ex);
                }
            }
        }

        public void AddDisposedAction(Action action)
        {
            _disposedActions.Push(action);
        }
    }
}