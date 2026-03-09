using ServiceStack.DataAnnotations;
using ServiceStack.DesignPatterns.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Model
{
    [FriendlyClassName("Model kategorii produktowej")]
    public class KategoriaProduktu : IPolaIDentyfikujaceRecznieDodanyObiekt, IHasLongId, IPoleJezyk
    {
        public KategoriaProduktu() {
            Dostep=AccesLevel.Wszyscy;
            PokazujFiltry = true;
        }

        public KategoriaProduktu(KategoriaProduktu c):this()
        {
            if (c == null) return;
            Id = c.Id;
            Nazwa = c.Nazwa;
            Widoczna = c.Widoczna;
            ObrazekId = c.ObrazekId;
            ParentId = c.ParentId;
            Opis = c.Opis;
            //OpisKrotki = c.OpisKrotki;
            GrupaId = c.GrupaId;
            Kolejnosc = c.Kolejnosc;
            PokazujFiltry = c.PokazujFiltry;
            LinkUrl = c.LinkUrl;
            MetaOpis = c.MetaOpis;
            MetaSlowaKluczowe = c.MetaSlowaKluczowe;
            Dostep = c.Dostep;
            MiniaturaId = c.MiniaturaId;
            OpisNaProdukt = c.OpisNaProdukt;
            KategoriaTresciSymbol = c.KategoriaTresciSymbol;
            KlasaCss = c.KlasaCss;
            Tekst1 = c.Tekst1;
            Tekst2 = c.Tekst2;
            Tekst3 = c.Tekst3;
            Tekst4 = c.Tekst4;
            Tekst5 = c.Tekst5;
            ZdjecieId1 = c.ZdjecieId1;
            ZdjecieId2 = c.ZdjecieId2;
            ZdjecieId3 = c.ZdjecieId3;
            ZdjecieId4 = c.ZdjecieId4;
            ZdjecieId5 = c.ZdjecieId5;
            JezykId = c.JezykId;
        }

        [UpdateColumnKey]
        [PrimaryKey]
        [WidoczneListaAdmin(true, true, false, false)]
        public long Id {get;set;}

        [Lokalizowane]
        [GrupaAtttribute("Dane podstawowe", 0)]
        [WidoczneListaAdmin(true, true, true, true)]
        public string Nazwa {get;set;}

        [GrupaAtttribute("Dane podstawowe", 0)]
        [WidoczneListaAdmin(true, true, true, false)]
        public bool Widoczna {get;set;}

        [Niewymagane]
        [GrupaAtttribute("Zdjęcia", 11)]
        [FriendlyName("Obrazek")]
        [WidoczneListaAdmin(true, false, true, false)]
        [WymuszonyTypEdytora(TypEdytora.PoleZdjecie)]
        public int? ObrazekId {get;set;}

        [WidoczneListaAdmin(true, true, false, false)]
        [Niewymagane]
        public long? ParentId {get;set;}

        [Lokalizowane]
        [GrupaAtttribute("Wygląd na stronie", 0)]
        [WidoczneListaAdmin(false, false, true, false)]
        [WymuszonyTypEdytora(TypEdytora.EdytorTekstowy)]
        [Niewymagane]
        public string Opis { get; set; }

        [GrupaAtttribute("Dane podstawowe", 0)]
        [WidoczneListaAdmin(true, true, true, false)]
        [Wymagane]
        [PobieranieSlownika("SolEx.Hurt.Core.Helper.SlownikGrupy,SolEx.Hurt.Core")]
        [FriendlyName("Grupa")]
        public long GrupaId {get;set;}

        [GrupaAtttribute("Dane podstawowe", 0)]
        [FriendlyName("Kolejność")]
        [WidoczneListaAdmin(true, true, true, true)]
        [Niewymagane]
        public int Kolejnosc {get;set;}

        [GrupaAtttribute("Dane podstawowe", 0)]
        [FriendlyName("Pokazuj filtry w kategorii")]
        [WidoczneListaAdmin(true, false, true, false)]
        public bool PokazujFiltry {get;set;}

        [GrupaAtttribute("Dane podstawowe", 0)]
        [FriendlyName("Link Url", FriendlyOpis = "skrót linka będzie kierował pod ten link, aby kierować do zewnętrznej strony, link musi zaczynać się od http://")]
        [WidoczneListaAdmin(false, false, true, false)]
        [Niewymagane]
        public AdresUrl LinkUrl { get; set; }

        [FriendlyName("Opis")]
        [GrupaAtttribute("Pozycjonowanie", 1)]
        [WidoczneListaAdmin(false, false, true, false)]
        [WymuszonyTypEdytora(TypEdytora.EdytorTekstowy)]
        [Niewymagane]
        public string MetaOpis { get; set; }

        [Niewymagane]
        [FriendlyName("Słowa kluczowe")]
        [GrupaAtttribute("Pozycjonowanie", 1)]
        [WidoczneListaAdmin(false, false, true, false)]
        [WymuszonyTypEdytora(TypEdytora.EdytorTekstowy)]
        public string MetaSlowaKluczowe { get; set; }

        [GrupaAtttribute("Dane podstawowe", 0)]
        [FriendlyName("Dostęp")]
        [WidoczneListaAdmin(false, false, true, false)]
        public AccesLevel Dostep {get;set;}

        [FriendlyName("Ikona w drzewie kategorii")]
        [GrupaAtttribute("Dane podstawowe", 0)]
        [WidoczneListaAdmin(false, false, true, false)]
        [WymuszonyTypEdytora(TypEdytora.PoleZdjecie)]
        [Niewymagane]
        public int? MiniaturaId { get; set; }

        [Lokalizowane]
        [GrupaAtttribute("Wygląd na stronie", 0)]
        [FriendlyName("Opis na kartę produktów - dla wszystkich produktów z kategorii")]
        [WidoczneListaAdmin(false, false, true, false)]
        [WymuszonyTypEdytora(TypEdytora.EdytorTekstowy)]
        [Niewymagane]
        public string OpisNaProdukt { get; set; }

        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Treść pokazywana jako opis (schemat)")]
        [GrupaAtttribute("Wygląd na stronie", 0)]
        [Niewymagane]
        [PobieranieSlownika("SolEx.Hurt.Core.Helper.SlownikTresciSystemowych,SolEx.Hurt.Core")]
        public string KategoriaTresciSymbol { get; set; }
        
        [FriendlyName("Klasa CSS")]
        [GrupaAtttribute("CSS", 2)]
        [WidoczneListaAdmin(false, false, true, false)]
        [Niewymagane]
        public string KlasaCss { get; set; }

        [FriendlyName("Kategoria produktu Tekst 1", true)]
        [Niewymagane]
        [Lokalizowane]
        [GrupaAtttribute("Pola tekstowe", 10)]
        [WidoczneListaAdmin(true, false, true, false)]
        [WymuszonyTypEdytora(TypEdytora.EdytorTekstowy)]
        public string Tekst1 { get; set; }

        [FriendlyName("Kategoria produktu Tekst 2", true)]
        [GrupaAtttribute("Pola tekstowe", 10)]
        [WidoczneListaAdmin(true, false, true, false)]
        [Niewymagane]
        [Lokalizowane]
        [WymuszonyTypEdytora(TypEdytora.EdytorTekstowy)]
        public string Tekst2 { get; set; }

        [FriendlyName("Kategoria produktu Tekst 3", true)]
        [GrupaAtttribute("Pola tekstowe", 10)]
        [WidoczneListaAdmin(true, false, true, false)]
        [Niewymagane]
        [Lokalizowane]
        [WymuszonyTypEdytora(TypEdytora.EdytorTekstowy)]
        public string Tekst3 { get; set; }

        [FriendlyName("Kategoria produktu Tekst 4", true)]
        [GrupaAtttribute("Pola tekstowe", 10)]
        [WidoczneListaAdmin(true, false, true, false)]
        [Niewymagane]
        [WymuszonyTypEdytora(TypEdytora.EdytorTekstowy)]
        [Lokalizowane]
        public string Tekst4 { get; set; }
        [FriendlyName("Kategoria produktu Tekst 5", true)]
        [GrupaAtttribute("Pola tekstowe", 10)]
        [WidoczneListaAdmin(true, false, true, false)]
        [Niewymagane]
        [Lokalizowane]
        [WymuszonyTypEdytora(TypEdytora.EdytorTekstowy)]
        public string Tekst5 { get; set; }

        [FriendlyName("Kategoria produktu Zdjęcie 1", true)]
        [Niewymagane]
        [Lokalizowane]
        [GrupaAtttribute("Zdjęcia", 11)]
        [WidoczneListaAdmin(true, false, true, false)]
        [WymuszonyTypEdytora(TypEdytora.PoleZdjecie)]
        public int? ZdjecieId1 { get; set; }

        [FriendlyName("Kategoria produktu Zdjęcie 2", true)]
        [Niewymagane]
        [GrupaAtttribute("Zdjęcia", 11)]
        [WidoczneListaAdmin(true, false, true, false)]
        [WymuszonyTypEdytora(TypEdytora.PoleZdjecie)]
        public int? ZdjecieId2 { get; set; }

        [FriendlyName("Kategoria produktu Zdjęcie 3", true)]
        [Niewymagane]
        [GrupaAtttribute("Zdjęcia", 11)]
        [WidoczneListaAdmin(true, false, true, false)]
        [WymuszonyTypEdytora(TypEdytora.PoleZdjecie)]
        public int? ZdjecieId3 { get; set; }
        [FriendlyName("Kategoria produktu Zdjęcie 4", true)]
        [Niewymagane]
        [GrupaAtttribute("Zdjęcia", 11)]
        [WidoczneListaAdmin(true, false, true, false)]
        [WymuszonyTypEdytora(TypEdytora.PoleZdjecie)]
        public int? ZdjecieId4 { get; set; }
        [FriendlyName("Kategoria produktu Zdjęcie 5", true)]
        [Niewymagane]
        [GrupaAtttribute("Zdjęcia", 11)]
        [WidoczneListaAdmin(true, false, true, false)]
        [WymuszonyTypEdytora(TypEdytora.PoleZdjecie)]
        public int? ZdjecieId5 { get; set; }


        public bool RecznieDodany()
        {
            return Id < 0;
        }
        [Ignore]
        public int JezykId { get; set; }
    }
}
