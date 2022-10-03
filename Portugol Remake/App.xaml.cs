using Newtonsoft.Json;
using Portugol_Remake.Lists;
using Portugol_Remake.Resources.Cursor;
using Portugol_Remake.Windows;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Portugol_Remake
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            //CustomCursor.No = new System.Windows.Input.Cursor(System.Windows.Application.GetResourceStream(new Uri("pack://application:,,,/Resources/Cursor/CursorNo.cur")).Stream, true);
            
            if (!File.Exists(@"Config/config.json"))
            {
                MainWindow mainView = new MainWindow();
                UserConfig.UserConfigurations = (UserConfiguriations)JsonConvert.DeserializeObject(File.ReadAllText(@"Config/config.jon"));
                switch(UserConfig.UserConfigurations.Theme)
                {
                    case Themes.Dark:
                        UserConfig.UserConfigurations.ThemeDictionary = Current.Resources.MergedDictionaries.Where(x => x.Source.OriginalString == "Resources/ResourceDictionaryDarkTheme.xaml").FirstOrDefault();
                        break;
                    case Themes.Bright:
                        UserConfig.UserConfigurations.ThemeDictionary = Current.Resources.MergedDictionaries.Where(x => x.Source.OriginalString == "Resources/ResourceDictionaryBrightTheme.xaml").FirstOrDefault();
                        break;
                }
                mainView.Show();
            }
            else
            {
                UserConfig.UserConfigurations = new UserConfiguriations();
                UserConfig.UserConfigurations.Theme = Themes.Dark;
                UserConfig.UserConfigurations.ThemeDictionary = Current.Resources.MergedDictionaries.Where(x => x.Source.OriginalString == "Resources/ResourceDictionaryDarkTheme.xaml").FirstOrDefault();

                if (!Directory.Exists(@"Config"))
                    Directory.CreateDirectory("Config");
                File.WriteAllText(@"Config/config.json", JsonConvert.SerializeObject(UserConfig.UserConfigurations));
                //ThemeChooser themeChooser = new ThemeChooser();
                //themeChooser.Show();
                EditorWindow textEditorWindow = new EditorWindow();
                textEditorWindow.Show();
            }
        }
    }
}
