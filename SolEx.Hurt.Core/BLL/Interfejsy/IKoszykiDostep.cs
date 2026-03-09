using SolEx.Hurt.Core.BLL.Web;
using SolEx.Hurt.Core.BLL.ZadaniaKoszyka;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Web;
using System;
using System.Collections.Generic;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core.BLL.Interfejsy
{
    public interface IKoszykiDostep
    {
        List<string> PobierzDostepneMagazyny(KoszykBll koszyk, IKlient aktualnyKlient);

        void UaktualnijKoszyk(KoszykBll koszyk, bool aktualizujPozycje = true);

        List<ISposobDostawy> WszystkjeSposobyDostawy(IKoszykiBLL koszyk);

        List<ISposobDostawy> DostepneSposobyDostawy(IKoszykiBLL koszyk);

        List<Platnosc> DostepneSposobyPlantosci(IKoszykiBLL koszyk);

        KoszykBll ZmienPozycjeKoszyka(List<KoszykPozycje> pozycje, IKlient k, out List<ModelBLL.Interfejsy.IKoszykPozycja> przekroczoneStany,
            out List<ModelBLL.Interfejsy.IKoszykPozycja> przekroczoneLimity, out List<IProduktKlienta> nowe,
            out List<ModelBLL.Interfejsy.IKoszykPozycja> zmienioneilosci, out List<ModelBLL.Interfejsy.IKoszykPozycja> dodane, KoszykBll koszyk);

        /// <summary>
        /// Dodaje, zwiêksza stan pozycji w koszyku
        /// </summary>
        /// <param name="item">Koszyk do którego dodajemy</param>
        /// <param name="klient"></param>
        /// <param name="wzor"></param>
        /// <returns>Dodana/Zmodyfikowana pozycja</returns>
        ModelBLL.Interfejsy.IKoszykPozycja DodajPozycje(IKoszykiBLL item, IKlient klient, IKoszykPozycja wzor);

        /// <summary>
        /// Spradza czy produkt ma przekroczony stan - NIE bierze pod uwagê modu³u przekroczone stany w koszyku
        /// </summary>
        /// <param name="koszyk"></param>
        /// <param name="pozycja"></param>
        /// <returns></returns>
        bool CzyJestPrzekroczonyStan(KoszykBll koszyk, ModelBLL.Interfejsy.IKoszykPozycja pozycja);

        bool CzyJestWlaczonyModulPrzekroczonychStanow { get; }

        void FinalizujKoszyk(IKoszykiBLL koszyk, IKlient klient, IKlient przedstawiciel);

        void FinalizacjaKoszyka(KoszykBll koszyk, IKlient aktualny, out bool akcepacja, IKlient klient, IKlient przedstawiciel);

        void AktualizujStany(ZamowieniaBLL koszyk, List<ZamowieniaProduktyBLL> pozycje);

        OdpowiedzKoszyk WygenerujKomunikaty(int jezyk, IKoszykiBLL result, List<IKoszykPozycja> przekroczoneStany,
            List<IKoszykPozycja> przekroczoneLimity, List<IProduktKlienta> nowe, List<IKoszykPozycja> zmienioneIlosci,
            List<IKoszykPozycja> pozycjeDodawane, IKlient aktualnyKlient);

        decimal SprawdzIlosc(IProduktKlienta produkt, long jednostka, decimal ilosc, decimal poprzedniailosc,decimal iloscWKoszyku = 0);

        decimal SprawdzIlosc(ModelBLL.Interfejsy.IKoszykPozycja pozycjaBLL);

        OdpowiedzKoszykaDlaPozycji ZmienStatusPozycjiInfoDostepnosc(IKlient klient, long produkt, int jezyk, out bool ist);

        //    ModelBLL.Interfejsy.IKoszykPozycja ZmienStatusPozycji(KoszykBll koszyk, IKlient klient, int produkt);

        OdpowiedzKoszykaDlaPozycji ZmienStatusPozycjiUlubione(IKlient klient, long produkt, int jezyk, out bool ist);

       // OdpowiedzKoszyk DodajUlubionne(IKlient AktualnyKlient, HashSet<long> produkty, int AktualnyJezyk);

        IList<Tuple<IProduktKlienta, ParametryIlosciProduktu>> PobierzDostepneGratisy(IKoszykiBLL koszyk, int AktualnyJezyk, IKlient AktualnyKlient);

        DodawanieProduktuPrzyciski PobierzParametryPrzyciskowDodawania(IKoszykiBLL koszyk, long produkt,
            TypPozycjiKoszyka typ, IKlient klient, int jezyk, bool? dodawanieTekstowe, string tekstprzyciskbrak = "Dodaj do koszyka",
            string tekstprzyciskjest = "W koszyku");

        long StworzNowyKoszyk(IKlient klient, string nazwa, TypKoszyka typ = TypKoszyka.Koszyk);

        void Odrzuc(KoszykBll koszyk, IKlient klient);

        decimal? PozostalyLimitWartosciZamowien(IKlient klient, IKoszykiBLL koszyk, out decimal przekroczono);

        int? PozostalyLimitIloscZamowien(IKlient k);

        bool PrzekroczoneLimityKoszyka(IKoszykiBLL koszyk);

        bool MoznaFinalizowacKoszykPrzezLimity(IKoszykiBLL koszyki);
        IList<KoszykPozycje> UzupelnijPozycjePoSelect(int jezykId, IKlient zadajacy, IList<KoszykPozycje> obj, object parametrDoMetodyPoSelect = null);
        IList<KoszykBll> MetodaPrzetwarzajacaPoSelect(int i, IKlient klient, IList<KoszykBll> koszyki, object arg4);
        Komunikat[] WykonajModulyKoszykowe(KoszykBll koszyk);

        JednostkaProduktu[] UzupelnijJednostkiOKrokDodania(List<JednostkaProduktu> jednostki,IProduktKlienta produkt);
        bool WyslijKoszykDoAkceptacji(KoszykBll koszyk, IKlient aktualnyKlient);
        bool CzyKlientMaKoszykiDoAkceptacji(long idKlienta, out int? ilosc);
        void UsunAkceptacjeKoszyka(long koszykId);
    }
}