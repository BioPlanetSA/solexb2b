using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using SolEx.Hurt.Sync.App.ServiceReference1;
using SolEx.Hurt.Sync.Core;
using SolEx.Hurt.Sync.Core.Configuration;
using SolEx.Hurt.Model;
using SolEx.Hurt.Sync.App.Modules_;
using SolEx.Hurt.CustomLogic.CoreDB;
using System.Diagnostics;
using System.Windows.Threading;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model.CustomSearchCriteria;
using SolEx.Hurt.DAL;
using System.Collections.Specialized;

namespace SolEx.Hurt.Sync.App.Controls
{
    public delegate void WorkEndDelegate();
    

    public partial class ExportDataControl : UserControl
    {

        
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public WorkEndDelegate WorkEndCallback;
        int denyBigPackageStart, denyBigPackageEnd;

        public ExportDataControl()
        {
            InitializeComponent();
        }

        public void loguj(string tekst)
        {
            edLog.Text +=  string.Format( "\r\n{0}", tekst);
        }

        public void Run()
        {
         denyBigPackageStart= Config.Settings.GetSettingInt("deny_big_package_start",-1);
         denyBigPackageEnd = Config.Settings.GetSettingInt("deny_big_package_end", -1);

            //if (Program.Param == "-auto")
            //{
            //    //TODO: auto eksport
            //}
        }

       
        private void btnStart_Click(object sender, EventArgs e)
        {
            edLog.Text = "";
            btnStart.Enabled = false;
            btnSendPrices.Enabled = false;
            btnCustomers.Enabled = false;
            btnStop.Enabled = true;

            bwMain.RunWorkerAsync();   
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            bwMain.CancelAsync();
        }


        private void btnSendPrices_Click(object sender, EventArgs e)
        {
            Program.Param2 = "-prices";
            btnStart_Click(sender, e);
        }

        private void OnProcessExited(object sender, EventArgs e)
        {
            this.Invoke((MethodInvoker)delegate
            {
                btnSendPrices.Enabled = true;
                loguj( "Wysyłanie cen zakończone pomyślnie.");
       
            });
        }

        private void btnCustomers_Click(object sender, EventArgs e)
        {
            denyBigPackageStart = -1; 
            denyBigPackageEnd = 25;
            btnStart_Click(sender, e);
        }

        private void bwMain_DoWork(object sender, DoWorkEventArgs e)
        {
            string folderZapisuPLikow = AppDomain.CurrentDomain.BaseDirectory + (Program.Param == "-auto" ? "paczka_temp_auto\\" : "paczka_temp\\");

            if (Directory.Exists(folderZapisuPLikow))
            {
                Directory.Delete(folderZapisuPLikow, true);
            }
            

            log.Info("parametr2" + Program.Param2 + "parametr2");
            string fileName = "paczka_db.zip";
            string fileNames = "";
            string zipFileName = "";
            string tableName = "";
            string[] fileName_2 =null;
           List<string> zipFileName_2 = new List<string>();
            string tableName_2 = "";
            bool csv = false, big = false;            

           

            ISyncProvider prov = SyncManager.GetProvider();
            SourceDB db = new SourceDB();
            if (prov != null)
            {

                bwMain.ReportProgress(10, " Generowanie paczki z danymi.");
                if (bwMain.CancellationPending) { e.Cancel = true; return; }

                List<int> pictureIds = Program.Pics;
                
                log.Info("Parametr " + Program.Param2);
                if (Program.Param2 == "-prices")
                {
                    if (!string.IsNullOrEmpty(Config.Settings.GetSettingString("ceny_calc_providers","")) || !string.IsNullOrEmpty(Config.Settings.GetSettingString("ceny_init_provider","")))
                    {
                        log.Info("Pobieranie wyliczonych cen");
                        bwMain.ReportProgress(10, "  Pobieranie wyliczonych cen ...");
                        if (bwMain.CancellationPending) { e.Cancel = true; return; }

                       // zipFileName_2 = "csv_ceny.zip";
                        tableName_2 = "flat_ceny";
                        csv = true;

                       fileName_2 = CoreManager.CreatePricesCSV("csv_ceny.csv", folderZapisuPLikow);
                       foreach (string s in fileName_2)
                       {
                           string zipName = Path.GetFileNameWithoutExtension(s) + ".zip";
                           SyncManager.SaveAndZipFile(s, null, folderZapisuPLikow, zipName, true);
                           zipFileName_2.Add(zipName);
                       }
                  
                    }
                    //else
                    {
                        log.Info("Pobieranie cen bazowych");
                        bwMain.ReportProgress(10, "  Pobieranie cen bazowych ...");
                        if (bwMain.CancellationPending) { e.Cancel = true; return; }

                        fileName = "csv_ceny_poziomy.csv";
                        zipFileName = "csv_ceny_poziomy.zip";
                        tableName = "ceny_poziomy_temp";

                        CoreManager.CreatePriceLevelsCSV(fileName, folderZapisuPLikow, prov.GetProductsPriceLevels(), !csv);
                        SyncManager.SaveAndZipFile(fileName, null, folderZapisuPLikow, zipFileName, true);
                        csv = true;
                    }
                }
                else if (Program.Param2 == "-lite")
                {
                    log.Info("Lite początek");
                    
                    try
                    {
                        string providers = Config.Settings.GetSettingString("additional_providers_lite","");
                        if (!string.IsNullOrEmpty(providers))
                        {
                          
                            bwMain.ReportProgress(0, "Wykonywanie dodatkowych operacji");
                            string[] providerNames = providers.Split(new string[] { ";;" }, StringSplitOptions.RemoveEmptyEntries);
                            foreach (string name in providerNames)
                            {
                                try
                                {
                                    NameValueCollection settnv=Config.Settings.ToNameValueCollection();
                                    settnv["program_mode2"] = Program.Param2;

                                    IImportDataModule module = AppModulesCore.GetProvider(name);
                                    module.ProgresChanged += new SolEx.Hurt.Sync.App.Modules_.ProgressChangedEventHandler(module_ProgresChanged);
                                    module.DoWork(settnv, db);
                                }
                                catch (Exception ex) { log.Info(ex.Message + " ___ " + ex.StackTrace); bwMain.ReportProgress(0, "   " + ex.Message); }
                            }
                            fileNames += folderZapisuPLikow + "csv_produkty_ukryte.csv";
                        }
                    }
                   
                    

                }
                else
                {
                    SolEx.Hurt.Model.StatusDuzejPaczki status = Config.PobierzStatusDuzejPaczki();
                    if (status == StatusDuzejPaczki.Przetwarzana)
                    {

                        e.Result = status; throw new Exception("Paczka przetwarzana");
                    }
                    big = true;
                    string sendWhat = null;

                    if (sendWhat == null || sendWhat == "1")
                    {
                        bwMain.ReportProgress(10, "  Pobieranie płatności...");
                        if (bwMain.CancellationPending) { e.Cancel = true; return; }
                        db.Payments = prov.GetPayments();

                        bwMain.ReportProgress(10, "  Pobieranie kategorii kontrahentów...");
                        if (bwMain.CancellationPending) { e.Cancel = true; return; }
                        db.CustomerCategories = prov.GetCustomerCategories();

                        bwMain.ReportProgress(10, "  Pobieranie kontrahentów...");
                        if (bwMain.CancellationPending) { e.Cancel = true; return; }
                        CustomerSearchCriteria criteria = new CustomerSearchCriteria();
                        List<klienci> klienciNaPlatformie =Sync.Core.SyncManager.GetCustomer(criteria);
                        db.Customers = prov.GetCustomers(klienciNaPlatformie);
                        bwMain.ReportProgress(10, "  Liczba kontrahentów: " + db.Customers.Count.ToString());

                        bwMain.ReportProgress(10, "  Pobieranie rabatów...");
                        if (bwMain.CancellationPending) { e.Cancel = true; return; }
                        db.Discounts = prov.GetDiscounts();
                        bwMain.ReportProgress(10, "  Liczba rabatów: " + (db.Discounts == null ? "-1" : db.Discounts.Count.ToString()));

                        bwMain.ReportProgress(10, "  Pobieranie kategorii...");
                        if (bwMain.CancellationPending) { e.Cancel = true; return; }
                        db.SourceCategories = prov.GetCategories();
                    }

                    if (Program.Param2 != "-semi" && (DateTime.Now.Hour <= denyBigPackageStart || DateTime.Now.Hour >= denyBigPackageEnd))
                    {
                        if (sendWhat == null || sendWhat == "1")
                        {
                            bwMain.ReportProgress(10, "  Pobieranie dokumentów...");
                            if (bwMain.CancellationPending) { e.Cancel = true; return; }
                         
                            IDokumentyRoznicowe roznicowyProvider = prov as IDokumentyRoznicowe;
                            if (roznicowyProvider != null)
                            {
                                #region dokumentyRoznicowe
                                Sync.Core.SyncManager.SendDocs(roznicowyProvider, CoreManager.GetDocumentsStartDate(null));
                                #endregion
                            }
                           
                            bwMain.ReportProgress(10, "  Pobieranie cech....");
                            if (bwMain.CancellationPending) { e.Cancel = true; return; }
                            db.Attributes = prov.GetAttributes();
                            bwMain.ReportProgress(10, "  Liczba cech: " + (db.Attributes == null ? "-1" : db.Attributes.Count.ToString()));
                        }

                        if (sendWhat == null || sendWhat == "2")
                        {
                            bwMain.ReportProgress(10, "  Pobieranie produktów...");
                            if (bwMain.CancellationPending) { e.Cancel = true; return; }
                            List<cechy> fieldAttributes = new List<cechy>();
                            db.Products = prov.GetProducts(pictureIds, out fieldAttributes);
                            db.Attributes.AddRange(fieldAttributes);
                            bwMain.ReportProgress(10, "  Liczba produktów: " + (db.Products == null ? "-1" : db.Products.Count.ToString()));
                        }
                    }
                }

                prov.CleanUp();
            }
            else 
            {
                log.Info("Nie znaleziony provider!");
            }
                try
                {
                    string providers = Config.Settings.GetSettingString("additional_providers","");
                    if (!string.IsNullOrEmpty(providers))
                    {
                        bwMain.ReportProgress(0, "Wykonywanie dodatkowych operacji");
                        string[] providerNames = providers.Split(new string[] { ";;" }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string name in providerNames)
                        {
                            try
                            {
                                NameValueCollection settnv = Config.Settings.ToNameValueCollection();
                                settnv["program_mode2"] = Program.Param2;
                                IImportDataModule module = AppModulesCore.GetProvider(name);
                                module.ProgresChanged += new SolEx.Hurt.Sync.App.Modules_.ProgressChangedEventHandler(module_ProgresChanged);
                                module.DoWork(settnv, db);
                            }
                            catch (Exception ex) { log.Info(ex.Message + " ___ " + ex.StackTrace + "__" +name); bwMain.ReportProgress(0, "   " + ex.Message); }
                        }
                        fileNames += folderZapisuPLikow + "csv_produkty_ukryte.csv;" + folderZapisuPLikow + "csv_dokumenty_listy.csv";
                    }
                }
                catch (Exception ex) { log.Info(ex.Message + " ___ " + ex.StackTrace); bwMain.ReportProgress(0, "   "+ ex.Message); }
            bwMain.ReportProgress(10, " Pakowanie wygenerowanej paczki z danymi:");
            bwMain.ReportProgress(10, string.Format(@"atrybuty: {0}, kategorie klientów: {1},  klienci: {2}, rabaty: {3}, dokumenty: {4}, produkty lite: {5}, płatności: {6}, produkty: {7}, kategorie źródłowe: {8}",
                db.Attributes == null ? 0 : db.Attributes.Count, db.CustomerCategories == null ? 0 : db.CustomerCategories.Count, db.Customers == null ? 0 : db.Customers.Count
                , db.Discounts == null ? 0 : db.Discounts.Count, db.Documents == null ? 0 : db.Documents.Count, db.LiteProducts == null ? 0 : db.LiteProducts.Count, db.Payments == null ? 0 : db.Payments.Count
                , db.Products == null ? 0 : db.Products.Count, db.SourceCategories == null ? 0 : db.SourceCategories.Count));

            if (bwMain.CancellationPending) { e.Cancel = true; return; }

            for (int i = 0; i < db.Customers.Count; i++)
            {
                db.Customers[i].Parameters = null;
            }
            for (int i = 0; i < db.Products.Count; i++)
            {
                db.Products[i].ErpPars = null;
            }

            //paczkowanie danych
            int wielkoscPaczki = 100000;
            if (db.Products.Count > wielkoscPaczki){
                bwMain.ReportProgress(10, " Ilość danych wymaga paczkowania danych");

                while(db.Products.Count > wielkoscPaczki) {
                    bwMain.ReportProgress(10, " Zapisywanie kolejnej paczki");
                    log.Info(string.Format("Zapisywanie kolejnej paczki", DateTime.Now));
                    List<Model.produkty> listaPomocnicza = db.Products.Take(wielkoscPaczki).ToList();
                    fileNames += ";" + SyncManager.SerializeObjectToFile(listaPomocnicza, folderZapisuPLikow);
                    db.Products.RemoveRange(0, wielkoscPaczki);
                }
            }

            string plikDoWyslania = null;

            if (csv && zipFileName != "")
            {
                string zipFileDoSynchro = folderZapisuPLikow + zipFileName;
                if (File.Exists(zipFileDoSynchro))
                    plikDoWyslania = zipFileDoSynchro;
            }
            else
            {
                log.Info(string.Format("Zapisywanie głównej paczki", DateTime.Now));
                fileNames += ";" + SyncManager.SerializeObjectToFile(db, folderZapisuPLikow);
                log.Info(string.Format("Pakowanie paczki", DateTime.Now));
                plikDoWyslania = SyncManager.CreatePackage(fileNames, folderZapisuPLikow);
            }


            if (!string.IsNullOrEmpty(plikDoWyslania))
            {
                bwMain.ReportProgress(10, " Wysyłanie paczki danych");
                bwMain.ReportProgress(10, "  Wysyłanie może trwać nawet do kilkudziesięciu minut.");

                if (bwMain.CancellationPending) { e.Cancel = true; return; }

                log.Info(csv ? zipFileName : fileName);
                try
                {
                    SyncManager.Upload(csv ? zipFileName : fileName, plikDoWyslania);
                    if (zipFileName_2.Count>0)
                    {
                        try
                        {
                            foreach (string s in zipFileName_2)
                            {
                                SyncManager.Upload(s, folderZapisuPLikow + s);
                            }
                        }
                        catch (Exception ex) { log.Info(ex.Message + " " + ex.StackTrace); }
                    }
                    if (Directory.Exists(folderZapisuPLikow))
                    {
                        Directory.Delete(folderZapisuPLikow, true);
                    }
                }
                catch (Exception ex) { log.Info(ex.Message + " " + ex.StackTrace); }
                log.Info("Wysłanie żądania o odebranie paczki...");
                SyncB2BClient client = new SyncB2BClient();

                client.Open();

                ResponseB2B resp = null;

                try
                {
                    bwMain.ReportProgress(0, " Wysłanie żądania o odebranie paczki... csv: " + csv.ToString());
                    if (bwMain.CancellationPending) { e.Cancel = true; return; }

                    IAsyncResult r = null;

                    if (csv)
                    {
                        r = client.BeginImportCSV(zipFileName, fileName, tableName, null, null);

                        log.Info("czekanie na odebranie paczki  start");
                        for (int i = 0; i < Program.WaitAttemptCount; ++i)
                        {
                            log.Info("czekanie na odebranie paczki " + i.ToString());
                            if (r.IsCompleted)
                            {
                                resp = client.EndImportCSV(r);
                                break;
                            }
                            Thread.Sleep(Program.WaitAttemptSleep);
                        }

                        if (zipFileName_2.Count > 0)
                        {
                            for (int j = 0; j< zipFileName_2.Count;j++ )
                            {

                                r = client.BeginImportCSV(zipFileName_2[j], fileName_2[j], tableName_2, null, null);

                                log.Info("czekanie na odebranie paczki2  start");
                                for (int i = 0; i < Program.WaitAttemptCount; ++i)
                                {
                                    log.Info("czekanie na odebranie paczki2 " + i.ToString());
                                    if (r.IsCompleted)
                                    {
                                        resp = client.EndImportCSV(r);
                                        break;
                                    }
                                    Thread.Sleep(Program.WaitAttemptSleep);
                                }
                            }
                        }
                    }
                    else
                    {
                        r = client.BeginImportData(fileName, null, null);
                        log.Info("czekanie na odebranie paczki3  start");
                        for (int i = 0; i < Program.WaitAttemptCount; ++i)
                        {
                            log.Info("czekanie na odebranie paczki3 " + i.ToString());
                            if (r.IsCompleted)
                            {
                                
                                resp = client.EndImportData(r);
                                break;
                            }
                            Thread.Sleep(Program.WaitAttemptSleep);
                        }
                        log.Info("Po pętlacha 3");
                    }
                }
                finally { try { if (client != null) client.Close(); } catch (Exception ex) { log.Info(ex.ToString()); } }
                log.Info("Po pętlacha");
                if (resp != null && resp.IsError)
                    throw new Exception(resp.Error);


                if (resp == null)
                    throw new Exception(string.Format("Aplikacja przez {0} minut nie otrzymała informacji o zakończeniu pracy przez usługę, nie będzie dłużej oczekiwać na nią",Program.WaitTimeMinutes));
            }
        }
        void module_ProgresChanged(object sender, SolEx.Hurt.Sync.App.Modules_.ProgressChangedEventArgs e)
        {
            bwMain.ReportProgress(0, e.Message);
        }

        private void bwMain_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            loguj( (string)e.UserState );
        }

        private void bwMain_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
                loguj( " Operacja została przerwana.");
            else if (e.Error != null)
            {
                Program.IsError = true;

                try
                {
                    log.Info(e.Error.Message + " ___ " + e.Error.StackTrace);
                    SolEx.Hurt.Core.SolexLogManager.SendReportEmail(e.Error, "SynchroApp - Eksport danych " + (Program.Param == "-auto" ? "auto" : ""), edLog.Text,
                        Config.Settings.GetSettingString("email_error",""),  Config.Settings.GetSettingString("email_customer_error",""));
                }
                catch { }
                loguj( " Wystąpił błąd! : " + e.Error.Message);

                WriteTimeStamp(Program.TimeStampParam);
            }
            else
            {
                StatusDuzejPaczki status = StatusDuzejPaczki.Wolny;
                try
                {
                  status=  (StatusDuzejPaczki)e.Result;
                }
                catch (Exception ex) { }
                if (status != null && status == StatusDuzejPaczki.Przetwarzana)
                {
                    loguj( " Operacja przerwana, dane są ciągle przetwarzane na serwerze.");
                }
                else
                {
                    loguj( " Operacja zakończona pomyślnie.");
                }
                WriteTimeStamp(Program.TimeStampParam);
            }
            btnStart.Enabled = true;
            btnSendPrices.Enabled = true;
            btnCustomers.Enabled = true;
            btnStop.Enabled = false;
            if (Program.Param == "-auto")
            {
                try
                {
                    WorkEndCallback();
                }
                catch { Application.Exit(); }
            }
        }

        private void WriteTimeStamp(string symbol)
        {
            if (string.IsNullOrEmpty(symbol))
                return;

            DateTime d = DateTime.Now;
            string s = null, data = DateTime.Now.ToString();            
            int t = Config.Settings.GetSettingInt((symbol.Contains("big_package") ? "big_package_execute_count" : "prices_execute_count"), 1), tt;


            if (t > 1)
            {
                s = File.ReadAllText(symbol);

                if (Int32.TryParse(s, out tt))
                {
                    if (t > ++tt)
                        data = tt.ToString();
                }
                else
                {
                    if (!DateTime.TryParse(s, out d))
                        d = new DateTime();
                    if (d.Date == DateTime.Now.Date)
                        return;
                    data = "1";
                }
            }

            StreamWriter sw = null;
            try
            {
                sw = File.CreateText(symbol);
                sw.Write(data);
            }
            finally { if (sw != null) { sw.Close(); sw.Dispose(); } }
        }

        

    }
}
