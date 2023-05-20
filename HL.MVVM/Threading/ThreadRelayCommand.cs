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
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Input;

namespace HL.MVVM.Threading
{
    /// <summary>
    /// A Relay Command that is executed on a Thread apart which can be paused/resumed and cancelled. It also can report progress and result information
    /// </summary>
    public class ThreadRelayCommand : ModelBase, ICommand
    {
        private ThreadRelayCommandExecute _ExecuteDelegate;
        private ThreadRelayCommandExecuteEventArgs _ExecuteEventArgs;
        private Predicate<object> _CanExecutePredicate;
        private Thread _Thread;
        private bool _IsRunning;

        /// <summary>
        /// The Thread where's the ThreadRelayCommandExecute is being runned
        /// </summary>
        public Thread Thread => _Thread;
        /// <summary>
        /// The running state of the background Thread. True if is running. False if is not running.
        /// </summary>
        public bool IsRunning => _IsRunning;

        /// <summary>
        /// The ThreadRelayCommandExecute to be executed
        /// </summary>
        public ThreadRelayCommandExecute ExecuteDelegate { get { return _ExecuteDelegate; } }

        /// <summary>
        /// The CanExecute predicate
        /// </summary>
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
            if (IsRunning) throw new InvalidOperationException("This command is already executing in background!!!");
            _IsRunning = true;
            OnPropertyChanged("IsRunning");
            _ExecuteEventArgs = new ThreadRelayCommandExecuteEventArgs(this, parameter);
            _Thread = new Thread(() =>
            {
                lock (_ExecuteEventArgs)
                {
                    try
                    {

                        _ExecuteDelegate.Invoke(_ExecuteEventArgs);

                    }
                    catch (Exception e)
                    {
                        _ExecuteEventArgs.ReportResult(ThreadRelayCommandExecuteResult.Error, e.Message, e);
                    }
                }
                _IsRunning = false;
                SafeInvoke(() => OnPropertyChanged("IsRunning"));
            });
            _Thread.IsBackground = true;
            _Thread.SetApartmentState(ApartmentState.STA);
            _Thread.Start();
            OnPropertyChanged("CanPause");
            OnPropertyChanged("CanResume");
            OnPropertyChanged("CanCancel");
        }

        public void ForceRequery()
        {
            CommandManager.Requery(this);
        }
        /// <summary>
        /// Check whether the undergoing thread can be paused
        /// </summary>
        public bool CanPause => (_ExecuteEventArgs != null && IsRunning && !_ExecuteEventArgs.IsCancelled) ? !_ExecuteEventArgs.IsPaused && _ExecuteEventArgs.Pausable : false;
        /// <summary>
        /// Check whether the undergoing thread can be resumed
        /// </summary>
        public bool CanResume => (_ExecuteEventArgs != null && IsRunning && !_ExecuteEventArgs.IsCancelled) ? _ExecuteEventArgs.IsPaused && _ExecuteEventArgs.Pausable : false;
        /// <summary>
        /// Check whether the undergoing thread can be cancelled
        /// </summary>
        public bool CanCancel => (_ExecuteEventArgs != null && IsRunning && !_ExecuteEventArgs.IsCancelled) ? !_ExecuteEventArgs.IsCancelled : false;

        /// <summary>
        /// Returns the progress reported by the work being executed
        /// </summary>
        public double Progress => (_ExecuteEventArgs != null) ? _ExecuteEventArgs.Progress : 0;
        /// <summary>
        /// Returns the maximum progress of the work being executed
        /// </summary>
        public double TotalProgress => (_ExecuteEventArgs != null) ? _ExecuteEventArgs.TotalProgress : 0;
        /// <summary>
        /// Returns the extimated time left reported by the work being executed
        /// </summary>
        public TimeSpan ExtimatedTimeLeft => (_ExecuteEventArgs != null) ? _ExecuteEventArgs.ExtimatedTimeLeft : TimeSpan.Zero;
        /// <summary>
        /// Returns the progress text reported by the work being executed
        /// </summary>
        public string ProgressText => (_ExecuteEventArgs != null) ? _ExecuteEventArgs.ProgressText : string.Empty;
        /// <summary>
        /// Returns the result containg interesting data about the work which was executing on background
        /// </summary>
        public ThreadRelayCommandExecuteResultData Result => (_ExecuteEventArgs != null) ? _ExecuteEventArgs.Result : null;

        /// <summary>
        /// Pauses, if possible, the undergoing work
        /// </summary>
        public void Pause()
        {
            if (_ExecuteEventArgs != null && CanPause) _ExecuteEventArgs.ResumePause();
        }

        /// <summary>
        /// Resumes, if possible, the undergoing work
        /// </summary>
        public void Resume()
        {
            if (_ExecuteEventArgs != null && CanResume) _ExecuteEventArgs.ResumePause();
        }

        /// <summary>
        /// Cancels, if possible, the undergoing work
        /// </summary>
        public void Cancel()
        {
            if (_ExecuteEventArgs != null && CanCancel) _ExecuteEventArgs.Cancel();
        }

        /// <summary>
        /// Event data for the ThreadRelayCommandExecute delegate
        /// </summary>
        public class ThreadRelayCommandExecuteEventArgs : EventArgs
        {
            internal ThreadRelayCommandExecuteEventArgs(ThreadRelayCommand Parent, object param)
            {
                _Parent = Parent;
                _Parameter = param;
            }

            private ThreadRelayCommand _Parent;
            private double _Progress;
            private double _TotalProgress;
            private bool _IsCancelled;
            private TimeSpan _ExtimatedTimeLeft;
            private bool _IsPaused;
            private string _ProgressText;
            private ThreadRelayCommandExecuteResultData _Result;
            private object _Parameter;
            private bool _Pausable = true;

            /// <summary>
            /// Gets or sets a value indicating whether this work can be paused. This property is set to true by default.
            /// </summary>
            public bool Pausable
            {
                get => _Pausable;
                set
                {
                    _Pausable = value;
                    ModelBase.SafeInvoke(() =>
                    {
                        _Parent.OnPropertyChanged("CanPause");
                        _Parent.OnPropertyChanged("CanResume");
                    });
                }
            }

            /// <summary>
            /// Returns the current reported progress
            /// </summary>
            public double Progress => _Progress;
            /// <summary>
            /// Returns the current maximum progress reported
            /// </summary>
            public double TotalProgress => _TotalProgress;
            /// <summary>
            /// Returns the current reported progress text
            /// </summary>
            public string ProgressText => _ProgressText;
            /// <summary>
            /// Returns the current reported extimated time left
            /// </summary>
            public TimeSpan ExtimatedTimeLeft => _ExtimatedTimeLeft;
            /// <summary>
            /// Returns a value indicating whether there is a Cancellation request for the current work
            /// </summary>
            public bool IsCancelled => _IsCancelled;
            /// <summary>
            /// Returns a value indicating whether there is a Pause request for the current work
            /// </summary>
            public bool IsPaused
            {
                get
                {
                    if (Pausable) return _IsPaused;
                    else return false;
                }
            }

            /// <summary>
            /// Returns the reported work's result
            /// </summary>
            public ThreadRelayCommandExecuteResultData Result => _Result;
            /// <summary>
            /// Returns the CommandParameter sent to this command
            /// </summary>
            public object Parameter => _Parameter;

            /// <summary>
            /// Reports the current progress
            /// </summary>
            /// <param name="Progress">A double value indicating the current progress</param>
            public void ReportProgress(double Progress)
            {
                _Progress = Progress;
                ModelBase.SafeInvoke(() =>
                {
                    _Parent.OnPropertyChanged("Progress");
                });
            }

            /// <summary>
            /// Reports the maximum possible progress
            /// </summary>
            /// <param name="TotalProgress">A double value indicating the maximum possible progress</param>
            public void ReportTotalProgress(double TotalProgress)
            {
                _TotalProgress = TotalProgress;
                ModelBase.SafeInvoke(() =>
                {
                    _Parent.OnPropertyChanged("TotalProgress");
                });
            }

            /// <summary>
            /// Reports a progress text aka, status text
            /// </summary>
            /// <param name="ProgressText">The progress text</param>
            public void ReportProgressText(string ProgressText)
            {
                _ProgressText = ProgressText;
                //retirar depois
                System.Diagnostics.Debug.WriteLine(DateTime.Now.ToShortDateString() + " | " + DateTime.Now.ToLongTimeString() + " - " + ProgressText);

                ModelBase.SafeInvoke(() =>
                {
                    _Parent.OnPropertyChanged("ProgressText");
                });
            }
            /// <summary>
            /// Reports the extimated time left for this work to be concluded
            /// </summary>
            /// <param name="TimeLeft">A TimeSpan containg the expected value</param>
            public void ReportTimeLeft(TimeSpan TimeLeft)
            {
                _ExtimatedTimeLeft = TimeLeft;
                ModelBase.SafeInvoke(() =>
                {
                    _Parent.OnPropertyChanged("ExtimatedTimeLeft");
                });
            }

            /// <summary>
            /// Reports a result for this work.
            /// </summary>
            /// <param name="result">A value indicating if this work has completed or was cancelled or had some error</param>
            /// <param name="Message">Some message related to the result</param>
            /// <param name="Exception">The exception that might had happened</param>
            public void ReportResult(ThreadRelayCommandExecuteResult result = ThreadRelayCommandExecuteResult.Completed, string Message = null, Exception Exception = null)
            {
                ModelBase.SafeInvoke(() =>
                {
                    _Result = new ThreadRelayCommandExecuteResultData(result, Exception, Message);
                    _Parent.OnPropertyChanged("Result");
                });
            }

            internal void Cancel()
            {
                _IsCancelled = true;
                ModelBase.SafeInvoke(() =>
                {
                    _Parent.OnPropertyChanged("CanCancel");
                    _Parent.OnPropertyChanged("CanPause");
                    _Parent.OnPropertyChanged("CanResume");
                });
            }

            internal void ResumePause()
            {
                if (!Pausable) return;
                _IsPaused = !_IsPaused;
                ModelBase.SafeInvoke(() =>
                {
                    _Parent.OnPropertyChanged("CanPause");
                    _Parent.OnPropertyChanged("CanResume");
                });
            }

            /// <summary>
            /// Useful when pausing the work. When pausing a loop, you can use Sleep() to free processor time in the working Thread.
            /// </summary>
            public void Sleep()
            {
                System.Threading.Thread.Sleep(0);
            }

            /// <summary>
            /// Freezes the Command Thread for the time specified.
            /// </summary>
            public void Sleep(int time)
            {
                System.Threading.Thread.Sleep(time);
            }

            /// <summary>
            /// Cross threading helper. Invokes a delegate into the UI thread.
            /// </summary>
            /// <param name="Delegate">The delegate to run in the UI thread</param>
            public void InvokeOnSourceThread(Action Delegate)
            {
                ModelBase.SafeInvoke(Delegate);
            }
        }
        /// <summary>
        /// Describes the method that will be executed by the ThreadRelayCommand
        /// </summary>
        /// <param name="e"></param>
        public delegate void ThreadRelayCommandExecute(ThreadRelayCommandExecuteEventArgs e);
        /// <summary>
        /// Describes the possible results for a ThreadRelayCommand
        /// </summary>
        public enum ThreadRelayCommandExecuteResult
        {
            Completed,
            Cancelled,
            Error
        };
        /// <summary>
        /// ThreadRelayCommand result data
        /// </summary>
        public class ThreadRelayCommandExecuteResultData
        {
            private ThreadRelayCommandExecuteResult _Result;
            private Exception _Exception;
            private string _Message;
            /// <summary>
            /// Describes the result for the work
            /// </summary>
            public ThreadRelayCommandExecuteResult Result => _Result;
            /// <summary>
            /// The exception which happened while executing the work
            /// </summary>
            public Exception Exception => _Exception;
            /// <summary>
            /// A message sent by the finished work
            /// </summary>
            public string Message => _Message;
            internal ThreadRelayCommandExecuteResultData(ThreadRelayCommandExecuteResult Result, Exception Exception, string Message)
            {
                _Result = Result;
                _Exception = Exception;
                _Message = Message;
            }
        }
    }
}
