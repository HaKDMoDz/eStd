using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;

namespace System.Wpf.Mvvm
{
    public abstract class ViewModelBase : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        private Dictionary<string, List<string>> _errors = new Dictionary<string, List<string>>();

        public ViewModelBase()
        {
        }

        public ViewModelBase(Window view)
        {
            View = view;
        }

        public IEnumerable GetErrors(string propertyName)
        {
            return _errors[propertyName];
        }

        public Window View { get; set; }

        protected virtual void Validate(string error, [CallerMemberName] string property = null)
        {
            AddError(error, property);
        }

        protected void AddError(string error, [CallerMemberName]
                                string property = null)
        {
            if (_errors.ContainsKey(property))
            {
                _errors[property].Add(error);
            }
            else
            {
                _errors.Add(property, new List<string>(new[] { error }));
            }
        }

        public bool HasErrors
        {
            get
            {
                return _errors.Count > 0;
            }
        }

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName]
                                      string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}