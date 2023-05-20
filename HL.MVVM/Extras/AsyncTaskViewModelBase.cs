using HL.MVVM.Async;
using System;
using System.Collections.Generic;
using System.Text;

namespace HL.MVVM.Extras
{
    public abstract class AsyncTaskViewModelBase : ViewModelBase
    {
        public AsyncRelayCommand RunCommand => GetCommand(new AsyncRelayCommand((object o) => { Run(o); }, (object o) => { return !RunCommand.IsRunning; }), "Run");

        protected abstract void Run(object Param);
    }
}
