using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace HL.MVVM
{
    /// <summary>
    /// Interface for all the RelayCommands
    /// </summary>
    public interface IRelayCommand : ICommand
    {
        /// <summary>
        /// Forces an update on the CanExecute state of this command
        /// </summary>
        void ForceRequery();
        /// <summary>
        /// The action to be executed by this command
        /// </summary>
        Action<object> Action { get;}
        /// <summary>
        /// The predicate used to define the CanExecute state of this command
        /// </summary>
        Predicate<object> CanExecutePredicate { get;}
    }
}
