using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
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

namespace MemoryUI
{
    /// <summary>
    /// Interaction logic for SliderCanvasLineDemo.xaml
    /// </summary>
    public partial class SliderCanvasLineDemo : Page
    {

        public SliderCanvasLineDemo()
        {
            InitializeComponent();
        }
    }
    public class KKSlider : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return 400 - ((double)value * 40); 
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}   
