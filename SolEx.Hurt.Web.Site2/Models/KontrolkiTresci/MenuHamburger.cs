using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Web.Site2.Models.KontrolkiTresci
{
    public class MenuHamburger: Menu, IDrzewoKategorii
    {
        public override string Nazwa
        {
            get { return "Menu hamburger"; }
        }

        public override string Kontroler
        {
            get { return "Tresci"; }
        }

        public override string Akcja
        {
            get { return "MenuHamburger"; }
        }

        public MenuHamburger()
        {
            this.MenuRodzajMenu = "MenuRozsuwane";
            base.MenuRodzajMenu = "MenuRozsuwane";

            PokazujReklamy = false;
            PokazujPodkategorie = true;
            PokazLiczbeProduktowWDrzewku = false;
            PokazujGrupyWgSposobu = SposobyPokazaniaGrupKategoriiProduktow.Akordion;
            WszystkieKategorieRozwiniete = false;
            PokazujWyloguj = true;

            PokazujKategorieWPostaciObrazkow = false;
            RozmiarObrazka = "ikona155";
        }

        //ukrywanie niepotrzebnych pol - z dziedziczenia z MENU
        [WidoczneListaAdmin(false, false, false, false)]
        [Niewymagane]
        public new string MenuRodzajMenu { get; set; }

        [WidoczneListaAdmin(false, false, false, false)]
        [Niewymagane]
        public new int MenuSzerokoscKolumny { get; set; }

        [WidoczneListaAdmin(false, false, false, false)]
        [Niewymagane]
        public override bool PokazujPodkategorie { get; set; }

        [WidoczneListaAdmin(false, false, false, false)]
        [Niewymagane]
        public new  bool PokazujReklamy { get; set; }

        [FriendlyName("Czy pokazywać drzewo kategorii produktów?")]
        [Wymagane]
        [WidoczneListaAdmin(false, false, true, true)]
        public bool PokazujKategorieProduktow { get; set; }

        /// <summary>
        /// kategorie produktow pokazuj w tabach
        /// </summary>
        [Niewymagane]
        [WidoczneListaAdmin(false, false, false, false)]
        public SposobyPokazaniaGrupKategoriiProduktow PokazujGrupyWgSposobu { get; set; }

        [FriendlyName("Czy w kategoriach produktów pokazywać liczbe produktów?")]
        [Wymagane]
        [WidoczneListaAdmin(false, false, true, true)]
        public bool PokazLiczbeProduktowWDrzewku { get; set; }

        [Niewymagane]
        [WidoczneListaAdmin(false, false, false, false)]
        public bool WszystkieKategorieRozwiniete { get; set; }

        [FriendlyName("Pokazuj kategorie w postaci obrazków - tylko dla kategorii które mają podany obrazek")]
        [WidoczneListaAdmin(true, true, true, true)]
        public bool PokazujKategorieWPostaciObrazkow { get; set; }

        [FriendlyName("Czy pokazywać przycisk wyloguj?")]
        [Wymagane]
        [WidoczneListaAdmin(false, false, true, true)]
        public bool PokazujWyloguj { get; set; }


        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Obrazki kategorii - rozmiar obrazka")]
        [PobieranieSlownika(typeof(SlownikRozmiarZdjec))]
        public string RozmiarObrazka { get; set; }
    }
}