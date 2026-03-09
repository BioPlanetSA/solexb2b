using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using log4net.Config;

namespace SolEx.Hurt.Sync.App.Updater
{
    static class Program
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public static bool DzialajWTle;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]

        static void Main()
        {
            XmlConfigurator.Configure();
            if (Environment.GetCommandLineArgs().Any(a => a.ToLower() == "-auto"))
            {
                DzialajWTle = true;
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Form1 forma = new Form1();
            if (CheckAppIsRunning())
            {
                string kom = "Wyłączam aplikację, jest już uruchomiona";
                Log.Error(kom);
                if (!DzialajWTle)
                {
                    MessageBox.Show(kom);
                }
                return;
            }
            if (DzialajWTle)
            {
                forma.UruchomUpdate();
            }
            else
            {
                Application.Run(forma);
            }            
        }

        public static string SciezkaKonfiguracji()
        {
            return  AppDomain.CurrentDomain.BaseDirectory+  "customAppSettings.config";
        }
        private static bool CheckAppIsRunning()
        {
            if (Environment.GetCommandLineArgs().Any(p => p.ToLower() == "-force"))
                return false;
            //ograniczenie tylko jednego uruchomienia
            Process proc = Process.GetCurrentProcess();

            try
            {
                //wyciągamy tylko procesy które nie są w tym samym katalogu niż uruchomiona aplikacja i mają różne id
                //dzięki temu na jednym komputerze klient może miec kilka synchronizatorów ale każdego z nich nie uruchomi dwa razy
                Process[] processes = Process.GetProcessesByName(proc.ProcessName).Where(a => a.MainModule.FileName == proc.MainModule.FileName).ToArray();
                if (processes.Length > 1)
                {
                    //jeśli program nie działa w trybie auto i użytkownik wybierze killowanie innych procesów to sprawdzamy czy ID procesów o tej nazwie jest różny od ID aktualnego procesu
                    return true;
                }
            }
            catch
            {
                //jeśli wykonywanie kodu trafi tutaj tzn że są uruchomione aplikacje x64 a aktualna jest x86 i nie może uzyskać do nich dostępu
                //używanie WMI rozwiązuje ten problem bo tam można się dobrać do ścieżki aplikacji
                const string wmiQueryString = "SELECT ProcessId, ExecutablePath, CommandLine FROM Win32_Process";
                using (var searcher = new ManagementObjectSearcher(wmiQueryString))
                using (var results = searcher.Get())
                {
                    var query = from p in Process.GetProcesses()
                                join mo in results.Cast<ManagementObject>()
                                on p.Id equals (int)(uint)mo["ProcessId"]
                                select new
                                {
                                    // Process = p,
                                    Path = (string)mo["ExecutablePath"],
                                    //  CommandLine = (string)mo["CommandLine"],
                                };

                    var wynik = query.Count(a => !string.IsNullOrEmpty(a.Path) && a.Path.EndsWith(proc.MainModule.FileName));
                    if (wynik > 1)
                        return true;
                }
            }
            return false;
        }
    }
}
