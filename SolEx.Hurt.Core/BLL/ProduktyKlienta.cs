using ServiceStack.Common;
using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Core;
using SolEx.Hurt.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FastMember;
using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core.BLL
{
    public class ProduktyKlienta : BllBazaCalosc, IProduktyKlienta
    {
        public ProduktyKlienta(ISolexBllCalosc calosc) : base(calosc)
        {
        }
    
       
        private HashSet<long> _idProduktowDostepnychWszystkie;

        public virtual HashSet<long> PobierzIdProduktowRzeczywiscieDostepnychDlaKlienta(IKlient klient)
        {
            HashSet<long> wynik = null;
            if (!klient.OfertaIndywidualizowana && _idProduktowDostepnychWszystkie != null)
            {
                return _idProduktowDostepnychWszystkie;
            }
            
            IList<ProduktKlienta> produktyWirtualne = null;
            wynik = this.PobierzIdProduktowRzeczywiscieDostepnychDlaKlienta(klient, out produktyWirtualne);

            //gdy klien ma oferte indywidualizowaną to zapisujemy to do zmiennej prywatnej
            if (!klient.OfertaIndywidualizowana)
            {
                _idProduktowDostepnychWszystkie = wynik;
            }
            return wynik;
        }

        /// <summary>
        /// Metoda zwraca id produktów dostępnych dla klienta
        /// </summary>
        /// <param name="klient"></param>
        /// <param name="produktyWirtualne"></param>
        /// <param name="zwracajTylkoIdProduktowBazowych">Paremetr okreśala czy chcemy zwracac bazowe id dla produktów wirtualnych</param>
        /// <returns></returns>
        public virtual HashSet<long> PobierzIdProduktowRzeczywiscieDostepnychDlaKlienta(IKlient klient, out IList<ProduktKlienta> produktyWirtualne, bool zwracajTylkoIdProduktowBazowych=false)
        {
            string klucz = CacheNameProdktyKlienta(klient);

            //Item1 - id produktów klienta
            //Item2 - id produktów Bazowych
            //Item3 - lista produktów wirtualnych
            Tuple<HashSet<long>, HashSet<long>, IList<ProduktKlienta>> produktyKlientaZbiorcze =
                LockHelper.PobierzInstancje.PobierzDaneWLocku_zUcyciemCache(klucz, () =>
                {
                    //spcjelnie w 2 krokach bo szukamy nie po SQL
                    HashSet<long> kategorieids = new HashSet<long>( Calosc.DostepDane.Pobierz<KategorieBLL>(klient).Select(x => x.Id) );

                    HashSet<long> produktyKlientaIDs = new HashSet<long>();
                    HashSet<long> produktyKlientaBazoweId = new HashSet<long>();
                    IList<ProduktKlienta> wirtualneProdukty = null;

                    //jesli klient nie ma zadnych kategorii to NIE będzie miał żadnych produktów
                    if (kategorieids != null && kategorieids.Any())
                    {
                        //wyciagmy produkty dla klienta dostepny - i jeszcze filtrujemy wg. kategorii tak żeby ukryć te produkty, których nie widzi kategorii klient
                        List<ProduktBazowy> produktyBazoweKlienta = SolexBllCalosc.PobierzInstancje.ProduktyUkryteBll.PobierzProduktyDostepneDlaKlienta(klient);
                        Dictionary<long, ProduktBazowy> produktyBazoweDlaWirtualnych = new Dictionary<long, ProduktBazowy>();

                        foreach (ProduktBazowy pb in produktyBazoweKlienta)
                        {
                            //brak kategorii dla produktu - omijamy go
                            if (pb.KategorieId == null || pb.KategorieId.IsEmpty())
                            {
                                continue;
                            }

                            if (kategorieids.Overlaps(pb.KategorieId))
                            {
                                if (pb.Abstrakcyjny)
                                {
                                    produktyBazoweDlaWirtualnych.Add(pb.Id, pb);
                                }
                                else
                                {
                                    produktyKlientaIDs.Add(pb.Id);
                                }
                            }
                        }

                        produktyKlientaBazoweId = new HashSet<long>(produktyKlientaIDs);
                    }

                    produktyKlientaZbiorcze = new Tuple<HashSet<long>, HashSet<long>, IList<ProduktKlienta>>(produktyKlientaIDs, produktyKlientaBazoweId, wirtualneProdukty);
                    return produktyKlientaZbiorcze;
                });

            produktyWirtualne = produktyKlientaZbiorcze.Item3;
            return zwracajTylkoIdProduktowBazowych ? produktyKlientaZbiorcze.Item2 : produktyKlientaZbiorcze.Item1;
        }

        private string CacheNameProdktyKlienta(IKlient klient)
        {
            return $"produkty_klienta_ids_klient_{klient.Id}_";
        }


        private static string kluczCacheProduktowKlienta(long idKlienta, int? jezykID = null)
        {
            if (jezykID.HasValue)
            {
                return $"produkty_klienta_obiekty_{idKlienta}_jezyk_{jezykID}";
            }
            return $"produkty_klienta_obiekty_{idKlienta}";
        }

        public void WyczyscCacheProduktyKlienta(IKlient klient = null)
        {
            if (klient == null)
            {
               throw new Exception("Tu powinien być klient a nie ma.");
            }

            Calosc.Rabaty.WyczyscCacheFlatCenKlienta(klient.Id);
            Calosc.Cache.UsunGdzieKluczRozpoczynaSieOd(kluczCacheProduktowKlienta(klient.Id));

            if (klient.KlientPodstawowy().Id != klient.Id)
            {
                Calosc.Cache.UsunGdzieKluczRozpoczynaSieOd(kluczCacheProduktowKlienta(klient.KlientPodstawowy().Id));
                Calosc.Rabaty.WyczyscCacheFlatCenKlienta(klient.KlientPodstawowy().Id);
            }
        }


        public IList<ProduktKlienta> PobierzProduktyKlientaZCache(int jezykID, IKlient klient)
        {
            string klucz = kluczCacheProduktowKlienta(klient.Id, jezykID);

            return LockHelper.PobierzInstancje.PobierzDaneWLocku_zUcyciemCache(klucz, () =>
            {
                IList<ProduktKlienta> wirtalneProdukty = null;
                HashSet<long> ids = PobierzIdProduktowRzeczywiscieDostepnychDlaKlienta(klient, out wirtalneProdukty, true);
                IList<ProduktKlienta> wynik = Calosc.DostepDane.Pobierz<ProduktBazowy>(jezykID, klient).Where(x => x.KategorieId != null && x.Kategorie != null &&
                    ids.Contains(x.Id)).Select(x => new ProduktKlienta(x, klient)).ToList();

                if (wirtalneProdukty != null && wirtalneProdukty.Any())
                {
                    wynik = wynik.Union(wirtalneProdukty).ToList();
                }

              //  bool gradacjeUzgledniaProduktyZCecha = Calosc.Konfiguracja.GradacjeUzgledniaProduktyZCecha.HasValue;
                var kategorie = Calosc.KategorieDostep.PobierzKategorieDostepneDlaKlienta(klient);

                //maly speedup - ustawienia widocznosci zamienikow przy bindignu a nie w locie, vat i kategorie
                foreach (ProduktKlienta p in wynik)
                {
                    //sprawdzamy dodaktowo czy na produkcie klienta zamieniki sa NULL - moze byc tak ze produtky wirtualne juz maja zamienimi uzupelnione
                    //if (p.Zamienniki != null && ((p as ProduktKlientaWirtualny)?.Zamienniki == null))
                    //{
                    //    //jesli produkt bazowy ma zamienniki to wyliczamy tylko te zamieniki dla produktu klienta ktore klient zobaczy (produkty dostepne dla klienta)
                    //    var zamiennikiJakieKlientWidzi = p.Zamienniki.Where(x => ids.Contains(x.Key)).ToDictionary(x => x.Key, x => x.Value);
                    //    if (zamiennikiJakieKlientWidzi.Count != p.Zamienniki.Count)
                    //    {
                    //        //przepisanie kolekcji tylko jesli zmiana
                    //        p.Zamienniki = zamiennikiJakieKlientWidzi;
                    //    }
                    //}

                    p.Vat = klient.IndywidualnaStawaVat.GetValueOrDefault(((ProduktBazowy) p).Vat);

                    //Pobieramy kategorie wzgledem id
                    List<KategorieBLL> kategorieWidoczneDlaKlienta = kategorie.Where(x => p.KategorieId.Contains(x.Key)).Select(x => x.Value).ToList();

                    if (kategorieWidoczneDlaKlienta.Count != p.KategorieId.Count)
                    {
                        //przypisanie tylko jesli kategorie sa rozne z bazowymi - zwykle beda sie pokrywac
                        p.Kategorie = kategorieWidoczneDlaKlienta;
                    }

                    //Uzupelniamu id cech produktu (cechy pobierane są z produktu bazowego) wraz z cechami wirtualnymi wyliczanymi takie jak moj katalog czy akcesoria
                    //   HashSet<long> _idCechProduktu;

                    ////uzupelniamy tylko jesli nie uzupelnione wczesniej - np. w wierulanych produktach mozna bylo to uzupelnic dla ProduktuKlienta
                    //if (p.IdCechPRoduktu == null)
                    //BCH: BIO nie ma produktow writualnych - zawsze roibmy nowa kolekcje cech
                    //{
                    //    if ((p as ProduktBazowy).IdCechPRoduktu != null)
                    //    {
                    //        _idCechProduktu = new HashSet<long>((p as ProduktBazowy).IdCechPRoduktu);
                    //    }
                    //    else
                    //    {
                    //        _idCechProduktu = new HashSet<long>();
                    //    }
                    //}
                    //else
                    //{
                    //    _idCechProduktu = p.IdCechPRoduktu;
                    //}


                    //bch:   p.IdCechPRoduktu sa przepisane w produkcie juz w konstrukturze - zmiana byla BCH bo cache sie pieprzyl strasznie

                    try
                    {
                        string atrTypStanu = SolexBllCalosc.PobierzInstancje.Konfiguracja.SymbolAtrybutCechyTypStanu;
                        string friendlyName = SolexBllCalosc.PobierzInstancje.CechyAtrybuty.PobierzPrzyjaznaNazweDlaTypuStanu(p.PobierzTypStany);
                        long tmpIdTs = SolexBllCalosc.PobierzInstancje.CechyAtrybuty.PobierzCecheOSymbolu(atrTypStanu + ":" + friendlyName, p.JezykId).Id;
                        p.IdCechPRoduktu.Add(tmpIdTs);
                    }catch(KeyNotFoundException ex)
                    {
                        Calosc.Log.Error($"Nie mozna pobrac typu stanu dla produktu id: {p.Id}, typ stanu: {p.PobierzTypStany}. Błąd: {ex.Message}");
                    }

                    //wyliczamy dodatkowe dane do cech - za kazdym razem
                    if (p.JestWMoimKatalogu())
                    {
                        p.IdCechPRoduktu.Add(SolexBllCalosc.PobierzInstancje.Konfiguracja.CechaMojKatalog);
                    }

                    if (p.JestWUlubionych())
                    {
                        p.IdCechPRoduktu.Add(SolexBllCalosc.PobierzInstancje.Konfiguracja.CechaUlubione.Id);
                    }

                    //p.IdCechPRoduktu = _idCechProduktu;
                    //Uzupełniamy cechy produktu klienta - potrzebne jest to ponieważ slownik Cech jest w produkcie bazowym a chcąc miec cechy dodawne w produkcie klienta jak np ulubione trzeba je pobrac i uzupelnić na produkcie klienta
                    // p.Cechy = Calosc.CechyAtrybuty.PobierzCechyOId(_idCechProduktu, jezyk).ToDictionary(x => x.Id, x => x);
                    p.IloscMinimalna = Calosc.Klienci.CzyKlientNieMaMinimumLogistyczne(klient) ? 0 : ((ProduktBazowy) p).IloscMinimalna;
                }

                return wynik;
            });
        }
        

        public IList<ProduktKlienta> ProduktySpelniajaceKryteria(long? kategorieId, string wyszukiwaneGlobalne, IKlient klient, int lang, Dictionary<int, HashSet<long>> filtry, Dictionary<int, HashSet<long>> staleFiltyIds, 
            string wyszukiwanieWewnatrzKategorii, IList<long> idProduktow = null)
        {
            //todo: troche bardziej mozna zoptymalizowac - czeste wywolania z tymi samymi parametami - czy nie dac cache chwilowego? sam nie wiem bartek
            if ( kategorieId == null && string.IsNullOrEmpty(wyszukiwaneGlobalne) &&
                (filtry == null || filtry.IsEmpty()) && (staleFiltyIds == null || staleFiltyIds.IsEmpty()) && (idProduktow == null || idProduktow.IsEmpty()))
            {
                return PobierzProduktyKlientaZCache(lang, klient);
            }

            //Pobieramy produkty klienta
            Dictionary<long, ProduktKlienta> produktyKlienta = this.PobierzProduktyKlientaZCache(lang, klient).ToDictionary(x=>x.Id,x=>x);

            string cacheName = null;

            if (!klient.OfertaIndywidualizowana)
            {
                HashSet<long> idProduktowDoWyciagniecia = new HashSet<long>();
                //jesli jest ulubione w filtrach to NIE wolno cachować bo lista jest dynaiczna - klient caly czas zmienia
                bool filtrWgUlubionych = (filtry != null && filtry.ContainsKey(Calosc.Konfiguracja.CechaUlubione.AtrybutId.Value)) || 
                                         (staleFiltyIds != null && staleFiltyIds.ContainsKey(Calosc.Konfiguracja.CechaUlubione.AtrybutId.Value));
                if (!filtrWgUlubionych)
                {
                    var filtryString = filtry != null ? string.Join("_", filtry.Values.SelectMany(x => x).Distinct().OrderBy(x => x)) : "";
                    var staleFiltryString = staleFiltyIds != null ? string.Join("_", staleFiltyIds.Values.SelectMany(x => x).Distinct().OrderBy(x => x)) : "";
                    string idProduktowString = idProduktow != null ? string.Join(",", idProduktow) : "";
                    cacheName = $"ProduktySpelniajaceKryteria_{kategorieId}_{wyszukiwaneGlobalne}_{filtryString}_{staleFiltryString}_{wyszukiwanieWewnatrzKategorii}_{idProduktowString}_{lang}";
                    idProduktowDoWyciagniecia = Calosc.Cache.PobierzObiekt<HashSet<long>>(cacheName);
                    if (idProduktowDoWyciagniecia != null)
                    {
                        return produktyKlienta.WhereKeyIsIn(idProduktowDoWyciagniecia);
                    }
                }
            }

            HashSet<long> idProduktowRozpatrywanych = new HashSet<long>(produktyKlienta.Keys);
            if (idProduktow != null && idProduktow.Any())
            {
                idProduktowRozpatrywanych.IntersectWith(idProduktow);
            }
           
            if (kategorieId == null && !string.IsNullOrEmpty(wyszukiwaneGlobalne) && !string.IsNullOrEmpty(wyszukiwanieWewnatrzKategorii))
            {
                throw new Exception("Nie można szukać wewnątrz kategorii bez podania kategorii");
            }


            if (kategorieId.HasValue)
            {
                if (Calosc.ProduktyKategorieDostep.ProduktyKategorieGrupowanePoKategorii.TryGetValue(kategorieId.Value, out HashSet<long> idsProduktow))
                {
                    idProduktowRozpatrywanych.IntersectWith(idsProduktow);
                    //wszystkieprodukty = produktyKlienta.WhereKeyIsIn(idsProduktow).ToDictionary(x => x.Id);
                }
            }


            //szukanie słów
            if (!string.IsNullOrEmpty(wyszukiwanieWewnatrzKategorii) || !string.IsNullOrEmpty(wyszukiwaneGlobalne))
            {
                //string frazaSzukania = $"{wyszukiwanieWewnatrzKategorii} {wyszukiwaneGlobalne}".Trim();

                //NOWE SZUKANIE - w SQLu
                string frazaSzukania = $"{wyszukiwanieWewnatrzKategorii} {wyszukiwaneGlobalne}";
                long[] collection = Calosc.Szukanie.FiltrowanieProduktowWedlugSzukanejFrazy(frazaSzukania, klient, lang, idProduktowRozpatrywanych, int.MaxValue);
                idProduktowRozpatrywanych = new HashSet<long>( collection );
            }


            Dictionary<long, ProduktKlienta> wszystkieprodukty = produktyKlienta.WhereKeyIsIn(idProduktowRozpatrywanych).ToDictionary(x => x.Id);

            if (filtry != null && filtry.Any()) //które produkty pasują wzglądem ograniczenia na filtry
            {
                foreach (KeyValuePair<int, HashSet<long>> i in filtry)
                {
                    wszystkieprodukty = wszystkieprodukty.Values.Where(x => i.Value.Overlaps(x.IdCechPRoduktu)).ToDictionary(x => x.Id);
                   // idProduktowRozpatrywanych.IntersectWith(bazowe.Select(x => x.Id));
                }
            }

            if (staleFiltyIds != null && staleFiltyIds.Any())
            {
                foreach (KeyValuePair<int, HashSet<long>> i in staleFiltyIds)
                {
                    wszystkieprodukty = wszystkieprodukty.Values.Where(x => i.Value.Overlaps(x.IdCechPRoduktu)).ToDictionary(x => x.Id);
                    //idProduktowRozpatrywanych.IntersectWith(bazowe.Select(x=>x.Id));
                }
            }

            if (cacheName != null)
            {
                //gownianie sprawdzenie czy juz jest w cache - niestety stara wersja bio - nie ma sensu tu nic zmieniac
                if ( Calosc.Cache.PobierzObiekt<HashSet<long>>(cacheName) == null ){
                     Calosc.Cache.DodajObiekt(cacheName, new HashSet<long>( wszystkieprodukty.Values.Select(x=>x.Id) ) ); 
                }
            }

            return wszystkieprodukty.Values.ToList();
        }
    
        public Dictionary<IProduktKlienta, KategorieBLL> WybierzProduktyDoPokazaniaWgStronyISortowania(long? kategorie, IList<ProduktKlienta> pasujaceprodukty, 
            IKlient customer, int jezykId, List<SortowaniePole> sortowanie,
         bool wylaczGrupowanie, int pominacIle, int ilePobrac, out int lacznie, out HashSet<long> wszystkieidsrodzinowe)
        {
            List<ProduktKategoriaDaneSortowania> tmp2 = PobierzIdProduktow(kategorie, pasujaceprodukty, customer, jezykId, sortowanie, wylaczGrupowanie, out wszystkieidsrodzinowe);
            lacznie = tmp2.Count;
            return tmp2.Skip(pominacIle).Take(ilePobrac).ToDictionary(x => x.produktKlienta, x => x.kategoria);
        }

        public List<ProduktKategoriaDaneSortowania> PobierzIdProduktow(long? kategorie, IList<ProduktKlienta> pasujaceprodukty, IKlient customer, int jezykId, List<SortowaniePole> sortowanie,
             bool wylaczGrupowanie, out HashSet<long> wszystkieidsrodzinowe)
        {
            IList<ProduktKategoriaDaneSortowania> ids = PobierzProdukty(kategorie, pasujaceprodukty, customer, sortowanie, jezykId, wylaczGrupowanie);
            wszystkieidsrodzinowe = new HashSet<long>( ids.Where(x => !string.IsNullOrEmpty(x.produktKlienta.Rodzina)).Select(x => x.produktKlienta.Id) );
            List<ProduktKategoriaDaneSortowania> tmp2 = new List<ProduktKategoriaDaneSortowania>(ids.Count);
            HashSet<string> rodzinyprzerobione = new HashSet<string>();
            foreach(var p in ids)
            {
                if (string.IsNullOrEmpty(p.produktKlienta.Rodzina) || !rodzinyprzerobione.Contains(p.produktKlienta.Rodzina))
                {
                    rodzinyprzerobione.Add(p.produktKlienta.Rodzina);
                    tmp2.Add(p);
                }
            }
            return tmp2;
        }

        public IList<ProduktKategoriaDaneSortowania> PobierzProdukty(long? kategorieId, IList<ProduktKlienta> wszystkie,
           IKlient klient, List<SortowaniePole> sortowanie, int lang, bool wylaczGrupowanie = false)
        {
            IList<ProduktKategoriaDaneSortowania> ids = new List<ProduktKategoriaDaneSortowania>(wszystkie.Count);
            IList<KategorieBLL> katgrupa = null;
            if (!wylaczGrupowanie)
            {
                KategorieBLL kategoriaDoKomplementarnych = null;
                if (kategorieId.HasValue)
                {
                    kategoriaDoKomplementarnych = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<KategorieBLL>(kategorieId, klient);
                }

                GrupaBLL grupaKomplementarnaId = SolexBllCalosc.PobierzInstancje.KategorieDostep.PobierzIdGrupyKomplementarnej(kategoriaDoKomplementarnych, klient, lang);

                if (grupaKomplementarnaId != null)
                {
                    katgrupa = grupaKomplementarnaId.PobierzKategorie(klient, null);
                }
            }

            IDictionary<long, HashSet<long>> laczniki = Calosc.ProduktyKategorieDostep.ProduktyKategorieGrupowanePoProdukcie;
              
            foreach(IProduktKlienta p in wszystkie)
            {
                HashSet<long> l = laczniki[(p as ProduktBazowy).Id];  //id musi byc z produtku bazowego bo moze byc produkt wirtualny
                KategorieBLL kk = null;

                if (katgrupa != null)
                {
                    //z kategorii podanych jako parametr o ile sa podane
                    if (kategorieId != null)
                    {
                        kk = katgrupa.FirstOrDefault(x => l.Contains(x.Id) && kategorieId == x.Id);
                    }
                   
                    //z kategorii komplementranej
                    if (kk == null)
                    {
                        kk = katgrupa.FirstOrDefault(x => l.Contains(x.Id));
                    }

                    ////jak dalej brak to jakakolwiek kategoria produktu widocznej dla klienta
                    //if (kk == null)
                    //{
                    //    kk = p.Kategorie.FirstOrDefault();
                    //}

                }
                ids.Add(new ProduktKategoriaDaneSortowania{kategoria = kk, kategoriaSciezka = kk != null ?  kk.Sciezka : null, produktKlienta = p});
            }

            if (sortowanie.Any() && ids.Any())
            {
                return PosortujKolekcjeProduktow(ids,sortowanie);
            }
            return ids;
        }

       //todo: to sortowanier to chyba cachować trzeba?
        public IList<ProduktKategoriaDaneSortowania> PosortujKolekcjeProduktow(IList<ProduktKategoriaDaneSortowania> ids, List<SortowaniePole> sort)
        {
            List<ParametrySortowanie> parametrysortowania = new List<ParametrySortowanie>();
            List<SortowaniePole> sortowanie = new List<SortowaniePole>(sort);

            //czy sa jacys ojcowie - jak tak to sortujemy po nich. W zdecydowanej wiekszosci nie bedzie ojcow
            if (ids.Any(x => x.produktKlienta.Ojciec))
            {
                sortowanie.Add(new SortowaniePole("Ojciec", KolejnoscSortowania.desc));
            }

            Dictionary<string, PropertyInfo> propertisyProduktu = typeof(ProduktKlienta).Properties();

            foreach (SortowaniePole s in sortowanie)
            {
                if (s.Pole == "IloscOgraniczonaDoMax")
                {
                    s.Pole = "IloscLaczna";
                }
                bool polezagniezdzone = s.Pole.Contains(".");
                PropertyInfo pi = null;
                if (polezagniezdzone)
                {
                    pi = Refleksja.ZnajdzPropertis(typeof(ProduktKlienta), s.Pole);
                }
                else
                {
                    pi = propertisyProduktu[s.Pole];
                }
                Type t = pi.PropertyType;
                bool liczbowy = t.IsEnum || t.IsAssignableFrom(typeof(decimal)) || t.IsAssignableFrom(typeof(int)) || t.IsAssignableFrom(typeof(WartoscLiczbowa)) || t.IsAssignableFrom(typeof(WartoscLiczbowaZaokraglana));
                bool isEnum = t.IsEnum;
                parametrysortowania.Add(new ParametrySortowanie() {nazwaPola =  s.Pole, parametrEnum =  isEnum, parametrLiczbowy =  liczbowy, polezagniezdzone = polezagniezdzone, propertis =  pi} );
            }

            TypeAccessor akcesor = typeof(ProduktKlienta).PobierzRefleksja();

            foreach (ProduktKategoriaDaneSortowania p in ids)
            {
                List<object> wartosciPolSortowania = new List<object>();
                foreach (ParametrySortowanie s in parametrysortowania)
                {
                    object wartoscDoSortowania;
                    if (s.polezagniezdzone)
                    {
                        wartoscDoSortowania = Refleksja.PobierzWartosc(p.produktKlienta, s.nazwaPola);
                    }
                    else
                    {
                        wartoscDoSortowania =  akcesor[p.produktKlienta, s.propertis.Name];
                    }

                    if (!s.parametrLiczbowy)
                    {
                        wartoscDoSortowania = (wartoscDoSortowania ?? "").ToString();
                    }
                    else if (s.parametrLiczbowy)
                    {
                        if (wartoscDoSortowania is WartoscLiczbowa)
                        {
                            wartoscDoSortowania = ((WartoscLiczbowa)wartoscDoSortowania).Wartosc;
                        }else if (wartoscDoSortowania is WartoscLiczbowaZaokraglana)
                        {
                            wartoscDoSortowania = ((WartoscLiczbowaZaokraglana) wartoscDoSortowania).Wartosc;
                        }
                        else
                        {
                            wartoscDoSortowania = (decimal) wartoscDoSortowania;
                        }
                    }
                    wartosciPolSortowania.Add(wartoscDoSortowania);
                }
                if (wartosciPolSortowania.Any())
                {
                    p.daneDoSortowania = wartosciPolSortowania;
                }
                else
                {
                    throw new Exception("Bład sortowania - nie ma zadnych warotsci? jak to mozliwe?");
                }
            }

            int i = 0;
            IOrderedEnumerable<ProduktKategoriaDaneSortowania> sortowane = ids.OrderBy(p => p.kategoriaSciezka??"zzzzz");
            foreach(var sortPole in sortowanie)
            {
                //musi byc zmienna wewnatrz - nie moze byc globalnej bo order jest robiony dopiero na koncu po WYJSCIU z petli
                int licznikWewnetrzny = i++;
                //ten warunek został przeniesiony do pierwszej pętli for 9735
                //if (sortPole.Pole == "IloscOgraniczonaDoMax" || sortPole.Pole == "IloscLaczna")
                //{
                //    sortowane = sortowane.ThenBy(x => x.produktKlienta.IloscLaczna);
                //}
                //else
                //{
                if (sortPole.KolejnoscSortowania == KolejnoscSortowania.desc)
                    {
                        sortowane = sortowane.ThenByDescending(p => p.daneDoSortowania[licznikWewnetrzny]);
                    }
                    else
                    {
                        sortowane = sortowane.ThenBy(p => p.daneDoSortowania[licznikWewnetrzny]);
                    }
                //}
            }

            return sortowane.ToList();
        }

    }

    public class ParametrySortowanie
    {
        public PropertyInfo propertis { get; set; }
        public bool parametrLiczbowy { get; set; }
        public bool parametrEnum { get; set; }
        public bool polezagniezdzone { get; set; }
        public string nazwaPola { get; set; }
    }


    public class ProduktKategoriaDaneSortowania
    {
        public IProduktKlienta produktKlienta { get; set; }
        public KategorieBLL kategoria { get; set; }
        public string kategoriaSciezka { get; set; }
        public List<object> daneDoSortowania { get; set; }
    }

}