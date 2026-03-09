using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using System.Threading;
using System.Reflection;
using System.Diagnostics;
using System.Management;
using SolEx.Hurt.Helpers;
using log4net.Config;
using System.IO;
using ServiceStack.Text;
using SolEx.Hurt.Core.Sync;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Helpers;


namespace SolEx.Hurt.Sync.App
{
    public static class Program
    {
        private static IAPIWywolania ApiWywolanie = APIWywolania.PobierzInstancje;
        public static List<Zadanie> ZadaniaDoUruchomienia;
        public static List<SyncModul> ModulyDoUruchomienia;
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public static bool DzialajWTle;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            //Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            //    AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            //    Application.ThreadException += Application_ThreadException;
 
            XmlConfigurator.Configure();
            Log.Info( "############### Start aplikacji ######################### ");

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
      
            AppDomain.CurrentDomain.AssemblyResolve += (currentDomain_AssemblyResolve);
            if (Environment.GetCommandLineArgs().Any(a => a.ToLower() == "-auto"))
            {
                DzialajWTle = true;
            }

            //spradzenie czy jest w ogole config
            if (!DzialajWTle && string.IsNullOrEmpty(ApiWywolanie.URL))
            {
                throw new Exception("Brak adresu WWW do strony - brak konfiguracji");
            }


            if (!CheckAppIsRunning())
            {
                //   Thread.CurrentThread.CurrentCulture =  CultureInfo.InvariantCulture;
                try
                {
                    UstawZadaniaDoUruchomienia();
                    if (!DzialajWTle && ZadaniaDoUruchomienia.Count == 0)
                    {
                        MessageBox.Show(@"Nie ma nic do uruchomienia");
                        return;
                    }
                  MainForm m  = new MainForm();
                m.InicjalizacjaFormatki();

                    Application.Run(m);
                }
                catch (Exception ex)
                {
                    string informacjaBledu = "Wystąpił błąd:" + ex.Message;
                    Log.Error(new Exception(informacjaBledu, ex));
                    if (!DzialajWTle)
                    {
                        MessageBox.Show(informacjaBledu + Environment.NewLine + ex.StackTrace);
                    }
                }
            }
            else
            {
                const string informacja = "Aplikacja była już uruchomiona";
                Log.Info(informacja);
                if (!DzialajWTle)
                {
                    MessageBox.Show(informacja);
                }
            }
            Environment.Exit(0);
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception exl = (Exception)e.ExceptionObject;
           Log.Error("Błąd CurrentDomain_UnhandledException");
            while (exl != null)
            {
                Log.Error(exl);
                exl = exl.InnerException;
            }
            Application.Exit();
        }

        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            Exception exl =e.Exception;
            Log.Error("Błąd Application_ThreadException");
            while (exl != null)
            {
                Log.Error(exl);
                exl = exl.InnerException;
            }
            Application.Exit();
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
                Process[] wszystkie = Process.GetProcessesByName(proc.ProcessName);

                var processes = wszystkie.Where(a => a.MainModule.FileName.Equals(proc.MainModule.FileName,StringComparison.InvariantCultureIgnoreCase)).ToArray();
                //    Log.InfoFormat("liczba znalezionych procesów {0} wszystkich {1}, aktualny path {2}", processes.Length, wszystkie.Length, proc.MainModule.FileName);
                //foreach (var p in wszystkie)
                //{
                //    Log.InfoFormat("wszystkie , mainmod path {0}",p.MainModule.FileName);
                //}
                if (processes.Length > 1)
                {
                    int ilosc = 0;
                    foreach (var process in processes)
                    {
                        if (process.HasExited || ((DateTime.Now - process.StartTime).Hours > 1))
                        {
                            Log.InfoFormat("Zamykamy proces synchronizatora jest zawieszony albo trwa zbyt długo.");
                            process.Kill();
                            continue;
                        }
                        ilosc++;
                        if (ilosc > 1)
                        {
                            //jeśli program nie działa w trybie auto i użytkownik wybierze killowanie innych procesów to sprawdzamy czy ID procesów o tej nazwie jest różny od ID aktualnego procesu
                            return true;
                        }
                    }
                }
            }
            catch
            {
                //jeśli wykonywanie kodu trafi tutaj tzn że są uruchomione aplikacje x64 a aktualna jest x86 i nie może uzyskać do nich dostępu
                //używanie WMI rozwiązuje ten problem bo tam można się dobrać do ścieżki aplikacji
                const string wmiQueryString = "SELECT ProcessId, ExecutablePath, CommandLine FROM Win32_Process";
                using (var searcher = new ManagementObjectSearcher(wmiQueryString))
                {
                    using (var results = searcher.Get())
                    {
                        var query = from p in Process.GetProcesses()
                            join mo in results.Cast<ManagementObject>()
                                on p.Id equals (int) (uint) mo["ProcessId"]
                            select new
                            {
                                // Process = p,
                                Path = (string) mo["ExecutablePath"],
                                //  CommandLine = (string)mo["CommandLine"],
                            };

                        var wynik = query.Count(a => !string.IsNullOrEmpty(a.Path) && a.Path.EndsWith(proc.MainModule.FileName));
                        Log.InfoFormat("sekcja wyjatku. liczba znalezionych procesów {0} ", wynik);
                        if (wynik > 1)
                            return true;
                    }
                }
            }
            return false;
        }
        static Assembly currentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            string katalog =  SyncManager.PobierzInstancje.Konfiguracja.KatalogProgramuKsiegowego;
            int idx = args.Name.IndexOf(",", StringComparison.Ordinal);
            if (idx == -1)
            {
                idx = args.Name.Length;
            }
            string plik = args.Name.Substring(0, idx);


            if (plik.EndsWith(".resources")) return null;
            plik += ".dll"; 
            if (string.IsNullOrEmpty(katalog))
            {
                return null;
            }
            
            string[] pliki = Directory.GetFiles(katalog,"*.*",SearchOption.AllDirectories);

            string sciezka = pliki.FirstOrDefault(x => x.EndsWith(plik));

            if (string.IsNullOrEmpty(sciezka))
            {
                sciezka = AppDomain.CurrentDomain.BaseDirectory + plik;
                if (!File.Exists(sciezka))
                {
                    throw new Exception(
                        $"Plik {plik} nie istnieje w ścieżce do systemu ERP (ustawienie - katalog_programu_ksiegowego): {SyncManager.PobierzInstancje.Konfiguracja.KatalogProgramuKsiegowego}. Sprawdz katalog systemu ERP czy jest dnbrze podany.");
                }
            }
            if (string.IsNullOrEmpty(sciezka))
            {
                throw new Exception("Plik nie istnieje: " + plik);
            }
            Log.DebugFormat($"Brak assembly: {args.Name}, ale będzie próba ładowania ze ścieżki: {sciezka}.");
            if (!System.IO.File.Exists(sciezka))
            {
                throw new Exception($"Brak pliku: {sciezka}");
            }
            Assembly asembylLadowane = null;
            try
            {
                asembylLadowane = Assembly.LoadFrom(sciezka);
            } catch (Exception e)
            {
                Log.DebugFormat($"Nie udało się załadować biblioteki: {args.Name} w ścieżce: {sciezka}. Sprawdz czy plik istnieje. Komunikat: {e.Message}." );
                Log.Error(e);
                throw;
            }

            return asembylLadowane;
        }

      
        private static void UstawZadaniaDoUruchomienia()
        {
            try
            {           
            var zadania = ApiWywolanie.PobierzZadaniaSynchronizatora();
               
            ZadaniaDoUruchomienia = new List<Zadanie>();
            ModulyDoUruchomienia = new List<SyncModul>();

                //sztuczne wywolnie czegokolwiek w 
                AppDomain.CurrentDomain.Load(AssemblyName.GetAssemblyName(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + "\\SolEx.Hurt.Sync.App.Modules_.dll"));
                Zadanie zadpdf = null;
             foreach (Zadanie t in zadania)
             {
                 if (t.TypZadaniaSynchronizacji != null)
                 {
                     if (t.TypZadaniaSynchronizacji == ElementySynchronizacji.ImportZamówień)
                     {
                         ZadaniaDoUruchomienia.Insert(0,t);
                     }
                     else if (t.TypZadaniaSynchronizacji == ElementySynchronizacji.WysylanieFakturPDF)
                     {
                         zadpdf = t;
                     }
                     else
                     {
                         ZadaniaDoUruchomienia.Add(t);
                     }
                   
                 } 
                 else
                 {
                     var mod = t.StworzKontrolke<SyncModul>();
                     mod.ZadanieBazowe = t;
                     if (mod.UruchamiacModul)
                     {
                         ModulyDoUruchomienia.Add(mod);
                     }
                 }

             }
                if (zadpdf != null)
                {
                    ZadaniaDoUruchomienia.Add(zadpdf);
                }

                Log.Info("Zadania do uruchomienia: " + ZadaniaDoUruchomienia.Count);
                Log.Info("Moduły do uruchomienia: " + ModulyDoUruchomienia.Count);
                Log.InfoFormat($"Spis licencji: {SyncManager.PobierzInstancje.Konfiguracja.Licenses.ToCsv()}");
            }
            catch (Exception e)
            {
                Log.Error("Błąd ustawiania modułów, prawdopodobnie wersja synchronizatora nie zgadza się z wersją platformy, błąd: " + e.Message);
                Log.Error(e);
                throw;
            }
        }

        public static void AktualizujZadanie(Zadanie z)
        {
            Zadanie tylkoCzasy = new Zadanie() {Id = z.Id, OstatnieUruchomienieStart = z.OstatnieUruchomienieStart, OstatnieUruchomienieKoniec = z.OstatnieUruchomienieKoniec};
            ApiWywolanie.AktualizujZadania(new List<Zadanie> { tylkoCzasy });
        }
    }
}
