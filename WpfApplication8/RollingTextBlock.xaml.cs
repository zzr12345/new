using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using WpfApplication8;

namespace WpfApplication8
{
    /// <summary>
    /// RollingTextBlock.xaml 的交互逻辑
    /// </summary>
    public partial class RollingTextBlock : UserControl
    {
        private bool canRoll = false;
        private double rollingInterval = 16;//每一步的偏移量
        private double offset = 6;//最大的偏移量
        private TextBlock currentTextBlock = null;
        private DispatcherTimer currentTimer = null;
        public RollingTextBlock()
        {
            InitializeComponent();
            Loaded += RoilingTextBlock_Loaded;
        }
        void RoilingTextBlock_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.currentTextBlock != null)
            {
                canRoll = this.currentTextBlock.ActualHeight > this.ActualHeight;
            }
            currentTimer = new System.Windows.Threading.DispatcherTimer();
            currentTimer.Interval = new TimeSpan(0, 0, 1);
            currentTimer.Tick += new EventHandler(currentTimer_Tick);
            currentTimer.Start();
        }

        public override void OnApplyTemplate()
        {
            try
            {
                base.OnApplyTemplate();
                currentTextBlock = this.GetTemplateChild("textBlock") as TextBlock;
            }
            catch (Exception)
            {

            }

        }

        void currentTimer_Tick(object sender, EventArgs e)
        {
            if (this.currentTextBlock != null && canRoll)
            {
                if (Math.Abs(Top) <= this.currentTextBlock.ActualHeight - offset)
                {
                    Top -= rollingInterval;
                }
                else
                {
                    Top = this.ActualHeight;
                }

            }
        }

        #region Dependency Properties
        public static DependencyProperty TextProperty =
           DependencyProperty.Register("Text", typeof(string), typeof(RollingTextBlock),
           new PropertyMetadata(""));

        public static new DependencyProperty FontSizeProperty =
            DependencyProperty.Register("FontSize", typeof(double), typeof(RollingTextBlock),
            new PropertyMetadata(14D));

        public static readonly new DependencyProperty ForegroundProperty =
           DependencyProperty.Register("Foreground", typeof(Brush), typeof(RollingTextBlock), new FrameworkPropertyMetadata(Brushes.Green));

        public static DependencyProperty LeftProperty =
           DependencyProperty.Register("Left", typeof(double), typeof(RollingTextBlock), new PropertyMetadata(0D));

        public static DependencyProperty TopProperty =
           DependencyProperty.Register("Top", typeof(double), typeof(RollingTextBlock), new PropertyMetadata(0D));

        #endregion

        #region Public Variables
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public new double FontSize
        {
            get { return (double)GetValue(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }

        public new Brush Foreground
        {
            get { return (Brush)GetValue(ForegroundProperty); }
            set { SetValue(ForegroundProperty, value); }
        }

        public double Left
        {
            get { return (double)GetValue(LeftProperty); }
            set { SetValue(LeftProperty, value); }
        }

        public double Top
        {
            get { return (double)GetValue(TopProperty); }
            set { SetValue(TopProperty, value); }
        }
        #endregion
    }

}
