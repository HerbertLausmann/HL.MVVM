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
using System.ComponentModel;
using System.Text;
using System.Threading;
using System.Windows.Input;

namespace HL.MVVM.Async
{
    /// <summary>
    /// A Relay Command that is executed on a Thread apart that can be paused/resumed and cancelled. It also can report progress information;
    /// </summary>
    public class ThreadRelayCommand : ModelBase, ICommand
    {
        private ThreadRelayCommandExecute _ExecuteDelegate;
        private ThreadRelayCommandExecuteEventArgs _ExecuteEventArgs;
        private Predicate<object> _CanExecutePredicate;
        private Thread _Thread;
        private bool _IsRunning;

        /// <summary>
        /// The Task where's the Action is being runned
        /// </summary>
        public Thread Thread => _Thread;
        /// <summary>
        /// The running state of the background Task. True if is running. False if is not running.
        /// </summary>
        public bool IsRunning => _IsRunning;

        public ThreadRelayCommandExecute ExecuteDelegate { get { return _ExecuteDelegate; } }
        public Predicate<object> CanExecutePredicate { get { return _CanExecutePredicate; } }

        /// <summary>
        /// Initializes the RelayCommand with an Action to be executed. The CanExecutePredicate is optional.
        /// </summary>
        /// <param name="Action">An Action to be executed by this RelayCommand</param>
        /// <param name="CanExecutePredicate">If set, defines the method that will check for the CanExecute state of this RelayCommand</param>
        public ThreadRelayCommand(ThreadRelayCommandExecute Execute, Predicate<object> CanExecutePredicate = null)
        {
            _ExecuteDelegate = Execute;
            _CanExecutePredicate = CanExecutePredicate;
        }

        public bool CanExecute(object parameter)
        {
            if (_ExecuteDelegate == null) return false;
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
            _IsRunning = true;
            OnPropertyChanged("IsRunning");
            _ExecuteEventArgs = new ThreadRelayCommandExecuteEventArgs(this);
            _Thread = new Thread(() =>
            {
                _ExecuteDelegate.Invoke(_ExecuteEventArgs);
                _IsRunning = false;
                SafeInvoke(() => OnPropertyChanged("IsRunning"));
            });
            _Thread.IsBackground = true;
            _Thread.SetApartmentState(ApartmentState.MTA);
            _Thread.Start();
        }

        public void ForceRequery()
        {
            CommandManager.Requery(this);
        }

        public bool CanPause => (_ExecuteEventArgs != null) ? !_ExecuteEventArgs.IsPaused : false;
        public bool CanResume => (_ExecuteEventArgs != null) ? _ExecuteEventArgs.IsPaused : false;
        public bool CanCancel => (_ExecuteEventArgs != null) ? !_ExecuteEventArgs.IsCancelled : false;

        public uint Progress => (_ExecuteEventArgs != null) ? _ExecuteEventArgs.Progress : 0;
        public TimeSpan ExtimatedTimeLeft => (_ExecuteEventArgs != null) ? _ExecuteEventArgs.ExtimatedTimeLeft : TimeSpan.Zero;
        public string ProgressText => (_ExecuteEventArgs != null) ? _ExecuteEventArgs.ProgressText : string.Empty;

        public void Pause()
        {
            if (_ExecuteEventArgs != null && CanPause) _ExecuteEventArgs.ResumePause();
        }

        public void Resume()
        {
            if (_ExecuteEventArgs != null && CanResume) _ExecuteEventArgs.ResumePause();
        }

        public void Cancel()
        {
            if (_ExecuteEventArgs != null && CanCancel) _ExecuteEventArgs.Cancel();
        }

        public class ThreadRelayCommandExecuteEventArgs : EventArgs
        {
            public ThreadRelayCommandExecuteEventArgs(ThreadRelayCommand Parent)
            {
                _Parent = Parent;
            }
            private ThreadRelayCommand _Parent;
            private uint _Progress;
            private bool _IsCancelled;
            private TimeSpan _ExtimatedTimeLeft;
            private bool _IsPaused;
            private string _ProgressText;

            public uint Progress => _Progress;
            public string ProgressText => _ProgressText;
            public TimeSpan ExtimatedTimeLeft => _ExtimatedTimeLeft;
            public bool IsCancelled => _IsCancelled;
            public bool IsPaused => _IsPaused;

            public void ReportProgress(uint Progress)
            {
                _Progress = Progress;
                ModelBase.SafeInvoke(() =>
                {
                    _Parent.OnPropertyChanged("Progress");
                });
            }

            public void ReportProgressText(string ProgressText)
            {
                _ProgressText = ProgressText;
                ModelBase.SafeInvoke(() =>
                {
                    _Parent.OnPropertyChanged("ProgressText");
                });
            }

            public void ReportTimeLeft(TimeSpan TimeLeft)
            {
                _ExtimatedTimeLeft = TimeLeft;
                ModelBase.SafeInvoke(() =>
                {
                    _Parent.OnPropertyChanged("ExtimatedTimeLeft");
                });
            }

            internal void Cancel()
            {
                _IsCancelled = true;
                ModelBase.SafeInvoke(() =>
                {
                    _Parent.OnPropertyChanged("CanCancel");
                });
            }

            internal void ResumePause()
            {
                _IsPaused = !_IsPaused;
                ModelBase.SafeInvoke(() =>
                {
                    _Parent.OnPropertyChanged("CanPause");
                    _Parent.OnPropertyChanged("CanResume");
                });
            }

            public void InvokeOnSourceThread(Action Delegate)
            {
                ModelBase.SafeInvoke(Delegate);
            }
        }
        public delegate void ThreadRelayCommandExecute(ThreadRelayCommandExecuteEventArgs e);
    }
}
