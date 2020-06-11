using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace conrod
{
    public class SetIfDifferentHelper : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void SetIfDifferent<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
        {
            if (field?.Equals(value) != true)
            {
                field = value;
                NotifyPropertyChange(propertyName);
            }
        }
        protected void NotifyPropertyChange(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
