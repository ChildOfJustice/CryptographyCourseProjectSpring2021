using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace CourseProjectCryptography2021.Spinner
{
    public partial class CircularProgressBar : UserControl
    {
        public static readonly DependencyProperty CirclesQuantityProperty =
            DependencyProperty.Register("CirclesQuantity", typeof(int), typeof(CircularProgressBar), new UIPropertyMetadata(5));
        public static readonly DependencyProperty SizeProperty =
            DependencyProperty.Register("CirclesSize", typeof(int), typeof(CircularProgressBar), new UIPropertyMetadata(20));
        
        public static readonly DependencyProperty SpeedProperty =
            DependencyProperty.Register("Speed", typeof(int), typeof(CircularProgressBar), new UIPropertyMetadata(30));
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Brush), typeof(CircularProgressBar), new UIPropertyMetadata(Brushes.Black));
        public static readonly DependencyProperty ClockDirectionProperty =
            DependencyProperty.Register("ClockDirection", typeof(bool), typeof(CircularProgressBar), new UIPropertyMetadata(true));
        public static readonly DependencyProperty RadiusProperty =
            DependencyProperty.Register("Radius", typeof(int), typeof(CircularProgressBar), new UIPropertyMetadata(50));

        public int CirclesQuantity
        {
            get { return (int) GetValue(CirclesQuantityProperty); }
            set { SetValue(CirclesQuantityProperty, value); }
        }
        public int CirclesSize
        {
            get { return (int)GetValue(SizeProperty); }
            set { SetValue(SizeProperty, value); }
        }
        public int Speed
        {
            get { return (int) GetValue(SpeedProperty); }
            set { SetValue(SpeedProperty, value); }
        }

        public int Size
        {
            get { return (int) GetValue(SizeProperty); }
            set { SetValue(SizeProperty, value); }
        }

        public Brush Color
        {
            get { return (Brush) GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }

        public bool ClockDirection
        {
            get { return (bool) GetValue(ClockDirectionProperty); }
            set { SetValue(ClockDirectionProperty, value); }
        }

        public int Radius
        {
            get { return (int) GetValue(RadiusProperty); }
            set { SetValue(RadiusProperty, value); }
        }

        private ObservableCollection<Ellipse> _ellipses;


        public CircularProgressBar()
        {
            InitializeComponent();
            
            Loaded += OnCanvasLoaded;
            
            //IsVisibleChanged += OnVisibleChanged;

            InitEllipses();
            
        }
        
        public ObservableCollection<Ellipse> Ellipses
        {
            get =>
                _ellipses;

            private set
            {
                _ellipses = value;
                //OnPropertyChanged(nameof(Ellipses));
            }
        }


        private static void SetPosition(DependencyObject ellipse, double x, double y)
        {
            ellipse.SetValue(Canvas.LeftProperty, x);
            ellipse.SetValue(Canvas.TopProperty, y);
        }
        
        private void InitEllipses()
        {
            _ellipses = new ObservableCollection<Ellipse>();
        }
        
        private void OnCanvasLoaded(object sender, RoutedEventArgs e)
        {
            
            double ActualSize = Size;
            
            double opacityStep = 1.0 / CirclesQuantity;
            for (int i = 0; i < CirclesQuantity; i++)
            {
                _ellipses.Add(new Ellipse {Height = ActualSize, Width = ActualSize, Fill = Color, Opacity = opacityStep*i});
                //ActualSize -= Size / CirclesQuantity;
            }

            double CircleShift = (360) / (CirclesQuantity);

            // Items.ItemsSource = elipses;
            int xCenter = Radius;
            int yCenter = Radius;
            // int xCenter = 20;
            // int yCenter = 20;
            
            if (ClockDirection)
                Animation.To = 360;
            else
                Animation.To = -360;

            try
            {
                double maxSpeed = 100;
                double temp = maxSpeed / Speed;
                int intTimeSpan = (int)Math.Round(temp);
                Animation.Duration = new Duration(TimeSpan.Parse("0:0:"+intTimeSpan));
            }
            catch (Exception ex)
            {
                MessageBox.Show((1/Speed).ToString());
            }

            int startX = Radius;
            int startY = 0;
            double phi = 0;
            foreach (int i in Enumerable.Range(0, _ellipses.Count))
            {
                _ellipses[i].Margin = GetThicknessForCirclePath(startX, startY, xCenter, yCenter,phi + i * CircleShift);
            }
        }

        private void OnEllipseCanvasLoaded(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < _ellipses.Count; i++)
            {
                SetPosition(_ellipses[i], -(Radius + Size/2), -(Radius + Size/2));
            }
        }

        private Thickness GetThicknessForCirclePath(int x, int y,int xCenter, int yCenter, double phi)
        {
            // double x = radius * Math.Cos(phi / 180.0 * Math.PI) + xCenter;
            // double y = radius * Math.Sin(phi / 180.0 * Math.PI) + yCenter;
            phi = phi / 180.0 * Math.PI;
            double _x = (Math.Cos(phi)*x - Math.Sin(phi)*y) + xCenter;
            double _y = (Math.Sin(phi)*x + Math.Cos(phi)*y) + yCenter;
            
            return new Thickness(_x, _y, 0, 0);
        }
    }
}