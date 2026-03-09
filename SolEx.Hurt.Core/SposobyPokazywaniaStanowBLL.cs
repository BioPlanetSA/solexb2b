using System.Text;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ServiceStack.Common;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;
using SposobPokazywaniaStanow = SolEx.Hurt.Core.ModelBLL.SposobPokazywaniaStanow;
using System.Collections.Concurrent;
using SolEx.Hurt.Core.Helper;

//
// mamy klase sposob pokazywania stanow -> ktora mowi o sposobie, dla kogo jest dostępny.
// mamy klase regula sposobu pokazywani stanow - regula definiuje w jakich warunkach ma sie pokazac stan
// 
// 

namespace SolEx.Hurt.Core
{
    public class SposobyPokazywaniaStanowBLL : LogikaBiznesBaza
    {
        public SposobyPokazywaniaStanowBLL(ISolexBllCalosc calosc) : base(calosc)
        {
        }
        /// <summary>
        /// Sprzwdzamy czy można pokazywać stany dla klienta
        /// </summary>
        /// <param name="klient"></param>
        /// <param name="dozwoloneRole"></param>
        /// <param name="idPRzedstawiciela"></param>
        /// <returns></returns>
        public virtual bool CzyPokazacStanNaPodstawieRoliKlienta(IKlient klient, List<RoleType> dozwoloneRole, IKlient idPRzedstawiciela)
        {
            //maly speedup - czy sa wszystkie role wybrane - lub zadna - jesli tak to zawsze pokazuj wszystkim
            if (dozwoloneRole == null || dozwoloneRole.Count == 0)
            {
                return true;
            }
            HashSet<RoleType> roleKlienta = klient.Role;
            //czy przedstawiciel zalogowany w imeniu klienta - jesli tak to przepisujemy role i bierzemy z przedstawiciela a nie klienta
            if (idPRzedstawiciela != null)
            {
                roleKlienta = idPRzedstawiciela.Role;
            }
          
            //u nas wszyscy sa klientami - wiec przechwyt musi byc - ze jesli jest wybrany TYLKO klient to widza TYLKO CI ktorzy sa TYLKO klientami 
            if (dozwoloneRole.Count == 1 && dozwoloneRole[0] == RoleType.Klient)
            {
                //ma wiecej niz 1 role  = MA NIE WIDZIEC !!
                if (roleKlienta.Count > 1)
                {
                    return false;
                }
                return true;
            }

            //sprawdzamy czy role sie zgadzaja - juz bez klienta 
            if (dozwoloneRole.Any(x => roleKlienta.Contains(x)))
            {
                return true;
            }

            return false;
        }

        private ConcurrentDictionary<KeyValuePair<long,long?>, List<SposobPokazywaniaStanow>> slownikSposobowDlaKlienta = new ConcurrentDictionary<KeyValuePair<long, long?>, List<SposobPokazywaniaStanow>>();
        private Dictionary<long, SposobPokazywaniaStanow> _slownikSposobowPokazywaniaStanow = null;

        private ConcurrentDictionary<long, bool> _czyKlientMaSposobyStanow = new ConcurrentDictionary<long, bool>();

        public bool CzyKlientMaSposobyPokazywaniaStanow(IKlient klient)
        {
            bool czyKlientMaStany = false;
            if (_czyKlientMaSposobyStanow.TryGetValue(klient.Id, out czyKlientMaStany))
            {
                return czyKlientMaStany;
            }

            var sposoby = this.WszystkieSposobyKlienta(klient, klient.JezykId, null);
            if (sposoby != null && sposoby.Any())
            {
                czyKlientMaStany = true;
            }
            else
            {
                czyKlientMaStany = false;
            }

            _czyKlientMaSposobyStanow.TryAdd(klient.Id, czyKlientMaStany);
            return czyKlientMaStany;
        }

        /// <summary>
        /// Pobieramy wszystkie sposoby pokazywania stanow dla klienta zwracany jest slownik gdzie kluczem jest pozycja natomiast jako wartosc zwracany jest slownik sposobu pokazywania stanu.
        /// Metoda działa tak że pobiera wszystkie stany dla klienta - wylicza jakie stany klient ma widzieć i zapis do szybkiego slownika gdzie klucz to keyValuePar(klientid,przedstawicielID), wartosc lista stanow.
        /// </summary>
        /// <param name="klient"></param>
        /// <param name="lang"></param>
        /// <param name="przedstawiciel"></param>
        /// <param name="idSposobowRozpartywanych"></param>
        /// <param name="sposobyZPozycjaNaLiscie"></param>
        /// <returns></returns>
        public Dictionary<PozycjaLista,  Dictionary<long, SposobPokazywaniaStanow>> WszystkieSposobyKlienta(IKlient klient, int lang, IKlient przedstawiciel, List<long> idSposobowRozpartywanych=null, bool sposobyZPozycjaNaLiscie=true )
        {
            if (!klient.CzyWidziStany)
            {
                return null;
            }
            if (_slownikSposobowPokazywaniaStanow == null)
            {
                LockHelper.PobierzInstancje.UruchomKodWLocku_BezUzywaniaCache("_slownikSposobowPokazywaniaStanow", () =>
                {
                    if (_slownikSposobowPokazywaniaStanow == null)
                    {
                        _slownikSposobowPokazywaniaStanow = Calosc.DostepDane.Pobierz<SposobPokazywaniaStanow>(lang, null).ToDictionary(x => x.Id, x => x);
                    }
                });
            }

            if (!_slownikSposobowPokazywaniaStanow.Any())
            {
                return null;
            }

            List<SposobPokazywaniaStanow> sposobyPokazywanaStanowKlienta;
            if (!slownikSposobowDlaKlienta.TryGetValue(new KeyValuePair<long, long?>(klient.Id, (przedstawiciel!=null)?przedstawiciel.Id:(long?) null), out sposobyPokazywanaStanowKlienta))
            {
                sposobyPokazywanaStanowKlienta = PobierzSposobyKlienta(klient, przedstawiciel, lang, _slownikSposobowPokazywaniaStanow);
                slownikSposobowDlaKlienta.TryAdd(new KeyValuePair<long, long?>(klient.Id, (przedstawiciel != null) ? przedstawiciel.Id : (long?)null),sposobyPokazywanaStanowKlienta);
            }

            List<SposobPokazywaniaStanow> stany = null;
            if (idSposobowRozpartywanych != null)
            {
                stany = sposobyPokazywanaStanowKlienta.Where(x=>idSposobowRozpartywanych.Contains(x.Id)).ToList();
            }
            else
            {
                stany = sposobyPokazywanaStanowKlienta;
            }

            if (sposobyZPozycjaNaLiscie)
            {
                return stany.Where(x=>x.PozycjaLista!=PozycjaLista.Brak).GroupBy(x => x.PozycjaLista).ToDictionary(x => x.Key, x => x.ToDictionary(y => y.Id, y => y));
            }
            return stany.GroupBy(x => x.PozycjaLista).ToDictionary(x => x.Key, x => x.ToDictionary(y => y.Id, y => y));
        }


        public IList<SposobPokazywaniaStanow> UzupelnijSposoby(int jezykID, IKlient zajadacyKlient, IList<SposobPokazywaniaStanow> listaSposobow, object daneDoSelecta)
        {
            foreach (var sposobPokazywaniaStanow in listaSposobow)
            {
                sposobPokazywaniaStanow.Reguly= SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<SposobPokazywaniaStanowRegula>(null, x => x.SposobId == sposobPokazywaniaStanow.Id).OrderBy(x => x.Kolejnosc).ToList();
            }
            return listaSposobow;
        }

        /// <summary>
        /// Pobieramy sposoby pokazywania stanów dla klienta
        /// </summary>
        /// <param name="klient"></param>
        /// <param name="przedstawiciel"></param>
        /// <param name="lang"></param>
        /// <param name="wszystkieSposoby"></param>
        /// <returns></returns>
        public virtual List<SposobPokazywaniaStanow> PobierzSposobyKlienta(IKlient klient, IKlient przedstawiciel, int lang, Dictionary<long,SposobPokazywaniaStanow> wszystkieSposoby)
        {
            List<SposobPokazywaniaStanow> stanyklienta = new List<SposobPokazywaniaStanow>();
            
            foreach (var sp in wszystkieSposoby)
            {
                if ((klient.Dostep != AccesLevel.Niezalogowani && !klient.Role.Contains(RoleType.Administrator)) && !Calosc.WidocznosciTypowBll.KlientMaDostepDoObiektu(klient, sp.Value))
                {
                    continue;
                }
                if (sp.Value.Dostep == AccesLevel.Wszyscy || sp.Value.Dostep == klient.Dostep)
                {
                    bool zgadzaSieRola = CzyPokazacStanNaPodstawieRoliKlienta(klient, sp.Value.DozwolonaRolaKlienta, przedstawiciel);
                    if (!zgadzaSieRola)
                        continue;
                    stanyklienta.Add(sp.Value);
                }
            }

            return stanyklienta;
        }



        /// <summary>
        /// Zwraca slownik w którym kluczem jest id spełnionej reguły a wartosc to obiekt zawierajacy spelnioną regułę oraz magazyn oraz stan dla niego,
        /// metoda zwraca pierwszą pasującą regułę dla produktu bazowego oraz magazynu
        /// </summary>
        /// <param name="sposob"></param>
        /// <param name="produkt">Produk bazowy dla którego sprawdzamy reguły (potrzebujemy produkt bazowy ze względu naprodukty wirtualne)</param>
        /// <param name="langId"></param>
        /// <param name="klient"></param>
        /// <returns></returns>
        public Dictionary<int,List<StanNaMagazynie>> PobierzStanProduktuWgSposobu(SposobPokazywaniaStanow sposob, IProduktBazowy produkt, int langId, IKlient klient)
        {
            if (sposob==null)
            {
                return null;
            }
            int? regulaId = null;
            Dictionary<int, List<StanNaMagazynie>> regulySpelniajace = new Dictionary<int, List<StanNaMagazynie>>();
            HashSet<Magazyn> magazynIdtmp = PobierzMagazynyDlaSposobuPokazyywaniaStanow(sposob, klient);
            if (magazynIdtmp == null)
                return null;
            foreach (var mag in magazynIdtmp)
            {
                decimal stan = produkt.PobierzStan(new HashSet<int>() { mag.Id });
                int? idReguly = Calosc.Cache.PobierzObiekt<int?>(GetCacheName(sposob.Id, produkt.Id, mag.Id, langId));
                if (idReguly == null)
                {
                    foreach (SposobPokazywaniaStanowRegula regula in sposob.Reguly)
                    {
                        if (SpelniaRegule(produkt, stan, regula, mag))
                        {
                            //trafiony zatopiony
                            regulaId = regula.Id;
                            Calosc.Cache.DodajObiekt(GetCacheName(sposob.Id, produkt.Id, mag.Id, langId), regulaId);
                            break;
                        }
                    }
                    Calosc.Cache.DodajObiekt(GetCacheName(sposob.Id, produkt.Id, mag.Id, langId), 0);
                }
                else
                {
                    regulaId = idReguly;
                }
                if (regulaId.HasValue && regulaId!=0)
                {
                    if (!regulySpelniajace.ContainsKey(regulaId.Value))
                    {
                        regulySpelniajace.Add(regulaId.Value, new List<StanNaMagazynie>());
                    }
                    regulySpelniajace[regulaId.Value].Add(new StanNaMagazynie(mag, stan, regulaId.Value));
                }
            }
            return regulySpelniajace;
        }

        /// <summary>
        /// Pobiera ID magazynu na podstawie sposobu pokazywania stanów.
        /// Dla ID != 0 zwraca to samo ID - to jest ID z kontrolki stanów.
        /// Jeśli sposób pokazywania stanu ma podaną kategorię klienta, z której ma pobrać symbol
        /// magazynu to jeśli aktualnie zalogowany klient ma taką kategorię to niej pobierze symbol
        /// magazynu a następnie metoda zwróci ID tego magazynu. Jeśli nie to metoda 
        /// zwróci ID magazynu które jest w sposobie pok. stanów lub jeśli nie jest to ustawione to zwraca domyślne
        /// ID magazynu do pobierania stanów.
        /// </summary>
        /// <param name="sposob">Sposób pokazywania stanów</param>
        /// <param name="klient">Klient</param>
        /// <returns>ID magazynu z którego ma pobrać stan</returns>
        public virtual HashSet<Magazyn> PobierzMagazynyDlaSposobuPokazyywaniaStanow(SposobPokazywaniaStanow sposob,IKlient klient)
        {
            HashSet<Magazyn> magazynyId = new HashSet<Magazyn>();

            //jesli sposob ma ustawiony magazyn domyslny to pokazujemy z tego domyslnego
            if (sposob.DomyslnyMagazynId.HasValue)
            {
                magazynyId.Add(Calosc.Konfiguracja.SlownikMagazynowPoId[ sposob.DomyslnyMagazynId.Value] );
                return magazynyId;
            }

            //czy klient ma magazyny dostepne
            if (klient.DostepneMagazyny != null && klient.DostepneMagazyny.Any())
            {
                magazynyId = new HashSet<Magazyn>( klient.DostepneMagazynyDlaKlienta );
                return magazynyId;
            }

            //magazyn domyslny
            magazynyId.Add(Calosc.ProduktyStanBll.MagazynDomyslny());

            return magazynyId;
        }

        public virtual string CacheSposobyDlaKlienta(long idKlienta, int jezyk, long przedstawicielId)
        {
            return string.Format("stany_klienta{0}_jezyk_{1}__przedstawiciel{2}", idKlienta,jezyk,przedstawicielId);
        }


        public virtual string GetCacheName(long idSposobu, long produktId, int magazynId, int langId )
        {
            return string.Format("stan_prod{0}_sposob_{1}__mag{2}_lang{3}", produktId, idSposobu, magazynId, langId);
        }

        /// <summary>
        /// Metoda sprawdzająca czy produkt spełnia warunek cyklicznej dostawy dla reguły
        /// </summary>
        /// <param name="produkt">Produk bazowy dla którego sprawdzamy warunek (potrzebujemy produkt bazowy ze względu naprodukty wirtualne)</param>
        /// <param name="regula"></param>
        /// <returns></returns>
        public bool SpelniaWarunekCyklicznejDostawy(IProduktBazowy produkt, SposobPokazywaniaStanowRegula regula)
        {
            bool warunekcykliczna = regula.CyklicznaDostawa == CyklkicznaDostawa.NieWplywa || (regula.CyklicznaDostawa == CyklkicznaDostawa.Posiada && produkt.NajblizszaDostawa.HasValue) ||
                             (regula.CyklicznaDostawa == CyklkicznaDostawa.NiePosiada && !produkt.NajblizszaDostawa.HasValue);
            return warunekcykliczna;
        }
        /// <summary>
        /// Metoda sprawdzająca czy produkt spełnia warunek typu stanu dla reguły
        /// </summary>
        /// <param name="produkt">Produk bazowy dla którego sprawdzamy warunek  (potrzebujemy produkt bazowy ze względu naprodukty wirtualne)</param>
        /// <param name="regula"></param>
        /// <returns></returns>
        public bool SpelniaWarunekTypuStanu(IProduktBazowy produkt, SposobPokazywaniaStanowRegula regula)
        {
            if (regula.WarunekStany == WarunekStanu.NieWplywa)
            {
                return true;
            }
            bool warunekTypStanu = (regula.WarunekStany == WarunekStanu.Rowny && produkt.PobierzTypStany == regula.TypStanu) || (regula.WarunekStany == WarunekStanu.Rozny && produkt.PobierzTypStany != regula.TypStanu);
            return warunekTypStanu;
        }
        /// <summary>
        /// Metoda sprawdzającza czy spełniony jest warunek ilości produktu
        /// </summary>
        /// <param name="produkt">Produk bazowy dla którego sprawdzamy warunek (potrzebujemy produkt bazowy ze względu naprodukty wirtualne)</param>
        /// <param name="regula"></param>
        /// <param name="stan"></param>
        /// <returns></returns>
        public bool SpelniaWarunekIlosci(IProduktBazowy produkt, SposobPokazywaniaStanowRegula regula, decimal stan)
        {

            decimal iloscDoPorownania = regula.IloscProduktu;
            if (regula.RazyStanMinimalny)
            {
                iloscDoPorownania *= produkt.StanMin;
            }
            return (regula.WarunekIlosci == Warunek.Wiesze && stan > iloscDoPorownania) ||
                   (regula.WarunekIlosci == Warunek.MniejszyRowny && stan <= iloscDoPorownania) ||
                   (regula.WarunekIlosci == Warunek.WiekszaRowna && stan >= iloscDoPorownania) ||
                   (regula.WarunekIlosci == Warunek.Mniejszy && stan < iloscDoPorownania) ||
                   (regula.WarunekIlosci == Warunek.Rowna && stan == iloscDoPorownania);
        }

        /// <summary>
        /// Metoda sprawdzająca czy spełniony jest warunek stanu minimalnego produktu
        /// </summary>
        /// <param name="produkt">Produk bazowy dla którego sprawdzamy warunek (potrzebujemy produkt bazowy ze względu naprodukty wirtualne)</param>
        /// <param name="regula"></param>
        /// <returns></returns>
        public bool SpelniaWarunekStanuMinimalnego(IProduktBazowy produkt, SposobPokazywaniaStanowRegula regula)
        {
            return (regula.WarunekIlosciMinimalnej == Warunek.Wiesze && produkt.StanMin > regula.IloscMinimalna) ||
                   (regula.WarunekIlosciMinimalnej == Warunek.MniejszyRowny && produkt.StanMin <= regula.IloscMinimalna) ||
                   (regula.WarunekIlosciMinimalnej == Warunek.WiekszaRowna && produkt.StanMin >= regula.IloscMinimalna) ||
                   (regula.WarunekIlosciMinimalnej == Warunek.Mniejszy && produkt.StanMin < regula.IloscMinimalna) ||
                   (regula.WarunekIlosciMinimalnej == Warunek.Rowna && produkt.StanMin == regula.IloscMinimalna);
        }

        /// <summary>
        /// Tworzenie szablonów reguł do pokazywania stanów.
        /// </summary>
        public void UtworzaSzablonyRegul()
        {
            var jezykiSystemu = SolexBllCalosc.PobierzInstancje.Konfiguracja.JezykiWSystemie;


            string katalog = PlikiDostep.PobierzInstancje.ZbudujSciezkeFizyczna("/Views/Shared/Stany");
            if (!Directory.Exists(katalog))
            {
                Directory.CreateDirectory(katalog);
            }
            else
            {
                string[] stareWidoki = Directory.GetFiles(katalog);
                foreach (var starywidok in stareWidoki)
                {
                    File.Delete(starywidok);
                }
            }


            foreach (var jezyk in jezykiSystemu)
            {
                IList<SposobPokazywaniaStanowRegula> reguly = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<SposobPokazywaniaStanowRegula>(jezyk.Key,null);
                foreach (var sposobPokazywaniaStanowRegula in reguly)
                {
                    string wynikHtml= "@using SolEx.Hurt.Core.BLL \n" +
                                    "@using SolEx.Hurt.Web.Site2.Helper \n " +
                                    "@model SolEx.Hurt.Web.Site2.Models.ParametryStany \n " +
                                    sposobPokazywaniaStanowRegula.WynikHtml;

                    string sciezka = string.Format("{0}/{1}_{2}.cshtml", katalog, sposobPokazywaniaStanowRegula.Id, jezyk.Value.Symbol);
                    if (!File.Exists(sciezka))
                    {
                        using (FileStream fs = File.Create(sciezka))
                        {
                            UTF8Encoding enc = new UTF8Encoding(true);
                            Byte[] info = enc.GetPreamble().Concat(enc.GetBytes(wynikHtml)).ToArray();
                            fs.Write(info, 0, info.Length);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Metoda sprawdzająca czy produkt spełnia regułe
        /// </summary>
        /// <param name="produkt">Produk bazowy dla którego sprawdzamy regułę (potrzebujemy produkt bazowy ze względu naprodukty wirtualne)</param>
        /// <param name="stan"></param>
        /// <param name="regula"></param>
        /// <param name="magazyn"></param>
        /// <returns></returns>
        public virtual bool SpelniaRegule(IProduktBazowy produkt, decimal stan, SposobPokazywaniaStanowRegula regula, Magazyn magazyn)
        {
            try
            {
                if (regula.CzyTerminDostawy)
                {
                    if (string.IsNullOrEmpty(produkt.Dostawa) || produkt.Dostawa == "0")
                    {
                        return false;
                    }
                }
                if (!SpelniaWarunekIlosci(produkt, regula, stan))
                {
                    return false;
                }
                if (!SpelniaWarunekStanuMinimalnego(produkt, regula))
                {
                    return false;
                }
                if (SpelniaWarunekCyklicznejDostawy(produkt, regula) && SpelniaWarunekTypuStanu(produkt, regula))
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Bład parsowania produkt {0} dostawa {1} stan {2} magazyn {3}", produkt.Id, produkt.Dostawa, stan, magazyn.Id), ex);
            }
            return false;
        }
    }
}
