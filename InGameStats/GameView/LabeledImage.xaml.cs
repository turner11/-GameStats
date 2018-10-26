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

namespace GameView
{
    /// <summary>
    /// Interaction logic for LabeledImage.xaml
    /// </summary>
    public partial class LabeledImage : UserControl
    {


        public static readonly DependencyProperty ImageSourceProperty 
            = DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(LabeledImage), 
                null);
        //new FrameworkPropertyMetadata(new LabeledImage()));

        //public static readonly DependencyProperty TextProperty
        //   = DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(LabeledImage),
        //       null);

        public ImageSource ImageSource
        {
            get { return GetValue(ImageSourceProperty) as ImageSource; }
            set { SetValue(ImageSourceProperty, value); }
        }
        //public static ImageSource ImageSource
        //{
        //    get { return this.ImageControl.Source; }
        //    set { this.ImageControl.Source = value; }
        //}

        

        public string Text
        {
            get { return "aaaaaaaa"; }// this.TextBox.Text; }
            set { this.TextBox.Text = value; }
        }


        public LabeledImage()
        {
            
            InitializeComponent();
        }
    }
}
