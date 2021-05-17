using System.ComponentModel;

namespace Task_3.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        
        protected void OnPropertiesChanged(params string[] propertiesNames)
        {
            foreach (var propertyName in propertiesNames)
            {
                OnPropertyChanged(propertyName);
            }
        }
    }
}