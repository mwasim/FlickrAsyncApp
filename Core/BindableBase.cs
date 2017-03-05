using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Core
{
    public class BindableBase : INotifyPropertyChanged
    {
        public void SetProperty<T>(ref T prop, T value, [CallerMemberName] string callerName = null)
        {
            if (!EqualityComparer<T>.Default.Equals(prop, value))
            {
                prop = value;
                OnPropertyChanged(callerName);
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
