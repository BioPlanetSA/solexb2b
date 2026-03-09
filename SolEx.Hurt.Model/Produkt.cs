using System;
using System.Collections.Generic;
using ServiceStack.DataAnnotations;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;
using SolEx.Hurt.Model.Web;

namespace SolEx.Hurt.Model
{
    /// <summary>
    /// Model produktu
    /// </summary>
    [TworzDynamicznieTabele]
    public class Produkt : IProdukty, IPolaIDentyfikujaceRecznieDodanyObiekt, IStringIntern
    {
        [WidoczneListaAdmin(true, true, false, false)]
        [FriendlyName("Id produktu")]
        public long Id { get; set; }

        [WidoczneListaAdmin(true, false, false, false)]
        [FriendlyName("Abstrakcyjny")]
        public virtual bool Abstrakcyjny { get; set; }

        [GrupaAtttribute("Podstawowe informacje", 0)]
        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Nazwa produktu")]
        [Lokalizowane]
        public virtual string Nazwa { get; set; }

        [GrupaAtttribute("Podstawowe informacje", 0)]
        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Symbol")]
        public string Kod { get; set; }

        [GrupaAtttribute("Podstawowe informacje", 0)]
        [FriendlyName("Czy towar widoczny")]
        [WidoczneListaAdmin(true, false, true, true)]
        public bool Widoczny { get; set; }

        [FriendlyName("Stan minimalny")]
        public decimal StanMin { get; set; }

        [GrupaAtttribute("Podstawowe informacje", 0)]
        [WidoczneListaAdmin(true, false, true, false)]
        [FriendlyName("Iloœæ w opakowaniu zbiorczym")]
        public decimal IloscWOpakowaniu { get; set; }

        [Niewymagane]
        [GrupaAtttribute("Podstawowe informacje", 0)]
        [WidoczneListaAdmin(true, false, true, true)]
        [WalidatorDanych("SolEx.Hurt.Core.BLL.WalidatoryDanych.Produkty.WalidatorKoduKreskowego,SolEx.Hurt.Core")]
        [FriendlyName("Kod kreskowy")]
        public string KodKreskowy { get; set; }

        [GrupaAtttribute("Podstawowe informacje", 0)]
        [WidoczneListaAdmin(true, false, true, false)]
        public decimal Vat { get; set; }

        [Niewymagane]
        [StringInternuj]
        [FriendlyName("PKWiU")]
        [WidoczneListaAdmin(true, false, true, false)]
        public string PKWiU { get; set; }

        [StringInternuj]
        [GrupaAtttribute("Opisy", 1)]
        [WidoczneListaAdmin(true, false, true, false)]
        [WymuszonyTypEdytora(TypEdytora.EdytorTekstowy)]
        [Niewymagane]
        [Lokalizowane]
        [FriendlyName("Opis")]
        public string Opis { get; set; }

        [StringInternuj]
        [GrupaAtttribute("Opisy", 1)]
        [WidoczneListaAdmin(true, false, true, false)]
        [FriendlyName("Opis krótki")]
        [MaksymalnaLiczbaZnakow(4000)]
        [Niewymagane]
        [Lokalizowane]
        public string OpisKrotki { get; set; }

        [StringInternuj]
        [GrupaAtttribute("Opisy", 1)]
        [WidoczneListaAdmin(true, false, true, false)]
        [FriendlyName("Opis 2")]
        [Niewymagane]
        [Lokalizowane]
        public string Opis2 { get; set; }

        [StringInternuj]
        [GrupaAtttribute("Opisy", 1)]
        [WidoczneListaAdmin(true, false, true, false)]
        [FriendlyName("Krótki opis 2")]
        [MaksymalnaLiczbaZnakow(4000)]
        [Niewymagane]
        [Lokalizowane]
        public string OpisKrotki2 { get; set; }

        [StringInternuj]
        [GrupaAtttribute("Opisy", 1)]
        [WidoczneListaAdmin(true, false, true, false)]
        [Niewymagane]
        [FriendlyName("Opis 3")]
        [Lokalizowane]
        public string Opis3 { get; set; }

        [StringInternuj]
        [GrupaAtttribute("Opisy", 1)]
        [WidoczneListaAdmin(true, false, true, false)]
        [FriendlyName("Krótki opis 3")]
        [MaksymalnaLiczbaZnakow(4000)]
        [Niewymagane]
        [Lokalizowane]
        public string OpisKrotki3 { get; set; }

        [StringInternuj]
        [GrupaAtttribute("Opisy", 1)]
        [WidoczneListaAdmin(true, false, true, false)]
        [FriendlyName("Opis 4")]
        [Niewymagane]
        [Lokalizowane]
        public string Opis4 { get; set; }

        [StringInternuj]
        [GrupaAtttribute("Opisy", 1)]
        [WidoczneListaAdmin(true, false, true, false)]
        [FriendlyName("Krótki opsis 4")]
        [MaksymalnaLiczbaZnakow(4000)]
        [Niewymagane]
        [Lokalizowane]
        public string OpisKrotki4 { get; set; }

        [StringInternuj]
        [GrupaAtttribute("Opisy", 1)]
        [WidoczneListaAdmin(true, false, true, false)]
        [FriendlyName("Opis 5")]
        [Niewymagane]
        [Lokalizowane]
        public string Opis5 { get; set; }

        [StringInternuj]
        [GrupaAtttribute("Opisy", 1)]
        [WidoczneListaAdmin(true, false, true, false)]
        [FriendlyName("Krótki opis 5")]
        [MaksymalnaLiczbaZnakow(4000)]
        [Niewymagane]
        [Lokalizowane]
        public string OpisKrotki5 { get; set; }

        [StringInternuj]
        [GrupaAtttribute("Opisy", 1)]
        [WidoczneListaAdmin(true, false, true, false)]
        [FriendlyName("Pole testowe 1")]
        [MaksymalnaLiczbaZnakow(1000)]
        [Niewymagane]
        [Lokalizowane]
        public string PoleTekst1 { get; set; }

        [StringInternuj]
        [GrupaAtttribute("Opisy", 1)]
        [WidoczneListaAdmin(true, false, true, false)]
        [FriendlyName("Pole tekst 2")]
        [MaksymalnaLiczbaZnakow(1000)]
        [Niewymagane]
        [Lokalizowane]
        public string PoleTekst2 { get; set; }

        [StringInternuj]
        [GrupaAtttribute("Opisy", 1)]
        [WidoczneListaAdmin(true, false, true, false)]
        [FriendlyName("Pole tekst 3")]
        [MaksymalnaLiczbaZnakow(1000)]
        [Niewymagane]
        [Lokalizowane]
        public string PoleTekst3 { get; set; }

        [StringInternuj]
        [GrupaAtttribute("Opisy", 1)]
        [WidoczneListaAdmin(true, false, true, false)]
        [FriendlyName("Pole tekst 4")]
        [MaksymalnaLiczbaZnakow(1000)]
        [Niewymagane]
        [Lokalizowane]
        public string PoleTekst4 { get; set; }

        [StringInternuj]
        [GrupaAtttribute("Opisy", 1)]
        [WidoczneListaAdmin(true, false, true, false)]
        [FriendlyName("Pole tekst 5")]
        [MaksymalnaLiczbaZnakow(1000)]
        [Niewymagane]
        [Lokalizowane]
        public string PoleTekst5 { get; set; }

        [FriendlyName("Pole liczba 1")]
        [WidoczneListaAdmin(true, false, true, false)]
        public decimal? PoleLiczba1 { get; set; }

        [WidoczneListaAdmin(true, false, true, false)]
        [FriendlyName("Pole liczba 2")]
        public decimal? PoleLiczba2 { get; set; }

        [WidoczneListaAdmin(true, false, true, false)]
        [FriendlyName("Pole liczba 3")]
        public decimal? PoleLiczba3 { get; set; }

        [Niewymagane]
        [StringInternuj]
        [WidoczneListaAdmin(false, false, true, false)]
        [FriendlyName("Dostawa")]
        public string Dostawa { get; set; }

        [StringInternuj]
        [FriendlyName("Rodzina")]
        [WidoczneListaAdmin(true, false, true, true)]
        [GrupaAtttribute("Podstawowe informacje", 0)]
        [Niewymagane]
        [Lokalizowane]
        public virtual string Rodzina { get; set; }

        [FriendlyName("Czy mail o nowym produkcie zosta³ wys³any")]
        [GrupaAtttribute("Podstawowe informacje", 0)]
        [WidoczneListaAdmin(true, false, true, false)]
        public bool WyslanoMailNowyProdukt { get; set; }

        [FriendlyName("Posiada ojca")]
        [WidoczneListaAdmin(true, false, false, false)]
        public bool Ojciec { get; set; }

        [Niewymagane]
        [StringInternuj]
        [FriendlyName("Adres www")]
        [WidoczneListaAdmin(true, false, true, false)]
        public string Www { get; set; }

       
        [FriendlyName("Waga brutto")]
        [WidoczneListaAdmin(true, false, true, false)]
        public decimal? Waga { get; set; }

        
        [FriendlyName("Pole liczba 4")]
        public decimal? PoleLiczba4 { get; set; }

        
        [FriendlyName("Pole liczba 5")]
        public decimal? PoleLiczba5 { get; set; }

        
        [FriendlyName("Kolumna liczba 1")]
        public int? KolumnaLiczba1 { get; set; }

        
        [FriendlyName("Kolumna liczba 2")]
        public int? KolumnaLiczba2 { get; set; }

        
        [FriendlyName("Kolumna liczba 3")]
        public int? KolumnaLiczba3 { get; set; }

        
        [FriendlyName("Kolumna liczba 4")]
        public int? KolumnaLiczba4 { get; set; }

        
        [FriendlyName("Kolumna liczba 5")]
        public int? KolumnaLiczba5 { get; set; }

        [StringInternuj]
        [FriendlyName("Kolumna tekst 1")]
        [MaksymalnaLiczbaZnakow(200)]
        [Lokalizowane]
        public string KolumnaTekst1 { get; set; }

        [StringInternuj]
        [FriendlyName("Kolumna tekst 2")]
        [MaksymalnaLiczbaZnakow(200)]
        [Lokalizowane]
        public string KolumnaTekst2 { get; set; }

        [StringInternuj]
        [FriendlyName("Kolumna tekst 3")]
        [MaksymalnaLiczbaZnakow(200)]
        [Lokalizowane]
        public string KolumnaTekst3 { get; set; }

        [StringInternuj]
        [FriendlyName("Kolumna tekst 4")]
        [MaksymalnaLiczbaZnakow(200)]
        [Lokalizowane]
        public string KolumnaTekst4 { get; set; }

        [StringInternuj]
        [FriendlyName("Kolumna tekst 5")]
        [MaksymalnaLiczbaZnakow(200)]
        [Lokalizowane]
        public string KolumnaTekst5 { get; set; }

        [FriendlyName("Minimum logistyczne")]
        [WidoczneListaAdmin(false, false, true, false)]
        public decimal IloscMinimalna { get; set; }

        public int? PrzedstawicielId { get; set; }

        [Niewymagane]
        [StringInternuj]
        [FriendlyName("Komunikat popup")]
        [WidoczneListaAdmin(true, false, true, false)]
        public string PopupKomunikat { get; set; }

        [StringInternuj]
        public string PopupTekst { get; set; }

        [GrupaAtttribute("Podstawowe informacje", 0)]
        [WidoczneListaAdmin(true, false, true, false)]
        [FriendlyName("Wymóg zakupu pe³nego opakowania zbiorczego")]
        public bool WymaganeOz { get; set; }

        [FriendlyName("Typ produktu")]
        [WidoczneListaAdmin(true, false, false, false)]
        public TypProduktu Typ { get; set; }
        
        //[Ignore]
        //[GrupaAtttribute("Podstawowe informacje", 0)]
        //[WidoczneListaAdmin(false, false, true, false)]
        //[WymuszonyTypEdytora(TypEdytora.PoleZdjecie)]
        //public int? ObrazekId { get; set; }

        [FriendlyName("Data dodania produktu na b2b")]
        [WidoczneListaAdmin(true, false, false, false)]
        public DateTime DataDodania { get; set; }

        [FriendlyName("Status ukryty dla produktu")]
        [WidoczneListaAdmin(true, false, false, false)]
        public int? StatusUkryty { get; set; }
     
        [FriendlyName("Odwrotne oci¹¿enie vat")]
        [WidoczneListaAdmin(true, false, true, false)]
        public bool VatOdwrotneObciazenie { get; set; }

        [FriendlyName("Objêtoœæ pozycji w m^3")]
        [WidoczneListaAdmin(true, false, true, false)]
        public decimal? Objetosc { get; set; }

        [FriendlyName("Numer Id menagera")]
        public long? MenagerId { get; set; }

        [FriendlyName("Widocznoœæ przedmiotu")]
        [WidoczneListaAdmin(true, false, false, false)]
        public AccesLevel? Widocznosc { get; set; }

        [FriendlyName("Przedmiot nie podlega rabatowi")]
        [WidoczneListaAdmin(true, false, true, false)]
        public bool NiePodlegaRabatowaniu { get; set; }

        [Niewymagane]
        [StringInternuj]
        [FriendlyName("Meta opis przedmiotu")]
        [WidoczneListaAdmin(true, false, true, false)]
        [Lokalizowane]
        public string MetaOpis { get; set; }

        [Niewymagane]
        [StringInternuj]
        [FriendlyName("Meta s³owa kluczowa dla przedmiotu")]
        [WidoczneListaAdmin(true, false, true, false)]
        [Lokalizowane]
        public string MetaSlowaKluczowe { get; set; }

        //Jest to lista danych dla produktów wirtualnych. kazdy element listy bedzie odpowiada³ jednemu produktowi wirtualnemu
        public List<Dictionary<object, object>> DaneDlaProduktowWirtualnych { get; set; }

        public Produkt()
        {
            Widoczny = true;
            IloscWOpakowaniu = 1;
            Vat = 23;
            Typ = TypProduktu.Produkt;
            DataDodania = DateTime.Now;
            StatusUkryty = 1;
            Widocznosc = AccesLevel.Wszyscy;
            WymaganaKoncesja = null;
            Abstrakcyjny = false;
        }

        public Produkt(Produkt baza) 
        {
            if (baza == null) return;
            Id = baza.Id;
            Abstrakcyjny = baza.Abstrakcyjny;
            Nazwa = baza.Nazwa;
            Kod = baza.Kod;
            Widoczny = baza.Widoczny;
            StanMin = baza.StanMin;
            IloscWOpakowaniu = baza.IloscWOpakowaniu;
            KodKreskowy = baza.KodKreskowy;
            Vat = baza.Vat;
            PKWiU = baza.PKWiU;
            Opis = baza.Opis;
            OpisKrotki = baza.OpisKrotki;
            Opis2 = baza.Opis2;
            OpisKrotki2 = baza.OpisKrotki2;
            Opis3 = baza.Opis3;
            OpisKrotki3 = baza.OpisKrotki3;
            Opis4 = baza.Opis4;
            OpisKrotki4 = baza.OpisKrotki4;
            Opis5 = baza.Opis5;
            OpisKrotki5 = baza.OpisKrotki5;
            PoleTekst1 = baza.PoleTekst1;
            PoleTekst2 = baza.PoleTekst2;
            PoleTekst3 = baza.PoleTekst3;
            PoleLiczba1 = baza.PoleLiczba1;
            PoleLiczba2 = baza.PoleLiczba2;
            PoleLiczba3 = baza.PoleLiczba3;
            Dostawa = baza.Dostawa;
            Rodzina = baza.Rodzina;
            Ojciec = baza.Ojciec;
            Www = baza.Www;
            Waga = baza.Waga;
            PoleTekst4 = baza.PoleTekst4;
            PoleTekst5 = baza.PoleTekst5;
            PoleLiczba4 = baza.PoleLiczba4;
            PoleLiczba5 = baza.PoleLiczba5;
            KolumnaLiczba1 = baza.KolumnaLiczba1;
            KolumnaLiczba2 = baza.KolumnaLiczba2;
            KolumnaLiczba3 = baza.KolumnaLiczba3;
            KolumnaLiczba4 = baza.KolumnaLiczba4;
            KolumnaLiczba5 = baza.KolumnaLiczba5;
            KolumnaTekst1 = baza.KolumnaTekst1;
            KolumnaTekst2 = baza.KolumnaTekst2;
            KolumnaTekst3 = baza.KolumnaTekst3;
            KolumnaTekst4 = baza.KolumnaTekst4;
            KolumnaTekst5 = baza.KolumnaTekst5;
            IloscMinimalna = baza.IloscMinimalna;
            PrzedstawicielId = baza.PrzedstawicielId;
            PopupKomunikat = baza.PopupKomunikat;
            WymaganeOz = baza.WymaganeOz;
            Typ = baza.Typ;
            DataDodania = baza.DataDodania;
            StatusUkryty = baza.StatusUkryty;
            VatOdwrotneObciazenie = baza.VatOdwrotneObciazenie;
            Objetosc = baza.Objetosc;
            MenagerId = baza.MenagerId;
            Widocznosc = baza.Widocznosc;
            NiePodlegaRabatowaniu = baza.NiePodlegaRabatowaniu;
            MetaOpis = baza.MetaOpis;
            MetaSlowaKluczowe = baza.MetaSlowaKluczowe;
            WyslanoMailNowyProdukt = baza.WyslanoMailNowyProdukt;
            CenaWPunktach = baza.CenaWPunktach;

           // OpJednostkoweWaga = baza.OpJednostkoweWaga;
            OpJednostkoweGlebokosc = baza.OpJednostkoweGlebokosc;
          //  OpJednostkoweObjetosc = baza.OpJednostkoweObjetosc;
            OpJednostkoweSzerokosc = baza.OpJednostkoweSzerokosc;
            OpJednostkoweWysokosc = baza.OpJednostkoweWysokosc;

            OpZbiorczeWaga = baza.OpZbiorczeWaga;
            OpZbiorczeIloscWOpakowaniu = baza.OpZbiorczeIloscWOpakowaniu;
            OpZbiorczeGlebokosc = baza.OpZbiorczeGlebokosc;
            OpZbiorczeObjetosc = baza.OpZbiorczeObjetosc;
            OpZbiorczeSzerokosc = baza.OpZbiorczeSzerokosc;
            OpZbiorczeWysokosc = baza.OpZbiorczeWysokosc;

            OpPaletaWaga = baza.OpPaletaWaga;
            OpPaletaGlebokosc = baza.OpPaletaGlebokosc;
            OpPaletaObjetosc = baza.OpPaletaObjetosc;
            OpPaletaSzerokosc = baza.OpPaletaSzerokosc;
            OpPaletaWysokosc = baza.OpPaletaWysokosc;
            OpPaletaIloscWOpakowaniu = baza.OpPaletaIloscWOpakowaniu;
            OpPaletaIloscNaWarstwie = baza.OpPaletaIloscNaWarstwie;
            WymaganaKoncesja = baza.WymaganaKoncesja;
            DaneDlaProduktowWirtualnych = baza.DaneDlaProduktowWirtualnych;
        }

        public Produkt(int id) : this()
        {
            Id = id;

        }

        /// <summary>
        /// wykorzystywany tylko przez modu³y dodatkowe do ustawienia widocznoœci produktów
        /// </summary>
        /// <param name="widocznosc"></param>
        /// <param name="status"></param>
        public void UstawWidocznoscProduktu(bool widocznosc, int? status = null)
        {
            if (!status.HasValue)
            {
                Widoczny = widocznosc;
                StatusUkryty = Convert.ToInt32(Widoczny);
            }

            else
            {

                if (widocznosc && !Widoczny)
                {
                    if (Math.Abs(StatusUkryty.Value) == Math.Abs(status.Value) ||
                        (StatusUkryty.Value == 1 || StatusUkryty.Value == 0))
                    {
                        Widoczny = widocznosc;
                    }
                }
                else if (!widocznosc && Widoczny)
                {
                    if (StatusUkryty.Value > status)
                    {
                        if (status > 0)
                            status = -status;

                        StatusUkryty = status;
                        Widoczny = widocznosc;
                    }
                }
                else if (widocznosc == Widoczny && status.HasValue)
                {
                    if (StatusUkryty == 1)
                    {
                        StatusUkryty = status;
                    }
                    else if (StatusUkryty == 0)
                    {
                        StatusUkryty = -status;
                    }
                }
            }
        }

        [WidoczneListaAdmin(true, false, true, true)]
        [FriendlyName("Cena w punktach")]
        public decimal CenaWPunktach { get; set; }

        [WidoczneListaAdmin(true, false, true, true)]
        [FriendlyName("G³êbokoœæ opakowania jednostkowego (cm)")]
        public decimal? OpJednostkoweGlebokosc { get; set; }

        [WidoczneListaAdmin(true, false, true, true)]
        [FriendlyName("Szerokoœæ opakowania jednostkowego (cm)")]
        public decimal? OpJednostkoweSzerokosc { get; set; }

        [WidoczneListaAdmin(true, false, true, true)]
        [FriendlyName("Wysokoœæ opakowania jednostkowego (cm)")]
        public decimal? OpJednostkoweWysokosc { get; set; }

        [WidoczneListaAdmin(true, false, true, true)]
        [FriendlyName("G³êbokoœæ opakowania zbiorczego (cm)")]
        public decimal? OpZbiorczeGlebokosc { get; set; }

        [WidoczneListaAdmin(true, false, true, true)]
        [FriendlyName("Szerokoœæ opakowania zbiorczego (cm)")]
        public decimal? OpZbiorczeSzerokosc { get; set; }

        [WidoczneListaAdmin(true, false, true, true)]
        [FriendlyName("Wysokoœæ opakowania zbiorczego (cm)")]
        public decimal? OpZbiorczeWysokosc { get; set; }

        //OBJETOSC bêdzie wyliczana z wymiarów - zmienia zgodnie z taskiem 9139 
        [WidoczneListaAdmin(true, false, true, true)]
        [FriendlyName("Objetosc opakowania zbiorczego")]
        public decimal? OpZbiorczeObjetosc { get; set; }

        [WidoczneListaAdmin(true, false, true, true)]
        [FriendlyName("Waga opakowania zbiorczego (kg)")]
        public decimal? OpZbiorczeWaga { get; set; }

        [WidoczneListaAdmin(true, false, true, true)]
        [FriendlyName("Iloœæ w opakowaniu zbiorczym")]
        public decimal? OpZbiorczeIloscWOpakowaniu { get; set; }

        [WidoczneListaAdmin(true, false, true, true)]
        [FriendlyName("G³êbokoœæ palety (cm)")]
        public decimal? OpPaletaGlebokosc { get; set; }

        [WidoczneListaAdmin(true, false, true, true)]
        [FriendlyName("Szerokoœæ palety (cm)")]
        public decimal? OpPaletaSzerokosc { get; set; }

        [WidoczneListaAdmin(true, false, true, true)]
        [FriendlyName("Wysokoœæ palety (cm)")]
        public decimal? OpPaletaWysokosc { get; set; }

        //OBJETOSC bêdzie wyliczana z wymiarów - zmienia zgodnie z taskiem 9139 
        [WidoczneListaAdmin(true, false, true, true)]
        [FriendlyName("Objetosc palety")]
        public decimal? OpPaletaObjetosc { get; set; }

        [WidoczneListaAdmin(true, false, true, true)]
        [FriendlyName("Waga palety (kg)")]
        public decimal? OpPaletaWaga { get; set; }

        [WidoczneListaAdmin(true, false, true, true)]
        [FriendlyName("Iloœæ sztuk na palecie")]
        public decimal? OpPaletaIloscWOpakowaniu { get; set; }

        [WidoczneListaAdmin(true, false, true, true)]
        [FriendlyName("Liczba sztuk na warstwie")]
        public decimal? OpPaletaIloscNaWarstwie { get; set; }

        [WidoczneListaAdmin(true, false, true, true)]
        [FriendlyName("Wymagane koncesje")]
        [Niewymagane]
        public HashSet<long> WymaganaKoncesja { get; set; }
        public bool RecznieDodany()
        {
            return Id < 0;
        }
    }
}
