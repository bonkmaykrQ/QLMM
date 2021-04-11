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
using System.Windows.Shapes;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Diagnostics;

namespace QLMM
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            //Show Window
            InitializeComponent();

            //Plant settings into fields
                this.GamePathBox.Text = (string)Variables.ConfigurationData["qlmm"]["GamePath"];
                this.ModsPathBox.Text = (string)Variables.ConfigurationData["qlmm"]["ModsPath"];
                DoWeSort.IsChecked = Variables.sortByEnabled;

            if (Variables.WasShitFound == false)
            {
                Error.Content = "QLMM failed to find Quake Live. Please locate it and paste it's location here.";
                //Error_Line2.Content = "If you are using this for another game, ignore this message.";
                Error_Line2.Content = "For other games, please read the QLMM documentation.";
                Error_Line3.Content = "Game Path should be the game's EXE. Mod Path should be your baseq3 folder.";
            }
            else {
                Error.Content = "";
                Error_Line2.Content = "";
                Error_Line3.Content = "Reverting to default settings will abruptly restart QLMM.";
            }

            //Show Copyright License
            license_full.AppendText("This program is free software: you can redistribute it and/or modify\nit under the terms of the GNU General Public License as published by\nthe Free Software Foundation, either version 3 of the License, or\n(at your option) any later version.\n\nThis program is distributed in the hope that it will be useful,\nbut WITHOUT ANY WARRANTY; without even the implied warranty of\nMERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the\nGNU General Public License for more details.\n\nYou should have received a copy of the GNU General Public License\nalong with this program.If not, see https://www.gnu.org/licenses/.\n");
        }

        //Clear Window fields
        private void SettingsClosed(object sender, EventArgs e)
        {
            //Clear Window fields

            //Set window variable back to null
            Variables.SettingsWindow = null;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            Variables.GamePath = GamePathBox.Text;
            Variables.ModsPath = ModsPathBox.Text;
            Variables.sortByEnabled = (bool)DoWeSort.IsChecked;

            if (Variables.ModsPath.EndsWith("\\") == false)
            {
                Variables.ModsPath = Variables.ModsPath + "\\";
            }

            //Write settings and store them in the main application
            JObject temp = JObject.FromObject(new
            {
                qlmm = new
                {
                    GamePath = Variables.GamePath,
                    ModsPath = Variables.ModsPath,
                    sortByEnabled = Variables.sortByEnabled
                }
            });

            Variables.ConfigurationData = temp;
            Directory.CreateDirectory(Variables.ConfigurationPath);
            File.WriteAllText(Variables.ConfigurationPath + "\\data.json", temp.ToString());

            //Close the settings window
            Window.GetWindow(SettingsWindow).Close();
            Variables.WasShitFound = true;
            Variables.QLMMWindow.SearchModsFolder(Variables.ModsPath);
        }

        private void RevertButton_Click(object sender, RoutedEventArgs e)
        {
            File.Delete(Variables.ConfigurationPath + "\\data.json");
            File.Delete(Variables.ConfigurationPath + "\\data_oldquake.json");
            File.Delete(Variables.ConfigurationPath + "\\data_warfork.json");
            Application.Current.Shutdown();
        }

        private void goToBMWebsite(object sender, MouseButtonEventArgs e)
        {
            Process.Start("https://bonkmaykrq.github.io");
        }

        private void openGBPage(object sender, MouseButtonEventArgs e)
        {
            Process.Start("https://gamebanana.com/tools/6969");
        }

        private void openGHpage(object sender, MouseButtonEventArgs e)
        {
            Process.Start("https://github.com/bonkmaykrQ/QLMM");
        }
    }
}
