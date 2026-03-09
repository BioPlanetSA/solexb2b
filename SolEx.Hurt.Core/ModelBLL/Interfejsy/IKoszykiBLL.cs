using System;
using System.Collections.Generic;
using ServiceStack.DataAnnotations;
using SolEx.Hurt.Core.BLL.Web;
using SolEx.Hurt.Core.BLL.ZadaniaKoszyka;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Core;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core.ModelBLL.Interfejsy
{
    public interface IKoszykiBLL
    {
        string Parametry { get; set; }
        string Hash { get; }
        bool NieDodawajDostawyDoKoszyka { get; set; }
        ISposobPlatnosci PlatnoscObiekt { get; set; }
        string Platnosc { get; }
        IKlient Klient { get; }
        SzablonAkceptacjiBll AktualnySzablonAkceptacji { get; }
        Adres Adres { get; }
        bool WEdycji { get; set; }

        bool UkryjAdresy { get; set; }
        long Id { get; set; }
        string Nazwa { get; set; }
        long KlientId { get; set; }
        TypKoszyka Typ { get; set; }
        bool Aktywny { get; set; }
        DateTime DataModyfikacji { get; set; }
        WartoscLiczbowa LacznaWartoscNetto();
        WartoscLiczbowa LacznaWartoscBrutto();
        WartoscLiczbowa CenaDostawyNetto();
        WartoscLiczbowa CalkowitaWartoscCenaKatalogowaNetto();
        WartoscLiczbowa CalkowitaWartoscCenaKatalogowaBrutto();
        WartoscLiczbowa WartoscVatCenaKatalogowa();
        WartoscLiczbowa WartoscVatCenaHurtowa();
        WartoscLiczbowa CalkowitaWartoscCenaKatalogowaNettoZysk();
        WartoscLiczbowa CalkowitaWartoscCenaKatalogowaBruttoZysk();
        WartoscLiczbowa VatZyskKlienta();
        WartoscLiczbowa CalkowitaWartoscHurtowaBrutto();
        WartoscLiczbowa CalkowitaWartoscHurtowaNetto();
        WartoscLiczbowa CalkowitaWartoscHurtowaNettoPoRabacie();
        WartoscLiczbowa CalkowitaWartoscHurtowaBruttoPoRabacie();
        WartoscLiczbowa VatRabat();
        decimal CalkowitaObjetoscKoszyka();
        WartoscLiczbowa CalkowityRabatNetto();
        WartoscLiczbowa CalkowityRabatBrutto();
        WartoscLiczbowa CalkowityRabatVat();

        /// <summary>
        /// Wylicza ile ca³kowicie procent rabatu ma klient
        /// </summary>
        /// 
        decimal CalkowityRabatProcent();

        WartoscLiczbowa CenaDostawyBrutto();
        ISposobDostawy KosztDostawy();

        /// <summary>
        /// Sumuje
        /// </summary>
        /// <returns></returns>
        decimal PobierzWartoscNetto();

        decimal PobierzWartoscBrutto();
        WartoscLiczbowa WagaCalokowita();

        /// <summary>
        /// Ca³kowita wartoœæ hurtowa vat (po rabacie)
        /// </summary>
        WartoscLiczbowa WartoscVat();

        Waluta WalutaKoszyka();
        void DodajaAutomatyczny(int produkt, decimal ilosc, decimal cena, int dodajacezadanie);

        [Ignore]
        List<KoszykPozycje> PobierzPozycje { get; }
        void UstawStatus(IKlient aktualnyKlient, StatusKoszyka statusKoszyka);
        bool CzyKoszykDoAkceptacji(IKlient aktualnyKlient, out SzablonAkceptacjiPoziomy poziom);
        void DodajPozycjeDoKoszyka(KoszykPozycje pozycja);
        Dictionary<long, decimal> PobierzIlosciProduktowWKoszyku();

        [Ignore]
        IKlient PrzedstawicielKtoryEdytujeKoszykWImieniuKlienta { get; set; }
        string Uwagi { get; set; }
        string MagazynDlaMm { get; set; }
        string MagazynRealizujacy { get; set; }
        int? PlatnoscId { get; set; }
        long? AdresId { get; set; }
        [Ignore]
        string DefinicjaDokumentuERP { get; set; }
        int? KosztDostawyId { get; set; }
        List<long> KlienciMogacyAkceptowacKoszyk { get; set; }
        [Ignore]
        string NumerZamowienia { get; }
        [Ignore]
        string KategoriaZamowienia { get; set; }
        DodatkowePoleKoszyka PobierzDodatkowyParemetr(int idModulu);

        void DodajDodatkowyParametr(int idModulu, string symbol, string[] wartosc);
        Dictionary<int, DodatkowePoleKoszyka> DodatkoweParametry { get; set; }
        string DodatkowePolaErp { get; set; }

        [Ignore]
        DateTime? TerminDostawy { get; }
        List<StatusKoszykaHistoria> HistoriaZmianStatusow { get; set; }
    }
}