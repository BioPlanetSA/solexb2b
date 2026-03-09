using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using ServiceStack.Common;
using ServiceStack.OrmLite;
using ServiceStack.Text;
using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Core.DostepDane;
using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core.BLL
{
    public class CechyAtrybuty : BllBazaCalosc, ICechyAtrybuty
    {
        public CechyAtrybuty(ISolexBllCalosc calosc) : base(calosc){

            //uzupelnic slownik typow stanow
            _slownikTypow = new ConcurrentDictionary<TypStanu, string>();
            foreach (TypStanu cts in Enum.GetValues(typeof(TypStanu)))
            {
                string friendlyName = cts.PobierzAtrybutDlaEnuma<FriendlyNameAttribute>().FriendlyName;
                _slownikTypow.TryAdd(cts, friendlyName);
            }

        }

        /// <summary>
        /// Metoda budująca klucz cache do pobrania cech z aktualnie dostepnych produktów
        /// </summary>
        /// <param name="wybraneKategorie"></param>
        /// <param name="klient"></param>
        /// <param name="szukaneGlobalne"></param>
        /// <param name="szukanieWKategorii"></param>
        /// <param name="staleFiltryWgAtrybutow"></param>
        /// <param name="wybraneJuzWczesniejCechyWgAtrybutow"></param>
        /// <returns></returns>
        public string ZbudujKluczCacheDlaFiltrowListyProduktow(long? wybraneKategorie, IKlient klient, string szukaneGlobalne, string szukanieWKategorii, Dictionary<int, HashSet<long>> staleFiltryWgAtrybutow, Dictionary<int, HashSet<long>> wybraneJuzWczesniejCechyWgAtrybutow, int iloscProduktow)
        {
            //jezeli jest produktów mniej niż 500 to nie robimy cache
            if (iloscProduktow < 500)
            {
                return null;
            }
            return string.Format("{0}_{1}_{2}_{3}_{4}_{5}",
                wybraneKategorie == 0 ? null : wybraneKategorie,
                string.IsNullOrEmpty(szukaneGlobalne) ? null : szukaneGlobalne,
                staleFiltryWgAtrybutow != null && staleFiltryWgAtrybutow.Any() ? string.Join("-", staleFiltryWgAtrybutow.Values.SelectMany(x=>x)) : null,
                string.IsNullOrEmpty(szukanieWKategorii) ? null : szukanieWKategorii,
                wybraneJuzWczesniejCechyWgAtrybutow != null && wybraneJuzWczesniejCechyWgAtrybutow.Any() ? string.Join("-", wybraneJuzWczesniejCechyWgAtrybutow.Values.SelectMany(x => x)) : null,
                klient.OfertaIndywidualizowana? klient.Id.ToString() : null);
        }
        


        /// <summary>
        ///     Główna metoda pobierająca filtry jakie mają się pokazać klientowi w danych okolicznościach. Algorytm działania nie opisany nigddzie niestety :(
        /// </summary>
        /// <param name="wybraneJuzWczesniejCechyWgAtrybutow"></param>
        /// <param name="wybraneKategorie"></param>
        /// <param name="staleFiltryWgAtrybutow"></param>
        /// <param name="szukaneGlobalne"></param>
        /// <param name="klient"></param>
        /// <param name="jezyk"></param>
        /// <param name="szukanieWKategorii"></param>
        /// <returns></returns>
        public List<AtrybutBll> PobierzWyfiltrowane(Dictionary<int, HashSet<long>> wybraneJuzWczesniejCechyWgAtrybutow, long? wybraneKategorie, Dictionary<int, HashSet<long>> staleFiltryWgAtrybutow, string szukaneGlobalne, IKlient klient, int jezyk, string szukanieWKategorii)
        {
#if DEBUG
            if (wybraneJuzWczesniejCechyWgAtrybutow != null && wybraneJuzWczesniejCechyWgAtrybutow.IsEmpty())
            {
                throw new Exception("Obiekt wybraneJuzWczesniejCechyWgAtrybutow ma być NULLem jeśli jest pusty!");
            }
#endif

            // wybieramy cechy nadrzędne dla wybranych cech (wszystkie w ramach jednego atrybutu) i tylko wspólne w ramach wszystkich wybranych atrybutów
            var wspolneCechyNadrzedne = new HashSet<long>();
            if (wybraneJuzWczesniejCechyWgAtrybutow != null)
            {
                foreach (var keyValuePair in wybraneJuzWczesniejCechyWgAtrybutow)
                {
                    //keyValuePair.Key -> idAtrybutu, keyValuePair.Value->idCech
                    var cechyDlaAtrybutu = PobierzWszystkieCechy(jezyk).WhereKeyIsIn(keyValuePair.Value);
                    var cechynadrzedne = cechyDlaAtrybutu.Where(x => (x.CechyNadrzedne != null) && x.CechyNadrzedne.Any()).SelectMany(x => x.CechyNadrzedne).ToArray();
                    if (wspolneCechyNadrzedne.Any())
                    {
                        wspolneCechyNadrzedne.UnionWith(cechynadrzedne);
                    }
                    else
                    {
                        wspolneCechyNadrzedne = new HashSet<long>( cechynadrzedne );
                    }
                }
            }

            var cechynadrzednewgAtrybutu = PobierzWszystkieCechy(jezyk).Values.Where(x => wspolneCechyNadrzedne.Contains(x.Id)).GroupBy(x => x.AtrybutId.GetValueOrDefault()).ToDictionary(x => x.Key, x => new HashSet<long>( x.Select(y => y.Id) ) );
            var wynikowaListaAtrybutow = new List<AtrybutBll>();


            //pobieramy produkty spełniające wszystkie kryteria bez brania pod uwagę wybrane atrybuty
            IList<ProduktKlienta> produktyWszystkieGdybyNieByloZadnychFiltrowWybranych = Calosc.ProduktyKlienta.ProduktySpelniajaceKryteria(wybraneKategorie, szukaneGlobalne, klient, jezyk, null, staleFiltryWgAtrybutow, szukanieWKategorii);
            //budulemy klucz dla cache z produktami spełniającymi wszystkie kryteria bez brania pod uwagę wybrane atrybuty
            string kluczCacheBezWybranychAtrybutow = ZbudujKluczCacheDlaFiltrowListyProduktow(wybraneKategorie,klient,szukaneGlobalne,szukanieWKategorii,staleFiltryWgAtrybutow,null, produktyWszystkieGdybyNieByloZadnychFiltrowWybranych.Count);
            //pobieramy atrybuty jakie posiadają produkty spełniające kryteria bez wybranych atrybutów
            //lista cech dla tych atrybutów jest pobierana z produktów spełniających kryteria
            List<AtrybutBll> atrybytyWszystkichProdukowGdybyNieByloFiltrowWybranych = PobierzAtrybutyListyProduktow(produktyWszystkieGdybyNieByloZadnychFiltrowWybranych, jezyk, klient, kluczCacheBezWybranychAtrybutow);


            //pobieramy produkty spełniające wszystkie kryteria bez brania oraz posiadające wybrane atrybuty
            IList<ProduktKlienta> produktyWszystkiePoWybranychFiltrachStartowych = Calosc.ProduktyKlienta.ProduktySpelniajaceKryteria(wybraneKategorie, szukaneGlobalne, klient, jezyk, wybraneJuzWczesniejCechyWgAtrybutow, staleFiltryWgAtrybutow, szukanieWKategorii);
            //budujemy klucz dla cache z produktami spełniającymi wszystkie kryteria oraz posiadające wybrane atrybuty
            string kluczCacheZWybranymiAtrybutami = ZbudujKluczCacheDlaFiltrowListyProduktow(wybraneKategorie, klient, szukaneGlobalne, szukanieWKategorii, staleFiltryWgAtrybutow, wybraneJuzWczesniejCechyWgAtrybutow, produktyWszystkiePoWybranychFiltrachStartowych.Count);
            //pobieramy atrybuty jakie posiadają produkty spełniające kryteria zawężone o wybrane atrybuty
            //lista cech dla tych atrybutów jest pobierana z produktów spełniających kryteria
            List<AtrybutBll> zawezone = PobierzAtrybutyListyProduktow(produktyWszystkiePoWybranychFiltrachStartowych, jezyk, klient, kluczCacheZWybranymiAtrybutami);

            //beda jakies atrybuty - trzeba kolekcje zrobic
            if (wybraneJuzWczesniejCechyWgAtrybutow == null)
            {
                wybraneJuzWczesniejCechyWgAtrybutow = new Dictionary<int, HashSet<long>>(atrybytyWszystkichProdukowGdybyNieByloFiltrowWybranych.Count);
            }

            var trybPokazywaniaFiltrow = Calosc.Konfiguracja.TrybPokazywaniaFiltrow;

            //przelatujemy po atrybutach wszystkich dostepnych dla produktow gdyby filtrow nie bylo
            foreach (var a in atrybytyWszystkichProdukowGdybyNieByloFiltrowWybranych)
            {
                if (!a.Widoczny || (!string.IsNullOrEmpty(szukaneGlobalne) && !a.PokazujWWyszukiwaniu) || (string.IsNullOrEmpty(szukaneGlobalne) && !a.PokazujNaLiscieProduktow))
                {
                    //kasowanie atrybutów ktorych i tak nie pokazujemy na liście produktow
                    continue;
                }

                AtrybutBll wynikowyAtrybutZCechamiDoWybrania = null;
                //jesli juz wczesniej byl wybrany filtr z tego atrybutu
                if (wybraneJuzWczesniejCechyWgAtrybutow.ContainsKey(a.Id))
                {
                    var wybranebez = new Dictionary<int, HashSet<long>>(wybraneJuzWczesniejCechyWgAtrybutow);
                    wybranebez.Remove(a.Id);
                    var ids = Calosc.ProduktyKlienta.ProduktySpelniajaceKryteria(wybraneKategorie, szukaneGlobalne, klient, jezyk, wybranebez, staleFiltryWgAtrybutow, szukanieWKategorii);
                    if (ids.IsEmpty())
                    {
                        continue;
                    }
                    kluczCacheZWybranymiAtrybutami = ZbudujKluczCacheDlaFiltrowListyProduktow(wybraneKategorie, klient, szukaneGlobalne, szukanieWKategorii, staleFiltryWgAtrybutow, wybranebez, ids.Count); 
                    //pobieramy atrybuty jakie posiadają produkty spełniające kryteria zawężone o wybrane atrybuty
                    //lista cech dla tych atrybutów jest pobierana z produktów spełniających kryteria
                    wynikowyAtrybutZCechamiDoWybrania = PobierzAtrybutyListyProduktow(ids, jezyk, klient, kluczCacheZWybranymiAtrybutami).First(x => x.Id == a.Id);
                }
                else
                {
                    //wczesniej nie bylo wybranego tego atrybuty
                    var atrybut = zawezone.FirstOrDefault(x => x.Id == a.Id);
                    if (atrybut == null)
                    {
                        continue;
                    }
                    wynikowyAtrybutZCechamiDoWybrania = atrybut;
                    //nie trzeba kopia robić bo juz jest kopia
                }

                if (wynikowyAtrybutZCechamiDoWybrania != null && wynikowyAtrybutZCechamiDoWybrania.Widoczny)
                {
                    if ((klient.Dostep == AccesLevel.Niezalogowani) && !wynikowyAtrybutZCechamiDoWybrania.CechyPokazujKatalog)
                    {
                        continue;
                    }

                    //lista cech dla atrybutów jest pobierana z produktów spełniających kryteria a nie z bazy
                    var listaCech = new List<CechyBll>(wynikowyAtrybutZCechamiDoWybrania.ListaCech);
                    foreach (var cechyBll in wynikowyAtrybutZCechamiDoWybrania.ListaCech)
                    {
                        if (cechyBll.CechyNadrzedne == null)
                        {
                            continue;
                        }
                        var usunCeche = false;
                        if (cechyBll.CechyNadrzedne.Any())
                        {
                            if (!wybraneJuzWczesniejCechyWgAtrybutow.Any() && (trybPokazywaniaFiltrow == TrybPokazywaniaFiltrow.WymuszajSciezke))
                            {
                                usunCeche = true;
                            }
                            foreach (var a2 in wybraneJuzWczesniejCechyWgAtrybutow)
                            {
                                if (!a2.Value.Overlaps(cechyBll.CechyNadrzedne))
                                {
                                    if (trybPokazywaniaFiltrow == TrybPokazywaniaFiltrow.WymuszajSciezke)
                                    {
                                        usunCeche = true;
                                    }
                                    else if (!wspolneCechyNadrzedne.Contains(cechyBll.Id))
                                    {
                                        usunCeche = true;
                                    }
                                }
                                else
                                {
                                    usunCeche = false;
                                    break;
                                }
                            }
                        }
                        if (usunCeche)
                        {
                            listaCech.Remove(cechyBll);
                            if ((trybPokazywaniaFiltrow == TrybPokazywaniaFiltrow.WymuszajSciezke) && wybraneJuzWczesniejCechyWgAtrybutow.Any() && wybraneJuzWczesniejCechyWgAtrybutow.ContainsKey(cechyBll.AtrybutId.Value) &&
                                wybraneJuzWczesniejCechyWgAtrybutow[cechyBll.AtrybutId.Value].Contains(cechyBll.Id))
                            {
                                wybraneJuzWczesniejCechyWgAtrybutow[cechyBll.AtrybutId.Value].Remove(cechyBll.Id);
                            }
                        }
                    }

                    //byly zmiany - przepisanie listy cech
                    if (listaCech.Count != wynikowyAtrybutZCechamiDoWybrania.ListaCech.Count)
                    {
                        wynikowyAtrybutZCechamiDoWybrania = new AtrybutBll(wynikowyAtrybutZCechamiDoWybrania, listaCech);
                    }
                    if (cechynadrzednewgAtrybutu.ContainsKey(wynikowyAtrybutZCechamiDoWybrania.Id))
                    {
                        var listaCechNadrzednychDlaAtrybutu = cechynadrzednewgAtrybutu[wynikowyAtrybutZCechamiDoWybrania.Id];
                        wynikowyAtrybutZCechamiDoWybrania.ListaCech.RemoveAll(x => !listaCechNadrzednychDlaAtrybutu.Contains(x.Id));
                    }

                    //co to jest?! bartek
                    if (wynikowyAtrybutZCechamiDoWybrania.ZawszeWszystkieCechy)
                    {
                        var pelen = Calosc.DostepDane.PobierzPojedynczy<AtrybutBll>(wynikowyAtrybutZCechamiDoWybrania.Id, jezyk);
                        wynikowyAtrybutZCechamiDoWybrania = new AtrybutBll(pelen, pelen.ListaCech);
                    }
                    if (wynikowyAtrybutZCechamiDoWybrania.ListaCech.Any())
                    {
                        if ((wynikowyAtrybutZCechamiDoWybrania.ListaCech.Count == 1) && wynikowyAtrybutZCechamiDoWybrania.UkryjJednaWartosc &&
                            !wybraneJuzWczesniejCechyWgAtrybutow.ContainsKey(wynikowyAtrybutZCechamiDoWybrania.ListaCech.First().AtrybutId.Value))
                        {
                            //dodatkowa weryfikacja czy pokazac atrybut jesli jest oznaczony jako Nie pokazuj jak jedna wartosc
                            // jesli atrybut byl jawnie wybrany to ZAWSZE pokazujemy, jak nie byl jawnie wybrany to nie pokazujemy
                            continue;
                        }
                        wynikowaListaAtrybutow.Add(wynikowyAtrybutZCechamiDoWybrania);
                    }
                }
            }

            //dodajemy do filtrow wszystkie wybrane - zeby uzytkownik mogl sobie odklikać - musi być na końcu bo mogly byc wyrzucone wczesniej
            foreach (var atrybutWybrany in wybraneJuzWczesniejCechyWgAtrybutow)
            {
                var atrybut = wynikowaListaAtrybutow.FirstOrDefault(x => x.Id == atrybutWybrany.Key);
                if (atrybut == null)
                {
                    atrybut = new AtrybutBll(Calosc.DostepDane.PobierzPojedynczy<AtrybutBll>(atrybutWybrany.Key, jezyk), new List<CechyBll>(PobierzCechyOId(atrybutWybrany.Value, jezyk)));
                    if (trybPokazywaniaFiltrow == TrybPokazywaniaFiltrow.WszystkieAtrybutyJednoczesnie)
                    {
                        wynikowaListaAtrybutow.Add(atrybut);
                    }
                }
                else
                {
                    var listaCech = atrybut.ListaCech;
                    foreach (var cecha in atrybutWybrany.Value)
                    {
                        if (listaCech.All(x => x.Id != cecha))
                        {
                            var c = PobierzWszystkieCechy(jezyk)[cecha];
                            listaCech.Add(c);
                        }
                    }
                    atrybut = new AtrybutBll(atrybut, listaCech);
                }
            }
            return wynikowaListaAtrybutow;
        }


        //private void PobierzCechyDlaProduktu(HashSet<long> idCech, Dictionary<long,CechyBll>cechy , HashSet<long> dodaneJuz, HashSet<int> filtrAtrybutyIDZawezanie, bool jestFiltrowanie,ref Dictionary<int, HashSet<CechyBll>>cechyIAtrybuty)
        //{
        //    if (jestFiltrowanie)
        //    { //pomijam sprawdzenie czy posiada Atrybut bo każda cecha bedzie posiadała atrybut.
        //        foreach (var cechaId in idCech)
        //        {
        //            if (dodaneJuz.Contains(cechaId))
        //            {
        //                continue;
        //            }
        //            CechyBll cecha;
        //            if (cechy.TryGetValue(cechaId, out cecha))
        //            {
        //                int idAtrybutu = cecha.AtrybutId.Value;
        //                if (cecha.Widoczna && filtrAtrybutyIDZawezanie.Contains(idAtrybutu))
        //                {
        //                    HashSet<CechyBll> cechyAtrybutu;
        //                    if (cechyIAtrybuty.TryGetValue(idAtrybutu, out cechyAtrybutu))
        //                    {
        //                        cechyAtrybutu.Add(cecha);
        //                    }
        //                    else
        //                    {
        //                        cechyIAtrybuty.Add(idAtrybutu, new HashSet<CechyBll>() { cecha });
        //                    }
        //                }
        //            }
        //        }

                
        //    }
        //    else
        //    {
        //        //jezeli nie ma filtrowania to wyciagamy tylko i wyłącznie widoczne cechy.
        //        foreach (var cechaId in idCech)
        //        {
        //            if (dodaneJuz.Contains(cechaId))
        //            {
        //                continue;
        //            }
        //            CechyBll cecha;
        //            if (cechy.TryGetValue(cechaId, out cecha))
        //            {
        //                if (cecha.Widoczna)
        //                {
        //                    int idAtrybutu = cecha.AtrybutId.Value;
        //                    HashSet<CechyBll> cechyAtrybutu;
        //                    if (cechyIAtrybuty.TryGetValue(idAtrybutu, out cechyAtrybutu))
        //                    {
        //                        cechyAtrybutu.Add(cecha);
        //                    }
        //                    else
        //                    {
        //                        cechyIAtrybuty.Add(idAtrybutu, new HashSet<CechyBll>() { cecha });
        //                    }

        //                }
        //            }
        //        }
        //    }
        //}
        public static object lok = new object();

        private HashSet<long> PobierzIdCech(IList<ProduktKlienta> wybraneProdukty)
        {
            HashSet<long> cechyIds = new HashSet<long>();
            foreach (var prod in wybraneProdukty)
            {
                cechyIds.UnionWith(prod.IdCechPRoduktu);
            }
            return cechyIds;
        }

        /// <summary>
        /// Uwaga! Metoda zwraca KOPIE obiektu atrybutu - przeznaczone do zmiany
        /// </summary>
        /// <param name="wybraneProdukty"></param>
        /// <param name="jezykId"></param>
        /// <param name="klient"></param>
        /// <param name="kluczCacheListyProduktow">klucz BEZ JEZYKA!!!</param>
        /// <param name="FiltrAtrybutyIDZawezanie">filtry które nas interesują (wykorzystywane tylko dla pobierania stałych filtrów)</param>
        /// <returns></returns>
        public List<AtrybutBll> PobierzAtrybutyListyProduktow(IList<ProduktKlienta> wybraneProdukty, int jezykId, IKlient klient, string kluczCacheListyProduktow)
        {
            HashSet<long> cechyIds = new HashSet<long>();
            //jezeli jest filtrowanie tzn ze zapytanie idzie od stałych filtrów
            Dictionary<long, CechyBll> wszystkieCechy = PobierzWszystkieWidoczneCechy_FiltryNaLiscieProduktow(jezykId);
            if (kluczCacheListyProduktow == null)
            {
                cechyIds = PobierzIdCech(wybraneProdukty);
            }
            else
            {
                cechyIds = SolexBllCalosc.PobierzInstancje.Cache.PobierzObiekt<HashSet<long>>(kluczCacheListyProduktow);
                if (cechyIds == null)
                {
                    cechyIds = PobierzIdCech(wybraneProdukty);
                    SolexBllCalosc.PobierzInstancje.Cache.DodajObiekt(kluczCacheListyProduktow, cechyIds);
                }
            }

            IEnumerable<CechyBll> cechy = wszystkieCechy.WhereKeyIsIn(cechyIds); ;

            //musimy tu sorotwac cechy do atrybutu
            Dictionary<int, List<CechyBll>> idsa = cechy.GroupBy(x => x.AtrybutId).ToDictionary(x => x.Key.GetValueOrDefault(), x => x.Select(z => z).OrderBy(p => p.Kolejnosc.GetValueOrDefault()).ThenBy(p => p.Nazwa).ToList());
            var klucze = new HashSet<int>( idsa.Keys );
            IEnumerable<AtrybutBll> atrybutyBazowe = Calosc.DostepDane.Pobierz(jezykId, null, x => klucze.Contains(x.Id),
                new[] { new SortowanieKryteria<AtrybutBll>(x => x.Kolejnosc, KolejnoscSortowania.asc, "Kolejnosc"), new SortowanieKryteria<AtrybutBll>(x => x.Nazwa, KolejnoscSortowania.asc, "Nazwa") });
            var wynik = new List<AtrybutBll>();
            foreach (var a in atrybutyBazowe)
            {
                //nowy atrybut dlatgo ze pozniej mozemy modyfikowac kolekcje
                wynik.Add(new AtrybutBll(a, idsa[a.Id]));
            }
            return wynik;
        }

        public IList<AtrybutBll> BindingAtrybutyPoSelect(int jezykID, IKlient zajadacyKlient, IList<AtrybutBll> listaAtrybutow, object daneDoSelecta)
        {
            foreach (var atrybutBll in listaAtrybutow)
            {
                atrybutBll.PoleDoBudowyLinkow = (string.IsNullOrEmpty(atrybutBll.Symbol) ? (string.IsNullOrEmpty(atrybutBll.Nazwa) ? atrybutBll.Id.ToString(CultureInfo.InvariantCulture) : atrybutBll.Nazwa) : atrybutBll.Symbol).Trim();
                atrybutBll.PoleDoBudowyLinkow = Tools.OczyscCiagDoLinkuURL(atrybutBll.PoleDoBudowyLinkow);
            }
            return listaAtrybutow;
        }

        /// <summary>
        ///     Metoda polegajaca na budowaniu slownika filtrow na podstawie stringa, Key- nazwa atrybutu, Value - Hashset z
        ///     nazwami cech
        /// </summary>
        /// <param name="filtry"></param>
        /// <returns></returns>
        protected Dictionary<string, HashSet<string>> ZbudujSlownikFiltrow(string filtry)
        {
            var slownik = new Dictionary<string, HashSet<string>>();
            var strRegex = @"(?<nazwa>[^\[\]]{1,})\[(?<param>[^\[\]]{1,})\]";
            var myRegexOptions = RegexOptions.None;
            var myRegex = new Regex(strRegex, myRegexOptions);
            foreach (Match myMatch in myRegex.Matches(filtry))
            {
                if (myMatch.Success)
                {
                    var atrybut = myMatch.Groups["nazwa"].ToString().Trim();
                    var cechy = myMatch.Groups["param"].ToString();
                    var cechytab = cechy.Split(new[] {";"}, StringSplitOptions.RemoveEmptyEntries);
                    if (!slownik.ContainsKey(atrybut))
                    {
                        slownik.Add(atrybut, new HashSet<string>());
                    }
                    foreach (var s in cechytab)
                    {
                        slownik[atrybut].Add(s.Trim());
                    }
                }
            }
            return slownik;
        }

        protected List<AtrybutBll> _wszystkieAtrybutyWgJezykow = null;

        /// <summary>
        /// Metoda budująca słownik w oparciu o ciag znakowy, Key - atrybut, Value HashSet z nazwami cech
        /// </summary>
        /// <param name="filtry"></param>
        /// <returns></returns>
        public Dictionary<int, HashSet<long>> SlownikFiltrow(string filtry)
        {
            var wynik = new Dictionary<int, HashSet<long>>();
            if (string.IsNullOrWhiteSpace(filtry))
            {
                return wynik;
            }

            Dictionary<string, HashSet<string>> dane = ZbudujSlownikFiltrow(filtry);

                if (dane == null || dane.IsEmpty())
                {
                    return wynik;
                }

                if (_wszystkieAtrybutyWgJezykow == null)
                {
                    _wszystkieAtrybutyWgJezykow = new List<AtrybutBll>();
                    foreach (var j in Calosc.Konfiguracja.JezykiWSystemie)
                    {
                        _wszystkieAtrybutyWgJezykow.AddRange(Calosc.DostepDane.Pobierz<AtrybutBll>(j.Key, null));
                    }
                }

                foreach (var v in dane)
                {
                    foreach (var a in _wszystkieAtrybutyWgJezykow)
                    {
                        if (a.PoleDoBudowyLinkow.Equals(v.Key, StringComparison.InvariantCultureIgnoreCase))
                        {
                        //czy juz jest dodany - musimy to sprawdzac bo w roznych jezyakch moze byc taka sama nazwa atrybutu
                        //todo: podawac metodzie jezyk w jakim sprawdzamy
                            if (wynik.ContainsKey(a.Id))
                            {
                                continue;
                            }

                            HashSet<long> listaCech = new HashSet<long>();
                                foreach (var cecha in v.Value)
                                {
                                    foreach (var ca in a.ListaCech)
                                    {
                                        if (string.Compare(ca.PobierzWyswietlanaNazwe, cecha, StringComparison.InvariantCultureIgnoreCase) == 0)
                                        {
                                            //jest cecha o tej samej nazwie - dodamy do wyniku
                                            listaCech.Add(ca.Id);
                                            break;
                                        }
                                    }
                                }
                        wynik.Add(a.Id, listaCech);
                        break;
                        }
                    }
            }
            return wynik;
        }

        public void UsunCacheAtrybutyICechy(IList<object> obj)
        {
            Calosc.Cache.UsunGdzieKluczRozpoczynaSieOd(Calosc.DostepDane.KluczCacheTypDanych<Atrybut>());
            Calosc.Cache.UsunGdzieKluczRozpoczynaSieOd(Calosc.DostepDane.KluczCacheTypDanych<Atrybut>());
            _slownikCechJezyki = null;
            _slownikCechPoSymbolu = null;
            _wszystkieAtrybutyWgJezykow = null;
            _slownikIdAtrybutowICech = null;
        }

        protected Dictionary<int, Dictionary<long, CechyBll>> _slownikCechJezyki;
        private Dictionary<string, long> _slownikCechPoSymbolu;

        private Dictionary<int, long[]> _slownikIdAtrybutowICech;
        /// <summary>
        /// Słownik Id atrybutów oraz id cech przypisanych do nich. 
        /// </summary>
        public Dictionary<int, long[]> SlownikIdAtrybutowIIdCech
        {
            get
            {
                if (_slownikIdAtrybutowICech == null)
                {
                    _slownikIdAtrybutowICech = PobierzWszystkieCechy(Calosc.Konfiguracja.JezykIDDomyslny).GroupBy(x => x.Value.AtrybutId.GetValueOrDefault())
                    .ToDictionary(x => x.Key, x => x.Select(z => z.Value).OrderBy(p => p.Kolejnosc.GetValueOrDefault()).ThenBy(p => p.Nazwa).Select(y => y.Id).ToArray());
                }
                return _slownikIdAtrybutowICech;
            }
        }

        protected ConcurrentDictionary<int, Dictionary<long, CechyBll>> _slownikCechJezyki_FiltryListaProdutkow;

        /// <summary>
        /// Pobiera aktualne cechy które są pokazywane na liscie produktów jako filtry - widoczny atrybut i cecha
        /// </summary>
        /// <param name="lang"></param>
        /// <param name="czyWszystkieWidoczne">Określa czy mamy zastosować warunek widoczności na liscie produktów - parametr potrzebny ze względu na stałe filtry</param>
        /// <returns></returns>
        public Dictionary<long, CechyBll> PobierzWszystkieWidoczneCechy_FiltryNaLiscieProduktow(int lang)
        {
            if (_slownikCechJezyki_FiltryListaProdutkow == null || !_slownikCechJezyki_FiltryListaProdutkow.ContainsKey(lang))
            {
                if (_slownikCechJezyki_FiltryListaProdutkow == null)
                {
                    _slownikCechJezyki_FiltryListaProdutkow = new ConcurrentDictionary<int, Dictionary<long, CechyBll>>();
                }
                _slownikCechJezyki_FiltryListaProdutkow.TryAdd(lang, PobierzWszystkieCechy(lang).Values.Where(x=>x.Widoczna && x.AtrybutId.HasValue && x.PobierzAtrybut().PokazujNaLiscieProduktow && x.PobierzAtrybut().Widoczny).ToDictionary(x=>x.Id,x=>x)); 
            }
            return _slownikCechJezyki_FiltryListaProdutkow[lang];
        }

        public Dictionary<long, CechyBll> PobierzWszystkieCechy(int lang)
        {
            if ((_slownikCechJezyki == null) || !_slownikCechJezyki.ContainsKey(lang))
            {
                if (_slownikCechJezyki == null)
                {
                    _slownikCechJezyki = new Dictionary<int, Dictionary<long, CechyBll>>();
                }
                _slownikCechJezyki.Add(lang, new Dictionary<long, CechyBll>());
                var cechy = Calosc.DostepDane.Pobierz<Cecha>(lang, null);
                if (cechy == null)
                {
                    _slownikCechJezyki[lang] = null;
                }
                else
                {
                    _slownikCechJezyki[lang] = cechy.Select(x => new CechyBll(x, lang)).ToDictionary(x => x.Id, x => x);
                }
            }
            return _slownikCechJezyki[lang];
        }

        public CechyBll PobierzCecheOSymbolu(string symbol, int lang)
        {
            if (_slownikCechPoSymbolu == null)
            {
                _slownikCechPoSymbolu = PobierzWszystkieCechy(lang).Values.ToDictionary(x => x.Symbol.ToLower(), x => x.Id);
            }
            try
            {
                var idCechy = _slownikCechPoSymbolu[symbol.ToLower()];
                return PobierzWszystkieCechy(lang)[idCechy];
            } catch (KeyNotFoundException)
            {
                return null;
            }
        }
        /// <summary>
        /// Pobieramy listę cech o id podamym w parametrach. 
        /// </summary>
        /// <param name="idCech"></param>
        /// <param name="langId"></param>
        /// <returns></returns>
        public HashSet<CechyBll> PobierzCechyOId(HashSet<long> idCech, int langId)
        {
            List<CechyBll> temp = PobierzWszystkieCechy(langId).WhereKeyIsIn(idCech);
            return new HashSet<CechyBll>(temp);
        }

        /// <summary>
        /// Metoda ZACHOWUJE kolejność cech w atrybucie
        /// </summary>
        /// <param name="atrybutId"></param>
        /// <param name="lang"></param>
        /// <returns></returns>
        public List<CechyBll> PobierzCecheDlaAtrybutu(int atrybutId, int lang)
        {
           try
            {
                var aId = SlownikIdAtrybutowIIdCech[atrybutId];
                if (aId != null)
                {
                    var wynik = new List<CechyBll>(aId.Length);
                    foreach (var c in aId)
                    {
                        wynik.Add(PobierzWszystkieCechy(lang)[c]);
                    }
                    return wynik;
                }
            } catch (KeyNotFoundException)
            {
                //brak klucza
            }
            return null;
        }

        public List<Cecha> AktualizujLubZapiszCechy(List<Cecha> data)
        {
            if (data.Any())
            {
                foreach (var cechy in data)
                {
                    cechy.Symbol = cechy.Symbol.ToLower();
                    if (cechy.JezykId == 0)
                    {
                        cechy.JezykId = SolexBllCalosc.PobierzInstancje.Konfiguracja.JezykIDPolski;
                    }
                }
                Calosc.DostepDane.AktualizujListe(data);
            }
            return data;
        }
        /// <summary>
        /// prywatna metoda sprawdzjąca czy automatyczne atrybuty są dodane jak tak to je zwraca w przeciwnym wypadku je dodaje
        /// </summary>
        /// <returns></returns>
        protected Dictionary<string,AtrybutBll> WygenerujAtrybuty(string symbolAtrybutuMojKatalog, string symbolAtrybutuAkcesorium, string symbolAtrybutuOfertaSpecjalna, string symbolAtrybutuUlubione, string symbolAtrybutuGradacja, int jezyk)
        {
            
            //tworzymy podstawowe atrybuty
            List<string> listaSymboliAtrbutow = new List<string>() { symbolAtrybutuMojKatalog, symbolAtrybutuAkcesorium, symbolAtrybutuOfertaSpecjalna, symbolAtrybutuUlubione, symbolAtrybutuGradacja };

            //Pobieramy atrybuty które już są na platformie
            Dictionary<string, AtrybutBll> listaAtrybutow = Calosc.DostepDane.Pobierz<AtrybutBll>(null, x => Sql.In(x.Symbol, listaSymboliAtrbutow)).ToDictionary(x=>x.Symbol,x=>x);
            if (listaAtrybutow.Count != listaSymboliAtrbutow.Count)
            {
                //Tworzymy automatyczne atrybuty
                AtrybutBll atrybutMojKatalog = new AtrybutBll(new Atrybut("Mój katalog") { Symbol = symbolAtrybutuMojKatalog, Kolejnosc = -9999, PokazujNaLiscieProduktow = true, Widoczny = true, PokazujWWyszukiwaniu = true, JezykId = jezyk }, null);
                AtrybutBll atrybutAkcesorium = new AtrybutBll(new Atrybut("Akcesoria") { Symbol = symbolAtrybutuAkcesorium, Kolejnosc = -9999, PokazujNaLiscieProduktow = true, Widoczny = true, PokazujWWyszukiwaniu = true, JezykId = jezyk }, null);
                AtrybutBll atrybutOfertaSpecjalna = new AtrybutBll(new Atrybut("Oferta specjalna") { Symbol = symbolAtrybutuOfertaSpecjalna, Kolejnosc = -9999, PokazujNaLiscieProduktow = true, Widoczny = true, PokazujWWyszukiwaniu = true, JezykId = jezyk }, null);
                AtrybutBll atrybutUlubione = new AtrybutBll(new Atrybut("Ulubione") { Symbol = symbolAtrybutuUlubione, Kolejnosc = -9999, PokazujNaLiscieProduktow = true, Widoczny = true, PokazujWWyszukiwaniu = true, JezykId = jezyk }, null);
                AtrybutBll atrybutGradacja = new AtrybutBll(new Atrybut("Gradacja") { Symbol = symbolAtrybutuGradacja, Kolejnosc = -9999, PokazujNaLiscieProduktow = true, Widoczny = true, PokazujWWyszukiwaniu = true, JezykId = jezyk }, null);

                Dictionary<string,AtrybutBll> listaAutomatycznychAtrybutow = new Dictionary<string, AtrybutBll>() { { symbolAtrybutuMojKatalog,atrybutMojKatalog } , { symbolAtrybutuAkcesorium, atrybutAkcesorium }, { symbolAtrybutuOfertaSpecjalna, atrybutOfertaSpecjalna }, { symbolAtrybutuUlubione,atrybutUlubione }, { symbolAtrybutuGradacja, atrybutGradacja } };

                //Jeżeli nie ma ani jednego dodajemy wszystkie
                if (listaAtrybutow.Count != 0)
                {
                    //jeżeli jakies były to dodajemy tylko te których brakuje
                    foreach (var s in listaSymboliAtrbutow)
                    {
                        if (listaAtrybutow.ContainsKey(s))
                        {
                            listaAutomatycznychAtrybutow.Remove(s);
                        }
                    }
                }
                Calosc.DostepDane.AktualizujListe(listaAutomatycznychAtrybutow.Values.ToList());
                //laczymy liste nowo dodanych elementow z juz istniejacymi - dopiero tutaj gdyż po aktualizacji mamy juz id dodanych atrybutów
                listaAtrybutow.AddRange(listaAutomatycznychAtrybutow);
            }
            return listaAtrybutow;
        }

        private ConcurrentDictionary<TypStanu, string> _slownikTypow;
       
        public string PobierzPrzyjaznaNazweDlaTypuStanu(TypStanu typ)
        {           
            if (_slownikTypow.TryGetValue(typ, out string result))
            {
                return result;
            }

            throw new Exception($"Nie można pobrać typu stanu: {typ} z kolekcji: {string.Join(",", _slownikTypow)}");
        }

        /// <summary>
        /// Metoda odpowiedzialna za generowanie automatycznych cech oraz atrybutów
        /// </summary>
        public void GenerujStandardoweCechy()
        {
            var jezyk = Calosc.Konfiguracja.JezykIDDomyslny;

            int jezykIdPolski = Calosc.Konfiguracja.JezykIDPolski;

            //tworzymy podstawowe atrybuty
            List<string> listaSymboliAtrbutow = new List<string>();

            //atrybut dla moj katalog
            string symbolAtrybutuMojKatalog = "automatyczne_MojKatalog";
            listaSymboliAtrbutow.Add(symbolAtrybutuMojKatalog);


            //atrybut dla akcesorium
            string symbolAtrybutuAkcesorium = "automatyczne_Akcesoria";
            listaSymboliAtrbutow.Add(symbolAtrybutuAkcesorium);


            //atrybut dla ofert specjalnych
            string symbolAtrybutuOfertaSpecjalna = "automatyczne_OfertaSpecjalna";
            listaSymboliAtrbutow.Add(symbolAtrybutuOfertaSpecjalna);


            //atrybut dla ulubionych
            string symbolAtrybutuUlubione = "automatyczne_Ulubione";
            listaSymboliAtrbutow.Add(symbolAtrybutuUlubione);


            //atrybut dla gradacji
            string symbolAtrybutuGradacja = "automatyczne_Gradacja";
            listaSymboliAtrbutow.Add(symbolAtrybutuGradacja);

            Dictionary<string,AtrybutBll> atrybuty = WygenerujAtrybuty(symbolAtrybutuMojKatalog, symbolAtrybutuAkcesorium, symbolAtrybutuOfertaSpecjalna, symbolAtrybutuUlubione, symbolAtrybutuGradacja, jezyk);

            List<Cecha> cechyDoAktualizacji = new List<Cecha>();

            Cecha c;

            //tworzenie cechy Moj katalog
            var symbol = Calosc.Konfiguracja.SymbolCechyMojKatalog;
            if (!string.IsNullOrEmpty(symbol))
            {
                c = UtworzCeche(symbol, jezykIdPolski, "Mój katalog", "<span class=\"label label-success\">Mój katalog</span>", "btn-success-outline", MetkaPozycjaLista.ZaNazwaDoPrawej, atrybuty[symbolAtrybutuMojKatalog].Id);
                cechyDoAktualizacji.Add(c);
            }
            var symbolAkcesoria = Calosc.Konfiguracja.SymbolCechyAkcesoria;
            if (!string.IsNullOrEmpty(symbolAkcesoria))
            {
                c = UtworzCeche(symbolAkcesoria, jezykIdPolski, "Akcesorium", "<span class=\"label label-primary\">Akcesorium</span>", "btn-info-outline", MetkaPozycjaLista.ZaNazwaDoPrawej, atrybuty[symbolAtrybutuAkcesorium].Id);
                cechyDoAktualizacji.Add(c);
            }
            var symbolPrZOfe = Calosc.Konfiguracja.SymbolCechyProduktZOferty;
            if (!string.IsNullOrEmpty(symbolPrZOfe))
            {
                c = UtworzCeche(symbolPrZOfe, jezykIdPolski, "Moje oferty specjalne", "<span class=\"label label-danger\">Oferta specjalna</span>", "btn-danger-outline", MetkaPozycjaLista.ZaNazwaDoPrawej, atrybuty[symbolAtrybutuOfertaSpecjalna].Id);
                cechyDoAktualizacji.Add(c);
            }
            var symbolUlubioneCecha = Calosc.Konfiguracja.SymbolCechyUlubione;
            if (!string.IsNullOrEmpty(symbolUlubioneCecha))
            {
                c = UtworzCeche(symbolUlubioneCecha, jezykIdPolski, "Ulubione", null, "btn-ulubione", MetkaPozycjaLista.ZaNazwaDoPrawej, atrybuty[symbolAtrybutuUlubione].Id);
                cechyDoAktualizacji.Add(c);
            }
            var symbolGradacjaCecha = Calosc.Konfiguracja.SymbolCechyGradacja;
            if (!string.IsNullOrEmpty(symbolGradacjaCecha))
            {
                c = UtworzCeche(symbolGradacjaCecha, jezykIdPolski, "Gradacja", "<span class=\"label label-success\">Gradacja</span>", "btn-success-outline", MetkaPozycjaLista.ZaNazwaDoPrawej, atrybuty[symbolAtrybutuGradacja].Id);
                cechyDoAktualizacji.Add(c);
            }

            // Typ stanu
            var atrTypStanu = Calosc.Konfiguracja.SymbolAtrybutCechyTypStanu;
            var ts = Calosc.DostepDane.PobierzPojedynczy<AtrybutBll>(x => x.Symbol == atrTypStanu, null);
            if (ts == null)
            {
                var atrTs = new AtrybutBll(new Atrybut(atrTypStanu) { Symbol = atrTypStanu }, null);
                atrTs.JezykId = jezyk;
                Calosc.DostepDane.AktualizujPojedynczy(atrTs);
                ts = atrTs; //Calosc.DostepDane.PobierzPojedynczy<AtrybutBll>(x => x.Symbol == atrTypStanu, null);
            }
            foreach (Enum cts in Enum.GetValues(typeof(TypStanu)))
            {
                string friendlyName = cts.PobierzAtrybutDlaEnuma<FriendlyNameAttribute>().FriendlyName;
                var symbolcvechy = atrTypStanu + ":" + friendlyName;
                c = UtworzCeche(symbolcvechy, jezykIdPolski, friendlyName, null, null, MetkaPozycjaLista.Brak, ts.Id);
                cechyDoAktualizacji.Add(c);
            }
            AktualizujLubZapiszCechy(cechyDoAktualizacji);
        }

        private Cecha UtworzCeche(string symbol, int jezykPolski, string nazwaCechy, string metkaOpis, string cssKlasy, MetkaPozycjaLista pozycja, int idAtrybutu)
        {
            Cecha c = PobierzCecheOSymbolu(symbol, jezykPolski);
            if (c == null)
            {
                c = new Cecha(nazwaCechy, symbol);
                c.MetkaOpis = metkaOpis;
                c.CssKlasy = cssKlasy;
                c.MetkaPozycjaLista = pozycja;
            }
            c.AtrybutId = idAtrybutu;
            return c;
        }

        public List<CechyBll> PobierzMetkiLista(MetkaPozycjaLista pozycja, HashSet<long> idCech, int jezykId, bool zalogowany)
        {
            return PobierzListeMetek(pozycja, idCech, jezykId, zalogowany);
        }

        public List<CechyBll> PobierzMetkaPozycjaKoszykGratisy(MetkaPozycjaKoszykGratisy pozycja, HashSet<long> idCech, int jezykId, bool zalogowany)
        {
            return PobierzListeMetek(pozycja, idCech, jezykId, zalogowany);
        }

        public List<CechyBll> PobierzMetkaPozycjaKoszykGratisyPopUp(MetkaPozycjaKoszykGratisyPopUp pozycja, HashSet<long> idCech, int jezykId, bool zalogowany)
        {
            return PobierzListeMetek(pozycja, idCech, jezykId, zalogowany);
        }

        public List<CechyBll> PobierzMetkaPozycjaKoszykAutomatyczne(MetkaPozycjaKoszykAutomatyczne pozycja, HashSet<long> idCech, int jezykId, bool zalogowany)
        {
            return PobierzListeMetek(pozycja, idCech, jezykId, zalogowany);
        }

        public List<CechyBll> PobierzMetkiRodzina(MetkaPozycjaRodziny pozycja, HashSet<long> idCech, int jezykId, bool zalogowany)
        {
            return PobierzListeMetek(pozycja, idCech, jezykId, zalogowany);
        }

        public List<CechyBll> PobierzMetkiSzczegoly(MetkaPozycjaSzczegoly pozycja, HashSet<long> idCech, int jezykId, bool zalogowany)
        {
            return PobierzListeMetek(pozycja, idCech, jezykId, zalogowany);
        }

        public List<CechyBll> PobierzMetkaPozycjaKoszykProdukty(MetkaPozycjaKoszykProdukty pozycja, HashSet<long> idCech, int jezykId, bool zalogowany)
        {
            return PobierzListeMetek(pozycja, idCech, jezykId, zalogowany);
        }

        public List<CechyBll> PobierzMetkiSzczegolyWarianty(MetkaPozycjaSzczegolyWarianty pozycja, HashSet<long> idCech, int jezykId, bool zalogowany)
        {
            return PobierzListeMetek(pozycja, idCech, jezykId, zalogowany);
        }

        public List<CechyBll> PobierzMetkiKafleProduktu(MetkaPozycjaKafle pozycja, HashSet<long> idCech, int jezykId, bool zalogowany)
        {
            return PobierzListeMetek(pozycja, idCech, jezykId, zalogowany);
        }

        private Dictionary<int, Dictionary<int, AtrybutBll>> _slownikAtrybutowZMetkamiDlaJezyka;

        private Dictionary<int, AtrybutBll> PobierzWszystkieAtrybutyZMetkami(int lang)
        {
            if(_slownikAtrybutowZMetkamiDlaJezyka != null && _slownikAtrybutowZMetkamiDlaJezyka.TryGetValue(lang, out Dictionary<int, AtrybutBll> _atrybuty)) {
                return _atrybuty;
            }

            //jak nie ma to lok zeby nie bylo dubli
            return LockHelper.PobierzInstancje.UruchomKodWLocku_BezUzywaniaCache($"_slownikAtrybutowZMetkamiDlaJezyka_{lang}", () =>
            {
                if (_slownikAtrybutowZMetkamiDlaJezyka == null)
                {
                    _slownikAtrybutowZMetkamiDlaJezyka = new Dictionary<int, Dictionary<int, AtrybutBll>>();
                }

                if (!_slownikAtrybutowZMetkamiDlaJezyka.ContainsKey(lang))
                {
                    _slownikAtrybutowZMetkamiDlaJezyka.Add(lang, new Dictionary<int, AtrybutBll>());
                    _slownikAtrybutowZMetkamiDlaJezyka[lang] = Calosc.DostepDane.Pobierz<AtrybutBll>(lang, null, x => (!string.IsNullOrEmpty(x.UniwersalnaMetkaOpis) || !string.IsNullOrEmpty(x.UniwersalnaMetkaKatalog)) && x.Widoczny).ToDictionary(x => x.Id, x => x);
                }
                return _slownikAtrybutowZMetkamiDlaJezyka[lang];
            });
        }

        //Kluczem jest nazwa enuma, wartosc - slownik gdzie klucz to pozycja z enuma oraz jakie cechy do tego pasuja, protected zeby mozna było sobie w łatwy sposob napisaćtesty jednostkowe
        protected Dictionary<string, Dictionary<string, HashSet<long>>> _slownikMetekNiezalogowani;
        protected Dictionary<string, Dictionary<string, HashSet<long>>> _slownikMetekZalogowani;
        protected Dictionary<int,AtrybutBll> _kolekcjaAtrbutowZMetka;
        public void UstawSlownikiMetek()
        {
            _slownikMetekNiezalogowani = new Dictionary<string, Dictionary<string, HashSet<long>>>();
            _slownikMetekZalogowani = new Dictionary<string, Dictionary<string, HashSet<long>>>();

            _kolekcjaAtrbutowZMetka = Calosc.DostepDane.Pobierz<AtrybutBll>(null, x => (!string.IsNullOrEmpty(x.UniwersalnaMetkaOpis) || !string.IsNullOrEmpty(x.UniwersalnaMetkaKatalog)) && x.Widoczny).ToDictionary(x=>x.Id,x=>x);
            //Dictionary<int, string> atrybutyZUniwersalnaMetka = Calosc.DostepDane.Pobierz<AtrybutBll>(null, x => !string.IsNullOrEmpty(x.UniwersalnaMetkaOpis)).ToDictionary(x=>x.Id, x=>x.UniwersalnaMetkaOpis);

            var cechyZMetkami = Calosc.DostepDane.Pobierz<CechyBll>(null, x => (x.MetkaOpis!=null && x.MetkaOpis != "") || (x.MetkaKatalog!=null & x.MetkaKatalog != "") || (_kolekcjaAtrbutowZMetka!=null && _kolekcjaAtrbutowZMetka.Any() && Sql.In(x.AtrybutId, _kolekcjaAtrbutowZMetka.Keys)));
            foreach (var cecha in cechyZMetkami)
            {
                //Dodajemy ceche w sytuacji gdy ma wypełnioną metkę dla zalogowanych bądz atrybut posiada uniwersalna metkę
                if (!string.IsNullOrEmpty(cecha.MetkaOpis))
                {
                    DodajCecheDoSlownikaMetek(cecha, ref _slownikMetekZalogowani);
                }
                else if (cecha.AtrybutId.HasValue && _kolekcjaAtrbutowZMetka.ContainsKey(cecha.AtrybutId.Value) && !string.IsNullOrEmpty(_kolekcjaAtrbutowZMetka[cecha.AtrybutId.Value].UniwersalnaMetkaOpis))
                {
                    CechyBll c = UstawUniwersalnaMetkeNaCeche(cecha, _kolekcjaAtrbutowZMetka[cecha.AtrybutId.Value],true);
                    DodajCecheDoSlownikaMetek(c, ref _slownikMetekZalogowani);
                }
                //Dodajemy ceche w sytuacji gdy ma wypełnioną metkę dla niezalogowanych bądz atrybut posiada uniwersalna metkę
                if (!string.IsNullOrEmpty(cecha.MetkaKatalog))
                {
                    DodajCecheDoSlownikaMetek(cecha, ref _slownikMetekNiezalogowani);
                }
                else if (cecha.AtrybutId.HasValue && _kolekcjaAtrbutowZMetka.ContainsKey(cecha.AtrybutId.Value) && !string.IsNullOrEmpty(_kolekcjaAtrbutowZMetka[cecha.AtrybutId.Value].UniwersalnaMetkaKatalog))
                {
                    CechyBll c = UstawUniwersalnaMetkeNaCeche(cecha, _kolekcjaAtrbutowZMetka[cecha.AtrybutId.Value], false);
                    DodajCecheDoSlownikaMetek(c, ref _slownikMetekNiezalogowani);
                }
            }
        }

        ///// <summary>
        ///// Metoda sprawdzjaca czy w slowniku jest klucz z enumem oraz podslowniku klucz z pozycja jezeli nie to dodaje klucz oraz wartosc - idCechy
        ///// </summary>
        ///// <param name="slownik"></param>
        ///// <param name="cecha"></param>
        ///// <param name="nazwaEnuma"></param>
        //private void DodajWartosciDoSlownikaMetki(ref Dictionary<string, Dictionary<string, HashSet<long>>> slownik, CechyBll cecha, string nazwaEnuma)
        //{
        //    var prop = cecha.GetType().GetProperty(nazwaEnuma);
        //    string pozycja = prop.GetValue(cecha).ToString();
        //    if (!slownik.ContainsKey(nazwaEnuma))
        //    {
        //        slownik.Add(nazwaEnuma, new Dictionary<string, HashSet<long>>());
        //    }
        //    if (!slownik[nazwaEnuma].ContainsKey(pozycja))
        //    {
        //        slownik[nazwaEnuma].Add(pozycja, new HashSet<long>());
        //    }
        //    slownik[nazwaEnuma][pozycja].Add(cecha.Id);
        //}

        /// <summary>
        /// Dodaje ceche do slownika metek względem pozycji
        /// </summary>
        /// <param name="cecha"></param>
        /// <param name="slownik"></param>
        private void DodajCecheDoSlownikaMetek(CechyBll cecha, ref Dictionary<string, Dictionary<string, HashSet<long>>> slownik)
        {
            if (cecha.MetkaPozycjaRodziny != MetkaPozycjaRodziny.Brak)
            {
                string pozycja = cecha.MetkaPozycjaRodziny.ToString();
                string nazwaEnuma = typeof(MetkaPozycjaRodziny).Name;
                if (!slownik.ContainsKey(nazwaEnuma))
                {
                    slownik.Add(nazwaEnuma, new Dictionary<string, HashSet<long>>());
                }
                if (!slownik[nazwaEnuma].ContainsKey(pozycja))
                {
                    slownik[nazwaEnuma].Add(pozycja, new HashSet<long>());
                }
                slownik[nazwaEnuma][pozycja].Add(cecha.Id);
            }
            if (cecha.MetkaPozycjaKoszykAutomatyczne != MetkaPozycjaKoszykAutomatyczne.Brak)
            {
                string pozycja = cecha.MetkaPozycjaKoszykAutomatyczne.ToString();
                string nazwaEnuma = typeof(MetkaPozycjaKoszykAutomatyczne).Name;
                if (!slownik.ContainsKey(nazwaEnuma))
                {
                    slownik.Add(nazwaEnuma, new Dictionary<string, HashSet<long>>());
                }
                if (!slownik[nazwaEnuma].ContainsKey(pozycja))
                {
                    slownik[nazwaEnuma].Add(pozycja, new HashSet<long>());
                }
                slownik[nazwaEnuma][pozycja].Add(cecha.Id);
            }
            if (cecha.MetkaPozycjaKafle != MetkaPozycjaKafle.Brak)
            {
                string pozycja = cecha.MetkaPozycjaKafle.ToString();
                string nazwaEnuma = typeof(MetkaPozycjaKafle).Name;
                if (!slownik.ContainsKey(nazwaEnuma))
                {
                    slownik.Add(nazwaEnuma, new Dictionary<string, HashSet<long>>());
                }
                if (!slownik[nazwaEnuma].ContainsKey(pozycja))
                {
                    slownik[nazwaEnuma].Add(pozycja, new HashSet<long>());
                }
                slownik[nazwaEnuma][pozycja].Add(cecha.Id);
            }
            if (cecha.MetkaPozycjaKoszykGratisy != MetkaPozycjaKoszykGratisy.Brak)
            {
                string pozycja = cecha.MetkaPozycjaKoszykGratisy.ToString();
                string nazwaEnuma = typeof(MetkaPozycjaKoszykGratisy).Name;
                if (!slownik.ContainsKey(nazwaEnuma))
                {
                    slownik.Add(nazwaEnuma, new Dictionary<string, HashSet<long>>());
                }
                if (!slownik[nazwaEnuma].ContainsKey(pozycja))
                {
                    slownik[nazwaEnuma].Add(pozycja, new HashSet<long>());
                }
                slownik[nazwaEnuma][pozycja].Add(cecha.Id);
            }
            if (cecha.MetkaPozycjaKoszykGratisyPopUp != MetkaPozycjaKoszykGratisyPopUp.Brak)
            {
                string pozycja = cecha.MetkaPozycjaKoszykGratisyPopUp.ToString();
                string nazwaEnuma = typeof(MetkaPozycjaKoszykGratisyPopUp).Name;
                if (!slownik.ContainsKey(nazwaEnuma))
                {
                    slownik.Add(nazwaEnuma, new Dictionary<string, HashSet<long>>());
                }
                if (!slownik[nazwaEnuma].ContainsKey(pozycja))
                {
                    slownik[nazwaEnuma].Add(pozycja, new HashSet<long>());
                }
                slownik[nazwaEnuma][pozycja].Add(cecha.Id);
            }
            if (cecha.MetkaPozycjaLista != MetkaPozycjaLista.Brak)
            {
                string pozycja = cecha.MetkaPozycjaLista.ToString();
                string nazwaEnuma = typeof(MetkaPozycjaLista).Name;
                if (!slownik.ContainsKey(nazwaEnuma))
                {
                    slownik.Add(nazwaEnuma, new Dictionary<string, HashSet<long>>());
                }
                if (!slownik[nazwaEnuma].ContainsKey(pozycja))
                {
                    slownik[nazwaEnuma].Add(pozycja, new HashSet<long>());
                }
                slownik[nazwaEnuma][pozycja].Add(cecha.Id);
            }
            if (cecha.MetkaPozycjaSzczegoly != MetkaPozycjaSzczegoly.Brak)
            {
                string pozycja = cecha.MetkaPozycjaSzczegoly.ToString();
                string nazwaEnuma = typeof(MetkaPozycjaSzczegoly).Name;
                if (!slownik.ContainsKey(nazwaEnuma))
                {
                    slownik.Add(nazwaEnuma, new Dictionary<string, HashSet<long>>());
                }
                if (!slownik[nazwaEnuma].ContainsKey(pozycja))
                {
                    slownik[nazwaEnuma].Add(pozycja, new HashSet<long>());
                }
                slownik[nazwaEnuma][pozycja].Add(cecha.Id);
            }
            if (cecha.MetkaPozycjaSzczegolyWarianty != MetkaPozycjaSzczegolyWarianty.Brak)
            {
                string pozycja = cecha.MetkaPozycjaSzczegolyWarianty.ToString();
                string nazwaEnuma = typeof(MetkaPozycjaSzczegolyWarianty).Name;
                if (!slownik.ContainsKey(nazwaEnuma))
                {
                    slownik.Add(nazwaEnuma, new Dictionary<string, HashSet<long>>());
                }
                if (!slownik[nazwaEnuma].ContainsKey(pozycja))
                {
                    slownik[nazwaEnuma].Add(pozycja, new HashSet<long>());
                }
                slownik[nazwaEnuma][pozycja].Add(cecha.Id);
            }
            if (cecha.MetkaPozycjaKoszykProdukty != MetkaPozycjaKoszykProdukty.Brak)
            {
                string pozycja = cecha.MetkaPozycjaKoszykProdukty.ToString();
                string nazwaEnuma = typeof(MetkaPozycjaKoszykProdukty).Name;
                if (!slownik.ContainsKey(nazwaEnuma))
                {
                    slownik.Add(nazwaEnuma, new Dictionary<string, HashSet<long>>());
                }
                if (!slownik[nazwaEnuma].ContainsKey(pozycja))
                {
                    slownik[nazwaEnuma].Add(pozycja, new HashSet<long>());
                }
                slownik[nazwaEnuma][pozycja].Add(cecha.Id);
            }
        }

        protected virtual CechyBll UstawPozycjeZAtrybutu(CechyBll cecha, AtrybutBll atrybut, bool zalogowany)
        {
            var c = new CechyBll(cecha,cecha.JezykId);
            c.MetkaPozycjaKoszykAutomatyczne = atrybut.MetkaPozycjaKoszykAutomatyczne;
            c.MetkaPozycjaLista = atrybut.MetkaPozycjaLista;
            c.MetkaPozycjaKoszykProdukty = atrybut.MetkaPozycjaKoszykProdukty;
            c.MetkaPozycjaSzczegoly = atrybut.MetkaPozycjaSzczegoly;
            c.MetkaPozycjaRodziny = atrybut.MetkaPozycjaRodziny;
            c.MetkaPozycjaKoszykGratisyPopUp = atrybut.MetkaPozycjaKoszykGratisyPopUp;
            c.MetkaPozycjaKafle = atrybut.MetkaPozycjaKafle;
            c.MetkaPozycjaKoszykGratisy = atrybut.MetkaPozycjaKoszykGratisy;
            //dodajemy kolejność zarówno atruutu jak i kolejność cech. 10275
            c.Kolejnosc = atrybut.Kolejnosc*100 +(cecha.Kolejnosc?? 0);
            return c;
        }

        private CechyBll UstawUniwersalnaMetkeNaCeche(CechyBll cecha, AtrybutBll atrybut, bool zalogowany)
        {
            var c = UstawPozycjeZAtrybutu(cecha, atrybut,zalogowany);
            if (zalogowany)
            {
                c.MetkaOpis = string.Format(atrybut.UniwersalnaMetkaOpis,cecha.Nazwa);
            }
            else
            {
                c.MetkaKatalog = string.Format(atrybut.UniwersalnaMetkaKatalog, cecha.Nazwa);
            }
            return c;
        }

        protected List<CechyBll> PobierzListeMetek<T>(T pozycja, HashSet<long> idCech, int jezykId, bool zalogowany)
        {
            Dictionary<string, Dictionary<string, HashSet<long>>> slownikMetek = zalogowany ? _slownikMetekZalogowani : _slownikMetekNiezalogowani;
            Type typPozycji = typeof(T);
            //W slowniku nie ma metek, brak metek dla konkretnego enuma (lista produktów, kafle, etc), brak metek na konkretnej pozycji zwracamy nulla
            if (slownikMetek == null || !slownikMetek.Any() || !slownikMetek.ContainsKey(typPozycji.Name) || !slownikMetek[typPozycji.Name].ContainsKey(pozycja.ToString()))
            {
                return null;
            }
            HashSet<long> idCechZMetkami = new HashSet<long>( slownikMetek[typPozycji.Name][pozycja.ToString()].Intersect(idCech) );

            //Brak metek dla wybranych cech
            if (idCechZMetkami == null || !idCechZMetkami.Any())
            {
                return null;
            }

            //Pobieramy cechy dla konkretnego jezyka
            Dictionary<long, CechyBll> slownikCech = PobierzCechyOId(idCechZMetkami, jezykId).ToDictionary(x => x.Id, x => x);

            //Wyciagamy cechy ktore posiadaja atrybut z uniwersalna metka
            var idCechZAtrubutemZMetka = slownikCech.Where(x => x.Value.AtrybutId.HasValue && _kolekcjaAtrbutowZMetka.ContainsKey(x.Value.AtrybutId.Value)).ToDictionary(x => x.Key, x => x.Value.AtrybutId);

            Dictionary<int, AtrybutBll> atrybutyZMetkami = PobierzWszystkieAtrybutyZMetkami(jezykId);

            foreach (var cechaId in idCechZAtrubutemZMetka)
            {
                if (zalogowany && string.IsNullOrEmpty(slownikCech[cechaId.Key].MetkaOpis))
                {
                    //nie trzeba robić sprawdzenia czy posiada ceche gdyż przy pobieraniu idCechZAtrubutemZMetka było sprawdzeni
                    slownikCech[cechaId.Key] = UstawUniwersalnaMetkeNaCeche(slownikCech[cechaId.Key], atrybutyZMetkami[cechaId.Value.Value], true);
                }
                else if (!zalogowany && string.IsNullOrEmpty(slownikCech[cechaId.Key].MetkaKatalog))
                {
                    //nie trzeba robić sprawdzenia czy posiada ceche gdyż przy pobieraniu idCechZAtrubutemZMetka było sprawdzeni
                    slownikCech[cechaId.Key] = UstawUniwersalnaMetkeNaCeche(slownikCech[cechaId.Key], atrybutyZMetkami[cechaId.Value.Value], false);
                }
            }
            return slownikCech.Values.OrderBy(x=>x.Kolejnosc).ToList();
            
        }

        public void PrzedAktualizacjaAtrybutow(IList<AtrybutBll> obj)
        {
            foreach (var item in obj)
            {
                if (string.IsNullOrEmpty(item.ProviderWyswietlania))
                {
                    item.ProviderWyswietlania = SolexBllCalosc.PobierzInstancje.Konfiguracja.TypDomyslnyFiltru;
                }
            }
        }

        public IList<RodzinyCechyUnikalne> BindPoSelectCechUnikatowych(int jezykID, IKlient zajadacyKlient, IList<RodzinyCechyUnikalne> listaCechUnikatowych, object daneDoSelecta)
        {
            if (jezykID == SolexBllCalosc.PobierzInstancje.Konfiguracja.JezykIDDomyslny)
            {
                return listaCechUnikatowych;
            }
            var tlumaczenia = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<Tlumaczenie>(jezykID, zajadacyKlient, x => x.Pole == "Rodzina" && Sql.In(x.ObiektId, listaCechUnikatowych.Select(y=>y.pid)))
                .ToDictionary(x => x.ObiektId, x => x.Wpis);

            foreach (var rodzinyCechyUnikalne in listaCechUnikatowych)
            {
                string wartosc;
                if (tlumaczenia.TryGetValue(rodzinyCechyUnikalne.pid, out wartosc))
                {
                    rodzinyCechyUnikalne.rodzina = wartosc;
                }
            }
            return listaCechUnikatowych;
        }
    }
}