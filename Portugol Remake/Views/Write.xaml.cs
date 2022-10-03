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

namespace Portugol_Remake.Views
{
    /// <summary>
    /// Interaction logic for Write.xaml
    /// </summary>
    public partial class Write : UserControl
    {
        public int ListPosition { get; set; }
        public Brush Fill
        {
            get
            {
                return MainCard.Fill;
            }
            set
            {
                MainCard.Fill = value;
            }
        }

        public Brush Stroke
        {
            get
            {
                return MainCard.Stroke;
            }
            set
            {
                MainCard.Stroke = value;
            }
        }

        public double StrokeThickness
        {
            get
            {
                return MainCard.StrokeThickness;
            }
            set
            {
                MainCard.StrokeThickness = value;
            }
        }

        public string Text
        {
            get
            {
                return MainText.Text;
            }
            set
            {
                MainText.Text = value;
            }
        }
        public Write()
        {
            InitializeComponent();
        }

        public void ShowLineBox()
        {
            Top.Visibility = Visibility.Visible;
            Bottom.Visibility = Visibility.Visible;
            Left.Visibility = Visibility.Visible;
            Right.Visibility = Visibility.Visible;
        }

        public void HideLineBox()
        {
            Top.Visibility = Visibility.Hidden;
            Bottom.Visibility = Visibility.Hidden;
            Left.Visibility = Visibility.Hidden;
            Right.Visibility = Visibility.Hidden;
        }
    }
}
