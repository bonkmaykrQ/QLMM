using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using GamebananaApi;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using GalaSoft.MvvmLight.Threading;
using NegativeLayer.Extensions;
using Modboy;
using System.Net;
//using Network;
using System.Net.Http;
using EasyHttp.Http;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using ICSharpCode.SharpZipLib.Zip;

namespace QLMMbackend
{
    class Program
    {
        static void Main(string[] args)
        {
            //FuckingFuckFuckDipshit.inputVars = args[1];

            try
            {
                FuckingFuckFuckDipshit.ParseArguments(args);

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(FuckingFuckFuckDipshit.main = new Alert());
            } catch (Exception e)
            {
                MessageBox.Show("An error has occurred!\n\n" + e.Message);
                Process.GetCurrentProcess().Kill();
            }
        }
    }

    public static class FuckingFuckFuckDipshit
    {
        public static Alert main;

        public static string inputVars;
        public static string httpresponse;
        public static string outerFilePath;
        public static string outerFileName;
        public static string modDisplayName;

        /// <summary>
        /// Parse the command line arguments and take appropriate actions. Stolen from Modboy
        /// </summary>
        public static void ParseArguments(string[] args)
        {
            if (!args.AnySafe()) return;

            bool launchedViaProtocolHandle = args[0].ContainsInvariant(Constants.ProtocolHandle);

            // Launched via protocol handle - parse task
            if (launchedViaProtocolHandle)
            {
                // find example at: https://dev.gamebanana.com/skins/150504?modboy
                // ex: modboy://Skin,150504,363769
                // Extract the input from the arguments
                var regex = new Regex(Constants.ProtocolHandle + "([A-Za-z]+),([0-9]+),([0-9]+)",
                    RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
                if (!regex.IsMatch(args[0]))
                {
                    MessageBox.Show("QLMM failed to process the URL. Sorry!");
                    return;
                }
                var match = regex.Match(args[0]);

                // Get the valuable data
                var subType = match.Groups[1].Value;
                var subId = match.Groups[2].Value;
                var fileId = match.Groups[3].Value;

                string httptarget = "https://gamebanana.com/apiv3/" + subType + "/" + subId;
                Uri uritarget = new Uri(httptarget);
                EasyHttp.Http.HttpClient client = new EasyHttp.Http.HttpClient();
                httpresponse = client.Get(httptarget).RawText;

                JObject apiJSON = JObject.Parse(httpresponse);

                try
                {
                    string modName = (string)apiJSON[""]["_sName"]; // Causes NullReferenceException, please fix.
                    string modUrl = "https://gamebanana.com/dl/" + fileId;
                    //string modfileName = (string)apiJSON["_aFiles"]["_sFile"];

                    // Create directory if necessary
                    if (!Directory.Exists(FileSystem.TempStorageDirectory))
                        Directory.CreateDirectory(FileSystem.TempStorageDirectory);

                    WebClient fileClient = new WebClient();
                    fileClient.DownloadFileCompleted += new AsyncCompletedEventHandler(Completed);
                    fileClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(ProgressChanged);
                    fileClient.DownloadFileAsync(new Uri(modUrl), Path.Combine(FileSystem.TempStorageDirectory, fileId));

                    outerFilePath = Path.Combine(FileSystem.TempStorageDirectory, fileId);
                    outerFileName = fileId;
                    modDisplayName = modName;

                    main.label1.Text = "Downloading " + modName + "...";
                }
                catch (Exception e) {
                    MessageBox.Show("An error has occurred!\n\n" + e.Message + "\n\n" + e.StackTrace);
                    Process.GetCurrentProcess().Kill();
                }

                
            }
        }

        public static void ProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            main.progressBar1.Value = e.ProgressPercentage;
        }

        public static void Completed(object sender, AsyncCompletedEventArgs e)
        {
            if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\bonkmaykr\\QLMM" + "\\data.json") == true)
            {
                string loadedjson = File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\bonkmaykr\\QLMM" + "\\data.json");
                string splooge = (string)JObject.Parse(loadedjson)["qlmm"]["ModsPath"];

                Random rng = new Random();
                int rng_result = rng.Next();

                //string oldName = explorer.SafeFileName;
                try
                {
                    FileStream stream = new FileStream(outerFilePath, FileMode.Open);
                    ZipFile zipStream = new ZipFile(stream);
                    //JObject parse = new JObject();

                    if (File.Exists(splooge + "pak01_" + outerFileName + rng_result + ".pk3") == false)
                    {
                        File.Copy(outerFilePath, splooge + "pak01_" + outerFileName + rng_result + ".pk3");
                    }
                    else
                    {
                        MessageBox.Show("ayy yo the pizza here");
                        Process.GetCurrentProcess().Close();
                    }

                    zipStream.Close();
                    stream.Close();

                    File.Delete(outerFilePath);
                }
                catch (Exception) {}
            }
            else
            {
                // Create a configuration file for first-time program launch.
                if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + "\\Steam\\steamapps\\common\\Quake Live\\quakelive_steam.exe") == true)
                {
                    string splooge = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + "\\Steam\\steamapps\\common\\Quake Live\\baseq3\\";

                    Random rng = new Random();
                    int rng_result = rng.Next();

                    //string oldName = explorer.SafeFileName;
                    try
                    {
                        FileStream stream = new FileStream(outerFilePath, FileMode.Open);
                        ZipFile zipStream = new ZipFile(stream);
                        //JObject parse = new JObject();

                        if (File.Exists(splooge + "pak01_" + outerFileName + rng_result + ".pk3") == false)
                        {
                            File.Copy(outerFilePath, splooge + "pak01_" + outerFileName + rng_result + ".pk3");
                        }
                        else
                        {
                            MessageBox.Show("ayy yo the pizza here");
                            Process.GetCurrentProcess().Close();
                        }

                        zipStream.Close();
                        stream.Close();

                        File.Delete(outerFilePath);
                    }
                    catch (Exception) { }
                }
                else
                {
                    MessageBox.Show("Failed to find installation directory.\n\nPlease open your QLMM settings and check the location of your baseq3 folder.");
                    Process.GetCurrentProcess().Close();
                }
            }

            MessageBox.Show("Download Complete.");
            Process.GetCurrentProcess().Close();
        }
    }
}
