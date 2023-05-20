/*Copyright (C) 2017 Herbert Lausmann. All rights reserved.
http://herbertdotlausmann.wordpress.com/
herbert.lausmann@hotmail.com

Redistribution and use in source and binary forms, with or without
modification, are allowed if the following conditions
are met:

 1. Redistributions of source code must retain the above copyright
    notice, this list of conditions and the following disclaimer.
 2. Redistributions in binary form must reproduce the above copyright
    notice and this list of conditions.    
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using System.Threading.Tasks;

namespace HL.MVVM.Async
{
    /// <summary>
    /// An async RelayCommand executes the Action in a separated Task object. Also provides a property to keep informed about the running state of the background work.
    /// </summary>
    public sealed class AsyncRelayCommand : ModelBase, IRelayCommand
    {
        private Action<object> _Action;
        private Predicate<object> _CanExecutePredicate;
        private Task _Task;
        private bool _IsRunning;

        /// <summary>
        /// The Task where's the Action is being runned
        /// </summary>
        public Task Task => _Task;
        /// <summary>
        /// The running state of the background Task. True if is running. False if is not running.
        /// </summary>
        public bool IsRunning => _IsRunning;

        public Action<object> Action { get { return _Action; } }
        public Predicate<object> CanExecutePredicate { get { return _CanExecutePredicate; } }

        /// <summary>
        /// Initializes the RelayCommand with an Action to be executed. The CanExecutePredicate is optional.
        /// </summary>
        /// <param name="Action">An Action to be executed by this RelayCommand</param>
        /// <param name="CanExecutePredicate">If set, defines the method that will check for the CanExecute state of this RelayCommand</param>
        public AsyncRelayCommand(Action<object> Action, Predicate<object> CanExecutePredicate = null)
        {
            _Action = Action;
            _CanExecutePredicate = CanExecutePredicate;
        }

        public bool CanExecute(object parameter)
        {
            if (_Action == null) return false;
            if (CanExecutePredicate != null)
                return IsRunning ? false : CanExecutePredicate.Invoke(parameter);
            else
                return !IsRunning;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            _Task = Task.Factory.StartNew(() =>
            {
                _IsRunning = true;

                SafeInvoke(() => OnPropertyChanged("IsRunning"));
                try
                {
                    _Action?.Invoke(parameter);
                }
                catch (Exception e)
                {
                    _IsRunning = false;
                    SafeInvoke(() => OnPropertyChanged("IsRunning"));
                    throw e;
                }
                _IsRunning = false;
                SafeInvoke(() => OnPropertyChanged("IsRunning"));
            }, TaskCreationOptions.LongRunning);
        }

        public void ForceRequery()
        {
            CommandManager.Requery(this);
        }
    }
}
