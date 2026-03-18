using ServiceStack.DataAnnotations;
using ServiceStack.DesignPatterns.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;
namespace SolEx.Hurt.Model
{
    public class TrescKolumna :IHasIntId, IPolaIDentyfikujaceRecznieDodanyObiekt, IStringIntern
    {
        [AutoIncrement]
        [PrimaryKeyAttribute]
        public int Id { get; set; }
        [IdentyfikatorObiektuNadrzednego(typeof(TrescWiersz))]
        public int TrescWierszId { get; set; }

        [StringInternuj]
        [Lokalizowane]
        public string ParametryKontrolkiSpecyficzne { get; set; }

        public bool RecznieDodany()
        {
            return true;
        }

        public TrescKolumna()
        {
            Szerokosc = 2;
            Dostep = AccesLevel.Wszyscy;
            OpisKontenera = "";
            DodatkoweKlasyCssReczneKolumny = "";
        }

        [Wymagane]
        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Opis kontenera")]
        [GrupaAtttribute("Ogólne", 1)]
        public string OpisKontenera { get; set; }

        public int Kolejnosc { get; set; }

        [WalidatorDanych("SolEx.Hurt.Core.BLL.WalidatoryDanych.TrescPojemnik.WalidatorSzerokosc,SolEx.Hurt.Core")]
        [GrupaAtttribute("Ogólne", 1)]
        [WidoczneListaAdmin(true, true, true, true)]
        public int Szerokosc { get; set; }

        [StringInternuj]
        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Kolor tła pojemnika")]
        [WymuszonyTypEdytora(TypEdytora.PoleKolor)]
        [GrupaAtttribute("Ogólne", 1)]
        public string KolorTla { get; set; }

        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Zdjęcia tła pojemnika")]
        [WymuszonyTypEdytora(TypEdytora.PoleZdjecie)]
        [GrupaAtttribute("Ogólne", 1)]
        public int? ObrazekTla { get; set; }

         [WidoczneListaAdmin(true, true, true, true)]
         [FriendlyName("Dodatkowe klasy css dla kontrolki")]
         [Niewymagane]
         [GrupaAtttribute("Ogólne", 1)]
         [PobieranieSlownika("SolEx.Hurt.Core.Helper.SlownikKlassCss,SolEx.Hurt.Core")]
         public string[] DodatkoweKlasyCssKolumny { get; set; }

        [StringInternuj]
        [WidoczneListaAdmin(true, true, true, true)]
         [FriendlyName("Dodatkowe klasy css ręcznie wpisane dla kontrolki")]
         [Niewymagane]
         [GrupaAtttribute("Ogólne", 1)]
         public string DodatkoweKlasyCssReczneKolumny { get; set; }

        [StringInternuj]
        public string RodzajKontrolki { get; set; }
         
         [WidoczneListaAdmin(true, true, true, true)]
         public AccesLevel Dostep { get; set; }

         [Niewymagane]
        [StringInternuj]
        [GrupaAtttribute("Ogólne", 1)]
         [WidoczneListaAdmin(true, true, true, true)]
         [FriendlyName("Margines - góra prawa dół lewo - wszystko w px - np. 5px 0px 5px 0px")]
         public string Marginesy { get; set; }

         [Niewymagane]
        [StringInternuj]
        [WidoczneListaAdmin(true, true, true, true)]
         [FriendlyName("Odstęp (padding) - góra prawa dół lewo - wszystko w px - np. 5px 0px 5px 0px")]
         [GrupaAtttribute("Ogólne", 1)]
         public string Paddingi { get; set; }


    }
}
