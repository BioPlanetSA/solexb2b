using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SolEx.Hurt.Sync.Core;
using SolEx.Hurt.Model;
using System.IO;
using SolEx.Hurt.Sync.Core.Configuration;
using SolEx.Hurt.Sync.App.ServiceReference1;
using System.Threading;
using SolEx.Hurt.Model.Core;
using SolEx.Hurt.Sync.App.Modules_;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.DAL;

//using SolEx.Hurt.Sync.Service;

namespace SolEx.Hurt.Sync.App.Controls
{
    public partial class ImportDataControl : UserControl
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public WorkEndDelegate WorkEndCallback;

        public ImportDataControl()
        {
            InitializeComponent();
            btnImportCustomer.Visible = SyncManager.GetProvider(Config.Settings.GetSettingString("provider","")) as ISciaganieNowychRejestracji !=null;
        }

        public void Run()
        {
            if (Program.Param == "-auto")
            {
                if (Config.Settings.GetSettingBool("import",true))
                {
                    try
                    {
                        WorkEndCallback();
                    }
                    catch
                    {
                        Application.Exit();
                    }
                }
                else
                {
                    log.Error("Początek importu");
                    if (Program.Param2 == "-semi")
                    {
                        log.Error("Początek importu kleintów");
                        btnImportCustomer_Click(btnImportCustomer, new EventArgs());
                    }
                    bwMain.RunWorkerAsync();
                }
            }
        }

   

        private void btnStart_Click(object sender, EventArgs e)
        {

            edLog.Text = "";
            btnStart.Enabled = false;
            btnImportCustomer.Enabled = false;
            btnStop.Enabled = true;

            bwMain.RunWorkerAsync();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            bwMain.CancelAsync();
        }

        private void bwMain_DoWork(object sender, DoWorkEventArgs e)
        {
            //e.Result = CreteTestORder();
            //return;
            string path = "paczka_b2b.zip";
            log.Info("import ");
            bwMain.ReportProgress(0, "Rozpoczęcie operacji.\r\n");
            bwMain.ReportProgress(0, " Otwieranie połączenia z serwerem...");

            SyncB2BClient client = new SyncB2BClient();

            client.Open();

            ResponseB2B resp = null;

            try
            {
                bwMain.ReportProgress(10, " Wysyłanie żądania o wygenerowanie paczki z danymi...");
                if (bwMain.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }

                IAsyncResult r = client.BeginExportData(path, null, null);

                for (int i = 0; i < Program.WaitAttemptCount; ++i)
                {
                    if (r.IsCompleted)
                    {
                        resp = client.EndExportData(r);
                        break;
                    }
                    Thread.Sleep(Program.WaitAttemptSleep);
                }
            }
            finally
            {
                try
                {
                    client.Close();
                }
                catch (Exception ex)
                {
                }
            }

            if (resp == null)
                throw new Exception("Nie udało się zaimportować danych z serwera");

            if (resp.IsError)
                throw new Exception(resp.Error);

            bwMain.ReportProgress(20, " Pobieranie wygenerowanej paczki...");
            if (bwMain.CancellationPending)
            {
                e.Cancel = true;
                return;
            }

            byte[] bytes = null;

            try
            {
                bytes = SyncManager.Download2(path);
            }
            catch
            {
                bwMain.ReportProgress(20, " Ponowienie próby...");
                if (bwMain.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }

                bytes = SyncManager.Download(path);
            }

            bwMain.ReportProgress(0, " Otwieranie paczki...");
            log.Error(bytes!=null?bytes.Length.ToString():"null");
            if (bwMain.CancellationPending)
            {
                e.Cancel = true;
                return;
            }
            log.Info("t-1");

            B2B b2b = (B2B)SyncManager.OpenPackage(bytes);
            log.Error("po rozpakowaniu paczki");
            bwMain.ReportProgress(0, "Liczba zamówień: "+ b2b.Orders.Count.ToString());
            log.Error("orders: " + b2b.Orders == null ? "null" : b2b.Orders.Count.ToString() );

            e.Result =  b2b;
        }

        private void bwMain_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            edLog.Text += (string)e.UserState + "\r\n";
        }
        private void bwMain_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            string folderDoZapisuPlikow = AppDomain.CurrentDomain.BaseDirectory + (Program.Param == "-auto" ? "paczka_temp_auto\\" : "paczka_temp\\");

            if (e.Cancelled)
            {
                edLog.Text += "\r\n\r\n Operacja została przerwana.";
                return;
            }
            else
                if (e.Error != null)
                {
                    Program.IsError = true;
                    try
                    {
                        SolEx.Hurt.Core.SolexLogManager.SendReportEmail(e.Error, "SynchroApp - Import danych " + (Program.Param == "-auto" ? "auto" : ""), edLog.Text,Config.Settings.GetSettingString("email_error","")
                            ,Config.Settings.GetSettingString("email_customer_error",""));
                    }
                    catch (Exception ex)
                    {
                        log.Info("mail " + ex.Message);
                    }
                    edLog.Text += "\r\n\r\n Wystąpił błąd! : " + e.Error.Message;

                    if (Program.Param == "-auto")
                    {
                        try
                        {
                            WorkEndCallback();
                        }
                        catch
                        {
                            Application.Exit();
                        }
                    }
                    return;
                }

            edLog.Text += " Pobieranie dostawcy...\r\n";

            try
            {
                ISyncProvider provider = SyncManager.GetProvider( Config.Settings.GetSettingString("provider","") );

                IImportingOrderModule dodatkowyModulImportowaniaZamowien = null;

                //if (Config.AppSettings.AllKeys.Any(k => k == "ModuluDodatkoweImportowaniaZamowien" && !string.IsNullOrEmpty(Config.AppSettings["ModuluDodatkoweImportowaniaZamowien"])))
                //{
                //    dodatkowyModulImportowaniaZamowien = App.Modules_.AppModulesCore.GetProviderImportingOrders(Config.AppSettings["ModuluDodatkoweImportowaniaZamowien"]);
                //}
                

                List<ExportedData> saved = new List<ExportedData>();
                ResponseB2B resp = null;
                edLog.Text += " Rozpoczęcie zapisywania informacji na systemie źródłowym...\r\n";
               
                if (e.Result != null && ((B2B)e.Result).Orders != null)
                {
                    foreach (Order o in ((B2B)e.Result).Orders)
                    {
                        edLog.Text += " Zapisywanie zamówienia (" + o.Id + ") ...\r\n";

                        List<Order> or = new List<Order>();
                        or.Add(o);

                        if (dodatkowyModulImportowaniaZamowien != null)
                        {
                            try
                            {
                                edLog.Text += " Uruchamianie zadania dodatkowego importowania zamówień...\r\n";
                                dodatkowyModulImportowaniaZamowien.DoWork(ref or);
                                edLog.Text += " Koniec zadania dodatkowego importowania zamówień\r\n";
                                provider.CleanUp();
                            }
                            catch (Exception ex)
                            {
                                edLog.Text += " Błąd uruchamianie zadania dodatkowego importowania zamówień: " + ex.Message + "\r\n" + ex.StackTrace + " \r\n";
                                if (ex.InnerException != null)
                                {
                                    edLog.Text += ex.InnerException.Message + "\r\n" + ex.InnerException.StackTrace + " \r\n";
                                }
                                log.Error("Błąd uruchomienia modułów dodatkowych zapisu zamówienia" + ex.Message);
                            }
                        }

                        try
                        {
                            try
                            {

                                List<SolEx.Hurt.Model.historia_dokumenty> d = provider.SetOrders(or);

                                if (d != null && d.Count > 0)
                                {
                                    ExportedData doc = new ExportedData();
                                    doc.CreateDate = d[0].data_utworzenia;
                                 
                                    doc.CustomerId = d[0].klient_id;
                                
                                    doc.DocumentName = d[0].nazwa_dokumentu;
                             
                                    doc.Id = o.Id;
                                    SendFile(doc, d, o);

                                    saved.Add(doc);
                                }
                            }
                            catch (SaveOrderException orderEx) {
                                if (o.Customer.opiekun_id != null)
                                {
                                   SolEx.Hurt.Model.CustomSearchCriteria.CustomerSearchCriteria criteria = new SolEx.Hurt.Model.CustomSearchCriteria.CustomerSearchCriteria();
                                    criteria.klient_id.Add(o.Customer.opiekun_id.Value);
                                    klienci opiekun= Sync.Core.SyncManager.GetCustomer(criteria).First();
                                    SolEx.Hurt.Core.SolexLogManager.SendReportEmail(orderEx, opiekun.email, Config.Settings.GetSettingString("email_error",""));
                                }
                                if(orderEx.InnerException != null)
                                {
                                    log.Error("Błąd pobierania zamówienia");
                                    log.Error(orderEx.InnerException.Message);
                                    log.Error(orderEx.InnerException.StackTrace);
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            try
                            {
                                log.Info("zapisywanie blad: " + ex.Message + " _ " + ex.StackTrace);
                                SolEx.Hurt.Core.SolexLogManager.SendReportEmail(ex, "SynchroApp - Import zamówienia " + (Program.Param == "-auto" ? "auto" : ""), edLog.Text,  Config.Settings.GetSettingString("email_error","")
                                    , Config.Settings.GetSettingString("email_customer_error",""));
                            }
                            catch (Exception ex1)
                            {
                            }

                            provider.CleanUp();
                            edLog.Text += " Zapisywanie zamówienia nie powiodło się. Wystąpił błąd : " + (ex == null ? "ex = null" : ex.Message + "\r\n" + ex.StackTrace + "\r\n" + (ex.InnerException != null ? ex.InnerException.Message : "null")) + "\r\n";
                        }
                    }

                    #region wysylanie potwierdzenia
                    edLog.Text += " Wysyłanie raportu o operacji...";
                    log.Info("Wysyłanie raportu o operacji...");
                    provider.CleanUp();

                    SyncB2BClient client = new SyncB2BClient();
                    client.Open();

                    try
                    {
                        IAsyncResult r = client.BeginExportDataComplete(saved.ToArray(), null, null);

                        for (int i = 0; i < Program.WaitAttemptCount; ++i)
                        {
                            log.Info("czekanie na odebranie paczki " + i.ToString());
                            if (r.IsCompleted)
                            {
                                resp = client.EndExportDataComplete(r);
                                break;
                            }
                            Thread.Sleep(Program.WaitAttemptSleep);
                        }
                    }
                    finally
                    {
                        if (client != null)
                            client.Close();
                    }

                    if (resp == null)
                        throw new Exception("Nie udało się wysłać raportu o zaimportowanych danych na serwer");

                    if (resp.IsError)
                        edLog.Text += resp.Error;
                    log.Info("koniec importu");
                    edLog.Text += "\r\n\r\n Operacja pobierania zamówień zakończona pomyślnie.";
                    edLog.Text += "\r\n\r\n Rozpoczęto aktualizację dokumentów.";
                    #endregion
                    DateTime date = CoreManager.GetDocumentsMiniStartDate();
          
                    IDokumentyRoznicowe roznicowyProvider = provider as IDokumentyRoznicowe;
                    if (roznicowyProvider != null)
                    {
                        #region dokumentyRoznicowe
                        Sync.Core.SyncManager.SendDocs(roznicowyProvider, date);
                        #endregion
                    }
                }

                if (resp != null && resp.IsError)
                {
                    edLog.Text += "\r\n\r\n Błąd wysyłania dokumentów.";
                    throw new Exception(resp.Error == null ? "błąd" : resp.Error);
                }
                else
                {
                    edLog.Text += "\r\n\r\n Operacja pobierania dokumentów zakończona pomyślnie.";
                }
            }
            catch (Exception ex)
            {
                Program.IsError = true;

                try
                {
                    log.Error(ex.Message);
                    log.Error(ex.StackTrace);
                    SolEx.Hurt.Core.SolexLogManager.SendReportEmail(ex, "SynchroApp - Import danych " + (Program.Param == "-auto" ? "auto" : ""), edLog.Text, 
                        Config.Settings.GetSettingString("email_error",""),  Config.Settings.GetSettingString("email_customer_error",""));
                }
                catch (Exception ex1)
                {
                }

                edLog.Text += "\r\n\r\n Wystąpił błąd! : " + ex.Message;
            }
            if (Config.GetLicense("dokumenty_powiadomienia"))
            {
                try
                {
                    edLog.Text += "\r\n\r\n Rozpoczęto wysyłanie powiadomień o niezapłaconych fakturach.";
                    string url = string.Format("http://{0}/DokumentyPlatnosciHandler.ashx", Config.PlatformURL);
                    //   log.Error(url);
                    MyWebRequest req = new MyWebRequest(url);
                    string data = req.GetResponse();
                    edLog.Text += "\r\n\r\n " + data;
                    edLog.Text += "\r\n\r\n Zakończono wysyłanie powiadomień o niezapłaconych fakturach.";
                }
                catch (Exception ex) { log.Error("Przypomnienie o przeterminowanych fakturach. " + ex.Message + "__" + ex.StackTrace); }
            }
            if (Config.GetLicense("import_szablonow"))
            {
                try
                {
                    edLog.Text += "\r\n\r\n Rozpoczęto imporowanie nowych maili.";
                    string url = string.Format("http://{0}/FormularzeMaileHandler.ashx", Config.PlatformURL);
                    //   log.Error(url);
                    MyWebRequest req = new MyWebRequest(url);
                    string data = req.GetResponse();
                    edLog.Text += "\r\n\r\n " + data;
                    edLog.Text += "\r\n\r\n Zakończono imporowanie nowych maili.";
                }
                catch (Exception ex) { log.Error("Import maili szablonów. " + ex.Message + "__" + ex.StackTrace); }
            }
            //sfera pobieranie pdf
            if (Config.Settings.GetSettingBool("dokumenty_elektroniczne_pdf",false))
            {

                try
                {
                    bool sendAllTime = false;
                    bool sendAll = Config.Settings.GetSettingBool("dokumenty_wyslac_wszyskie",false,false);
                    string godzinaWyslaniaWszystichDokuemntow = Config.Settings.GetSettingString("dokumenty_automatyczne_wyslanie_pdf","00:00", false);
                    DateTime lastSend = Config.Settings.GetSettingDateTime("dokumenty_automatyczne_wyslanie_pdf_ostanie", DateTime.Now, false);

                        DateTime minData = DateTime.Now.Date;
                        string[] date = godzinaWyslaniaWszystichDokuemntow.Split(':');
                        if (date.Length > 0)
                        {
                            minData = minData.AddHours(int.Parse(date[0]));
                            if (date.Length > 1)
                            {
                                  minData =  minData.AddHours(int.Parse(date[1]));
                            }
                        }

                        if (minData < DateTime.Now    && DateTime.Now.AddHours(-24) > lastSend)
                        {
                            sendAllTime = true;
                        }
                    edLog.Text += "\r\n\r\n Rozpoczęto drukowanie pdf faktur.";
                    ISyncProvider provider = SyncManager.GetProvider();
                    SyncB2BClient client = null;
                    ResponseB2B resp = null;
                    string sfera_fileNames = "paczka_sfera.zip";
                    //List<string> documentToDeleteOnServer = new List<string>();
                    List<string> filesToSend = null;

                    filesToSend = new List<string>();
                    SolEx.Hurt.Sync.Core.Configuration.SendingDocs newSendingDocs = new SolEx.Hurt.Sync.Core.Configuration.SendingDocs(sendAll || sendAllTime);
                    filesToSend = provider.CreateDocs(newSendingDocs);
                    newSendingDocs.SaveData();
                    StringBuilder fileNames = new StringBuilder();
                    provider.CleanUp();
                    foreach (string file in filesToSend)
                    {
                        fileNames.Append(file + ";");
                    }
                    SourceDB db = new SourceDB();

                    try
                    {
                        fileNames.Append(";" + SyncManager.SerializeObjectToFile(db, folderDoZapisuPlikow));
                        string plikDoWyslania = SyncManager.CreatePackage(fileNames.ToString(), folderDoZapisuPlikow);

                        edLog.Text += "\r\n Wysyłanie paczki o rozmiarze.";

                        SyncManager.Upload(sfera_fileNames, plikDoWyslania);
                        edLog.Text += "\r\n Wysłanie żądania o odebranie paczki...";
                        client = new SyncB2BClient();

                        client.Open();
                        IAsyncResult r = null;
                        resp = null;
                        List<string> fileNameToSend = new List<string>(filesToSend.Count);
                        for (int i = 0; i < filesToSend.Count; i++)
                        {
                            fileNameToSend.Add(Path.GetFileName(filesToSend[i]));
                        }
                        r = client.BeginSferaExportDocsComplete(sfera_fileNames, null, fileNameToSend.ToArray(), null, null);

                        for (int i = 0; i < Program.WaitAttemptCount; ++i)
                        {
                            if (r.IsCompleted)
                            {
                                resp = client.EndSferaExportDocsComplete(r);
                                break;
                            }
                            Thread.Sleep(Program.WaitAttemptSleep);
                        }
                    }
                    catch(Exception ex)
                    {
                        log.Error(ex.Message + "__" + ex.StackTrace);
                    }
                    finally
                    {
                        if (client != null)
                            client.Close();
                    }
                    if (resp==null ||( resp != null && resp.IsError))
                    {
                        edLog.Text += "\r\n\r\n Błąd wysyłania dokumentów sfera.";
                        throw new Exception(resp.Error == null ? "błąd" : resp.Error);
                    }
                    else
                    {
                        edLog.Text += "\r\n\r\n Operacja drukowania zakończona pomyślnie.";
                        Config.Settings.SetSetting("dokumenty_wyslac_wszyskie","0","bool");
                        if (sendAllTime)
                        {
                            Config.Settings.SetSetting("dokumenty_automatyczne_wyslanie_pdf_ostanie",DateTime.Now.ToString(),"string");
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.Error("sfera" + ex.Message + " " + ex.StackTrace);
                }
            }

            btnStart.Enabled = true;
            btnStop.Enabled = false;
            btnImportCustomer.Enabled = true;
            if (Program.Param == "-auto")
            {
                try
                {
                    WorkEndCallback();
                }
                catch
                {
                    Application.Exit();
                }
            }
        }
        private ExportedDataItem[] GetMissingItems(List<OrderItem> list)
        {
            if (list == null)
                return null;
            return list.Select(p => new ExportedDataItem { Id = p.ProductId, Quantity = p.QuantityMissing }).ToArray();
        }

        private void SendFile(ExportedData doc, List<historia_dokumenty> d, Order o)
        {
            //if (!string.IsNullOrEmpty(d[0].FilePath))
            //{
            //    try
            //    {
            //        string fileName = System.IO.Path.GetFileName(d[0].FilePath);

            //        SyncManager.Upload(fileName, d[0].FilePath);

            //        doc.FileName = fileName;

            //        try
            //        {
            //            File.Delete(d[0].FilePath);
            //        }
            //        catch
            //        {
            //        }
            //    }
            //    catch (Exception ex2)
            //    {
            //        try
            //        {
            //            SolEx.Hurt.Core.SolexLogManager.SendReportEmail(ex2, "SynchroApp - Import zamówienia " + "pdf upload" + (Program.Param == "-auto" ? "auto" : ""), edLog.Text,
            //                Config.Settings.GetSettingString("email_error",""),  Config.Settings.GetSettingString("email_customer_error",""));
            //        }
            //        catch
            //        {
            //            throw;
            //        }
            //    }
            //}
        }

        private void btnImportCustomer_Click(object sender, EventArgs e)
        {
            try
            {
                edLog.Text = "";
                edLog.Text += "\r\n\r\n Rozpoczęto dodawnie nowych kontrachentów.";
             
                MyWebRequest req = new MyWebRequest(string.Format("http://{0}/RejestracjePobieranieHandler.ashx", Config.PlatformURL));
                string data = req.GetResponse();
                List<Rejestracja> items = (List<Rejestracja>)SolEx.Hurt.Helpers.JSonHelper.Deserialize(data, typeof(List<Rejestracja>));

                ISyncProvider prov = SyncManager.GetProvider(Config.Settings.GetSettingString("provider",""));

                List<Rejestracja> response = new List<Rejestracja>();
                for (int i = 0; i < items.Count; i++)
                {
                    edLog.Text += string.Format("\r\n\r\n Zapisywanie klienta o mailu: {0} i nipie: {1}", items[i].Email, items[i].NIP);
                    try
                    {
                        response.Add(((ISciaganieNowychRejestracji)prov).DodajKlientow(items[i]));
                        edLog.Text += string.Format("\r\n\r\n Zapisywanie klienta o mailu: {0} i nipie: {1} zakończone powodzeniem. \r\n Klient zapisany pod symbolem {2}", items[i].Email, items[i].NIP,items[i].Symbol);
                    }
                    catch (CustomerSaveException error)
                    {
                        string msg= string.Format(" Zapisywanie klienta o mailu: {0} i nipie: {1} nie powiodło się. Powód: {2}. Rejestracja nie będzie ponownie wysyłana. Proszę poprawić dane i ponownie odznaczyć ją do eksportu", items[i].Email, items[i].NIP,error.Reason);
                         edLog.Text +="\r\n\r\n"+msg;
                        response.Add(error.Register);
                        SolEx.Hurt.Core.SolexLogManager.SendReportEmail(msg, "Dodawanie klienta nie powiodło się", Config.Settings.GetSettingString("email_error",""));
                    }
                }
                prov.CleanUp();
                string send = SolEx.Hurt.Helpers.JSonHelper.Serialize(response);
                MyWebRequest request = new MyWebRequest(string.Format("http://{0}/RejestracjeAktualizacjaHandler.ashx", Config.PlatformURL), "POST", send);
                edLog.Text += "\r\n\r\n Zakończono dodawnie nowych kontrachentów.";
            }
            catch (Exception ex) { log.Error("Import klientów " + ex.Message + "__" + ex.StackTrace);
            edLog.Text += "\r\n\r\n Nie udało się dodać nowych klientów.";
            edLog.Text += "\r\n\r\n Błąd: " +ex.Message +".";
            }
        }
    }
}
