using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using CourseProjectCryptography2021.Spinner;
using Task_3.WPF_Control_Elements.Spinner;

namespace Task_3.WPF_Control_Elements.SpinnerDialog
{
    
    // с возможностью настройки извне: текста, размера
    // шрифта текста, а также необходимых свойств инкрустированного в
    // диалог элемента управления Spinner
    
    
    public partial class SpinnerDialogElement : UserControl
    {
        
        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register("Message", typeof(string), typeof(CircularProgressBar), new UIPropertyMetadata("Please Wait..."));
        public static readonly DependencyProperty MessageTextSizeProperty =
            DependencyProperty.Register("MessageTextSize", typeof(int), typeof(CircularProgressBar), new UIPropertyMetadata(20));
        public static readonly DependencyProperty SpinnerCirclesQuantityProperty =
            DependencyProperty.Register("SpinnerCirclesQuantity", typeof(int), typeof(CircularProgressBar), new UIPropertyMetadata(5));
        public static readonly DependencyProperty SpinnerSizeProperty =
            DependencyProperty.Register("SpinnerCirclesSize", typeof(int), typeof(CircularProgressBar), new UIPropertyMetadata(20));
        public static readonly DependencyProperty SpinnerSpeedProperty =
            DependencyProperty.Register("SpinnerSpeed", typeof(int), typeof(CircularProgressBar), new UIPropertyMetadata(30));
        public static readonly DependencyProperty SpinnerColorProperty =
            DependencyProperty.Register("SpinnerColor", typeof(Brush), typeof(CircularProgressBar), new UIPropertyMetadata(Brushes.Black));
        public static readonly DependencyProperty SpinnerClockDirectionProperty =
            DependencyProperty.Register("SpinnerClockDirection", typeof(bool), typeof(CircularProgressBar), new UIPropertyMetadata(true));

        
        public string Message
        {
            get { return (string) GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }
        public int MessageTextSize
        {
            get { return (int)GetValue(MessageTextSizeProperty); }
            set { SetValue(MessageTextSizeProperty, value); }
        }
        public int SpinnerCirclesQuantity
        {
            get { return (int) GetValue(SpinnerCirclesQuantityProperty); }
            set { SetValue(SpinnerCirclesQuantityProperty, value); }
        }
        public int SpinnerCirclesSize
        {
            get { return (int)GetValue(SpinnerSizeProperty); }
            set { SetValue(SpinnerSizeProperty, value); }
        }
        public int SpinnerSpeed
        {
            get { return (int) GetValue(SpinnerSpeedProperty); }
            set { SetValue(SpinnerSpeedProperty, value); }
        }

        public int SpinnerSize
        {
            get { return (int) GetValue(SpinnerSizeProperty); }
            set { SetValue(SpinnerSizeProperty, value); }
        }

        public Brush SpinnerColor
        {
            get { return (Brush) GetValue(SpinnerColorProperty); }
            set { SetValue(SpinnerColorProperty, value); }
        }

        public bool SpinnerClockDirection
        {
            get { return (bool) GetValue(SpinnerClockDirectionProperty); }
            set { SetValue(SpinnerClockDirectionProperty, value); }
        }






        private UserControl content;
        
        
        
        
        
        
        
        
        
        
        
        
        public SpinnerDialogElement()
        {
            InitializeComponent();
        }
        public SpinnerDialogElement(UserControl _content)
        {
            content = _content;
            InitializeComponent();
            
        }

        private void OnLoad(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("!!!!@");
            //DialogHost.WorkSpaceBorder.CornerRadius = new CornerRadius(100,0,0,0);
            //DialogHost.WorkSpaceBorder.DataContext = null;

            if (content == null)
            {
                var c = new Content();
                c.msg.Text = Message;
                DialogHost.ContentPresenter.Content = new Content();
            }
            else
                DialogHost.ContentPresenter.Content = content;


            // MessageBox.Show("COMMAND");
            // ContentWindow win = new ContentWindow();
            // if (owner != null)
            //     win.Owner = owner;
            //var vm = new CircularProgressBar();
            //win.DataContext = vm;
            // win.ShowDialog();

            //DO NOT WORK
            //DialogHost.WorkSpaceBorder.DataContext = new Content();
        }
    }
}