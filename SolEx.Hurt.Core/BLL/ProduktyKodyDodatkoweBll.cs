using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using System.Collections.Generic;
using System.Linq;

namespace SolEx.Hurt.Core.BLL
{
    public class ProduktyKodyDodatkoweBll
    {
        private static ProduktyKodyDodatkoweBll _instancja = new ProduktyKodyDodatkoweBll();

        public static ProduktyKodyDodatkoweBll PobierzInstancje
        {
            get { return _instancja; }
        }

        //public virtual List<ProduktyKodyDodatkowe> PobierzKodyProduktu(long produkt)
        //{
        //    return KodyKreskoweWgProduktow().ContainsKey(produkt)
        //               ? KodyKreskoweWgProduktow()[produkt]
        //               : new List<ProduktyKodyDodatkowe>();
        //}

        public Dictionary<long, List<ProduktyKodyDodatkowe>> KodyKreskoweWgProduktow()
        {
            Dictionary<long, List<ProduktyKodyDodatkowe>> wynik = SolexBllCalosc.PobierzInstancje.Cache.PobierzObiekt<Dictionary<long, List<ProduktyKodyDodatkowe>>>(CacheKodyWgProduktow);
            if (wynik == null)
            {
                wynik = new Dictionary<long, List<ProduktyKodyDodatkowe>>();
                foreach (ProduktyKodyDodatkowe kod in WszystkieKody())
                {
                    if (wynik.ContainsKey(kod.ProduktId))
                    {
                        wynik[kod.ProduktId].Add(kod);
                    }
                    else
                    {
                        wynik.Add(kod.ProduktId, new List<ProduktyKodyDodatkowe> { kod });
                    }
                }
                SolexBllCalosc.PobierzInstancje.Cache.DodajObiekt(CacheKodyWgProduktow, wynik);
            }
            return wynik;
        }

        public List<ProduktyKodyDodatkowe> WszystkieKody()
        {
            List<ProduktyKodyDodatkowe> wynik = SolexBllCalosc.PobierzInstancje.Cache.PobierzObiekt<List<ProduktyKodyDodatkowe>>(CacheKodyWszystkie);
            if (wynik == null)
            {
                wynik = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<ProduktyKodyDodatkowe>(null).ToList();
                SolexBllCalosc.PobierzInstancje.Cache.DodajObiekt(CacheKodyWszystkie, wynik);
            }
            return wynik;
        }

        private const string CacheKodyWszystkie = "produkty_kody_dodatkowe_wszystkie";
        private const string CacheKodyWgProduktow = "produkty_kody_dodatkowe_wg_produktow";

        public List<ProduktyKodyDodatkowe> Aktualizuj(List<ProduktyKodyDodatkowe> data)
        {
            List<long> produktywsystemie = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<ProduktBazowy>(null).Select(x => x.Id).ToList();
            List<long> klienciWSystemie = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<Klient>(null).Select(x => x.Id).ToList();
            data.RemoveAll(x => !produktywsystemie.Contains(x.ProduktId));
            data.RemoveAll(x => x.KlientId != null && !klienciWSystemie.Contains(x.KlientId.GetValueOrDefault()));
            if (data.Count > 0)
            {
                SolexBllCalosc.PobierzInstancje.DostepDane.AktualizujListe<ProduktyKodyDodatkowe>(data);
                UsunCache();
            }
            return data;
        }

        private void UsunCache()
        {
            SolexBllCalosc.PobierzInstancje.Cache.UsunObiekt(CacheKodyWszystkie);
            SolexBllCalosc.PobierzInstancje.Cache.UsunObiekt(CacheKodyWgProduktow);
        }

        public void Usun(List<int> list)
        {
            if (list.Count > 0)
            {
                SolexBllCalosc.PobierzInstancje.DostepDane.Usun<ProduktyKodyDodatkowe, int>(list);
                UsunCache();
            }
        }
    }
}