using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace HL.MVVM
{
    /// <summary>
    /// A simple RelayCommand with an Action to execute and a Predicate to query the CanExecute value.
    /// </summary>
    public sealed class RelayCommand : IRelayCommand
    {
        private Action<object> _Action;
        private Predicate<object> _CanExecutePredicate;

        public Action<object> Action { get { return _Action; } }
        public Predicate<object> CanExecutePredicate { get { return _CanExecutePredicate; } }

        /// <summary>
        /// Initializes the RelayCommand with an Action to be executed. The CanExecutePredicate is optional.
        /// </summary>
        /// <param name="Action">An Action to be executed by this RelayCommand</param>
        /// <param name="CanExecutePredicate">If set, defines the method that will check for the CanExecute state of this RelayCommand</param>
        public RelayCommand(Action<object> Action, Predicate<object> CanExecutePredicate = null)
        {
            _Action = Action;
            _CanExecutePredicate = CanExecutePredicate;
        }

        public bool CanExecute(object parameter)
        {
            if (_Action == null) return false;
            if (CanExecutePredicate != null)
                return CanExecutePredicate.Invoke(parameter);
            else
                return true;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            _Action?.Invoke(parameter);
        }

        public void ForceRequery()
        {
            CommandManager.Requery(this);
        }
    }
}
