using System;
using System.Configuration;
using System.Windows.Forms;

namespace SolEx.Hurt.Sync.App.Updater
{
    public partial class EdycjaKonfiguracji : Form
    {
        public EdycjaKonfiguracji()
        {
            InitializeComponent();
            tbklucz.Text = ConfigurationManager.AppSettings["api_token"];
            tburl.Text = ConfigurationManager.AppSettings["url"];

        }

        private void button1_Click(object sender, EventArgs e)
        {
            System.Configuration.Configuration config =
             ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings.Clear();
            // Add an Application Setting.
            config.AppSettings.Settings.Add("url", tburl.Text);
            config.AppSettings.Settings.Add("api_token", tbklucz.Text);
            // Save the configuration file.
            config.Save(ConfigurationSaveMode.Modified);

            // Force a reload of a changed section.
            ConfigurationManager.RefreshSection("appSettings");
            Close();
        }
    }
}
