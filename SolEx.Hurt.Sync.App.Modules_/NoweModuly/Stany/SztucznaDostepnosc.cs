using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using ServiceStack.Common;
using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.Sync;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Interfaces.SyncModuly;


namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Stany
{
    public class SztucznaDostepnosc : SyncModul, IModulStany
    {
        [FriendlyName("Atrybut z zapisanym składem")]
        [PobieranieSlownika(typeof(SlownikAtrybutow))]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public HashSet<int> Atrybut { get; set; }

        [FriendlyName("Separator")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string Separator { get; set; }

        [FriendlyName("Dla jakiego magazynu zwiększyć stan")]
        [PobieranieSlownika(typeof(SlownikMagazynow))]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public int Magazyn { get; set; }
        

        [FriendlyName("Czy liczyć ilość możliwych kompletów do zbudowania")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public bool CzyLiczyc { get; set; }

        [FriendlyName("Czy rozkładać zestway oraz dodawać ilość do produktu")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public bool CzyRozkladacZestaw { get; set; }

        [FriendlyName("Według jakiego pola wyszukiwać produkty")]
        [PobieranieSlownika(typeof(SlownikPolProduktow))]
        [WidoczneListaAdminAttribute(false, false, true, false)]

        public string Pole { get; set; }

        [FriendlyName("Czy zaokrąglać wynik do pełnej liczby")]
        public bool Zaokraglac { get; set; }

        public SztucznaDostepnosc()
        {
            Separator = ";";
            CzyRozkladacZestaw = true;
            Zaokraglac = true;
        }
    
        private PropertyInfo _pi;
        private PropertyInfo PoleInfo
        {
            get
            {
                if (_pi == null)
                {
                    Produkt p = new Produkt();
                    _pi = p.GetType().GetProperties().FirstOrDefault(x => x.Name == Pole);
                    if (_pi == null)
                    {
                        throw new InvalidOperationException("Brak pola o nazwie " + Pole);
                    }
                }
                return _pi;
            }
        }

        /// <summary>
        /// Metoda która dla zestawów z konkretną cecha wylicza ile ich w sumie jest (zestawów)
        /// </summary>
        /// <param name="idProduktowZCecha"></param>
        /// <param name="slownikStanowZestawow"></param>
        /// <returns></returns>
        public decimal PobierzSumeProduktowZTakimSamymSkladem(HashSet<long> idProduktowZCecha, Dictionary<long, decimal> slownikStanowZestawow )
        {
            decimal wynik = 0;
            //Obliczamy ile jest zestawów z tym składem ( z tą cecha)
            foreach (var idProduktuZCecha in idProduktowZCecha)
            {
                if (!slownikStanowZestawow.ContainsKey(idProduktuZCecha) || slownikStanowZestawow[idProduktuZCecha] == 0)
                {
                    continue;
                }
                var stany = slownikStanowZestawow[idProduktuZCecha];
                wynik += stany;
                slownikStanowZestawow[idProduktuZCecha] = 0;
            }
            return wynik;
        }

        /// <summary>
        /// Metoda która wyciąga ze składnika (cecha podzielona wględem separatora dla składników), wartość która bęzie mapowana z odpowiednim polem produktu (wybrane w konfiguracji modulu) oraz ilość produktu która jest wykorzystywana w zestawie
        /// </summary>
        /// <param name="s"></param>
        /// <param name="regex"></param>
        /// <param name="ilosc"></param>
        /// <returns></returns>
        public string PobierzDaneSkladnika(string s, Regex regex, out decimal ilosc)
        {
            //brak separatora od ilosci od nazwy - cecha to identyfikator produktu
            string nazwa = s;
            ilosc = 1;
            if (!regex.IsMatch(s))
            {
                return nazwa;
            }
            //dzielimy cechę względem * separator oddziela identyfikator produkty od ilości
            int idx = s.IndexOf("*", StringComparison.InvariantCultureIgnoreCase);
            if (idx == -1)
            {
                return nazwa;
            }
            nazwa = s.Substring(idx + 1);
            ilosc = decimal.Parse(s.Substring(0, idx));
            return nazwa;
        }

        private void RozbijanieCechy(Regex regex, string[] cechy, List<Produkt> produkty, List<ProduktStan> listaWejsciowa, ref Dictionary<long, decimal> slownik, HashSet<long> idProduktowZCecha, ref Dictionary<long, decimal> slownikStanowZestawow, ref bool rozkladac, Dictionary<string,long> slownikWartoscIIdProduktu)
        {
            //ilosc produktów (wszystkie zestawy z cecha
            decimal iloscGlownychSztuk = PobierzSumeProduktowZTakimSamymSkladem(idProduktowZCecha,slownikStanowZestawow);

            if (iloscGlownychSztuk == 0)
            {
                return;
            }

            //Przechodzimy po skłdanikach cechy
            foreach (var s in cechy)
            {
                decimal ilosc;
                string nazwa = PobierzDaneSkladnika(s, regex, out ilosc);



                long prodId;
                if (!slownikWartoscIIdProduktu.TryGetValue(nazwa, out prodId))
                {
                    return;
                }

                ////szukamy produktu którego pole z ustawienia bedzie równe temu z modulu
                //var prod = produkty.FirstOrDefault(x => PoleInfo.GetValue(x) != null && PoleInfo.GetValue(x).ToString() == nazwa);
                ////jeżeli nie ma takiego produktu to go pomijamy
                //if (prod == null)
                //{
                //    return;
                //}
                //sprawdzamy czy stan dla produktu był juz zmieniany w module jeżeli tak to do wcześniejszego stany dodajemy ilosc z aktualnego zestawu
                if (slownik.Keys.Contains(prodId))
                {
                    slownik[prodId] += ilosc * iloscGlownychSztuk;

                }
                else
                {
                    //stan zmieniany w module po raz pierwszy pobieramy stan produktu oraz dodajemy ilosc wyliczoną z zestawu
                    decimal iloscSztuk = listaWejsciowa.Where(x => x.ProduktId == prodId).Sum(y => y.Stan);
                    slownik.Add(prodId, iloscGlownychSztuk * ilosc + iloscSztuk);
                }

                //Sprawdzamy czy produkt z zestawu też jest zestawem jeżeli tak to aktualizujemy slownik zestawów oraz ustawiamy zmienna rozkładaj która mówio tym że trzeba wykonać kolejny rozkład zestawów
                if (!slownikStanowZestawow.ContainsKey(prodId)) continue;
                //Jeżeli produkt z zestawu jest również zestawem ustawiamy mu stan w slowniku zestawów
                slownikStanowZestawow[prodId] = slownik[prodId];
                rozkladac = true;
            }
        }
        /// <summary>
        /// Modul który rozkłada zestawy bądz składa z dostępnych produktów.
        /// </summary>
        /// <param name="listaWejsciowa"></param>
        /// <param name="magazyny"></param>
        /// <param name="produkty"></param>
        public void Przetworz(ref Dictionary<int, List<ProduktStan>> listaWejsciowa, List<Magazyn> magazyny, List<Produkt> produkty)
        {
            string a = @"^[\d]*\,*[\d]{1,4}";
            Regex myRegex = new Regex(a);

            //Pobieramy idCech dla atrybutu który określa cechy zestawów
            HashSet<long> listaIdCech = new HashSet<long>( CechyNaB2B.Keys );
            //Pobieramy łączniki tylko cech które określają zestaw
            List<ProduktCecha> result = ApiWywolanie.PobierzCechyProdukty(listaIdCech).Values.Where(x => CechyNaB2B.ContainsKey(x.CechaId)).ToList();
            //Tworzymy slownik gdzie klucz to idCechyZestawu oraz wartosc to idListyProduktow
            Dictionary<long, List<long>> slownikZestawow = result.GroupBy(x => x.CechaId).ToDictionary(x => x.Key, x => x.Select(y => y.ProduktId).ToList());
            //Wyciagamy idPrduktów które mają ceche zestawów (są zestawem)
            HashSet<long> idZestawow = new HashSet<long>( slownikZestawow.SelectMany(x => x.Value) );

            //slownik gdzie kluczem jest wartosc z pola z konfiguracji natomiast wartosc toid produktu
            Dictionary<string, long> slownikWartoscIIdProduktu = produkty.ToDictionary(x => PoleInfo.GetValue(x).ToString(), x => x.Id);

            //Wyciagamy stany zestawów dla wszystkich magazynów
            Dictionary<long, decimal> slownikStanowZestawow = listaWejsciowa.Values.SelectMany(x => x).Where(x => idZestawow.Contains(x.ProduktId)).GroupBy(x => x.ProduktId).ToDictionary(x => x.Key, x => x.Sum(y => y.Stan));
            //wyciągamy te produkty które są zestawami ale nie ma ich w stanch bo maja ilość równa zero
            HashSet<long> idProduktuBezStanu = new HashSet<long>( idZestawow.Except(slownikStanowZestawow.Keys) );
            //slownik bazowych ilosc które maja produkty potrzebne w składaniu zestawów
            var ilosciBazowe = listaWejsciowa.SelectMany(x => x.Value).GroupBy(x => x.ProduktId).ToDictionary(x=>x.Key,x=>x.Sum(y=>y.Stan));


            HashSet<long>brakProduktowNaPlatformie = new HashSet<long>();

            //Sprawdzamy czy sa zestawy bez stanów jeżeli tak to dodajemy je do slownika zestawów gdyż tam koniecznie musimy bieć wszystkie zestawy

            if (idProduktuBezStanu != null && idProduktuBezStanu.Any())
            {
                foreach (var id in idProduktuBezStanu)
                {
                    slownikStanowZestawow.Add(id,0);
                }
            }

            //Sprawdzamy czy chcemy rozkład zestaw
            if (CzyRozkladacZestaw)
            {
                //sprawdzamy czy wogóle jest jakiś zestaw który stan ma większy od zera
                if (slownikStanowZestawow.Any(x => x.Value != 0))
                {
                    bool rozkladac = true;
                    Dictionary<long, decimal> slownik = new Dictionary<long, decimal>();
                    //Wszystkie stany wszystkich produktów
                    var listaStanow = listaWejsciowa.Values.SelectMany(x => x).ToList();
                    while (rozkladac)
                    {
                        rozkladac = false;
                        foreach (var cecha in slownikZestawow)
                        {
                            //Jezeli brak produktów dla cechy idziemy do nastepnej cechy
                            if (!cecha.Value.Any())
                            {
                                continue;
                            }
                            Cecha cechaB2b;
                            //Wyciagamy obiekt cechy 
                            if (!CechyNaB2B.TryGetValue(cecha.Key, out cechaB2b))
                            {
                                continue;
                            }
                            //dzielimy nazwe cechy ze wględu na separator - dceaje nam to tablice składników zestawu
                            string[] cechytab = cechaB2b.Nazwa.Split(new[] { Separator }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToArray();
                            RozbijanieCechy(myRegex, cechytab, produkty, listaStanow, ref slownik, new HashSet<long>( cecha.Value), ref slownikStanowZestawow, ref rozkladac, slownikWartoscIIdProduktu);
                        }
                    }
                    foreach (var stanyProduktow in slownik)
                    {
                        listaWejsciowa[Magazyn].First(x => x.ProduktId == stanyProduktow.Key).Stan = stanyProduktow.Value;
                    }
                }
            }
            //Jeżeli nie liczymy ilości zestawów które możemy złożyć to pomijamy
            if (!CzyLiczyc)
            {
                return;
            }
            //Budujemy słownik gdzie kluczem będzie nazwa cechy(skład zestawu) oraz lista idProduktów które posiadają taki skład
            Dictionary<long, string> slownikProduktowICechZestawow = new Dictionary<long, string>();
            foreach (var cecha in slownikZestawow)
            {
                foreach (var produkt in cecha.Value)
                {
                    if (slownikProduktowICechZestawow.ContainsKey(produkt))
                    {
                        throw new Exception($"Błąd skonfigurowanych zestawów produkt o id: {produkt}, posiada więcej niż jedną cechę zestawów te cechy to: {slownikProduktowICechZestawow[produkt]}, oraz {CechyNaB2B[cecha.Key].Nazwa}");
                    }
                    slownikProduktowICechZestawow.Add(produkt, CechyNaB2B[cecha.Key].Nazwa);
                }
            }

            bool skladac = true;
            //Pobieramy cechy zestawów które w składzie nie maja innych zestawów
            HashSet<long> idCechZZestawami = PobierzIdCechKtoreMajaZestawow(CechyNaB2B, idZestawow, myRegex, produkty, slownikWartoscIIdProduktu);
            HashSet<long> idProduktowDoSkladania = new HashSet<long>(slownikZestawow.Where(x=> !idCechZZestawami.Contains(x.Key)).SelectMany(x=>x.Value));
            while (skladac)
            {
                HashSet<long> doZmiany = new HashSet<long>();
                foreach (var idProd in idProduktowDoSkladania)
                {
                    //Dzielimy nazwę cechy względem separatora składników
                    string[] cechytab = slownikProduktowICechZestawow[idProd].Split(new[] {Separator}, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToArray();
                    //slownik gdzie kluczem jest idProduktu oraz wartość to ilość danego produktu potrzebna do zbudowania zestawu
                    Dictionary<long, decimal> potrzebneTowary = new Dictionary<long, decimal>();

                    int? iloscProduktu = null;
                    foreach (var c in cechytab)
                    {
                        decimal ilosc;
                        string nazwa = PobierzDaneSkladnika(c, myRegex, out ilosc);
                        
                        //Sprawdzamy czy w składzie ktoś nie zrobił błędu i nie wpisał 0
                        if (ilosc == 0)
                        {
                            Log.InfoFormat($"W zestawie: {c} jest błędna ilość wpisana dla produktu {idProd}. Zestaw ten zostanie pominięty");
                            break;
                        }
                        
                        //Pobieramy produkt względem identyfikatora pobranego ze składu
                        long prodId;
                        if (!slownikWartoscIIdProduktu.TryGetValue(nazwa, out prodId))
                        {
                            LogiFormatki.PobierzInstancje.LogujInfo("Nie znaleziono produktu z zestawu na b2b o identyfikatorze {0} w polu {2}, pomijam wyliczanie dodatkowego stanu dla produktu {1}", nazwa, idProd, PoleInfo.Name);
                            prodId = nazwa.WygenerujIDObiektuSHAWersjaLong();
                            brakProduktowNaPlatformie.Add(prodId);
                        }
                        //Dodajemy do słownika potrzebna ilosc 
                        potrzebneTowary.Add(prodId, ilosc);
                    }

                    PoliczIloscMozliwychZestawowDoZbudowania(potrzebneTowary, listaWejsciowa, ref iloscProduktu, brakProduktowNaPlatformie);

                    string wartoscPola;
                    List<long> listaCechZProduktem;
                    //Jezeli ilość mnożliwych zestawów do złożęnia to 0 to pomijamy
                    if (iloscProduktu == 0)
                    {
                        //Wyciagamy z produktu wartosc pola po którym moduł mapuje produkty ze składnikami
                        wartoscPola = slownikWartoscIIdProduktu.FirstOrDefault(x => x.Value == idProd).Key;

                        //Wyciagamy te cechy które maja w sobie iterowany produkt (cechy zastawów gdzie iterowany produkt jest wykorzystywany
                        listaCechZProduktem = PobierzZestawySkladajaceSieZProduktu(wartoscPola, CechyNaB2B.Values.ToList(), myRegex);
                        doZmiany.UnionWith(result.Where(x => listaCechZProduktem.Contains(x.CechaId)).Select(x => x.ProduktId));
                        continue;
                    }

                    //Stan bazowy potrzeby nam jest ze względu na fakt iż do możliwej ilości jaką możemy złożyć trzeba dodać ilość zestawów jakie już mamy na stanies
                    decimal stanBazowy;
                    if (!ilosciBazowe.TryGetValue(idProd, out stanBazowy))
                    {
                        stanBazowy = 0;
                    }

                    //ustawiamy stan dla zestawu ilość jaką można złożyć + stan bazowy zestawu
                    listaWejsciowa[Magazyn].First(x => x.ProduktId == idProd).Stan = stanBazowy + (iloscProduktu ?? 0);

                    ////Wyciagamy z produktu wartosc pola po którym moduł mapuje produkty ze składnikami
                    wartoscPola = slownikWartoscIIdProduktu.FirstOrDefault(x => x.Value == idProd).Key;

                    //Wyciagamy te cechy które maja w sobie iterowany produkt (cechy zastawów gdzie iterowany produkt jest wykorzystywany
                    listaCechZProduktem = PobierzZestawySkladajaceSieZProduktu(wartoscPola, CechyNaB2B.Values.ToList(), myRegex);
                    doZmiany.UnionWith(result.Where(x => listaCechZProduktem.Contains(x.CechaId)).Select(x => x.ProduktId));
                }
                idProduktowDoSkladania = new HashSet<long>(doZmiany);
                skladac = idProduktowDoSkladania.Any();
            }
        }


        public void PoliczIloscMozliwychZestawowDoZbudowania(Dictionary<long, decimal> potrzebneTowary, Dictionary<int, List<ProduktStan>> listaWejsciowa, ref int? iloscProduktu, HashSet<long>brakujaceTowary)
        {
            //Przechodzimy po potrzebnych towarach do zbudowania zestawu
            foreach (var towar in potrzebneTowary)
            {
                //pobieramy jaki jest stan dla produktu potrzebnego
                decimal stan = 0;
                if (!brakujaceTowary.Contains(towar.Key))
                {
                    ProduktStan produktStan = listaWejsciowa[Magazyn].FirstOrDefault(x => x.ProduktId == towar.Key);
                    if (produktStan != null)
                    {
                        stan = produktStan.Stan;
                    }
                }
                
                //Wyliczamy możliwą ilość zestawów które możemy złożyć dla konkretnego produktu
                    int iloscMozliwa = (int)(stan / towar.Value);
                //Jezeli nie ustawialiśmy wcześniej mozliwej ilosci (rozpatrujemy pierwszy składnik zestawu) lub wyliczona ilosc jest mniejsza niz wcześniejsza wpisujemy tą ilość jako możliwą do zbudowania
                if (iloscProduktu == null || iloscMozliwa < iloscProduktu.Value)
                {
                    iloscProduktu = iloscMozliwa;
                }
            }
        }

        /// <summary>
        /// Metoda ta pobiera cechy w których składzie nie ma już innych zestawów (ostatni element zestawu)
        /// </summary>
        /// <param name="cechyB2B"></param>
        /// <param name="idZestawow"></param>
        /// <param name="myRegex"></param>
        /// <param name="produkty"></param>
        /// <returns></returns>
        public HashSet<long> PobierzIdCechKtoreMajaZestawow(Dictionary<long, Cecha>cechyB2B, HashSet<long>idZestawow, Regex myRegex, List<Produkt>produkty, Dictionary<string,long> slownikWartoscIIdProduktu)
        {
            HashSet<long>wynik = new HashSet<long>();
            HashSet<string>nazwySkladnikow = new HashSet<string>();

            foreach (var cecha in cechyB2B)
            {
                string[] cechytab = cecha.Value.Nazwa.Split(new[] { Separator }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToArray();
                foreach (var s in cechytab)
                {
                    decimal ilosc;
                    string nazwa = PobierzDaneSkladnika(s, myRegex, out ilosc);
                    nazwySkladnikow.Add(nazwa);

                    long idProduktu;
                    if (!slownikWartoscIIdProduktu.TryGetValue(nazwa, out idProduktu) || !idZestawow.Contains(idProduktu))
                    {
                        continue;
                    }
                    wynik.Add(cecha.Key);
                    break;
                }
            }
            return wynik;
        }

        private Dictionary<long, Cecha> _cechyB2B;
        public virtual Dictionary<long,Cecha> CechyNaB2B => _cechyB2B ?? (_cechyB2B = ApiWywolanie.PobierzCechyDlaAtrybutow(Atrybut));

        public override string uwagi => "";

        /// <summary>
        /// Metoda pobierająca które cechy zestawów zawierają w sobie składnik (wartosc) - trzeba je przeliczyć jeszcze raz
        /// </summary>
        /// <param name="wartosc"></param>
        /// <param name="listaCech"></param>
        /// <param name="myRegex"></param>
        /// <returns></returns>
        public List<long> PobierzZestawySkladajaceSieZProduktu(string wartosc, List<Cecha> listaCech, Regex myRegex)
        {
            List<long> listaCechDoPoprawy = new List<long>();
            foreach (var cecha in listaCech)
            {
                string[] cechytab = cecha.Nazwa.Split(new[] { Separator }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToArray();
                foreach (var c in cechytab)
                {
                    decimal ilosc;
                    string nazwa = PobierzDaneSkladnika(c, myRegex, out ilosc);
                    if (nazwa.Trim().Equals(wartosc.Trim(),StringComparison.InvariantCultureIgnoreCase) && !listaCechDoPoprawy.Contains(cecha.Id))
                    {
                        listaCechDoPoprawy.Add(cecha.Id);
                        break;
                    }
                }
            }
            return listaCechDoPoprawy;
        } 



    }
}
