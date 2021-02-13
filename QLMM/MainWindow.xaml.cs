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
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json.Linq;
using ICSharpCode.SharpZipLib.Zip;
using System.Windows.Forms;

namespace QLMM
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //public Process GameProcess;
        //public Window1 SettingsWindow;
        public bool AlertResponse;
        string SelectedModPath;
        public string DeletingThis;
        public string DeletingThisShorthand;
        public ItemCollection listData;

        OpenFileDialog explorer;

        /// <summary>
        /// The base window for QLMM.
        /// <para>It contains all of the neccessary functions for the program to operate.</para>
        /// <para>IO operations, game executable startup, and more are all handled here.</para>
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            Variables.QLMMWindow = this;
            SelectionDetails.AppendText("QLMM by bonkmaykr\nSelect a mod from the list to view its information, or add a mod to the list using the \"Import Mod\" button below.");
            listData = ModsList.Items;

            SteamStartButton.Click += new RoutedEventHandler(StartSteam);
            GameStartButton.Click += new RoutedEventHandler(StartGame);
            ProgramOptionsButton.Click += new RoutedEventHandler(OpenSettings);


            // JSON Config Loader
            if (File.Exists(Variables.ConfigurationPath + "\\data.json") == true)
            {
                string loadedjson = File.ReadAllText(Variables.ConfigurationPath + "\\data.json");
                Variables.ConfigurationData = JObject.Parse(loadedjson);

                if (Variables.ConfigurationData["qlmm"]["sortByEnabled"] != null) {
                    Variables.sortByEnabled = (bool)Variables.ConfigurationData["qlmm"]["sortByEnabled"];
                } else
                {
                    Variables.ConfigurationData["qlmm"]["sortByEnabled"] = false;
                    Variables.sortByEnabled = (bool)Variables.ConfigurationData["qlmm"]["sortByEnabled"];
                }

                SearchModsFolder((string)Variables.ConfigurationData["qlmm"]["ModsPath"]);
            }
            else {
                // Create a configuration file for first-time program launch.
                if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + "\\Steam\\steamapps\\common\\Quake Live\\quakelive_steam.exe") == true)
                {
                    JObject temp = JObject.FromObject(new
                    {
                        qlmm = new
                        {
                            GamePath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + "\\Steam\\steamapps\\common\\Quake Live\\quakelive_steam.exe",
                            ModsPath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + "\\Steam\\steamapps\\common\\Quake Live\\baseq3\\",
                            sortByEnabled = false
                        }
                    });
                    Variables.ConfigurationData = temp;
                    Directory.CreateDirectory(Variables.ConfigurationPath);
                    File.WriteAllText(Variables.ConfigurationPath + "\\data.json", temp.ToString());
                    SearchModsFolder((string)Variables.ConfigurationData["qlmm"]["ModsPath"]);
                }
                else {
                    Variables.WasShitFound = false;
                    OpenSettings();
                }
            }

            //MainWindow1.Closed += new RoutedEventHandler(Shutdown);
        }

        private void CurrentAlertBox_Closed(object sender, EventArgs e)
        {
            if (AlertResponse == true) {
                AlertResponse = false;
                File.Delete(DeletingThis);
                DeletingThis = null;
                SearchModsFolder((string)Variables.ConfigurationData["qlmm"]["ModsPath"]);
            }
        }

        // List of blacklisted preinstalled pk3s:
        // - pak00.pk3
        // - bin.pk3
        // - curry.pk3
        // - common-spog.pk3
        // - common-q3map2.pk3
        public void SearchModsFolder(string path) {
            Variables.ModListingData = new List<ModDefinition>();   // Reset current modlist data.
            ModsList.Items.Clear();                                 //
            int numberFailed2Parse = 0;                             // if this number is EVER above 0 then someone did a oopsie!

            try {
                string[] enabledfoldercontents = Directory.GetFiles(path, "*.pk3", SearchOption.TopDirectoryOnly);
                string[] disabledfoldercontents = Directory.GetFiles(path, "*.pk3.disabled", SearchOption.TopDirectoryOnly);
                List<string> foldercontents = new List<string>();

                foreach (string sortThis in enabledfoldercontents) {
                    foldercontents.Add(sortThis);
                }
                foreach (string sortThis in disabledfoldercontents)
                {
                    foldercontents.Add(sortThis);
                }
                if (Variables.sortByEnabled == false) { foldercontents.Sort(); }

                foreach (string currentpak in foldercontents) {
                    ModDefinition currentMod = new ModDefinition(currentpak);

                    int shit1 = currentpak.LastIndexOf("\\");       // file name
                    string name = currentpak.Substring(shit1);      //
                    if (currentpak.EndsWith(".disabled")) {         //
                        currentMod.IsDisabled = true;               //
                    }                                               //

                    if (name.StartsWith("\\")) {
                        name = name.Substring(1);
                    }
                    if (name.EndsWith(".disabled"))
                    {
                        name = name.Remove(name.LastIndexOf(".disabled"));
                    }

                    FileStream fileStream = new FileStream(currentpak, FileMode.Open, FileAccess.ReadWrite);
                    ZipFile zipFile = new ZipFile(fileStream);

                    //int metaLocation = zipFile.FindEntry("_meta", false);
                    // condition used to be metaLocation == -1
                    if (zipFile.ZipFileComment == "" || zipFile.ZipFileComment == null)
                    {
                        currentMod.name = name;
                        currentMod.author = "<unknown>";
                        currentMod.version = "<unknown>";
                        currentMod.description = "";
                    }
                    else
                    {
                        try
                        {
                            JObject jo = JObject.Parse(zipFile.ZipFileComment);
                            currentMod.name = (string)jo["qlmm"]["name"];
                            currentMod.author = (string)jo["qlmm"]["author"];
                            currentMod.version = (string)jo["qlmm"]["version"];
                            currentMod.description = (string)jo["qlmm"]["description"];
                        }
                        catch (Newtonsoft.Json.JsonReaderException bruhMoment) {
                            numberFailed2Parse++;
                            currentMod.name = name;
                            currentMod.author = "<unknown>";
                            currentMod.version = "<unknown>";
                            currentMod.description = "";
                        }
                    }

                    if (currentMod.author == "" || currentMod.author == null)
                    {
                        currentMod.author = "<unknown>";
                    }
                    if (currentMod.version == "" || currentMod.version == null)
                    {
                        currentMod.version = "<unknown>";
                    }

                    // Filter out pre-installed pak files
                    if (currentMod.name != "pak00.pk3" &&
                        currentMod.name != "curry.pk3" &&
                        currentMod.name != "bin.pk3" &&
                        currentMod.name != "common-spog.pk3" &&
                        currentMod.name != "common-q3map2.pk3")
                    {
                        Variables.ModListingData.Add(currentMod);
                    }

                    zipFile.Close();
                    fileStream.Close(); // this is important in order to prevent crashing
                }

                SelectionDetails.Document = new FlowDocument();
                SelectionDetails.AppendText("QLMM by bonkmaykr\nSelect a mod from the list to view its information, or add a mod to the list using the \"Import Mod\" button below.");

                int numberOfDisabled = 0;
                foreach (ModDefinition checkThis in Variables.ModListingData)
                {
                    if (checkThis.IsDisabled == true)
                    {
                        numberOfDisabled++;
                    }
                }

                SelectionDetails.AppendText("\n\n" + Variables.ModListingData.Count + " packages discovered\n" + numberOfDisabled + " of them are disabled");

                if (numberFailed2Parse > 0)
                {
                    SelectionDetails.AppendText("\nFailed to parse the metadata of " + numberFailed2Parse + " or more packages, are they corrupted?");
                }

            } catch (DirectoryNotFoundException YouFuckingDumbass) {
                SelectionDetails.Document = new FlowDocument();
                SelectionDetails.AppendText("We failed to find any mod files due to a fatal error, sorry! Please make sure you typed the correct paths in your settings. Here's the error:\n\n" + YouFuckingDumbass.Message);
            } catch (FileNotFoundException YouFuckingDumbass) {
                SelectionDetails.Document = new FlowDocument();
                SelectionDetails.AppendText("We failed to find any mod files due to a fatal error, sorry! Please make sure you typed the correct paths in your settings. Here's the error:\n\n" + YouFuckingDumbass.Message);
            } catch (ArgumentException YouFuckingDumbass)
            {
                SelectionDetails.Document = new FlowDocument();
                SelectionDetails.AppendText("We failed to find any mod files due to a fatal error, sorry! Please make sure you typed the correct paths in your settings. Here's the error:\n\n" + YouFuckingDumbass.Message);
            }

            foreach (ModDefinition currentMod in Variables.ModListingData) {
                string isEnabledIndicator = "";

                // maybe allow the user to disable this?
                if (currentMod.IsDisabled == true) {isEnabledIndicator = " (Disabled)";}// else {isEnabledIndicator = " (Enabled)";}

                ListBoxItem currentListItem = new ListBoxItem();
                currentListItem.Content = currentMod.name + isEnabledIndicator;
                ModsList.Items.Add(currentListItem);

            }
        }

        private void Shutdown(object sender, EventArgs e)
        {
            if (Variables.SettingsWindow != null) { Variables.SettingsWindow.Close(); }
        }

        private void StartSteam(object sender, RoutedEventArgs e)
        {
            /*GameProcess = */
            Process.Start("steam://run/282440");
            //Window.GetWindow(MainWindow1).Hide();
            System.Windows.Application.Current.Shutdown();

            //GameProcess.WaitForExit();
            //Window.GetWindow(MainWindow1).Show();
        }

        private void StartGame(object sender, RoutedEventArgs e)
        {
            if (File.Exists((string)Variables.ConfigurationData["qlmm"]["GamePath"]))
            {
                // The reason for all of this mess below is that without setting the working directory,
                // we face the risk of Quake trying to search for it's assets in the Mod Manager's
                // location instead. So unless the user puts the mod manager inside of the game's folder,
                // then the game will most likely crash.
                //
                // Additionally, we don't currently have the actual working directory, and we don't want
                // to make the user type the same path FUCKING TWICE just so our dumb little program can
                // find it, so of course we do some math.

                Process temp = new Process();
                int shit1 = ((string)Variables.ConfigurationData["qlmm"]["GamePath"]).LastIndexOf("\\");
                int shit2 = ((string)Variables.ConfigurationData["qlmm"]["GamePath"]).Length; // this honestly isn't even needed
                temp.StartInfo.FileName = (string)Variables.ConfigurationData["qlmm"]["GamePath"];
                temp.StartInfo.WorkingDirectory = ((string)Variables.ConfigurationData["qlmm"]["GamePath"]).Substring(0, (shit1));
                temp.Start();
                System.Windows.Application.Current.Shutdown();
            }
            else {
                Variables.WasShitFound = false;
                OpenSettings();
            }

        }
        private void OpenSettings() //Non-event version
        {
            if (Variables.SettingsWindow == null)
            {
                Variables.SettingsWindow = new Window1();
                Variables.SettingsWindow.Owner = GetWindow(this);
                Variables.SettingsWindow.Show();
            }
        }
        private void OpenSettings(object sender, RoutedEventArgs e) //Event version
        {
            if (Variables.SettingsWindow == null) {
                Variables.WasShitFound = true; // if the options button is clicked we reset this variable
                Variables.SettingsWindow = new Window1();
                Variables.SettingsWindow.Owner = GetWindow(this);
                Variables.SettingsWindow.Show();
            }
        }

        /*private void ProcessExited()
        {
            Window.GetWindow(MainWindow1).Show();
        }*/

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((System.Windows.Controls.ListBox)sender).SelectedIndex >= 0) {
                DeleteModFileButton.IsEnabled = true;
                ToggleModButton.IsEnabled = true;
                SelectedModPath = Variables.ModListingData[((System.Windows.Controls.ListBox)sender).SelectedIndex].path;
                DeletingThisShorthand = Variables.ModListingData[ModsList.SelectedIndex].name;

                string creationDate = File.GetCreationTime(Variables.ModListingData[((System.Windows.Controls.ListBox)sender).SelectedIndex].path).ToString();
                string modificationDate = File.GetLastWriteTime(Variables.ModListingData[((System.Windows.Controls.ListBox)sender).SelectedIndex].path).ToString();

                SelectionDetails.Document = new FlowDocument();
                SelectionDetails.AppendText("QLMM by bonkmaykr\nSelect a mod from the list to view its information, or add a mod to the list using the \"Import Mod\" button below.");
                SelectionDetails.AppendText("\n\nName: " + Variables.ModListingData[((System.Windows.Controls.ListBox)sender).SelectedIndex].name);
                SelectionDetails.AppendText("\nAuthor: " + Variables.ModListingData[((System.Windows.Controls.ListBox)sender).SelectedIndex].author);
                SelectionDetails.AppendText("\nVersion: " + Variables.ModListingData[((System.Windows.Controls.ListBox)sender).SelectedIndex].version);
                SelectionDetails.AppendText("\n\nLast Modified at " + creationDate);
                SelectionDetails.AppendText("\nFirst Created at " + modificationDate);
                SelectionDetails.AppendText("\n\n" + Variables.ModListingData[((System.Windows.Controls.ListBox)sender).SelectedIndex].description);

                if (Variables.ModListingData[((System.Windows.Controls.ListBox)sender).SelectedIndex].IsDisabled == false) {
                    SelectionDetails.AppendText("\n\nThis mod is currently enabled.");
                } else
                {
                    SelectionDetails.AppendText("\n\nThis mod is currently disabled.");
                }
            } else
            {
                DeleteModFileButton.IsEnabled = false;
                ToggleModButton.IsEnabled = false;
                DeletingThisShorthand = "";
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SelectionDetails.Document = new FlowDocument();
            SelectionDetails.AppendText("The button works!");
        }

        private void OpenModsFolderButton_Click(object sender, RoutedEventArgs e)
        {
            string temppath = (string)Variables.ConfigurationData["qlmm"]["ModsPath"];  // Prevent any screwups that may happen
            if (temppath.EndsWith("\\"))                                                // if the user's file path ends in a backslash,
            {                                                                           // because Explorer is really stupid and won't
                temppath = temppath.Remove(temppath.LastIndexOf("\\"));                 // open the correct path if it does.
            }

            try
            {
                Process.Start("explorer.exe", temppath);
            }
            catch (FileNotFoundException YouFuckingDumbass)
            {
                SelectionDetails.Document = new FlowDocument();
                SelectionDetails.AppendText("Microsoft Explorer did something stupid, sorry! Here's the error:\n\n" + YouFuckingDumbass.Message);
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            Alert currentAlertBox = new Alert(this, "test", SelectedModPath, true);
            currentAlertBox.Show();
            AlertResponse = false;
            currentAlertBox.Closed += CurrentAlertBox_Closed;
            DeletingThis = SelectedModPath;
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            SearchModsFolder((string)Variables.ConfigurationData["qlmm"]["ModsPath"]);
        }

        private void ImportModButton_Click(object sender, RoutedEventArgs e)
        {
            explorer = new OpenFileDialog();
            explorer.Multiselect = false;
            explorer.Filter = "idTech3 Game Files (*.pk3)|*.pk3|ZIP Archives (*.zip)|*.zip";
            //explorer.ShowDialog();
            //explorer.FileOk += Explorer_FileOk;

            if (explorer.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Random rng = new Random();
                int rng_result = rng.Next();

                string oldName = explorer.SafeFileName;
                bool shouldWeCreateJSON = false;
                try
                {
                    FileStream stream = new FileStream(explorer.FileName, FileMode.Open);
                    ZipFile zipStream = new ZipFile(stream);
                    //JObject parse = new JObject();

                    if (zipStream.ZipFileComment == "" || zipStream.ZipFileComment == null)
                    {
                        shouldWeCreateJSON = true;
                    }

                    zipStream.Close();
                    stream.Close();
                } catch (Exception bruhMoment)
                {
                    string doNothingLol = bruhMoment.Message;
                }

                if (File.Exists((string)Variables.ConfigurationData["qlmm"]["ModsPath"] + "pak01_" + rng_result + ".pk3") == false)
                {
                    File.Copy(explorer.FileName, (string)Variables.ConfigurationData["qlmm"]["ModsPath"] + "pak01_" + rng_result + ".pk3");
                }

                if (shouldWeCreateJSON == true) {
                    FileStream addToThis = new FileStream((string)Variables.ConfigurationData["qlmm"]["ModsPath"] + "pak01_" + rng_result + ".pk3", FileMode.OpenOrCreate);
                    ZipFile add2zip = new ZipFile(addToThis);
                    JObject newOutput = JObject.FromObject(new
                    {
                        qlmm = new
                        {
                            name = oldName
                        }
                    });

                    add2zip.BeginUpdate();
                    add2zip.SetComment(newOutput.ToString());
                    add2zip.CommitUpdate();

                    add2zip.Close();
                    addToThis.Close();
                }

                SearchModsFolder((string)Variables.ConfigurationData["qlmm"]["ModsPath"]);
            }
        }

        private void Explorer_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Random rng = new Random();
            int rng_result = rng.Next();
            if (File.Exists((string)Variables.ConfigurationData["qlmm"]["ModsPath"] + "pak01_" + rng_result + ".pk3") == false)
            {
                //File.Copy(explorer.FileName, (string)Variables.ConfigurationData["qlmm"]["ModsPath"] + "pak01_" + rng_result + ".pk3");
                //explorer.OpenFile().CopyTo(new FileStream((string)Variables.ConfigurationData["qlmm"]["ModsPath"] + "pak01_" + rng_result + ".pk3", FileMode.Create));

            }

            SearchModsFolder((string)Variables.ConfigurationData["qlmm"]["ModsPath"]);
        }

        private void EnableDisableButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedModPath.EndsWith(".disabled") != true)
            {
                File.Move(SelectedModPath, SelectedModPath + ".disabled");
            }
            else {
                string newName = SelectedModPath;
                int disabledExtLocation;
                disabledExtLocation = newName.LastIndexOf(".");
                newName = newName.Remove(disabledExtLocation);

                if (disabledExtLocation >= 0) { File.Move(SelectedModPath, newName); }
            }

            SearchModsFolder((string)Variables.ConfigurationData["qlmm"]["ModsPath"]);
        }
    }
}
