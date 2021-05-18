using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Task_3.ViewModels;

namespace Task_3.WPF_Control_Elements.DialogHost
{
    public partial class DialogHostElement : UserControl
    {
        
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(DialogHostElement), new UIPropertyMetadata(new CornerRadius(0, 0, 0, 0)));
        public static readonly DependencyProperty TransparencyProperty =
            DependencyProperty.Register("Transparency", typeof(double), typeof(DialogHostElement), new UIPropertyMetadata(0.4));
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(RelayCommand), typeof(DialogHostElement), new UIPropertyMetadata(new RelayCommand(
                o => { })));
        
        public CornerRadius AllCornerRadius
        {
            get { return (CornerRadius) GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }
        public double Transparency
        {
            get { return (double) GetValue(TransparencyProperty); }
            set { SetValue(TransparencyProperty, value); }
        }
        public RelayCommand Command
        {
            get { return (RelayCommand) GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }


        private DialogHostViewModel mainDataContext;
        public DialogHostElement()
        {
            InitializeComponent();
            Loaded += OnLoad;
            mainDataContext = new DialogHostViewModel();
            this.DataContext = mainDataContext;
        }

        private void OnLoad(object sender, RoutedEventArgs e)
        {
            WorkSpaceBorder.CornerRadius = AllCornerRadius;
            //BackGroundBorder.Background = new SolidColorBrush(Color.FromArgb(Transparency, 0, 0, 0));
            BackGroundBorder.Opacity = Transparency;
            mainDataContext.OpenDialogCommand = Command;
        }
    }
}