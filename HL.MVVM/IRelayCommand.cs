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

namespace HL.MVVM
{
    /// <summary>
    /// Interface for all the RelayCommands
    /// </summary>
    public interface IRelayCommand : ICommand
    {
        /// <summary>
        /// Forces an update on the CanExecute state of this command
        /// </summary>
        void ForceRequery();
        /// <summary>
        /// The action to be executed by this command
        /// </summary>
        Action<object> Action { get;}
        /// <summary>
        /// The predicate used to define the CanExecute state of this command
        /// </summary>
        Predicate<object> CanExecutePredicate { get;}
    }
}
