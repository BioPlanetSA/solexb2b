using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Interfaces;
using SolEx.Hurt.Model.Interfaces.Sync;

namespace SolEx.Hurt.Core.BLL.WirtualneProduktyProvidery
{
    /// <summary>
    /// klasa abstrakcyjna po której dziedziszą klasy kastomizowane dla klienta do tworzenia produktów wirtualnych. Klase która ma u klienta być wykorzystywana wybiera się w ustawieniach B2B
    /// </summary>
    public abstract class ProduktyWirtualneProvider
    {
        public ISolexBllCalosc Calosc = SolexBllCalosc.PobierzInstancje;

        protected abstract IList<ProduktKlienta> generujWirtualneProdukty(int jezyk, IKlient klient, Dictionary<long, ProduktBazowy> produktyNaBazieKtorychZrobicWirtualne);

        public static object lok = new object();

        /// <summary>
        /// Metoda pobierająca produkty wirtualne w locie w B2B podczas pobierania produktów klienta. Metoda dostaje liste produktów klienta na podstawie ktorych należy zrobić wirtualne.
        /// Nie wolno w tej metodzie wyliczać żadnych parametrów - wszystkie parametry wirtualizcji musza być już gotowe. Paramertry są przeliczane w metodzie synchronizacji SynchronizatorPrzetwarzajWirtualneProdukty
        /// </summary>
        /// <param name="jezyk"></param>
        /// <param name="klient"></param>
        /// <param name="produktyNaBazieKtorychZrobicWirtualne"></param>
        /// <returns></returns>
        public IList<ProduktKlienta> PobierzWirtualneProdukty(int jezyk, IKlient klient, Dictionary<long, ProduktBazowy> produktyNaBazieKtorychZrobicWirtualne)
        {
            string cacheName = $"wirtualne_produkty_klient{klient.Id}_jezyk{klient.JezykId}";

            int iloscAbstrakcyjnychNiewidocznychPrzedGenerowaniem = produktyNaBazieKtorychZrobicWirtualne.Count(x => !x.Value.Widoczny);

            IList<ProduktKlienta> wynik =  Calosc.Cache.PobierzObiekt( () => generujWirtualneProdukty(jezyk, klient, produktyNaBazieKtorychZrobicWirtualne), lok, cacheName);

            int iloscAbstrakcyjnychNiewidocznychPoGenerowaniu = produktyNaBazieKtorychZrobicWirtualne.Count(x => !x.Value.Widoczny);
            if (iloscAbstrakcyjnychNiewidocznychPrzedGenerowaniem != iloscAbstrakcyjnychNiewidocznychPoGenerowaniu)
            {
                throw new Exception("Nie wolno ustawiać widoczności produktów abstrakcyjnych w providerze produktów wirtualnych");
            }


            return wynik;
        }

        /// <summary>
        /// Metoda uruchamiana w synchroniacji po stronie klienta ktora oznacza produkty jako bazowe do tworzenia wirtualnych. Metoda zajmuje się również przygotowaniem parametrów witualizacji 
        /// - parametry powinny być zapisane w polu ParametryDlaWirtualizacji
        /// </summary>
        /// <param name="produkty"></param>
        /// <param name="produktyTlumaczenia"></param>
        /// <param name="produktyNaB2B"></param>
        /// <param name="jednostki"></param>
        /// <param name="produktyCechyErp"></param>
        /// <param name="pkzErp"></param>
        /// <param name="aktualnyProvider"></param>
        /// <param name="produktuUkryteErp"></param>
        /// <param name="zamiennikierp"></param>
        /// <param name="kategorie"></param>
        /// <param name="cechytmp"></param>
        /// <param name="atrybuty"></param>
        /// <param name="klienci"></param>
        public abstract void SynchronizatorPrzetwarzajWirtualneProdukty(ref List<Produkt> produkty, ref List<Tlumaczenie> produktyTlumaczenia, Dictionary<long, Produkt> produktyNaB2B, ref List<JednostkaProduktu> jednostki, ref Dictionary<long, ProduktCecha> produktyCechyErp, ref List<ProduktKategoria> pkzErp, ISyncProvider aktualnyProvider, ref List<ProduktUkryty> produktuUkryteErp, ref List<ProduktyZamienniki> zamiennikierp, Dictionary<long, KategoriaProduktu> kategorie, ref List<Cecha> cechytmp, ref List<Atrybut> atrybuty, ref Dictionary<long, Model.Klient> klienci );

        /// <summary>
        /// metoda która przed aktualizacją pozycji dokumentów przerabia produkty abstrakcyjne na wirtualne - gdybyśmy w historii potrzebowali produkty wirtualne a nie prawdziwe
        /// </summary>
        /// <param name="klient"></param>
        /// <param name="pozycjePrawdziwe"></param>
        public abstract void PoprawProduktyNaDokumentach(IKlient klient, ref List<HistoriaDokumentuProdukt> pozycjePrawdziwe);

        /// <summary>
        /// Określay czy produkty abstrakcyjne (bazowe) mają być widoczne dla klienta. Domyślnie False
        /// </summary>
        public bool PokazujProduktyBazowe => false;

        public abstract bool WplywaNaWidocznoscProduktowDlaKlienta { get; }

        /// <summary>
        /// Czy aktualizować klientów po utworzeniu produktów wirtualnych. Domyślnie na nie. 
        /// </summary>
        public virtual bool AktualizujKlientow => false;

        /// <summary>
        /// Tworzymy obiekt pliku na podstawie ścieżki. 
        /// </summary>
        /// <param name="sciezkapliku"></param>
        /// <param name="sciezkaDoUsuniecia"></param>
        /// <returns></returns>
        public Plik UtworzPlik(string sciezkapliku, string sciezkaDoUsuniecia)
        {
            FileInfo info = new FileInfo(sciezkapliku);
            string katalogZapisu = info.DirectoryName.Replace(sciezkaDoUsuniecia, "") + "/";
            Plik plik = new Plik
            {
                Nazwa = info.Name,
                nazwaLokalna = info.Name,
                Sciezka = info.DirectoryName + "\\",
                Id = sciezkapliku.Trim().WygenerujIDObiektuSHA(),
                RodzajPliku = RodzajPliku.PlikDoPrzeniesieniaJedenNaJedenOdKlienta,
                lokalnaSciezkaDoZapisuPliku = katalogZapisu
            };
            try
            {
                plik.Rozmiar = (int) info.Length;
                plik.Data = info.LastWriteTime.ToUniversalTime().AddMilliseconds(-info.LastWriteTime.ToUniversalTime().Millisecond);

            }
            catch { }
            
            return plik;
        }

        /// <summary>
        /// Wysyłamy pliki do produktów wirtualnych na serwer
        /// </summary>
        /// <param name="listaPlikow"></param>
        /// <param name="sciezkaDoZapisuPlikow"></param>
        /// <param name="sciezkaFolderNaDysku"></param>
        public void WyslijPlikiNaSerwer(List<Plik> listaPlikow, string sciezkaDoZapisuPlikow, string sciezkaFolderNaDysku)
        {
            //Pobieramy wszsystkie pliki 
            List<Plik> plikiB2B = APIWywolania.PobierzInstancje.PlikNaB2BPobierz().Where(x => x.RodzajPliku == RodzajPliku.PlikDoPrzeniesieniaJedenNaJedenOdKlienta).ToList();
            HashSet<int> idPlikow = new HashSet<int>();
            List<int> doUsuniecia = new List<int>();
            Calosc.Log.InfoFormat($"Ilosc plików na b2b: {plikiB2B.Count}");
            Calosc.Log.InfoFormat($"Ilosc plików na na dysku : {listaPlikow.Count}");
            foreach (var plik in plikiB2B)
            {
                string sciezka = plik.Sciezka.Replace(sciezkaDoZapisuPlikow, sciezkaFolderNaDysku).Replace('/', '\\');
                var plikk = listaPlikow.FirstOrDefault(x => x.CzyTeSamePliki(plik) && x.Sciezka.Equals(sciezka, StringComparison.InvariantCultureIgnoreCase));
                if (plikk != null)
                {
                    idPlikow.Add(plikk.Id);
                }
                else
                {
                    doUsuniecia.Add(plik.Id);
                }
            }
            List<Plik> doWyslania = listaPlikow.Where(x => !idPlikow.Contains(x.Id)).ToList();
            Calosc.Log.InfoFormat($"Ilosc plikow do usuniecia: {doUsuniecia.Count}");
            APIWywolania.PobierzInstancje.PlikNaB2BUsun(doUsuniecia);
            Calosc.Log.InfoFormat($"Ilosc plikow do dodania: {doWyslania.Count}");
            APIWywolania.PobierzInstancje.PlikiNaB2BDodajPaczkowanie(doWyslania, s => SolexBllCalosc.PobierzInstancje.Log.Error(s));
        }

    }

}
