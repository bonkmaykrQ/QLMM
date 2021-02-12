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
using System.IO;

namespace QLMM
{
    /// <summary>
    /// Interaction logic for Alert.xaml
    /// </summary>
    public partial class Alert : Window
    {
        public Window ass;
        //Type setAction;

        /// <summary>
        /// Creates an alert box for QLMM.
        /// </summary>
        /// <param name="message">The message that should appear.</param>
        /// <param name="yesOrNo">Whether or not there should be a "no" option.</param>
        public Alert(Window baseWindow, string message, string filesToDelete, bool yesOrNo, string okMessage = "OK", string noMessage = "Cancel")
        {
            InitializeComponent();
            //Owner = Variables.QLMMWindow;
            ass = baseWindow;

            if (yesOrNo == false)
            {
                CancelButton.Content = null;
                CancelButton.IsEnabled = false;
                CancelButton.Width = 0;
                CancelButton.Height = 0;

                OKButton.Margin = new Thickness(10, 0, 10, 10);
                //this.WindowStyle = WindowStyle.ToolWindow;
            }

            AlertBoxMessage.Text = AlertBoxMessage.Text + " " + Variables.QLMMWindow.DeletingThisShorthand + "?";
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            File.Delete(Variables.QLMMWindow.DeletingThis);
            Variables.QLMMWindow.SearchModsFolder((string)Variables.ConfigurationData["qlmm"]["ModsPath"]);
            Close();
        }
    }
}
