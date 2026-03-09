using ServiceStack.DesignPatterns.Model;
using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;

namespace SolEx.Hurt.Web.Site2.Models.KontrolkiTresci
{
    public enum SposobyPokazaniaGrupKategoriiProduktow
    {
        Taby = 1,
        Metki = 2,
        Akordion = 3
    }

    public interface IDrzewoKategorii: IHasIntId
    {
        SposobyPokazaniaGrupKategoriiProduktow PokazujGrupyWgSposobu { get; set; }
        bool PokazLiczbeProduktowWDrzewku { get; set; }
        bool WszystkieKategorieRozwiniete { get; set; }
        bool PokazujKategorieWPostaciObrazkow { get; set; }
        string RozmiarObrazka { get; set; }
    }

    public class DrzewoKategorii : KontrolkaTresciBaza, IDrzewoKategorii
    {
        public DrzewoKategorii()
        {
            PokazujGrupyWgSposobu = SposobyPokazaniaGrupKategoriiProduktow.Taby;
            PokazLiczbeProduktowWDrzewku = true;
            WszystkieKategorieRozwiniete = false;

            PokazujKategorieWPostaciObrazkow = true;
            RozmiarObrazka = "ikona155";
        }
        public override string Grupa
        {
            get { return "Produkty"; }
        }

        public override string Nazwa
        {
            get { return "Drzewo kategorii produktów"; }
        }

        public override string Kontroler
        {
            get { return "KategorieProduktowe"; }
        }

        public override string Akcja
        {
            get { return "Drzewko"; }
        }

        public new  TrescKolumna DomyslneWartosciDlaNowejKontrolki = new TrescKolumna()
        {
            Szerokosc = 12,
            DodatkoweKlasyCssKolumny = new string[] { "hidden-md-down" }
        };

        [FriendlyName("Pokazuj grupy kategorii w tabach")]
        [WidoczneListaAdmin(true, true, true, true)]
        public SposobyPokazaniaGrupKategoriiProduktow PokazujGrupyWgSposobu { get; set; }

        [FriendlyName("Czy w drzewku kategorii ma się pokazywać liczba produktów")]
        [WidoczneListaAdmin(true, false, true, false)]
        public bool PokazLiczbeProduktowWDrzewku { get; set; }

        [FriendlyName("Czy domyślnie wszystkie kategorie renderować rozwinięte?")]
        [Wymagane]
        [WidoczneListaAdmin(true, true, true, true)]
        public bool WszystkieKategorieRozwiniete { get; set; }


        [FriendlyName("Obrazki kategorii - pokazuj obrazki dla kategorii które mają podany obrazek")]
        [WidoczneListaAdmin(true, true, true, true)]
        public bool PokazujKategorieWPostaciObrazkow { get; set; }

        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Obrazki kategorii - rozmiar obrazka")]
        [PobieranieSlownika(typeof(SlownikRozmiarZdjec))]
        public string RozmiarObrazka { get; set; }
    }
}