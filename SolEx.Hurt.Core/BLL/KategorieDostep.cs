using System;
using ServiceStack.Common;
using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Core.DostepDane;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Web;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core.BLL
{
    public class KategorieDostep : LogikaBiznesBaza, IKategorieDostep
    {
        public KategorieDostep(ISolexBllCalosc calosc) : base(calosc)
        {
        }

        public GrupaBLL PobierzIdGrupyKomplementarnej(KategorieBLL kategoria, IKlient klient, int lang)
        {
            GrupaBLL wynik = null;
            if (kategoria == null)
            {
                //ladujemy kategorie kompletanrana z wyszukiwarki
                wynik = Calosc.DostepDane.Pobierz<GrupaBLL>(null, x => x.GrupujWyszukiwanie).FirstOrDefault();
            }
            else
            {
                if (kategoria.Grupa.GrupaKomplementarnaId.HasValue)
                {
                    wynik = Calosc.DostepDane.PobierzPojedynczy<GrupaBLL>(kategoria.Grupa.GrupaKomplementarnaId, lang, klient);
                }
            }

            return wynik;
        }
        
        public HashSet<long> PobierzWszystkieKategorie(HashSet<long> katIDs, bool wszystkiepoziomy)
        {
            HashSet<long> wynik = new HashSet<long>();
            if (katIDs == null)
            {
                return wynik;
            }

            wynik = new HashSet<long>(katIDs);

            if (Calosc.Konfiguracja.WielowybieralnoscKategorii)
            {
                var kategorie = Calosc.DostepDane.Pobierz<KategorieBLL>(null, x => katIDs.Contains(x.Id));
                foreach (var k in kategorie)
                {
                     wynik.UnionWith(k.PobierzIdWszystkichDzieci());
                }
            }
           
            return wynik;
        }

        //public int LiczbaProduktow(KategorieBLL kategoria, IKlient k, Dictionary<int, HashSet<long>> stale, string szukaneGlobalne, int jezyk)
        //{
        //    //todo! MEGA PROBLEM!!
        //    HashSet<long> kategorieId = PobierzWszystkieKategorie(new HashSet<long> { kategoria.Id }, true);
        //    var produkty = Calosc.ProduktyKlienta.ProduktySpelniajaceKryteria(kategorieId, szukaneGlobalne, k, jezyk, null, stale, null);   //do liczenia produktów nie bierzemy szukania wew. kategorii
        //    return produkty.Count;
        //}

        public List<KategorieBLL> PobierzDzieci(KategorieBLL kategorie)
        {
            HashSet<long> ids = PobierzStukure(kategorie.Id);
            if (ids.IsEmpty())
            {
                return new List<KategorieBLL>();
            }

            return Calosc.DostepDane.Pobierz(kategorie.JezykId, null, x => ids.Contains(x.Id), new[]
            {
                new SortowanieKryteria<KategorieBLL>(x=>x.Kolejnosc,KolejnoscSortowania.asc, "Kolejnosc"),
                new SortowanieKryteria<KategorieBLL>(x=>x.Nazwa,KolejnoscSortowania.asc, "Nazwa")
            }).ToList();
        }

        private HashSet<long> PobierzStukure(long idParenta)
        {
            Dictionary<long, HashSet<long>> result = Calosc.Cache.PobierzObiekt<Dictionary<long, HashSet<long>>>(GetCacheNameSturktura());
            if (result == null)
            {
                result = new Dictionary<long, HashSet<long>>();
                var bazowe = Calosc.DostepDane.Pobierz<KategorieBLL>(null);
                foreach (var k in bazowe)
                {
                    long parent = k.ParentId.GetValueOrDefault();
                    if (!result.ContainsKey(parent))
                    {
                        result.Add(parent, new HashSet<long>());
                    }
                    result[parent].Add(k.Id);
                }
                Calosc.Cache.DodajObiekt(GetCacheNameSturktura(), result);
            }
            return result.ContainsKey(idParenta) ? result[idParenta] : new HashSet<long>();
        }

        /// <summary>
        /// metoda walidatora - nie trzeba jej samodzielnie wykonywac
        /// </summary>
        /// <param name="grupa"></param>
        /// <param name="klient"></param>
        /// <returns></returns>
        public bool JestWidocznaDlaKlienta(GrupaBLL grupa, IKlient klient)
        {
            //metoda do walidatora grupy
            if (grupa.Widoczna && (grupa.Dostep == AccesLevel.Wszyscy || grupa.Dostep == klient.Dostep))
            {
                //nie chcemy tego warunku bo wplywa na wydajnosc bardzo, a poza tym robi sie petla wywolania bo w kategoriach walidujemy po grupie i jest w nieskaczonosc zabawa
               // return grupa.PobierzKategorie(klient).Any(p => SolexBllCalosc.PobierzInstancje.KategorieDostep.JestWidocznaDlaKlienta(p, klient));
                return true;
            }
            return false;
        }

        /// <summary>
        /// metoda dla walidatra - nie trzeba jej recznie uruchamiac
        /// </summary>
        /// <param name="kategoria"></param>
        /// <param name="klient"></param>
        /// <param name="dlaRabatu"></param>
        /// <returns></returns>
        public bool JestWidocznaDlaKlienta(KategorieBLL kategoria, IKlient klient)
        {
            if (!kategoria.Widoczna) return false;

            if (kategoria.Dostep != AccesLevel.Wszyscy && klient.Dostep != kategoria.Dostep)
            {
                return false;
            }

            var grupy = kategoria.Grupa;
            if (grupy == null)
            {
                return false;
            }

            if (!grupy.Widoczna)    //&& ( Calosc.Konfiguracja.PokazujRabatyTylkoZWidocznychGrup)
            {
                return false;
            }

            if (grupy.Dostep != AccesLevel.Wszyscy && grupy.Dostep != klient.Dostep)
            {
                return false;
            }

            return true;
        }

        public IList<KategorieBLL> MetodaPrzetwarzajacaPoSelect(int jezykId, IKlient klient, IList<KategorieBLL> kats, object arg4)
        {
            Dictionary<long, GrupaBLL> grupy = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<GrupaBLL>(jezykId, null).ToDictionary(x => x.Id, x => x);
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            //!!!!!!!!!! wszystkie propertisy tu uzuplenine trzeba dopisać do konstrktora kopiujacego !!!!!!!!!!!!!!
            //!!!!!!!!!!~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            foreach (KategorieBLL kat in kats)
            {
                //zmiana gwiazdki w linku na litere a - specjalnie dla Bio Planet
                kat.FriendlyLinkURL = Tools.OczyscCiagDoLinkuURL(kat.Nazwa.Replace('*', 'a'));
                kat.Grupa = grupy[kat.GrupaId];
            }

            return kats;
        }
        
        /// <summary>
        /// ABSOLUTNIE NIE WOLNO używać tej metody do wyciągania kategorii produktów dostepnych dla klienta z wszystkimi jego produktami!!
        /// </summary>
        /// <param name="kategoria"></param>
        /// <param name="klient"></param>
        /// <param name="rozpartywanePRodukty"></param>
        /// <returns></returns>
        public bool JestWidocznaDlaKlienta(KategorieBLL kategoria, IKlient klient,  HashSet<long> rozpartywanePRodukty)
        {
            if (rozpartywanePRodukty.IsEmpty())
            {
                return false; //skoro żaden produkt nie pasuje do szukania to na pewno będzie niewidoczna
            }

            //bool wyndz = kategoria.Dzieci.Any(kat => JestWidocznaDlaKlienta(kat, klient, rozpartywanePRodukty));
            //if (wyndz)
            //{
            //    return true;
            //}
            HashSet<long> ids;
            if (Calosc.ProduktyKategorieDostep.ProduktyKategorieGrupowanePoKategorii.TryGetValue(kategoria.Id, out ids))
            {
                if (rozpartywanePRodukty.Overlaps(ids))
                {
                    return true;
                }
            }
            return false;
        }

        public List<IProduktKlienta> PobierzProdukty(KategorieBLL kategoria, IKlient k = null, Dictionary<int, HashSet<long>> stale = null)
        {
            if (k == null)
            {
                k = Calosc.Klienci.KlientNiezalogowany();
            }
           // long? kategorieId = PobierzWszystkieKategorie( kategoria.Id , Calosc.Konfiguracja.WielowybieralnoscKategorii);
            var ids = Calosc.ProduktyKlienta.ProduktySpelniajaceKryteria(kategoria.Id, null, k, kategoria.JezykId, new Dictionary<int, HashSet<long>>(), stale, null);
            return ids.Select(x => (IProduktKlienta)x).ToList();
        }

        public KategorieBLL KategoriaNadrzedna(KategorieBLL kategoria)
        {
            if (kategoria.ParentId == null) return null;
            return Calosc.DostepDane.PobierzPojedynczy<KategorieBLL>(kategoria.ParentId.Value, kategoria.JezykId);
        }

        private string GetCacheNameSturktura()
        {
            return string.Format("kategorie_struktura");
        }

        /// <summary>
        /// glowna metoda do pobiernai kategorii dla klientow
        /// </summary>
        /// <param name="idGrupy"></param>
        /// <param name="jezykId"></param>
        /// <param name="klient"></param>
        /// <param name="rozpartywanePRodukty"></param>
        /// <returns></returns>
        public IList<KategorieBLL> PobierzDrzewkoKategorii(long idGrupy, int jezykId, IKlient klient, HashSet<long> rozpartywanePRodukty = null)
        {
            Expression<Func<KategorieBLL, bool>> filtr = x => x.GrupaId == idGrupy;

            //jesli sa podane ropatrywane produkty to najpier wyciagmy kategorie z tymi produktami -z powodu wielokatygorowosci
            if (rozpartywanePRodukty != null)
            {
                if (rozpartywanePRodukty.IsEmpty())
                {
                    return null;    //nic nie bedzie bo nie ma prodktow
                }

                List<HashSet<long>> kategorieProduktu = Calosc.ProduktyKategorieDostep.ProduktyKategorieGrupowanePoProdukcie.WhereKeyIsIn(rozpartywanePRodukty);
                HashSet<long> ids = new HashSet<long>();
                foreach (var k in kategorieProduktu)
                {
                    ids.UnionWith(k);
                }

                if (ids.IsEmpty())
                {
                    return null;
                }

                filtr = x => ids.Contains(x.Id) && x.GrupaId == idGrupy;
            }

            var lista = Calosc.DostepDane.Pobierz<KategorieBLL>(jezykId, klient, filtr, new[]
            {
                new SortowanieKryteria<KategorieBLL>(p => p.Kolejnosc, KolejnoscSortowania.asc, "Kolejnosc" ),
                new SortowanieKryteria<KategorieBLL>(p => p.Nazwa, KolejnoscSortowania.asc, "Nazwa")
            });


            if (!SolexBllCalosc.PobierzInstancje.Konfiguracja.WielowybieralnoscKategorii)
            {
                //jak nie ma wielowybieralnosci - to musimy sie upewnic ze mamy wyciagnietych wszystkich parentow 
                //- w przecinym wypadku nie wyrenderuje sie nic bo renderowanie idzie od ojca do dziecka

            }

            return lista;
        }

        public void Usun(IEnumerable<long> ids)
        {
            foreach (long i in ids)
            {
                KategorieBLL kat = Calosc.DostepDane.PobierzPojedynczy<KategorieBLL>(i);
                if (kat == null)
                {
                    continue;
                }
                foreach (KategorieBLL t in kat.Dzieci)
                {
                    Usun(new HashSet<long> { t.Id });
                }
                Calosc.DostepDane.UsunPojedynczy<KategorieBLL>(i);
            }
        }

        //private static Dictionary<long, Dictionary<long, KategorieBLL>> _kategorieDostepneDlaKlientow = new Dictionary<long, Dictionary<long, KategorieBLL>>();

        private static object lok = new object();

        public Dictionary<long,KategorieBLL> PobierzKategorieDostepneDlaKlienta(IKlient klient)
        {
            return Calosc.Cache.PobierzObiekt(() =>
            {
                return SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<KategorieBLL>(klient).ToDictionary(x => x.Id, x => x);
            }, lok, "kategorieKlienta_{0}", klient.Id);
        }

        public KategorieBLL CzyNaPodstwieCechy(CechyBll cecha)
        {
            return Calosc.DostepDane.PobierzPojedynczy<KategorieBLL>(cecha.Id);
        }

        public void AktualizujKategorie(IList<KategoriaProduktu> list)
        {
            List<KategoriaProduktu> kolekcjaBezParenta = list.Select(a => new KategoriaProduktu(a)).ToList(); 
            kolekcjaBezParenta.ForEach(x => x.ParentId = null);
            SolexBllCalosc.PobierzInstancje.DostepDane.AktualizujListe<KategoriaProduktu>(kolekcjaBezParenta);
            SolexBllCalosc.PobierzInstancje.DostepDane.AktualizujListe<KategoriaProduktu>(list);
        }

        public void UsunCache(IList<object> obj)
        {
            Calosc.Cache.UsunGdzieKluczRozpoczynaSieOd("kategorieKlienta_");
            Calosc.Cache.UsunGdzieKluczRozpoczynaSieOd(Calosc.DostepDane.KluczCacheTypDanych<KategorieBLL>());
        }
    }
}