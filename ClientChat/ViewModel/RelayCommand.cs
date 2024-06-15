using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ClientChat.ViewModel
{
    public class RelayCommand : ICommand
    {
        private readonly Func<Task> _execute;
        private readonly Action _executeSync;

        public RelayCommand(Func<Task> execute)
        {
            _execute = execute;
        }

        public RelayCommand(Action executeSync)
        {
            _executeSync = executeSync;
        }

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter)
        {
            if (_execute != null)
            {
                _execute();
            }
            else
            {
                _executeSync();
            }
        }

        public event EventHandler CanExecuteChanged;
    }
}
