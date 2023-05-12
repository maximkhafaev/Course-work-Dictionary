using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace TreeDictionary
{
    class Command : ICommand
    {
        private Action actionWithoutParam;
        private Action<Object> actionWithParam;

        private Func<Boolean> checkWithoutParam;
        private Func<Object, Boolean> checkWithParam;

        private bool useParam;
        private bool useCheck;

        public Command(Action action)
        {
            useParam = false;
            actionWithoutParam = action;
        }

        public Command(Action<object> action)
        {
            useParam = true;
            actionWithParam = action;
        }

        public Command(Action action, Func<Boolean> check)
        {
            useParam = false;
            useCheck = true;

            actionWithoutParam = action;
            checkWithoutParam = check;
        }

        public Command(Action<object> action, Func<Object, Boolean> check)
        {
            useParam = true;
            useCheck = true;

            actionWithParam = action;
            checkWithParam = check;
        }

        public bool CanExecute(object parameter)
        {
            return !useCheck || (useParam ? checkWithParam.Invoke(parameter) : checkWithoutParam.Invoke());
        }

        public void Execute(object parameter)
        {
            if (useParam)
            {
                actionWithParam.Invoke(parameter);
            }
            else
            {
                actionWithoutParam.Invoke();
            }
        }

        public event EventHandler CanExecuteChanged;
    }
}
