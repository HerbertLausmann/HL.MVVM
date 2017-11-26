# HL.MVVM
Minimalist cross platform MVVM framework

## Features

This is my own tiny MVVM framework that I use for my projects. It's a cross platform .NET CORE 2 library.
I've built some interesting classes like:

1. RelayCommand
2. AsyncRelayCommand (That executes a delegate inside a task)
3. ThreadRelayCommand (Runs a delegate inside a new thread. It supports progress report, cancellation and pause/resume)
4. ThreadSafeObservableCollection (A ObservableCollection built for multithreading scenarios. It prevent cross threading operation exceptions)
5. CommandManager (A custom Command Manager that detects when the CanExecute state of a command might have changed)
6. Base classes for Models and ViewModels that will give you a big hand.

Hope you enjoy it! :)
