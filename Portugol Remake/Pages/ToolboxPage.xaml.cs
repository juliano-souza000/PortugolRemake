using Portugol_Remake.Lists;
using Portugol_Remake.Resources.Cursor;
using Portugol_Remake.Utils;
using Portugol_Remake.Windows;
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
    /// Interaction logic for ToolboxPage.xaml
    /// </summary>
    public partial class ToolboxPage : Page
    {
        private EditorWindow ParentWindow { get; set; }
        private FlowchartEditorPage ParentFlowchartPage { get; set; }
        private int PreviousSelectedItemIndex = -1;
        private bool CanDragAndDropControl;

        public ToolboxPage(EditorWindow parent, Page page, ToolboxType toolboxType = ToolboxType.Unknow)
        {
            InitializeComponent();

            UserControlList.Style = UserConfig.UserConfigurations.ThemeDictionary["ToolboxWindowListViewStyle"] as Style;
            UserControlList.Resources.Add(typeof(ListViewItem), UserConfig.UserConfigurations.ThemeDictionary["UserControlListItemTemplateStyle"] as Style);

            UserControlList.SelectionChanged += UserControlList_SelectionChanged;
            UserControlList.PreviewMouseLeftButtonDown += UserControlList_MouseLeftButtonDown;
            
            UserControlList.SelectionMode = SelectionMode.Single;

            ParentWindow = parent;
            
            switch (toolboxType)
            {
                case ToolboxType.Flowchart:
                    ParentFlowchartPage = page as FlowchartEditorPage;

                    ParentFlowchartPage.GridContent.AllowDrop = true;
                    ParentFlowchartPage.GridContent.Drop += ParentFlowchartPage.GridContent_Drop;

                    InitializeToolboxWithFlowchart();
                    break;
            }
        }

        private void InitializeToolboxWithFlowchart()
        {
            List<ControlInfo> items = new List<ControlInfo>();

            ControlInfo terminator = new ControlInfo() { ControlName = "Terminador", ItemType = ItemType.Terminator };
            ControlInfo process = new ControlInfo() { ControlName = "Processo", ItemType = ItemType.Process };
            ControlInfo write = new ControlInfo() { ControlName = "Escrever", ItemType = ItemType.Write };
            ControlInfo read = new ControlInfo() { ControlName = "Ler", ItemType = ItemType.Read };
            ControlInfo decision = new ControlInfo() { ControlName = "Decisão", ItemType = ItemType.Decision };
            ControlInfo connection = new ControlInfo() { ControlName = "Conexão", ItemType = ItemType.Connection };
            ControlInfo connect = new ControlInfo() { ControlName = "Conectar", ItemType = ItemType.Connect };

            terminator.ControlIcon = new System.Windows.Shapes.Rectangle
            {
                Width = 15,
                Height = 7,
                RadiusX = 3.5,
                RadiusY = 15,
                Stroke = new SolidColorBrush(Color.FromRgb(255, 255, 255)),
                StrokeThickness = 0.5
            };

            process.ControlIcon = new Views.Process
            {
                Width = 15,
                Height = 7,
                Stroke = new SolidColorBrush(Color.FromRgb(255, 255, 255)),
                StrokeThickness = 0.5
            };

            write.ControlIcon = new Views.Write
            {
                Width = 15,
                Height = 7,
                Stroke = new SolidColorBrush(Color.FromRgb(255, 255, 255)),
                StrokeThickness = 0.5
            };

            read.ControlIcon = new Views.Read
            {
                Width = 15,
                Height = 7,
                Stroke = new SolidColorBrush(Color.FromRgb(255, 255, 255)),
                StrokeThickness = 0.5
            };

            decision.ControlIcon = new Views.Decision
            {
                Width = 15,
                Height = 7,
                Stroke = new SolidColorBrush(Color.FromRgb(255, 255, 255)),
                StrokeThickness = 0.5
            };

            connection.ControlIcon = new Views.Connection
            {
                Width = 7,
                Height = 7,
                Stroke = new SolidColorBrush(Color.FromRgb(255, 255, 255)),
                StrokeThickness = 0.5
            };

            Grid connectGrid = new Grid();
            
            var line = new Line
            {
                Width = 7,
                Height = 7,
                X1 = 0,
                Y1 = 0,
                X2 = 7,
                Y2 = 7,
                Stroke = new SolidColorBrush(Color.FromRgb(255, 255, 255)),
                StrokeThickness = 0.5
            };

            Polygon lineEnd = new Polygon
            {
                Width = 7,
                Height = 7,
                Points = new PointCollection { new Point(5,7), new Point(7,5), new Point(7,7) },
                Stroke = new SolidColorBrush(Color.FromRgb(255, 255, 255)),
                StrokeThickness = 0.5
            };

            connectGrid.Children.Add(line);
            connectGrid.Children.Add(lineEnd);
            connect.ControlIcon = connectGrid;

            items.Add(terminator);
            items.Add(process);
            items.Add(write);
            items.Add(read);
            items.Add(decision);
            items.Add(connection);
            items.Add(connect);

            DataContext = items;
        }

        private void UserControlList_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            CanDragAndDropControl = true;

            if (PreviousSelectedItemIndex != -1 && UserControlList.SelectedIndex == PreviousSelectedItemIndex)
            {
                DragDrop.DoDragDrop(UserControlList.SelectedItem as DependencyObject, (UserControlList.SelectedItem as ControlInfo).ItemType, DragDropEffects.Copy);
                PreviousSelectedItemIndex = -1;
                CanDragAndDropControl = false;
            }
        }

        private void UserControlList_SelectionChanged(object sender, SelectionChangedEventArgs e) 
        {
            ParentFlowchartPage.GridContent.Cursor = Cursors.Arrow;
            ParentFlowchartPage.IsDrawLineEnabled = false;
            if (CanDragAndDropControl)
            {
                if (PreviousSelectedItemIndex != -1)
                    UserControlList.SelectedIndex = PreviousSelectedItemIndex;
            }
            PreviousSelectedItemIndex = UserControlList.SelectedIndex;

            if (PreviousSelectedItemIndex != -1 && UserControlList.SelectedIndex == PreviousSelectedItemIndex && (UserControlList.SelectedItem as ControlInfo).ItemType != ItemType.Connect)
            {
                DragDrop.DoDragDrop(UserControlList.SelectedItem as DependencyObject, (UserControlList.SelectedItem as ControlInfo).ItemType, DragDropEffects.Copy);
                CanDragAndDropControl = false;
            }
            else if((UserControlList.SelectedItem as ControlInfo).ItemType == ItemType.Connect)
            {
                ParentFlowchartPage.GridContent.Cursor = Cursors.Cross;
                ParentFlowchartPage.IsDrawLineEnabled = true;
            }
        }

        public class ControlInfo : UIElement
        {
            public object ControlIcon { get; set; }
            public string ControlName { get; set; }
            public ItemType ItemType { get; set; }
        }
    }

    public enum ToolboxType
    {
        Flowchart,
        TextEditor,
        Unknow
    }
    public enum ItemType
    {
        Terminator,
        Process,
        Read,
        Write,
        Decision,
        Connection,
        Connect,
        Unknow
    }
}
