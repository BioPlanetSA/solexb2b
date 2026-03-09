using System;
using System.Collections.Generic;
using ServiceStack.DataAnnotations;
using SolEx.Hurt.Core.BLL.Web;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Core;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;
using SolEx.Hurt.Model.Interfaces.Modele;
using SolEx.Hurt.Model.Web;

namespace SolEx.Hurt.Core.ModelBLL.Interfejsy
{
    
   
    public interface IKoszykPozycja
    {
        [PrimaryKey]
        [UpdateColumnKey]
        [AutoIncrement]
        [FriendlyName("ID pozycji")]
        long Id { get; set; }

        long KoszykId { get; set; }
        [FriendlyName("Numer id produkt")]
        long ProduktId { get; set; }

        [FriendlyName("Iloœæ w koszyku")]
        decimal Ilosc { get; set; }

        [FriendlyName("Wybrana jednostka(liczba)")]
        long? JednostkaId { get; set; }

        decimal RabatDodatkowy { get; set; }

        string PowodDodatkowegoRabatu { get; set; }

        //string PrzedstawicielNazwa { get; set; }
        long? PrzedstawicielId { get; set; }

        decimal? WymuszonaCenaNettoPrzedstawiciel { get; set; }

        decimal? WymuszonaCenaNettoModul { get; set; }
        [FriendlyName("Data dodania")]
        DateTime DataDodania { get; set; }

        DateTime? DataZmiany { get; set; }

        IndywidualizacjaWartosc[] Indywidualizacja { get; set; }

        int? DodajaceZadanie { get; set; }

        TypPozycjiKoszyka TypPozycji { get; set; }

        int? Hash { get; set; }
        string Opis { get; set; }
        long ProduktBazowyId { get; set; }
        decimal DoklanaCenaNetto();
        decimal DoklanaCenaBruto();
        StanKoszyk StanKoszyk { get; set; }

        [Ignore]
        bool ZmianianaIlosc { get; set; }

        bool KupowanyWJednostcePodstawowej();
        JednostkaProduktu Jednostka();

        [FriendlyName("Opis dodatkowego rabatu")]
        [Ignore]
        string OpisRabatu { get; }
        [Ignore]
        IProduktKlienta Produkt { get; }
        decimal IloscWJednostcePodstawowej { get; set; }
        [FriendlyName("Wartoœæ netto")]
        WartoscLiczbowa WartoscNetto { get; }

        [FriendlyName("Wartoœæ brutto")]
        WartoscLiczbowa WartoscBrutto { get; }

        [FriendlyName("Wartoœæ vat")]
        WartoscLiczbowa WartoscVat { get; }

        [FriendlyName("Wartoœæ Detaliczna netto")]
        WartoscLiczbowa WartoscDetalicznaNetto();

        [FriendlyName("Wartoœæ Detaliczna brutto")]
        WartoscLiczbowa WartoscDetalicznaBrutto();

        [FriendlyName("Wartoœæ Detaliczna vat")]
        WartoscLiczbowa WartoscDetalicznaVat { get; }

        [FriendlyName("Waga")]
        decimal? WagaPozycji();

        [FriendlyName("Cena brutto")]
        WartoscLiczbowa CenaBrutto();

        [FriendlyName("Cena netto")]
        WartoscLiczbowa CenaNetto { get; }


        [FriendlyName("Cena netto w jednostce podstawowej")]
        decimal CenaNettoPodstawowa { get; }

        [FriendlyName("Cena brutto w jednostce podstawowej")]
        decimal CenaBruttoPodstawowa();

        [FriendlyName("Cena w punktach")]
        decimal CenaWPunktach();

        [FriendlyName("Ca³kowity rabat")]
        WartoscLiczbowa CalkowityRabat();

        void ZmienDodatkowyRabat(decimal wartosc, string opis, TrybLiczeniaRabatuWKoszyku tryb, string dodatkoweInfoDymek = null);

        string KolorTla { get; set; }
        [Ignore]
        int KolejnoscSortowania { get; set; }
        [Ignore]
        string PieczatkaGrafika { get; set; }

        [Ignore]
        string PowodDodatkowegoRabatu_DodatkoweInfoDymek { get; set; }

        ParametryIlosciProduktu DodawanieProduktu { get; set; }
        IKlient Klient { get; set; }
        string Waluta();
        decimal CenaNettoPodstawowa_BezCenyPrzedstawiciela();
        string OpisIndywidualizacji();
    }
}