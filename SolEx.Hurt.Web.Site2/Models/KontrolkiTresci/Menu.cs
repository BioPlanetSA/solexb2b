using System.Web.Mvc;
using System.Web.Routing;
using System.Web.UI.HtmlControls;
using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Web.Site2.Controllers;

namespace SolEx.Hurt.Web.Site2.Models.KontrolkiTresci
{
    public class Menu : KontrolkaTresciBaza
    {
        public override string Nazwa
        {
            get { return "Menu"; }
        }

        public override string Kontroler
        {
            get { return "Tresci"; }
        }

        public override string Akcja
        {
            get { return "Menu"; }
        }

        public override string Grupa
        {
            get { return "Wygląd"; }
        }

        public Menu()
        {
            MenuRodzajMenu = "MenuKlasyczneRozwijane";
            MenuSzerokoscKolumny = 3;
        }

        [WidoczneListaAdmin(true, true, true, true)]
        public virtual bool PokazujPodkategorie { get; set; }

        [WidoczneListaAdmin(true, true, true, true)]
        public bool PokazujReklamy { get; set; }

        [WidoczneListaAdmin(true,true,true,true)]
        [PobieranieSlownika(typeof(SlownikDrzewkoMenu))]
        [FriendlyName("Symbol korzenia treści")]
        [Wymagane]
        public string SymbolKorzen { get; set; }

        [WymuszonyTypEdytora(TypEdytora.PoleDropDown)]
        [WidoczneListaAdmin(true, true, true, true)]
        [PobieranieSlownika(typeof(SlownikSzerokosciKolumny))]
        [FriendlyName("Menu szerokość kolumny",FriendlyOpis = "Opcja wykorzystywana tylko w menu kolumnowym")]
        public int MenuSzerokoscKolumny { get; set; }

        [WidoczneListaAdmin(true, true, true, true)]
        [PobieranieSlownika(typeof(SlownikRodzajowMenu))]
        [Wymagane]
        [FriendlyName("Rodzaj menu")]
        public string MenuRodzajMenu { get; set; }

        //[WidoczneListaAdmin]
        //[FriendlyName("Czy w zakładce produktów dynamicznie ładować drzewo kategorii produktów?")]
        //public bool KategorieProduktoweMenu { get; set; }

        //[WidoczneListaAdmin]
        //[FriendlyName("Pokazć liczbę produktów w danej kategorii w drzewku kategorii")]
        //public bool PokazLiczbeProduktowWDrzewku { get; set; }
    }
}