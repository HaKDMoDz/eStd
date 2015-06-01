using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace System.Wpf.Mvvm.Commands
{
    public class DelegateCommand : ICommand
    {
        private readonly Action<object> _execute;

        private readonly Predicate<object> _canExetute;

        public DelegateCommand(Action<object> execute, Predicate<object> canExetute)
        {
            this._execute = execute;
            this._canExetute = canExetute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExetute(parameter);
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }

        public static AsyncDelegateCommand FromAsyncHandler(Func<object, Task> _execute, Predicate<object> _canExecute)
        {
            return new AsyncDelegateCommand(_execute, _canExecute);
        }

        public void RaiseCanExecuteChanged()
        {
            var handler = CanExecuteChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        public event EventHandler CanExecuteChanged;
    }
}