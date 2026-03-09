using System;
using System.Collections.Generic;
using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Model.Interfaces
{
    public interface IConfigSynchro
    {
        System.Collections.Specialized.NameValueCollection DoKolekcjiWartosciNazw();
        HashSet<string> DomyslneUprawnieniaPracownik { get; }
        HashSet<string> DomyslneUprawnieniaPrzedstawiciel { get; }
        string EmailFromPrzyjaznaNazwa { get; }
       
        string KatalogZObrazkami { get; }
        int? SubiektKodTransakcjiDlaKlientowEU { get; }
        int DokumentyMailOnNowymIleDniWstecz { get; }
        bool WieleJezykowWSystemie { get; }
        Dictionary<int, Jezyk> JezykiWSystemie { get; set; }
        Dictionary<string, Jezyk> JezykiWSystemieSlownikPoSymbolu { get; }
        int JezykIDDomyslny { get; }
        string SubiektPodmiot { get; }
        bool SubiektSzyfrujHaslo { get; }
        int[] AtrybutyRodzin { get; }
        int CoIleDniZmieniacHaslo { get; }
        bool AtrybutZCechy { get; }
        string SzablonNiestandardowyNazwa { get; }
        string SzablonNiestandardowySciezkaWzgledna { get; }
        Dictionary<int, StatusZamowienia> StatusyZamowien { get; }
        int JezykIDPolski { get; }
        string[] PolaWlasneCechy { get; }
        string AtrybutKategoriiZERP { get; }
        string AtrybutProducentaZERP { get; }
        List<string> PolaDoWyzerowania { get; }
        char[] SeparatorGrupKlientow { get; }
        HashSet<ZCzegoLiczycGradacje> ZCzegoLiczycGradacje { get; }
      //  DateTime GradacjeOdKiedyLiczyc { get; }
        bool GradacjeUwzgledniajRodziny { get; }
        int OptimaIdSzablonuWydrukuDoPdf { get; }
        int? ProduktyNaZamowienieCechaID { get; }
        decimal ProduktyNaWyczerpaniu_procentStanuMinimalnego { get; }
        decimal ProduktyNieDostepnePrzezDluzszyCzas_iloscDni { get; }
        bool PodczasWyszukiwaniaZmienPolskeZnaki { get; }
        bool ZamowieniaTworzRezerwacje { get; }
        string EnovaPoleDoPobieraniaZDokumentow { get; }
        string EnovaPoleDoZapisywaniaUwagNaPozycjiDokumentu { get; }
        string EnovaPole2DoZapisywaniaUwagNaPozycjiDokumentu { get; }
        bool SortowanieNaturalneListyProduktow { get; }
        bool SprawdzajStanyMagazynowe { get; }
        int ZIleDniDomyslniePokazywacDokumenty { get; }
        bool WielowybieralnoscKategorii { get; }
        string OptimaAtrybutZeZdjeciami { get; }
        string ERPcs { get; }

        string ERPcs2 { get; }
        string ERPHaslo { get; }
        string ERPLogin { get; }
        bool PokazujCenyDlaNiezalogowanych { get; }
        bool WysylajPowiadomienieFakturaGdyBrakPdf { get; }
        string EnovaDoZapisuKategoriiZamowienia { get; }
        string EnovaCechaDoPobraniaJakoStatusDokumentu { get; }
        string KatalogProgramuKsiegowego { get; }
        string EnovaPoleDoPobieraniaZDokumentow2 { get; }
        int? GradacjeUzgledniaProduktyZCecha { get; }
        int? ProduktyDropshipingCechaID { get; }
        bool BlokujDodawanieDoKoszykaDlaBrakujacychProduktow { get; }

        /// <summary>
        /// Sprawdza czy dana licencja jest w³¹czona
        /// </summary>
        /// <param name="key">Nazwa licencj</param>
        /// <returns></returns>
        bool GetLicense(Licencje key);

        Dictionary<long, string> Tlumaczenia(int? lang = null);

        //SystemSettings ObiektUstawienSystemu(int? langID);
       // Dictionary<string, string> PobierzTlumaczenie(int lang, string[] symbol);
        string PobierzTlumaczenie(int lang, string symbol, params object[] parametryFormatuFrazy);
        string PobierzTlumaczenie(int lang, string symbol, string symnbolDoHash, out long symbolHash,MiejsceFrazy miejsce = MiejsceFrazy.Brak, params object[] parametryFormatuFrazy);
        //int GetSystemTypeId(Type type);
        int GetPriceLevelHurt{ get; }
        
        int? GetPriceLevelDetal { get; }
     
        string PobierzSerieDlaWaluty(string waluta);


        bool ProduktyDropshipingPokazujNaStanieJesliJest { get; }


        void PrzeladujResetujStatusy();

        /// <summary>
        /// grupa automatyczna dla kategorii klientów 
        /// </summary>
        string CechaAuto { get; }

     
        

        bool EksportTylkoKontZHaslem { get; }

        string EnovaPoleDoZapisuPlatnikaDokumentu { get; }

        bool EnovaZamowieniaBufor { get; }

        int MaksimumDokumentowWPaczce { get; }

        string PobierzSzablonWydrukuEnova(string symboljezykawydruku);


        char[] SeparatorAtrybutowWCechach { get; }

        string PrzedzialyCenowe { get; }


        IEnumerable<string> DokumentyWyszukiwanie { get; }
        List<string> ProduktyWyszukiwanie { get; }

        IEnumerable<string> KoszykPozycjeWyszukiwanie { get; }

        int CzasPrzechowywaniaZmian { get; }

        int SferaMaksPobranNaOkres { get; }

        int SferaPobieranieLimitOkres { get; }

        //bool MinimumLogistyczneWymagane { get;}

        HashSet<int> DeaktywujMinimumLogistyczneDlaWybranychKategoriiKlientow { get; }
       // bool MinimumLogistyczneWymagane { get;}

        WidcznoscProduktowWSubiekcie SubiektWidocznoscTowarow { get; }

        WidcznoscProduktowWXl XlWidocznoscTowarow { get; }
        void SynchronizacjaPobierzLokalizacjeNazwa(Produkt item, Jezyk jezyk, string domyslnePole, Dictionary<string, string> pars, ref List<Tlumaczenie> tlumaczenia);
        void SynchronizacjaPobierzLokalizacjeKod(Produkt item, Jezyk jezyk, string domyslnePole, Dictionary<string, string> pars, ref List<Tlumaczenie> tlumaczenia);
        void SynchronizacjaPobierzLokalizacjeOpis(Produkt item, Jezyk jezyk, string domyslnePole, Dictionary<string, string> pars, ref List<Tlumaczenie> tlumaczenia);
        void SynchronizacjaPobierzLokalizacjeOpis2(Produkt item, Jezyk j, string domyslnePole, Dictionary<string, string> pars, ref List<Tlumaczenie> tlumaczenia);
        void SynchronizacjaPobierzLokalizacjeOpis3(Produkt item, Jezyk j, string domyslnePole, Dictionary<string, string> pars, ref List<Tlumaczenie> tlumaczenia);
        void SynchronizacjaPobierzLokalizacjeOpis4(Produkt item, Jezyk j, string domyslnePole, Dictionary<string, string> pars, ref List<Tlumaczenie> tlumaczenia);   
        void SynchronizacjaPobierzLokalizacjeOpis5(Produkt item, Jezyk j, string domyslnePole, Dictionary<string, string> pars, ref List<Tlumaczenie> tlumaczenia);
        void SynchronizacjaPobierzLokalizacjeOpisKrotki(Produkt item, Jezyk j, string domyslnePole, Dictionary<string, string> pars, ref List<Tlumaczenie> tlumaczenia);
        void SynchronizacjaPobierzLokalizacjeOpisKrotki2(Produkt item, Jezyk j, string domyslnePole, Dictionary<string, string> pars, ref List<Tlumaczenie> tlumaczenia);
        void SynchronizacjaPobierzLokalizacjeOpisKrotki3(Produkt item, Jezyk j, string domyslnePole, Dictionary<string, string> pars, ref List<Tlumaczenie> tlumaczenia);
        void SynchronizacjaPobierzLokalizacjeOpisKrotki4(Produkt item, Jezyk j, string domyslnePole, Dictionary<string, string> pars, ref List<Tlumaczenie> tlumaczenia);
        void SynchronizacjaPobierzLokalizacjeOpisKrotki5(Produkt item, Jezyk j, string domyslnePole, Dictionary<string, string> pars, ref List<Tlumaczenie> tlumaczenia);
        void SynchronizacjaPobierzLokalizacjePoleTekst1(Produkt item, Jezyk j, string domyslnePole, Dictionary<string, string> pars, ref List<Tlumaczenie> tlumaczenia);  
        void SynchronizacjaPobierzLokalizacjePoleTekst2(Produkt item, Jezyk j, string domyslnePole, Dictionary<string, string> pars, ref List<Tlumaczenie> tlumaczenia);
        void SynchronizacjaPobierzLokalizacjePoleTekst3(Produkt item, Jezyk j, string domyslnePole, Dictionary<string, string> pars, ref List<Tlumaczenie> tlumaczenia);
        void SynchronizacjaPobierzLokalizacjePoleTekst4(Produkt item, Jezyk j, string domyslnePole, Dictionary<string, string> pars, ref List<Tlumaczenie> tlumaczenia);
        void SynchronizacjaPobierzLokalizacjePoleTekst5(Produkt item, Jezyk j, string domyslnePole, Dictionary<string, string> pars, ref List<Tlumaczenie> tlumaczenia);
        void SynchronizacjaPobierzLokalizacjeRodzina(Produkt item, Jezyk j, string domyslnePole, Dictionary<string, string> pars, ref List<Tlumaczenie> tlumaczenia);
        void SynchronizacjaPobierzLokalizacjeKolumnaTekst1(Produkt item, Jezyk j, string domyslnePole, Dictionary<string, string> pars, ref List<Tlumaczenie> tlumaczenia);
        void SynchronizacjaPobierzLokalizacjeKolumnaTekst2(Produkt item, Jezyk j, string domyslnePole, Dictionary<string, string> pars, ref List<Tlumaczenie> tlumaczenia);
        void SynchronizacjaPobierzLokalizacjeKolumnaTekst3(Produkt item, Jezyk j, string domyslnePole, Dictionary<string, string> pars, ref List<Tlumaczenie> tlumaczenia);
        void SynchronizacjaPobierzLokalizacjeKolumnaTekst4(Produkt item, Jezyk j, string domyslnePole, Dictionary<string, string> pars, ref List<Tlumaczenie> tlumaczenia);
        void SynchronizacjaPobierzLokalizacjeKolumnaTekst5(Produkt item, Jezyk j, string domyslnePole, Dictionary<string, string> pars, ref List<Tlumaczenie> tlumaczenia);
        void SynchronizacjaPobierzLokalizacjeMetaOpis(Produkt item, Jezyk j, string domyslnePole, Dictionary<string, string> pars, ref List<Tlumaczenie> tlumaczenia);
        void SynchronizacjaPobierzLokalizacjeMetaSlowaKluczowe(Produkt item, Jezyk j, string domyslnePole, Dictionary<string, string> pars, ref List<Tlumaczenie> tlumaczenia);

        void SynchronizacjaPobierzCenaWPunktach(Produkt item, string domyslnePole, Dictionary<string, string> pars);


        void SynchronizacjaPobierzGlebokoscOpakowaniaJednostkowego(Produkt item, string domyslnePole,Dictionary<string, string> pars);
        void SynchronizacjaPobierzSzerokoscOpakowaniaJednostkowego(Produkt item, string domyslnePole,Dictionary<string, string> pars);
        void SynchronizacjaPobierzWysokoscOpakowaniaJednostkowego(Produkt item, string domyslnePole,Dictionary<string, string> pars);
        //void SynchronizacjaPobierzWageOpakowaniaJednostkowego(Produkt item, string domyslnePole, Dictionary<string, string> pars);
        //void SynchronizacjaPobierzObjetoscOpakowaniaJednostkowego(Produkt item, string domyslnePole, Dictionary<string, string> pars);

        void SynchronizacjaPobierzGlebokoscOpakowaniaZbiorczego(Produkt item, string domyslnePole,Dictionary<string, string> pars);
        void SynchronizacjaPobierzSzerokoscOpakowaniaZbiorczego(Produkt item, string domyslnePole,Dictionary<string, string> pars);
        void SynchronizacjaPobierzWysokoscOpakowaniaZbiorczego(Produkt item, string domyslnePole,Dictionary<string, string> pars);
        void SynchronizacjaPobierzWageOpakowaniaZbiorczego(Produkt item, string domyslnePole, Dictionary<string, string> pars);
        void SynchronizacjaPobierzObjetoscOpakowaniaZbiorczego(Produkt item, string domyslnePole, Dictionary<string, string> pars);
        void SynchronizacjaPobierzIloscWOpakowaniuZbiorczym(Produkt item, string domyslnePole, Dictionary<string, string> pars);

        void SynchronizacjaPobierzGlebokoscPalety(Produkt item, string domyslnePole, Dictionary<string, string> pars);
        void SynchronizacjaPobierzSzerokoscPalety(Produkt item, string domyslnePole, Dictionary<string, string> pars);
        void SynchronizacjaPobierzWysokoscPalety(Produkt item, string domyslnePole, Dictionary<string, string> pars);
        void SynchronizacjaPobierzWagePalety(Produkt item, string domyslnePole, Dictionary<string, string> pars);
        void SynchronizacjaPobierzObjetoscPalety(Produkt item, string domyslnePole, Dictionary<string, string> pars);
        void SynchronizacjaPobierzLiczbaSztukNaWarstwie(Produkt item, string domyslnePole, Dictionary<string, string> pars);
        void SynchronizacjaPobierzLiczbaSztukNaPalecie(Produkt item, string domyslnePole, Dictionary<string, string> pars);



        void SynchronizacjaPobierzPoleLiczba1(Produkt item,  string domyslnePole, Dictionary<string, string> pars);
        void SynchronizacjaPobierzWidocznoscProduktuZPola(Produkt item, string domyslnePole, Dictionary<string, string> pars);
        void SynchronizacjaPobierzObjetoscProduktu(Produkt item, string domyslnePole, Dictionary<string, string> pars);
        void SynchronizacjaPobierzWageProduktu(Produkt item, string domyslnePole, Dictionary<string, string> pars);
        void SynchronizacjaPobierzPoleLiczba2(Produkt item, string domyslnePole, Dictionary<string, string> pars);
        void SynchronizacjaPobierzPoleLiczba3(Produkt item, string domyslnePole, Dictionary<string, string> pars);
        void SynchronizacjaPobierzPoleLiczba4(Produkt item, string domyslnePole, Dictionary<string, string> pars);
        void SynchronizacjaPobierzPoleLiczba5(Produkt item, string domyslnePole, Dictionary<string, string> pars);
        void SynchronizacjaPobierzKolumnaLiczba1(Produkt item, string domyslnePole, Dictionary<string, string> pars);
        void SynchronizacjaPobierzKolumnaLiczba2(Produkt item, string domyslnePole, Dictionary<string, string> pars);
        void SynchronizacjaPobierzKolumnaLiczba3(Produkt item, string domyslnePole, Dictionary<string, string> pars);
        void SynchronizacjaPobierzKolumnaLiczba4(Produkt item, string domyslnePole, Dictionary<string, string> pars);
        void SynchronizacjaPobierzKolumnaLiczba5(Produkt item, string domyslnePole, Dictionary<string, string> pars);
        void SynchronizacjaPobierzIloscMinimlna(Produkt item, string domyslnePole, Dictionary<string, string> pars);
        void SynchronizacjaPobierzIloscWOpakowaniu(Produkt item, string domyslnePole, Dictionary<string, string> pars);

        int? IdAtrybutuDostawy { get; }

        bool ZamowienieWImieniuKlientaWysylajMaile { get; }

        bool BrakPlatnosciKlientaJesliTerminJestZerowy { get; }
        int? IleWczesniejZmianaDostawa { get;}

        string KatalogDoZapisuZalacznikowZFormularzy { get; }

        int CzasWyswietlaniaKoszyka { get; }


        string BazowaData { get;}
        string SeparatorMail { get; }

        string OpakowanieSql { get; }

        string IloscMinSql { get; }


        bool LiczonyOdCenyNetto { get;}

        string XlMagazynuZapisZamowien { get;}


        int XlPoziomKursu { get; }

        bool UzwagledniaRezerwacjeStanow { get; }

        bool ImieNazwiskoKlienta { get; }

        string WfmagPoleEmailPracownika { get;}

        string WfmagPoleHasloPracownika { get; }

        //string B2BWaluta { get;}

        int SubiektStatusDokumentu { get; }



        string OptimaNazwaFirmy { get; }



        string B2bFaktoring { get; }


        int OptimaRodzajDokumentu { get;}

        int OptimaTypDokumentu { get; }


        string OptimaSeriaDokumentu { get; }


        string SymbolMagazynow { get; }

        string OptimaNazwaDokumentu { get; }

        string NazwaFirmy { get; }

        string B2bUkryty { get; }


        string ProduktStanMinimalny { get;}


        string WfmagTypProduktow { get; }



        string AtrybutyPrefiks { get; }


        bool WfmagCzyPobieracKorzenDrzewaKategorii { get; }


        string WfmagSymbolArtykulu { get; }


        string WfmagStanProduktu { get; }


        string WfmagNazwaFirmy { get; }


        string WfmagFormatNumeracji { get; }


        string WfmagMagazynZamowienia { get; }


     //   bool StanRezerwacji { get; }


       // bool Rezerwacja { get; }

        string WfmagKtoreCechyKategorie { get; }
        string[] SeparatoryDrzewkaKategorii { get; }

        string EmailNazwaUzytkownika { get;  }

        string MailingEmailFrom { get; }

        string MailingEmailNazwaUzytkownika { get;}

        string EmailHost { get; }

        string EmailHaslo { get;}

        string MailingEmailHost { get; }

        string MailingEmailHaslo { get; }

        bool MaileTylkoSolex { get;}
        //IEnumerable<string> KlienciWyszukiwanie { get; }

  

        bool InfoPrzekroczoneStany { get; }
        bool DadawanieAtrybutuDoKategorii { get; }


        string DomyslneZdjecieSciezka { get; }


        void SynchronizacjaPobierzPoleTekst1(Klient item, string p, Dictionary<string, string> pars);

        void SynchronizacjaPobierzPoleMagazynDomyslny(Klient item, string p, Dictionary<string, string> pars);

        void SynchronizacjaPobierzPoleTekst5(Klient item, string p, Dictionary<string, string> pars);

        void SynchronizacjaPobierzPoleTekst4(Klient item, string p, Dictionary<string, string> pars);

        void SynchronizacjaPobierzPoleTekst3(Klient item, string p, Dictionary<string, string> pars);

        void SynchronizacjaPobierzPoleTekst2(Klient item, string p, Dictionary<string, string> pars);

        void SynchronizacjaPobierzPoleIndywidualnaStawaVat(Klient item, string p, Dictionary<string, string> pars,bool kontrahentue,bool krajue);

       void SynchronizacjaPobierzPoleDomyslnaWaluta(Klient item, string empty, string waluta, Dictionary<string, string> pars);

        void SynchronizacjaPobierzPoleBlokadaZamowien(Klient item, string p, Dictionary<string, string> pars);

        void SynchronizacjaPobierzMinimalnaWartoscZamowienia(Klient item, string p, Dictionary<string, string> pars);

        void SynchronizacjaPobierzPoleOpiekun(Klient item, string p, Dictionary<string, string> pars,string domyslny=null);

        void SynchronizacjaPobierzPoleDrugiOpiekun(Klient item, string p, Dictionary<string, string> pars);

        void SynchronizacjaPobierzPolePrzedstawiciel(Klient item, string p, Dictionary<string, string> pars);

        void SynchronizacjaPobierzPoleEmail(Klient item, string email);
        string PoleHasloPobierz(string domyslne, Dictionary<string, string> pars);
        void SynchronizacjaPobierzPoleHasloZrodlowe(Klient item,string empty, Dictionary<string, string> pars);
        void SynchronizacjaPobierzPoleJezyk(Klient item, string p, Dictionary<string, string> pars);

        void SynchronizacjaPobierzKredytWykorzystano(Klient item, string p, Dictionary<string, string> pars);
        void SynchronizacjaPobierzDostepneMagazyny(Klient item, string p, Dictionary<string, string> pars);
        void SynchronizacjaPobierzKredytLimit(Klient item, string p, Dictionary<string, string> pars);
        void SynchronizacjaPobierzKredytPozostalo(Klient item, string p, Dictionary<string, string> pars);
        
        void SynchronizacjaUstawPoziomCeny(Klient item, int? nullable);

        void SynchronizacjaPobierzPoleSkype(Klient item, string domyslne, Dictionary<string, string> pars);

        void SynchronizacjaPobierzPoleGaduGadu(Klient item, string domyslne, Dictionary<string, string> pars);

        void SynchronizacjaPobierzPoleKlientNadrzedny(Klient item, string p, Dictionary<string, string> pars);

        void SynchronizacjaPobierzPoleDostawa(Produkt item, string p, Dictionary<string, string> pars, DateTime? data_dostawy);
        void SynchronizacjaPobierzPoleOjciec(Produkt item, string domyslne, Dictionary<string, string> pars);
        HashSet<ModulyOptima> JakieModulyOptima { get; }

        string SapAdresSerweraLicencji { get;  }

        string SapSciezkaKataloguPdf { get; }

        string KatalogPlikowWymianySymplex { get; }
        string NazwaPlikuZKontrahentamiWymianySymplex { get; }
        string NazwaPlikuZDokumentamiWymianySymplex { get; }
        string NazwaPlikuZProduktamiWymianySymplex { get; }
        string NazwaPlikuZKategoriamiWymianySymplex { get; }
        string KatalogPlikowWymianyTema { get; }
        string KatalogPlikowZDokumentamiTema { get; }
        string NazwaPlikuZPrzedstawicielamiTema { get; }
        string NazwaPlikuZProducentamiTema { get; }
        string NazwaPlikuZKontrahentamiTema { get; }
        string NazwaPlikuZDokumentamiTema { get; }
        string NazwaPlikuZProduktamiTema { get; }
        string NazwaPlikuZCenamiTema { get; }
        string NazwaPlikuZRabatamiTema { get; }
        string NazwaPlikuZeStanamiTema { get; }
        string SeparatorDoPlikuCsvTema { get; }

        string ApiAdresVendo { get; }
        string ApiKontoVendo { get; }
        string ApiHasloVendo { get; }
        string KatalogPlikowWymianyZamowienSymplex { get; }
        ERPProviderzy ProviderERP { get; }
        //IList<TypWSystemie> SystemTypes { get; }


        string TypDomyslnyFiltru { get;  }

        string EnovaTymczasowaSciezkaPDF { get;  }

        WidcznoscProduktowWOptimie OptimaKtoreTowaryEksportowac { get;}

        string BazaXl { get; }


        int XlIdFirmy { get;  }

        int XlTypDokumentuZamowienia { get;  }

        DateTime DokumentyOdKiedyPobierane { get; }

        int MaksimumWydrukowPDF { get;  }

        DateTime OdKiedyDrukowacPdf { get;  }

        int CzasDoZamknieciaSynchronizacji { get;}
        string SymplexSciezkaKataloguPdf { get; }
        string EmailCustomerError { get; }

        TrybPokazywaniaFiltrow TrybPokazywaniaFiltrow { get;}
        HashSet<string> SlowaWymaganeWDokumencie { get;}
        HashSet<string> SlowaZakazaneWDokumencie { get; }
        string SqlDoWyciaganiWzorcaWydrukuSubiektBioPlanet { get; }
    }
}