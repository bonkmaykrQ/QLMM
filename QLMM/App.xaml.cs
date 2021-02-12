using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json.Linq;


namespace QLMM
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>

    public partial class App : Application
    {

    }

    /// <summary>
    /// Program-wide Global Variables
    /// </summary>
    public static class Variables {

        /// <summary>
        /// Contains the data to be displayed inside of the main window's list view.
        /// </summary>
        public static List<ModDefinition> ModListingData = new List<ModDefinition>();
        //public static ModDefinition[] ModListingData = new ModDefinition[0];
        /// <summary>
        /// The path to Quake's exectuable.
        /// </summary>
        public static string GamePath = "";
        /// <summary>
        /// The path to the user's assets folder. It is usually named "baseq3".
        /// </summary>
        public static string ModsPath = "";
        /// <summary>
        /// The variable that contains the settings dialog.
        /// </summary>
        public static Window1 SettingsWindow = null;
        /// <summary>
        /// The path to the program's data folder.
        /// </summary>
        public static string ConfigurationPath = 
        (Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\bonkmaykr\\QLMM");
        /// <summary>
        /// Settings file contents
        /// </summary>
        public static JObject ConfigurationData = null;
        /// <summary>
        /// Tells the Settings menu wether or not the game files were able to be automatically discovered.
        /// <para>See: <seealso cref="Window1.Window1"/></para>
        /// </summary>
        public static bool WasShitFound = true;
        /// <summary>
        /// Variable that holds the Main Window for other windows to refer to.
        /// </summary>
        public static MainWindow QLMMWindow;
    }
}
