namespace SolEx.Hurt.Model.Enums
{
    public enum ZdarzenieGlowne
    {
        //----------- KLIENCI -----------
        [ZdarzenieGrupa(ZdarzenieGrupa.Klienci)] PowitanieOdSzefa,
        [ZdarzenieGrupa(ZdarzenieGrupa.Klienci)] LogowanieDoSystemu_Udane,
        [ZdarzenieGrupa(ZdarzenieGrupa.Klienci)] LogowanieDoSystemu_Nieudane,
        [ZdarzenieGrupa(ZdarzenieGrupa.Klienci)] DodanieNowegoKlienta,
        [ZdarzenieGrupa(ZdarzenieGrupa.Klienci)] NowaRejestracja,
        [ZdarzenieGrupa(ZdarzenieGrupa.Klienci)] ResetHasla,
        [ZdarzenieGrupa(ZdarzenieGrupa.Klienci)] WyslanieFormularza,
        [ZdarzenieGrupa(ZdarzenieGrupa.Klienci)] ZapisanieDoNewslettera,
        [ZdarzenieGrupa(ZdarzenieGrupa.Klienci)] ZmianaAdresuIP,
        [ZdarzenieGrupa(ZdarzenieGrupa.Klienci)] WysylanieNewslettera,
        [ZdarzenieGrupa(ZdarzenieGrupa.Klienci)] WyswietlenieKomunikatu,


        //------- PRODUKTY ---------------
        [ZdarzenieGrupa(ZdarzenieGrupa.Produkty)] ZapytanieOStan,
        [ZdarzenieGrupa(ZdarzenieGrupa.Produkty)] ProsbaOInformacjeODostepnosci,
        [ZdarzenieGrupa(ZdarzenieGrupa.Produkty)] PojawienieSieProduktowNaMagazynieInformacjaODostepnosci,
        [ZdarzenieGrupa(ZdarzenieGrupa.Produkty)] NoweProduktyWSystemie,
        [ZdarzenieGrupa(ZdarzenieGrupa.Produkty)] ProduktyPrzyjeteNaMagazyn,
        [ZdarzenieGrupa(ZdarzenieGrupa.Produkty)] WyszukiwanieProduktu,


        //------- API  ---------
        [ZdarzenieGrupa(ZdarzenieGrupa.API)] GenerowanieNowegoKluczaApi = 100,
        [ZdarzenieGrupa(ZdarzenieGrupa.API)] PobieranieDanych = 101,


        //---------- DOKUMENTY ---------
        [ZdarzenieGrupa(ZdarzenieGrupa.Dokumenty)] PrzypomnienieNiezaplaconejFakturze = 500,
        [ZdarzenieGrupa(ZdarzenieGrupa.Dokumenty)] PrzypomnieniePrzeterminowaneFaktury = 501,
        [ZdarzenieGrupa(ZdarzenieGrupa.Dokumenty)] PobieranieDokumentu,
        [ZdarzenieGrupa(ZdarzenieGrupa.Dokumenty)] NoweDokumentyDlaKlienta,
        [ZdarzenieGrupa(ZdarzenieGrupa.Dokumenty)] NoweListyPrzywozowe,
        [ZdarzenieGrupa(ZdarzenieGrupa.Dokumenty)] ZmianaTerminuRealizacji,
        [ZdarzenieGrupa(ZdarzenieGrupa.Dokumenty)] ZmianaStatusuDokumentu,
        [ZdarzenieGrupa(ZdarzenieGrupa.Dokumenty)] UsuniecieZbednychPdf,

        //------------- ZAMÓWIENIE --------------

        [ZdarzenieGrupa(ZdarzenieGrupa.Zamówienia)] NoweZamowienie_Finalizacja,
        [ZdarzenieGrupa(ZdarzenieGrupa.Zamówienia)] NoweZamowienie_PoImporcieERP,
        [ZdarzenieGrupa(ZdarzenieGrupa.Zamówienia)] BladImportu,

        //----------------- SUBKONTA -------------

        [ZdarzenieGrupa(ZdarzenieGrupa.Subkonta)] ZamówienieOdrzucone, 
        [ZdarzenieGrupa(ZdarzenieGrupa.Subkonta)] ZamówienieZaakceptowane,
        [ZdarzenieGrupa(ZdarzenieGrupa.Subkonta)] ZamówienieWysłaneDoAkceptacji,
        [ZdarzenieGrupa(ZdarzenieGrupa.Subkonta)] NowyPracownik,
        [ZdarzenieGrupa(ZdarzenieGrupa.Subkonta)] NowyAdres,

        //----------------- KOSZYK -------------

        [ZdarzenieGrupa(ZdarzenieGrupa.Koszyk)] ImportPliku = 700,

    }

    public enum ZdarzenieGrupa
    {
        Klienci = 1,
        Dokumenty = 2,
        Produkty = 3,
        API = 4,
        Zamówienia = 5,
        Subkonta = 6,
        Koszyk =7
    }
}
