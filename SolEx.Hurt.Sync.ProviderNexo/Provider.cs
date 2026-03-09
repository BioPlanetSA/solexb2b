using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using log4net;
using SolEx.Hurt.Core.Sync;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Interfaces;
using SolEx.Hurt.Model.Interfaces.Sync;

namespace SolEx.Hurt.Sync.ProviderNexo
{
    public class Provider : ISyncProvider
    {
        private IConfigSynchro _config;
        private const int grupyoffset = 100000;
        public void UstawParametryLaczenia(IConfigSynchro config)
        {
            _config = config;
        }
        private ILog log
        {
            get
            {
                return LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            }
        }
        public string SourceCS { get; set; }

        public List<Produkt> PobierzProdukty(out List<Tlumaczenie> tlumaczenia, out List<JednostkaProduktu> jednostki, HashSet<string> magazyny)
        {
            string zapytanie = "select idObjektu = a.Id, pw.*,apw.* from ModelDanychContainer.Asortymenty a left join ModelDanychContainer.Asortymenty_PolaWlasneAsortyment_Adv pw on (a.Id=pw.Id) " +
                               "join ModelDanychContainer.Asortymenty_PolaWlasneAsortyment apw on(a.Id=apw.Id)";
            Dictionary<long, Dictionary<string, string>> slownikPolWlasnych = PobierzPolaWlasnie(zapytanie);
            string z =
                "   select sv.Stawka,kk.Kod,a.* from ModelDanychContainer.Asortymenty a left join ModelDanychContainer.KodyKreskowe kk on (a.Id=kk.JednostkaMiaryAsortymentu_Id)  join " +
                "ModelDanychContainer.StawkiVat sv on (a.StawkaVatSprzedaz_Id=sv.Id) where a.IsInRecycleBin=0 and 1=(case when @visible=1 then a.SklepInternetowy  " +
                "else (case when @visible=2 then a.SerwisAukcyjny  " +
                "else (case when @visible=3 then a.SprzedazMobilna  " +
                "else 1 end) end) end)";

            tlumaczenia = new List<Tlumaczenie>();
            jednostki = new List<JednostkaProduktu>();
            List<Produkt> listaProduktow = new List<Produkt>();
            if (!CzyWyciagamyProdukty())
            {
                return listaProduktow;
            }
            int visibility = (int)_config.SubiektWidocznoscTowarow;
           
             using (SqlConnection conn = new SqlConnection(_config.ERPcs))
            {
                conn.Open();
                using (SqlCommand command = new SqlCommand(z))
                {
                    command.Connection = conn;
                    command.Parameters.AddWithValue("@visible", visibility);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = DataHelper.dbi("Id", reader);
                            string nazwa = DataHelper.dbs("Nazwa", reader);
                            string opis = DataHelper.dbs("Opis", reader);
                            string PKWiU = DataHelper.dbs("PKWiU", reader);
                            string kod = DataHelper.dbs("Kod", reader);
                            string symbol = DataHelper.dbs("Symbol", reader);
                            decimal stawka = DataHelper.dbd("Stawka", reader);
                            string WWW = DataHelper.dbs("StronaWWW", reader);
                            int PodlegaOdwrotnemuObciazeniu = DataHelper.dbi("PodlegaOdwrotnemuObciazeniu", reader);
                            int rodzaj = DataHelper.dbi("Rodzaj_Id", reader);

                            Dictionary<string, string> pars = slownikPolWlasnych[id];
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                string nazwapola = reader.GetName(i);
                                if (nazwapola == "Id" || nazwapola == "TimeStamp" )
                                {
                                    continue;
                                }
                                object val = reader[i];
                                pars.Add(nazwapola, val == null ? "" : val.ToString());
                            }

                            Produkt item = new Produkt();
                            item.UstawWidocznoscProduktu(true);
                            item.KodKreskowy = !string.IsNullOrEmpty(kod) ? kod : null;
                            item.Id = id;
                            if (rodzaj == 1)
                                item.Typ = TypProduktu.Usluga;
                            item.PKWiU = PKWiU;
                            item.Www = WWW;
                            item.Vat = stawka;
                            item.VatOdwrotneObciazenie = PodlegaOdwrotnemuObciazeniu == 1;
                            item.Opis = opis;


                            _config.SynchronizacjaPobierzObjetoscProduktu(item, "", pars);
                            _config.SynchronizacjaPobierzWageProduktu(item, "", pars);
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
                            _config.SynchronizacjaPobierzIloscWOpakowaniu(item, "", pars);
                            _config.SynchronizacjaPobierzPoleDostawa(item, "", pars,null);
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
                                _config.SynchronizacjaPobierzLokalizacjeNazwa(item, j, "Nazwa", pars, ref tlumaczenia);
                                _config.SynchronizacjaPobierzLokalizacjeKod(item, j, "Symbol", pars, ref tlumaczenia);
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
                            if (string.IsNullOrEmpty(item.Nazwa))
                            {
                                item.Nazwa = nazwa;
                            }
                            if (string.IsNullOrEmpty(item.Kod))
                            {
                                item.Kod = symbol;
                            }
                            listaProduktow.Add(item);
                        }
                    }
                }
                string zapytanieJednostki = " select jma.Asortyment_Id,jma.JednostkaMiary_Id, jm.Nazwa, glowna =case when jma.AsortymentPodstawowej_Id is null then 0 else 1 end, przelicznik = case when p.LiczbaJednostkiNadrzednej is null then 1 else p.LiczbaJednostkiNadrzednej end  from ModelDanychContainer.JednostkiMiarAsortymentow jma join ModelDanychContainer.JednostkiMiar jm " +
                                            "on (jm.Id=jma.JednostkaMiary_Id) left join ModelDanychContainer.PrzelicznikiJednostekMiarAsortymentu p on (jma.Id = p.JednostkaPodrzedna_Id)";

                using (SqlCommand command = new SqlCommand(zapytanieJednostki))
                {
                    command.Connection = conn;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = DataHelper.dbi("JednostkaMiary_Id", reader);
                            string nazwa = DataHelper.dbs("Nazwa", reader).Trim();
                            int idProduktu = DataHelper.dbi("Asortyment_Id", reader);
                            bool podstawowa = DataHelper.dbi("glowna",reader) == 1;
                            decimal przelicznik = DataHelper.dbd("przelicznik", reader);


                            JednostkaProduktu jp = new JednostkaProduktu
                            {
                                Podstawowa = podstawowa,
                                Id = id,
                                Nazwa = nazwa,
                                ProduktId = idProduktu,
                                Przelicznik = przelicznik
                            };
                            jednostki.Add(jp);
                        }
                    }
                }


                conn.Close();
            }
            return listaProduktow;
        }

        //private int PobierzIdMagazynu(string symbol = null)
        //{
        //    string zapytanie;
        //    if (string.IsNullOrEmpty(symbol))
        //    {
        //        zapytanie = "select Id from ModelDanychContainer.Magazyny where MagazynGlowny=1;";
        //    }
        //    else
        //    {
        //        zapytanie = string.Format("select Id from ModelDanychContainer.Magazyny where symbol like '{0}';",symbol);
        //    }
        //    using (SqlConnection conn = new SqlConnection(_config.ERPcs))
        //    {
        //        conn.Open();
        //        //Wyciagamy nazwy atrybutow
        //        using (SqlCommand command = new SqlCommand(zapytanie))
        //        {
        //            command.Connection = conn;
        //            using (SqlDataReader reader = command.ExecuteReader())
        //            {
        //                while (reader.Read())
        //                {
        //                    id
        //                }
        //            }
        //        }
        //        conn.Close();
        //    }



        //}

        private bool CzyWyciagamyProdukty()
        {
            return _config.SubiektWidocznoscTowarow != WidcznoscProduktowWSubiekcie.ZadenProduktNieJestPobierany;
        }
        public List<ProduktCecha> PobierzCechyProduktow_Polaczenia(int[] atrybutydlaktorychpobieramycechy)
        {
            List<ProduktCecha> result = new List<ProduktCecha>();
            using (SqlConnection conn = new SqlConnection(_config.ERPcs))
            {
                conn.Open();
                //Wyciagamy laczniki do grup
                using (SqlCommand command = new SqlCommand("select Id,Grupa_Id from ModelDanychContainer.Asortymenty;"))
                {
                    command.Connection = conn;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int idProduktu = DataHelper.dbi("Id", reader);
                            int grupaId = DataHelper.dbi("Grupa_Id", reader);
                            result.Add(new ProduktCecha(idProduktu,grupaId+grupyoffset));
                            
                        }
                    }
                }
                //Pobieramy laczniki do cech
                using (SqlCommand command = new SqlCommand("  select Cechy_Id,Asortymenty_Id from ModelDanychContainer.CechyAsortymentuAsortyment;"))
                {
                    command.Connection = conn;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int produktId = DataHelper.dbi("Asortymenty_Id", reader);
                            int cechaId = DataHelper.dbi("Cechy_Id", reader);
                            result.Add(new ProduktCecha(produktId,cechaId));
                        }
                    }
                }

                conn.Close();
            }
            return result;
        }

        public List<KategoriaKlienta> PobierzKategorieKlientow()
        {
            throw new NotImplementedException();
        }

        public List<KlientKategoriaKlienta> PobierzKategorieKlientowPolaczenia()
        {
            return new List<KlientKategoriaKlienta>();
        }


        private Dictionary<long, Dictionary<string, string>> PobierzPolaWlasnie(string zapytanie)
        {
            Dictionary<long, Dictionary<string, string>> wynik = new Dictionary<long, Dictionary<string, string>>();
            using (SqlConnection conn = new SqlConnection(_config.ERPcs))
            {
                conn.Open();
                //Wyciagamy nazwy atrybutow
                using (SqlCommand command = new SqlCommand(zapytanie))
                {
                    command.Connection = conn;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                int idPodmiotu = DataHelper.dbi("idObjektu", reader);
                                string nazwapola = reader.GetName(i);
                                if (nazwapola == "Id" || nazwapola == "TimeStamp" || nazwapola == "idObjektu")
                                {
                                    continue;
                                }
                                object val = reader[i];
                                if (!wynik.ContainsKey(idPodmiotu))
                                {
                                    wynik.Add(idPodmiotu, new Dictionary<string, string>());
                                }
                                wynik[idPodmiotu].Add(nazwapola, val.ToString());
                            }
                        }
                    }
                }
                conn.Close();
            }
            return wynik;
        } 


        public Dictionary<long, Klient> PobierzKlientow(List<Klient> klienciNaPlatformie, out Dictionary<Adres, KlientAdres> adresy)
        {
            adresy=new Dictionary<Adres, KlientAdres>();
            Dictionary<long, Klient> items = new Dictionary<long, Klient>();
            string zapytanie = "  select idObjektu = a.Id, pw.*, apw.* from ModelDanychContainer.Podmioty a left join ModelDanychContainer.Podmioty_PolaWlasnePodmiot_Adv pw on (a.Id=pw.Id) " +
                               "left join ModelDanychContainer.Podmioty_PolaWlasnePodmiot apw on(apw.Id=a.Id);";
            Dictionary<long, Dictionary<string, string>> slownikPolWlasnych = PobierzPolaWlasnie(zapytanie);
            using (SqlConnection conn = new SqlConnection(_config.ERPcs))
            {
                conn.Open();
                //Wyciagamy nazwy atrybutow
                using (SqlCommand command = new SqlCommand("select  p.*,w.symbol, " +
                                                           "email = (select top 1 Wartosc from ModelDanychContainer.Kontakty where Rodzaj_Id=3 and Podmiot_Id=p.Id), " +
                                                           "strona = (select top 1 Wartosc from ModelDanychContainer.Kontakty where Rodzaj_Id=4 and Podmiot_Id=p.Id)		" +
                                                           "from ModelDanychContainer.Podmioty p join ModelDanychContainer.Waluty w on (p.Podmiot_DomyslnaWalutaSprzedazy_Id=w.Id)"))
                {
                    command.Connection = conn;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int idKlienta = DataHelper.dbi("Id", reader);
                            string nazwaKlienta = DataHelper.dbs("NazwaSkrocona", reader);
                            string nip = DataHelper.dbs("NIP", reader);
                            string telefon = DataHelper.dbs("Telefon", reader);
                            int? idPoziomuCeny = DataHelper.dbin("Podmiot_PoziomCen_Id", reader);
                            string symbolWaluty = DataHelper.dbs("symbol", reader);
                            string symbolKlienta = DataHelper.dbs("Sygnatura_PelnaSygnatura", reader);
                            string email = DataHelper.dbs("email", reader);
                            int? poziomCenyKlienta = DataHelper.dbin("Podmiot_PoziomCen_Id", reader);
                            if (string.IsNullOrEmpty(email))
                            {
                                continue;
                            }

                            Klient item = new Klient(idKlienta);
                            item.Nazwa = nazwaKlienta.Trim();
                            item.Nip = nip.Trim();
                            item.Telefon = telefon;
                            item.Symbol = symbolKlienta;
                            item.PoziomCenowyId = idPoziomuCeny;
                            item.WalutaId = symbolWaluty.WygenerujIDObiektuSHAWersjaLong();
                            item.Email = email;


                            Dictionary<string, string> pars = slownikPolWlasnych[item.Id];
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                string nazwapola = reader.GetName(i);
                                if (nazwapola == "Id" || nazwapola == "TimeStamp")
                                {
                                    continue;
                                }
                                object val = reader[i];
                                pars.Add(nazwapola, val == null ? "" : val.ToString());
                            }
                            
                            _config.SynchronizacjaPobierzPoleHasloZrodlowe(item, "", pars);
                            if (_config.EksportTylkoKontZHaslem && string.IsNullOrEmpty(item.HasloZrodlowe))
                            {
                                continue;
                            }
                            _config.SynchronizacjaPobierzDostepneMagazyny(item, "", pars);
                            _config.SynchronizacjaPobierzKredytWykorzystano(item, "", pars);
                            _config.SynchronizacjaPobierzKredytPozostalo(item, "", pars);
                            _config.SynchronizacjaPobierzPoleJezyk(item, "", pars);
                            _config.SynchronizacjaPobierzPoleSkype(item, "", pars);
                            _config.SynchronizacjaPobierzPoleGaduGadu(item, "", pars);
                            _config.SynchronizacjaPobierzPoleTekst1(item, "", pars);
                            _config.SynchronizacjaPobierzPoleTekst2(item, "", pars);
                            _config.SynchronizacjaPobierzPoleTekst3(item, "", pars);
                            _config.SynchronizacjaPobierzPoleTekst4(item, "", pars);
                            _config.SynchronizacjaPobierzPoleTekst5(item, "", pars);
                            _config.SynchronizacjaPobierzPoleMagazynDomyslny(item, "", pars);
                            _config.SynchronizacjaPobierzPoleIndywidualnaStawaVat(item, "", pars,false,false);
                            _config.SynchronizacjaPobierzPoleDomyslnaWaluta(item, "", symbolWaluty, pars);
                            _config.SynchronizacjaPobierzPoleBlokadaZamowien(item, "", pars);
                            _config.SynchronizacjaPobierzMinimalnaWartoscZamowienia(item, "", pars);
                            _config.SynchronizacjaPobierzPoleOpiekun(item, "", pars);
                            _config.SynchronizacjaPobierzPoleDrugiOpiekun(item, "", pars);
                            _config.SynchronizacjaPobierzPolePrzedstawiciel(item, "", pars);
                            _config.SynchronizacjaPobierzPoleKlientNadrzedny(item, "", pars);
                            _config.SynchronizacjaPobierzPoleEmail(item, email);
                            _config.SynchronizacjaUstawPoziomCeny(item, poziomCenyKlienta != null && poziomCenyKlienta != 0 ? poziomCenyKlienta.Value : (int?)null);
                            items.Add(item.Id, item);



                        }
                    }
                }
                //Pobieramy Adresy
                using (SqlCommand command = new SqlCommand("select ap.Podmiot_Id,x.Miejscowosc,x.KodPocztowy, x.Wojewodztwo_Id,x.Ulica, x.NrDomu, x.NrLokalu,a.Panstwo_Id, " +
                                                           "(case when ap.PodmiotAdresuDostaw_Id is not null  then 2 else (case when ap.PodmiotAdresuKorespondencyjnego_Id is not null then 3 " +
                                                           "else (case when ap.PodmiotAdresuPodstawowego_Id is not null then 1 end) end) end) as RodzajAdresu " +
                                                           "from (select * from ModelDanychContainer.AdresySzczegoly where Ulica not like '') x  " +
                                                           "join ModelDanychContainer.Adresy_AdresPodmiotu ap on (ap.Id=x.Adres_Id) " +
                                                           "join ModelDanychContainer.Adresy a on (a.Id=x.Adres_Id);"))
                {
                    command.Connection = conn;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string miejscowosc = DataHelper.dbs("Miejscowosc", reader);
                            string kodPocztowy = DataHelper.dbs("KodPocztowy", reader);
                            string ulica = DataHelper.dbs("Ulica", reader);

                            int podmiotId = DataHelper.dbi("Podmiot_Id", reader);
                            string nrDomu = DataHelper.dbs("NrDomu", reader);
                            int? wojwodztwo = DataHelper.dbin("Wojewodztwo_Id", reader);
                            string nrLokalu = DataHelper.dbs("NrLokalu", reader);
                            int? panstwo = DataHelper.dbin("Panstwo_Id", reader);
                            int rodzajAdresu = DataHelper.dbi("RodzajAdresu", reader);
                            TypAdresu typ = TypAdresu.Brak;
                            switch (rodzajAdresu)
                            {
                                case 1:
                                    typ=TypAdresu.Glowny;
                                    break;
                                case 2:
                                    typ = TypAdresu.Wysylki;
                                    break;
                                case 3:
                                    typ = TypAdresu.Korespondencyjny;
                                    break;
                            }
                            string ulicaKlienta = string.Empty;
                            if (string.IsNullOrEmpty(ulica) || string.IsNullOrEmpty(nrLokalu))
                            {
                                string wartosc = string.IsNullOrEmpty(nrDomu) ? nrLokalu : nrDomu;
                                ulicaKlienta = string.IsNullOrEmpty(wartosc)? ulica: string.Format("{0} {1}", ulica, wartosc);
                            }
                            else
                            {
                                ulicaKlienta = string.Format("{0} {1}/{2}", ulica, nrDomu, nrLokalu);
                            }
                            
                            Adres adres = new Adres(0, ulicaKlienta, kodPocztowy, miejscowosc, panstwo, wojwodztwo);
                            adres.Id = adres.WygenerujIDObiektu();
                            adres.TypAdresu = typ;
                            if (!items.ContainsKey(podmiotId))
                            {
                                continue;
                            }
                            adresy.Add(adres, new KlientAdres() { KlientId = podmiotId, AdresId = adres.Id, TypAdresu = typ });
                        }
                    }
                }

                conn.Close();
            }
            return items;
        }

        public List<Cecha> PobierzCechyIAtrybuty(out List<Atrybut> atrybuty, int[] atrybutydlaktorychniepobieramycechy)
        {
            log.Debug("Początek pobierania cech");
            List<Cecha> listaCech = new List<Cecha>();
            Dictionary<int, Atrybut> slownikAtrybutow = new Dictionary<int, Atrybut>();
            string nazwaAtrybutu = _config.AtrybutKategoriiZERP;
            string cechaAuto = _config.CechaAuto;
            bool atrybutZCechy = _config.AtrybutZCechy;

            using (SqlConnection conn = new SqlConnection(_config.ERPcs))
            {
                conn.Open();
                //Wyciagamy nazwy atrybutow
                using (SqlCommand command = new SqlCommand("select Id, Nazwa from ModelDanychContainer.GrupyAsortymentu"))
                {
                    command.Connection = conn;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int idGrupy = DataHelper.dbi("Id", reader);
                            string nazwa = DataHelper.dbs("Nazwa", reader);
                            Atrybut atrybut;
                            Cecha c = SubiektBaza.PobierzInstancje.StworzCecheZGrupy(out atrybut, grupyoffset,new KeyValuePair<int, string>(idGrupy, nazwa), nazwaAtrybutu,cechaAuto, _config.SeparatorAtrybutowWCechach, atrybutZCechy);
                            if (atrybut != null)
                            {
                                if (!slownikAtrybutow.ContainsKey(atrybut.Id))
                                {
                                    slownikAtrybutow.Add(atrybut.Id,atrybut);
                                }
                            }
                            if (listaCech.Any(x => x.Symbol == c.Symbol))
                            {
                                log.Error("Pomijanie cechy, już istnieje" + c.Symbol);
                                continue;
                            }
                            listaCech.Add(c);
                        }
                    }
                }
                //Pobieramy Cechy z tabeli cech
                using (SqlCommand command = new SqlCommand("select Id, Nazwa from ModelDanychContainer.CechyAsortymentu;"))
                {
                    command.Connection = conn;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int idCechy = DataHelper.dbi("Id", reader);
                            string nazwa = DataHelper.dbs("Nazwa", reader);

                            Cecha c = new Cecha(nazwa, nazwa);
                            c.Id = idCechy;
                            c.Widoczna = true;
                            string nazwaCechy = c.Nazwa;
                            string symbol =c.Nazwa;
                            Atrybut tmp = null;
                            if (atrybutZCechy)
                            {
                                tmp = AtrybutyWyszukiwanie.PobierzInstancje.WyciagnijAtrybutZCechy(ref nazwaCechy, ref symbol, _config.SeparatorAtrybutowWCechach, cechaAuto);
                            }
                            else
                            {
                                log.Info("Wyłaczone ustawienie wyciagania atrybutow z cechy");
                            }
                            if (tmp != null)
                            {
                                if (!slownikAtrybutow.ContainsKey(tmp.Id))
                                {
                                    slownikAtrybutow.Add(tmp.Id, tmp);
                                }
                                c.AtrybutId = tmp.Id;
                                c.Nazwa = nazwaCechy;
                            }
                            c.Symbol = symbol;
                            if (string.IsNullOrEmpty(c.Nazwa))
                            {
                                c.Nazwa = c.Symbol;
                            }
                            if (c.AtrybutId == null)
                                continue;
                            c.Symbol = c.Symbol.Trim().ToLower();
                            if (listaCech.Any(x => x.Symbol == c.Symbol))
                            {
                                log.Error("Pomijanie cechy, już istnieje" + c.Symbol);
                                continue;
                            }
                            listaCech.Add(c);

                        }
                    }
                }
               
                conn.Close();
            }


            atrybuty = slownikAtrybutow.Values.ToList();
            return listaCech;
        }

        public List<ZamowienieSynchronizacja> ImportZamowien(List<ZamowienieSynchronizacja> zamowienia, Dictionary<long, Klient> wszyscy)
        {
            throw new NotImplementedException();
        }

        public List<StatusZamowienia> PobierzStatusyDokumentow()
        {
            throw new NotImplementedException();
        }

        public Dictionary<long, decimal> PobierzStanyDlaMagazynu(string mag)
        {
            throw new NotImplementedException();
        }

        public string przesunMazazyn(List<ZamowienieProdukt> suBraki, string mag, string docelowy, string nr, string uwagi = "")
        {
            throw new NotImplementedException();
        }

        public bool CleanUp()
        {
            return true;
        }

        public List<CenaPoziomu> PobierzPoziomyCenoweProduktow()
        {
            //return new List<CenaPoziomu>();
            List<CenaPoziomu> items = new List<CenaPoziomu>();
            if (!CzyWyciagamyProdukty())
            {
                return items;
            }
            int visibility = (int)_config.SubiektWidocznoscTowarow;

            using (SqlConnection conn = new SqlConnection(_config.ERPcs))
            {
                conn.Open();
                //Wyciagamy nazwy atrybutow
                using (SqlCommand command = new SqlCommand("select pc.CenaNetto, pc.Cennik_Id,pc.Asortyment_Id from ModelDanychContainer.PozycjeCennika pc join ModelDanychContainer.Asortymenty a on (pc.Asortyment_Id=a.Id) " +
                                                           "where a.IsInRecycleBin=0 and 1=(case when @visible=1 then a.SklepInternetowy  " +
                "else (case when @visible=2 then a.SerwisAukcyjny else (case when @visible=3 then a.SprzedazMobilna else 1 end) end) end)"))
                {
                    command.Connection = conn;
                    command.Parameters.AddWithValue("@visible", visibility);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int idProduktu = DataHelper.dbi("Asortyment_Id", reader);
                            int idPoziomuCenowego = DataHelper.dbi("Cennik_Id", reader);
                            decimal cenaNetto = DataHelper.dbd("CenaNetto", reader);

                            items.Add(new CenaPoziomu(idPoziomuCenowego,cenaNetto,idProduktu));
                        }
                    }
                }
                conn.Close();
            }
            return items;
        }

        public List<PoziomCenowy> PobierzDostepnePoziomyCen()
        {
            List<PoziomCenowy>poziomyCen = new List<PoziomCenowy>();
            using (SqlConnection conn = new SqlConnection(_config.ERPcs))
            {
                conn.Open();
                //Wyciagamy nazwy atrybutow
                using (SqlCommand command = new SqlCommand("Select pc.Id,pc.Tytul, w.Symbol from ModelDanychContainer.Cenniki pc join ModelDanychContainer.Waluty w on pc.Waluta_Id=w.Id"))
                {
                    command.Connection = conn;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = DataHelper.dbi("Id", reader);
                            string nazwaPoziomu = DataHelper.dbs("Tytul", reader);
                            string symbolWaluty = DataHelper.dbs("Symbol", reader);

                            PoziomCenowy price = new PoziomCenowy();
                            price.Id = id;
                            price.Nazwa = nazwaPoziomu;
                            price.WalutaId = symbolWaluty.ToLower().WygenerujIDObiektuSHAWersjaLong();
                            poziomyCen.Add(price);
                        }
                    }
                }
                conn.Close();
            }
            return poziomyCen;
        }

        public List<Rabat> PobierzRabaty(Dictionary<long, Klient> dlaKogoLiczyc, Dictionary<long, Produkt> produkty, List<PoziomCenowy> ceny, List<Cecha> cechy, List<ProduktCecha> cechyProdukty, Dictionary<long, KategoriaProduktu> kategorie, List<ProduktKategoria> produktyKategorie, IDictionary<int, KategoriaKlienta> kategorieKlientow, IDictionary<long, KlientKategoriaKlienta> klienciKategorie)
        {
            throw new NotImplementedException();
        }

        public List<Kraje> PobierzKraje()
        {
            List<Kraje> listaKrajow = new List<Kraje>();

            using (SqlConnection conn = new SqlConnection(_config.ERPcs))
            {
                conn.Open();
                //Wyciagamy nazwy atrybutow
                using (SqlCommand command = new SqlCommand("select Id, Nazwa,KodPanstwaUE from ModelDanychContainer.Panstwa;"))
                {
                    command.Connection = conn;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int idPanstwa = DataHelper.dbi("Id", reader);
                            string nazwa = DataHelper.dbs("Nazwa", reader);
                            string symbol = DataHelper.dbs("KodPanstwaUE", reader);
                            listaKrajow.Add(new Kraje(idPanstwa,nazwa,symbol));

                        }
                    }
                }
                conn.Close();
            }

            return listaKrajow;

        }

        public List<Region> PobierzRegiony()
        {
            List<Region> listaRegionow = new List<Region>();

            using (SqlConnection conn = new SqlConnection(_config.ERPcs))
            {
                conn.Open();
                //Wyciagamy nazwy atrybutow
                using (SqlCommand command = new SqlCommand("select Id,Nazwa,Panstwo_Id from ModelDanychContainer.Wojewodztwa;"))
                {
                    command.Connection = conn;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int idREgionu = DataHelper.dbi("Id", reader);
                            int idPanstwa = DataHelper.dbi("Panstwo_Id", reader);
                            string nazwa = DataHelper.dbs("Nazwa", reader);

                            listaRegionow.Add(new Region(idREgionu, nazwa, idPanstwa));

                        }
                    }
                }
                conn.Close();
            }

            return listaRegionow;
            
        }

        public List<Magazyn> PobierzMagazynyErp()
        {
            throw new NotImplementedException();
        }

        public Dictionary<long, Waluta> PobierzDostepneWaluty()
        {
            Dictionary<long,Waluta> slownikWalut = new Dictionary<long, Waluta>();
            using (SqlConnection conn = new SqlConnection(_config.ERPcs))
            {
                conn.Open();
                //Wyciagamy nazwy atrybutow
                using (SqlCommand command = new SqlCommand("Select symbol from ModelDanychContainer.Waluty"))
                {
                    command.Connection = conn;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string symbol = DataHelper.dbs("symbol", reader);

                            Waluta w = new Waluta();
                            w.WalutaB2b = symbol;
                            w.WalutaErp = symbol;
                            w.Id = symbol.ToLower().WygenerujIDObiektuSHAWersjaLong();

                            if(!slownikWalut.ContainsKey(w.Id))slownikWalut.Add(w.Id,w);

                        }
                    }
                }
                conn.Close();
            }
            return slownikWalut;
        }
    }
}
