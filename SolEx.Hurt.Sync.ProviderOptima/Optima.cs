using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using CDNBase;
using CDNHeal;
using System.Reflection;
using log4net;
using Optima.Common.Logic.DynamicParametersLanguage;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model;
using Optima.Common.Logic.Wydruki;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Sync.ProviderOptima
{
    public  class Optima
    {
        private static Optima _instancja = new Optima();

        public static Optima PobierzInstancje
        {
            get { return _instancja;  }
            set { _instancja = value; }
        }

        public string Operator, Haslo;
        private IConfigBLL _configBll;
        public IConfigBLL ConfigBll
        {
            get { return _configBll; }
            set { _configBll = value; }
        }

        private  Application _app;
        private  ILogin _login;
        private  AdoSession _session = null;

        private  Application App
        {
            get
            {
                if (_app == null)
                {
                    Environment.CurrentDirectory = ConfigBll.KatalogProgramuKsiegowego;
                    HashSet<ModulyOptima> moduly= ConfigBll.JakieModulyOptima;

                    _app = new Application();
                    _app.LockApp(256, 5000, null, null, null, null);

                    //object[] hPar = new object[] { 0, 0, 0, 0, 0, !modulyPlus, 0, 0, 0, 0, 0, 0, 0, 0, !modulyPlus, modulyPlus, modulyPlus };// do jakich modułów się logujemy
                    ////Kolejno: KP, KH, KHP, ST, FA, MAG, PK, PKXL, CRM, ANL, DET, BIU, SRW, ODB, KB, KBP, HAP
                    //// ILogin Login([MarshalAs(UnmanagedType.Struct), In] object vUser,

                    //_login = _app.Login(ConfigBll.ERPLogin, ConfigBll.ERPHaslo, ConfigBll.OptimaNazwaFirmy, hPar[0], hPar[1], hPar[2], hPar[3], hPar[4], hPar[5], hPar[6], hPar[7], hPar[8], hPar[9], hPar[10], hPar[11], hPar[12], hPar[13], hPar[14], hPar[15], hPar[16]);
                    //_session = _login.CreateSession();


                    object[] hPar = new object[] { 0, 0, 0, 0, 0, moduly.Contains(ModulyOptima.Handel), 0, 0, 0, 0, 0, 0, 0, 0, moduly.Contains(ModulyOptima.Kasa_bank), moduly.Contains(ModulyOptima.Kasa_bank_PLUS), moduly.Contains(ModulyOptima.Handel_PLUS), 0 };// do jakich modułów się logujemy
                    object[] parametry = { ConfigBll.ERPLogin, ConfigBll.ERPHaslo, ConfigBll.OptimaNazwaFirmy, hPar[0], hPar[1], hPar[2], hPar[3], hPar[4], hPar[5], hPar[6], hPar[7], hPar[8], hPar[9], hPar[10], hPar[11], hPar[12], hPar[13], hPar[14], hPar[15], hPar[16], hPar[17] };
                    var metoda = _app.GetType().GetMethods().FirstOrDefault(x => x.Name == "Login" && x.GetParameters().Count() == parametry.Count());
                    if (metoda != null)
                    {
                        _login = metoda.Invoke(_app, parametry) as ILogin;
                    }
                    else
                    {
                        throw new Exception(string.Format("Nie znalezionio metody: Login, wywoływanej z ilością parametrów: {0}", parametry.Count()));
                    }
                    _session = _login.CreateSession();

                    //var metoda = _app.GetType().GetMethods().FirstOrDefault(x => x.Name == "Login" && x.GetParameters().Count() == parametry.Count());
                    //if (metoda != null)
                    //{
                    //    _login = (ILogin) metoda.Invoke(_app, parametry);
                    //}
                    //else
                    //{
                    //    log.Debug("Problem przy stworzeniu _loginu");
                    //}
                    //Kolejno: KP, KH, KHP, ST, FA, MAG, PK, PKXL, CRM, ANL, DET, BIU, SRW, ODB, KB, KBP, HAP
                    // ILogin Login([MarshalAs(UnmanagedType.Struct), In] object vUser,
                    //_login = _app.Login(ConfigBll.ERPLogin, ConfigBll.ERPHaslo, ConfigBll.OptimaNazwaFirmy, hPar[0], hPar[1], hPar[2], hPar[3], hPar[4], hPar[5], hPar[6], hPar[7], hPar[8], hPar[9], hPar[10], hPar[11], hPar[12], hPar[13], hPar[14], hPar[15], hPar[16]);
                    //_session = _login.CreateSession();
                }
                return _app;
            }
            set
            {
                _app = value;
            }
        }

        private  ILogin Login
        {
            get
            {
                if (App != null)
                {
                    return _login;
                }
                return null;
            }
            set
            {
                _login = value;
            }
        }

        public  AdoSession Sesja
        {
            get
            {
                if (App != null)
                {
                    return _session;
                }
                return null;
            }
        }


        public  IKontrahent DodajKontrahenta(long customerId, Klient c)
        {

            CDNHeal.Kontrahenci Kontrahenci = (CDNHeal.Kontrahenci)Sesja.CreateObject("CDN.Kontrahenci", null);
            object o = null;
            try
            {
                o = Kontrahenci["Knt_KntId=" + customerId + ""];
            }
            catch
            {
            }

            if (o == null && c.KlientNadrzednyId == (int?)null)
                o = Kontrahenci["Knt_Kod='" + c.Symbol + "'"];

            if (o != null)
            {
                return (IKontrahent)o;
            }

            throw new Exception("Brak zdefiniowanego kontrahenta o symbolu: " + c.Symbol);

        }
        private static  ILog log
        {
            get
            {
                return LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            }
        }

        private const string WNo_KursM = "WNo_KursM";
        private const string WNo_KursL = "WNo_KursL";
        private const string DataKursu = "WNo_TStamp";
        private Dictionary<string, List<KeyValuePair<string, object>>> PobierzKursyWalut(string erp_cs)
        {
            if (string.IsNullOrEmpty(erp_cs))
            {
                return new Dictionary<string, List<KeyValuePair<string, object>>>();
            }
            Dictionary<string, List<KeyValuePair<string, object>>> wynik = new Dictionary<string, List<KeyValuePair<string, object>>>();
            using (SqlConnection conn = new SqlConnection(erp_cs))
            {
                conn.Open();

                using (SqlCommand command = new SqlCommand("select top 3 wno.WNo_WNaID,wno.WNo_TStamp,WNo_KursL,wno.WNo_KursM, wna.WNa_Symbol from cdn.WalNotowania wno join cdn.WalNazwy wna on wna.WNa_WNaID=wno.WNo_WNaID order by WNo_TStamp desc;"))
                {
                    command.Connection = conn;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string symbol = DataHelper.dbs("WNa_Symbol", reader);
                            decimal KursM = DataHelper.dbd("WNo_KursM", reader);
                            decimal KursL = DataHelper.dbd("WNo_KursL", reader);
                            DateTime dataKursu = DataHelper.dbdt("WNo_TStamp", reader);
                            List<KeyValuePair<string, object>> listaKursow = new List<KeyValuePair<string, object>>();
                            listaKursow.Add(new KeyValuePair<string, object>(WNo_KursM, KursM));
                            listaKursow.Add(new KeyValuePair<string, object>(WNo_KursL, KursL));
                            listaKursow.Add(new KeyValuePair<string, object>(DataKursu, dataKursu));
                            wynik.Add(symbol,listaKursow);
                        }
                    }
                }

                conn.Close();
            }
            return wynik;
        }
        public  string DodanieDokumentu(ZamowienieSynchronizacja or, int magId, out int docId, out string docNum,  Klient klient, string erp_cs2,Dictionary<long,Model.Waluta> waluty)
        {
            log.DebugFormat("Dodawanie zamowinia o id: {0}",or.Id);
            long mainId = klient.KlientNadrzednyId == (int?)null ? or.KlientId : klient.KlientNadrzednyId.Value;
            long subId = klient.Id < 1 && klient.KlientNadrzednyId != (int?)null ? klient.KlientNadrzednyId.Value : klient.Id;

            IKontrahent KontrahentGlowny = DodajKontrahenta(mainId, klient);
            IKontrahent KontrahentOdbiorca = DodajKontrahenta(subId, klient);
           
            DefinicjeDokumentow definicjeDokumentow = (DefinicjeDokumentow)Sesja.CreateObject("CDN.DefinicjeDokumentow", null);
           
            CDNHlmn.DokumentyHaMag Faktury = (CDNHlmn.DokumentyHaMag)Sesja.CreateObject("CDN.DokumentyHaMag", null);
            CDNHlmn.IDokumentHaMag Faktura = (CDNHlmn.IDokumentHaMag)Faktury.AddNew(null);
            

            int rodzaj = ConfigBll.OptimaRodzajDokumentu;
            int typ = ConfigBll.OptimaTypDokumentu;
        
            Faktura.Rodzaj = rodzaj;
            Faktura.TypDokumentu = typ; // RO

            //Ustawiamy podmiot
            Faktura.Podmiot = KontrahentGlowny;
            Faktura.Odbiorca = KontrahentOdbiorca;
           
            //definicja dokumentu
            var definicja = (DefinicjaDokumentu)definicjeDokumentow["DDf_Symbol='" + ConfigBll.OptimaNazwaDokumentu + "'"];
            Object[] parameters;
            parameters = new Object[1];
            parameters[0] = definicja;
            Faktura.Numerator.GetType().InvokeMember("DefinicjaDokumentu", BindingFlags.SetProperty,
            null, Faktura.Numerator, parameters);

            //seria
            if (!string.IsNullOrEmpty(ConfigBll.OptimaSeriaDokumentu))
            {
                parameters[0] = ConfigBll.OptimaSeriaDokumentu;
                Faktura.Numerator.GetType().InvokeMember("Rejestr", BindingFlags.SetProperty, null, Faktura.Numerator, parameters);
            }
            //od ceny brutto
            Faktura.TypNB = 1;// or.NettoPrices ? 1 : 2; //2;
            Faktura.Bufor = 1;            //Ustawiamy bufor
            Faktura.DataDok = DateTime.Now; //Ustawiamy date
            if (or.TerminDostawy.HasValue)
                Faktura.DataTransportu = or.TerminDostawy.Value;
        
            Faktura.MagazynZrodlowyID = magId;
           
            //Dodajemy pozycje
            ICollection Pozycje = Faktura.Elementy;
            foreach (ZamowienieProdukt item in or.pozycje)
            {
                try
                {
                    CDNHlmn.IElementHaMag Pozycja = (CDNHlmn.IElementHaMag)Pozycje.AddNew(null);
                    Pozycja.TowarID = (int)item.ProduktIdBazowy;
                    Pozycja.Ilosc = Convert.ToDouble(item.Ilosc);
                    Pozycja.WartoscNettoWal = item.CenaNetto * item.Ilosc;

                    object[] parametry = { item.Jednostka };
                    Helper.SprawdzCzyFunkcjaIstnieje(ref Pozycja, "UstawJednostkeTowaru", parametry, null, CoSprawdzamyOptima.Metoda);

                    //try
                    //{
                    //    Pozycja.UstawJednostkeTowaru(item.jednostka);
                    //}
                    //catch (Exception ex)
                    //{
                    //    log.ErrorFormat("Nie udało ustawić się jednostki {0} dla produktu: {1}",item.jednostka,item.Symbol);
                    //    log.Error(ex);
                    //}
                    
                }
                catch(Exception ex)
                {
                    throw new Exception("Nie można dodać produktu o id do faktury: " + item.ProduktId + " w cenie wartość netto =" + (item.Ilosc * item.CenaNetto) + ". Upewnij się czy produkt jest w Optimie. Błąd: " + ex.Message);
                }
            }
            
            //dopasowanie płatności
            if (or.WalutaId != null && waluty[(long)or.WalutaId].WalutaErp != "PLN")
            {
                Faktura.WalutaSymbol = waluty[(long)or.WalutaId].WalutaErp;
            }

            Dictionary<string, List<KeyValuePair<string, object>>> kursyWalut = PobierzKursyWalut(erp_cs2);

            if (or.WalutaId != null && kursyWalut.ContainsKey(waluty[(long)or.WalutaId].WalutaErp))
            {
                double kL = double.Parse(kursyWalut[waluty[(long)or.WalutaId].WalutaErp].First(x => x.Key == WNo_KursL).Value.ToString());
                int kM = Int32.Parse(kursyWalut[waluty[(long)or.WalutaId].WalutaErp].First(x => x.Key == WNo_KursM).Value.ToString());
                Faktura.DataKur = DateTime.Parse(kursyWalut[waluty[(long)or.WalutaId].WalutaErp].First(x => x.Key == DataKursu).Value.ToString());
                Faktura.KursM = kM;
                Faktura.KursL = kL;
            }

            Faktura.NumerObcy = or.NumerZPlatformy;
            Faktura.Uwagi = or.Uwagi.Trim();
            //Faktura.Uwagi = (or.uwagi == null ? or.uwagi : or.uwagi.Replace("\\r\\n", "\r\n"));


            if (klient.KlientEu)
                Faktura.Export = 3;
           
            //zapisujemy
            Sesja.Save();

            docId = Faktura.ID;
            docNum = Faktura.NumerPelny;

            return "";
        }
    
        public  void Wylogowanie()
        {
            try
            {
                App.LoginOut();
                Login = null;
                App.UnlockApp();
            }
            catch
            {
            }
            App = null;
        }
        public bool Drukuj(int docId,string sciezka)
        {
            if (Sesja == null)
            {
                throw new Exception("Brak sesji zalogowanego użytkownika do Optimy");
            }

            if (Login == null)
            {
                throw new Exception("Brak zalogowanego użytkownika do Optimy");
            }
            Wydruk druk = new Wydruk();
            druk.GenRapNewReportInitObjects = "FakturaSpr";
            
            druk.FiltrAplikacji = "TrN_TRNID = " + docId;
            druk.Rodzaj = RodzajWydruku.RegularnyWydruk;
            druk.SprawdzRodzajTyp();
            druk.ID = ConfigBll.OptimaIdSzablonuWydrukuDoPdf;
            druk.Standardowy = true;
            if (druk.ID < 100)
            {
                druk.Standardowy = false;
            }
            druk.TrybCOM = false;
            druk.ZamknijOknoPoWykonaniu = true;
            //if (druk.ID < 100)
            //{
            //    druk.Typ = TypWydruku.GenRap;
            //}
            //else
            //{
            //    druk.Typ = TypWydruku.Crystal;
            //}
            druk.IloscKopii = 1;
            
            //bartek dodaje - eksperymenty . dziala bez tych wpisow ponizej tez
        //    druk.PoZaladowaniuDanychWydruku += druk_PoZaladowaniuDanychWydruku;
            druk.DokumentTyp = 302;
            druk.DomyslnyFormatEksportu = UrzadzenieWydrukuFormatEksportu.PortableDocFormat;
            druk.Autor = "SOLEX B2B";
            // druk.DokumentID = 1;
            druk.DomyslnyTypUrzadzenia = UrzadzenieWydrukuTyp.PlikEksp;
            // druk.IgnorujFiltrApp = false;

            //druk.GenRapNewReportInitObjects = "FakturaSpr";
            //druk.GenRapContextAttribute = "TrNEdycjaFS";
            //druk.GenRapReportType = 2;
            //druk.GenRapNewReportInitObjects = "FakturaSpr";

           // druk.FiltrDefinicji_Silnik = "";
          //  druk.FiltrWynikowy_Silnik = "";

            druk.DokumentID = docId;
            //druk.ParametryDynamiczne["CDN_APLIKACJA"] = new DynamicParameter("CDN_APLIKACJA", "testowy");

            //druk.ParametryDynamiczneDefinicji["CDN_APLIKACJA"] = new DynamicParameter("CDN_APLIKACJA", "testowy");

            //druk.UstawParametryWydrukuEvent += DrukOnUstawParametryWydrukuEvent;

            //druk.UstawParametrDynamiczny("CDN_APLIKACJA","543434");

          //  druk.UstawParametrDynamiczny("Identyfikator", 1);

            //druk.ParametryDynamiczne["CDN_APLIKACJA"].Source = druk.ParametryDynamiczneDefinicji["CDN_APLIKACJA"].Source =  "";
           // druk.ParametryDynamiczne["CDN_APLIKACJA"].Value = 
                
           // druk.ParametryDynamiczne["CDN_APLIKACJA"].ComplexValueCode =  "Wydrukowane z platformy hurtowej SOLEX B2B";
           // druk.ParametryDynamiczneDefinicji["CDN_APLIKACJA"].Value = druk.ParametryDynamiczneDefinicji["CDN_APLIKACJA"].ComplexValueCode = "Wydrukowane z platformy hurtowej SOLEX B2B";

          //  druk.ImportujParametryDynamiczneDefinicji();
          //  druk.ResetujParametry();

            //druk.LadujDaneStartowe();
            //druk.LadujDaneKompletne(false);
            //druk.Definicja =  druk.DefinicjaDB = "1";
            //druk.LadowanieDanych = true;

            Helper.SprawdzCzyFunkcjaIstnieje(ref druk, "NieSprawdzajUprawnien", null, true, CoSprawdzamyOptima.Properties);

            //var zmienneDynamiczne = new Dictionary<string, DynamicVariable>();
            var zmienneDynamiczne = new Dictionary<string, DynamicVariable>();
            object[] parametryStaraOptima = { "CDN_Aplikacja", "Wydrukowane z platformy hurtowej SOLEX B2B", DynamicVariableValueType.Text, null };
            object[] parametryNowaOptima = { "CDN_Aplikacja", "Wydrukowane z platformy hurtowej SOLEX B2B", DynamicVariableValueType.Text };

            DynamicVariable dv = new DynamicVariable();
            var konstruktor = dv.GetType().GetConstructor(new Type[]{typeof (string), typeof (string), typeof (DynamicVariableValueType)});
            if (konstruktor != null)
            {
                zmienneDynamiczne.Add("CDN_Aplikacja", (DynamicVariable) konstruktor.Invoke(parametryNowaOptima));
                //zmienneDynamiczne.Add("CDN_Aplikacja",new DynamicVariable("CDN_Aplikacja", "SOLEX", DynamicVariableValueType.Text));
            }
            else
            {
                konstruktor = dv.GetType().GetConstructor(new Type[] { typeof(string), typeof(string), typeof(DynamicVariableValueType), typeof(DynamicVariableValueRange) });
                if (konstruktor != null)
                {
                    zmienneDynamiczne.Add("CDN_Aplikacja", (DynamicVariable)konstruktor.Invoke(parametryStaraOptima));
                }
            }
            //if(DynamicVariable.GetType())


                //.FirstOrDefault(x => x.Name == nazwa && x.GetParameters().Count() == parametry.Count());
            //DynamicVariable dv = new DynamicVariable();
            
            //dv = Helper.StworzKonstruktor(parametryStaraOptima, parametryNowaOptima, dv);

            //zmienneDynamiczne.Add("CDN_Aplikacja",dv);
            // new DynamicVariable("CDN_Aplikacja", "SOLEX", DynamicVariableValueType.Text)
            //druk.Podgladaj();
            object[] parametry = { UrzadzenieWydrukuFormatEksportu.PortableDocFormat, sciezka, zmienneDynamiczne };
            Helper.SprawdzCzyFunkcjaIstnieje(ref druk, "EksportujDoPliku", parametry, null, CoSprawdzamyOptima.Metoda);
            //druk.EksportujDoPliku(UrzadzenieWydrukuFormatEksportu.PortableDocFormat, sciezka, zmienneDynamiczne);
            //Thread.Sleep(1 * 1000);
            return true;
        }
    }
}
