using Newtonsoft.Json;
using Portugol_Remake.Lists;
using Portugol_Remake.Utils;
using Portugol_Remake.Views;
using Portugol_Remake.Views.Shapes;
using Portugol_Remake.Windows;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Resources;
using System.Windows.Shapes;
using Process = Portugol_Remake.Views.Process;
using Line = System.Windows.Shapes.Line;
using Portugol_Remake.Windows.Dialog;

namespace Portugol_Remake.Pages
{
    /// <summary>
    /// Interaction logic for FlowchartEditor.xaml
    /// </summary>
    public partial class FlowchartEditorPage : Page
    {
        public bool IsSaved { get; set; }
        public string Path { get; set; }

        private object DragObject;
        private Point Offset;

        public List<FlowchartStructureRoot> FlowChart { get; private set; }

        private EditorWindow ParentWindow;

        public bool IsDrawLineEnabled = false;

        public FlowchartEditorPage()
        {
            InitializeComponent();
            
            this.Style = UserConfig.UserConfigurations.ThemeDictionary["FlowchartPageStyle"] as Style;
            MainGrid.Style = UserConfig.UserConfigurations.ThemeDictionary["FlowchartMainGridStyle"] as Style;

            GenerateFlowchart();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //GridContent.Height = GridContent.ActualHeight + 100;
            //GridContent.Width = GridContent.ActualWidth + 100;
            ParentWindow = Window.GetWindow(this) as EditorWindow;
            ToolboxPage toolboxPage = new ToolboxPage(ParentWindow, this, ToolboxType.Flowchart);
            ParentWindow.MenuSideBarToolbox.Click += MenuSideBarToolbox_Click;

            Binding heightBinding = new Binding();
            heightBinding.Source = ParentWindow.MenuSideBarToolboxFrame;
            heightBinding.Path = new PropertyPath("ActualHeight");
            heightBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            BindingOperations.SetBinding(toolboxPage, Page.HeightProperty, heightBinding);

            ParentWindow.MenuSideBarToolboxFrame.Content = toolboxPage;
        }

        private void MenuSideBarToolbox_Click(object sender, RoutedEventArgs e)
        {
            if(ParentWindow.MenuSideBarToolboxFrame.Visibility != Visibility.Visible)
                ParentWindow.MenuSideBarToolboxFrame.Visibility = Visibility.Visible;
            else
                ParentWindow.MenuSideBarToolboxFrame.Visibility = Visibility.Hidden;
        }

        private void GenerateFlowchart()
        {
            string converterfileDirectory = string.Format("{0}\\CoffeeIDE\\",Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
            if (!Directory.Exists(converterfileDirectory))
                Directory.CreateDirectory(converterfileDirectory);
            if (!File.Exists(converterfileDirectory + "FluxParser.jar"))
            {
                var jarArc = System.Windows.Application.GetResourceStream(new Uri("pack://application:,,,/Resources/Other/FlowchartConverter.jar"));
                using (var fileStream = File.Create(converterfileDirectory + "FluxParser.jar"))
                {
                    jarArc.Stream.Seek(0, SeekOrigin.Begin);
                    jarArc.Stream.CopyTo(fileStream);
                }
            }

            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.FileName = "javaw";
            process.StartInfo.Arguments = "-jar " + converterfileDirectory + "FluxParser.jar D:\\Downloads\\PortugolExercicios\\Fluxograma\\atividade_2.flux " + converterfileDirectory + "parsed.json";
            process.Start();
            process.WaitForExit();
            FlowChart = JsonConvert.DeserializeObject<List<FlowchartStructureRoot>>(File.ReadAllText(converterfileDirectory + "parsed.json"));

            for (int j = 0; j < FlowChart.Count; j++)
            {
                int l = -1;
                if (FlowChart[j].Content.Next != null)
                {
                    var item = FlowChart[j].Content.Next.Dest;
                    for (int k = 0; k < FlowChart.Count; k++)
                    {
                        if ((FlowChart[k].Content.BottonRight.X == item.BottonRight.X && FlowChart[k].Content.BottonRight.Y == item.BottonRight.Y) && (FlowChart[k].Content.Center.X == item.Center.X && FlowChart[k].Content.Center.Y == item.Center.Y) && (FlowChart[k].Content.TopLeft.X == item.TopLeft.X && FlowChart[k].Content.TopLeft.Y == item.TopLeft.Y) && FlowChart[k].Content.Text == item.Text)
                        {
                            l = k;
                            break;
                        }
                    }
                    if (l >= 0)
                        FlowChart[l].Content.Previous.Add(j);
                }
                else
                {
                    if (FlowChart[j].Content.IfTrue != null)
                    {
                        var item = FlowChart[j].Content.IfTrue.Dest;
                        for (int k = 0; k < FlowChart.Count; k++)
                        {
                            if ((FlowChart[k].Content.BottonRight.X == item.BottonRight.X && FlowChart[k].Content.BottonRight.Y == item.BottonRight.Y) && (FlowChart[k].Content.Center.X == item.Center.X && FlowChart[k].Content.Center.Y == item.Center.Y) && (FlowChart[k].Content.TopLeft.X == item.TopLeft.X && FlowChart[k].Content.TopLeft.Y == item.TopLeft.Y) && FlowChart[k].Content.Text == item.Text)
                            {
                                l = k;
                                break;
                            }
                        }
                        if (l >= 0)
                        {
                            FlowChart[l].Content.Previous.Add(j);
                            FlowChart[l].Content.IsFromTrue = true;
                        }
                    }
                    if (FlowChart[j].Content.IfFalse != null)
                    {
                        var item = FlowChart[j].Content.IfFalse.Dest;
                        for (int k = 0; k < FlowChart.Count; k++)
                        {
                            if ((FlowChart[k].Content.BottonRight.X == item.BottonRight.X && FlowChart[k].Content.BottonRight.Y == item.BottonRight.Y) && (FlowChart[k].Content.Center.X == item.Center.X && FlowChart[k].Content.Center.Y == item.Center.Y) && (FlowChart[k].Content.TopLeft.X == item.TopLeft.X && FlowChart[k].Content.TopLeft.Y == item.TopLeft.Y) && FlowChart[k].Content.Text == item.Text)
                            {
                                l = k;
                                break;
                            }

                        }
                        if (l >= 0)
                        {
                            FlowChart[l].Content.Previous.Add(j);
                            FlowChart[l].Content.IsFromFalse = true;
                        }
                    }
                }
            }
            int i = 0;
            foreach (var item in FlowChart)
            {
                double topPad = 0;
                object objectDraw = null;
                if (i > 0)
                {
                    topPad = item.Content.BottonRight.Y - FlowChart[i - 1].Content.TopLeft.Y;
                }

                switch(item.Type)
                {
                    case "Terminador":
                        objectDraw = new Terminator
                        {
                            VerticalAlignment = VerticalAlignment.Top,
                            HorizontalAlignment = HorizontalAlignment.Left,
                            Fill = new SolidColorBrush(Color.FromArgb(item.Content.FormColor.Alpha, item.Content.FormColor.Red, item.Content.FormColor.Green, item.Content.FormColor.Blue)),
                            Text = item.Content.Text,
                            ListPosition = i,
                            Margin = new Thickness(item.Content.TopLeft.X, item.Content.TopLeft.Y, 0, 0),
                            Width = item.Content.BottonRight.X - item.Content.TopLeft.X,
                            Height = item.Content.BottonRight.Y - item.Content.TopLeft.Y,
                        };
                        break;
                    case "Processo":
                        objectDraw = new Views.Process
                        {
                            VerticalAlignment = VerticalAlignment.Top,
                            HorizontalAlignment = HorizontalAlignment.Left,
                            Fill = new SolidColorBrush(Color.FromArgb(item.Content.FormColor.Alpha, item.Content.FormColor.Red, item.Content.FormColor.Green, item.Content.FormColor.Blue)),
                            Text = item.Content.Text,
                            ListPosition = i,
                            Margin = new Thickness(item.Content.TopLeft.X, item.Content.TopLeft.Y, 0, 0),
                            Width = item.Content.BottonRight.X - item.Content.TopLeft.X,
                            Height = item.Content.BottonRight.Y - item.Content.TopLeft.Y
                        };
                        break;
                    case "Escrita":
                        objectDraw = new Write
                        {
                            VerticalAlignment = VerticalAlignment.Top,
                            HorizontalAlignment = HorizontalAlignment.Left,
                            Fill = new SolidColorBrush(Color.FromArgb(item.Content.FormColor.Alpha, item.Content.FormColor.Red, item.Content.FormColor.Green, item.Content.FormColor.Blue)),
                            Text = item.Content.Text,
                            ListPosition = i,
                            Margin = new Thickness(item.Content.TopLeft.X, item.Content.TopLeft.Y, 0, 0),
                            Width = item.Content.BottonRight.X - item.Content.TopLeft.X,
                            Height = item.Content.BottonRight.Y - item.Content.TopLeft.Y
                        };
                        break;
                    case "Leitura":
                        objectDraw = new Read
                        {
                            VerticalAlignment = VerticalAlignment.Top,
                            HorizontalAlignment = HorizontalAlignment.Left,
                            Fill = new SolidColorBrush(Color.FromArgb(item.Content.FormColor.Alpha, item.Content.FormColor.Red, item.Content.FormColor.Green, item.Content.FormColor.Blue)),
                            Text = item.Content.Text,
                            ListPosition = i,
                            Margin = new Thickness(item.Content.TopLeft.X, item.Content.TopLeft.Y, 0, 0),
                            Width = item.Content.BottonRight.X - item.Content.TopLeft.X,
                            Height = item.Content.BottonRight.Y - item.Content.TopLeft.Y
                        };
                        break;
                    case "Decisao":
                        objectDraw = new Decision
                        {
                            VerticalAlignment = VerticalAlignment.Top,
                            HorizontalAlignment = HorizontalAlignment.Left,
                            Fill = new SolidColorBrush(Color.FromArgb(item.Content.FormColor.Alpha, item.Content.FormColor.Red, item.Content.FormColor.Green, item.Content.FormColor.Blue)),
                            Text = item.Content.Text,
                            ListPosition = i,
                            Margin = new Thickness(item.Content.TopLeft.X - 1, item.Content.TopLeft.Y - 0.5, 0, 0),
                            Width = item.Content.BottonRight.X - item.Content.TopLeft.X + 1.5,
                            Height = item.Content.BottonRight.Y - item.Content.TopLeft.Y + 1,
                        };
                        break;
                    case "Conexao":
                        objectDraw = new Connection
                        {
                            VerticalAlignment = VerticalAlignment.Top,
                            HorizontalAlignment = HorizontalAlignment.Left,
                            Fill = new SolidColorBrush(Color.FromArgb(item.Content.FormColor.Alpha, item.Content.FormColor.Red, item.Content.FormColor.Green, item.Content.FormColor.Blue)),
                            Margin = new Thickness(item.Content.TopLeft.X, item.Content.TopLeft.Y, 0, 0),
                            ListPosition = i,
                            Width = item.Content.BottonRight.X - item.Content.TopLeft.X,
                            Height = item.Content.BottonRight.Y - item.Content.TopLeft.Y
                        };
                        break;
                }
                DrawLines(item);
                (objectDraw as UIElement).PreviewMouseDown += FlowchartEditorPage_PreviewMouseDown;
                (objectDraw as UIElement).PreviewMouseMove += FlowchartEditorPage_PreviewMouseMove;
                (objectDraw as UIElement).PreviewMouseUp += FlowchartEditorPage_PreviewMouseUp;
                (objectDraw as UIElement).MouseEnter += FlowchartEditorPage_MouseEnter;
                (objectDraw as UIElement).MouseLeave += FlowchartEditorPage_MouseLeave;
                (objectDraw as UserControl).MouseDoubleClick += FlowchartEditorPage_MouseDoubleClick;
                GridContent.Children.Add(objectDraw as UIElement);
                i++;
            }
        }

        public void GridContent_Drop(object sender, DragEventArgs e)
        {
            Point dropPoint = e.GetPosition(e.OriginalSource as UIElement);
            ParentWindow.MenuSideBarToolboxFrame.Visibility = Visibility.Hidden;
            object objectDraw = null;
            switch (e.Data.GetData(typeof(ItemType)))
            {
                case ItemType.Terminator:
                    {
                        string text;
                        bool canAdd = true;

                        if (FlowChart.Exists(x => x.Content.Text == "Inicio" && x.Type == "Terminador") && FlowChart.Exists(x => x.Content.Text == "Fim" && x.Type == "Terminador"))
                            canAdd = false;

                        if (!FlowChart.Exists(x => x.Content.Text == "Inicio" && x.Type == "Terminador"))
                            text = "Inicio";
                        else
                            text = "Fim";

                        if (!FlowChart.Exists(x => x.Content.Text == "Fim" && x.Type == "Terminador"))
                            text = "Fim";

                        if (canAdd)
                        {
                            var listItem = new FlowchartStructureRoot
                            {
                                Type = "Terminador",
                                Content = new FlowchartStructure
                                {
                                    TopLeft = new XY
                                    {
                                        X = dropPoint.X,
                                        Y = dropPoint.Y
                                    },

                                    BottonRight = new XY
                                    {
                                        X = dropPoint.X + 100,
                                        Y = dropPoint.Y + 35
                                    },

                                    Center = new XY
                                    {
                                        X = (dropPoint.X + (dropPoint.X + 100)) / 2,
                                        Y = (dropPoint.Y + (dropPoint.Y + 35)) / 2
                                    },

                                    Text = text,

                                    FormColor = new FormColor
                                    {
                                        Alpha = 255,
                                        Blue = 255,
                                        Green = 153,
                                        Red = 255,
                                        RGB = -26113,
                                        Transparency = 1
                                    },
                                }
                            };
                            FlowChart.Add(listItem);

                            objectDraw = new Views.Terminator
                            {
                                VerticalAlignment = VerticalAlignment.Top,
                                HorizontalAlignment = HorizontalAlignment.Left,
                                Fill = new SolidColorBrush(Color.FromArgb(listItem.Content.FormColor.Alpha, listItem.Content.FormColor.Red, listItem.Content.FormColor.Green, listItem.Content.FormColor.Blue)),
                                Text = text,
                                ListPosition = FlowChart.Count - 1,
                                Margin = new Thickness(dropPoint.X, dropPoint.Y, 0, 0),
                                Width = 100,
                                Height = 35,
                            };

                        }
                    }
                    break;
                case ItemType.Process:
                    {
                        string text = "";
                        var listItem = new FlowchartStructureRoot
                        {
                            Type = "Processo",
                            Content = new FlowchartStructure
                            {
                                TopLeft = new XY
                                {
                                    X = dropPoint.X,
                                    Y = dropPoint.Y
                                },

                                BottonRight = new XY
                                {
                                    X = dropPoint.X + 100,
                                    Y = dropPoint.Y + 35
                                },

                                Center = new XY
                                {
                                    X = (dropPoint.X + (dropPoint.X + 100)) / 2,
                                    Y = (dropPoint.Y + (dropPoint.Y + 35)) / 2
                                },

                                Text = text,

                                FormColor = new FormColor
                                {
                                    Alpha = 255,
                                    Blue = 194,
                                    Green = 233,
                                    Red = 208,
                                    RGB = -3085886,
                                    Transparency = 1
                                },
                            }
                        };
                        FlowChart.Add(listItem);

                        objectDraw = new Views.Process
                        {
                            VerticalAlignment = VerticalAlignment.Top,
                            HorizontalAlignment = HorizontalAlignment.Left,
                            Fill = new SolidColorBrush(Color.FromArgb(listItem.Content.FormColor.Alpha, listItem.Content.FormColor.Red, listItem.Content.FormColor.Green, listItem.Content.FormColor.Blue)),
                            Text = text,
                            ListPosition = FlowChart.Count - 1,
                            Margin = new Thickness(dropPoint.X, dropPoint.Y, 0, 0),
                            Width = 100,
                            Height = 35,
                        };

                    }
                    break;
                case ItemType.Write:
                    {
                        string text = "";
                        var listItem = new FlowchartStructureRoot
                        {
                            Type = "Escrita",
                            Content = new FlowchartStructure
                            {
                                TopLeft = new XY
                                {
                                    X = dropPoint.X,
                                    Y = dropPoint.Y
                                },

                                BottonRight = new XY
                                {
                                    X = dropPoint.X + 100,
                                    Y = dropPoint.Y + 35
                                },

                                Center = new XY
                                {
                                    X = (dropPoint.X + (dropPoint.X + 100)) / 2,
                                    Y = (dropPoint.Y + (dropPoint.Y + 35)) / 2
                                },

                                Text = text,

                                FormColor = new FormColor
                                {
                                    Alpha = 255,
                                    Blue = 227,
                                    Green = 224,
                                    Red = 187,
                                    RGB = -4464413,
                                    Transparency = 1
                                },
                            }
                        };
                        FlowChart.Add(listItem);

                        objectDraw = new Views.Write
                        {
                            VerticalAlignment = VerticalAlignment.Top,
                            HorizontalAlignment = HorizontalAlignment.Left,
                            Fill = new SolidColorBrush(Color.FromArgb(listItem.Content.FormColor.Alpha, listItem.Content.FormColor.Red, listItem.Content.FormColor.Green, listItem.Content.FormColor.Blue)),
                            Text = text,
                            ListPosition = FlowChart.Count - 1,
                            Margin = new Thickness(dropPoint.X, dropPoint.Y, 0, 0),
                            Width = 100,
                            Height = 35,
                        };

                    }
                    break;
                case ItemType.Read:
                    {
                        string text = "";
                        var listItem = new FlowchartStructureRoot
                        {
                            Type = "Leitura",
                            Content = new FlowchartStructure
                            {
                                TopLeft = new XY
                                {
                                    X = dropPoint.X,
                                    Y = dropPoint.Y
                                },

                                BottonRight = new XY
                                {
                                    X = dropPoint.X + 100,
                                    Y = dropPoint.Y + 35
                                },

                                Center = new XY
                                {
                                    X = (dropPoint.X + (dropPoint.X + 100)) / 2,
                                    Y = (dropPoint.Y + (dropPoint.Y + 35)) / 2
                                },

                                Text = text,

                                FormColor = new FormColor
                                {
                                    Alpha = 255,
                                    Blue = 204,
                                    Green = 204,
                                    Red = 255,
                                    RGB = -13108,
                                    Transparency = 1
                                },
                            }
                        };
                        FlowChart.Add(listItem);

                        objectDraw = new Views.Read
                        {
                            VerticalAlignment = VerticalAlignment.Top,
                            HorizontalAlignment = HorizontalAlignment.Left,
                            Fill = new SolidColorBrush(Color.FromArgb(listItem.Content.FormColor.Alpha, listItem.Content.FormColor.Red, listItem.Content.FormColor.Green, listItem.Content.FormColor.Blue)),
                            Text = text,
                            ListPosition = FlowChart.Count - 1,
                            Margin = new Thickness(dropPoint.X, dropPoint.Y, 0, 0),
                            Width = 100,
                            Height = 35,
                        };
                    }
                    break;
                case ItemType.Decision:
                    {
                        string text = "";
                        var listItem = new FlowchartStructureRoot
                        {
                            Type = "Decisao",
                            Content = new FlowchartStructure
                            {
                                TopLeft = new XY
                                {
                                    X = dropPoint.X,
                                    Y = dropPoint.Y
                                },

                                BottonRight = new XY
                                {
                                    X = dropPoint.X + 120,
                                    Y = dropPoint.Y + 60
                                },

                                Center = new XY
                                {
                                    X = (dropPoint.X + (dropPoint.X + 120)) / 2,
                                    Y = (dropPoint.Y + (dropPoint.Y + 60)) / 2
                                },

                                Text = text,

                                FormColor = new FormColor
                                {
                                    Alpha = 255,
                                    Blue = 255,
                                    Green = 204,
                                    Red = 204,
                                    RGB = -3355393,
                                    Transparency = 1
                                },
                            }
                        };
                        FlowChart.Add(listItem);

                        objectDraw = new Views.Decision
                        {
                            VerticalAlignment = VerticalAlignment.Top,
                            HorizontalAlignment = HorizontalAlignment.Left,
                            Fill = new SolidColorBrush(Color.FromArgb(listItem.Content.FormColor.Alpha, listItem.Content.FormColor.Red, listItem.Content.FormColor.Green, listItem.Content.FormColor.Blue)),
                            Text = text,
                            ListPosition = FlowChart.Count - 1,
                            Margin = new Thickness(dropPoint.X, dropPoint.Y, 0, 0),
                            Width = 120,
                            Height = 60,
                        };
                    }
                    break;
                case ItemType.Connection:
                    {
                        var listItem = new FlowchartStructureRoot
                        {
                            Type = "Conexao",
                            Content = new FlowchartStructure
                            {
                                TopLeft = new XY
                                {
                                    X = dropPoint.X,
                                    Y = dropPoint.Y
                                },

                                BottonRight = new XY
                                {
                                    X = dropPoint.X + 30,
                                    Y = dropPoint.Y + 30
                                },

                                Center = new XY
                                {
                                    X = (dropPoint.X + (dropPoint.X + 30)) / 2,
                                    Y = (dropPoint.Y + (dropPoint.Y + 30)) / 2
                                },

                                Text = "",

                                FormColor = new FormColor
                                {
                                    Alpha = 255,
                                    Blue = 0,
                                    Green = 255,
                                    Red = 255,
                                    RGB = -256,
                                    Transparency = 1
                                },
                            }
                        };
                        FlowChart.Add(listItem);

                        objectDraw = new Views.Connection
                        {
                            VerticalAlignment = VerticalAlignment.Top,
                            HorizontalAlignment = HorizontalAlignment.Left,
                            Fill = new SolidColorBrush(Color.FromArgb(listItem.Content.FormColor.Alpha, listItem.Content.FormColor.Red, listItem.Content.FormColor.Green, listItem.Content.FormColor.Blue)),
                            ListPosition = FlowChart.Count - 1,
                            Margin = new Thickness(dropPoint.X, dropPoint.Y, 0, 0),
                            Width = 30,
                            Height = 30,
                        };
                    }
                    break;
            }
            (objectDraw as UIElement).PreviewMouseDown += FlowchartEditorPage_PreviewMouseDown;
            (objectDraw as UIElement).PreviewMouseMove += FlowchartEditorPage_PreviewMouseMove;
            (objectDraw as UIElement).PreviewMouseUp += FlowchartEditorPage_PreviewMouseUp;
            (objectDraw as UIElement).MouseEnter += FlowchartEditorPage_MouseEnter;
            (objectDraw as UIElement).MouseLeave += FlowchartEditorPage_MouseLeave;
            (objectDraw as UserControl).MouseDoubleClick += FlowchartEditorPage_MouseDoubleClick;
            GridContent.Children.Add(objectDraw as UIElement);
        }

        private void FlowchartEditorPage_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (DragObject == null)
                    return;

                (DragObject as FrameworkElement).Margin = new Thickness(e.GetPosition((DragObject as FrameworkElement).Parent as FrameworkElement).X - Offset.X, e.GetPosition((DragObject as FrameworkElement).Parent as FrameworkElement).Y - Offset.Y, 0, 0);
                int currIndex = -1;
                List<int> prevIndexList = new List<int>();
                switch (DragObject.GetType().Name)
                {
                    case "Terminator":
                        currIndex = (DragObject as Terminator).ListPosition;
                        break;
                    case "Process":
                        currIndex = (DragObject as Process).ListPosition;
                        break;
                    case "Write":
                        currIndex = (DragObject as Write).ListPosition;
                        break;
                    case "Read":
                        currIndex = (DragObject as Read).ListPosition;
                        break;
                    case "Decision":
                        currIndex = (DragObject as Decision).ListPosition;
                        break;
                    case "Connection":
                        currIndex = (DragObject as Connection).ListPosition;
                        break;
                }
                prevIndexList = FlowChart[currIndex].Content.Previous;

                FlowChart[currIndex].Content.TopLeft.X = (DragObject as FrameworkElement).Margin.Left;
                FlowChart[currIndex].Content.TopLeft.Y = (DragObject as FrameworkElement).Margin.Top;
                FlowChart[currIndex].Content.BottonRight.X = (DragObject as FrameworkElement).Margin.Left + (DragObject as FrameworkElement).Width;
                FlowChart[currIndex].Content.BottonRight.Y = (DragObject as FrameworkElement).Margin.Top + (DragObject as FrameworkElement).Height;
                FlowChart[currIndex].Content.Center.X = (FlowChart[currIndex].Content.TopLeft.X + FlowChart[currIndex].Content.BottonRight.X) / 2;
                FlowChart[currIndex].Content.Center.Y = (FlowChart[currIndex].Content.TopLeft.Y + FlowChart[currIndex].Content.BottonRight.Y) / 2;

                DrawLines(FlowChart[currIndex]);
                foreach (var prevIndex in prevIndexList)
                {
                    if (prevIndex >= 0)
                    {
                        if (FlowChart[prevIndex].Content.Next != null)
                        {
                            FlowChart[prevIndex].Content.Next.Dest = FlowChart[currIndex].Content;
                        }
                        else
                        {
                            if (FlowChart[currIndex].Content.IsFromTrue)
                                FlowChart[prevIndex].Content.IfTrue.Dest = FlowChart[currIndex].Content;
                            else if(FlowChart[currIndex].Content.IsFromFalse)
                                FlowChart[prevIndex].Content.IfFalse.Dest = FlowChart[currIndex].Content;
                        }
                        DrawLines(FlowChart[prevIndex]);
                    }
                }
            }
            
        }

        private void FlowchartEditorPage_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            switch (sender.GetType().Name)
            {
                case "Process":
                    {
                        var obj = sender as Process;
                        InputBox inputBox = new InputBox(obj.Text);
                        inputBox.Owner = ParentWindow;
                        if (inputBox.ShowDialog() == true)
                        {
                            obj.Text = inputBox.UserInput;
                            FlowChart[obj.ListPosition].Content.Text = obj.Text;
                        } 
                    }
                    break;
                case "Write":
                    {
                        var obj = sender as Write;
                        InputBox inputBox = new InputBox(obj.Text);
                        inputBox.Owner = ParentWindow;
                        if (inputBox.ShowDialog() == true)
                        {
                            obj.Text = inputBox.UserInput;
                            FlowChart[obj.ListPosition].Content.Text = obj.Text;
                        }
                    }
                    break;
                case "Read":
                    {
                        var obj = sender as Read;
                        InputBox inputBox = new InputBox(obj.Text);
                        inputBox.Owner = ParentWindow;
                        if (inputBox.ShowDialog() == true)
                        {
                            obj.Text = inputBox.UserInput;
                            FlowChart[obj.ListPosition].Content.Text = obj.Text;
                        }
                    }
                    break;
                case "Decision":
                    {
                        var obj = sender as Decision;
                        InputBox inputBox = new InputBox(obj.Text);
                        inputBox.Owner = ParentWindow;
                        if (inputBox.ShowDialog() == true)
                        {
                            obj.Text = inputBox.UserInput;
                            FlowChart[obj.ListPosition].Content.Text = obj.Text;
                        }
                    }
                    break;
            }
        }

        private void FlowchartEditorPage_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            DragObject = sender;
            Offset = e.GetPosition((DragObject as UIElement));
            (DragObject as UIElement).CaptureMouse();
        }

        private void FlowchartEditorPage_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if(DragObject != null)
                (DragObject as UIElement).ReleaseMouseCapture();
            DragObject = null;
        }

        private void FlowchartEditorPage_MouseLeave(object sender, MouseEventArgs e)
        {
            if(IsDrawLineEnabled)
            {
                switch (sender.GetType().Name)
                {
                    case "Terminator":
                        {
                            var obj = sender as Terminator;
                            obj.HideLineBox();
                        }
                        break;
                    case "Process":
                        {
                            var obj = sender as Process;
                            obj.HideLineBox();
                        }
                        break;
                    case "Write":
                        {
                            var obj = sender as Write;
                            obj.HideLineBox();
                        }
                        break;
                    case "Read":
                        {
                            var obj = sender as Read;
                            obj.HideLineBox();
                        }
                        break;
                    case "Decision":
                        {
                            var obj = sender as Decision;
                            obj.HideLineBox();
                        }
                        break;
                    case "Connection":
                        {
                            var obj = sender as Connection;
                            obj.HideLineBox();
                        }
                        break;
                }
            }
        }

        private void FlowchartEditorPage_MouseEnter(object sender, MouseEventArgs e)
        {
            if (IsDrawLineEnabled)
            {
                switch (sender.GetType().Name)
                {
                    case "Terminator":
                        {
                            var obj = sender as Terminator;
                            obj.ShowLineBox();
                        }
                        break;
                    case "Process":
                        {
                            var obj = sender as Process;
                            obj.ShowLineBox();
                        }
                        break;
                    case "Write":
                        {
                            var obj = sender as Write;
                            obj.ShowLineBox();
                        }
                        break;
                    case "Read":
                        {
                            var obj = sender as Read;
                            obj.ShowLineBox();
                        }
                        break;
                    case "Decision":
                        {
                            var obj = sender as Decision;
                            obj.ShowLineBox();
                        }
                        break;
                    case "Connection":
                        {
                            var obj = sender as Connection;
                            obj.ShowLineBox();
                        }
                        break;
                }
            }
        }

        private void Grid_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                var offsetY = ContentScroller.VerticalOffset;
                var offsetX = ContentScroller.HorizontalOffset;
                //MainGrid.Style;
                
                var matTrans = GridContent.LayoutTransform as MatrixTransform;
                var pos1 = e.GetPosition(MainGrid);

                var scale = e.Delta > 0 ? 1.1 : 1 / 1.1;

                var mat = matTrans.Matrix;
                mat.ScaleAt(scale, scale, pos1.X, pos1.Y);
                matTrans.Matrix = mat;

                ContentScroller.ScrollToVerticalOffset(offsetY * scale);
                ContentScroller.ScrollToHorizontalOffset(offsetX * scale);

                e.Handled = true;
               
            }
        }

        private void ContentScroller_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                e.Handled = true;
            }
        }

        private void DrawLines(FlowchartStructureRoot item)
        {

            if (item.Content.Next != null)
            {
                Views.Shapes.Line line = new Views.Shapes.Line()
                {
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    StrokeThickness = 1,
                    Stroke = new SolidColorBrush(Color.FromRgb(255, 255, 255)),
                    Fill = new SolidColorBrush(Color.FromRgb(255, 255, 255)),
                    StartPosition = item.Content.Next.OrigPos,
                    EndPosition = item.Content.Next.DestPos,
                };

                switch (item.Content.Next.OrigPos)
                {
                    case Position.Top:
                        line.X1 = item.Content.Center.X;
                        line.Y1 = item.Content.TopLeft.Y;
                        break;
                    case Position.Bottom:
                        line.X1 = item.Content.Center.X;
                        line.Y1 = item.Content.BottonRight.Y;
                        break;
                    case Position.Left:
                        line.X1 = item.Content.TopLeft.X;
                        line.Y1 = item.Content.Center.Y;
                        break;
                    case Position.Right:
                        line.X1 = item.Content.BottonRight.X;
                        line.Y1 = item.Content.Center.Y;
                        break;
                }

                switch (item.Content.Next.DestPos)
                {
                    case Position.Top:
                        line.X2 = item.Content.Next.Dest.Center.X;
                        line.Y2 = item.Content.Next.Dest.TopLeft.Y - 5;
                        break;
                    case Position.Bottom:
                        line.X2 = item.Content.Next.Dest.Center.X;
                        line.X2 = item.Content.Next.Dest.BottonRight.Y + 5;
                        break;
                    case Position.Left:
                        line.X2 = item.Content.Next.Dest.TopLeft.X - 2;
                        line.Y2 = item.Content.Next.Dest.Center.Y;
                        break;
                    case Position.Right:
                        line.X2 = item.Content.Next.Dest.BottonRight.X + 2;
                        line.Y2 = item.Content.Next.Dest.Center.Y;
                        break;
                }
                //GridContent.Children.Add(line);
                if (item.Content.Next.LineIndex < 0)
                {
                    GridContent.Children.Add(line);
                    item.Content.Next.LineIndex = GridContent.Children.IndexOf(line);
                }
                else
                {
                    GridContent.Children.RemoveAt(item.Content.Next.LineIndex);
                    GridContent.Children.Insert(item.Content.Next.LineIndex, line);
                }
            }
            else
            {
                if (item.Content.IfTrue != null)
                {
                    Views.Shapes.Line line = new Views.Shapes.Line()
                    {
                        VerticalAlignment = VerticalAlignment.Top,
                        HorizontalAlignment = HorizontalAlignment.Left,
                        StrokeThickness = 1,
                        Stroke = new SolidColorBrush(Color.FromRgb(255, 255, 255)),
                        Fill = new SolidColorBrush(Color.FromRgb(255, 255, 255)),
                        StartPosition = item.Content.IfTrue.OrigPos,
                        EndPosition = item.Content.IfTrue.DestPos,
                    };

                    switch (item.Content.IfTrue.OrigPos)
                    {
                        case Position.Top:
                            line.X1 = item.Content.Center.X;
                            line.Y1 = item.Content.TopLeft.Y;
                            break;
                        case Position.Bottom:
                            line.X1 = item.Content.Center.X;
                            line.Y1 = item.Content.BottonRight.Y;
                            break;
                        case Position.Left:
                            line.X1 = item.Content.TopLeft.X;
                            line.Y1 = item.Content.Center.Y;
                            break;
                        case Position.Right:
                            line.X1 = item.Content.BottonRight.X;
                            line.Y1 = item.Content.Center.Y;
                            break;
                    }

                    switch (item.Content.IfTrue.DestPos)
                    {
                        case Position.Top:
                            line.X2 = item.Content.IfTrue.Dest.Center.X;
                            line.Y2 = item.Content.IfTrue.Dest.TopLeft.Y - 5;
                            break;
                        case Position.Bottom:
                            line.X2 = item.Content.IfTrue.Dest.Center.X;
                            line.X2 = item.Content.IfTrue.Dest.BottonRight.Y + 5;
                            break;
                        case Position.Left:
                            line.X2 = item.Content.IfTrue.Dest.TopLeft.X - 2;
                            line.Y2 = item.Content.IfTrue.Dest.Center.Y;
                            break;
                        case Position.Right:
                            line.X2 = item.Content.IfTrue.Dest.BottonRight.X + 2;
                            line.Y2 = item.Content.IfTrue.Dest.Center.Y;
                            break;
                    }

                    if (item.Content.IfTrue.LineIndex < 0)
                    {
                        GridContent.Children.Add(line);
                        item.Content.IfTrue.LineIndex = GridContent.Children.IndexOf(line);
                    }
                    else
                    {
                        GridContent.Children.RemoveAt(item.Content.IfTrue.LineIndex);
                        GridContent.Children.Insert(item.Content.IfTrue.LineIndex, line);
                    }

                }

                if (item.Content.IfFalse != null)
                {
                    Views.Shapes.Line line = new Views.Shapes.Line()
                    {
                        VerticalAlignment = VerticalAlignment.Top,
                        HorizontalAlignment = HorizontalAlignment.Left,
                        StrokeThickness = 1,
                        Stroke = new SolidColorBrush(Color.FromRgb(255, 255, 255)),
                        Fill = new SolidColorBrush(Color.FromRgb(255, 255, 255)),
                        StartPosition = item.Content.IfFalse.OrigPos,
                        EndPosition = item.Content.IfFalse.DestPos,
                    };

                    switch (item.Content.IfFalse.OrigPos)
                    {
                        case Position.Top:
                            line.X1 = item.Content.Center.X;
                            line.Y1 = item.Content.TopLeft.Y;
                            break;
                        case Position.Bottom:
                            line.X1 = item.Content.Center.X;
                            line.Y1 = item.Content.BottonRight.Y;
                            break;
                        case Position.Left:
                            line.X1 = item.Content.TopLeft.X;
                            line.Y1 = item.Content.Center.Y;
                            break;
                        case Position.Right:
                            line.X1 = item.Content.BottonRight.X;
                            line.Y1 = item.Content.Center.Y;
                            break;
                    }

                    switch (item.Content.IfFalse.DestPos)
                    {
                        case Position.Top:
                            line.X2 = item.Content.IfFalse.Dest.Center.X;
                            line.Y2 = item.Content.IfFalse.Dest.TopLeft.Y - 5;
                            break;
                        case Position.Bottom:
                            line.X2 = item.Content.IfFalse.Dest.Center.X;
                            line.X2 = item.Content.IfFalse.Dest.BottonRight.Y + 5;
                            break;
                        case Position.Left:
                            line.X2 = item.Content.IfFalse.Dest.TopLeft.X - 2;
                            line.Y2 = item.Content.IfFalse.Dest.Center.Y;
                            break;
                        case Position.Right:
                            line.X2 = item.Content.IfFalse.Dest.BottonRight.X + 2;
                            line.Y2 = item.Content.IfFalse.Dest.Center.Y;
                            break;
                    }

                    if (item.Content.IfFalse.LineIndex < 0)
                    {
                        GridContent.Children.Add(line);
                        item.Content.IfFalse.LineIndex = GridContent.Children.IndexOf(line);
                    }
                    else
                    {
                        GridContent.Children.RemoveAt(item.Content.IfFalse.LineIndex);
                        GridContent.Children.Insert(item.Content.IfFalse.LineIndex, line);
                    }

                }
            }
        }
    }
}
