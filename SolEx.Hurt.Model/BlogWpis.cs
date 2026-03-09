using System;
using System.Collections.Generic;
using ServiceStack.DataAnnotations;
using ServiceStack.DesignPatterns.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Model
{
    
    public class BlogWpis:IPolaIDentyfikujaceRecznieDodanyObiekt, IObiektWidocznyDlaOkreslonychGrupKlientow
    {
        public BlogWpis()
        {
            DataDodania = DateTime.Now;
        }
        [PrimaryKey]
        [AutoIncrement]
        [WidoczneListaAdmin(true, true, false, false)]
        public long Id { get; set; }

        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Tytuł")]
        [Lokalizowane]
        [GrupaAtttribute("Ogólne", 1)]
        public string Tytul { get; set; }

        [WidoczneListaAdmin(true, false, false, false)]
        [GrupaAtttribute("Ogólne", 1)]
        [FriendlyName("Data dodania")]
        public DateTime DataDodania { get; set; }

        [WidoczneListaAdmin(true, true, false, false)]
        [GrupaAtttribute("Ogólne", 1)]
        public int? AutorId { get; set; }

        [WidoczneListaAdmin(true, true, true, true)]
        [GrupaAtttribute("Ogólne", 1)]
        [FriendlyName("Dla kogo widoczny")]
        public AccesLevel PoziomWidocznosci { get; set; }

        [WidoczneListaAdmin(true, true, true, true)]
        [GrupaAtttribute("Ogólne", 1)]
        public bool Aktywny { get; set; }

        [WidoczneListaAdmin(true, true, true, false)]
        [WymuszonyTypEdytora(TypEdytora.EdytorTekstowy)]
        [FriendlyName("Treść")]
        [Lokalizowane]
        [GrupaAtttribute("Ogólne", 1)]
        public string Tresc { get; set; }

        [WidoczneListaAdmin(true, true, true, true)]
        [WymuszonyTypEdytora(TypEdytora.PoleZdjecie)]
        [FriendlyName("Zdjęcie")]
        [GrupaAtttribute("Ogólne", 1)]
        public int? ZdjecieId { get; set; }

        [FriendlyName("Blog Tekst 1",true)]
        [Niewymagane]
        [GrupaAtttribute("Pola tekstowe", 10)]
        [Lokalizowane]
        [WidoczneListaAdmin(true, false, true, false)]
        [WymuszonyTypEdytora(TypEdytora.EdytorTekstowy)]
        public string Tekst1 { get; set; }

        [FriendlyName("Blog Tekst 2", true)]
        [Lokalizowane]
        [GrupaAtttribute("Pola tekstowe", 10)]
        [WidoczneListaAdmin(true, false, true, false)]
        [Niewymagane]
        [WymuszonyTypEdytora(TypEdytora.EdytorTekstowy)]
        public string Tekst2 { get; set; }

        [FriendlyName("Blog Tekst 3", true)]
        [GrupaAtttribute("Pola tekstowe", 10)]
        [Lokalizowane]
        [WidoczneListaAdmin(true, false, true, false)]
        [Niewymagane]
        [WymuszonyTypEdytora(TypEdytora.EdytorTekstowy)]
        public string Tekst3 { get; set; }

        [FriendlyName("Blog Tekst 4", true)]
        [GrupaAtttribute("Pola tekstowe", 10)]
        [Lokalizowane]
        [WidoczneListaAdmin(true, false, true, false)]
        [Niewymagane]
        [WymuszonyTypEdytora(TypEdytora.EdytorTekstowy)]

        public string Tekst4 { get; set; }
        [FriendlyName("Blog Tekst 5", true)]
        [Lokalizowane]
        [GrupaAtttribute("Pola tekstowe", 10)]
        [WidoczneListaAdmin(true, false, true, false)]
        [Niewymagane]
        [WymuszonyTypEdytora(TypEdytora.EdytorTekstowy)]
        public string Tekst5 { get; set; }

        [FriendlyName("Blog Zdjęcie 1", true)]
        [Niewymagane]
        [GrupaAtttribute("Zdjęcia", 11)]
        [WidoczneListaAdmin(true, false, true, false)]
        [WymuszonyTypEdytora(TypEdytora.PoleZdjecie)]
        public int? ZdjecieId1 { get; set; }
        [FriendlyName("Blog Zdjęcie 2", true)]
        [Niewymagane]
        [GrupaAtttribute("Zdjęcia", 11)]
        [WidoczneListaAdmin(true, false, true, false)]
        [WymuszonyTypEdytora(TypEdytora.PoleZdjecie)]
        public int? ZdjecieId2 { get; set; }
        [FriendlyName("Blog Zdjęcie 3", true)]
        [Niewymagane]
        [GrupaAtttribute("Zdjęcia", 11)]
        [WidoczneListaAdmin(true, false, true, false)]
        [WymuszonyTypEdytora(TypEdytora.PoleZdjecie)]
        public int? ZdjecieId3 { get; set; }
        [FriendlyName("Blog Zdjęcie 4", true)]
        [Niewymagane]
        [GrupaAtttribute("Zdjęcia", 11)]
        [WidoczneListaAdmin(true, false, true, false)]
        [WymuszonyTypEdytora(TypEdytora.PoleZdjecie)]
        public int? ZdjecieId4 { get; set; }
        [FriendlyName("Blog Zdjęcie 5", true)]
        [Niewymagane]
        [GrupaAtttribute("Zdjęcia", 11)]
        [WidoczneListaAdmin(true, false, true, false)]
        [WymuszonyTypEdytora(TypEdytora.PoleZdjecie)]
        public int? ZdjecieId5 { get; set; }

        [FriendlyName("Słowa kluczowe (tagi) - uzupełniaj po przecinku")]
        [Niewymagane]
        [GrupaAtttribute("Ogólne", 1)]
        [WidoczneListaAdmin(true, false, true, false)]
        public string Tagi { get; set; }

        [FriendlyName("Krótki opis - pod pozycjonowanie")]
        [Niewymagane]
        [GrupaAtttribute("Ogólne", 1)]
        [Lokalizowane]
        [WidoczneListaAdmin(true, false, true, false)]
        public string KrotkiOpis { get; set; }

        [PobieranieSlownika("SolEx.Hurt.Core.Helper.SlownikProduktow,SolEx.Hurt.Core")]
        [GrupaAtttribute("Powiazane produkty", 1)]
        [FriendlyName("Powiazane produkty")]
        [WidoczneListaAdmin(true, false, true, false)]
        [Niewymagane]
        public int[] ListaPowiazanychProduktow { get; set; }

        [PobieranieSlownika("SolEx.Hurt.Core.Helper.SlownikKatalogZasoby,SolEx.Hurt.Core")]
        [FriendlyName("Folder do galerii zdjęć 1")]
        [WidoczneListaAdmin(true, false, true, false)]
        [Niewymagane]
        public string FolderDoGaleriiZdjec1 { get; set; }

        [PobieranieSlownika("SolEx.Hurt.Core.Helper.SlownikKatalogZasoby,SolEx.Hurt.Core")]
        [FriendlyName("Folder do galerii zdjęć 2")]
        [WidoczneListaAdmin(true, false, true, false)]
        [Niewymagane]
        public string FolderDoGaleriiZdjec2 { get; set; }

        [PobieranieSlownika("SolEx.Hurt.Core.Helper.SlownikKatalogZasoby,SolEx.Hurt.Core")]
        [FriendlyName("Folder do galerii zdjęć 3")]
        [WidoczneListaAdmin(true, false, true, false)]
        [Niewymagane]
        public string FolderDoGaleriiZdjec3 { get; set; }

        [WidoczneListaAdmin(true, false, true, false)]
        [Niewymagane]
        [FriendlyName("Link alternatywny", FriendlyOpis = "skrót linka będzie kierował pod ten link")]
        public AdresUrl LinkAlternetywny { get; set; }

        //[WidoczneListaAdmin(true, false, true, false)]
        //[Niewymagane]
        //[FriendlyName("Adres alternatywny url")]
        //[Opis("skrót linka będzie kierował pod ten link")]
        ////[WymuszonyTypEdytora(TypEdytora.AdresUrl)]
        //public AdresUrl AdresAlternetywnyUrl { get; set; }

        [FriendlyName("Blog - wybrana lista 1", true)]
        [Niewymagane]
        [GrupaAtttribute("Listy produktów", 15)]
        [WidoczneListaAdmin(true, false, true, false)]
        [PobieranieSlownika("SolEx.Hurt.Core.Helper.SlownikProduktow,SolEx.Hurt.Core")]
        public HashSet<long> WybraneIdProduktow1 { get; set; }

        [FriendlyName("Blog - wybrana lista 2", true)]
        [GrupaAtttribute("Listy produktów", 15)]
        [WidoczneListaAdmin(true, false, true, false)]
        [Niewymagane]
        [PobieranieSlownika("SolEx.Hurt.Core.Helper.SlownikProduktow,SolEx.Hurt.Core")]
       // [KolekcjaProduktowDoWybranychProduktow]
        public HashSet<long> WybraneIdProduktow2 { get; set; }

        [FriendlyName("Blog - wybrana lista 3", true)]
        [GrupaAtttribute("Listy produktów", 15)]
        [WidoczneListaAdmin(true, false, true, false)]
        [Niewymagane]
        [PobieranieSlownika("SolEx.Hurt.Core.Helper.SlownikProduktow,SolEx.Hurt.Core")]
       // [KolekcjaProduktowDoWybranychProduktow]
        public HashSet<long> WybraneIdProduktow3 { get; set; }

        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Kolejność")]
        [GrupaAtttribute("Ogólne", 1)]
        public int Kolejnosc { get; set; }

        [FriendlyName("Kategorie klientów którym będzie pokazany lub ukryty wpis bloga")]
        [WymuszonyTypEdytora(TypEdytora.WidocznoscDlaKlientow)]
        [Ignore]
        [Niewymagane]
        [WidoczneListaAdmin(false, false, true, false)]
        public WidocznosciTypow Widocznosc { get; set; }

        public bool RecznieDodany()
        {
            return true;
        }
     
    }
}
