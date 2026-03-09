using System.Linq;
using System;
using ServiceStack.Common;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.Enums;
using System.Collections.Generic;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core
{
    public class ProduktyUkryteBll: LogikaBiznesBaza
    {
        public ProduktyUkryteBll(ISolexBllCalosc calosc) : base(calosc)
        {
        }

        private List<long> PrzetworzGrupeRegul(IEnumerable<ProduktUkryty> reguly)
        {
           List<long> wynik=new List<long>(100);
           foreach (var rk in reguly)
           {
               if (rk.ProduktZrodloId.HasValue)
               {
                   wynik.Add(rk.ProduktZrodloId.Value);
               }
               else if (rk.KategoriaId.HasValue)
               {
                   if (SolexBllCalosc.PobierzInstancje.ProduktyKategorieDostep.ProduktyKategorieGrupowanePoKategorii.ContainsKey(rk.KategoriaId.Value))
                   {
                       wynik.AddRange(SolexBllCalosc.PobierzInstancje.ProduktyKategorieDostep.ProduktyKategorieGrupowanePoKategorii[rk.KategoriaId.Value]);
                   }
               }
               else if (rk.CechaProduktuId.HasValue)
               {
                   if (SolexBllCalosc.PobierzInstancje.CechyProduktyDostep.WszystkieLacznikiWgCech.ContainsKey(rk.CechaProduktuId.Value))
                   {
                       wynik.AddRange(SolexBllCalosc.PobierzInstancje.CechyProduktyDostep.WszystkieLacznikiWgCech[rk.CechaProduktuId.Value]);
                   }
               }
           }
           return wynik;
       }

        private Dictionary<KatalogKlientaTypy, HashSet<ProduktUkryty>> _slownik_regul_wg_typow = null;
        private string PobierzCacheIdentyfikator(IKlient klient, KatalogKlientaTypy typ)
        {
            return string.Format("ProduktUkryty_k{0}_typ{1}", klient.Id, typ);
        }

        /// <summary>
        /// Na podstawie podanego typu katalogu klienta i ID klienta lub partnera (lub bez ID dla wszystkich klientów) zwraca ID dostępnych produktów
        /// </summary>
        /// <param name="klient">klient/oddział lub null jeśli ma zwrócić ID produktów dla wszystkich</param>
        /// <param name="typ">Typ katalogu klienta (widoczne, ukryte etc)</param>
        /// <returns>Zwraca listę ID produktów widocznych dla danego klienta</returns>
        public HashSet<long> Get(IKlient klient, KatalogKlientaTypy typ)
        {
                if (_slownik_regul_wg_typow == null)
                {
                    _slownik_regul_wg_typow = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<ProduktUkryty>(null).GroupBy(x => x.Tryb).ToDictionary(x => x.Key, x => new HashSet<ProduktUkryty>( x ) );
                }
                if (!_slownik_regul_wg_typow.ContainsKey(typ))
                {
                    return null;
                }

                HashSet<ProduktUkryty> reguly = _slownik_regul_wg_typow[typ];

                HashSet<long> wszystkieID = new HashSet<long>();
                if (klient != null)
                {
                    //reguly na konkretnego klienta - bez znaczenia na inne parametry. w sumie mozna by dodac kontrole ze partner musi sie zgadzac
                    var regkli = reguly.Where(x => x.KlientZrodloId == klient.Id);
                    wszystkieID.UnionWith(PrzetworzGrupeRegul(regkli));

                    //kategoria klientow
                    var regulyKategoriewszystkie = reguly.Where(p => p.KategoriaKlientowId != null && p.KlientZrodloId == null).ToList();
                    if (regulyKategoriewszystkie.Any())
                    {
                        foreach (var kategoriaKliena in klient.Kategorie) //reguly na kategorie klientow
                        {
                            var regulyKategorie = regulyKategoriewszystkie.Where(p => p.KategoriaKlientowId == kategoriaKliena);
                            wszystkieID.UnionWith(PrzetworzGrupeRegul(regulyKategorie));
                        }
                    }
                    //partnerów - ogolna regula
                    var regprzedstawiciele = reguly.Where(x => x.PrzedstawicielId.HasValue && x.PrzedstawicielId == klient.OddzialDoJakiegoNalezyKlient && x.KlientZrodloId == null && x.KategoriaKlientowId == null);
                    wszystkieID.UnionWith(PrzetworzGrupeRegul(regprzedstawiciele));
                }
                //reguly na wszystkich
                var pp = reguly.Where(p => p.KlientZrodloId == null && p.KategoriaKlientowId == null && p.PrzedstawicielId == null);
                wszystkieID.UnionWith(PrzetworzGrupeRegul(pp));

            if (wszystkieID.Count == 0)
            {
                return null;
            }

                return wszystkieID;
        }


        public  void ZapiszAktualizuj(List<ProduktUkryty> doZMiany)
        {
            if (doZMiany.Count > 0)
            {
                SolexBllCalosc.PobierzInstancje.DostepDane.AktualizujListe<ProduktUkryty>(doZMiany);
            }
        }
        
        public  void WyczyszCache(IList<object> zmienioneWpisy = null )
        {
            SolexBllCalosc.PobierzInstancje.Cache.UsunGdzieKluczRozpoczynaSieOd("ofertacalkowita_klienta");
            foreach (Klient k in SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<Klient>(null))
            {
                SolexBllCalosc.PobierzInstancje.ProduktyKlienta.WyczyscCacheProduktyKlienta(k);
            }
            _slownik_regul_wg_typow = null;
        }


        /// <summary>
        /// Metoda NIE cachowana - cahcowane jest na produkty klienta- id produktów rzeczywiście dostępnych dla klienta
        /// </summary>
        /// <param name="klient"></param>
        /// <returns></returns>
        public virtual List<ProduktBazowy> PobierzProduktyDostepneDlaKlienta(IKlient klient)
        {
            if (klient.Dostep == AccesLevel.Niezalogowani && klient.OddzialDoJakiegoNalezyKlient != 0)
            {
                klient = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<Klient>(klient.OddzialDoJakiegoNalezyKlient);
            }

            //nie cachujemy - bo cachuje sie wyzej
            List<ProduktBazowy> produkt = null;
                IKlient k = klient.KlientPodstawowy();
                if (SolexBllCalosc.PobierzInstancje.Konfiguracja.GetLicense(Licencje.katalog_klienta) && klient.Id != 0 && !klient.CzyAdministrator) //jesli jest adminem to ZAWSZE ma wszystkie produkty - bez zawężania
                    {
                        produkt = new List<ProduktBazowy>();
                        HashSet<long> dostepne = Get(k, KatalogKlientaTypy.Dostepne);
                        if (dostepne != null)
                        {
                            var temp = Get(klient, KatalogKlientaTypy.Dostepne);
                            if (temp != null)
                            {
                                dostepne.UnionWith(temp);
                            }
                        }
                        else
                        {
                            dostepne = Get(klient, KatalogKlientaTypy.Dostepne);
                        }

                        HashSet<long> mojkatalog = Get(k, KatalogKlientaTypy.MojKatalog);
                        if (mojkatalog != null)
                        {
                            var temp = Get(klient, KatalogKlientaTypy.MojKatalog);
                            if (temp != null)
                            {
                                mojkatalog.UnionWith(temp);
                            }
                        }
                        else
                        {
                            mojkatalog = Get(klient, KatalogKlientaTypy.MojKatalog);
                        }

                        HashSet<long> wykluczone = Get(k, KatalogKlientaTypy.Wykluczenia);

                        IList<ProduktBazowy> wszystkie = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<ProduktBazowy>(null);

                        foreach (var produktyBazowy in wszystkie)
                        {
                            if (produktyBazowy.Jednostki == null || produktyBazowy.Jednostki.IsEmpty() || !produktyBazowy.Jednostki.Any(x=> x.Aktywna)) continue;
                            if (!produktyBazowy.Widoczny) continue;
                            if (produktyBazowy.Widocznosc != AccesLevel.Wszyscy && produktyBazowy.Widocznosc != k.Dostep) continue;
                            if (produktyBazowy.PrzedstawicielId != null && produktyBazowy.PrzedstawicielId != k.PrzedstawicielId && produktyBazowy.PrzedstawicielId != k.Id) continue;
                            if ( (dostepne != null && dostepne.Contains(produktyBazowy.Id)) || (mojkatalog != null && mojkatalog.Contains(produktyBazowy.Id))  )
                            {
                                produkt.Add(produktyBazowy);
                                continue;
                            }

                            if (klient.PelnaOferta && (wykluczone == null || !wykluczone.Contains(produktyBazowy.Id)   ) )
                            {
                                produkt.Add(produktyBazowy);
                            }
                        }
                    }
                    else
                    {
                        produkt = BLL.SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<ProduktBazowy>(null, p => p.Widoczny && (p.Widocznosc == AccesLevel.Wszyscy || p.Widocznosc == k.Dostep),null,1,int.MaxValue).ToList();
                    }
            return produkt;
        }

       
    }
}
