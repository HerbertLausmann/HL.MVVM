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
using System.Windows.Input;
using System.Threading;
using System.Threading.Tasks;

namespace HL.MVVM
{
    /// <summary>
    /// CommandManager watchs all the Models and ViewModel to detect changes that should interfer on the CanExecute state of a Command
    /// </summary>
    public sealed class CommandManager : ModelBase
    {
        /// <summary>
        /// Raises everytime that the CommandManager detects changes on Models and/or ViewModels that could affect the Commands
        /// </summary>
        public static event EventHandler RequerySuggested;

        /// <summary>
        /// Internal use only. Safe invokes (on the UI thread, to prevent Cross Threading exceptions) the RequerySuggested event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnRequerySuggested(object sender, EventArgs e)
        {
            SafeInvoke(() => RequerySuggested?.Invoke(sender, e));
        }

        //The code bellow is used to supress multiple calls in a short while
        private static Task CallSupresser;
        private static void OnRegisteredObjectChanged(object sender, EventArgs e)
        {
            if (CallSupresser == null || CallSupresser.IsCompleted == true)
            {
                CallSupresser?.Dispose();
                CallSupresser = Task.Run(async () =>
                {
                    await Task.Delay(100);
                    OnRequerySuggested(sender, e);
                });
            }
        }

        /// <summary>
        /// Registers the given object to be monitored. When any property within it changes, it will suggest a global requery for all the linked Commands.
        /// </summary>
        /// <param name="obj">An object that inherits from the INotifyPropertyChanged interface</param>
        public static void RegisterObject(INotifyPropertyChanged obj)
        {
            obj.PropertyChanged += OnRegisteredObjectChanged;
        }

        /// <summary>
        /// Unregisters the given object and stops monitoring it.
        /// </summary>
        /// <param name="obj">An object that inherits from the INotifyPropertyChanged interface</param>
        public static void UnRegisterObject(INotifyPropertyChanged obj)
        {
            obj.PropertyChanged -= OnRegisteredObjectChanged;
        }

        /// <summary>
        /// Forces a Global Requery process
        /// </summary>
        /// <param name="sender">The object that is asking for the Requery</param>
        public static void Requery(object sender)
        {
            OnRequerySuggested(sender, new EventArgs());
        }
    }
}
