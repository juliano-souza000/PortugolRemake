using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Portugol_Remake.Lists
{
    public class UserConfiguriations
    {
        public Themes Theme { get; set; }

        [JsonIgnore]
        public ResourceDictionary ThemeDictionary { get; set; }
        
    }
    public class UserConfig
    {
        public static UserConfiguriations UserConfigurations { get; set; }
    }

    public enum Themes
    {
        Dark,
        Bright
    }

}
