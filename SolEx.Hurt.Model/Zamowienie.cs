using System.Collections.Generic;
using System.Linq;
using ServiceStack.DataAnnotations;
using ServiceStack.DesignPatterns.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using System;
using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Model
{
    //ZAKAZ bezposrenido korzystania z tego obiektu - trzeba korzystać z ZamowieniaBLL !!
    public class Zamowienie : DokumentBazowy
    {
        public Zamowienie()
        {
        }

        public Zamowienie(Zamowienie baza)
        {
            if (baza == null)
            {
                return;
            }
            Id = baza.Id;
            KlientId = baza.KlientId;
            DataUtworzenia = baza.DataUtworzenia;
            StatusId = baza.StatusId;
            Uwagi = baza.Uwagi;
            PoziomCenyId = baza.PoziomCenyId;
            MagazynRealizujacy = baza.MagazynRealizujacy;
            AdresId = baza.AdresId;
            WalutaId = baza.WalutaId;
            TerminDostawy = baza.TerminDostawy;
            MagazynDlaMm = baza.MagazynDlaMm;
            KategoriaZamowienia = baza.KategoriaZamowienia;
            BladKomunikat = baza.BladKomunikat;
            PracownikSkladajacyId = baza.PracownikSkladajacyId;
            NazwaPlatnosci = baza.NazwaPlatnosci;
            NumerWlasnyZamowieniaKlienta = baza.NumerWlasnyZamowieniaKlienta;
            NumerTymczasowyZamowienia = baza.NumerTymczasowyZamowienia;
            DodatkowePola = baza.DodatkowePola;
            DefinicjaDokumentuErp = baza.DefinicjaDokumentuErp;
            WartoscBrutto = baza.WartoscBrutto;
            WartoscNetto = baza.WartoscNetto;
            Pliki = baza.Pliki;
        }

        public int? PoziomCenyId { get; set; }

        /// <summary>
        /// Numer tymczasowy dla zamowienia generowany podczas finalizacji koszyka
        /// </summary>
        public string NumerTymczasowyZamowienia { get; set; }

        /// <summary>
        /// Magazyn z jakiego należy wystawić dokument sprzedaży. Jeśli magazyn podstawowy jest inny niż realizujący to wystawiana jest MM z podstawowego na realizujący
        /// </summary>
        [WidoczneListaAdmin(true, false, false, false)]
        [FriendlyName("Magazyn realizujący (na jaki będzi zapisany dokument)", FriendlyOpis = "Magazyn z jakiego należy wystawić dokument sprzedaży. Jeśli magazyn podstawowy jest inny niż realizujący to wystawiana jest MM z podstawowego na realizujący")]
        public string MagazynRealizujacy { get; set; }

        /// <summary>
        /// Dokumenty z ERP jakie zostały zrobione na podstawie tego zamowienia
        /// </summary>
        /// <returns></returns>
        public long? AdresId { get; set; }

        public DateTime? TerminDostawy { get; set; }

        /// <summary>
        /// Magazyn z jakiego towar zostanie ściągnięty. Jeśli magazyn dla MM jest inny niż realizujący to wystawiana jest MM z podstawowego na realizujący
        /// </summary>
        public string MagazynDlaMm { get; set; }

        public string KategoriaZamowienia { get; set; }

        [FriendlyName("Komunikat błędu importu")]
        [WidoczneListaAdmin(true, false, false, false)]
        public virtual string BladKomunikat { get; set; }

        [FriendlyName("Id pracownika składającego zamówienie")]
        [WidoczneListaAdmin(true, false, false, false)]
        public long? PracownikSkladajacyId { get; set; }

        //public HashSet<string> DokumentNazwaSynchronizacja { get; set; }
        public string DodatkowePola { get; set; }

        public string DefinicjaDokumentuErp { get; set; }

        [FriendlyName("Numer zamowienia klienta")]
        public virtual string NumerWlasnyZamowieniaKlienta { get; set; }

        public virtual Adres Adres { get; set; }

        public int[] Pliki { get; set; }

        [Ignore]
        public Dictionary<long, string> dokumentyERPStworzoneZZamowienia { get; set; }

        [FriendlyName("Złożone przez pracownika")]
        [Ignore]
        public bool CzyZlozonePrzezPracownika => PracownikSkladajacyId != null;

        [Ignore]
        public new StatusImportuZamowieniaDoErp StatusId
        {
            get
            {
                return (StatusImportuZamowieniaDoErp)base.StatusId.Value;
            }
            set
            {
                base.StatusId = (int)value;
            }
        }
    }
}