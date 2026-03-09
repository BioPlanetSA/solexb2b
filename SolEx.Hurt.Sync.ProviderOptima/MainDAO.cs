using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using CDNHeal;
using Optima.Common.Helpers;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Interfaces;

using System.IO;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.Sync;
using SolEx.Hurt.Model.CustomSearchCriteria;
using SolEx.Hurt.Model.Web;

namespace SolEx.Hurt.Sync.ProviderOptima
{
    public class MainDAO
    {
        private   log4net.ILog log
        {
            get
            {
                return log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            }
        }
        private const int grupyoffset = 100000;
        private const int markioffset = 10000;
        private const int producnetoffset = 0;
        private MainDataContext db;

      
        private Optima _optima = Optima.PobierzInstancje;
        public Optima OptimaInstancja
        {
            get { return _optima; }
            set { _optima = value; }
        }

        private IConfigSynchro _config;

        public MainDAO(IConfigSynchro config)
        {
            _config = config;
            db = new MainDataContext(_config.ERPcs);
            db.CommandTimeout = 90;
            OptimaInstancja.Operator = _config.ERPLogin;
            OptimaInstancja.Haslo = _config.ERPHaslo;
        }

        // Rodzaje kontrahentów
        // 0 - dostawca odbiorca
        // 1 - dostawca
        // 2 - odbiorca
        //
        // TODO
        // 1. odbiorcy
        // 2. grupy kontrahentów
        internal Dictionary<long, Klient> PobierzKlientow(List<Klient> klienciNaPlatformie, out Dictionary<Model.Adres, KlientAdres> adresy)
        {

            List<KntAtrybuty> wszystkieatrybuty = db.KntAtrybuties.ToList();
            var atrybuty = wszystkieatrybuty.DistinctBy(x => x.DefAtrybuty.DeA_Kod).Select(x => x.DefAtrybuty.DeA_Kod).ToList();
            //Dictionary<int,List<string>> atrybuty = new Dictionary<int, List<string>>();


            List<Rabaty> rabaty = db.Rabaties.ToList();
            List<KntOdbiorcy> odbiorcy = db.KntOdbiorcies.ToList();
            Dictionary<long, Klient> items = new Dictionary<long, Klient>();
            //List<Kontrahenci> q = db.Kontrahencis.Where(a => a.Knt_PodmiotTyp == 1).ToList();

            List<Kraje> kraje = PobierzKraje();
            List<Region> wojewodztwa = PobierzWojewodztwa();



            adresy = new Dictionary<Model.Adres, KlientAdres>();
            // operatorzy
            //string cs = _config.ERPcs2;
            //KonfiguracjaDataContext konf = new KonfiguracjaDataContext(cs);
            //var ops = konf.Operatorzies.Select(p => new { p.Ope_OpeID, p.Ope_Kod, p.Ope_Nazwisko, p.Ope_Opis, p.Ope_Email, p.Ope_Telefon }).ToList();
            bool czyIstniejeWidokPracownikow = DataHelper.CzyIstniejeTabela("PracownicyViews", _config.ERPcs);
            Dictionary<long, Klient> pracownicy = new Dictionary<long, Klient>();
            string doklej = string.Empty;
            bool czyJestTelefon = DataHelper.CzyKolumnaIstnieje("Operatorzy", "Ope_Telefon", _config.ERPcs2);
            if (czyJestTelefon)
            {
                doklej = ", Ope_Telefon";
            }
            string zapytanieOpertorzy =string.Format("select Ope_OpeID, Ope_Kod,Ope_Nazwisko, Ope_Opis,Ope_Email{0} from cdn.Operatorzy;",doklej);
            SqlCommand cmd = null;
            SqlDataReader rd=null;
            SqlConnection conn = null;
            Dictionary<long, Klient> slownikKlientow = new Dictionary<long, Klient>();
            try
            {
                conn = new SqlConnection(_config.ERPcs2);
                conn.Open();

                cmd = new SqlCommand(zapytanieOpertorzy, conn);
                rd = cmd.ExecuteReader();
                while (rd.Read())
                {
                    long Ope_OpeID = DataHelper.dbl("Ope_OpeID", rd);
                    string Ope_Kod = DataHelper.dbs("Ope_Kod", rd);
                    string Ope_Nazwisko = DataHelper.dbs("Ope_Nazwisko", rd);
                    string Ope_Email = DataHelper.dbs("Ope_Email", rd);
                    string Ope_Telefon = string.Empty;
                    if (czyJestTelefon)
                    {
                        Ope_Telefon = DataHelper.dbs("Ope_Telefon", rd);
                    }
                    Klient k = new Klient();
                    k.Id = Ope_OpeID;
                    k.Symbol = Ope_Kod;
                    k.Nazwa = Ope_Nazwisko;
                    k.Email = Ope_Email;
                    k.Telefon = Ope_Telefon;
                    slownikKlientow.Add(k.Id,k);
                }


                rd.Close();
                rd.Dispose();
                cmd.Dispose();

                
                if (czyIstniejeWidokPracownikow)
                {
                    string pracownicySQL = "select PracId, Kod, Imie, Nazwisko, EMail,Telefon1 from cdn.PracownicyView;";
                    cmd = new SqlCommand(pracownicySQL, conn);
                    rd = cmd.ExecuteReader();
                    while (rd.Read())
                    {
                        long PracId = DataHelper.dbl("PracId", rd);
                        string Kod = DataHelper.dbs("Kod", rd);
                        string Imie = DataHelper.dbs("Imie", rd);
                        string Nazwisko = DataHelper.dbs("Nazwisko", rd);
                        string EMail = DataHelper.dbs("EMail", rd);
                        string Telefon1 = DataHelper.dbs("Telefon1", rd);

                        Klient pracownik = new Klient();
                        pracownik.Id = PracId;
                        pracownik.Nazwa = string.Format("{0} {1}", Imie, Nazwisko);
                        pracownik.Symbol = Kod;
                        pracownik.Telefon = Telefon1;
                       
                        if (slownikKlientow.Values.Any(x => x.Email.Equals(EMail,StringComparison.InvariantCultureIgnoreCase)))
                        {
                          
                            LogiFormatki.PobierzInstancje.LogujInfo("Zdublowany adres email dla pracownika {0} email {1}, do adresu email dodano numer pracownika",pracownik.Nazwa,EMail);
                            EMail = PracId + "_" + EMail;
                        }
                        pracownik.Email = EMail.Trim().ToLower();
                        pracownicy.Add(pracownik.Id,pracownik);

                    }
                    rd.Close();
                    rd.Dispose();
                    cmd.Dispose();
                }
                conn.Close(); conn.Dispose();

                conn = new SqlConnection(_config.ERPcs);
                conn.Open();
                string dokleic = string.Empty;
                bool CzySaKory = DataHelper.CzyKolumnaIstnieje("Kontrahenci", "Knt_KorUlica", _config.ERPcs);
                
                if (CzySaKory)
                {
                    dokleic=", Knt_KorUlica, Knt_KorMiasto, Knt_KorKodPocztowy, Knt_KorKraj,Knt_KorNrDomu,Knt_KorNrLokalu";
                }
                string kontrahenciSQL = string.Format("select Knt_KntId, Knt_Miasto, Knt_Kraj, Knt_KodPocztowy, Knt_Ulica, Knt_NrDomu, Knt_NrLokalu, Knt_Wojewodztwo, Knt_Email, Knt_Ceny, Knt_OpiekunTyp, Knt_OpiekunId, Knt_Nieaktywny, Knt_Export, Knt_Nazwa1, Knt_Nazwa2, Knt_Nip,Knt_NipKraj, Knt_Telefon1, Knt_Kod, Knt_LimitKredytu, Knt_LimitKredytuWykorzystany{0} from cdn.Kontrahenci where Knt_PodmiotTyp=1;", dokleic);
                cmd = new SqlCommand(kontrahenciSQL, conn);
                rd = cmd.ExecuteReader();
                while (rd.Read())
                {
                    int Knt_KntId = DataHelper.dbi("Knt_KntId", rd);
                    string Knt_Miasto = DataHelper.dbs("Knt_Miasto", rd);
                    string Knt_Kraj = DataHelper.dbs("Knt_Kraj", rd);
                    string Knt_KodPocztowy = DataHelper.dbs("Knt_KodPocztowy", rd);
                    string Knt_Ulica = DataHelper.dbs("Knt_Ulica", rd);
                    string Knt_NrDomu = DataHelper.dbs("Knt_NrDomu", rd);
                    string Knt_NrLokalu = DataHelper.dbs("Knt_NrLokalu", rd);
                    string Knt_Wojewodztwo = DataHelper.dbs("Knt_Wojewodztwo", rd);
                    string Knt_Email = DataHelper.dbs("Knt_Email", rd);
                    int Knt_Ceny = DataHelper.dbi("Knt_Ceny", rd);
                    int? Knt_OpiekunTyp = DataHelper.dbin("Knt_OpiekunTyp", rd);
                    int? Knt_OpiekunId = DataHelper.dbin("Knt_OpiekunId", rd);
                    int Knt_Nieaktywny = DataHelper.dbi("Knt_Nieaktywny", rd);
                    int Knt_Export = DataHelper.dbi("Knt_Export", rd);
                    string Knt_Nazwa1 = DataHelper.dbs("Knt_Nazwa1", rd);
                    string Knt_Nazwa2 = DataHelper.dbs("Knt_Nazwa2", rd);
                    string Knt_Nip = DataHelper.dbs("Knt_Nip", rd);
                    string Knt_NipKraj = DataHelper.dbs("Knt_NipKraj", rd);
                    string Knt_Telefon1 = DataHelper.dbs("Knt_Telefon1", rd);
                    string Knt_Kod = DataHelper.dbs("Knt_Kod", rd);
                    decimal Knt_LimitKredytu = DataHelper.dbd("Knt_LimitKredytu", rd);
                    decimal Knt_LimitKredytuWykorzystany = DataHelper.dbd("Knt_LimitKredytuWykorzystany", rd);
                    string Knt_KorUlica=null;
                    string Knt_KorMiasto = null;
                    string Knt_KorKodPocztowy = null;
                    string Knt_KorKraj = null;
                    string Knt_KorNrDomu = null;
                    string Knt_KorNrLokalu = null;

                    if (CzySaKory)
                    {
                        Knt_KorUlica = DataHelper.dbs("Knt_KorUlica", rd);
                        Knt_KorMiasto = DataHelper.dbs("Knt_KorMiasto", rd);
                        Knt_KorKodPocztowy = DataHelper.dbs("Knt_KorKodPocztowy", rd);
                        Knt_KorKraj = DataHelper.dbs("Knt_KorKraj", rd);
                        Knt_KorNrDomu = DataHelper.dbs("Knt_KorNrDomu", rd);
                        Knt_KorNrLokalu = DataHelper.dbs("Knt_KorNrLokalu", rd);
                    }

                    List<int> cechyklienta = wszystkieatrybuty.Where(a => a.KnA_PodmiotId == Knt_KntId && a.KnA_PodmiotTyp == 1).Select(a=>a.KnA_DeAId).ToList();
                    //List<KntAtrybuty> cechyklienta = wszystkieatrybuty.Where(a => a.KnA_PodmiotId == Knt_KntId).ToList();
                    //int aaaaa = cechyklienta.Count();
                    //var cechyKlienta = cechyklienta.Where();

                    Rabaty ra = rabaty.FirstOrDefault(r => r.Rab_PodmiotTyp == 1 && r.Rab_Typ == 2 && r.Rab_PodmiotId == Knt_KntId);

                    KntOdbiorcy od = odbiorcy.FirstOrDefault(a => a.Odb_OdbKntID == Knt_KntId);

                    KntAtrybuty f =
                        wszystkieatrybuty.FirstOrDefault(
                            d1 =>
                                d1.KnA_PodmiotTyp == 1 &&
                                d1.DefAtrybuty.DeA_Kod == _config.B2bFaktoring &&
                            d1.KnA_PodmiotId == Knt_KntId);


                    Model.Adres adresKorespondencyjny = null;
                    Model.Adres it = new Model.Adres();
                    it.Id = Knt_KntId;
                    it.Miasto = Knt_Miasto;
                    it.AutorId = Knt_KntId;
                    if (!string.IsNullOrEmpty(Knt_Kraj) && kraje.Any(a => a.Nazwa.ToLower() == Knt_Kraj.ToLower()))
                        it.KrajId = WygenerujIdDlaStringa(Knt_Kraj);

                    it.KodPocztowy = Knt_KodPocztowy;
                    it.UlicaNr = Knt_Ulica + " " + Knt_NrDomu +
                                 (string.IsNullOrEmpty(Knt_NrLokalu) || string.IsNullOrEmpty(Knt_Ulica) ? "" : "/") +
                                 Knt_NrLokalu;
                   // it.KlientId = Knt_KntId;

                    if (!string.IsNullOrEmpty(Knt_Wojewodztwo) && wojewodztwa.Any(a => a.Nazwa == Knt_Wojewodztwo))
                        it.RegionId = WygenerujIdDlaStringa(Knt_Wojewodztwo);



                    //dodatkowe adresy dla klienta
                    if (!string.IsNullOrEmpty(Knt_KorUlica) && !string.IsNullOrEmpty(Knt_KorMiasto))
                    {

                        adresKorespondencyjny = new Model.Adres();
                        adresKorespondencyjny.Miasto = Knt_KorMiasto;
                        adresKorespondencyjny.KodPocztowy = Knt_KorKodPocztowy ?? "";

                        if (!string.IsNullOrEmpty(Knt_KorKraj) && kraje.Any(a => a.Nazwa == Knt_KorKraj))
                            adresKorespondencyjny.KrajId = WygenerujIdDlaStringa(Knt_KorKraj);

                        adresKorespondencyjny.UlicaNr = Knt_KorUlica + " " + Knt_KorNrDomu +
                                                        (string.IsNullOrEmpty(Knt_KorNrLokalu) || string.IsNullOrEmpty(Knt_KorUlica) ? "" : "/") +
                                                        Knt_KorNrLokalu;
                       // adresKorespondencyjny.KlientId = Knt_KntId;

                        adresKorespondencyjny.Id = Knt_KntId + 1000000;  //przesuniecie adresow dla KORESPONDENCYJNEGO
                    }

                    Klient item = new Klient(Knt_KntId);
                    item.Aktywny = Knt_Nieaktywny == 0;
                    item.KlientEu = Knt_Export == 3;
                    item.Eksport = Knt_Export == 1;
                    item.Id = Knt_KntId;
                    item.Nazwa = Knt_Nazwa1 + " " + Knt_Nazwa2;
                    item.Nip = string.Format("{0} {1}", Knt_NipKraj, Knt_Nip).Trim();
                    item.KlientNadrzednyId = od != null ? od.Odb_KntOdbID : null;
                    item.Telefon = Knt_Telefon1;
                    item.Symbol = Knt_Kod;
                    item.Rabat = ra == null ? 0 : ra.Rab_Rabat;
                   // item.faktoring = (f != null && f.KnA_WartoscTxt != null) && (f.KnA_WartoscTxt.ToUpper() == "TAK");
                    item.LimitKredytu = Knt_LimitKredytu;
                    

                    //distinct na KOD atrybutu - minus jest taki ze tylko jedna wartosc atrybutu bedzie wyslana - troche kiepsko
                    //musi byc taki dziwny distinc dlatego ze w milu zdazylo sie 2 razy ten sam atrybut dziwnie jest w sqlu
                    //.DistinctBy(x=> x.DefAtrybuty.DeA_Kod + "|" + x.KnA_WartoscTxt)
                    Dictionary<string, string> pars = new Dictionary<string, string>();

                    foreach (var kntAtrybuty in atrybuty)
                    {
                        var wartosc = wszystkieatrybuty.FirstOrDefault(x => x.KnA_PodmiotId == item.Id && x.DefAtrybuty.DeA_Kod == kntAtrybuty);
                        string a = string.Empty;
                        if (wartosc != null && !string.IsNullOrEmpty(wartosc.KnA_WartoscTxt))
                        {
                            a = wartosc.KnA_WartoscTxt;
                        }
                        else
                        {
                            a = "";
                        }
                        pars.Add(kntAtrybuty, a);
                    }
                    pars.Add("Knt_LimitKredytu", Knt_LimitKredytu.ToString());
                    //string.Format("select distinct da.DeA_Kod, wartosc_Cechy = (Select KnA_WartoscTxt from cdn.KntAtrybuty where KnA_PodmiotId={0} and KnA_DeAId=da.DeA_DeAId) from cdn.KntAtrybuty kn join cdn.DefAtrybuty da on kn.KnA_DeAId=da.DeA_DeAId;", Knt_KntId);




                    item.IloscWykorzystanegoKredytu = Knt_LimitKredytuWykorzystany;
                    //_config.SynchronizacjaPobierzPoleHasloZrodlowe(item, "", pars);
                    _config.SynchronizacjaPobierzPoleHasloZrodlowe(item, "", pars);
                    if (_config.EksportTylkoKontZHaslem && string.IsNullOrEmpty(item.HasloZrodlowe))
                    {
                        continue;
                    }

                    _config.SynchronizacjaPobierzDostepneMagazyny(item, "", pars);
                    _config.SynchronizacjaPobierzPoleJezyk(item, "", pars);
                    _config.SynchronizacjaPobierzPoleTekst1(item, "", pars);
                    _config.SynchronizacjaPobierzPoleTekst2(item, "", pars);
                    _config.SynchronizacjaPobierzPoleTekst3(item, "", pars);
                    _config.SynchronizacjaPobierzPoleTekst4(item, "", pars);
                    _config.SynchronizacjaPobierzPoleTekst5(item, "", pars);
                    _config.SynchronizacjaPobierzPoleMagazynDomyslny(item, "", pars);
                    _config.SynchronizacjaPobierzPoleIndywidualnaStawaVat(item, "", pars, item.KlientEu, true);
                    _config.SynchronizacjaPobierzPoleDomyslnaWaluta(item, "", "", pars);
                    _config.SynchronizacjaPobierzPoleBlokadaZamowien(item, "", pars);
                    _config.SynchronizacjaPobierzMinimalnaWartoscZamowienia(item, "", pars);
                    _config.SynchronizacjaPobierzPoleOpiekun(item, "", pars);
                    _config.SynchronizacjaPobierzPoleDrugiOpiekun(item, "", pars);
                    _config.SynchronizacjaPobierzPolePrzedstawiciel(item, "", pars);
                    _config.SynchronizacjaPobierzPoleEmail(item, Knt_Email);
                    _config.SynchronizacjaUstawPoziomCeny(item, Knt_Ceny != 0 ? Knt_Ceny : (int?)null);
                    _config.SynchronizacjaPobierzPoleSkype(item, "", pars);
                    _config.SynchronizacjaPobierzPoleGaduGadu(item, "", pars);
                    _config.SynchronizacjaPobierzPoleKlientNadrzedny(item, "", pars);
                    _config.SynchronizacjaPobierzKredytWykorzystano(item, "Knt_LimitKredytuWykorzystany", pars);
                    _config.SynchronizacjaPobierzKredytLimit(item, "Knt_LimitKredytu", pars);
                    _config.SynchronizacjaPobierzKredytPozostalo(item, "", pars);
                    adresy.Add(it, new KlientAdres(){AdresId = it.Id,KlientId = Knt_KntId, TypAdresu = TypAdresu.Glowny}); 
                    
                    if (adresKorespondencyjny != null)
                    {
                        adresy.Add(adresKorespondencyjny, new KlientAdres() { AdresId = it.Id, KlientId = Knt_KntId, TypAdresu = TypAdresu.Korespondencyjny });
                    }


                    //string opiekunowie =
                    //    string.Format(
                    //        "select Ope_OpeID, Ope_Kod,Ope_Nazwisko, Ope_Opis,Ope_Email{0} from cdn.Operatorzy where Ope_OpeID={1};",doklej, v.Knt_OpiekunId);

                    if (Knt_OpiekunTyp == 8)
                    {
                        //var op = ops.FirstOrDefault(p => p.Ope_OpeID == v.Knt_OpiekunId);
                        var op = slownikKlientow.FirstOrDefault(p => p.Key == Knt_OpiekunId).Value;

                        if (op != null)
                        {
                            long opiekunID = op.Id + 100000;
                            Klient opiekun = klienciNaPlatformie.FirstOrDefault(a => a.Id == opiekunID);

                            if (opiekun == null)
                            {
                                opiekun = new Klient { Id = opiekunID, HasloZrodlowe = Tools.PobierzInstancje.WygenerujString(8) };
                            }
                            else opiekun = new Klient(opiekun);
                            opiekun.Aktywny = true;
                            opiekun.Nazwa = op.Nazwa;
                            opiekun.Symbol = op.Symbol;
                            opiekun.Telefon = op.Telefon;
                            opiekun.Email = op.Email.Trim().ToLower();
                            if (!opiekun.Role.Contains(RoleType.Klient))
                            {
                                opiekun.Role.Add(RoleType.Klient);
                            }
                            if (!opiekun.Role.Contains(RoleType.Pracownik))
                            {
                                opiekun.Role.Add(RoleType.Pracownik);
                            }
                           
                            if (!items.ContainsKey(opiekun.Id))
                                items.Add(opiekun.Id, opiekun);

                            item.OpiekunId = opiekun.Id;
                        }
                    }
                    //pracownik albo właściciel/wspólnik
                    else if (Knt_OpiekunTyp == 3 && czyIstniejeWidokPracownikow)
                    {
                        var op = pracownicy.FirstOrDefault(a => a.Key == Knt_OpiekunId).Value;

                        if (op != null)
                        {
                            long opiekunID = op.Id + 200000;
                            Klient opiekun = klienciNaPlatformie.FirstOrDefault(a => a.Id == opiekunID);

                            if (opiekun == null)
                            {
                                opiekun = new Klient { Id = opiekunID, HasloZrodlowe = Tools.PobierzInstancje.WygenerujString(8) };
                                log.Debug("Nowy opiekun o mailu " + opiekun.Email);
                            }
                            else opiekun = new Klient(opiekun);
                            opiekun.Aktywny = true;
                            opiekun.Nazwa = op.Nazwa;
                            opiekun.Symbol = op.Symbol;
                            opiekun.Telefon = op.Telefon;
                            opiekun.Email = op.Email;
                            opiekun.Role.Add(RoleType.Klient);
                            opiekun.Role.Add(RoleType.Pracownik);
                            opiekun.Role.Add(RoleType.Przedstawiciel);
                            if (!items.ContainsKey(opiekun.Id))
                            {
                                items.Add(opiekun.Id, opiekun);
                                log.Debug(string.Format("Dodano klienta o mailu {0} haslo {1}", opiekun.Email, opiekun.HasloZrodlowe));
                            }

                            item.OpiekunId = opiekun.Id;
                        }
                    }
                    items.Add(item.Id, item);

                 }

            }
            finally
            {
                if (rd != null) { rd.Close(); rd.Dispose(); }
                if (cmd != null) cmd.Dispose();
                if (conn != null) { conn.Close(); conn.Dispose(); }
            }

            
           

            LogiFormatki.PobierzInstancje.LogujInfo("Klientów: " + items.Count);
            return items;
        }
        private int presuniestatuskat = 10000;
        internal List<StatusZamowienia> PobierzStatusyDokumnetow()
        {
            string nazwaDokumentu = _config.OptimaNazwaDokumentu;
            int docType = _config.OptimaTypDokumentu;
            string sql = string.Format("select DDf_DDfID, DDf_Symbol, DDf_Nazwa from cdn.DokDefinicje where DDf_Klasa={0} and DDf_Symbol!='{1}';", docType, nazwaDokumentu);
            List<StatusZamowienia> items = new List<StatusZamowienia>();
            SqlConnection conn = null;
            SqlCommand cmd = null;
            SqlDataReader rd = null;
            try
            {

                conn = new SqlConnection(_config.ERPcs);
                conn.Open();

                cmd = new SqlCommand(sql, conn);
                rd = cmd.ExecuteReader();
                while (rd.Read())//pobieranie  produktów
                {
                    string DDf_Symbol = DataHelper.dbs("DDf_Symbol", rd);
                    string DDf_Nazwa = DataHelper.dbs("DDf_Nazwa", rd);
                    int id = DataHelper.dbi("DDf_DDfID", rd)+presuniestatuskat;

                    items.Add(new StatusZamowienia { Id = id, Symbol = DDf_Symbol, Nazwa = DDf_Nazwa });
                    
                }
                rd.Close();
                rd.Dispose();
                cmd.Dispose();
               
            }
            finally
            {
                if (rd != null) { rd.Close(); rd.Dispose(); }
                if (cmd != null) cmd.Dispose();
                if (conn != null) { conn.Close(); conn.Dispose(); }
            }
            return items;
        }


        internal Dictionary<HistoriaDokumentu, List<HistoriaDokumentuProdukt>> GetDocuments(Dictionary<int, long> dokumentyNaPlatformie, DateTime startDate, List<Klient> klienciNaPlatformie)
        {
            List<HistoriaDokumentu> dokDoWywaleniaBrakJednostki = new List<HistoriaDokumentu>();
            Dictionary<string, Jednostka> jednostkiDoDodania = new Dictionary<string, Jednostka>();
            var jednostkiSlownik = APIWywolania.PobierzInstancje.PobierzJednostki().Values.Where(x => x.Aktywna).ToDictionary(x => x.Nazwa, x => x.Id);
            Dictionary<HistoriaDokumentu, List<HistoriaDokumentuProdukt>> items = new Dictionary<HistoriaDokumentu, List<HistoriaDokumentuProdukt>>();
            List<long> klienci = klienciNaPlatformie.Select(a => a.Id).ToList();
            int wieloscpaczkidokumentow = _config.MaksimumDokumentowWPaczce;

            int docType = _config.OptimaTypDokumentu;
            string nazwaDok = _config.OptimaNazwaDokumentu;
            db.CommandTimeout = 180;
            StringBuilder sb = new StringBuilder();
            foreach (var a in klienci)
            {
                sb.AppendFormat("{0},", a);
            }
            int index = sb.ToString().LastIndexOf(',');
            string zapytanie =
                string.Format(
                    "select p.TrN_TrNID, p.TrN_DDfId, dd.DDf_Symbol, p.TrN_DataDok, p.TrN_NumerPelny, p.TrN_PodID,p.TrN_FPlId, " +
                    "p.TrN_Waluta, p.TrN_DataWys,p.TrN_RazemBruttoWal,p.TrN_RazemNettoWal,p.TrN_RazemNettoWal,p.TrN_Opis," +
                    "p.TrN_StatusInt,p.TrN_ZwroconoCalaIlosc,p.TrN_OdbID" +
                  //  ",dr.TrR_FaId, PelnaNazwa=(select TrN_NumerPelny from cdn.TraNag where dr.TrR_FaId=TrN_TrNID)" +
                    " from cdn.TraNag p " +
                 //   "left join cdn.TraNagRelacje dr on dr.TrR_TrNId=p.TrN_TrNID  and dr.TrR_FaTyp=302 " +
                    "left join cdn.DokDefinicje dd on p.TrN_DDfId = dd.DDf_DDfID where p.TrN_Bufor=0 and p.TrN_Anulowany = 0 and p.TrN_PodID is not null and p.TrN_PodmiotTyp = 1 " +
                    "and p.TrN_TypDokumentu={0} and p.TrN_PodID in({1}) and p.TrN_DataDok >= '{2}';", docType, sb.ToString().Substring(0, index), startDate);
            
            SqlConnection conn = null;
            SqlCommand cmd = null;
            SqlDataReader rd = null;
            try
            {
                conn = new SqlConnection(_config.ERPcs);
                conn.Open();
                cmd = new SqlCommand(zapytanie, conn);
                rd = cmd.ExecuteReader();
                while (rd.Read())
                {
                    int TrN_TrNID = DataHelper.dbi("TrN_TrNID", rd);
                    DateTime TrN_DataDok = DataHelper.dbdt("TrN_DataDok", rd);
                    string TrN_NumerPelny = DataHelper.dbs("TrN_NumerPelny", rd);
                    int? TrN_PodID = DataHelper.dbin("TrN_PodID", rd);
                    string TrN_Waluta = DataHelper.dbs("TrN_Waluta", rd);
                    DateTime TrN_DataWys = DataHelper.dbdt("TrN_DataWys", rd);
                    decimal TrN_RazemBruttoWal = DataHelper.dbd("TrN_RazemBruttoWal", rd);
                    decimal TrN_RazemNettoWal = DataHelper.dbd("TrN_RazemNettoWal", rd);
                    string TrN_Opis = DataHelper.dbs("TrN_Opis", rd);
                    int TrN_StatusInt = DataHelper.dbi("TrN_StatusInt", rd);
                    int? TrN_OdbID = DataHelper.dbin("TrN_OdbID", rd);
                    int? TrR_FaId = null;// DataHelper.dbin("TrR_FaId", rd);
                    string PelnaNazwa = null;// DataHelper.dbs("PelnaNazwa", rd);
                    int TrN_DDfId = DataHelper.dbi("TrN_DDfId", rd)+presuniestatuskat;
                    string DDf_Symbol = DataHelper.dbs("DDf_Symbol", rd);
                    string waluta = string.IsNullOrEmpty(TrN_Waluta) ? "PLN" : TrN_Waluta;
                    long? walutaId =  (string.IsNullOrEmpty(waluta) ? "PLN" : waluta).ToLower().WygenerujIDObiektuSHAWersjaLong();
                    HistoriaDokumentu d = new HistoriaDokumentu(TrN_TrNID,
                       (long)TrN_PodID.Value,
                       RodzajDokumentu.Zamowienie,
                       TrN_NumerPelny,
                       TrN_DataDok,
                       TrN_StatusInt != 0,
                       TrR_FaId,
                       PelnaNazwa,
                       TrN_Opis,
                       TrN_RazemBruttoWal,
                       TrN_RazemNettoWal,
                       TrN_RazemBruttoWal,
                       walutaId,
                       "",
                       TrN_DataWys,
                      
                       true);
                    if (DDf_Symbol != nazwaDok)
                    {
                        d.StatusId = TrN_DDfId;
                    }

                    if (TrN_OdbID.HasValue && klienci.Contains(TrN_OdbID.Value))
                        d.OdbiorcaId = TrN_OdbID;

                    long hash = Tools.PobierzInstancje.PoliczHashDokumentu(d);
                    if (!dokumentyNaPlatformie.Any(p => p.Key == d.Id && p.Value == hash))
                    {
                        items.Add(d,new List<HistoriaDokumentuProdukt>());
                    }
                    if (items.Count > wieloscpaczkidokumentow)
                    {
                        break;
                    }
                }
            }
            finally
            {
                if (rd != null) { rd.Close(); rd.Dispose(); }
                if (cmd != null) cmd.Dispose();
                if (conn != null) { conn.Close(); conn.Dispose(); }
            }
            if (items.Count < wieloscpaczkidokumentow)
            {

                string zapytaniePrzeterminowane =
                    string.Format(
                        "select nalezne=p.TrN_RazemBruttoWal-roz.zaplacone,p.TrN_TrNID, p.TrN_DataDok, p.TrN_NumerPelny,p.TrN_PodID, f.FPl_Nazwa,p.TrN_Waluta,p.TrN_Termin,p.TrN_RazemBruttoWal,p.TrN_RazemNettoWal,p.TrN_Opis, p.TrN_OdbID," +
                        //   " dr.TrR_FaId, PelnaNazwa=(select TrN_NumerPelny from cdn.TraNag where dr.TrR_FaId=TrN_TrNID)  ," +
                        " Opoznienie = datediff(d, GETDATE(),  p.TrN_Termin)" +
                        "from cdn.TraNag p left join cdn.BnkZdarzenia bz on bz.BZd_DokumentID=p.TrN_TrNID left join cdn.FormyPlatnosci f on f.FPl_FPlId=p.TrN_FPlId " +
                        //    "left join cdn.TraNagRelacje dr on TrR_TrNId=p.TrN_TrNID and dr.TrR_FaTyp={2}  " +
                        " left join (select BZd_DokumentID, zaplacone=sum(BZd_KwotaRoz) from  cdn.BnkZdarzenia group by BZd_DokumentID) roz on roz.BZd_DokumentID=p.TrN_TrNID" +
                        " where p.TrN_Bufor=0 and p.TrN_Anulowany = 0 and TrN_PodID is not null and p.TrN_PodmiotTyp = 1 and p.TrN_TypDokumentu = 302 and p.TrN_DataDok>='{0}' and bz.BZd_DokumentTyp=1 and bz.BZd_Rozliczono != 2 and TrN_PodID in ({1})",
                        startDate, sb.ToString().Substring(0, index), docType);

                try
                {
                    conn = new SqlConnection(_config.ERPcs);
                    conn.Open();
                    cmd = new SqlCommand(zapytaniePrzeterminowane, conn);
                    rd = cmd.ExecuteReader();
                    while (rd.Read())
                    {
                        int TrN_TrNID = DataHelper.dbi("TrN_TrNID", rd);
                        DateTime TrN_DataDok = DataHelper.dbdt("TrN_DataDok", rd);
                        string TrN_NumerPelny = DataHelper.dbs("TrN_NumerPelny", rd);
                        int? TrN_PodID = DataHelper.dbin("TrN_PodID", rd);
                        string FPl_Nazwa = DataHelper.dbs("FPl_Nazwa", rd);
                        string TrN_Waluta = DataHelper.dbs("TrN_Waluta", rd);
                        DateTime TrN_Termin = DataHelper.dbdt("TrN_Termin", rd);
                        decimal nalezne = DataHelper.dbd("nalezne", rd);
                        decimal TrN_RazemBruttoWal = DataHelper.dbd("TrN_RazemBruttoWal", rd);
                        decimal TrN_RazemNettoWal = DataHelper.dbd("TrN_RazemNettoWal", rd);
                        string TrN_Opis = DataHelper.dbs("TrN_Opis", rd);
                        int? TrN_OdbID = DataHelper.dbin("TrN_OdbID", rd);
                        string PelnaNazwa = null; // DataHelper.dbs("PelnaNazwa", rd);


                        string waluta = string.IsNullOrEmpty(TrN_Waluta) ? "PLN" : TrN_Waluta;
                        long? walutaId =
                            (string.IsNullOrEmpty(waluta) ? "PLN" : waluta).ToLower().WygenerujIDObiektuSHAWersjaLong();


                        HistoriaDokumentu d = new HistoriaDokumentu(TrN_TrNID, (long)TrN_PodID.Value,
                            RodzajDokumentu.Faktura, TrN_NumerPelny,
                            TrN_DataDok
                            , false, TrN_TrNID,
                            PelnaNazwa
                            , TrN_Opis, nalezne,
                            TrN_RazemNettoWal
                            , TrN_RazemBruttoWal,
                            walutaId, FPl_Nazwa,
                            TrN_Termin, true);

                        if (TrN_OdbID.HasValue && klienci.Contains(TrN_OdbID.Value))
                            d.OdbiorcaId = TrN_OdbID;
                        long hash = Tools.PobierzInstancje.PoliczHashDokumentu(d);
                        if (!dokumentyNaPlatformie.Any(p => p.Key == d.Id && p.Value == hash))
                        {
                            items.Add(d, new List<HistoriaDokumentuProdukt>());
                        }
                        if (items.Count > wieloscpaczkidokumentow)
                        {
                            break;
                        }
                    }
                }

                finally
                {
                    if (rd != null)
                    {
                        rd.Close();
                        rd.Dispose();
                    }
                    if (cmd != null) cmd.Dispose();
                    if (conn != null)
                    {
                        conn.Close();
                        conn.Dispose();
                    }
                }
            }

            if (items.Count < wieloscpaczkidokumentow)
            {
                string zapytanieZrealizowane = string.Format(
                    "select p.TrN_TrNID, p.TrN_DataDok, p.TrN_NumerPelny,p.TrN_PodID, f.FPl_Nazwa,p.TrN_Waluta,p.TrN_Termin,p.TrN_RazemBruttoWal,p.TrN_RazemNettoWal,p.TrN_Opis," +
                    " p.TrN_OdbID " +
                    //  " ,dr.TrR_FaId,PelnaNazwa=(select TrN_NumerPelny from cdn.TraNag where dr.TrR_FaId=TrN_TrNID) " +
                    "from cdn.TraNag p left join cdn.BnkZdarzenia bz on bz.BZd_DokumentID=p.TrN_TrNID left join cdn.FormyPlatnosci f on f.FPl_FPlId=p.TrN_FPlId " +
                    //  "left join cdn.TraNagRelacje dr on TrR_TrNId=p.TrN_TrNID and dr.TrR_FaTyp={2}" +
                    " where p.TrN_Bufor=0 and p.TrN_Anulowany = 0 and TrN_PodID is not null and p.TrN_PodmiotTyp = 1 and p.TrN_TypDokumentu = 302 and p.TrN_DataDok>='{0}' and bz.BZd_DokumentTyp=1 and bz.BZd_Rozliczono = 2 and TrN_PodID in ({1})",
                    startDate, sb.ToString().Substring(0, index), docType);
                try
                {
                    conn = new SqlConnection(_config.ERPcs);
                    conn.Open();
                    cmd = new SqlCommand(zapytanieZrealizowane, conn);

                    rd = cmd.ExecuteReader();
                    while (rd.Read())
                    {
                        int TrN_TrNID = DataHelper.dbi("TrN_TrNID", rd);
                        DateTime TrN_DataDok = DataHelper.dbdt("TrN_DataDok", rd);
                        string TrN_NumerPelny = DataHelper.dbs("TrN_NumerPelny", rd);
                        int? TrN_PodID = DataHelper.dbin("TrN_PodID", rd);
                        string FPl_Nazwa = DataHelper.dbs("FPl_Nazwa", rd);
                        string TrN_Waluta = DataHelper.dbs("TrN_Waluta", rd);
                        DateTime TrN_Termin = DataHelper.dbdt("TrN_Termin", rd);
                        decimal TrN_RazemBruttoWal = DataHelper.dbd("TrN_RazemBruttoWal", rd);
                        decimal TrN_RazemNettoWal = DataHelper.dbd("TrN_RazemNettoWal", rd);
                        string TrN_Opis = DataHelper.dbs("TrN_Opis", rd);
                        int? TrN_OdbID = DataHelper.dbin("TrN_OdbID", rd);
                        string PelnaNazwa = null; // DataHelper.dbs("PelnaNazwa", rd);

                        string waluta = string.IsNullOrEmpty(TrN_Waluta) ? "PLN" : TrN_Waluta;



                        HistoriaDokumentu d = new HistoriaDokumentu(TrN_TrNID, (long)TrN_PodID.Value,
                            RodzajDokumentu.Faktura, TrN_NumerPelny,
                            TrN_DataDok
                            , true, TrN_TrNID,
                            PelnaNazwa
                            , TrN_Opis, 0, TrN_RazemNettoWal
                            , TrN_RazemBruttoWal,
                            string.IsNullOrEmpty(waluta)
                                ? ("PLN").ToLower().WygenerujIDObiektuSHAWersjaLong()
                                : waluta.ToLower().WygenerujIDObiektuSHAWersjaLong()
                            , FPl_Nazwa,
                            TrN_Termin, true);
                        if (TrN_OdbID.HasValue && klienci.Contains(TrN_OdbID.Value))
                            d.OdbiorcaId = TrN_OdbID;

                        long hash = Tools.PobierzInstancje.PoliczHashDokumentu(d);
                        if (!dokumentyNaPlatformie.Any(p => p.Key == d.Id && p.Value == hash))
                        {
                            items.Add(d, new List<HistoriaDokumentuProdukt>());
                        }
                        if (items.Count > wieloscpaczkidokumentow)
                        {
                            break;
                        }
                    }
                }
                finally
                {
                    if (rd != null)
                    {
                        rd.Close();
                        rd.Dispose();
                    }
                    if (cmd != null) cmd.Dispose();
                    if (conn != null)
                    {
                        conn.Close();
                        conn.Dispose();
                    }
                }

            }

            foreach (var hd in items)
            {
                var pr = db.TraElems.Where(p => p.TrE_TrNId == hd.Key.Id).Select(p => new
                  {
                      netto = p.TrE_Cena0WD,
                      brutto = decimal.Round(p.TrE_Cena0 * (1 + p.TrE_Stawka / 100), 2),
                      p.TrE_TrNId,
                      p.TrE_TrEID,
                      TrE_Ilosc = (p.TrE_Ilosc * p.TrE_JMPrzelicznikM) / p.TrE_JMPrzelicznikL,
                      p.TrE_Jm,
                      p.Towary.Twr_Nazwa,
                      Twr_Kod = p.Towary.Twr_Kod,
                      p.TrE_Stawka,
                      p.TrE_WartoscBruttoWal,
                      p.TrE_WartoscNettoWal,
                      wartosc_vat = p.TrE_WartoscBruttoWal - p.TrE_WartoscNettoWal,
                      p.TrE_Rabat,p.TrE_TwrId,
                     nettoporabacie = p.TrE_KursL == 1 ? p.TrE_CenaT: p.TrE_CenaW,
                     bruttoporabacie= p.TrE_KursL==1 ? decimal.Round(p.TrE_CenaT * (1 + p.TrE_Stawka / 100), 2) : decimal.Round(p.TrE_CenaW * (1 + p.TrE_Stawka / 100), 2),                      
                  });

                long jednostkaId = 0;
                
                List<HistoriaDokumentuProdukt> pozycje = new List<HistoriaDokumentuProdukt>();
                foreach (var a in pr)
                {
                    string jedn = a.TrE_Jm.Trim();
                    if (!string.IsNullOrEmpty(jedn))
                    {
                        if (jednostkiSlownik.ContainsKey(jedn))
                        {
                            jednostkaId = jednostkiSlownik[jedn];
                        }
                        else
                        {
                            long? jed = jednostkiSlownik.Where(x => x.Key.TrimEnd(".") == jedn.TrimEnd(".")).Select(x => x.Value).FirstOrDefault();
                            if (jed != null)
                            {
                                jednostkaId = jed.Value;
                            }
                            else
                            {
                                if (jednostkiDoDodania.ContainsKey(jedn))
                                {
                                    jednostkaId = jednostkiDoDodania[jedn].Id;
                                }
                                else
                                {
                                    Jednostka jednostka = new Jednostka();
                                    jednostka.Nazwa = jedn;
                                    jednostka.Id = jedn.WygenerujIDObiektu();
                                    jednostka.JezykId = _config.JezykIDDomyslny;
                                    jednostkiDoDodania.Add(jedn, jednostka);
                                    jednostkaId = jednostka.Id;
                                }
                            }
                            
                        }
                    }
                    else
                    {
                        LogiFormatki.PobierzInstancje.LogujInfo("Dokument o numerze {0} nie mozna pbrać takiej pozycji bo nie ma jednostki w systemie o nazwie {1}", hd.Key.NazwaDokumentu, a.TrE_Jm);
                        dokDoWywaleniaBrakJednostki.Add(hd.Key);
                        
                        break;
                    }
                    HistoriaDokumentuProdukt it = new HistoriaDokumentuProdukt
                    {
                        NazwaProduktu = a.Twr_Nazwa,
                        KodProduktu = a.Twr_Kod,
                        CenaBrutto = a.brutto,
                        CenaNetto = a.netto,
                        Ilosc = a.TrE_Ilosc,
                        JednostkaMiary = jednostkaId,
                        Rabat = a.TrE_Rabat,
                        Vat = a.TrE_Stawka,
                        WartoscVat = a.wartosc_vat,
                        Id = a.TrE_TrEID,
                        DokumentId = a.TrE_TrNId,
                        ProduktId = a.TrE_TwrId.Value,  //jak wwyali wyajtek to znaczy ze trzeba omijac takie cos - bo produkt id jest u nas WYMAGANE
                        WartoscBrutto = a.TrE_WartoscBruttoWal,
                        WartoscNetto = a.TrE_WartoscNettoWal,
                        CenaNettoPoRabacie = a.nettoporabacie,
                        CenaBruttoPoRabacie = a.bruttoporabacie,
                        ProduktIdBazowy = a.TrE_TwrId.Value
                    };
                   
                    pozycje.Add(it);
                }
                hd.Value.AddRange(pozycje);
            }
            foreach (var historiaDokumentu in dokDoWywaleniaBrakJednostki)
            {
                items.Remove(historiaDokumentu);
            }

            if (jednostkiDoDodania.Any())
            {
                LogiFormatki.PobierzInstancje.LogujInfo("Ilość jednostek do dodania: {0}", jednostkiDoDodania.Count);
                APIWywolania.PobierzInstancje.AktualizujJednostki(jednostkiDoDodania.Values.ToList());
            }
            return items;
        }
        internal List<Rabat> GetDiscounts(List<long> dlaKogoLiczyc, List<Produkt> produkty, List<PoziomCenowy> ceny, List<Cecha> cechy, List<ProduktCecha> cechy_produkty, Dictionary<long, KategoriaProduktu> kategorie, List<ProduktKategoria> produkty_kategorie, IDictionary<int, KategoriaKlienta> KategorieKlientow, IDictionary<long, KlientKategoriaKlienta> klienciKategorie)
        {
            SqlConnection conn = null;
            SqlCommand cmd = null;
            SqlDataReader dr = null;
            List<Rabat> items = new List<Rabat>();
            try
            {
                conn = new SqlConnection(_config.ERPcs);
                conn.Open();
                cmd = new SqlCommand(@"select Rab_Cena, Rab_PodmiotId, Rab_Rabat,Rab_RabId, Rab_TwrId, Rab_Typ, Rab_Waluta from CDN.Rabaty Where (Rab_PodmiotTyp=1 or Rab_PodmiotId is null) or ((Rab_PodmiotTyp is null and Rab_PodmiotId is not null) and Rab_Typ = 12)", conn);
                dr = cmd.ExecuteReader();
                
                while (dr.Read())
                {
                    decimal Rab_Cena = DataHelper.dbd("Rab_Cena", dr);
                    int? Rab_PodmiotId = DataHelper.dbin("Rab_PodmiotId", dr);
                    decimal Rab_Rabat = DataHelper.dbd("Rab_Rabat", dr);
                    int Rab_RabId = DataHelper.dbi("Rab_RabId", dr);
                    int? Rab_TwrId = DataHelper.dbin("Rab_TwrId", dr);
                    int Rab_Typ = DataHelper.dbi("Rab_Typ", dr);
                    

                    Rabat d = new Rabat();
                    d.TypRabatu = RabatTyp.Zaawansowany;
                    d.Wartosc1 = Rab_Rabat == 0 ? Rab_Cena : Rab_Rabat;
                    d.Wartosc2 = d.Wartosc1;
                    d.Wartosc3 = d.Wartosc2;
                    d.TypWartosci = Rab_Rabat == 0 ? RabatSposob.StalaCena : RabatSposob.Procentowy;

                    string Rab_Waluta = string.IsNullOrEmpty(DataHelper.dbs("Rab_Waluta", dr)) && d.TypWartosci!=RabatSposob.Procentowy ? "PLN" : DataHelper.dbs("Rab_Waluta", dr);

                    if (d.TypWartosci == RabatSposob.StalaCena && string.IsNullOrEmpty(Rab_Waluta))
                    {
                        LogiFormatki.PobierzInstancje.LogujError(new Exception(string.Format("Waluta jest wymagana przy dodawaniu rabatu o typie stala cena, rabat o id: {0} jest pomijany", Rab_RabId)));
                        continue;
                    }
                    d.WalutaId = string.IsNullOrEmpty(Rab_Waluta) ? (long?) null : (Rab_Waluta.ToLower().WygenerujIDObiektuSHAWersjaLong());

                    if (Rab_Typ == 3 || Rab_Typ == 5 || Rab_Typ == 7)
                        d.KategoriaProduktowId = Rab_TwrId + grupyoffset;
                    else
                        d.ProduktId = Rab_TwrId;
                    if (Rab_Typ == 1 || Rab_Typ == 2 || Rab_Typ == 5 || Rab_Typ == 6 || Rab_Typ == 13)
                        d.KlientId = Rab_PodmiotId;
                    if (Rab_Typ == 3 || Rab_Typ == 4 || Rab_Typ == 12)
                        d.KategoriaKlientowId = Rab_PodmiotId;
                        items.Add(d);
                }

            }
            finally
            {
                if (dr != null) { dr.Close(); dr.Dispose();}
                if (cmd != null) { cmd.Dispose();}
                if (conn != null) { conn.Close(); conn.Dispose();}
            }
            return items;
        }


        private Dictionary<int, Dictionary<string, string>> PobierzAtrybutyDlaProduktu()
        {
            Dictionary<int, Dictionary<string, string>> wynik = new Dictionary<int, Dictionary<string, string>>();
            List<Atrybut> listaAtrybutow = new List<Atrybut>();
          
            string zapytanie = string.Format("select Twr_TwrId from cdn.Towary");
            SqlConnection conn = null;
            SqlCommand cmd = null;
            SqlDataReader rd = null;
            HashSet<int> produkty = new HashSet<int>();
            try
            {
                conn = new SqlConnection(_config.ERPcs);
                conn.Open();
                cmd = new SqlCommand(zapytanie, conn);
                rd = cmd.ExecuteReader();
                while (rd.Read())
                {
                    int Twr_TwrId = DataHelper.dbi("Twr_TwrId", rd);
                    produkty.Add(Twr_TwrId);
                }
            }
            finally
            {
                if (rd != null) { rd.Close(); rd.Dispose(); }
                if (cmd != null) { cmd.Dispose(); }
                if (conn != null) { conn.Close(); conn.Dispose(); }
            }
            //int[] produkty=new int[5];//dodaj wyciaganie id produktow

            var cechy = GetAttributes(out listaAtrybutow).ToDictionary(x => x.Id, x => x);
            var laczniki = GetAttributesLacznik().GroupBy(x => x.ProduktId).ToDictionary(x => x.Key, x =>new HashSet<long>( x.Select(y => y.CechaId)));
            foreach (var p in produkty)
            {
                Dictionary<string, string> sw = new Dictionary<string, string>();
                var lp = laczniki.ContainsKey(p) ? laczniki[p] : new HashSet<long>();
                var cechyproduktu = cechy.WhereKeyIsIn(lp).GroupBy(x=>x.AtrybutId).ToDictionary(x => x.Key, x => x.ToList());
                foreach (var a in listaAtrybutow)
                {
                    var wartosc = cechyproduktu.ContainsKey(a.Id) ? cechyproduktu[a.Id] : null;
                    StringBuilder sb=new StringBuilder();
                    if (wartosc != null)
                    {
                        wartosc.ForEach(x=>sb.AppendFormat("{0}, ", x.Nazwa));
                    }
                    sw.Add(a.Nazwa, sb.ToString().Trim().Trim(','));
                }
                wynik.Add(p, sw);


            }
            return wynik;
        }

        private Dictionary<long, DateTime> PobierzTerminyDostaw()
        {
            Dictionary<long, DateTime> termin = new Dictionary<long, DateTime>();


            SqlConnection conn = null;
            SqlCommand cmd = null;
            SqlDataReader rd = null;
            try
            {
                conn = new SqlConnection(_config.ERPcs);
                conn.Open();

                cmd = new SqlCommand(
                        "SELECT te.TrE_TwrId,data = Max(tn.TrN_DataWys) FROM cdn.TraElem te join cdn.TraNag tn on te.TrE_TrNId=tn.TrN_TrNID where tn.TrN_TypDokumentu=309 and tn.TrN_DataWys>SYSDATETIME() group by te.TrE_TwrId",
                        conn);
                cmd.CommandTimeout = 60000;
                rd = cmd.ExecuteReader();
                while (rd.Read())
                {
                    long idProduktu = DataHelper.dbl("TrE_TwrId", rd);
                    DateTime dataDostawy = DataHelper.dbdt("data", rd);
                    if (!termin.ContainsKey(idProduktu))
                    {
                        termin.Add(idProduktu, dataDostawy);
                    }
                }
            }
            finally
            {
                if (rd != null)
                {
                    rd.Close();
                    rd.Dispose();
                }
                if (cmd != null) cmd.Dispose();
                if (conn != null)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
            return termin;
        }

        internal List<Produkt> PobierzProdukty(out List<Tlumaczenie> tlumaczenia, out List<JednostkaProduktu> jednostki)
        {
            jednostki = new List<JednostkaProduktu>();
            tlumaczenia = new List<Tlumaczenie>();
            //string b2b_ukryj = _config.OptimaUkrywajProduktyZAtrybutem;
            //string b2b_wysylaj = _config.OptimaWysylajTylkoProduktyZAtrybutem;
            bool czyIstniejeTabela = DataHelper.CzyIstniejeTabela("TwrJMZ", _config.ERPcs);

            Dictionary<int, Dictionary<string, string>> atrybutyProduktu = PobierzAtrybutyDlaProduktu();
            Dictionary<long, DateTime> terminyDostaw = PobierzTerminyDostaw();

            List<TwrJMZ> jednostkidodatkowe= new List<TwrJMZ>();
            if (czyIstniejeTabela)
            {
                jednostkidodatkowe = db.TwrJMZs.ToList();
            }
             
            bool czyJestOdwrotneObciazenie = DataHelper.CzyKolumnaIstnieje("Towary", "Twr_OdwrotneObciazenie", _config.ERPcs);
            string doklej = string.Empty;
            if (czyJestOdwrotneObciazenie)
            {
                doklej = ", Twr_OdwrotneObciazenie";
            }
            string produktSQL = string.Format("select Twr_Nazwa, Twr_NieAktywny, Twr_Opis, Twr_Stawka, Twr_TwrId, Twr_URL, Twr_IloscMin, Twr_JM, Twr_JMZ, jm_przelicznik = Twr_JMPrzelicznikL/Twr_JMPrzelicznikM, Twr_Kod, Twr_NumerKat, Twr_SWW, Twr_Typ, Twr_Produkt, Twr_EAN, Twr_MinCenaMarza, Twr_UdostepniajWCenniku, Twr_WagaKG{0} from cdn.Towary",doklej);

            List<Produkt> items = new List<Produkt>();
            //int pokazujCennik;
            //if (!int.TryParse(_config.PokazujTylkoCennik, out pokazujCennik))
            //    pokazujCennik = 0;
            var zestawystany = PobierzStanyZestawow();
            SqlConnection conn = null;
            SqlCommand cmd = null;
            SqlDataReader rd = null;
            try
            {
                conn = new SqlConnection(_config.ERPcs);
                conn.Open();

                WidcznoscProduktowWOptimie ustawienie =_config.OptimaKtoreTowaryEksportowac;
                cmd = new SqlCommand(produktSQL, conn);
                rd = cmd.ExecuteReader();
                while (rd.Read())
                {
                    Produkt item = new Produkt();

                    int twr_Typ = DataHelper.dbi("Twr_Typ", rd);
                    int Twr_NieAktywny = DataHelper.dbi("Twr_NieAktywny", rd);
                    int Twr_UdostepniajWCenniku = DataHelper.dbi("Twr_UdostepniajWCenniku", rd);
                    string Twr_EAN = DataHelper.dbs("Twr_EAN", rd);
                    int Twr_TwrId = DataHelper.dbi("Twr_TwrId", rd);
                    decimal Twr_IloscMin = DataHelper.dbd("Twr_IloscMin", rd);
                    decimal? Twr_WagaKG = DataHelper.dbdn("Twr_WagaKG", rd);
                    string Twr_SWW = DataHelper.dbs("Twr_SWW", rd);
                    decimal Twr_Stawka = DataHelper.dbd("Twr_Stawka", rd);
                    string Twr_JM = DataHelper.dbs("Twr_JM", rd);
                    string Twr_Nazwa = DataHelper.dbs("Twr_Nazwa", rd);
                    string Twr_NumerKat = DataHelper.dbs("Twr_NumerKat", rd);
                    string Twr_Kod = DataHelper.dbs("Twr_Kod", rd);
                    string Twr_Opis = DataHelper.dbs("Twr_Opis", rd);
                    decimal Twr_MinCenaMarza = DataHelper.dbd("Twr_MinCenaMarza", rd);
                    int Twr_OdwrotneObciazenie = 0;
                    if (czyJestOdwrotneObciazenie)
                    {
                        Twr_OdwrotneObciazenie = DataHelper.dbi("Twr_OdwrotneObciazenie", rd);
                    }
                    
                    //Twr_NieAktywny
                    
                    //IF Twr_UdostepniajWCenniku sdfsdfsfer
                    if (Twr_NieAktywny == 1)
                    {
                        continue;
                    }
                    if (ustawienie == WidcznoscProduktowWOptimie.TylkoZCennikaZewnetrznego && Twr_UdostepniajWCenniku == 0)
                    {
                        continue;
                    }

                    if (twr_Typ == 0)
                    {
                        item.Typ = TypProduktu.Usluga;
                    }

                    item.KodKreskowy = Twr_EAN;
                    item.Id = Twr_TwrId;
                    item.StanMin = Twr_IloscMin;
                   // item.Waga = Twr_WagaKG == null ? 0 : Twr_WagaKG.Value;
                    item.PKWiU = Twr_SWW;
                    item.Vat = Twr_Stawka;
                    item.UstawWidocznoscProduktu(true);
                    JednostkaProduktu jp = new JednostkaProduktu
                    {
                        Podstawowa = true,
                        Id = Twr_JM.WygenerujIDObiektu(),
                        Nazwa = Twr_JM,
                        ProduktId = item.Id,
                        Przelicznik = 1
                    };

                    if (!jednostki.Any(a => a.Id == jp.Id && a.ProduktId == jp.ProduktId))
                    {
                        jednostki.Add(jp);
                    }

                    if (czyJestOdwrotneObciazenie && Twr_OdwrotneObciazenie == 1)
                    {
                        item.Vat = 0;
                        item.VatOdwrotneObciazenie = true;
                    }


                    if (czyIstniejeTabela)
                    {
                        List<TwrJMZ> jednostkadodatkowa =
                            jednostkidodatkowe.Where(a => a.TwJZ_TwrID == item.Id).ToList();

                        foreach (TwrJMZ twrJmz in jednostkadodatkowa)
                        {
                            JednostkaProduktu jednostka = jednostki.FirstOrDefault(a => a.Nazwa == twrJmz.TwJZ_JM);

                            JednostkaProduktu jp3 = new JednostkaProduktu
                            {
                                Podstawowa = false,
                                Id =
                                    jednostka != null ? jednostka.Id : twrJmz.TwJZ_JM.WygenerujIDObiektu(),
                                Nazwa = twrJmz.TwJZ_JM,
                                ProduktId = item.Id,
                                Przelicznik = twrJmz.TwJZ_JMPrzelicznikL <= 0 ? 1 : twrJmz.TwJZ_JMPrzelicznikL
                            };
                            if (!jednostki.Any(a => a.Id == jp3.Id && a.ProduktId == jp3.ProduktId))
                            {
                                jednostki.Add(jp3);
                            }
                        }
                    }


                    Dictionary<string, string> pars = atrybutyProduktu[Twr_TwrId];
                    pars.Add("Twr_Nazwa", Twr_Nazwa);
                    pars.Add("Twr_NumerKat", Twr_NumerKat);
                    pars.Add("Twr_Kod", Twr_Kod);
                    pars.Add("Twr_Opis", Twr_Opis != "0" ? Twr_Opis : "");
                    pars.Add("Twr_WagaKG", Twr_WagaKG.HasValue ? Twr_WagaKG.Value.ToString(CultureInfo.InvariantCulture) : null);
                    pars.Add("Twr_MinCenaMarza", Twr_MinCenaMarza.ToString(CultureInfo.InvariantCulture));
                    bool znalezionozestaw = false;
                    if (zestawystany.Any())
                    {
                        string stan;
                        znalezionozestaw = zestawystany.TryGetValue((int)item.Id, out  stan);
                        log.DebugFormat("Dla produktu {0}, znalezion zestaw {1}, zestaw {2}", item.Id, znalezionozestaw, stan ?? "");
                        pars.Add("StanZestawu", stan??"");
                    }


                    pars.Add("TerminDostawy", terminyDostaw.ContainsKey(Twr_TwrId) ? terminyDostaw[Twr_TwrId].ToShortDateString() : null);
                    _config.SynchronizacjaPobierzObjetoscProduktu(item, "", pars);
                    _config.SynchronizacjaPobierzWageProduktu(item, "Twr_WagaKG", pars);
                    _config.SynchronizacjaPobierzWidocznoscProduktuZPola(item, "", pars);
                    _config.SynchronizacjaPobierzPoleLiczba1(item, "", pars);
                    _config.SynchronizacjaPobierzPoleLiczba2(item, "", pars);
                    _config.SynchronizacjaPobierzPoleLiczba3(item, "", pars);
                    _config.SynchronizacjaPobierzPoleLiczba4(item, "", pars);
                    _config.SynchronizacjaPobierzPoleLiczba5(item, "", pars);
                    _config.SynchronizacjaPobierzKolumnaLiczba1(item, "", pars);
                    _config.SynchronizacjaPobierzKolumnaLiczba2(item, "", pars);
                    _config.SynchronizacjaPobierzKolumnaLiczba3(item, "", pars);
                    _config.SynchronizacjaPobierzKolumnaLiczba4(item, "", pars);
                    _config.SynchronizacjaPobierzKolumnaLiczba5(item, "", pars);
                    _config.SynchronizacjaPobierzPoleDostawa(item, "", pars, null);

                    _config.SynchronizacjaPobierzIloscMinimlna(item, "", pars);
                    _config.SynchronizacjaPobierzIloscWOpakowaniu(item, "", pars);

                    _config.SynchronizacjaPobierzPoleOjciec(item, "", pars);

                    //wymiary dla opakowań
                    _config.SynchronizacjaPobierzGlebokoscOpakowaniaJednostkowego(item, "", pars);
                    _config.SynchronizacjaPobierzWysokoscOpakowaniaJednostkowego(item, "", pars);
                    _config.SynchronizacjaPobierzSzerokoscOpakowaniaJednostkowego(item, "", pars);
                    
                    _config.SynchronizacjaPobierzGlebokoscOpakowaniaZbiorczego(item, "", pars);
                    _config.SynchronizacjaPobierzWysokoscOpakowaniaZbiorczego(item, "", pars);
                    _config.SynchronizacjaPobierzSzerokoscOpakowaniaZbiorczego(item, "", pars);
                    _config.SynchronizacjaPobierzWageOpakowaniaZbiorczego(item, "", pars);
                    _config.SynchronizacjaPobierzObjetoscOpakowaniaZbiorczego(item, "", pars);
                    _config.SynchronizacjaPobierzIloscWOpakowaniuZbiorczym(item, "", pars);

                    _config.SynchronizacjaPobierzGlebokoscPalety(item, "", pars);
                    _config.SynchronizacjaPobierzWysokoscPalety(item, "", pars);
                    _config.SynchronizacjaPobierzSzerokoscPalety(item, "", pars);
                    _config.SynchronizacjaPobierzWagePalety(item, "", pars);
                    _config.SynchronizacjaPobierzObjetoscPalety(item, "", pars);
                    _config.SynchronizacjaPobierzLiczbaSztukNaPalecie(item, "", pars);
                    _config.SynchronizacjaPobierzLiczbaSztukNaWarstwie(item, "", pars);
                    foreach (Jezyk j in _config.JezykiWSystemie.Values)
                    {
                        _config.SynchronizacjaPobierzLokalizacjeNazwa(item, j, "Twr_Nazwa", pars, ref tlumaczenia);
                        _config.SynchronizacjaPobierzLokalizacjeKod(item, j, "Twr_Kod", pars, ref tlumaczenia);
                        _config.SynchronizacjaPobierzLokalizacjeOpis(item, j, "", pars, ref tlumaczenia);
                        _config.SynchronizacjaPobierzLokalizacjeOpis2(item, j, "", pars, ref tlumaczenia);
                        _config.SynchronizacjaPobierzLokalizacjeOpis3(item, j, "", pars, ref tlumaczenia);
                        _config.SynchronizacjaPobierzLokalizacjeOpis4(item, j, "", pars, ref tlumaczenia);
                        _config.SynchronizacjaPobierzLokalizacjeOpis5(item, j, "", pars, ref tlumaczenia);
                        _config.SynchronizacjaPobierzLokalizacjeOpisKrotki(item, j, "", pars, ref tlumaczenia);
                        _config.SynchronizacjaPobierzLokalizacjeOpisKrotki2(item, j, "", pars, ref tlumaczenia);
                        _config.SynchronizacjaPobierzLokalizacjeOpisKrotki3(item, j, "", pars, ref tlumaczenia);
                        _config.SynchronizacjaPobierzLokalizacjeOpisKrotki4(item, j, "", pars, ref tlumaczenia);
                        _config.SynchronizacjaPobierzLokalizacjeOpisKrotki5(item, j, "", pars, ref tlumaczenia);
                        _config.SynchronizacjaPobierzLokalizacjePoleTekst1(item, j, "", pars, ref tlumaczenia);
                        _config.SynchronizacjaPobierzLokalizacjePoleTekst2(item, j, "", pars, ref tlumaczenia);
                        _config.SynchronizacjaPobierzLokalizacjePoleTekst3(item, j, "", pars, ref tlumaczenia);
                        _config.SynchronizacjaPobierzLokalizacjePoleTekst4(item, j, "", pars, ref tlumaczenia);
                        _config.SynchronizacjaPobierzLokalizacjePoleTekst5(item, j, "", pars, ref tlumaczenia);
                        _config.SynchronizacjaPobierzLokalizacjeKolumnaTekst1(item, j, "", pars, ref tlumaczenia);
                        _config.SynchronizacjaPobierzLokalizacjeKolumnaTekst2(item, j, "", pars, ref tlumaczenia);
                        _config.SynchronizacjaPobierzLokalizacjeKolumnaTekst3(item, j, "", pars, ref tlumaczenia);
                        _config.SynchronizacjaPobierzLokalizacjeKolumnaTekst4(item, j, "", pars, ref tlumaczenia);
                        _config.SynchronizacjaPobierzLokalizacjeKolumnaTekst5(item, j, "", pars, ref tlumaczenia);
                        _config.SynchronizacjaPobierzLokalizacjeMetaOpis(item, j, "", pars, ref tlumaczenia);
                        _config.SynchronizacjaPobierzLokalizacjeMetaSlowaKluczowe(item, j, "", pars, ref tlumaczenia);
                        _config.SynchronizacjaPobierzLokalizacjeRodzina(item, j, "", pars, ref tlumaczenia);
                    }
                    if (string.IsNullOrWhiteSpace(item.Nazwa))
                    {
                        item.Nazwa = Twr_Nazwa;
                    }
                    if (string.IsNullOrWhiteSpace(item.Kod))
                    {
                        item.Kod = Twr_Kod;
                    }
                    //if (!string.IsNullOrEmpty(item.Opis))
                    //{
                    //    item.Opis = item.Opis.ToHtml();
                    //}
                    if (znalezionozestaw)
                    {
                        log.DebugFormat("dane obiektu {0}",item.OpisObiektu() );
                    }
                    if (items.All(a => a.Id != item.Id))
                    {
                        items.Add(item);
                    }
                }

                


                //item.UstawWidocznoscProduktu(((Twr_NieAktywny == 0)));

                //if (Twr_Typ == 0)
                //{
                //    item.Typ = TypProduktu.Usluga;
                //}
                //else if (pokazujCennik != 0)
                //{
                //    bool val = Twr_UdostepniajWCenniku != 0;
                //    item.UstawWidocznoscProduktu((((pokazujCennik == 1 && val) || (pokazujCennik == -1 && !val)) ? true : false));
                //}
                //else
                //{
                //  //  item.UstawWidocznoscProduktu(((Twr_NieAktywny == 0)));
                //    //item.UstawWidocznoscProduktu((!ukryty && (Twr_NieAktywny == 0)));
                //}

            }
            finally
            {
                if (rd != null) { rd.Close(); rd.Dispose(); }
                if (cmd != null) cmd.Dispose();
                if (conn != null) { conn.Close(); conn.Dispose(); }
            }

           return items;
        }
        internal Dictionary<long, decimal> GetLiteProducts(string mag)
        {
            bool res = _config.UzwagledniaRezerwacjeStanow;
            Magazyny magazyn = db.Magazynies.FirstOrDefault(a => a.Mag_Symbol == mag);
            var q = (from p in db.Towaries
                     join i in db.TwrIloscis on db.TwrIlosciID(p.Twr_TwrId, magazyn.Mag_MagId, DateTime.Now) equals i.TwI_TwIId into JoinI
                     from i in JoinI.DefaultIfEmpty()
                     where p.Twr_Typ == 1
                     select new { p.Twr_TwrId, p.Twr_IloscMin, ilosc = i == null ? 0 : (res ? i.TwI_Ilosc - i.TwI_Rezerwacje : i.TwI_Ilosc) }).ToList();
            return q.ToDictionary(v => (long)v.Twr_TwrId, v => v.ilosc);
        }

        private Dictionary<int, string> PobierzStanyZestawow()
        {
            string sql = @"select PdR_TwrId,sklad from cdn.ProdReceptury rec join(

select PdS_PdRId,
 STUFF((
    SELECT '; ' +REPLACE(CAST(PdS_IloscJM AS VARCHAR(20)),'.',',') + '*' + CAST(PdS_TwrId AS VARCHAR(20)) 
    FROM cdn.ProdSkladniki
    WHERE PdS_PdRId = calosc.PdS_PdRId 
    FOR XML PATH(''),TYPE).value('(./text())[1]','VARCHAR(MAX)')
  ,1,2,'') AS sklad from cdn.ProdSkladniki calosc group by PdS_PdRId) skl on rec.PdR_PdRId=skl.PdS_PdRId";
            Dictionary<int, string> wynik = new Dictionary<int, string>();
            using (SqlConnection conn=new SqlConnection(_config.ERPcs))
            {
                conn.Open();
                using (SqlCommand cmd=new SqlCommand(sql,conn))
                {
                    using (SqlDataReader rd=cmd.ExecuteReader())
                    {
                        while (rd.Read())
                        {


                            int produkt = DataHelper.dbi("PdR_TwrId", rd);
                            string stan = DataHelper.dbs("sklad", rd);
                            wynik.Add(produkt, stan);
                        }
                    }
                }
            }
            return wynik;

        }

        internal List<KategoriaKlienta> GetCustomerCategories()
        {
            

            //wyciaganie kategorii z atrybutów
            //List<KntAtrybuty> wszystkieatrybuty = db.KntAtrybuties.ToList();
            //List<DefAtrybuty> definicjeatrybutow = db.DefAtrybuties.ToList();

            //foreach (DefAtrybuty atrybut in definicjeatrybutow)
            //{
            //    if (ConfigBLL.PobierzInstancje.KategorieKlientowPolaWlasne.Contains(atrybut.DeA_Nazwa))
            //    {
            //        var listaWartosci = wszystkieatrybuty.Where(x => x.KnA_DeAId == atrybut.DeA_DeAId).ToList();
            //        if (!listaWartosci.Any())
            //        {
            //            continue;
            //        }
            //    }
            //}

            //atrybuty z platnosci
            //List<Kontrahenci> listaKlientow = db.Kontrahencis.Where(x => x.Knt_Termin > 0 && x.Knt_TerminPlat == 1).ToList();


            //var q = (from c in db.Grupies
            //         where c.Gru_Typ == 31
            //         select new { c.Gru_GruID, c.Gru_Nazwa, c.Gru_Opis }).ToList();
            

            //foreach (var v in q)
            //{
               
            //}


            
            List<KategoriaKlienta> items = new List<KategoriaKlienta>();
            SqlConnection conn = null;
            SqlCommand cmd = null;
            SqlDataReader rd = null;
            try
            {
                string grupySQL = "select Gru_GruID, Gru_Nazwa, Gru_Opis from cdn.Grupy where Gru_Typ=31;";
                conn = new SqlConnection(_config.ERPcs);
                conn.Open();
                cmd = new SqlCommand(grupySQL, conn);
                rd = cmd.ExecuteReader();
                while (rd.Read())
                {
                    int Gru_GruID = DataHelper.dbi("Gru_GruID", rd);
                    string Gru_Nazwa = DataHelper.dbs("Gru_Nazwa", rd).Trim().ToLower();
                    string Gru_Opis = DataHelper.dbs("Gru_Opis", rd).Trim().ToLower();

                    KategoriaKlienta item = new KategoriaKlienta();
                    item.Id = Gru_GruID;

                    item.Nazwa = string.IsNullOrEmpty(Gru_Opis)? Gru_Nazwa : Gru_Opis;
                    item.Grupa = "GRUPA GŁÓWNA";
                    items.Add(item);
                }

                rd.Close();
                rd.Dispose();
                cmd.Dispose();
                string kontrachentTerminSql = "select Knt_Termin from cdn.Kontrahenci where Knt_Termin>0 and Knt_TerminPlat=1;";
                

                cmd = new SqlCommand(kontrachentTerminSql, conn);
                rd = cmd.ExecuteReader();
                while (rd.Read())
                {
                    int Knt_Termin = DataHelper.dbi("Knt_Termin", rd);
                    if (items.Any(x => x.Nazwa == Knt_Termin.ToString()))
                    {
                        continue;
                    }
                    KategoriaKlienta item = new KategoriaKlienta();
                    item.Nazwa = Knt_Termin.ToString();
                    item.Grupa = "TERMIN PRZELEWU";
                    item.Id = Knt_Termin*10000; //offset plus termin
                    items.Add(item);
                }
            }
            finally
            {
                if (rd != null) { rd.Close(); rd.Dispose(); }
                if (cmd != null) cmd.Dispose();
                if (conn != null) { conn.Close(); conn.Dispose(); }
            }
            return items;
        }

        public int StworzIDCechy(int idAtrybutu, string wartosc)
        {
            //zawsze liczymy id z lowera dla Optimy
            return (idAtrybutu.ToString() + wartosc.ToLower()).WygenerujIDObiektuSHA(1);
        }

        public string StworzSymbolCechey(string atrybut, string cecha)
        {
            return string.IsNullOrEmpty(cecha) ? (_config.CechaAuto + ":" + atrybut) : (atrybut + ":" + cecha).ToLower().Trim();
        }

        internal List<Cecha> GetAttributes(out List<Atrybut> atrybuty)
        {
          Dictionary<int,Atrybut> atrydic=new Dictionary<int, Atrybut>();
           // atrybuty = new List<atrybuty>();

            List<Cecha> items = new List<Cecha>();
           

            Dictionary<string, Cecha> slownikCech = new Dictionary<string, Cecha>();

            string cechaAuto = _config.CechaAuto;
            char[] separ = _config.SeparatorAtrybutowWCechach;
            bool atrybutZCechy = _config.AtrybutZCechy;


            string sqlatrybuty = "select p.TwA_TwAId, p.TwA_WartoscTxt, p.TwA_TwrId, p.TwA_DeAId, da.DeA_Nazwa, da.DeA_Kod from cdn.twratrybuty p join cdn.DefAtrybuty da on da.dea_deaid=p.TwA_deaid";



            SqlConnection conn = null;
            SqlCommand cmd = null;
            SqlDataReader rd = null;
            try
            {
                conn = new SqlConnection(_config.ERPcs);
                conn.Open();


                cmd = new SqlCommand(sqlatrybuty, conn);
                rd = cmd.ExecuteReader();
                while (rd.Read())
                {
                    int TwA_DeAId = DataHelper.dbi("TwA_DeAId", rd);
                    string DeA_Nazwa = DataHelper.dbs("DeA_Nazwa", rd);
                    string DeA_Kod = DataHelper.dbs("DeA_Kod", rd);
                    string TwA_WartoscTxt = DataHelper.dbs("TwA_WartoscTxt", rd);
                    Atrybut atrybut = atrydic.ContainsKey(TwA_DeAId) ? atrydic[TwA_DeAId] : null;
                    if (atrybut == null)
                    {
                        if (DeA_Nazwa == "")
                        {
                            DeA_Nazwa = DeA_Kod;
                        }
                        atrybut = new Atrybut(DeA_Nazwa);
                        atrybut.Id = TwA_DeAId;
                        atrybut.Symbol = DeA_Kod;
                        atrybut.Kolejnosc = 0;
                        atrydic.Add(TwA_DeAId, atrybut);
                    }

                    string sym = StworzSymbolCechey(DeA_Kod, TwA_WartoscTxt);//string.IsNullOrEmpty(TwA_WartoscTxt) ? (_config.CechaAuto + ":" + DeA_Kod) : (DeA_Kod + ":" + TwA_WartoscTxt).ToLower().Trim();

                    Cecha item = new Cecha();
                    item.Symbol = sym;
                    item.Nazwa = string.IsNullOrEmpty(TwA_WartoscTxt) ? DeA_Kod : TwA_WartoscTxt;
                    item.AtrybutId = TwA_DeAId;
                    item.Widoczna = true;
                    
                    item.Id = StworzIDCechy(TwA_DeAId, sym);
                    if (!slownikCech.ContainsKey(item.Symbol))
                        slownikCech.Add(item.Symbol, item);
                }
            }finally
            {
                if (rd != null) { rd.Close(); rd.Dispose(); }
                if (cmd != null) cmd.Dispose();
                if (conn != null) { conn.Close(); conn.Dispose(); }
            }



            //kategorie
            var qk = (from c in db.TwrGrupyViews
                      where (c.TwGId != 0 && c.Nazwa != "")
                      select new
                      {
                          TwG_GIDNumer = c.GIDNumer,
                          TwG_Nazwa = c.Nazwa,
                          TwG_GrONumer = c.GrONumer,

                      }).ToList();

            foreach (var v in qk)
            {
                Cecha item = new Cecha();
                item.Id = grupyoffset + v.TwG_GIDNumer;

                char separator = v.TwG_Nazwa.Contains("_") ? '_' : ':';
                string nowaNazwa = v.TwG_Nazwa.Trim();

                var parent = qk.FirstOrDefault(a => a.TwG_GIDNumer == v.TwG_GrONumer);
                while (parent != null)
                {
                    nowaNazwa = parent.TwG_Nazwa.Trim() + "\\" + nowaNazwa;

                    parent = qk.FirstOrDefault(a => a.TwG_GIDNumer == parent.TwG_GrONumer);
                }

                string[] obiekty = nowaNazwa.Split(separator);
                item.Nazwa = obiekty.Last().Trim();

                string nazwaAtrybutu = _config.AtrybutKategoriiZERP;

                string symbol = ((obiekty.Length == 1 ? nazwaAtrybutu : obiekty.First().Trim()) + separator + obiekty.Last().Trim()).ToLower();

                if (string.IsNullOrEmpty(item.Nazwa))
                    continue;

                item.Widoczna = true;
                string nazwaatrybutu;
                if (obiekty.Length == 1)
                {
                    nazwaatrybutu = nazwaAtrybutu + separator + item.Nazwa;
                }
                else nazwaatrybutu = nowaNazwa;


                Atrybut tmp = atrybutZCechy ? AtrybutyWyszukiwanie.PobierzInstancje.WyciagnijAtrybutZCechy(ref nazwaatrybutu, ref symbol, separ, cechaAuto) : null;

                if (tmp != null)
                {
                    if (atrydic.All(p => p.Key != tmp.Id))
                    {
                        atrydic.Add(tmp.Id,tmp);
                    }
                    item.AtrybutId = tmp.Id;
                }
                item.Symbol = symbol;

                if (!slownikCech.ContainsKey(item.Symbol))
                    slownikCech.Add(item.Symbol, item);
            }

            DodajSlownikJakoCeche("SELECT  Mrk_MrkId,Mrk_Nazwa FROM cdn.Marki", "Marka", markioffset, ref slownikCech, ref atrydic);
            DodajSlownikJakoCeche("SELECT Prd_PrdId,Prd_Nazwa FROM CDN.Producenci", "Producent", producnetoffset, ref slownikCech, ref atrydic);
            items = slownikCech.Select(a => a.Value).ToList();
            atrybuty = atrydic.Values.ToList();
            return items;
        }




        private int WygenrujIdAtrybut(string nazwa)
        {
            return nazwa.WygenerujIDObiektu();
        }
        private void DodajSlownikJakoCeche(string zapytanie, string atrybutNazwa, int offset, ref Dictionary<string, Cecha> cechy, ref Dictionary<int, Atrybut> atrybuty)
        {
            SqlConnection conn = null;
            SqlCommand cmd = null;
            SqlDataReader rd = null;
            int atrybutid = WygenrujIdAtrybut(atrybutNazwa);
            try
            {
                List<Cecha> markic = new List<Cecha>();
                conn = new SqlConnection(_config.ERPcs);
                conn.Open();
                cmd = new SqlCommand(zapytanie, conn);
                rd = cmd.ExecuteReader();
                while (rd.Read())
                {
                    string marka = rd[1].ToString();
                    int markaid = Int32.Parse(rd[0].ToString());
                    string sym = (atrybutNazwa + ":" + marka).ToLower().Trim();

                    Cecha item = new Cecha();
                    item.Symbol = sym;
                    item.Nazwa = marka;
                    item.AtrybutId = atrybutid;
                    item.Widoczna = true;
                    item.Id = markaid + offset;
                    markic.Add(item);
                }
                if (markic.Any())
                {
                    foreach (Cecha mc in markic)
                    {
                        if (!cechy.ContainsKey(mc.Symbol))
                        {
                            cechy.Add(mc.Symbol, mc);
                        }
                        
                    }
                    Atrybut atrybut = new Atrybut(atrybutNazwa);
                    atrybut.Id = atrybutid;
                    atrybut.Symbol = atrybutNazwa;
                    atrybut.Kolejnosc = 0;
                    if (!atrybuty.ContainsKey(atrybut.Id))
                    {
                        atrybuty.Add(atrybut.Id, atrybut);
                    }
                    
                }
            }
            finally
            {
                if (rd != null) { rd.Close(); rd.Dispose(); }
                if (cmd != null) { cmd.Dispose(); }
                if (conn != null) { conn.Close(); conn.Dispose(); }
            }
        }


        internal List<ZamowienieSynchronizacja> SetOrders(List<ZamowienieSynchronizacja> list, Dictionary<long, Klient> wszyscy)
        {
            int docId = 0;
            string docNum = "";
            IEnumerable<Klient> klienci = wszyscy.Values;  
            // magazyn
            string sid = _config.SymbolMagazynow;

            if (string.IsNullOrEmpty(sid))
                throw new Exception("Brak zdefiniowanego parametru, symbole_magazynow.");

            string[] sids = sid.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);


            string w = string.Empty;
            foreach (var side in sids)
            {
                w += string.Format("'{0}',", side);
            }
            string zapytanie = "select Mag_MagId, Mag_Symbol from cdn.Magazyny";
            List<int> m = new List<int>();
            SqlConnection conn = null;
            SqlCommand cmd = null;
            SqlDataReader rd = null;
            try
            {
                conn = new SqlConnection(_config.ERPcs);
                conn.Open();
                cmd = new SqlCommand(zapytanie, conn);
                rd = cmd.ExecuteReader();
                while (rd.Read())
                {
                    int Mag_MagId = DataHelper.dbi("Mag_MagId", rd);
                    string Mag_Symbol = DataHelper.dbs("Mag_Symbol", rd);

                    if (sids.FirstOrDefault(x=>x == Mag_Symbol)!=null)
                    {
                        m.Add(Mag_MagId);
                    }
                }
            }
            finally
            {
                if (rd != null) { rd.Close(); rd.Dispose(); }
                if (cmd != null) { cmd.Dispose(); }
                if (conn != null) { conn.Close(); conn.Dispose(); }
            }
            //m= db.Magazynies.Where(p => sids.Contains(p.Mag_Symbol)).Select(p => p.Mag_MagId).ToList<int>();

            if (m.Count == 0)
                throw new Exception("Niepoprawny parametr, symbole_magazynow: " + sid);

            List<ZamowienieSynchronizacja> docs = new List<ZamowienieSynchronizacja>();
            try
            {
                foreach (ZamowienieSynchronizacja o in list)
                {
                    try
                    {
                        Klient klient = klienci.FirstOrDefault(a => a.Id == o.KlientId);
                        LogiFormatki.PobierzInstancje.LogujInfo("Import zamówienia o ID " + o.Id);
                        Dictionary<long,Model.Waluta> waluty = PobierzDostepneWaluty();
                        OptimaInstancja.DodanieDokumentu(o, m[0], out docId, out docNum, klient, _config.ERPcs2,waluty);
                        o.StatusId = StatusImportuZamowieniaDoErp.Zaimportowane;//_config.StatusyZamowien.Values.First(p => p.Symbol == "Zapisane").Id;
                        o.ListaDokumentowZamowienia = new List<ZamowienieDokumenty>() { new ZamowienieDokumenty(o.Id, docId, docNum) };
                        LogiFormatki.PobierzInstancje.LogujInfo("Zaimportowano zamówienie o ID " + o.Id);
                    }

                    catch (COMException comex)
                    {
                        log.Error(comex);
                        DodajKomunikatBledu(comex.InnerException == null ? comex.Message : comex.InnerException.Message, o);
                    }
                    catch (Exception ex)
                    {
                       LogiFormatki.PobierzInstancje.LogujError(new Exception(string.Format("Zamowienie o id: {0} nie zostało zaimportowane", o.Id)));
                       log.Error(ex);
                       DodajKomunikatBledu(ex.Message, o);
                    }

                    docs.Add(o);
                }
            }
            finally
            {
                OptimaInstancja.Wylogowanie();
            }
            return docs;
        }

        private void DodajKomunikatBledu(string komunikat, ZamowienieSynchronizacja o)
        {
            o.StatusId = StatusImportuZamowieniaDoErp.Błąd;//_config.StatusyZamowien.Values.First(p => p.Symbol == "Błąd").Id;
            o.BladKomunikat = komunikat;
        }

        internal List<PoziomCenowy> GetPriceLevels()
        {
            //  var c = db.TwrCenies.Select(p => new { p.TwC_TwCNumer, p.TwC_TwrID, p.TwC_Waluta, p.TwC_Wartosc }).ToList();
            List<PoziomCenowy> prices = new List<PoziomCenowy>();
            foreach (DefCeny d in db.DefCenies.OrderBy(p => p.DfC_DfCId) )
            {
                //  var cena = c.FirstOrDefault(a => a.TwC_TwCNumer == d.DfC_DfCId);
                PoziomCenowy price = new PoziomCenowy();
                price.WalutaId = d.DfC_Waluta.ToLower().WygenerujIDObiektuSHAWersjaLong(); ; // cena != null ? cena.TwC_Waluta : d.DfC_Waluta;
                price.Id = d.DfC_Lp;    //LP a nie ID !!!! Optima ma tak ze jak usuwamy poziom to LP jest prawdziwym id
                price.Nazwa = d.DfC_Nazwa;
                prices.Add(price);
            }

            return prices;
        }
        internal Dictionary<long, Model.Waluta> PobierzDostepneWaluty()
        {
            var wynik = new Dictionary<long, Model.Waluta>();
            foreach (DefCeny d in db.DefCenies.OrderBy(p => p.DfC_DfCId))
            {
                Model.Waluta waluta = new Model.Waluta();
                waluta.Id = d.DfC_Waluta.ToLower().WygenerujIDObiektuSHAWersjaLong();
                waluta.WalutaB2b = waluta.WalutaErp = d.DfC_Waluta;
                wynik.Add(waluta.Id, waluta);
            }

            return wynik;
        }

        internal List<CenaPoziomu> GetProductPrices()
        {
            List<CenaPoziomu> ceny = new List<CenaPoziomu>();
            List<TwrCeny> c = db.TwrCenies.ToList();
            List<PoziomCenowy> prices = GetPriceLevels();



            string towarySql = "select Twr_TwrId, Twr_Stawka from cdn.Towary";
            SqlConnection conn = null;
            SqlCommand cmd = null;
            SqlDataReader rd = null;
            try
            {
                conn = new SqlConnection(_config.ERPcs);
                conn.Open();

                cmd = new SqlCommand(towarySql, conn);
                rd = cmd.ExecuteReader();
                while (rd.Read())
                {
                    int Twr_TwrId = DataHelper.dbi("Twr_TwrId", rd);
                    decimal Twr_Stawka = DataHelper.dbd("Twr_Stawka", rd);
                    for (int i = 0; i < prices.Count; i++)
                    {
                        CenaPoziomu cena = new CenaPoziomu();
                        cena.PoziomId = prices[i].Id;
                        cena.ProduktId = Twr_TwrId;
                        TwrCeny tmp = c.FirstOrDefault(p => p.TwC_TwCNumer == i + 1 && p.TwC_TwrID == Twr_TwrId);

                        if (tmp == null)
                        {
                            cena.Netto = 0;
                        }
                        else if (tmp.TwC_Typ == 2)
                        {
                            //priceNetto = Decimal.Round(Price / (1 + VAT / 100), 2);
                            cena.Netto = Decimal.Round(tmp.TwC_Wartosc / (1 + Twr_Stawka / 100), 2);
                        }
                        else
                        {
                            cena.Netto = tmp.TwC_Wartosc;
                        }

                        ceny.Add(cena);
                    }
                }

                
                
            }
            finally
            {
                if (rd != null) { rd.Close(); rd.Dispose(); }
                if (cmd != null) cmd.Dispose();
                if (conn != null) { conn.Close(); conn.Dispose(); }
            }
            
            //List<Towary> produkty = db.Towaries.ToList(); //PobierzProdukty(out tlumaczeniatmp);
            //foreach (Towary v in produkty)
            //{
                
            //}

            LogiFormatki.PobierzInstancje.LogujInfo("Poziomów cenowych: " + ceny.Count);

            return ceny;
        }
        internal List<ProduktCecha> GetAttributesLacznik()
        {
        
            Dictionary<long, ProduktCecha> items = new Dictionary<long, ProduktCecha>();
            SqlConnection conn = null;
            SqlCommand cmd = null;
            SqlDataReader rd = null;
            try
            {
                conn = new SqlConnection(_config.ERPcs);
                conn.Open();
                string atrybutySQL = "select tA.TwA_TwrId, tA.TwA_DeAId, tA.TwA_WartoscTxt, dA.DeA_Kod from cdn.TwrAtrybuty tA join cdn.DefAtrybuty dA on tA.TwA_DeAId=dA.DeA_DeAId;";
                cmd = new SqlCommand(atrybutySQL, conn);
                rd = cmd.ExecuteReader();
                while (rd.Read())
                {
                    int? TwA_TwrId = DataHelper.dbin("TwA_TwrId", rd);
                    int TwA_DeAId = DataHelper.dbi("TwA_DeAId", rd);
                    string TwA_WartoscTxt = DataHelper.dbs("TwA_WartoscTxt", rd);
                    string DeA_Kod = DataHelper.dbs("DeA_Kod", rd);
                    if (TwA_TwrId.HasValue)
                    {
                        //outem zwracam wartosc
                        ProduktCecha cp = new ProduktCecha();
                        cp.ProduktId = TwA_TwrId.Value;
                        string sym = StworzSymbolCechey(DeA_Kod, TwA_WartoscTxt);
                        cp.CechaId = StworzIDCechy(TwA_DeAId, sym);
                        if (!items.ContainsKey(cp.Id))
                            items.Add(cp.Id, cp);
                    }
                }
                //kategorie
                rd.Close();
                rd.Dispose();
                cmd.Dispose();
                cmd = new SqlCommand("select TwG_GIDNumer, TwG_GrONumer from cdn.TwrGrupy where TwG_GIDTyp=16;", conn);
                rd = cmd.ExecuteReader();
                while (rd.Read())
                {
                    int TwG_GrONumer = DataHelper.dbi("TwG_GrONumer", rd);
                    int TwG_GIDNumer = DataHelper.dbi("TwG_GIDNumer", rd);


                    ProduktCecha cp = new ProduktCecha
                    {
                        CechaId = TwG_GrONumer + grupyoffset,
                        ProduktId = TwG_GIDNumer
                    };
                    if (!items.ContainsKey(cp.Id))
                        items.Add(cp.Id, cp);
                }
            }
            finally
            {
                if (rd != null) { rd.Close(); rd.Dispose(); }
                if (cmd != null) cmd.Dispose();
                if (conn != null) { conn.Close(); conn.Dispose(); }
            }





           
                    //var q = (from p in db.TwrAtrybuties
                    //    select
                    //        new
                    //        {
                    //            p.TwA_TwrId,
                    //            p.TwA_DeAId,
                    //            p.TwA_WartoscTxt
                    //        }).ToList();

                    //foreach (var cechyprodukty in q)
                    //{
                    //    if (cechyprodukty.TwA_TwrId.HasValue)
                    //    {
                    //        cechy_produkty cp = new cechy_produkty();
                    //        cp.produkt_id = cechyprodukty.TwA_TwrId.Value;
                    //        cp.cecha_id = StworzIDCechy(cechyprodukty.TwA_DeAId, cechyprodukty.TwA_WartoscTxt);
                    //        cp.rodzaj = -1;
                    //        if (!items.ContainsKey(cp.ZbudujKlucz()))
                    //            items.Add(cp.ZbudujKlucz(), cp);
                    //    }
                    //}
        

            //kategoria
         //   List<Towary> produkty = db.Towaries.ToList();
            //var qk = (from c in db.TwrGrupies
            //          where c.TwG_GIDTyp == 16
            //          select new { c.TwG_GIDNumer,  c.TwG_GrONumer }).ToList();

            ////foreach (Towary towar in produkty)
            ////{
            //  //  var g = qk.Where(a => a.TwG_GIDNumer == towar.Twr_TwrId).ToList();
            //    foreach (var gr in qk)
            //    {
            //        cechy_produkty cp = new cechy_produkty
            //        {
            //            cecha_id = gr.TwG_GrONumer + grupyoffset,
            //            produkt_id = gr.TwG_GIDNumer,
            //            rodzaj = -10
            //        };
            //        if (!items.ContainsKey(cp.ZbudujKlucz()))
            //            items.Add(cp.ZbudujKlucz(), cp);
            ////    }

            //}
           
            List<ProduktCecha> laczniki= items.Values.ToList();
            laczniki.AddRange(PobierzLacznikiDoMarekOrazProducentow());
            return laczniki;
        }

        private IEnumerable<ProduktCecha> PobierzLacznikiDoMarekOrazProducentow()
        {
            List<ProduktCecha> laczniki=new List<ProduktCecha>();
            SqlConnection conn = null;
            SqlCommand cmd = null;
            SqlDataReader rd = null;
         
            try
            {
               
                conn = new SqlConnection(_config.ERPcs);
                conn.Open();
                cmd = new SqlCommand("SELECT towar=Twr_TwrId,producent=Twr_PrdID,marka=Twr_MrkID  FROM CDN.Towary", conn);
                rd = cmd.ExecuteReader();
                while (rd.Read())
                {
                    int towar = DataHelper.dbi("towar", rd);
                    int? markaid = DataHelper.dbin("marka", rd);
                    int? producent = DataHelper.dbin("producent", rd);
                    if (markaid.HasValue)
                    {
                        laczniki.Add(new ProduktCecha{CechaId = markaid.Value+markioffset,ProduktId = towar});
                    }
                    if (producent.HasValue)
                    {
                        laczniki.Add(new ProduktCecha { CechaId = producent.Value + producnetoffset, ProduktId = towar });
                    }
                  
                }
             
            }
            finally
            {
                if (rd != null) { rd.Close(); rd.Dispose(); }
                if (cmd != null) { cmd.Dispose(); }
                if (conn != null) { conn.Close(); conn.Dispose(); }
            }
            return laczniki;
        }

        internal List<KlientKategoriaKlienta> KategorieKlientowPolaczenia()
        {
            List<KategoriaKlienta> katkl = GetCustomerCategories();
            List<KlientKategoriaKlienta> klienciKategorie = new List<KlientKategoriaKlienta>();
            SqlConnection conn = null;
            SqlCommand cmd = null;
            SqlDataReader rd = null;
            try
            {

                conn = new SqlConnection(_config.ERPcs);
                conn.Open();
                cmd = new SqlCommand("select k.Knt_Grupa,g.Gru_Opis, k.Knt_KntId, k.Knt_TerminPlat, k.Knt_Termin from cdn.Kontrahenci k join cdn.Grupy g on k.Knt_Grupa=g.Gru_Nazwa where k.Knt_PodmiotTyp=1;", conn);
                rd = cmd.ExecuteReader();
                while (rd.Read())
                {
                    string Knt_Grupa = DataHelper.dbs("Knt_Grupa", rd).Trim().ToLower(); 
                    string Gru_Opis = DataHelper.dbs("Gru_Opis", rd).Trim().ToLower();
                    int Knt_KntId = DataHelper.dbi("Knt_KntId", rd);
                    int Knt_TerminPlat = DataHelper.dbi("Knt_TerminPlat", rd);
                    int Knt_Termin = DataHelper.dbi("Knt_Termin", rd);

                    if (!string.IsNullOrEmpty(Knt_Grupa))
                    {
                        string grupa = string.IsNullOrEmpty(Gru_Opis) ? Knt_Grupa : Gru_Opis;
                        KategoriaKlienta kategoria = katkl.FirstOrDefault(a => a.Nazwa.ToLower() == grupa);
                        if (kategoria != null)
                        {
                            KlientKategoriaKlienta kk = new KlientKategoriaKlienta();
                            kk.KategoriaKlientaId = kategoria.Id;
                            kk.KlientId = Knt_KntId;
                            klienciKategorie.Add(kk);
                        }
                    }

                    if (Knt_Termin > 0 && Knt_TerminPlat == 1)
                    {
                        KategoriaKlienta kategoria = katkl.FirstOrDefault(a => a.Nazwa == Knt_Termin.ToString() && a.Grupa == "TERMIN PRZELEWU");
                        if (kategoria != null)
                        {
                            KlientKategoriaKlienta kk = new KlientKategoriaKlienta();
                            kk.KategoriaKlientaId = kategoria.Id;
                            kk.KlientId = Knt_KntId;
                            klienciKategorie.Add(kk);
                        }
                    }

                }

            }
            finally
            {
                if (rd != null) { rd.Close(); rd.Dispose(); }
                if (cmd != null) { cmd.Dispose(); }
                if (conn != null) { conn.Close(); conn.Dispose(); }
            }
            return klienciKategorie;
        }

        internal List<int> GetDocumentsToDelete(Dictionary<int, long> dokumentyNaPlatformie, HashSet<long> idKlientowB2b)
        {
            int docType = _config.OptimaTypDokumentu;
            db.CommandTimeout = 180;
            List<int> idstypy = new List<int> { docType, 302 };
            List<int> iddokow = dokumentyNaPlatformie.Select(a => a.Key).ToList();
            List<int> idserp = db.TraNags.Where(x => idstypy.Contains(x.TrN_TypDokumentu) && x.TrN_PodmiotTyp == 1 && x.TrN_PodID != (int?)null && x.TrN_Anulowany == 0).Select(x => x.TrN_TrNID).ToList();
            iddokow.RemoveAll(idserp.Contains);
            return iddokow;//do wywalenia te ktore były na b2b, ale nie znaleziono w erp
        }

        /// <summary>
        /// Sprawdza czy jest to optima w wersji 2015 w górę (zawiera tabelę DaneBinarneLinki)
        /// </summary>
        /// <returns></returns>
        private bool CzyJestNowaOptima()
        {
            string zapytanie = @"DECLARE @WYNIK INT;
  SET @WYNIK = 0;
  IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'CDN' 
                 AND  TABLE_NAME = 'DaneBinarneLinki'))
BEGIN
     SET @WYNIK = 1;
END
SELECT @WYNIK;";
            SqlConnection subConn = null;
            SqlCommand subcmd = null;
            int wynik = 0;
            try
            {
                subConn = new SqlConnection(_config.ERPcs);
                subConn.Open();
                subcmd =
                    new SqlCommand(zapytanie, subConn);

                wynik = (int)subcmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                log.Error(ex.Message + " " + ex.StackTrace);
                throw;
            }
            finally
            {
                if (subConn != null) subConn.Close();
                if (subcmd != null) subcmd.Dispose();
            }

            return wynik == 1;
        }

        const string SELECT_OPTIMA_BYTES = @"select dane=DAB_Wartosc  from cdn.danebinarne db where db.DAB_DABID={0}";

        internal void ZapiszZdjecia(string path, string separator, TypyPolDoDopasowaniaZdjecia polaZapisuZdjec)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            List<Plik> files = new List<Plik>();

            List<string> pliki = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories).ToList();

            foreach (string file in pliki)
            {
                FileInfo info = new FileInfo(file);
                Plik ptemp = new Plik { Data = info.LastWriteTime.ToUniversalTime().AddMilliseconds(-info.LastWriteTime.ToUniversalTime().Millisecond), Nazwa = info.Name, nazwaLokalna = info.Name, Rozmiar = (int)info.Length, Sciezka = info.DirectoryName + "\\", Id = 0 };
                files.Add(ptemp);
            }

            //jeśli jest to nowa optima to są wyciągane załączniki zamiast tylko danych binarnych z jednego atrybutu
            if (CzyJestNowaOptima())
            {
                ZapiszZdjeciaOptima2015(path, separator, polaZapisuZdjec, files,pliki);
            }
            else
            {
                ZapiszZdjeciaOptima2014(path, separator, polaZapisuZdjec, files, pliki);
            }
        }

        //wyciąga załączniki z optimy w wersji conajmniej 2015 czyli albo kopiuje fizyczne zdjęcie do podanego katalogu albo zapisuje dane binarne zdjęcia
        private void ZapiszZdjeciaOptima2015(string path, string separator, TypyPolDoDopasowaniaZdjecia polaZapisuZdjec, List<Plik> files, List<string> pliki)
        {
            log.Debug("Rozpoczynanie pobierania zdjęć z załączników z nowej Optimy");

            SqlConnection subConn = null;
            SqlCommand subcmd = null;
            try
            {
                subConn = new SqlConnection(_config.ERPcs);
                subConn.Open();
                subcmd =
                    new SqlCommand(
                        String.Format(
                            @"select idzdjecia=dbl.DBL_DBLID,produktid= t.Twr_TwrId,nazwa=t.Twr_Nazwa,symbol=t.Twr_Kod,ean=t.Twr_EAN, sciezka=dbl.DBL_Sciezka, binarne=DBL_WBazie, binarneID=DBL_DabId, kolejnosc=DBL_Lp from cdn.DaneBinarneLinki dbl
                                    join cdn.Towary t on dbl.DBL_TwrID = t.Twr_TwrId where dbl.DBL_Typ = 4"),
                        subConn);

                List<Plik> subiektPics = new List<Plik>(20000);
                using (SqlDataReader subrd = subcmd.ExecuteReader())
                {
                    while (subrd.Read())
                    {
                        int idzdjecia = DataHelper.dbi("idzdjecia", subrd);
                        string name = DataHelper.dbs("nazwa", subrd).Trim();
                        int towarid = DataHelper.dbi("produktid", subrd);
                        string ean = DataHelper.dbs("ean", subrd).Trim();
                        string symbol = DataHelper.dbs("symbol", subrd).Trim();
                        string sciezka = DataHelper.dbs("sciezka", subrd);
                        int? binarneID = DataHelper.dbin("binarneID", subrd);
                        bool czybinarne = DataHelper.dbi("binarne", subrd) == 1;
                        int kolejnoscZdjecia = DataHelper.dbi("kolejnosc", subrd);

                        bool main = kolejnoscZdjecia == 1;

                        string numerZdjecia = (kolejnoscZdjecia.ToString() + idzdjecia.ToString());

                        string nazwaZdjecia = TextHelper.PobierzInstancje.WygenerujNazweZdjecia(polaZapisuZdjec,separator, towarid, ean, symbol,  main, Convert.ToInt32(numerZdjecia));

                        log.DebugFormat("Nazwa nowego zdjęcia: {0}, w tym numer zdjęcia: {1} i kolejność: {2}",nazwaZdjecia, numerZdjecia, kolejnoscZdjecia);


                        Plik temp = new Plik
                        {
                            Id = binarneID.Value,
                            Nazwa = nazwaZdjecia
                        };
                        temp.Sciezka = path + "\\" + temp.Nazwa[0] + "\\";
                        temp.PoprawNazwaPlikuDlaURL();
                        temp.DoPobrania = false;

                        //sprawdzać czy fotka istnieje i różni się datą lub rozmiarem
                        if (!czybinarne && !string.IsNullOrEmpty(sciezka) && File.Exists(sciezka))
                        {
                            string nazwadozapisu = path + "\\" + nazwaZdjecia;

                            bool czykopiowac = false;
                            if (!File.Exists(nazwadozapisu))
                            {
                                czykopiowac = true;
                            }
                            else
                            {
                                FileInfo zrodlo = new FileInfo(sciezka);
                                FileInfo cel = new FileInfo(nazwadozapisu);

                                if (zrodlo.CreationTimeUtc != cel.CreationTimeUtc || zrodlo.Length != cel.Length)
                                    czykopiowac = true;
                            }

                            if (czykopiowac)
                                File.Copy(sciezka, nazwadozapisu);
                        }
                        else
                        {
                            temp.DoPobrania = true;
                        }

                        subiektPics.Add(temp);
                    }
                }

                log.DebugFormat("Wstępnie wybrano {0} zdjęć z Optimy", subiektPics.Count);

                for (int i = 0; i < subiektPics.Count; i++) // porownywanie zdjec
                {
                    if (!subiektPics[i].DoPobrania)
                        continue;

                    //upewniam się czy plik zdjęcia na pewno istnieje, jeśli nie to je pobieram
                    if (files.All(p => p.Nazwa != subiektPics[i].Nazwa))
                    {
                        try
                        {
                            if (subConn.State == ConnectionState.Closed)
                                subConn.Open();
                            subcmd = new SqlCommand(string.Format(SELECT_OPTIMA_BYTES, subiektPics[i].Id), subConn);
                            subiektPics[i].DanePlikBase64 = Convert.ToBase64String((byte[])subcmd.ExecuteScalar());
                            CreatePic(subiektPics[i]);
                            subiektPics[i].DanePlikBase64 = null;
                        }
                        catch (Exception ex)
                        {
                            log.Debug("Pobieranie zdjęcia z optimy " + subiektPics[i].Nazwa + " " + ex.Message + " " + ex.StackTrace);
                            //throw;
                        }
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
                            log.Error(new Exception(string.Format("Nie udało się usunąć pliku {0} z powodu błędu: {1}", plik, ex.Message), ex));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message + " " + ex.StackTrace);
                throw;
            }
            finally
            {
                if (subConn != null) subConn.Close();
                if (subcmd != null) subcmd.Dispose();
            }
        }

        private void ZapiszZdjeciaOptima2014(string path, string separator, TypyPolDoDopasowaniaZdjecia polaZapisuZdjec, List<Plik> files, List<string> pliki)
        {
            if (string.IsNullOrEmpty(_config.OptimaAtrybutZeZdjeciami))
            {
               throw new InvalidOperationException("Proszę podać nazwę atrybutu ze zdjęciami");
            }
            SqlConnection subConn = null;
            SqlCommand subcmd = null;
            try
            {
                subConn = new SqlConnection(_config.ERPcs);
                subConn.Open();
                subcmd =
                    new SqlCommand(
                        String.Format(
                            @"select idzdjecia=TwA_DABID,produktid= t.Twr_TwrId,nazwa=t.Twr_Nazwa,symbol=t.Twr_Kod,glowne=1,ean=t.Twr_EAN from cdn.TwrAtrybuty a
                                     join cdn.DefAtrybuty da on a.TwA_DeAId=da.DeA_DeAId join cdn.Towary t on a.TwA_TwrId=t.Twr_TwrId  
                                     where da.DeA_Kod=@atrybut and TwA_DABID is not null
                                     order by a.twa_twAid"),
                        subConn);
                subcmd.Parameters.AddWithValue("@atrybut", _config.OptimaAtrybutZeZdjeciami);
                List<Plik> subiektPics = new List<Plik>(10000);
                using (SqlDataReader subrd = subcmd.ExecuteReader())
                {
                    while (subrd.Read())
                    {
                        string symbol = DataHelper.dbs("symbol", subrd);
                        int idzdjecia = DataHelper.dbi("idzdjecia", subrd);
                        bool main = true;
                        string name = DataHelper.dbs("nazwa", subrd);
                        int towarid = DataHelper.dbi("produktid", subrd);
                        string ean = DataHelper.dbs("ean", subrd);

                        Plik temp = new Plik { Id = idzdjecia, Nazwa = TextHelper.PobierzInstancje.WygenerujNazweZdjecia(polaZapisuZdjec, separator, towarid, ean, symbol, main, idzdjecia) };
                        temp.Sciezka = path + "\\" + temp.Nazwa[0] + "\\";
                        temp.PoprawNazwaPlikuDlaURL();
                        subiektPics.Add(temp);
                    }
                } //wyciagniecie danych z suba

                log.DebugFormat("Wstępnie wybrano {0} zdjęć z Optimy", subiektPics.Count);

                for (int i = 0; i < subiektPics.Count; i++) // porownywanie zdjec
                {
                    //upewniam się czy plik zdjęcia na pewno istnieje, jeśli nie to je pobieram
                    if (files.All(p => p.Nazwa != subiektPics[i].Nazwa))
                    {
                        try
                        {
                            if (subConn.State == ConnectionState.Closed)
                                subConn.Open();
                            subcmd = new SqlCommand(string.Format(SELECT_OPTIMA_BYTES, subiektPics[i].Id), subConn);
                            subiektPics[i].DanePlikBase64 = Convert.ToBase64String((byte[])subcmd.ExecuteScalar());
                            CreatePic(subiektPics[i]);
                            subiektPics[i].DanePlikBase64 = null;
                        }
                        catch (Exception ex)
                        {
                            log.Debug("Pobieranie zdjęcia z optimy " + subiektPics[i].Nazwa + " " + ex.Message + " " + ex.StackTrace);
                            //throw;
                        }
                    }
                    //   log.DebugFormat("sub pic {0}",subiektPics[i].SciezkaWzgledna);
                }

                foreach (string plik in pliki)
                {
                  //  log.DebugFormat("plik {0}",plik);
                    if (subiektPics.All(a => a.SciezkaWzgledna != plik))
                    {
                        try
                        {
                            File.Delete(plik);
                        }
                        catch (Exception ex)
                        {
                            log.Error(new Exception(string.Format("Nie udało się usunąć pliku {0} z powodu błędu: {1}", plik, ex.Message), ex));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message + " " + ex.StackTrace);
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

        internal Rejestracja CreateCustomers(Rejestracja items)
        {
            IKontrahent klient = DodajKontrahenta(items);
            items.StatusEksportu = RegisterExportStatus.Exported;
            items.KlientId = klient.ID;
            return items;
        }

        public static bool SprawdzenieCzyJestKontrahentONIP(string nip)
        {
            if (string.IsNullOrEmpty(nip))
            {
                throw new Exception("NIP jest pusty!");
            }

            bool KlientJest = false;

            CDNHlmn.DokumentyHaMag Faktury = (CDNHlmn.DokumentyHaMag)Optima.PobierzInstancje.Sesja.CreateObject("CDN.DokumentyHaMag", null);

            OP_KASBOLib.Banki Banki = (OP_KASBOLib.Banki)Optima.PobierzInstancje.Sesja.CreateObject("CDN.Banki", null);
            CDNHeal.Kategorie Kategorie = (CDNHeal.Kategorie)Optima.PobierzInstancje.Sesja.CreateObject("CDN.Kategorie", null);

            CDNHeal.Kontrahenci Kontrahenci = (CDNHeal.Kontrahenci)Optima.PobierzInstancje.Sesja.CreateObject("CDN.Kontrahenci", null);
            try
            {
                if ((CDNHeal.IKontrahent)Kontrahenci["Knt_Nip='" + nip + "'"] != null)
                {
                    KlientJest = true;
                }
            }
            catch { }

            return KlientJest;
        }

        public bool CzyJestKlientOSymbolu(string symbol, CDNHeal.Kontrahenci kontrahenci)
        {
            if ((CDNHeal.IKontrahent)kontrahenci["Knt_Kod='" + symbol + "'"] == null)
            {
                return false;
            }

            return true;
        }

        public CDNHeal.IKontrahent DodajKontrahenta(Rejestracja o)
        {
            OP_KASBOLib.Banki Banki = (OP_KASBOLib.Banki)Optima.PobierzInstancje.Sesja.CreateObject("CDN.Banki", null);
            CDNHeal.Kategorie Kategorie = (CDNHeal.Kategorie)Optima.PobierzInstancje.Sesja.CreateObject("CDN.Kategorie", null);

            CDNHeal.Kontrahenci Kontrahenci = (CDNHeal.Kontrahenci)Optima.PobierzInstancje.Sesja.CreateObject("CDN.Kontrahenci", null);

            if (!string.IsNullOrEmpty(o.Nip))
            {
                log.Info("Szukanie klienta po nip = " + o.Nip);
                //forma taka jak wpisana przez czlowieka
                try
                {
                    if ((CDNHeal.IKontrahent)Kontrahenci["Knt_Nip='" + o.Nip + "'"] != null)
                    {
                        return (CDNHeal.IKontrahent)Kontrahenci["Knt_Nip='" + o.Nip + "'"];
                    }
                }
                catch { }
              
                try
                {
                    if ((CDNHeal.IKontrahent)Kontrahenci["Knt_Nip='" + o.Nip + "'"] != null)
                    {
                        return (CDNHeal.IKontrahent)Kontrahenci["Knt_Nip='" + o.Nip + "'"];
                    }
                }
                catch { }
            }

            // sprawdzenie po mailu
            string email = o.Email;
            if (!string.IsNullOrEmpty(email))
            {
                log.Info("Szukanie klienta po emailu = " + email);
                try
                {
                    if ((CDNHeal.IKontrahent)Kontrahenci["Knt_Email='" + email + "'"] != null)
                    {
                        return (CDNHeal.IKontrahent)Kontrahenci["Knt_Email='" + email + "'"];
                    }
                }
                catch { }
            }
            //nowy
            log.Info("Dodawanie nowego kontrahenta");
            CDNHeal.IKontrahent Kontrahent = (CDNHeal.IKontrahent)Kontrahenci.AddNew(null);
            CDNHeal.IAdres Adres = Kontrahent.Adres;

            //ustalenie akronimu
            string akronim = "b2b_" + o.Id;

            try
            {
                if ((CDNHeal.IKontrahent)Kontrahenci["Knt_Kod='" + akronim + "'"] != null)
                {
                    throw new Exception(
                    string.Format(
                        "Klient o kodzie {0} nie mógł zostać zapisany. Nie udało się utworzyć unikalnego akronimu. Spróbuj zmienić w ustawieniach programu Solex Sync prefix dla nowych kontrahentów na krótszy.",
                        akronim));
                }
            }
            catch
            {
                //jeśli się wywali to ok, nie ma takiego klienta
            }

            try
            {
                string telefon = o.Telefon;
                if (!string.IsNullOrEmpty(telefon))
                    Kontrahent.Telefon = telefon;

            }
            catch (NullReferenceException)
            {
                log.Info(string.Format("Nie znaleziono numeru telefonu dla kontrahenta {0}", akronim));
            }
            catch (Exception ex)
            {
                log.Error("Nie udało się pobrać numeru telefonu z powodu błędu: " + ex.Message, ex);
            }

            log.Info("Znaleziono wolny akronim: " + akronim);

            Adres.KodPocztowy = o.KodPocztowy;

            Adres.Kraj = o.Panstwo;

            Adres.Miasto = o.Miasto;

            Adres.Ulica = o.Ulica;

            Kontrahent.Akronim = akronim;
            Kontrahent.Email = o.Email;
            Kontrahent.Nazwa1 = o.Nazwa;
            Kontrahent.Nazwa2 = o.ImieNazwisko;
            //            Kontrahent.Kategoria = (CDNHeal.Kategoria)Kategorie["Kat_KodSzczegol='KLIENCI DETALICZNI'");
            //Kontrahent.Grupa = "KLIENCI DETALICZNI";
            Kontrahent.Rodzaj_Dostawca = 0;
            Kontrahent.Rodzaj_Konkurencja = 0;
            Kontrahent.Rodzaj_Partner = 0;
            Kontrahent.Rodzaj_Potencjalny = 0;
            Kontrahent.Rodzaj_Odbiorca = 1;
            //czy klient jest firmą?


            log.Info("Ustawianie NIP: " + o.Nip);
            //Kontrahent.Nip = o.NIP ?? "";
            int pod = string.IsNullOrEmpty(o.Nip) ? 0 : 1;
            Kontrahent.Finalny = pod;
            Kontrahent.NumerNIP.Nip = o.Nip ?? "";
            Kontrahent.NumerNIP.NIPKraj = o.Nip == null ? "" : "PLN";
            Kontrahent.PodatekVAT = o.Nip == null ? 1 : 0;

            
            //try
            //{
            //    Kontrahent.PodatekVAT = 1;
            //}
            //catch (Exception ex)
            //{
            //    log.InfoFormat("Błąd: {0}", ex.Message);
            //}
            
            //else
            //{
            //    Kontrahent.Finalny = 1;
            //    Kontrahent.PodatekVAT = 0;
            //}
            //Kontrahent.Finalny =pod;
            

            //if (!string.IsNullOrEmpty(o.NIP))
            //{
            //    try
            //    {
            //        log.Info("Ustawianie NIP: " + o.NIP);
            //        log.Info("Ustawianie NIP: " + o.NIP);
            //        CDNHeal.INumerNIP NumerNIP = Kontrahent.NumerNIP;
            //        NumerNIP.UstawNIP("PL", o.NIP, 0);
            //        //Kontrahent.PodatekVAT = 1;
            //        //log.Info("Ustawianie NIP: " + o.NIP);
            //        //NumerNIP NumerNIP = new NumerNIPClass();
            //        //log.InfoFormat("1");
            //        //Kontrahent.Nip = "";
            //        //NumerNIP.UstawNIP("PL", o.NIP, 0);
            //        //log.InfoFormat("2");
            //        Kontrahent.Finalny = 1;
            //        Kontrahent.PodatekVAT = 1;
            //    }
            //    catch (Exception ex)
            //    {
            //        log.ErrorFormat("Błąd: {0}", ex.Message);
            //    }
                
            //}
            //else
            //{
            //    //klient detaliczny
            //    Kontrahent.Finalny = 1;
            //    Kontrahent.PodatekVAT = 0;
            //}

            try
            {
                log.Info("Zapisywanie klienta");
                Optima.PobierzInstancje.Sesja.Save();
                log.Info("Klient zapisany");
            }
            catch (System.Runtime.InteropServices.COMException ex)
            {
                log.Error(string.Format(
                    "Klient nie mógł zostać zapisany - sprawdź poprawność danych. W szczególności sprawdź NIP! NIP: {0}, akronim:{1}, Treść błędu: {2}", o.Nip, akronim, ex.Message), ex);
                throw;
            }

            catch (Exception ex)
            {
                log.Error("Klient nie mógł zostać zapisany - sprawdź poprawność danych. W szczególności sprawdź NIP!",
                          ex);
                throw;
            }

            return Kontrahent;
        }

        internal List<Kraje> PobierzKraje()
        {
            //List<string> q = db.Kontrahencis.Where(a => a.Knt_PodmiotTyp == 1).DistinctBy(a => a.Knt_Kraj).Select(a => a.Knt_Kraj).ToList().Where(a=> !string.IsNullOrEmpty(a)).ToList();
            Dictionary<int, Kraje> kraje = new Dictionary<int, Kraje>();
            SqlConnection conn = null;
            SqlCommand cmd = null;
            SqlDataReader rd = null;
            string KrajeSQL = "select distinct Knt_Kraj from cdn.Kontrahenci where Knt_PodmiotTyp=1 and Knt_Kraj!='';";
            try
            {
                conn = new SqlConnection(_config.ERPcs);
                conn.Open();

                cmd=new SqlCommand(KrajeSQL,conn);
                rd = cmd.ExecuteReader();
                while (rd.Read())
                {
                    string Knt_Kraj = DataHelper.dbs("Knt_Kraj", rd);


                    int id = WygenerujIdDlaStringa(Knt_Kraj);

                    Kraje nowyKraj = new Kraje();
                    nowyKraj.Id = id;
                    nowyKraj.Nazwa = Knt_Kraj;
                    nowyKraj.Widoczny = true;
                    nowyKraj.Synchronizowane = true;

                    if (!kraje.ContainsKey(id))
                        kraje.Add(id, nowyKraj);
                }
            }
            finally
            {
                if (rd != null) { rd.Close(); rd.Dispose(); }
                if (cmd != null) cmd.Dispose();
                if (conn != null) { conn.Close(); conn.Dispose(); }
            }
            return kraje.Values.ToList();
        }

        internal List<Region> PobierzWojewodztwa()
        {
            Dictionary<string, Kraje> kraje = PobierzKraje().ToDictionary(a => a.Nazwa.ToLower(), a => a);
            Dictionary<int, Region> regiony = new Dictionary<int, Region>();
            SqlConnection conn = null;
            SqlCommand cmd = null;
            SqlDataReader rd = null;
            string wojewodztwaSQL = "select distinct Knt_Wojewodztwo, Knt_Kraj from cdn.Kontrahenci where Knt_PodmiotTyp=1 and Knt_Wojewodztwo!='' and Knt_Kraj!='';";
            try
            {
                conn = new SqlConnection(_config.ERPcs);
                conn.Open();

                cmd = new SqlCommand(wojewodztwaSQL, conn);
                rd = cmd.ExecuteReader();
                while (rd.Read())
                {
                    string Knt_Kraj = DataHelper.dbs("Knt_Kraj", rd);
                    string Knt_Wojewodztwo = DataHelper.dbs("Knt_Wojewodztwo", rd);
                    int id = WygenerujIdDlaStringa(Knt_Wojewodztwo);

                    Region nowyRegion = new Region();
                    nowyRegion.Id = id;
                    nowyRegion.Nazwa = Knt_Wojewodztwo;
                    nowyRegion.Widoczny = true;
                    nowyRegion.Synchronizowane = true;

                    if (kraje.ContainsKey(Knt_Kraj.ToLower()))
                        nowyRegion.KrajId = kraje[Knt_Kraj.ToLower()].Id;


                    if (!regiony.ContainsKey(id))
                        regiony.Add(id, nowyRegion);
                }
            }
            finally
            {
                if (rd != null) { rd.Close(); rd.Dispose(); }
                if (cmd != null) cmd.Dispose();
                if (conn != null) { conn.Close(); conn.Dispose(); }
            }

            //var q = db.Kontrahencis.Where(a => a.Knt_PodmiotTyp == 1).DistinctBy(a => a.Knt_Wojewodztwo).Select(a => new {a.Knt_Wojewodztwo, a.Knt_Kraj}).ToList().Where(a=>  !string.IsNullOrEmpty(a.Knt_Wojewodztwo)).ToList();
            
            //foreach (var region in q)
            //{
                
            //}

            return regiony.Values.ToList();
        }

        private int WygenerujIdDlaStringa(string dane)
        {
            return dane.ToLower().WygenerujIDObiektuSHA(1);
        }

        public Dictionary<int, Dictionary<string, string>> PobierzNrListowPrzewozowych(HashSet<int> dokumentyID)
        {
            string nazwaPola = "NrListuPrzewozowego";
            db.CommandTimeout = 180;
            var zk = (from r in db.TraNags
                      //where dokumentyID.Contains(r.TrN_TrNID)
                      select new
                      {
                          r.TrN_TrNID,
                          r.TrN_NrListuPrzewozowego
                      }).ToList();


            Dictionary<int, Dictionary<string, string>> listyprzewozowe = new Dictionary<int, Dictionary<string, string>>();

            foreach (var list in zk)
            {
                if (string.IsNullOrEmpty(list.TrN_NrListuPrzewozowego) || !dokumentyID.Contains(list.TrN_TrNID))
                    continue;

                Dictionary<string,string> pola = new Dictionary<string, string>(1);
                pola.Add(nazwaPola, list.TrN_NrListuPrzewozowego);

                listyprzewozowe.Add(list.TrN_TrNID, pola);
            }

            return listyprzewozowe;
        }

        internal List<ProduktyZamienniki> PobierzZamienniki(long produkt)
        {
            List<Zamienniki> zamienniki = db.Zamiennikis.Where(a => a.ZAM_TwrId == produkt).OrderBy(a=> a.ZAM_Lp).ToList();

            List<ProduktyZamienniki> listazamiennikow = new List<ProduktyZamienniki>();

            foreach (Zamienniki zamiennik in zamienniki)
            {
                ProduktyZamienniki pz = new ProduktyZamienniki();
                pz.ProduktId = zamiennik.ZAM_TwrId;
                pz.ZamiennikId = zamiennik.ZAM_ZamTwrId;

                listazamiennikow.Add(pz);
            }

            return listazamiennikow;
        }

        internal List<ProduktyKodyDodatkowe> PobierzAlternatywneKodyKreskowe()
        {
            List<ProduktyKodyDodatkowe> items = new List<ProduktyKodyDodatkowe>();
            using (SqlConnection conn = new SqlConnection(_config.ERPcs))
            {
                conn.Open();

                using (SqlCommand command = new SqlCommand("select k.TwE_TwEId, k.TwE_TwrID, k.TwE_EAN from cdn.TwrEan k join cdn.Towary t on k.TwE_TwrID=t.Twr_TwrId where t.Twr_NieAktywny=0;"))//deklaracje atrybutów produktu
                {
                    command.Connection = conn;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = DataHelper.dbi("TwE_TwEId", reader);
                            int produkt_id = DataHelper.dbi("TwE_TwrID", reader);
                            string kod = DataHelper.dbs("TwE_EAN", reader);
                            items.Add(new ProduktyKodyDodatkowe() { Id = id, ProduktId = produkt_id, Kod = kod });
                        }
                    }
                }
                conn.Close();
            }
            return items;
        }
    }
}
