using ServiceStack.DataAnnotations;
using ServiceStack.DesignPatterns.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Model
{
    /// <summary>
    /// Model atrybutu zawierającego kolekcję cech
    /// </summary>
    [TworzDynamicznieTabele]
    public class Atrybut : IHasIntId, IPolaIDentyfikujaceRecznieDodanyObiekt, IPoleJezyk
    {
        public Atrybut()
        {
            this.UkryjJednaWartosc = true;
            PobierajCechy = true;
            MetkaPozycjaSzczegoly = MetkaPozycjaSzczegoly.PodNazwaLinia1;
            MetkaPozycjaLista = MetkaPozycjaLista.PodNazwa;
            MetkaPozycjaRodziny = MetkaPozycjaRodziny.PodNazwa;
            MetkaPozycjaSzczegolyWarianty = MetkaPozycjaSzczegolyWarianty.PodNazwa;
            MetkaPozycjaKoszykProdukty = MetkaPozycjaKoszykProdukty.PodNazwa;
            MetkaPozycjaKoszykAutomatyczne = MetkaPozycjaKoszykAutomatyczne.PodNazwa;
            MetkaPozycjaKoszykGratisy = MetkaPozycjaKoszykGratisy.PodNazwa;
            MetkaPozycjaKoszykGratisyPopUp = MetkaPozycjaKoszykGratisyPopUp.PodNazwa;
            PokazujNazweAtrybutuJakoNaglowekFiltra = false;

            MetkaPozycjaKafle = MetkaPozycjaKafle.PodNazwa;

            //domyslnie NIE chcemy pokazywać
            Widoczny = PokazujNaLiscieProduktow = PokazujWWyszukiwaniu = PokazujWPlikachIntegracji = false;
        }
        public bool RecznieDodany()
        {
            return Id < 0;
        }
        public Atrybut(string nazwa,int id=0):this()
        {
            Id = id;
            Nazwa = nazwa;
           // Widoczny = PokazujNaLiscieProduktow = PokazujWWyszukiwaniu = PokazujWPlikachIntegracji = false;
        }

        public Atrybut(Atrybut c)
        {
            if (c == null)
            {
                return;
            }
            Id = c.Id;
            Nazwa = c.Nazwa;
            Widoczny = c.Widoczny;
            Kolejnosc = c.Kolejnosc;
            ProviderWyswietlania = c.ProviderWyswietlania;
            Symbol = c.Symbol;
            ZawszeWszystkieCechy = c.ZawszeWszystkieCechy;
            NazwaOpisowa = c.NazwaOpisowa;
            PokazujWWyszukiwaniu = c.PokazujWWyszukiwaniu;
            UkryjJednaWartosc = c.UkryjJednaWartosc;
            PokazujNaLiscieProduktow = c.PokazujNaLiscieProduktow;
            CechyPokazujKatalog = c.CechyPokazujKatalog;
            PokazujOpisMetki = c.PokazujOpisMetki;
            PobierajCechy = c.PobierajCechy;
            Szerokosc = c.Szerokosc;
            UniwersalnaMetkaOpis = c.UniwersalnaMetkaOpis;
            UniwersalnaMetkaKatalog = c.UniwersalnaMetkaKatalog;
            MetkaPozycjaSzczegoly = c.MetkaPozycjaSzczegoly;
            MetkaPozycjaLista = c.MetkaPozycjaLista;
            MetkaPozycjaRodziny = c.MetkaPozycjaRodziny;
            MetkaPozycjaSzczegolyWarianty = c.MetkaPozycjaSzczegolyWarianty;
            MetkaPozycjaKoszykProdukty = c.MetkaPozycjaKoszykProdukty;
            MetkaPozycjaKoszykAutomatyczne = c.MetkaPozycjaKoszykAutomatyczne;
            MetkaPozycjaKoszykGratisy = c.MetkaPozycjaKoszykGratisy;
            MetkaPozycjaKoszykGratisyPopUp = c.MetkaPozycjaKoszykGratisyPopUp;
            PokazujNazweAtrybutuJakoNaglowekFiltra = c.PokazujNazweAtrybutuJakoNaglowekFiltra;
            MetkaPozycjaKafle = c.MetkaPozycjaKafle;
            JezykId = c.JezykId;
            this.PokazujWPlikachIntegracji = c.PokazujWPlikachIntegracji;
        }

        /// <summary>
        /// ID atrybutu
        /// </summary>
        [UpdateColumnKey]
        [PrimaryKey]
        [WidoczneListaAdmin(true, true, false, false)]
        [GrupaAtttribute("Dane podstawowe", 0)]
        public int Id { get; set; }
        
        /// <summary>
        /// Nazwa atrybutu
        /// </summary>
        [WidoczneListaAdmin(true, true, true, true)]
        [GrupaAtttribute("Dane podstawowe", 0)]
        [Lokalizowane]
        public string Nazwa { get; set; }
        
        /// <summary>
        /// Kolejność pokazywania atrybutuów
        /// </summary>
        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Kolejność")]
        [GrupaAtttribute("Dane podstawowe", 0)]
        public int Kolejnosc { get; set; }
        
        /// <summary>
        /// Czy atrybut jest gdziekolwiek widoczny
        /// </summary>
        [WidoczneListaAdmin(true, true, true, true)]
        [GrupaAtttribute("Dane podstawowe", 0)]
        public bool Widoczny { get; set; }

        [WidoczneListaAdmin(true, false, true, true)]
        [FriendlyName("Szerokość")]
        [GrupaAtttribute("Dane podstawowe", 0)]
        [Niewymagane]
        public string Szerokosc { get; set; }
        
        /// <summary>
        /// W jaki spoób ma być renderowany atrybut
        /// </summary>
        [WidoczneListaAdmin(true, false, true, true)]
        [PobieranieSlownika("SolEx.Hurt.Core.Helper.SlownikAtrybutSposobyPokazywania,SolEx.Hurt.Core")]
        [FriendlyName("Sposób pokazywania")]
        [GrupaAtttribute("Dane podstawowe", 0)]
        [WymuszonyTypEdytora(TypEdytora.PoleDropDown)]
        public string ProviderWyswietlania { get; set; }
        
        /// <summary>
        /// Symbol atrybutu
        /// </summary>
        [WidoczneListaAdmin(true, true, false, false)]
        [GrupaAtttribute("Dane podstawowe", 0)]
        [MaksymalnaLiczbaZnakow(100)]
        public string Symbol { get; set; }
       
        /// <summary>
        /// Czy dany atrybut ma mieć zawsze wszystkie cechy pokazywane
        /// </summary>
        [FriendlyName("Czy dany atrybut ma mieć zawsze wszystkie cechy pokazywane", FriendlyOpis = "Zaznaczenie tej opcji spowoduje wyświetlenie zawsze wszystkich cech dla atrybutu")]
        [WidoczneListaAdmin(false, false, true, true)]
        [GrupaAtttribute("Dane podstawowe", 0)]
        public bool ZawszeWszystkieCechy { get; set; }
        
        /// <summary>
        /// Ładna nazwa atrybutu
        /// </summary>
        [FriendlyName("Ładna nazwa atrybutu")]
        [WidoczneListaAdmin(true, true, true, true)]
        [GrupaAtttribute("Metki", 2)]
        [Niewymagane]
        [Lokalizowane]
        public string NazwaOpisowa { get; set; }   
        
        /// <summary>
        /// Czy pokazywać atrybut przy wyszukiwaniu
        /// </summary>
        [WidoczneListaAdmin(true, false, true, false)]
        [FriendlyName("Czy pokazywać w wyszukiwaniu")]
        [GrupaAtttribute("Filtry", 0)]
        public bool PokazujWWyszukiwaniu { get; set; }


        /// <summary>
        /// Czy pokazywać nazwę atrybutu jako nagłówek (opis) filtra
        /// </summary>
        [WidoczneListaAdmin(true, false, true, true)]
        [FriendlyName("Czy pokazywać nazwę atrybutu jako nagłówek (opis) filtra")]
        [GrupaAtttribute("Filtry", 0)]
        public bool PokazujNazweAtrybutuJakoNaglowekFiltra { get; set; }
        
        /// <summary>
        /// Czy ukryć atrybut jeśli ma tylko jedną cechę
        /// </summary>
        [WidoczneListaAdmin(true, false, true, false)]
        [FriendlyName("Czy ukryć atrybut jeśli ma tylko jedną cechę")]
        [GrupaAtttribute("Filtry", 0)]
        public bool UkryjJednaWartosc { get; set; }
        
        /// <summary>
        /// Czy pokazywać atrybut na liście produktów z kategorii
        /// </summary>
        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Pokazywać na liście produktów")]
        [GrupaAtttribute("Filtry", 0)]
        public bool PokazujNaLiscieProduktow { get; set; }

        /// <summary>
        /// Czy pokazywać atrybut na liście produktów z kategorii
        /// </summary>
        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Pokazywać w plikach integracji")]
        [GrupaAtttribute("Filtry", 0)]
        public bool PokazujWPlikachIntegracji { get; set; }

        /// <summary>
        /// Czy pokazywać dany atrybut w katalogu
        /// </summary>
        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Pokazywać dany filtr w katalogu bez logowania")]
        [GrupaAtttribute("Filtry", 0)]
        public bool CechyPokazujKatalog { get; set; }

        /// <summary>
        /// Pokazuj nazwę atrybutu jako nazwę metki
        /// </summary>
        [WidoczneListaAdmin(false, false, true, false)]
        [FriendlyName("Czy pokazywać ładną nazwę atrybutu jako nazwę metki", FriendlyOpis = "Zaznaczenie tej opcji spowoduje wyświetlenie wartości z pola 'Ładna nazwa atrybutu' jako początek metki. <br/> W wyniku otrzymamy metkę ŁadnaNazwa:NazwaMetki. Nazwa metki pobierana jest z cechy: dla zalogowanych z pola Metka, dla niezalogowanych z pola Metka katalog")]
        [GrupaAtttribute("Metki", 2)]
        public bool PokazujOpisMetki { get; set; }

        /// <summary>
        /// Czy importować z ERP cechy dla tego atrybutu (zaznacz aby nie pobierać te cechy które nie są potrzebne na B2B)	
        /// </summary>
        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Czy importować z ERP cechy dla tego atrybutu", FriendlyOpis = "Zaznacz tą opcję aby nie pobierać cech dla atrybutu na B2B")]
        [GrupaAtttribute("Dane podstawowe", 0)]
        public bool PobierajCechy { get; set; }

        /// <summary>
        /// Metka dodawana do wszystkich cech o tym atrybucie - zalogowani
        /// </summary>
        [WidoczneListaAdmin(false, false, true, true)]
        [Niewymagane]
        [GrupaAtttribute("Metki", 2)]
        [FriendlyName("Metka dodawana do wszystkich cech o tym atrybucie",FriendlyOpis = "Metka ta zostanie ustawiona dla wszystkich cech mających dany atrybut. Wpisanie {0} spowoduje wstawienie w to miejsce nazwy cechy")]
        [WymuszonyTypEdytora(TypEdytora.EdytorTekstowy)]
        [Lokalizowane]
        public string UniwersalnaMetkaOpis { get; set; }


        /// <summary>
        /// Metka dodawana do wszystkich cech o tym atrybucie - niezalogowani
        /// </summary>
        [WidoczneListaAdmin(false, false, true, true)]
        [Niewymagane]
        [GrupaAtttribute("Metki", 2)]
        [FriendlyName("Metka katalog dodawana do wszystkich cech o tym atrybucie ", FriendlyOpis = "Metka katalog zostanie ustawiona dla wszystkich cech mających dany atrybut. Wpisanie {0} spowoduje wstawienie w to miejsce nazwy cechy")]
        [WymuszonyTypEdytora(TypEdytora.EdytorTekstowy)]
        [Lokalizowane]
        public string UniwersalnaMetkaKatalog { get; set; }

        /// <summary>
        /// Pozycja metki w koszyku dla pozycji dodanej automatycznie
        /// </summary>
        [WidoczneListaAdmin(false, false, true, true)]
        [GrupaAtttribute("Metki", 2)]
        public MetkaPozycjaKoszykAutomatyczne MetkaPozycjaKoszykAutomatyczne { get; set; }


        /// <summary>
        /// Pozycja metki w koszyku dla pozycji dodanej przez klienta
        /// </summary>
        [WidoczneListaAdmin(false, false, true, true)]
        [GrupaAtttribute("Metki", 2)]
        public MetkaPozycjaKoszykProdukty MetkaPozycjaKoszykProdukty { get; set; }

        /// <summary>
        /// Pozycja metki w na karcie produktu
        /// </summary>
        [WidoczneListaAdmin(false, false, true, true)]
        [GrupaAtttribute("Metki", 2)]
        public MetkaPozycjaSzczegoly MetkaPozycjaSzczegoly { get; set; }
        /// <summary>
        /// Pozycja metki na liście produktów
        /// </summary>
        [WidoczneListaAdmin(false, false, true, true)]
        [GrupaAtttribute("Metki", 2)]
        public MetkaPozycjaLista MetkaPozycjaLista { get; set; }
        /// <summary>
        /// Pozycja metki dla produktow rodzinowych
        /// </summary>
        [WidoczneListaAdmin(false, false, true, true)]
        [GrupaAtttribute("Metki", 2)]
        public MetkaPozycjaRodziny MetkaPozycjaRodziny { get; set; }
        /// <summary>
        /// Pozycja metki w koszyku dla pozycji gratisowych
        /// </summary>
        [WidoczneListaAdmin(false, false, true, true)]
        [GrupaAtttribute("Metki", 2)]
        public MetkaPozycjaKoszykGratisy MetkaPozycjaKoszykGratisy { get; set; }
        /// <summary>
        /// Pozycja metki przy wyborze gratisów z popupa
        /// </summary>
        [WidoczneListaAdmin(false, false, true, true)]
        [GrupaAtttribute("Metki", 2)]
        public MetkaPozycjaKoszykGratisyPopUp MetkaPozycjaKoszykGratisyPopUp { get; set; }

        /// <summary>
        /// Pozycja metki na liscie wariantów
        /// </summary>
        [WidoczneListaAdmin(false, false, true, true)]
        [GrupaAtttribute("Metki", 2)]
        public MetkaPozycjaSzczegolyWarianty MetkaPozycjaSzczegolyWarianty { get; set; }

        [WidoczneListaAdmin(false, false, true, true)]
        [GrupaAtttribute("Metki", 2)]
        public MetkaPozycjaKafle MetkaPozycjaKafle { get; set; }

        public override string ToString()
        {
            return string.Format("Id: {0}, Nazwa: {1}",Id,Nazwa);
        }

        [Ignore]
        public int JezykId { get; set; }
    }
}
