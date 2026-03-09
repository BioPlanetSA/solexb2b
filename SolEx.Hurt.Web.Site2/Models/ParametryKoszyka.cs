using System.Collections.Generic;
using System.Linq;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.BLL.Web;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;
using SolEx.Hurt.Model.Web;
using SolEx.Hurt.Web.Site2.Models.KontrolkiTresci.Koszyk;

namespace SolEx.Hurt.Web.Site2.Models
{
    public class ParametryKoszyka
    {
        public int Id
        {
            get { return _kontrolkaId; }
            set
            {
                _kontrolkaId = value;
                if (_kontrolkaId != 0)
                {
                    KontrolkaKoszyka = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<TrescKolumnaBll>(_kontrolkaId).Kontrolka() as KoszykCalosc;
                }
            }
        }

        private int _kontrolkaId;

        public KoszykCalosc KontrolkaKoszyka { get; private set; }

        public string PoprzedniaStrona { get; set; }

        public ParametryKoszyka() { }

        public ParametryKoszyka(ParametryKoszyka param)
        {
            Id = param.Id;
            KontrolkaKoszyka = param.KontrolkaKoszyka;
        }
        public KoszykBll KoszykObiekt { get; set; }
        public ParametrModulu<long, string>[] Adresy { get; set; }

        //todo: wywalic te parametry i zamienic na normalne pola w koszyku
        public Dictionary<int, DodatkowePoleKoszyka> SlownikParametrow { get; set; }
        public Komunikat[] Komunikaty { get; set; }

        public bool ZmianaCen => SolexBllCalosc.PobierzInstancje.Konfiguracja.GetLicense(Licencje.ZmianaCenPrzedstawiciel) && KoszykObiekt.PrzedstawicielKtoryEdytujeKoszykWImieniuKlienta!=null;

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
            wid.PokazywacDateDodania = KontrolkaKoszyka.PokazywacDateDodaniaDoKoszyka;
            wid.PokazywacMetkeRodzinowa = KontrolkaKoszyka.PokazywacMetkeRodzinowaKoszykProdukty;
            wid.PokazywacZdjecieProduktu = KontrolkaKoszyka.PokazywacZdjecieProduktuKoszykProdukty;
            wid.PokazywacNazweProduktu = KontrolkaKoszyka.PokazywacNazweProduktuKoszykProdukty;
            wid.PokazywacSymbolProduktu = KontrolkaKoszyka.PokazywacSymbolProduktuKoszykProdukty;
            wid.PokazywacKodKreskowy = KontrolkaKoszyka.PokazywacKodKreskowyProduktuKoszykProdukty;
            wid.CenaJednostowa = KontrolkaKoszyka.CenaPoRabaciePokazuj;
            wid.PokazywacVat = KontrolkaKoszyka.PokazywacVatKoszykProdukty;
            wid.UkrywacJednoskeMiaryIIlosc = SolexBllCalosc.PobierzInstancje.Konfiguracja.UkryjJednostkiMiary;
            wid.WartoscPozycji = KontrolkaKoszyka.CenaPoRabaciePokazuj; 
            wid.PokazywacWartoscVat = KontrolkaKoszyka.PokazywacWartoscVatKoszykProdukty;
            wid.CenaHurtowa = KontrolkaKoszyka.PokazywacCenaKatalogowaProdukty;
            wid.PokazywacWage = KontrolkaKoszyka.PokazywacWageProdukty;
            wid.FormatPokazywanejWagi = KontrolkaKoszyka.FormatPokazywanejWagi;
            wid.TypPozycjiKoszyka = TypPozycjiKoszyka.Zwykly;
            return wid;
        }

        public WidocznoscKolumnKoszyk WidocznoscKolumnProduktyZaPkt()
        {
            WidocznoscKolumnKoszyk wid = new WidocznoscKolumnKoszyk();
            wid.PokazywacDateDodania = KontrolkaKoszyka.PokazywacDateDodaniaDoKoszykaProduktyZaPkt;
            wid.PokazywacMetkeRodzinowa = KontrolkaKoszyka.PokazywacMetkeRodzinowaKoszykProduktyZaPkt;
            wid.PokazywacZdjecieProduktu = KontrolkaKoszyka.PokazywacZdjecieProduktuKoszykProduktyZaPkt;
            wid.PokazywacNazweProduktu = KontrolkaKoszyka.PokazywacNazweProduktuKoszykProduktyZaPkt;
            wid.PokazywacSymbolProduktu = KontrolkaKoszyka.PokazywacSymbolProduktuKoszykProduktyZaPkt;
            wid.PokazywacKodKreskowy = KontrolkaKoszyka.PokazywacKodKreskowyProduktuKoszykProduktyZaPkt;
            wid.CenaJednostowa = KontrolkaKoszyka.CenaPoRabaciePokazuj;
            wid.PokazywacVat = false;
            wid.UkrywacJednoskeMiaryIIlosc = SolexBllCalosc.PobierzInstancje.Konfiguracja.UkryjJednostkiMiary;
            wid.WartoscPozycji = KontrolkaKoszyka.CenaPoRabaciePokazuj;
            wid.PokazywacWartoscVat = false;
            wid.CenaHurtowa = JakieCenyPokazywac.Brak;
            wid.PokazywacWage = KontrolkaKoszyka.PokazywacWageProduktyZaPkt;
            wid.FormatPokazywanejWagi = KontrolkaKoszyka.FormatPokazywanejWagi;
            wid.CenaWPunktach = true;
            wid.TypPozycjiKoszyka = TypPozycjiKoszyka.ZaPunkty;
            return wid;
        }

        public WidocznoscKolumnKoszyk WidocznoscKolumnProduktyGratisy()
        {
            WidocznoscKolumnKoszyk wid = new WidocznoscKolumnKoszyk();
            wid.PokazywacDateDodania = KontrolkaKoszyka.PokazywacDateDodaniaDoKoszykaProduktyGratisy;
            wid.PokazywacMetkeRodzinowa = KontrolkaKoszyka.PokazywacMetkeRodzinowaKoszykProduktyGratisy;
            wid.PokazywacZdjecieProduktu = KontrolkaKoszyka.PokazywacZdjecieProduktuKoszykProduktyGratisy;
            wid.PokazywacNazweProduktu = KontrolkaKoszyka.PokazywacNazweProduktuKoszykProduktyGratisy;
            wid.PokazywacSymbolProduktu = KontrolkaKoszyka.PokazywacSymbolProduktuKoszykProduktyGratisy;
            wid.PokazywacKodKreskowy = KontrolkaKoszyka.PokazywacKodKreskowyProduktuKoszykProduktyGratisy;
            wid.CenaJednostowa = KontrolkaKoszyka.CenaPoRabaciePokazuj;
            wid.PokazywacVat = KontrolkaKoszyka.PokazywacVatKoszykProduktyGratisy;
            wid.UkrywacJednoskeMiaryIIlosc = SolexBllCalosc.PobierzInstancje.Konfiguracja.UkryjJednostkiMiary;
            wid.WartoscPozycji = KontrolkaKoszyka.CenaPoRabaciePokazuj;
            wid.PokazywacWartoscVat = KontrolkaKoszyka.PokazywacWartoscVatKoszykProduktyGratisy;
            wid.CenaHurtowa = KontrolkaKoszyka.PokazywacCenaKatalogowaProduktyGratisy;
            wid.PokazywacWage = KontrolkaKoszyka.PokazywacWageProduktyGratisy;
            wid.FormatPokazywanejWagi = KontrolkaKoszyka.FormatPokazywanejWagi;
            wid.TypPozycjiKoszyka = TypPozycjiKoszyka.Gratis;
            return wid;
        }

        public WidocznoscKolumnKoszyk WidocznoscKolumnProduktyAutomatyczne()
        {
            WidocznoscKolumnKoszyk wid = new WidocznoscKolumnKoszyk();
            wid.PokazywacDateDodania = KontrolkaKoszyka.PokazywacDateDodaniaDoKoszykaProduktyAutomatyczne;
            wid.PokazywacMetkeRodzinowa = KontrolkaKoszyka.PokazywacMetkeRodzinowaKoszykProduktyAutomatyczne;
            wid.PokazywacZdjecieProduktu = KontrolkaKoszyka.PokazywacZdjecieProduktuKoszykProduktyAutomatyczne;
            wid.PokazywacNazweProduktu = KontrolkaKoszyka.PokazywacNazweProduktuKoszykProduktyAutomatyczne;
            wid.PokazywacSymbolProduktu = KontrolkaKoszyka.PokazywacSymbolProduktuKoszykProduktyAutomatyczne;
            wid.PokazywacKodKreskowy = KontrolkaKoszyka.PokazywacKodKreskowyProduktuKoszykProduktyAutomatyczne;
            wid.CenaJednostowa = KontrolkaKoszyka.CenaPoRabaciePokazuj;
            wid.PokazywacVat = KontrolkaKoszyka.PokazywacVatKoszykProduktyGratisy;
            wid.UkrywacJednoskeMiaryIIlosc = SolexBllCalosc.PobierzInstancje.Konfiguracja.UkryjJednostkiMiary;
            wid.WartoscPozycji = KontrolkaKoszyka.CenaPoRabaciePokazuj;
            wid.PokazywacWartoscVat = KontrolkaKoszyka.PokazywacWartoscVatKoszykProduktyAutomatyczne;
            wid.CenaHurtowa = KontrolkaKoszyka.PokazywacCenaKatalogowaProduktyAutomatyczne;
            wid.PokazywacWage = KontrolkaKoszyka.PokazywacWageProduktyAutomatyczne;
            wid.FormatPokazywanejWagi = KontrolkaKoszyka.FormatPokazywanejWagi;
            wid.TypPozycjiKoszyka = TypPozycjiKoszyka.Automatyczny;
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
