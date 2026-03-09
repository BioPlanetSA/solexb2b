using System;
using System.Collections.Generic;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Web;
using System.Linq;
using System.Security.Policy;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core.BLL.Web
{
    public class ParametryKoszyka
    {
        public UstawieniaKoszyka Ustawienia { get; set; }
        public ParametryKoszyka() { }
        public string SzukanaFraza { get; set; }
        public KoszykBll KoszykObiekt { get; set; }
        public IKlient AktualnyKlient { get; set; }
        public ParametrModulu<long, string>[] Adresy { get; set; }

        //todo: wywalic te parametry i zamienic na normalne pola w koszyku
        public SlownikParametrowKoszyka SlownikParametrow { get; set; }
        public Komunikat[] Komunikaty { get; set; }

        public bool CzySaGratisyDoWyboru
        {
            get
            {
                return this.KoszykObiekt.PobierzPozycje.Any(x => x.TypPozycji == TypPozycjiKoszyka.Gratis);
            }
        }

        public bool ZmianaCen
        {
            get
            {
               
                return SolexBllCalosc.PobierzInstancje.Konfiguracja.GetLicense(Licencje.ZmianaCenPrzedstawiciel) && KoszykObiekt.PrzedstawicielKtoryEdytujeKoszykWImieniuKlienta!=null;
            }
        }

        public bool Finalizacja { get; set; }

        public bool Akceptacja { get; set; }

        public ParametrModulu<int, string>[] Dostawy { get; set; }
        public ParametrModulu<int, string>[] Platnosci { get; set; }
        public DodatkowePolaKoszykaPogrupowane[] PolaWlasneKoszyka { get; set; }
        public List<string> DostepneMagazyny { get; set; }
        public ParametrModulu<int, Komunikat>[] ModulyGratisow { get; set; }

        public string TekstButtonaFinalizacji { get; set; }

        public bool WidoczneCeny { get; set; }

        public WidocznoscKolumnKoszyk WidocznoscKolumnProdukty()
        {
            WidocznoscKolumnKoszyk wid = new WidocznoscKolumnKoszyk();
            wid.PokazywacDateDodania = Ustawienia.PokazywacDateDodaniaDoKoszyka;
            wid.PokazywacMetkeRodzinowa = Ustawienia.PokazywacMetkeRodzinowaKoszykProdukty;
            wid.PokazywacZdjecieProduktu = Ustawienia.PokazywacZdjecieProduktuKoszykProdukty;
            wid.PokazywacNazweProduktu = Ustawienia.PokazywacNazweProduktuKoszykProdukty;
            wid.PokazywacSymbolProduktu = Ustawienia.PokazywacSymbolProduktuKoszykProdukty;
            wid.PokazywacKodKreskowy = Ustawienia.PokazywacKodKreskowyProduktuKoszykProdukty;
            wid.CenaJednostowa = SolexBllCalosc.PobierzInstancje.Konfiguracja.CenaPoRabacieListaProduktowPokazuj;
            wid.PokazywacVat = Ustawienia.PokazywacVatKoszykProdukty;
            wid.UkrywacJednoskeMiaryIIlosc = SolexBllCalosc.PobierzInstancje.Konfiguracja.UkryjJednostkiMiary;
            wid.WartoscPozycji = SolexBllCalosc.PobierzInstancje.Konfiguracja.CenaPoRabacieListaProduktowPokazuj; 
            wid.PokazywacWartoscVat = Ustawienia.PokazywacWartoscVatKoszykProdukty;
            wid.CenaHurtowa = Ustawienia.PokazywacCenaKatalogowaProdukty;
            wid.PokazywacWage = Ustawienia.PokazywacWageProdukty;
            wid.FormatPokazywanejWagi = Ustawienia.FormatPokazywanejWagi;
            return wid;
        }

        public WidocznoscKolumnKoszyk WidocznoscKolumnProduktyZaPkt()
        {
            WidocznoscKolumnKoszyk wid = new WidocznoscKolumnKoszyk();
            wid.PokazywacDateDodania = Ustawienia.PokazywacDateDodaniaDoKoszykaProduktyZaPkt;
            wid.PokazywacMetkeRodzinowa = Ustawienia.PokazywacMetkeRodzinowaKoszykProduktyZaPkt;
            wid.PokazywacZdjecieProduktu = Ustawienia.PokazywacZdjecieProduktuKoszykProduktyZaPkt;
            wid.PokazywacNazweProduktu = Ustawienia.PokazywacNazweProduktuKoszykProduktyZaPkt;
            wid.PokazywacSymbolProduktu = Ustawienia.PokazywacSymbolProduktuKoszykProduktyZaPkt;
            wid.PokazywacKodKreskowy = Ustawienia.PokazywacKodKreskowyProduktuKoszykProduktyZaPkt;
            wid.CenaJednostowa = SolexBllCalosc.PobierzInstancje.Konfiguracja.CenaPoRabacieListaProduktowPokazuj;
            wid.PokazywacVat = false;
            wid.UkrywacJednoskeMiaryIIlosc = SolexBllCalosc.PobierzInstancje.Konfiguracja.UkryjJednostkiMiary;
            wid.WartoscPozycji = SolexBllCalosc.PobierzInstancje.Konfiguracja.CenaPoRabacieListaProduktowPokazuj;
            wid.PokazywacWartoscVat = false;
            wid.CenaHurtowa=JakieCenyPokazywac.Brak;
            wid.PokazywacWage = Ustawienia.PokazywacWageProduktyZaPkt;
            wid.FormatPokazywanejWagi = Ustawienia.FormatPokazywanejWagi;
            wid.CenaWPunktach = true;
            return wid;
        }

        public WidocznoscKolumnKoszyk WidocznoscKolumnProduktyGratisy()
        {
            WidocznoscKolumnKoszyk wid = new WidocznoscKolumnKoszyk();
            wid.PokazywacDateDodania = Ustawienia.PokazywacDateDodaniaDoKoszykaProduktyGratisy;
            wid.PokazywacMetkeRodzinowa = Ustawienia.PokazywacMetkeRodzinowaKoszykProduktyGratisy;
            wid.PokazywacZdjecieProduktu = Ustawienia.PokazywacZdjecieProduktuKoszykProduktyGratisy;
            wid.PokazywacNazweProduktu = Ustawienia.PokazywacNazweProduktuKoszykProduktyGratisy;
            wid.PokazywacSymbolProduktu = Ustawienia.PokazywacSymbolProduktuKoszykProduktyGratisy;
            wid.PokazywacKodKreskowy = Ustawienia.PokazywacKodKreskowyProduktuKoszykProduktyGratisy;
            wid.CenaJednostowa = SolexBllCalosc.PobierzInstancje.Konfiguracja.CenaPoRabacieListaProduktowPokazuj;
            wid.PokazywacVat = Ustawienia.PokazywacVatKoszykProduktyGratisy;
            wid.UkrywacJednoskeMiaryIIlosc = SolexBllCalosc.PobierzInstancje.Konfiguracja.UkryjJednostkiMiary;
            wid.WartoscPozycji = SolexBllCalosc.PobierzInstancje.Konfiguracja.CenaPoRabacieListaProduktowPokazuj;
            wid.PokazywacWartoscVat = Ustawienia.PokazywacWartoscVatKoszykProduktyGratisy;
            wid.CenaHurtowa = Ustawienia.PokazywacCenaKatalogowaProduktyGratisy;
            wid.PokazywacWage = Ustawienia.PokazywacWageProduktyGratisy;
            wid.FormatPokazywanejWagi = Ustawienia.FormatPokazywanejWagi;
            return wid;
        }

        public WidocznoscKolumnKoszyk WidocznoscKolumnProduktyAutomatyczne()
        {
            WidocznoscKolumnKoszyk wid = new WidocznoscKolumnKoszyk();
            wid.PokazywacDateDodania = Ustawienia.PokazywacDateDodaniaDoKoszykaProduktyAutomatyczne;
            wid.PokazywacMetkeRodzinowa = Ustawienia.PokazywacMetkeRodzinowaKoszykProduktyAutomatyczne;
            wid.PokazywacZdjecieProduktu = Ustawienia.PokazywacZdjecieProduktuKoszykProduktyAutomatyczne;
            wid.PokazywacNazweProduktu = Ustawienia.PokazywacNazweProduktuKoszykProduktyAutomatyczne;
            wid.PokazywacSymbolProduktu = Ustawienia.PokazywacSymbolProduktuKoszykProduktyAutomatyczne;
            wid.PokazywacKodKreskowy = Ustawienia.PokazywacKodKreskowyProduktuKoszykProduktyAutomatyczne;
            wid.CenaJednostowa = SolexBllCalosc.PobierzInstancje.Konfiguracja.CenaPoRabacieListaProduktowPokazuj;
            wid.PokazywacVat = Ustawienia.PokazywacVatKoszykProduktyGratisy;
            wid.UkrywacJednoskeMiaryIIlosc = SolexBllCalosc.PobierzInstancje.Konfiguracja.UkryjJednostkiMiary;
            wid.WartoscPozycji = SolexBllCalosc.PobierzInstancje.Konfiguracja.CenaPoRabacieListaProduktowPokazuj;
            wid.PokazywacWartoscVat = Ustawienia.PokazywacWartoscVatKoszykProduktyAutomatyczne;
            wid.CenaHurtowa = Ustawienia.PokazywacCenaKatalogowaProduktyAutomatyczne;
            wid.PokazywacWage = Ustawienia.PokazywacWageProduktyAutomatyczne;
            wid.FormatPokazywanejWagi = Ustawienia.FormatPokazywanejWagi;
            return wid;
        }
    }

    public class DodatkowePolaKoszykaPogrupowane
    {
        public PozycjaNaWidokuKoszyka Pozycja { get; set; }
        public DodatkowePoleKoszyka[] Pola { get; set; }
    }

    public class ParametrModulu<T, TVal>
    {
        public T Klucz { get; set; }
        public TVal Wartosc { get; set; }
    }

}