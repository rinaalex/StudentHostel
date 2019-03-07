using System;
using System.Windows.Input;
using System.Windows.Threading;

namespace StudentHostelApp.Commands
{
    public class Command : ICommand
    {
        private Action methodToExecute = null;
        private Func<bool> methodToDetectCanExecute = null;

        public Command(Action methodToExecute, Func<bool> methodToDetectCanExecute)
        {
            this.methodToExecute = methodToExecute;
            this.methodToDetectCanExecute = methodToDetectCanExecute;
        }

        public void Execute(object parameter)
        {
            this.methodToExecute();
        }

        public bool CanExecute(object parameter)
        {
            if (this.methodToDetectCanExecute==null)
            {
                return true;
            }
            else
            {
                return this.methodToDetectCanExecute();
            }
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}
