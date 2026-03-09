using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model.Web;

namespace SolEx.Hurt.Sync.App.Updater
{
    public partial class Form1 : Form
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public Form1()
        {
            InitializeComponent();
            Timer timer1 = new Timer(); //Initialize a new Timer of name timer1
            timer1.Interval = (240000);
            timer1.Tick += new EventHandler(timer1_Tick); //Link the Tick event with timer1_Tick
            timer1.Start(); //Start the timer
            if(!KonfiguracjaPoprawna())
            {
                EdycjaKonfiguracji konfig = new EdycjaKonfiguracji();
                konfig.ShowDialog(this);
            }

            string sciezkaExe = Assembly.GetExecutingAssembly().Location;
            txtSciezkaHarmonogram.Text = sciezkaExe;

            lblAdres.Text = ConfigurationManager.AppSettings["url"];
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            Close();
        }
        private bool KonfiguracjaPoprawna()
        {
            if (!File.Exists(Program.SciezkaKonfiguracji()))
            {
                File.WriteAllText(Program.SciezkaKonfiguracji(), "<appSettings></appSettings>");
                ConfigurationManager.RefreshSection("appSettings");
                return false;
            }
            if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["api_token"]))
            {
                return false;
            }
            if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["url"]))
            {
                return false;
            }
            return true;
        }
        private void WylaczAplikacje()
        {
            if (Application.MessageLoop)
            {
                Application.Exit();
            }
            else
            {
                Environment.Exit(0);
            }
        }

        private void btnSprawdzAktualizacje_Click(object sender, EventArgs e)
        {
            btnSprawdzAktualizacje.Enabled = false;
            SprawdzIPobierzPliki();
            UruchomSynchornizator();
        }

        private void SprawdzIPobierzPliki()
        {
            string[] istniejacepliki = Directory.GetFiles(KatalogSync());
            List<string> dousuniecia=new List<string>(istniejacepliki);
            dousuniecia.RemoveAll(x => x.Contains("logi.txt"));
            dousuniecia.RemoveAll(x => x.Contains("SolEx.Hurt.Sync.App.exe.config"));

                List<DanePlik> nadysku = new List<DanePlik>();
                foreach (var p in istniejacepliki)
                {
                    string hash = Tools.PobierzInstancje.GetMd5Hash(File.ReadAllBytes(p));
                    string nazwa = Path.GetFileName(p);
                    DateTime mod = File.GetLastWriteTime(p);
                    DanePlik pl = new DanePlik(nazwa, mod, p,hash);
                    nadysku.Add(pl);
                }

            List<DanePlik> pliki;
            try
            {
                pliki = APIWywolania.PobierzInstancje.PobierzPlikiSynchronizatora().ToList();
            }
            catch (Exception ex)
            {
                if (!Program.DzialajWTle)
                {
                    MessageBox.Show(string.Format("Nie można pobrać plików z serwera. Błąd komunikacji z platformą. Wiadomość: {0} \r\n. Stack trace: {1}", ex.Message, ex.StackTrace));
                }
                return;
            }
            
            int i = 1;
                foreach (DanePlik p in pliki)
                {
                    dousuniecia.RemoveAll(x => x.Equals(SciezkaDoLokalnegoPliku(p), StringComparison.InvariantCultureIgnoreCase));
                    try
                    {
                        int postep = (int) (((float) i/(float) pliki.Count)*100);
                        bool aktualizowac = CzyAktualizowac(p, nadysku);
                        p.Status = "Aktualny";
                        if (aktualizowac)
                        {
                            p.Status = "W trakcie aktualizacji";
                        }
                        AktualizujStatus(postep, p);

                        if (aktualizowac)
                        {
                            log.DebugFormat("Aktualizowanie pliku synchronizatora: {0}. Data ostatenigo zapisu: {1}", p.Nazwa, p.DataModyfikacji);

                            ZapiszPlikNaDysku(p);

                            p.Status = "Zaktualizowany";
                            AktualizujStatus(postep, p);
                        }
                        i++;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(string.Format("Bład ładowania pliku: {0}. \r\n Sprawdź czy synchronizator nie jest uruchomiony.\r\n Błąd: {1}", p.Nazwa, ex.Message));
                    }
                }
            foreach (var p in dousuniecia)
            {
                File.Delete(p);
            }

        }

        private string SciezkaDoLokalnegoPliku(DanePlik p)
        {
            return Path.Combine(KatalogSync(), p.Nazwa);
        }
        private void ZapiszPlikNaDysku(DanePlik p)
        {
            string lokalny = SciezkaDoLokalnegoPliku(p);
            using (var webClient = new WebClient())
            {
                webClient.Encoding = Encoding.UTF8;
                webClient.DownloadFile(p.UrlPlik, lokalny);
            }
            string ext = Path.GetExtension(lokalny);
            if (!string.IsNullOrEmpty(ext) && ext.Equals(".txt", StringComparison.InvariantCultureIgnoreCase))
            {
                File.Copy(lokalny,lokalny.Replace(ext,".config"),true);
            }
        }

        private bool CzyAktualizowac(DanePlik plikwww, List<DanePlik> plikinadysku)
        {
            var p = plikinadysku.FirstOrDefault(x => x.Nazwa.Equals(plikwww.Nazwa,StringComparison.InvariantCultureIgnoreCase));

            if (p == null)
            {
                return true;
            }

            if (p.DataModyfikacji < plikwww.DataModyfikacji || !p.Hash.Equals(plikwww.Hash,StringComparison.InvariantCultureIgnoreCase))
            {
                return true;
            }

            return false;
        }
        public string KatalogSync()
        {
            string katalog = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Synchronizator");
            if (!Directory.Exists(katalog))
            {
                Directory.CreateDirectory(katalog);
            }
            return katalog;
        }

        private void UruchomSynchornizator()
        {
            string plikExe = Path.Combine(KatalogSync(), "Solex.hurt.sync.app.exe");

            if (!File.Exists(plikExe))
            {
                MessageBox.Show(string.Format("Brak synchronizatora do uruchomienia - brak pliku: {0}", plikExe), "Błąd", MessageBoxButtons.OK);
                return;
            }

            System.Diagnostics.Process.Start(plikExe, Program.DzialajWTle ? "-auto" : "");
            WylaczAplikacje();
        }


        private void AktualizujStatus(int wartoscProgresbar, DanePlik plik)
        {
            progresBar.Value = wartoscProgresbar;
            lblStatus.Text = string.Format("Plik {0}: {1}", plik.Nazwa, plik.Status);
            Application.DoEvents();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            EdycjaKonfiguracji konfig=new EdycjaKonfiguracji();
            konfig.ShowDialog(this);
        }

        internal void UruchomUpdate()
        {
            ShowInTaskbar = false;
            btnSprawdzAktualizacje_Click(this, new EventArgs());
        }

    }


}