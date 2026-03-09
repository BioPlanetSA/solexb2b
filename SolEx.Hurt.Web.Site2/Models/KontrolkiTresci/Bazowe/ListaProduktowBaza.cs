using System.Collections.Generic;
using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Web.Site2.Models.KontrolkiTresci.Bazowe
{
    public abstract class ListaProduktowBaza:KontrolkaTresciBaza
    {
        protected ListaProduktowBaza()
        {
            SzablonRodziny = "ObokSiebie";
            IleDzieciNiepokazujRodzin = 0;
            LCenaDetaliczna = JakieCenyPokazywac.NettoBrutto;
            LCenaPrzedRabatem = JakieCenyPokazywac.NettoBrutto;
            CenaPoRabacie = JakieCenyPokazywac.NettoBrutto;
            LRabat = true;
            LVat = true;
            ZyskKlienta = false;
            BelkaDostepnosci = true;
            LSymbol = true;
            LKodKreskowy = true;
            LGradacja = GradacjaSposobPokazywania.Brak;
            LRozmiarZdjecia = "ico82x82wp";
            LNaglowekListaProduktowPokazuj = true;

            RodzinyPokazujZdjeciaDzieci = true;
            CechyRodzinoweListaProduktowPokazuj = true;

            KVat = MiejscaNaKaflachNaDole.Brak;
            KJM = MiejscaNaKaflachNaDole.ObokKoszyka;
            KOZ = MiejscaNaKaflachNaDole.Brak;
            KRabat = MiejscaNaKaflachNaDole.Brak;
            KSymbol = MiejscaNaKaflach.PodZdjeciem;
            KNazwa = MiejscaNaKaflach.PodZdjeciem;
            KKodKreskowy = MiejscaNaKaflach.PodZdjeciem;
            KCenaPrzedRabatem = JakieCenyPokazywac.Brak;
            KCenaPoRabacie = JakieCenyPokazywac.Netto;
            KRozmiarZdjecia = "ico195x195p";

            PokazujKoszyk = true;
        }
     
        [Niewymagane]
        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Kolumna 20",FriendlyOpis = "Pole z którego będzie pobrana wartość dla kolumna 20")]
        [Lokalizowane]
        [PobieranieSlownika(typeof(SlownikPolProduktow))]
        [GrupaAtttribute("Lista", 3)]
        public string Kolumna20 { get; set; }

        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Cena detaliczna lista produktów",FriendlyOpis = "Jakie ceny pokazywać na liście produktów dla ceny detalicznej")]
        [GrupaAtttribute("Lista", 2)]
        [Niewymagane]
        public JakieCenyPokazywac LCenaDetaliczna { get; set; }

        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Cena przed rabatem lista produktów",FriendlyOpis = "Jakie ceny pokazywać na liście produktów dla ceny przed rabatem")]
        [GrupaAtttribute("Lista", 2)]
        [Niewymagane]
        public JakieCenyPokazywac LCenaPrzedRabatem { get; set; }

        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Rabat lista produktów",FriendlyOpis = "Czy pokazywać rabat na liście produktów")]
        [GrupaAtttribute("Lista", 2)]
        public bool LRabat { get; set; }

        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Vat na liście produktów",FriendlyOpis = "Czy pokazywać vat na liście produktów")]
        [GrupaAtttribute("Lista", 2)]
        public bool LVat { get; set; }


        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Cena po rabacie lista produktów",FriendlyOpis = "Jakie ceny pokazywać na liście produktów dla ceny po rabacie")]
        [GrupaAtttribute("Lista", 2)]
        [Niewymagane]
        public JakieCenyPokazywac CenaPoRabacie { get; set; }

        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Zysk klienta",FriendlyOpis = "Czy pokazywać zysk klienta")]
        [GrupaAtttribute("Lista", 2)]
        public bool ZyskKlienta { get; set; }


        [Niewymagane]
        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Kolumna 80",FriendlyOpis = "Pole z którego będzie pobrana wartość dla kolumna 80")]
        [Lokalizowane]
        [PobieranieSlownika(typeof(SlownikPolProduktow))]
        [GrupaAtttribute("Lista", 3)]
        public string Kolumna80 { get; set; }

        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Czy pokazywać belkę dostępności")]
        [GrupaAtttribute("Lista", 2)]
        public bool BelkaDostepnosci { get; set; }

        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Symbol lista produktów",FriendlyOpis = "Czy pokazywać symbol na liście produktów")]
        [GrupaAtttribute("Lista", 2)]
        public bool LSymbol { get; set; }

        [Niewymagane]
        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Wzorzec formatowania wyświetlnaje frazy w kolumnie 80. ",FriendlyOpis = "Jako fraza do pokazania użyj symbolu: {0} - czyli np. aby pokazać wartość z znakiem % na końcu należy wpisać tutaj '{0}%'. Format może zawierać HTML - czyli dozwolona jest składnia np. '<b>{0}</b>")]
        [GrupaAtttribute("Lista", 2)]
        public string Lkolumna80format { get; set; }

        [Niewymagane]
        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Wzorzec formatowania wyświetlnaje frazy w kolumnie 20",FriendlyOpis = "Jako fraza do pokazania użyj symbolu: {0} - czyli np. aby pokazać wartość z znakiem % na końcu należy wpisać tutaj '{0}%'. Format może zawierać HTML - czyli dozwolona jest składnia np. '<b>{0}</b>")]
        [GrupaAtttribute("Lista", 2)]
        public string LKolumna20Format { get; set; }

        [Niewymagane]
        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Nagłówek nad produktami",FriendlyOpis = "np. nazwa aktualnej kategorii. Jako podstawienie {kategoria} możesz użyć nazwy aktualnej kategorii, {grupa} nazwa aktualnej grupy kategorii")]
        public string NaglowekNadProduktami { get; set; }


        [Niewymagane]
        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Czy pokazywać nagłówek kolumn nad listą produktów",FriendlyOpis = "Nagłowki kolumn np.: nazwa, symbol, itp..")]
        public bool LNaglowekListaProduktowPokazuj { get; set; }

        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Kod kreskowy na liście produktów",FriendlyOpis = "Czy pokazywać kod kreskowy na liście produktów")]
        [GrupaAtttribute("Lista", 2)]
        public bool LKodKreskowy { get; set; }


        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Gradacji na liście produktów",FriendlyOpis = "Sposób pokazywania gradacji na liście produktów")]
        [GrupaAtttribute("Lista", 2)]
        [Niewymagane]
        public GradacjaSposobPokazywania LGradacja { get; set; }

        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Vat - kafle",FriendlyOpis = "Pozycja vatu na kaflach")]
        [GrupaAtttribute("Kafle", 2)]
        public MiejscaNaKaflachNaDole KVat { get; set; }

        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Jednostak miary kafle",FriendlyOpis = "Pozycja jednostki miary produktu na kaflach")]
        [GrupaAtttribute("Kafle", 2)]
        public MiejscaNaKaflachNaDole KJM { get; set; }

        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Opakowania zboiorcze kafle",FriendlyOpis = "Pozycja opakowanie zbiorecze produktu na kaflach")]
        [GrupaAtttribute("Kafle", 2)]
        public MiejscaNaKaflachNaDole KOZ { get; set; }

        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Rabat kafle", FriendlyOpis = "Pozycja rabatu produktu na kaflach")]
        [GrupaAtttribute("Kafle", 2)]
        public  MiejscaNaKaflachNaDole KRabat { get; set; }

        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Symbol produktu kafle",FriendlyOpis = "Pozycja symbolu produktu na kaflach")]
        [GrupaAtttribute("Kafle", 2)]
        public  MiejscaNaKaflach KSymbol { get; set; }

        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Nazwa produktu kafle",FriendlyOpis = "Pozycja nazwy produktu na kaflach")]
        [GrupaAtttribute("Kafle", 2)]
        public MiejscaNaKaflach KNazwa { get; set; }

        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Kod kreskowy produktu kafle",FriendlyOpis = "Pozycja kodu kreskowego produktu na kaflach")]
        [GrupaAtttribute("Kafle", 2)]
        public MiejscaNaKaflach KKodKreskowy { get; set; }

        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Cena przed rabatem produktu kafle",FriendlyOpis = "Jakie ceny przed rabatem pokazywać na kaflach")]
        [GrupaAtttribute("Kafle", 2)]
        public JakieCenyPokazywac KCenaPrzedRabatem { get; set; }

        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Cena po rabacie produktu kafle",FriendlyOpis = "Jakie ceny po rabacie pokazywać na kaflach")]
        [GrupaAtttribute("Kafle", 2)]
        public JakieCenyPokazywac KCenaPoRabacie { get; set; }

        [Niewymagane]
        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Zdjecie domyslny rozmiar",FriendlyOpis = "Domyślny rozmiar zdjęć na kaflach.")]
        [PobieranieSlownika(typeof(SlownikRozmiarZdjec))]
        [GrupaAtttribute("Kafle", 2)]
        public string KRozmiarZdjecia { get; set; }

        [Niewymagane]
        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Zdjecie domyslny rozmiar", FriendlyOpis = "Domyślny rozmiar zdjęć na kaflach.")]
        [PobieranieSlownika(typeof(SlownikRozmiarZdjec))]
        [GrupaAtttribute("Lista", 2)]
        public string LRozmiarZdjecia { get; set; }

        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Czy pokazywać cechy rodzinowe na liście produktów")]
        [GrupaAtttribute("Produkty rodzinowe", 5)]
        public bool CechyRodzinoweListaProduktowPokazuj { get; set; }

        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Czy w pokazywanych rodzinach pokazywac zdjecia miniatury dzieci")]
        [GrupaAtttribute("Produkty rodzinowe", 5)]
        public bool RodzinyPokazujZdjeciaDzieci { get; set; }

        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Czy pokazywać koszyk")]
        public bool PokazujKoszyk { get; set; }

        [Niewymagane]
        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Dodatkowa klasa jaka ma być nadana liście produktów")]
        
        public string KlasaListaProduktow { get; set; }

        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Ilość dzieci w rodzinie",FriendlyOpis = "Nie pokazuj rodzin na liscie jesli jest wiecej dzieci niz wartość ustawienia. Wpisanie 0 oznacza brak limitu")]
        [GrupaAtttribute("Lista", 3)]
        public int IleDzieciNiepokazujRodzin { get; set; }

        /// <summary>
        /// szablon dla rodzin
        /// </summary>
        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Widok rodzinowy",FriendlyOpis = "Widok generowany dla produktów rodzinowych")]
        [PobieranieSlownika(typeof(SlownikWidokowListyProduktowRodzinowych))]
        [GrupaAtttribute("Lista", 2)]
        public string SzablonRodziny { get; set; }
    }
}