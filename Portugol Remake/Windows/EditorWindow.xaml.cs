using Microsoft.Win32;
using Portugol_Remake.Lists;
using Portugol_Remake.Pages;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Resources;
using System.Windows.Shapes;

namespace Portugol_Remake.Windows
{
    /// <summary>
    /// Interaction logic for TextEditor.xaml
    /// </summary>
    public partial class EditorWindow : Window
    {
        private int NewFileCount { get; set; } = 1;
        private static IntPtr WindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case 0x0024:
                    WmGetMinMaxInfo(hwnd, lParam);
                    handled = true;
                    break;
            }
            return (IntPtr)0;
        }

        private static void WmGetMinMaxInfo(IntPtr hwnd, IntPtr lParam)
        {
            MINMAXINFO mmi = (MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(MINMAXINFO));
            int MONITOR_DEFAULTTONEAREST = 0x00000002;
            IntPtr monitor = MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST);
            if (monitor != IntPtr.Zero)
            {
                MONITORINFO monitorInfo = new MONITORINFO();
                GetMonitorInfo(monitor, monitorInfo);
                RECT rcWorkArea = monitorInfo.rcWork;
                RECT rcMonitorArea = monitorInfo.rcMonitor;
                mmi.ptMaxPosition.x = Math.Abs(rcWorkArea.left - rcMonitorArea.left);
                mmi.ptMaxPosition.y = Math.Abs(rcWorkArea.top - rcMonitorArea.top);
                mmi.ptMaxSize.x = Math.Abs(rcWorkArea.right - rcWorkArea.left);
                mmi.ptMaxSize.y = Math.Abs(rcWorkArea.bottom - rcWorkArea.top);
            }
            Marshal.StructureToPtr(mmi, lParam, true);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            /// <summary>x coordinate of point.</summary>
            public int x;
            /// <summary>y coordinate of point.</summary>
            public int y;
            /// <summary>Construct a point of coordinates (x,y).</summary>
            public POINT(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MINMAXINFO
        {
            public POINT ptReserved;
            public POINT ptMaxSize;
            public POINT ptMaxPosition;
            public POINT ptMinTrackSize;
            public POINT ptMaxTrackSize;
        };

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class MONITORINFO
        {
            public int cbSize = Marshal.SizeOf(typeof(MONITORINFO));
            public RECT rcMonitor = new RECT();
            public RECT rcWork = new RECT();
            public int dwFlags = 0;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 0)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
            public static readonly RECT Empty = new RECT();
            public int Width { get { return Math.Abs(right - left); } }
            public int Height { get { return bottom - top; } }
            public RECT(int left, int top, int right, int bottom)
            {
                this.left = left;
                this.top = top;
                this.right = right;
                this.bottom = bottom;
            }
            public RECT(RECT rcSrc)
            {
                left = rcSrc.left;
                top = rcSrc.top;
                right = rcSrc.right;
                bottom = rcSrc.bottom;
            }
            public bool IsEmpty { get { return left >= right || top >= bottom; } }
            public override string ToString()
            {
                if (this == Empty) { return "RECT {Empty}"; }
                return "RECT { left : " + left + " / top : " + top + " / right : " + right + " / bottom : " + bottom + " }";
            }
            public override bool Equals(object obj)
            {
                if (!(obj is Rect)) { return false; }
                return (this == (RECT)obj);
            }
            /// <summary>Return the HashCode for this struct (not garanteed to be unique)</summary>
            public override int GetHashCode() => left.GetHashCode() + top.GetHashCode() + right.GetHashCode() + bottom.GetHashCode();
            /// <summary> Determine if 2 RECT are equal (deep compare)</summary>
            public static bool operator ==(RECT rect1, RECT rect2) { return (rect1.left == rect2.left && rect1.top == rect2.top && rect1.right == rect2.right && rect1.bottom == rect2.bottom); }
            /// <summary> Determine if 2 RECT are different(deep compare)</summary>
            public static bool operator !=(RECT rect1, RECT rect2) { return !(rect1 == rect2); }
        }

        [DllImport("user32")]
        internal static extern bool GetMonitorInfo(IntPtr hMonitor, MONITORINFO lpmi);

        [DllImport("User32")]
        internal static extern IntPtr MonitorFromWindow(IntPtr handle, int flags);
        public EditorWindow()
        {
            InitializeComponent();
            SourceInitialized += TextEditorWindow_SourceInitialized;
            //dict.Add(test.Keys[]);
            this.Style = UserConfig.UserConfigurations.ThemeDictionary["WindowTheme"] as Style;
            TabController.Style = UserConfig.UserConfigurations.ThemeDictionary["TabController"] as Style;
            TabConsoleController.Style = UserConfig.UserConfigurations.ThemeDictionary["TabConsoleController"] as Style;
            TabController.Resources.Add(typeof(TabItem), UserConfig.UserConfigurations.ThemeDictionary["TabStyle"] as Style);
            TabConsoleController.Resources.Add(typeof(TabItem), UserConfig.UserConfigurations.ThemeDictionary["TabConsoleStyle"] as Style);

            MinimizeButton.Style = UserConfig.UserConfigurations.ThemeDictionary["CaptionButtonStyle"] as Style;
            ResizeButton.Style = UserConfig.UserConfigurations.ThemeDictionary["CaptionButtonStyle"] as Style;
            CloseButton.Style = UserConfig.UserConfigurations.ThemeDictionary["CaptionButtonStyle"] as Style;

            MenuBar.Style = UserConfig.UserConfigurations.ThemeDictionary["MenuStyle"] as Style;
            //MenuBar.ContextMenu.Style = UserConfig.UserConfigurations.ThemeDictionary["ContextMenuStyle"] as Style;
            //MenuBar.Resources.Add(typeof(Separator), UserConfig.UserConfigurations.ThemeDictionary["SeparatorStyle"] as Style);
            MenuBar.Resources.Add(typeof(MenuItem), UserConfig.UserConfigurations.ThemeDictionary["MenuItemStyle"] as Style);
            MenuSideBar.Resources.Add(typeof(MenuItem), UserConfig.UserConfigurations.ThemeDictionary["MenuItemSideBarStyle"] as Style);

            MenuSideBarToolboxFrame.Style = UserConfig.UserConfigurations.ThemeDictionary["ToolboxWindowFrameStyle"] as Style;

            AddNewFlowchartFileToEditor();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ResizeButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                ResizeButton.Content = Resources["MaximizeButton"];
                this.WindowState = WindowState.Normal;
            }
            else
            {
                ResizeButton.Content = Resources["RestoreButton"];
                this.WindowState = WindowState.Maximized;
            }
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void TextEditorWindow_SourceInitialized(object sender, EventArgs e)
        {
            IntPtr handle = (new WindowInteropHelper(this)).Handle;
            HwndSource.FromHwnd(handle).AddHook(new HwndSourceHook(WindowProc));
        }

        private void MenuItem_Open_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog();
            fd.Filter = "Algoritimo |*.alg|Fluxograma |*.flux";
            fd.ShowDialog();
            if (!string.IsNullOrWhiteSpace(fd.FileName))
            {
                if(fd.FileName.EndsWith(".alg"))
                    AddNewTextFileToEditor(File.ReadAllText(fd.FileName), new Uri(fd.FileName).Segments.Last(), fd.FileName);
            }
        }

        private void MenuItem_Newfile_Click(object sender, RoutedEventArgs e)
        {
            AddNewTextFileToEditor("");
        }

        private void MenuItem_Save_Click(object sender, RoutedEventArgs e)
        {
            var tabItem = TabController.Items[TabController.SelectedIndex] as TabItem;
            var frame = tabItem.Content as Frame;
            var page = frame.Content as TextEditorPage;

            if (string.IsNullOrWhiteSpace(page.Path))
            {

                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Algoritimo |*.alg|Fluxograma |*.flux";
                saveFileDialog.ShowDialog();

                if (string.IsNullOrWhiteSpace(saveFileDialog.FileName))
                    return;
                page.Path = saveFileDialog.FileName;
            }

            tabItem.Header = new Uri(page.Path).Segments.Last();
            if (page.Path.EndsWith(".alg"))
                File.WriteAllText(page.Path, page.TextEditorBox.Text);
        }

        private void MenuItem_SaveAs_Click(object sender, RoutedEventArgs e)
        {
            var tabItem = TabController.Items[TabController.SelectedIndex] as TabItem;
            var frame = tabItem.Content as Frame;
            var page = frame.Content as TextEditorPage;

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Algoritimo |*.alg|Fluxograma |*.flux";
            saveFileDialog.ShowDialog();

            if (string.IsNullOrWhiteSpace(saveFileDialog.FileName))
                return;
            page.Path = saveFileDialog.FileName;

            tabItem.Header = new Uri(page.Path).Segments.Last();
            if (page.Path.EndsWith(".alg"))
                File.WriteAllText(page.Path, page.TextEditorBox.Text);
        }

        private void AddNewTextFileToEditor(string fileContent = "", string fileName = "", string filePath = "")
        {
            TabItem tab = new TabItem();
            Frame frame = new Frame();
            TextEditorPage page = new TextEditorPage();
            if(string.IsNullOrWhiteSpace(fileName))
                fileName = string.Format("Novo arquivo {0}", NewFileCount++);

            page.Path = filePath;
            tab.Header = fileName;
            page.TextEditorBox.Text = fileContent;
            frame.Content = page;
            tab.Content = frame;
            TabController.Items.Add(tab);
            TabController.SelectedIndex = TabController.Items.Count - 1;
        }

        private void AddNewFlowchartFileToEditor(string fileName = "", string filePath = "")
        {
            TabItem tab = new TabItem();
            Frame frame = new Frame();
            FlowchartEditorPage page = new FlowchartEditorPage();
            if (string.IsNullOrWhiteSpace(fileName))
                fileName = string.Format("Novo arquivo {0}", NewFileCount++);

            page.Path = filePath;
            tab.Header = fileName;

            frame.Content = page;
            tab.Content = frame;
            TabController.Items.Add(tab);
            TabController.SelectedIndex = TabController.Items.Count - 1;
        }
    }
}
