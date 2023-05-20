using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HL.MVVM.Threading
{
    public class ParallelThreadViewModel : ViewModelBase
    {
        private ThreadSafeObservableCollection<ParallelThreadModel> _parallelThreads;

        public ThreadSafeObservableCollection<ParallelThreadModel> ParallelThreads
        {
            get { return _parallelThreads; }
            private set { SetField(ref _parallelThreads, value); }
        }

        public ParallelThreadViewModel()
        {
            ParallelThreads = new ThreadSafeObservableCollection<ParallelThreadModel>();
        }

        public ParallelThreadModel ParallelRun(Action action, bool waitcurrent = false)
        {
            var executingThread = ParallelThreadModel.TryGetExecutingParallelThread();
            bool shouldWaitFor = !(executingThread is null) && waitcurrent;
            if (!shouldWaitFor)
            {
                var parallelThreadModel = new ParallelThreadModel(action);
                ParallelThreads.Add(parallelThreadModel);
                parallelThreadModel.Run();
                return parallelThreadModel;
            }
            else
            {
                var parallelThreadModel = new ParallelThreadModel(action);
                ParallelThreads.Add(parallelThreadModel);
                parallelThreadModel.WaitForAndRun(parallelThreadModel);
                return parallelThreadModel;
            }
        }
        public ParallelThreadModel ParallelRun(string TaskName, Action action, bool waitcurrent = false)
        {
            var executingThread = ParallelThreadModel.TryGetExecutingParallelThread();
            bool shouldWaitFor = !(executingThread is null) && waitcurrent;
            if (!shouldWaitFor)
            {
                var parallelThreadModel = new ParallelThreadModel(TaskName, action);
                ParallelThreads.Add(parallelThreadModel);
                parallelThreadModel.Run();
                return parallelThreadModel;
            }
            else
            {
                var parallelThreadModel = new ParallelThreadModel(TaskName, action);
                ParallelThreads.Add(parallelThreadModel);
                parallelThreadModel.WaitForAndRun(parallelThreadModel);
                return parallelThreadModel;
            }
        }
    }
}