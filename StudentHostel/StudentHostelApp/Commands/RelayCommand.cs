using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace StudentHostelApp.Commands
{
    public class RelayCommand: ICommand
    {
        private Action<object> methodToExecute;
        private Func<object, bool> methodToDetectCanExecute;

        public RelayCommand(Action<object> methodToExecute):this(methodToExecute,null)
        {

        }

        public RelayCommand(Action<object>methodToExecute, Func<object,bool>methodToDetectCanExecute)
        {
            this.methodToExecute = methodToExecute;
            this.methodToDetectCanExecute = methodToDetectCanExecute;
        }

        public void Execute(object parameter)
        {
            this.methodToExecute(parameter);
        }

        public bool CanExecute(object parameter)
        {
            if (this.methodToDetectCanExecute != null)
            {
                return this.methodToDetectCanExecute(parameter);
            }
            else
                return true;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}
