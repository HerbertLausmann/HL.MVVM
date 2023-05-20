using System;
using System.Collections.Concurrent;
using System.Threading;

namespace HL.MVVM.Threading
{
    public class ParallelThreadModel : ModelBase
    {
        private static readonly ConcurrentDictionary<Thread, ParallelThreadModel> ThreadMap = new ConcurrentDictionary<Thread, ParallelThreadModel>();
        private string _status;
        public string Status
        {
            get { return _status; }
            private set { SetField(ref _status, value); }
        }

        private double _progress = 0;
        public double Progress
        {
            get { return _progress; }
            private set { SetField(ref _progress, value); }
        }

        private double _maximumProgress = 0;
        public double MaximumProgress
        {
            get { return _maximumProgress; }
            private set { SetField(ref _maximumProgress, value); }
        }

        private bool _isRunning;
        public bool IsRunning
        {
            get { return _isRunning; }
            private set { SetField(ref _isRunning, value); }
        }

        private string _name = "Running";
        public string Name
        {
            get { return _name; }
            set { SetField(ref _name, value); }
        }

        public Guid ID { get; } = Guid.NewGuid();

        private Action _action;
        private Thread _thread;

        public ParallelThreadModel(Action action)
        {
            _action = action;
        }
        public ParallelThreadModel(string TaskName, Action action)
        {
            _action = action;
            _name = TaskName;
        }

        public void Run()
        {
            if (!IsRunning)
            {
                _thread = new Thread(() =>
                {
                    ThreadMap.TryAdd(Thread.CurrentThread, this);
                    IsRunning = true;
                    try
                    {
                        _action();
                    }
                    catch (Exception ex)
                    {
                        // Handle the exception or log it as needed
                    }
                    finally
                    {
                        ThreadMap.TryRemove(Thread.CurrentThread, out _);
                        IsRunning = false;
                    }
                });
                _thread.SetApartmentState(ApartmentState.MTA);
                _thread.Start();
            }
        }

        public void WaitForAndRun(params ParallelThreadModel[] ParallelThread)
        {
            if (!IsRunning)
            {
                _thread = new Thread(() =>
                {
                    ThreadMap.TryAdd(Thread.CurrentThread, this);
                    IsRunning = true;
                    try
                    {
                        bool stopWaiting = true;
                        while (true)
                        {
                            foreach (var item in ParallelThread)
                            {
                                stopWaiting &= item.IsRunning;
                            }
                            if (stopWaiting)
                                break;
                            Thread.Sleep(20);
                            stopWaiting = true;
                        }
                        _action();
                    }
                    catch (Exception ex)
                    {
                        // Handle the exception or log it as needed
                    }
                    finally
                    {
                        ThreadMap.TryRemove(Thread.CurrentThread, out _);
                        IsRunning = false;
                    }
                });
                _thread.SetApartmentState(ApartmentState.MTA);
                _thread.Start();
            }
        }

        public static void ReportProgress(string status, double progress, double maximumProgress)
        {
            if (ThreadMap.TryGetValue(Thread.CurrentThread, out ParallelThreadModel model))
            {
                model.Status = status;
                model.Progress = progress;
                model.MaximumProgress = maximumProgress;
            }
            else
            {
                throw new InvalidOperationException("The ReportProgress method should only be called from within the Action executed by a ParallelThreadModel.");
            }
        }

        public static void ReportProgress(string status, double progress)
        {
            if (ThreadMap.TryGetValue(Thread.CurrentThread, out ParallelThreadModel model))
            {
                model.Status = status;
                model.Progress = progress;
            }
            else
            {
                throw new InvalidOperationException("The ReportProgress method should only be called from within the Action executed by a ParallelThreadModel.");
            }
        }

        public static void ReportProgress(double progress)
        {
            if (ThreadMap.TryGetValue(Thread.CurrentThread, out ParallelThreadModel model))
            {
                model.Progress = progress;
            }
            else
            {
                throw new InvalidOperationException("The ReportProgress method should only be called from within the Action executed by a ParallelThreadModel.");
            }
        }

        public static void ReportProgress(string status)
        {
            if (ThreadMap.TryGetValue(Thread.CurrentThread, out ParallelThreadModel model))
            {
                model.Status = status;
            }
            else
            {
                throw new InvalidOperationException("The ReportProgress method should only be called from within the Action executed by a ParallelThreadModel.");
            }
        }

        public static ParallelThreadModel Run(Action Action, string name = null)
        {
            ParallelThreadModel p = new ParallelThreadModel(name, Action);
            p.Run();
            return p;
        }

        public static ParallelThreadModel WaitForAndRun(Action Action, params ParallelThreadModel[] ParallelThread)
        {
            ParallelThreadModel p = new ParallelThreadModel(Action);
            p.Run();
            return p;
        }

        public static ParallelThreadModel TryGetExecutingParallelThread()
        {
            if (ThreadMap.TryGetValue(Thread.CurrentThread, out ParallelThreadModel model))
            {
                return model;
            }
            else
            {
                return null;
            }
        }
    }
}
