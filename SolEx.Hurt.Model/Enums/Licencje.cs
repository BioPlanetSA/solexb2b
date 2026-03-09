using SolEx.Hurt.Model.AtrybutyKlas;

namespace SolEx.Hurt.Model.Enums
{
    public enum Licencje
    {
        [FriendlyName("Kamstof AOW")]
        DokumentyKamsoftAow = -51,
        [FriendlyName("Kamsoft SOW")]
        DokumentyKamsoft = -50,

        DokumentyOptimaTXT = -40,

        DokumentySmallBussinesExp = -30,

        ZmianaCenPrzedstawiciel = -24,
        TerminRealizacji = -23,
        DaneDoPrzelewu = -22,

        subkonta = -17,

        OptimaEPP = -6,
        produkty_uch = -5,
        DokumentyCSVSymbol = -4,
        DokumentyEPP = -3,
        DokumentyCSV = -2,
        DokumentyXML = -1,
        
        Brak = 0,
        //Partnerzy = 1,
        moj_katalog = 2,
        katalog_klienta = 3,
        DokumentyPDF = 4,
        DokumentyKucharscy = 5,
        DokumentyPCMarket5 = 6,
        limity_ilosciowe = 7,
        generowanie_katalogu = 8,
        stale_ceny = 9,
        formularze = 10,
        reklamacje = 11,
        przedstawiciele = 12,
        produkty_do_katalogu = 13,
        dokumenty_raporty = 14,
        koszyk_przechowalnie = 15,
        produkty_do_przechowalni_zbiorczo = 16,

        sklepy = 18,
        Partnerzy = 19,
        przedstawiciele_wyslij_faktury = 20,
        ZmianaIp = 21,

        DokumentUsuwanie = 23,
        Afiliacyjna = 24,
        UlubioneZbiorczo = 25,
        Punkty = 26,

        Wielokoszykowosc = 28,

    }
}
