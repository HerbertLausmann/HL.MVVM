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
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace HL.MVVM
{
    /// <summary>
    /// Base class for all ViewModels
    /// </summary>
    public abstract class ViewModelBase : ModelBase
    {

        protected override void Initialize()
        {
            _Commands = new Dictionary<string, ICommand>(5);
            base.Initialize();
        }
        /// <summary>
        /// The internal Commands dictionary
        /// </summary>
        private Dictionary<string, ICommand> _Commands;

        /// <summary>
        /// Try to return a ICommand. If it doesn't exists, create it using the Command param and then return it.
        /// </summary>
        /// <param name="Command">The command to be created/returned</param>
        /// <param name="Name">The name of the Command</param>
        /// <returns>Returns the Command</returns>
        protected ICommand GetCommand(ICommand Command, [System.Runtime.CompilerServices.CallerMemberName]string Name = null)
        {
            begin:
            if (_Commands.TryGetValue(Name, out ICommand cmd))
                return cmd;
            else
            {
                _Commands.Add(Name, Command);
                goto begin;
            }
        }

        /// <summary>
        /// Try to return a RelayCommand. If it doesn't exists, create it using the Command param and then return it.
        /// </summary>
        /// <param name="Command">The command to be created/returned</param>
        /// <param name="Name">The name of the RelayCommand</param>
        /// <returns>Returns the RelayCommand</returns>
        protected RelayCommand GetCommand(RelayCommand Command, [System.Runtime.CompilerServices.CallerMemberName]string Name = null)
        {
            begin:
            if (_Commands.TryGetValue(Name, out ICommand cmd))
                return cmd as RelayCommand;
            else
            {
                _Commands.Add(Name, Command);
                goto begin;
            }
        }

        /// <summary>
        /// Try to return a AsyncRelayCommand. If it doesn't exists, create it using the Command param and then return it.
        /// </summary>
        /// <param name="Command">The command to be created/returned</param>
        /// <param name="Name">The name of the AsyncRelayCommand</param>
        /// <returns>Returns the AsyncRelayCommand</returns>
        protected Async.AsyncRelayCommand GetCommand(Async.AsyncRelayCommand Command, [System.Runtime.CompilerServices.CallerMemberName]string Name = null)
        {
            begin:
            if (_Commands.TryGetValue(Name, out ICommand cmd))
                return cmd as Async.AsyncRelayCommand;
            else
            {
                _Commands.Add(Name, Command);
                goto begin;
            }
        }
        /// <summary>
        /// Try to return a ThreadRelayCommand. If it doesn't exists, create it using the Command param and then return it.
        /// </summary>
        /// <param name="Command">The command to be created/returned</param>
        /// <param name="Name">The name of the ThreadRelayCommand</param>
        /// <returns>Returns the ThreadRelayCommand</returns>
        protected Threading.ThreadRelayCommand GetCommand(Threading.ThreadRelayCommand Command, [System.Runtime.CompilerServices.CallerMemberName]string Name = null)
        {
            begin:
            if (_Commands.TryGetValue(Name, out ICommand cmd))
                return cmd as Threading.ThreadRelayCommand;
            else
            {
                _Commands.Add(Name, Command);
                goto begin;
            }
        }
        /// <summary>
        /// Try to return the Command by its name. If does not exists, retuns null.
        /// </summary>
        /// <param name="Name">The Command's name</param>
        /// <returns>Returns the founded Command or null if not founded</returns>
        protected ICommand GetCommand(string Name)
        {
            if (_Commands.TryGetValue(Name, out ICommand cmd))
                return cmd;
            else
                return null;
        }

        /// <summary>
        /// Used to rewrite an existent Command or to Create it
        /// </summary>
        /// <param name="Command">The command to be Created/Rewrited</param>
        /// <param name="Name">The name of the Command</param>
        protected void SetCommand(ICommand Command, [System.Runtime.CompilerServices.CallerMemberName]string Name = null)
        {
            if (Command == null || string.IsNullOrWhiteSpace(Name)) return;
            if (_Commands.ContainsKey(Name))
            {
                _Commands[Name] = Command;
            }
            else
            {
                _Commands.Add(Name, Command);
            }
        }
    }
}
