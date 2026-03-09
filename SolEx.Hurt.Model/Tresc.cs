using ServiceStack.DataAnnotations;
using ServiceStack.DesignPatterns.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Model
{

    /// <summary>
    /// Klasa wiersza
    /// </summary>
    public class Tresc : IPolaIDentyfikujaceRecznieDodanyObiekt, IObiektWidocznyDlaOkreslonychGrupKlientow
    {
        [AutoIncrement]
        [PrimaryKey]
        public long Id { get; set; }

        public long? NadrzednaId { get; set; }

        [Lokalizowane]
        [Niewymagane]
        [WidoczneListaAdmin(true, true, true, true)]
        [GrupaAtttribute("Ogólne",1)]
        public string Nazwa { get; set; }

        [WalidatorDanych("SolEx.Hurt.Core.BLL.WalidatoryDanych.Tresc.WalidatorSymbol,SolEx.Hurt.Core")]
        [Niewymagane]
        [WidoczneListaAdmin(true, true, true, true)]
        [GrupaAtttribute("Ogólne", 1)]
        public string Symbol { get; set; }

        [FriendlyName("Link")]
        [Niewymagane]
        [WidoczneListaAdmin(true, true, true, true)]
        [GrupaAtttribute("Ogólne", 1)]
        [LinkDokumentacji("/HELP/LinkAlternatywny.html")]
        public string LinkAlternatywny { get; set; }
      
        [FriendlyName("Pokazuj strone w menu")]
        [WidoczneListaAdmin]
        public bool PokazujWMenu { get; set; }

        [WidoczneListaAdmin(true, true, true, true)]
        [GrupaAtttribute("Ogólne", 1)]
        public AccesLevel Dostep { get; set; }

        public int Kolejnosc { get; set; }

        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Autor")]
        [Niewymagane]
        [PobieranieSlownika("SolEx.Hurt.Core.Helper.SlownikPracownikow,SolEx.Hurt.Core")]
        public int? AutorId { get; set; }

        [WidoczneListaAdmin(true, true, true, true)]
        [GrupaAtttribute("Ogólne", 1)]
        public bool Aktywny { get; set; }

        [GrupaAtttribute("Pozycjonowanie", 3)]
        [WidoczneListaAdmin(true, true, true, true)]
        [Niewymagane]
        [Lokalizowane]
        public string MetaOpis { get; set; }

        [GrupaAtttribute("Pozycjonowanie", 3)]
        [WidoczneListaAdmin(true, true, true, true)]
        [Niewymagane]
        [Lokalizowane]
        public string MetaSlowaKluczowe { get; set; }

        public bool Systemowa { get; set; }

        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Kategoria pokazywana jako nagłówek")]
        [Niewymagane]
        [PobieranieSlownika("SolEx.Hurt.Core.Helper.SlownikTresciSystemowych,SolEx.Hurt.Core")]
        public string TrescPokazywanaJakoNaglowek { get; set; }

        [FriendlyName("Kategoria pokazywana jako stopka")]
        [WidoczneListaAdmin(true, true, true, true)]
        [Niewymagane]
        [PobieranieSlownika("SolEx.Hurt.Core.Helper.SlownikTresciSystemowych,SolEx.Hurt.Core")]
        public string TrescPokazywanaJakoStopka { get; set; }

        [FriendlyName("Kategoria pokazywana jako lewe menu")]
        [WidoczneListaAdmin(true, true, true, true)]
        [Niewymagane]
        [PobieranieSlownika("SolEx.Hurt.Core.Helper.SlownikTresciSystemowych,SolEx.Hurt.Core")]
        public string TrescPokazywanaJakoLeweMenu { get; set; }

        [FriendlyName("Szerokość aktualnej strony (treści)")]
        [WidoczneListaAdmin(true, true, true, true)]
        public int Szerokosc { get; set; }
            
         [WidoczneListaAdmin(true, true, true, true)]
         [FriendlyName("Dodatkowe klasy css dla strony")]
         [Niewymagane]
         [PobieranieSlownika("SolEx.Hurt.Core.Helper.SlownikKlassCss,SolEx.Hurt.Core")]
         public string[] DodatkoweKlasyCss { get; set; }
        
         [WidoczneListaAdmin(true, true, true, true)]
         [FriendlyName("Dodatkowe klasy css ręcznie wpisane")]
         [Niewymagane]
         public string DodatkoweKlasyCssReczne { get; set; }
         [WidoczneListaAdmin(true, true, true, true)]
         [FriendlyName("Kategoria pokazywana jako reklama menu")]
         [Niewymagane]
         [PobieranieSlownika("SolEx.Hurt.Core.Helper.SlownikTresciSystemowych,SolEx.Hurt.Core")]
         public string TrescPokazywanaJakoReklamaMenu { get; set; }

        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Widoczne dla ról",FriendlyOpis = "Może być puste wtedy widzą wszyscy")]
        [Niewymagane]
        public RoleType[] Rola { get; set; }
        [FriendlyName("Widoczność")]
        [WymuszonyTypEdytora(TypEdytora.WidocznoscDlaKlientow)]
        [Ignore]
        [WidoczneListaAdmin]
        public WidocznosciTypow Widocznosc { get; set; }

        [FriendlyName("Sposób otwierania treści")]
        [WidoczneListaAdmin]
        public SposobOtwieraniaModal SposobOtwierania { get; set; }

        public Tresc(Tresc tresc)
        {
            if (tresc == null)
            {
                return;
            }
            Id = tresc.Id;
            NadrzednaId = tresc.NadrzednaId;
            Nazwa = tresc.Nazwa;
            Symbol = tresc.Symbol;
            Dostep = tresc.Dostep;
            Kolejnosc = tresc.Kolejnosc;
            Aktywny = tresc.Aktywny;
            AutorId = tresc.AutorId;
            MetaOpis = tresc.MetaOpis;
            MetaSlowaKluczowe = tresc.MetaSlowaKluczowe;
            Systemowa = tresc.Systemowa;
            TrescPokazywanaJakoNaglowek = tresc.TrescPokazywanaJakoNaglowek;
            TrescPokazywanaJakoStopka = tresc.TrescPokazywanaJakoStopka;
            TrescPokazywanaJakoLeweMenu = tresc.TrescPokazywanaJakoLeweMenu;
            DodatkoweKlasyCss = tresc.DodatkoweKlasyCss;
            DodatkoweKlasyCssReczne = tresc.DodatkoweKlasyCssReczne;
            TrescPokazywanaJakoReklamaMenu = tresc.TrescPokazywanaJakoReklamaMenu;
            Rola = tresc.Rola;
            Szerokosc = tresc.Szerokosc;
            LinkAlternatywny = tresc.LinkAlternatywny;
            SposobOtwierania = tresc.SposobOtwierania;
            PokazujWMenu = tresc.PokazujWMenu;
        }

        public Tresc()
        {
            SposobOtwierania=SposobOtwieraniaModal.ZalogowaniNieModalNiezalogowaniNieModal;
            PokazujWMenu = true;
        }
        public bool RecznieDodany()
        {
            return true;
        }
    }
}
