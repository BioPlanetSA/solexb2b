using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ServiceStack.Common;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.Sync;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Interfaces.Sync;
using SolEx.Hurt.Sync.App.Modules_.Rozne;
using SolEx.Hurt.Model.Interfaces;
using SolEx.Hurt.Model.Interfaces.SyncModuly;


namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Produkty
{
    public class SMMASH_ProduktyZPlikow : ProduktySMMASH,IModulProdukty,IModulRabaty
    {
        public IConfigSynchro Konfiguracja = SyncManager.PobierzInstancje.Konfiguracja;

        [FriendlyName("Nazwa atrybutu z ścieżką do folderu")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string NazwaAtrybutuFolderu { get; set; }
        
        [FriendlyName("Ścieżka na dysku do katalogu z projektami")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string SciezkaFolderNaDysku { get; set; }

        [FriendlyName("Id atrybutu rozmiarów")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public int idAtrybutuRozmiar { get; set; }

        [FriendlyName("Id atrybutu nazwy rodzin")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public int idAtrybutuRodzin{ get; set; }

        public void Przetworz(ref List<Produkt> listaWejsciowa, ref List<Tlumaczenie> produktyTlumaczenia, Dictionary<long, Produkt> produktyNaB2B,
            ref List<JednostkaProduktu> jednostki, ref Dictionary<long, ProduktCecha> lacznikiCech, ref List<ProduktKategoria> lacznikiKategorii, 
            ISyncProvider provider, ref List<ProduktUkryty> produktuUkryteErp, ref List<ProduktyZamienniki> zamienniki, Dictionary<long, KategoriaProduktu> kategorie,
            ref List<Cecha> cechy, ref List<Atrybut> atrybuty)
        {
            PrzetworzGlobal(ref listaWejsciowa, produktyNaB2B, ref jednostki, ref lacznikiCech, ref lacznikiKategorii, ref produktuUkryteErp, ref produktyTlumaczenia);   
        }

        public void Przetworz(ref List<Rabat> rabatyNaB2B, ref List<ProduktUkryty> produktyUkryteNaB2B, ref Dictionary<long, Konfekcje> konfekcjaNaB2B,
            IDictionary<long, Klient> dlaKogoLiczyc, Dictionary<long, Produkt> produkty, List<PoziomCenowy> ceny, List<Cecha> cechy, 
            Dictionary<long, ProduktCecha> cechyProdukty, Dictionary<long, KategoriaProduktu> kategorie, List<ProduktKategoria> produktyKategorie,
            ref IDictionary<int, KategoriaKlienta> kategorieKlientow, ref IDictionary<long, KlientKategoriaKlienta> klienciKategorie)
        {
            List<Produkt> listaWejsciowa = produkty.Values.AsParallel().Where(a => a.PoleLiczba1 == null).ToList();
            List<JednostkaProduktu> jednostki = new List<JednostkaProduktu>();
            List<Tlumaczenie> produktyTlumaczenia=new List<Tlumaczenie>();

            PrzetworzGlobal(ref listaWejsciowa, produkty, ref jednostki, ref cechyProdukty, ref produktyKategorie, ref produktyUkryteNaB2B, ref produktyTlumaczenia);   
        }

        private void PrzetworzGlobal(ref List<Produkt> listaWejsciowa, Dictionary<long, Produkt> produktyNaB2B, ref List<JednostkaProduktu> jednostki, 
            ref Dictionary<long,ProduktCecha> lacznikiCech, ref List< ProduktKategoria> lacznikiKategorii, ref List< ProduktUkryty> produktuUkryteErp,ref List<Tlumaczenie> produktyTlumaczenia)
        {
            HashSet<long> listawejsciowaslownik = new HashSet<long>( listaWejsciowa.Select(x => x.Id) );
            Dictionary<long, List<ProduktKategoria>> lacznikkategoriiwgproduktu = lacznikiKategorii.GroupBy(x => x.ProduktId).ToDictionary(x => x.Key, x => x.ToList());
            if (!Directory.Exists(SciezkaFolderNaDysku))
            {
                throw new InvalidOperationException("Ścieżka nie istnieje: " + SciezkaFolderNaDysku);
            }
            if (idAtrybutuRozmiar == 0)
            {
                throw new Exception("Brak definicji atrybutu dla rozmiarów");
            }
            if (idAtrybutuRodzin == 0)
            {
                throw new Exception("Brak definicji atrybutu dla rodzin");
            }
               Dictionary<long, List<JednostkaProduktu>> jednosttkiwgproduktow = jednostki.GroupBy(x => x.ProduktId).ToDictionary(x => x.Key, x => x.ToList());
            Dictionary<long, HashSet<long>> lacznikichechwgproduktow = lacznikiCech.Values.GroupBy(x => x.ProduktId).ToDictionary(x => x.Key, x => new HashSet<long>( x.Select(y=>y.CechaId) ) );
            var atrybuty = ApiWywolanie.PobierzAtrybuty().Values;
            var cechy = ApiWywolanie.PobierzCechy().Values;
            var tablicaCechRodzn = cechy.Where(x => x.AtrybutId == idAtrybutuRodzin).ToList();
            var tablicacechrozmiarow = cechy.Where(x => x.AtrybutId == idAtrybutuRozmiar).ToList();
            IEnumerable<Klient> klienci = ApiWywolanie.PobierzKlientow().Values;
            var atrybut = atrybuty.FirstOrDefault(a => a.Nazwa.ToLower() == NazwaAtrybutuFolderu.ToLower());
            if (atrybut == null)
            {
                Log.Error("Nie znaleziono atrybutu o nazwie (wielkość liter ma znaczenie):  " + NazwaAtrybutuFolderu);
                return;
            }
            List<Cecha> foldery = cechy.Where(x => x.AtrybutId == atrybut.Id).ToList();
            Log.DebugFormat("Lista cech dla wirtualnych folderów: {0}", foldery.Count);
            Dictionary<long, Produkt> podstawowe = listaWejsciowa.Where(x => x.PoleLiczba1 == null).ToDictionary(x => x.Id, x => x);
            Dictionary<Cecha, List<Tuple<Produkt, string, string>>> bazowe = new Dictionary<Cecha, List<Tuple<Produkt, string, string>>>(foldery.Count);

            int stworzoneProdukty = 0;

            foreach (Cecha c in foldery)
            {
                List<ProduktCecha> laczniki = lacznikiCech.Where(x => x.Value.CechaId == c.Id).Select(x => x.Value).ToList();
                if (laczniki.Count == 0)
                {
                    Log.Warn("Brak produktu dla folderu:" + c.Symbol);
                    continue;
                }
                bazowe.Add(c, new List<Tuple<Produkt, string, string>>(laczniki.Count));
                HashSet<long> idspodstawowych = new HashSet<long>( laczniki.Select(x => x.ProduktId) );
                var produktycechy = podstawowe.WhereKeyIsIn(idspodstawowych);
                foreach (var produkt in produktycechy)
                {
                    string rozmiar = PobierzRozmiar(produkt.Nazwa);
                    string bazowyKodBezRozmiaru = PobierzBazowyKodBezRozmiaru(produkt.Kod, rozmiar);
                    Tuple<Produkt, string, string> dane = new Tuple<Produkt, string, string>(produkt, rozmiar, bazowyKodBezRozmiaru);
                    bazowe[c].Add(dane); 
                }
            }

            if(produktuUkryteErp == null)
                produktuUkryteErp  = new List<ProduktUkryty>();
            foreach (Klient k in klienci)
            {
                foreach (Cecha f in bazowe.Keys)
                {
                    string sciezkaProjektu = Path.Combine(SciezkaFolderNaDysku, k.Symbol, f.Nazwa);
                    if (!Directory.Exists(sciezkaProjektu))
                    {
                        Log.ErrorFormat("Brak plików dla klienta {0} - katalog {1} nie istnieje lub nie ma do niego dostępu", k.Symbol, sciezkaProjektu);
                        continue; 
                    }
                    string[] listaPlikow = Directory.GetFiles(sciezkaProjektu);
                    foreach (string plik in listaPlikow)
                    {
                        string ext = Path.GetExtension(plik) ?? "";                            //wskaznik do zdjecia
                        string nazwzdjecja = TextHelper.PobierzInstancje.OczyscNazwePliku(plik.Replace("/", "\\").Replace(SciezkaFolderNaDysku, "", StringComparison.InvariantCultureIgnoreCase).Replace(ext, "", StringComparison.InvariantCultureIgnoreCase)).Trim(new[] { '-' }).ToLower();
                        if (nazwzdjecja.Contains("@"))
                        {
                            nazwzdjecja = nazwzdjecja.Substring(0, nazwzdjecja.IndexOf("@", StringComparison.Ordinal));
                        }
                        string nazwapliku = (Path.GetFileNameWithoutExtension(plik)??"");
                        if (nazwapliku.Contains("@"))
                        {
                            nazwapliku = nazwapliku.Substring(0, nazwapliku.IndexOf("@", StringComparison.Ordinal));
                        }
                        int pozycja = nazwapliku.IndexOf("VISUAL", StringComparison.InvariantCultureIgnoreCase);
                        if (pozycja < 0)
                        {
                            continue;
                        }
                        string nazwaTymczasowaPliku = nazwapliku.Substring(pozycja);
                        if (nazwaTymczasowaPliku.Contains("@"))
                        {
                            nazwaTymczasowaPliku = nazwaTymczasowaPliku.Split('@')[0];//bierzemy czesc do malpy, reszta to numer fotki
                        }
                        string nazwaProjektu = nazwaTymczasowaPliku;
                        foreach (Tuple<Produkt, string, string> baza in bazowe[f])                        //zakldanie produktow wirtualnych
                        {
                            string rozmiar = baza.Item2;
                            string bazowyKodBezRozmiaru = baza.Item3;
                            string nowyKod = (bazowyKodBezRozmiaru + "-" + k.Symbol + "-" + nazwaProjektu).Replace(" ", "").Replace("--", "-").Trim();  

                            if (!rozmiar.IsNullOrEmpty())
                            {
                                nowyKod += "-" + rozmiar;
                            }
                            nowyKod = nowyKod.ToUpper();
                            long idWyliczone = Math.Abs((1000000 * baza.Item1.Id) + (k.Id * 1000) + Math.Abs(nowyKod.GetHashCode()));
                    
                            if (listawejsciowaslownik.Contains(idWyliczone))
                            {
                                Log.ErrorFormat("Produkt o kodzie {0} już istnieje. Prawdopodobnie zdublowany projekt {1} w ścieżce {2}", nowyKod, nazwaProjektu, sciezkaProjektu);
                                continue;
                            }
                            listawejsciowaslownik.Add(idWyliczone);
                            Produkt nowyProdukt = new Produkt { DataDodania = DateTime.Now, Id = idWyliczone };
                            ++stworzoneProdukty;
                            if (produktyNaB2B.ContainsKey(idWyliczone))
                            {
                                nowyProdukt.DataDodania =produktyNaB2B[idWyliczone].DataDodania;
                            }
                            nowyProdukt.KopiujPola(baza.Item1, new { nowyProdukt.Id, nowyProdukt.DataDodania, kod = nowyProdukt.Kod, kod_kreskowy = nowyProdukt.KodKreskowy, nazwa = nowyProdukt.Nazwa });
                            nowyProdukt.Kod = nowyKod;
                            nowyProdukt.Nazwa = nazwaProjektu.ToUpper();
                            nowyProdukt.Rodzina = nazwaProjektu.ToUpper();  //dodanie rodziny
                            nowyProdukt.PoleLiczba2 = k.Id;
                            nowyProdukt.KodKreskowy = null;
                            nowyProdukt.Widoczny = true;
                            nowyProdukt.PoleLiczba1 = baza.Item1.Id; //wskaznik na towar bazowy
                            nowyProdukt.PoleTekst1 = nazwzdjecja;
                            nowyProdukt.PoleTekst2 = nazwaTymczasowaPliku;
                            
                            listaWejsciowa.Add(nowyProdukt);
                            if (jednosttkiwgproduktow.ContainsKey(baza.Item1.Id))
                            {
                                foreach (JednostkaProduktu j in jednosttkiwgproduktow[baza.Item1.Id]) //przepisanie jednostki
                                {
                                    JednostkaProduktu jTemp = new JednostkaProduktu();
                                    jTemp.ProduktId = nowyProdukt.Id;
                                    jTemp.Przelicznik = j.Przelicznik;
                                    jTemp.Podstawowa = j.Podstawowa;
                                    jTemp.Nazwa = j.Nazwa;
                                    jTemp.Id = j.Id;
                                    jednostki.Add(jTemp);
                                }
                            }

                            HashSet<long> cp = lacznikichechwgproduktow.ContainsKey(baza.Item1.Id) ? lacznikichechwgproduktow[baza.Item1.Id] : new HashSet<long>();
                            if (!lacznikichechwgproduktow.ContainsKey(nowyProdukt.Id))
                            {
                                lacznikichechwgproduktow.Add(nowyProdukt.Id, new HashSet<long>());
                            }
                            foreach (int c in cp)
                            {
                                if(tablicacechrozmiarow.Any(x=>x.Id==c)) continue; //pomijamy rozmiary

                                if (lacznikichechwgproduktow[nowyProdukt.Id].Contains(c))
                                {
                                    continue;
                                }
                                ProduktCecha cTemp = new ProduktCecha { ProduktId = nowyProdukt.Id, CechaId = c };
                                lacznikiCech.Add(cTemp.Id, cTemp);
                                lacznikichechwgproduktow[nowyProdukt.Id].Add(cTemp.CechaId);
                                var rodz = tablicaCechRodzn.FirstOrDefault(x => x.Id == c);
                                if (rodz != null)
                                {
                                    string prefix = rodz.Nazwa;
                                    if (!prefix.EndsWith("-"))
                                    {
                                        prefix += "-";
                                    }
                                    foreach (var j in Konfiguracja.JezykiWSystemie)
                                    {
                                        if(j.Value.Domyslny) continue;
                                        Tlumaczenie s=new Tlumaczenie();
                                        s.ObiektId = nowyProdukt.Id;
                                        s.Typ = typeof (ProduktBazowy).PobierzOpisTypu();
                                        s.Pole = "rodzina";
                                        s.JezykId = j.Key;
                                        s.Wpis= Konfiguracja.PobierzTlumaczenie(j.Key, prefix) + nowyProdukt.Rodzina;
                                        produktyTlumaczenia.Add(s);
                                    }
                                    nowyProdukt.Rodzina = Konfiguracja.PobierzTlumaczenie(Konfiguracja.JezykIDPolski, prefix) + nowyProdukt.Rodzina;
                                }
                            }
                            var r = tablicacechrozmiarow.FirstOrDefault(x => x.Nazwa == rozmiar);
                            if (r != null && !lacznikichechwgproduktow[nowyProdukt.Id].Contains(r.Id))
                            {
                                ProduktCecha cTemp = new ProduktCecha();
                                cTemp.ProduktId = nowyProdukt.Id;
                                cTemp.CechaId = r.Id;
                                lacznikiCech.Add(cTemp.Id, cTemp);
                                lacznikichechwgproduktow[nowyProdukt.Id].Add(r.Id);
                            }
                            List<ProduktKategoria> kats = lacznikkategoriiwgproduktu.ContainsKey(baza.Item1.Id) ? lacznikkategoriiwgproduktu[baza.Item1.Id] : new List<ProduktKategoria>();
                            foreach (ProduktKategoria pk in kats)
                            {
                                if (!lacznikkategoriiwgproduktu.ContainsKey(nowyProdukt.Id))
                                {
                                    lacznikkategoriiwgproduktu.Add(nowyProdukt.Id, new List<ProduktKategoria>());
                                }
                                if (lacznikkategoriiwgproduktu[nowyProdukt.Id].Any(x => x.KategoriaId == pk.KategoriaId))
                                {
                                    continue;
                                }
                                    ProduktKategoria pkTemp = new ProduktKategoria(pk);
                                    pkTemp.ProduktId = nowyProdukt.Id;
                                    pkTemp.Rodzaj = -10;
                                    lacznikiKategorii.Add( pkTemp);
                                    lacznikkategoriiwgproduktu[nowyProdukt.Id].Add(pkTemp);
                            }
                            ProduktUkryty u = new ProduktUkryty { KlientZrodloId = k.Id, ProduktZrodloId = idWyliczone, Tryb = KatalogKlientaTypy.MojKatalog };
              
                                produktuUkryteErp.Add( u);
                        }
                    }
                }
            }
            LogiFormatki.PobierzInstancje.LogujDebug(string.Format("Koniec modułu produktów z plików. Dodano {0} produtków wirtualnych z pliku", stworzoneProdukty));
        }

        public override string uwagi
        {
            get { return "Moduł dla SMMASH, tworzący wirtualne produkty na podstawie katalogów na dysku";  }
        }
    }
}
