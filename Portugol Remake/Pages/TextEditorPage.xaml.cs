using Portugol_Remake.Lists;
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

namespace Portugol_Remake.Pages
{
    /// <summary>
    /// Interaction logic for TextEditorPage.xaml
    /// </summary>
    public partial class TextEditorPage : Page
    {
        public bool IsSaved { get; set; }
        public string Path { get; set; }
        public TextEditorPage()
        {
            InitializeComponent();
            TextEditorBox.Style = UserConfig.UserConfigurations.ThemeDictionary["TextBox"] as Style;
        }
    }
}
