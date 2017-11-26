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
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace HL.MVVM
{
    public abstract class ModelBase : System.ComponentModel.INotifyPropertyChanged, IDisposable
    {
        /// <summary>
        /// Provides an easy way to run code in the UI Thread from another thread that you could be running in
        /// </summary>
        protected static readonly SynchronizationContext Context =
SynchronizationContext.Current ?? new SynchronizationContext();

        public ModelBase()
        {
            Initialize();
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string Name)
        {
            PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(Name));
        }

        protected void SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (Object.Equals(field, value))
            {
                return;
            }
            else
            {
                field = value;
                OnPropertyChanged(propertyName);
            }
        }
        protected virtual void Initialize()
        {
            //Registers this object to be monitored by the CommandManager
            //When any property change happens to this object, it tells the Command Manager
            CommandManager.RegisterObject(this);
        }

        /// <summary>
        /// Invokes the given delegate on the UI thread preventing cross threading operation exceptions between
        /// an eventual background thread that you are running in and the UI thread.
        /// </summary>
        /// <param name="Delegate">A delegate to run</param>
        protected static void SafeInvoke(Action Delegate)
        {
            Context.Send((param) => Delegate.Invoke(), null);
        }
        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    CommandManager.UnRegisterObject(this);
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~ModelBase() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}