using System.Windows;
using System.Windows.Input;
using Task_3.ViewModels;
using Task_3.WPF_Control_Elements.Spinner;

namespace Task_3.WPF_Control_Elements.DialogHost
{
    public class DialogHostViewModel : BaseViewModel
    {
        private RelayCommand openDialogCommand;
        
        
        public RelayCommand OpenDialogCommand
        {
            set
            {
                openDialogCommand = value;
                OnPropertyChanged(nameof(OpenDialogCommand));
            }
            get
            {
                return openDialogCommand ??
                       (openDialogCommand = new RelayCommand(obj =>
                       {
                           MessageBox.Show("COMMAND");
                           //DialogWindow win = new DialogWindow();
                           // if (owner != null)
                           //     win.Owner = owner;
                           //var vm = new CircularProgressBar();
                           //win.DataContext = vm;
                           //win.ShowDialog();
                       }));
            }
        }
    }
}