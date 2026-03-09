using ServiceStack.Common;
using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SolEx.Hurt.Core.BLL.RegulyKoszyka
{
    [FriendlyName("Łączna wartość koszyka", FriendlyOpis = "NIE MOŻNA UŻYWA WARUNKU GDY PRODUKTY MAJĄ WŁACZONĄ GRADACJE. <br/>Wartość koszyka musi znajdować się przedziale <br />Uwaga! Wykluczanie ma pierwszeństwo nad uwzględnianiem")]
    public class LacznaWartoscKoszyka : RegulaKoszyka, IRegulaCalegoKoszyka, IRegulaPozycji, ITestowalna
    {
        public LacznaWartoscKoszyka()
        {
            SposobUwzgledniania = SposobUwzgledniania.Wszystkie;
        }

        public override string Opis => "NIE MOŻNA UŻYWA WARUNKU GDY PRODUKTY MAJĄ WŁACZONĄ GRADACJE";

       
        [FriendlyName("Jakie pozycje uwzględniać")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public SposobUwzgledniania SposobUwzgledniania { get; set; }

        [FriendlyName("Wartość minimalna koszyka - Jeśli pole niewypełnione to brak wartości minimalnej koszyka")]
        [Niewymagane]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public decimal? WartoscMinimalna { get; set; }

        [Niewymagane]
        [FriendlyName("Wartość maksymalna koszyka - Jeśli pole niewypełnione to brak wartości maksymalnej koszyka")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public decimal? WartoscMaksymalna { get; set; }

        [FriendlyName("Nie - wartość liczona wg cen nettto, Tak - wg cen brutto")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public bool CzyBrutto { get; set; }

        [Niewymagane]
        [Obsolete("Pole wycofane. Zaleca się użycie pola Symbole")]
        [FriendlyName("Symbole cechy oddzielone  ;. Jeśli wypełnione to uwzględniamy tylko produkty mające którą którąkolwiek z wpisanych cech")]
        [WidoczneListaAdminAttribute(false, false, false, false)]
        public string SymboleCech { get; set; }

        [Niewymagane]
        [FriendlyName("Symbole cech",FriendlyOpis = "Uwzględniać tylko produkty mające cechy (jeśli wypełnione to uwzględniamy tylko produkty mające którąkolwiek z wpisanych cech)")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        [PobieranieSlownika(typeof(SlownikCech))]
        public List<string> Symbole { get; set; }

        [Niewymagane]
        [FriendlyName("Symbole cech wykluczonych",FriendlyOpis = "Nie uwzględniaj produktów mających cechy (jeśli wypełnione to nie uwzględniamy produktów mających którąkolwiek z wpisanych cech)<br /><b style='color:red'>Uwaga! Wykluczanie ma pierwszeństwo nad uwzględnianiem.</b>")]
        [PobieranieSlownika(typeof(SlownikCech))]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public List<string> SymboleWykluczone { get; set; }

        public List<string> TestPoprawnosci()
        {
            List<string> listaBledow = Przedzial.Sprawdzenie(WartoscMinimalna, WartoscMaksymalna);
            if (Symbole == null)
            {
                foreach (var symbol in Cechy)
                {
                    CechyBll cechaa = SolexBllCalosc.PobierzInstancje.CechyAtrybuty.PobierzCecheOSymbolu(symbol, SolexBllCalosc.PobierzInstancje.Konfiguracja.JezykIDPolski);
                    if (cechaa == null)
                    {
                        listaBledow.Add($"Brak cechy o symbolu: {symbol}");
                    }
                }
            }
            return listaBledow;
        }

        public bool KoszykSpelniaRegule(IKoszykiBLL koszyk)
        {
            decimal wartosc = WyliczWartosc(koszyk);

            bool dolnylimit = !WartoscMinimalna.HasValue || wartosc >= WartoscMinimalna.Value;
            bool gornylimic = !WartoscMaksymalna.HasValue || wartosc < WartoscMaksymalna.Value;

            return dolnylimit && gornylimic;
        }

        public decimal WyliczWartosc(IKoszykiBLL koszyk)
        {
            decimal wartosc = 0;
            switch (SposobUwzgledniania)
            {
                case SposobUwzgledniania.Wszystkie:
                    wartosc = CalkowitaWartoscKoszyka(koszyk);
                    break;

                case SposobUwzgledniania.TylkoDostepne:
                    wartosc = WyliczWartoscKoszykTylkoDostepnychPozycji(koszyk);
                    break;

                case SposobUwzgledniania.TylkoNiedostepne:
                    wartosc = CalkowitaWartoscKoszyka(koszyk) - WyliczWartoscKoszykTylkoDostepnychPozycji(koszyk); //niedostepne to calkowita wartosc - wartosc dostepnych
                    break;

                case SposobUwzgledniania.ProduktyZeStanemWiekszymOdZera:
                    wartosc = WyliczWartoscKoszykPozycjeStanWiekszyOdZera(koszyk);
                    break;
            }
            return wartosc;
        }

        private decimal WyliczWartoscKoszykPozycjeStanWiekszyOdZera(IKoszykiBLL koszyk)
        {
            decimal wynik = 0;
            foreach (var pozycja in koszyk.PobierzPozycje.Where(Pasuje))
            {
                if (pozycja.Produkt.IloscLaczna > 0)
                {
                    wynik += CzyBrutto ? pozycja.WartoscBrutto : pozycja.WartoscNetto;
                }
            }
            return wynik;
        }

        private decimal CalkowitaWartoscKoszyka(IKoszykiBLL koszyk)
        {
            decimal wynik = 0;
            foreach (var pozycja in koszyk.PobierzPozycje.Where(Pasuje))
            { 
                wynik += CzyBrutto ? pozycja.WartoscBrutto : pozycja.WartoscNetto;
            }
            return wynik;
        }

        private List<string> _cechy;

        private List<string> Cechy
        {
            get
            {
                if (_cechy == null)
                {
                    _cechy = new List<string>();
                    if (Symbole != null && (Symbole.Any() && Symbole[0] != string.Empty))
                    {
                        HashSet<long> idCech = new HashSet<long>( Symbole.Select(long.Parse) );
                        _cechy = SolexBllCalosc.PobierzInstancje.CechyAtrybuty.PobierzWszystkieCechy(SolexBllCalosc.PobierzInstancje.Konfiguracja.JezykIDPolski).Where(x => idCech.Contains(x.Key)).Select(y => y.Value.Symbol).ToList();
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(SymboleCech))
                        {
                            string[] cechy = SymboleCech.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                            _cechy.AddRange(cechy);
                        }
                    }
                }
                return _cechy;
            }
        }

        private List<string> _cechyWykluczone;

        private List<string> CechyWykluczone
        {
            get
            {
                if (_cechyWykluczone == null)
                {
                    _cechyWykluczone = new List<string>();
                    if (SymboleWykluczone != null && (SymboleWykluczone.Any() && SymboleWykluczone[0] != string.Empty))
                    {
                        HashSet<long> idCech = new HashSet<long>( SymboleWykluczone.Select(long.Parse) );
                        _cechyWykluczone = SolexBllCalosc.PobierzInstancje.CechyAtrybuty.PobierzWszystkieCechy(SolexBllCalosc.PobierzInstancje.Konfiguracja.JezykIDPolski).Where(x => idCech.Contains(x.Key)).Select(y => y.Value.Symbol).ToList();
                    }
                }
                return _cechyWykluczone;
            }
        }

        private bool Pasuje(IKoszykPozycja pozycja)
        {
            if (pozycja.TypPozycji == TypPozycjiKoszyka.ZaPunkty || pozycja.TypPozycji == TypPozycjiKoszyka.Gratis)
            {
                return false;
            }
            if (!CechyWykluczone.Any() && !Cechy.Any())
            {
                return true;
            }
            if (CechyWykluczone.Any(c => pozycja.Produkt.Cechy.Any(a => a.Value.Symbol.Equals(c, StringComparison.InvariantCultureIgnoreCase))))
            {
                return false;
            }
            return !Cechy.Any() || Cechy.Any(c => pozycja.Produkt.Cechy.Any(a => a.Value.Symbol.Equals(c, StringComparison.InvariantCultureIgnoreCase)));
        }

        private decimal WyliczWartoscKoszykTylkoDostepnychPozycji(IKoszykiBLL koszyk)
        {
            decimal wynik = 0;
            foreach (var pozycja in koszyk.PobierzPozycje.Where(Pasuje))
            {
                decimal ilosc = pozycja.IloscWJednostcePodstawowej > pozycja.Produkt.IloscLaczna ? pozycja.Produkt.IloscLaczna : pozycja.IloscWJednostcePodstawowej;
                decimal wartoscdostwepnych = ilosc * (CzyBrutto ? pozycja.CenaBruttoPodstawowa() : pozycja.CenaNettoPodstawowa);
                wynik += wartoscdostwepnych;
            }
            return wynik;
        }

        public bool PozycjaSpelniaRegule(IKoszykPozycja pozycja, IKoszykiBLL koszyk)
        {
            return KoszykSpelniaRegule(koszyk);
        }
    }
}