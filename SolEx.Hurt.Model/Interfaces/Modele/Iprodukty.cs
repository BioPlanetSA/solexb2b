using System;
using System.Collections.Generic;
using ServiceStack.DesignPatterns.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Web;

namespace SolEx.Hurt.Model.Interfaces
{
    public interface IProdukty : IHasLongId
    {
        
        [FriendlyName("Nazwa produktu")]
        [WidoczneListaAdmin( true,true,true,true)]
        string Nazwa { get; set; }
        
        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Symbol produktu")]
        string Kod { get; set; }
        
        [FriendlyName("Czy towar widoczny")]
        bool Widoczny { get; set; }

        [FriendlyName("Stan minimalny")]
        decimal StanMin { get; set; }

        [FriendlyName("Iloœæ w opakowaniu zbiorczym")]
        decimal IloscWOpakowaniu { get; set; }

        [FriendlyName("Kod kreskowy")]
        string KodKreskowy { get; set; }
        
        decimal Vat { get; set; }

        [FriendlyName("PKWiU")]
        string PKWiU { get; set; }

        [FriendlyName("Dostawa")]

        string Dostawa { get; set; }

        [FriendlyName("Rodzina")]
        string Rodzina { get; set; }

        [FriendlyName("Posiada ojca")]
        bool Ojciec { get; set; }
        
        [FriendlyName("Adres www")]
        string Www { get; set; }
        
        [FriendlyName("Waga")]
        decimal? Waga { get; set; }

        [FriendlyName("Opis")]
        
        string Opis { get; set; }

        [FriendlyName("Opis krótki")]
        
        string OpisKrotki { get; set; }

        [FriendlyName("Opis 2")]
        string Opis2 { get; set; }

        [FriendlyName("Krótki opis 2")]
        string OpisKrotki2 { get; set; }

        [FriendlyName("Opis 3")]
        string Opis3 { get; set; }

        [FriendlyName("Krótki opis 3")]
        string OpisKrotki3 { get; set; }

        [FriendlyName("Opis 4")]
        string Opis4 { get; set; }

        [FriendlyName("Krótki opsis 4")]
        string OpisKrotki4 { get; set; }
        
        [FriendlyName("Opis 5")]
        string Opis5 { get; set; }

        [FriendlyName("Krótki opis 5")]
        string OpisKrotki5 { get; set; }

        [FriendlyName("Pole testowe 1")]
        string PoleTekst1 { get; set; }

        [FriendlyName("pole_tekst 2")]
        string PoleTekst2 { get; set; }

        [FriendlyName("Pole tekst 3")]
        string PoleTekst3 { get; set; }
      
        [FriendlyName("Pole tekst 4")]
        string PoleTekst4 { get; set; }
        
        [FriendlyName("Pole tekst 5")]
        string PoleTekst5 { get; set; }

        [FriendlyName("Pole liczba 1")]
        decimal? PoleLiczba1 { get; set; }

        [FriendlyName("Pole liczba 2")]
        decimal? PoleLiczba2 { get; set; }

        [FriendlyName("Pole liczba 3")]
        decimal? PoleLiczba3 { get; set; } 

        [FriendlyName("Pole liczba 4")]
        decimal? PoleLiczba4 { get; set; }

        [FriendlyName("Pole liczba 5")]
        decimal? PoleLiczba5 { get; set; }

        [FriendlyName("Kolumna liczba 1")]
        int? KolumnaLiczba1 { get; set; }

        [FriendlyName("Kolumna liczba 2")]
        int? KolumnaLiczba2 { get; set; }
        
        [FriendlyName("Kolumna liczba 3")]
        int? KolumnaLiczba3 { get; set; }
        
        [FriendlyName("Kolumna liczba 4")]
        int? KolumnaLiczba4 { get; set; }
        
        [FriendlyName("Kolumna liczba 5")]
        int? KolumnaLiczba5 { get; set; }

        [FriendlyName("Kolumna tekst 1")]
        string KolumnaTekst1 { get; set; }

        [FriendlyName("Kolumna tekst 2")]
        string KolumnaTekst2 { get; set; }

        [FriendlyName("Kolumna tekst 3")]
        string KolumnaTekst3 { get; set; }
        
        [FriendlyName("Kolumna tekst 4")]
        string KolumnaTekst4 { get; set; }
        
        [FriendlyName("Kolumna tekst 5")]
        string KolumnaTekst5 { get; set; }

        [FriendlyName("Iloœæ minimalna")]
        decimal IloscMinimalna { get; set; }

        int? PrzedstawicielId { get; set; }
        

        [FriendlyName("Komunikat popup")]
        string PopupKomunikat { get; set; }
        
        string PopupTekst { get; set; }

        [FriendlyName("Czy jest wymóg zakupu pe³nego opakowania zbiorczego")]
        bool WymaganeOz { get; set; }

        [FriendlyName("Typ produktu")]
        TypProduktu Typ { get; set; }

       
        [FriendlyName("Data dodania")]
        DateTime DataDodania { get; set; }

        [FriendlyName("Status ukryty dla produktu")]
        int? StatusUkryty { get; set; }

      

        [FriendlyName("Odwrotne oci¹¿enie vat")]
        bool VatOdwrotneObciazenie { get; set; }

        [FriendlyName("Objêtoœæ pozycji w m^3")]
        decimal? Objetosc { get; set; }

        [FriendlyName("Numer Id menagera")]
        long? MenagerId { get; set; }

        [FriendlyName("Widocznoœæ przedmiotu")]
        AccesLevel? Widocznosc { get; set; }

        [FriendlyName("Przedmiot nie podlega rabatowi")]
        bool NiePodlegaRabatowaniu { get; set; }

        [FriendlyName("Meta opis przedmiotu")]
        string MetaOpis { get; set; }

        [FriendlyName("Meta s³owa kluczowa dla przedmiotu")]
        string MetaSlowaKluczowe { get; set; }
        [FriendlyName("Informacja czy by³ wys³any email o pojawieniu sie produktu")]
        [WidoczneListaAdmin(true, true, true, true)]
        bool WyslanoMailNowyProdukt { get; set; }

        /// <summary>
        /// wykorzystywany tylko przez modu³y dodatkowe do ustawienia widocznoœci produktów
        /// </summary>
        /// <param name="widocznosc"></param>
        /// <param name="status"></param>
        void UstawWidocznoscProduktu(bool widocznosc, int? status = null);

        decimal? OpJednostkoweGlebokosc { get; set; }
        decimal? OpJednostkoweSzerokosc { get; set; }
        decimal? OpJednostkoweWysokosc { get; set; }
        //decimal? OpJednostkoweObjetosc { get; set; }
        //decimal? OpJednostkoweWaga { get; set; }

        decimal? OpZbiorczeGlebokosc { get; set; }
        decimal? OpZbiorczeSzerokosc { get; set; }
        decimal? OpZbiorczeWysokosc { get; set; }
        decimal? OpZbiorczeObjetosc { get; set; }
        decimal? OpZbiorczeWaga { get; set; }
        decimal? OpZbiorczeIloscWOpakowaniu { get; set; }

        decimal? OpPaletaGlebokosc { get; set; }
        decimal? OpPaletaSzerokosc { get; set; }
        decimal? OpPaletaWysokosc { get; set; }
        decimal? OpPaletaObjetosc { get; set; }
        decimal? OpPaletaWaga { get; set; }


        decimal? OpPaletaIloscNaWarstwie { get; set; }
        decimal? OpPaletaIloscWOpakowaniu { get; set; }

        [FriendlyName("Cena w punktach")]
        decimal CenaWPunktach { get; set; }
    }
}