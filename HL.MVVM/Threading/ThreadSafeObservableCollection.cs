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
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HL.MVVM.Threading
{
    /// <summary>
    /// A safe replacement for the usual ObservableCollection<T>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ThreadSafeObservableCollection<T> : ObservableCollection<T>
    {
        private SynchronizationContext SynchronizationContext;

        public ThreadSafeObservableCollection()
        {
            SynchronizationContext = SynchronizationContext.Current;

            // current synchronization context will be null if we're not in UI Thread
            if (SynchronizationContext == null)
                throw new InvalidOperationException("This collection must be instantiated from UI Thread, if not, you have to pass SynchronizationContext to constructor.");
            CommandManager.RegisterObject(this);
        }

        public ThreadSafeObservableCollection(SynchronizationContext synchronizationContext)
        {
            this.SynchronizationContext = synchronizationContext ?? throw new ArgumentNullException("synchronizationContext");
            CommandManager.RegisterObject(this);
        }

        ~ThreadSafeObservableCollection()
        {
            CommandManager.UnRegisterObject(this);
        }

        protected override void ClearItems()
        {
            this.SynchronizationContext.Send(new SendOrPostCallback((param) => base.ClearItems()), null);
        }

        protected override void InsertItem(int index, T item)
        {
            this.SynchronizationContext.Send(new SendOrPostCallback((param) => base.InsertItem(index, item)), null);
        }

        protected override void RemoveItem(int index)
        {
            this.SynchronizationContext.Send(new SendOrPostCallback((param) => base.RemoveItem(index)), null);
        }

        protected override void SetItem(int index, T item)
        {
            this.SynchronizationContext.Send(new SendOrPostCallback((param) => base.SetItem(index, item)), null);
        }

        protected override void MoveItem(int oldIndex, int newIndex)
        {
            this.SynchronizationContext.Send(new SendOrPostCallback((param) => base.MoveItem(oldIndex, newIndex)), null);
        }
    }
}
