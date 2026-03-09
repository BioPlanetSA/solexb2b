using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Interfaces;
using SolEx.Hurt.Model.Interfaces.Sync;

using System.Data.SqlClient;
using InsERT;
using ServiceStack.Text;
using SolEx.ERP.SubiektGT;
using SolEx.Hurt.Core.Sync;
using Adres = SolEx.Hurt.Model.Adres;
using Waluta = SolEx.Hurt.Model.Waluta;

namespace SolEx.Hurt.Sync.ProviderSubiekt
{
    public class Provider : IWystawianieDokumentu ,ISyncProvider, ISciaganieNowychRejestracji, IDokumentyRoznicowe, IEksportZdjecNadysk, IPobieraniePolaDokumentu, IDrukowanieDokumentowPdf, ITworzenieKategorii, IPobieranieKodwKreskowych, ILaczenieZamowien
    {
        private MainDAO _dao;
        public MainDAO DAO
        {
            get
            {
                if (_dao == null)
                    _dao = new MainDAO(_config);
                return _dao;
            }
        }

        private log4net.ILog Log => log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region ISyncProvider Members

        protected IConfigSynchro _config;
        public void UstawParametryLaczenia(IConfigSynchro config)
        {
            _config = config;
        }

        public string SourceCS { get; set; }

        public List<Produkt> PobierzProdukty(out List<Tlumaczenie> slownikiTlumaczen, out List<JednostkaProduktu> jednostki, HashSet<string> magazyny)
        {
            return DAO.PobierzProdukty(out slownikiTlumaczen, out jednostki,magazyny);
        }
        public List<ProduktCecha> PobierzCechyProduktow_Polaczenia(int[] atrybutydlaktorychpobieramycechy)
        {
            return DAO.GetProductTraits();
        }

        public List<ProduktyKodyDodatkowe> PobierzAlternatywneKodyKreskowe()
        {
            return DAO.PobierzAlternatywneKodyKreskowe();
        }

        public List<StatusZamowienia> PobierzStatusyDokumentow()
        {
            return DAO.PobierzStatusyDokumnetow();
        }

        public string przesunMazazyn(List<ZamowienieProdukt> suBraki, string mag, string docelowy, string nr, string uwagi)
        {
            return DAO.przesunMazazyn(suBraki,mag,docelowy,nr,uwagi);
        }

        public Dictionary<long, decimal> PobierzStanyDlaMagazynu(string mag)
        {
            int idMag = -1;

            try
            {
                idMag = Magazyny.PobierzIdMagazynu(mag);
            }
            catch
            {
                if (!PolaWlasne.SlownikPolWlasnychRozszerzonychIProstych[TypObiektu.Towar].ContainsKey(mag))
                {
                    Log.Error(new Exception(string.Format("Magazyn o symbolu {0} nie istnieje w Subiekcie. Nie ma te¿ pola w³asnego o nazwie {0}", mag)));

                }
            }
            LogiFormatki.PobierzInstancje.LogujInfo("id magazynu: {0}", idMag);
            //z magazynu
            //Dictionary<long, decimal> temp = idMag != -1 ? Magazyny.PobierzStanyProduktow(idMag, _config.UzwagledniaRezerwacjeStanow) : Magazyny.PobierzStanyProduktowZPolaWlasnego(mag);
            Dictionary<long, decimal> temp = idMag != -1 ? DAO.PobierzStanyProduktow(idMag, _config.UzwagledniaRezerwacjeStanow) : Magazyny.PobierzStanyProduktowZPolaWlasnego(mag);

            return temp;
        }


        public Dictionary<long, Klient> PobierzKlientow(List<Klient> klienciNaPlatformie, out Dictionary<Adres, KlientAdres> adresy)
        {

            return DAO.PobierzKlientow(out adresy);
        }
        public List<KategoriaKlienta> PobierzKategorieKlientow()
        {
            return DAO.GetCustomerCategories();
        }

        public List<Cecha> PobierzCechyIAtrybuty(out List<Atrybut> atrybuty, int[] atrybutydlaktorychniepobieramycechy)
        {
            return DAO.GetAttributes(out atrybuty);
        }

        public virtual List<ZamowienieSynchronizacja> ImportZamowien(List<ZamowienieSynchronizacja> list,Dictionary<long, Klient> wszyscy )
        {
            if (string.IsNullOrEmpty(_config.ERPLogin))
                throw new Exception("Pusty ERPLogin");
            if(string.IsNullOrEmpty(_config.ERPcs))
                throw new Exception("Pusty ERPcs");
            return DAO.SetOrders(list,wszyscy);
        }

        public List<CenaPoziomu> PobierzPoziomyCenoweProduktow()
        {
            return DAO.GetProductsPriceLevels();
        }

        public bool CleanUp()
        {
            if (DAO != null)
            {
                DAO.CleanUp();
            }
        
            return true;
        }

        #endregion


 

        public Rejestracja ImportKlientowDoERP(Rejestracja klienci)
        {
            return DAO.CreateCustomers(klienci);
        }

        public Dictionary<HistoriaDokumentu, List<HistoriaDokumentuProdukt>> DokumentyDoWyslania(Dictionary<int, long> dokumentyNaPlatformie, DateTime @from, List<Klient> klienci)
        {
            return DAO.GetDocumentToSend(dokumentyNaPlatformie, from, klienci);
        }

        public List<int> DokumentyDoUsuniecia(Dictionary<int, long> dokumentyNaPlatformie, HashSet<long>idKlientowB2b)
        {
            return DAO.GetDocumentToDelete(dokumentyNaPlatformie, idKlientowB2b);
        }
        public List<PoziomCenowy> PobierzDostepnePoziomyCen()
        {
            return DAO.PobierzDostepnePoziomyCen();
        }

        public Dictionary<long,Waluta> PobierzDostepneWaluty()
        {
            return DAO.PobierzDostepneWaluty();
        }

        public List<KlientKategoriaKlienta> PobierzKategorieKlientowPolaczenia()
        {
            return DAO.PobierzKategorieKlientowPolaczenia();
        }

        #region pobieranie zdjêæ

        const string SelectSubiektBytes = @"select zd_Zdjecie  from tw_ZdjecieTw where zd_Id={0}";

        public void ZapiszZdjeciaNaDysk(string path, string separator, TypyPolDoDopasowaniaZdjecia polaZapisuZdjec)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
       
            SqlConnection subConn = null;
            SqlCommand subcmd = null;
            try
            {

                List<Plik> files = new List<Plik>();

                List<string> pliki = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories).ToList();

                foreach (string file in pliki)
                {
                    FileInfo info = new FileInfo(file);
                    Plik ptemp = new Plik { Data = info.LastWriteTime.ToUniversalTime().AddMilliseconds(-info.LastWriteTime.ToUniversalTime().Millisecond), Nazwa = info.Name, nazwaLokalna = info.Name, Rozmiar = (int)info.Length, Sciezka = info.DirectoryName + "\\", Id = 0 };
                    files.Add(ptemp);
                }
                 //pliki znalezione w katalogu
                subConn = new SqlConnection(_config.ERPcs);
                subConn.Open();
                subcmd = new SqlCommand("select zd_id,t.tw_symbol,zd_Glowne,zd_Crc,t.tw_Nazwa, t.tw_Id, t.tw_PodstKodKresk from tw_ZdjecieTw zt join tw__towar t on zt.zd_IdTowar=t.tw_Id  order by zd_Glowne desc, zd_id",subConn);
                List<Plik> subiektPics = new List<Plik>(10000);
                using (SqlDataReader subrd = subcmd.ExecuteReader())
                {
                    while (subrd.Read())
                    {
                        string symbol = DataHelper.dbs("tw_symbol", subrd).Trim();
                        int idzdjecia = DataHelper.dbi("zd_id", subrd);
                        bool main = DataHelper.dbb("zd_Glowne", subrd);
                        int towarid = DataHelper.dbi("tw_Id", subrd);
                        string ean = DataHelper.dbs("tw_PodstKodKresk", subrd);

                        Plik temp = new Plik { Id = idzdjecia, Nazwa = TextHelper.PobierzInstancje.WygenerujNazweZdjecia(polaZapisuZdjec, separator, towarid, ean, symbol, main, idzdjecia) };
                        temp.Sciezka = path + "\\" + temp.Nazwa[0] + "\\";
                        temp.PoprawNazwaPlikuDlaURL();
                        subiektPics.Add(temp);
                    }
                } //wyciagniecie danych z suba

                Log.DebugFormat("Wstêpnie wybrano {0} zdjêæ z Subiekta", subiektPics.Count);

                foreach (Plik t in subiektPics) {
//upewniam siê czy plik zdjêcia na pewno istnieje, jeœli nie to je pobieram
                    if (files.Any(p => p.Nazwa == t.Nazwa)) { continue; }
                    try
                    {
                        if(subConn.State == ConnectionState.Closed)
                            subConn.Open();
                        subcmd = new SqlCommand(string.Format(SelectSubiektBytes, t.Id), subConn);
                        t.DanePlikBase64 = Convert.ToBase64String((byte[]) subcmd.ExecuteScalar());
                        CreatePic(t);


                        t.DanePlikBase64 = null;
                    }
                    catch (Exception ex)
                    {
                        Log.Debug("Pobieranie zdjêcia z subiekta " + t.Nazwa + " " + ex.Message + " " + ex.StackTrace);
                        //throw;
                    }
                }
                foreach (string plik in pliki)
                {
                    if (subiektPics.All(a => a.SciezkaWzgledna != plik))
                    {
                        try
                        {
                            File.Delete(plik);
                        }
                        catch (Exception ex)
                        {
                            Log.Error(new Exception(string.Format("Nie uda³o siê usun¹æ pliku {0} z powodu b³êdu: {1}", plik, ex.Message), ex));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message + " " + ex.StackTrace);
                throw;
            }
            finally
            {
                if (subConn != null) subConn.Close();
                if (subcmd != null) subcmd.Dispose();
            }
        }
        public void CreatePic(Plik plik)
        {
            PlikiBase64.Base64ToFile(plik.DanePlikBase64, plik.SciezkaWzgledna);
        }


        #endregion

         public string PobierzPole(int dokumentId, string pole)
         {
             return DAO.PobierzPoleDokumentu(dokumentId,pole);
         }

         public Dictionary<int, Dictionary<string, string>> PobierzPole(HashSet<int> dokumentyId)
         {
             return DAO.PobierzPolaDokumentu(dokumentyId);
         }



         public virtual bool DrukujPdfDokument(StatusDokumentuPDF id, ref string sciezka)
         {
            return   DAO.DrukujPDFDokument(id.IdDokumentu,sciezka);
         }

         public void PrzetworzKategorie(List<Grupa> grupyPRoduktow)
         {
             var pobraneKategorie = DAO.PobierzGrupy();
             const char pusty = '\0';
             for (int i = 0; i < pobraneKategorie.Count; i++)
             {
                 char separator = pobraneKategorie[i].Value.Contains('\\')
                                       ? '\\'
                                       : pobraneKategorie[i].Value.Contains('/') ? '/' : pusty;

                 if (separator != pusty)
                 {
                     string nowaKategoria = pobraneKategorie[i].Value.Substring(0, pobraneKategorie[i].Value.LastIndexOf(separator)).Trim();

                     if (pobraneKategorie.All(a => a.Value != nowaKategoria))
                     {
                         int? id = DAO.DodajNowaGrupe(nowaKategoria);
                         if(id.HasValue)
                         {
                             pobraneKategorie.Add(new KeyValuePair<int, string>(id.Value,nowaKategoria));
                         }
                     }
                 }

             }

             foreach (Grupa grupy in grupyPRoduktow)
             {
                 var pobraneKategorieZcech = DAO.ListaKategoriiZCech(grupy);

                 for (int i = 0; i < pobraneKategorieZcech.Count; i++)
                 {
                     char separator = pobraneKategorieZcech[i].Value.Contains('\\')
                                          ? '\\'
                                          : pobraneKategorieZcech[i].Value.Contains('/') ? '/' : pusty;

                     if (separator != pusty)
                     {
                         string nowaKategoria = pobraneKategorieZcech[i].Value.Substring(0,
                                                                                         pobraneKategorieZcech[i].Value
                                                                                                                 .LastIndexOf
                                                                                             (separator));

                         if (pobraneKategorieZcech.All(a => a.Value != nowaKategoria))
                         {
                             int? id = DAO.DodajNowaCecheDlaKategorii(nowaKategoria);
                             if (id.HasValue)
                             {
                                 pobraneKategorieZcech.Add(new KeyValuePair<int, string>(id.Value, nowaKategoria));
                             }
                         }
                     }

                 }
             }
         }
         public List<Rabat> PobierzRabaty(Dictionary<long, Klient> dlaKogoLiczyc, Dictionary<long, Produkt> produkty, List<PoziomCenowy> ceny, List<Cecha> cechy, List<ProduktCecha> cechyProdukty, Dictionary<long, KategoriaProduktu> kategorie, List<ProduktKategoria> produktyKategorie, IDictionary<int, KategoriaKlienta> kategorieKlientow, IDictionary<long, KlientKategoriaKlienta> klienciKategorie)
         {
             return DAO.PobierzRabaty(dlaKogoLiczyc);
         }

         public string WystawDokument(ZamowienieSynchronizacja zamowienie, int dlakogo, string dlakogoSymbol, string magazyn, TypDokumentu typ, out string braki)
         {
             return DAO.WystawWz(zamowienie, dlakogo, dlakogoSymbol, magazyn, typ,out braki);
         }


         public List<Kraje> PobierzKraje()
         {
             return DAO.PobierzKraje();
         }


         public List<Region> PobierzRegiony()
         {
             return DAO.PobierzRegiony();
         }


         public List<Magazyn> PobierzMagazynyErp()
         {
             return DAO.PobierzMagazyny();
         }

        public object PrzetorzZamowieniePoPolaczeniu(object dokument, Dictionary<string, object> parametry)
        {
            Log.Debug($"Parametry: {parametry.ToJson()}");
            object uwagi;
            if (parametry.TryGetValue("Uwagi", out uwagi))
            {
                if(!string.IsNullOrEmpty(uwagi.ToString())) ((SuDokument)dokument).Uwagi = uwagi;
            }
            
            object flaga;
            if (parametry.TryGetValue("Flaga", out flaga))
            {
                var dok = (SuDokument) dokument;
                string tmpFlaga = flaga.ToString();
                if (tmpFlaga.Equals("USUN", StringComparison.InvariantCultureIgnoreCase))
                {
                    Log.Info($"Usuwam flagê dla dokumentu: {dok.NumerPelny}");
                   DAO.UsunFlage((int)dok.Identyfikator,8);
                }
                else
                {
                    int idFlagi = DAO.PobierzIdFlagi(tmpFlaga, 8);
                    Log.Debug($"IdFlagi: {idFlagi}");
                    if (idFlagi > 0)
                    {
                        bool jestJuzWpisDlaDokumenty = DAO.SprawdzWpis((int)dok.Identyfikator, idFlagi, 8);
                        if (jestJuzWpisDlaDokumenty)
                        {
                            Log.Debug("jest dokument aktualizujemy");
                            DAO.ZmienFlageDlaDokumentu((int) dok.Identyfikator, idFlagi, 8);
                        }
                        else
                        {
                            DAO.DodajFlageDlaDokumentu((int)dok.Identyfikator, idFlagi, 8);

                        }
                    }
                }
            }
            Log.Debug($"Koniec przetwarzania dokumentu");
            return dokument;
        }
    }
}
