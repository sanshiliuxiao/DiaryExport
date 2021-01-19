using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace DiaryExport.Commands
{
    public class RelayCommand : ICommand
    {
        private readonly Func<Boolean> _canExecute;
        private readonly Action _execute;

        public event EventHandler CanExecuteChanged
        { 
            add
            {
                if (_canExecute != null)
                {
                    CommandManager.RequerySuggested += value;
                }
            }
            remove
            {
                if (_canExecute != null)
                {
                    CommandManager.RequerySuggested -= value;
                }
            }
        }


        public RelayCommand(Action execute) : this(execute, null)
        {

        }
        public RelayCommand(Action execute, Func<Boolean> canExecute)
        {
            if (execute == null)
            {
                throw new ArgumentNullException("execute");
                
            }
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute();
        }

        public void Execute(object parameter)
        {
            _execute();
        }
    }
}
