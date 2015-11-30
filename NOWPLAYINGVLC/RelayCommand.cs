using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Diagnostics;
namespace NOWPLAYINGVLC
{
    // from MSDN: THE MODEL-VIEW-VIEWMODEL (MVVM) DESIGN PATTERN
    // https://msdn.microsoft.com/en-us/magazine/dd419663.aspx#id0090030
    public class RelayCommand : RelayCommand<object>
    {
        public RelayCommand(Action<object> execute) : base(execute) { }
        public RelayCommand(Action<object> execute, Predicate<object> canExecute) : base(execute, canExecute) { }
    }

    public class RelayCommand<T> : ICommand
    {
        #region Fields

        readonly Action<T> _execute;
        readonly Predicate<T> _canExecute;

        #endregion // Fields

        #region Constructors

        public RelayCommand(Action<T> execute) : this(execute, null)
        {
        }

        public RelayCommand(Action<T> execute, Predicate<T> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            _execute = execute;
            _canExecute = canExecute;
        }
        #endregion // Constructors

        #region ICommand Members

        [DebuggerStepThrough]
        bool ICommand.CanExecute(object parameter)
        {
            return CanExecute((parameter == null || !(parameter is T)) ? default(T) : (T)parameter);
        }

        [DebuggerStepThrough]
        public bool CanExecute(T parameter)
        {
            return _canExecute == null ? true : _canExecute(parameter);
        }

        event EventHandler ICommand.CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        void ICommand.Execute(object parameter)
        {
            Execute((parameter == null || !(parameter is T)) ? default(T) : (T)parameter);
        }

        public void Execute(T parameter)
        {
            _execute(parameter);
        }

        #endregion // ICommand Members
    }
}
