using System;
using System.Collections.Generic;
using System.Linq;
using ServiceStack.Common;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.CustomSearchCriteria;

namespace SolEx.Hurt.Core
{
    public class JednostkaProduktuBll : LogikaBiznesBaza, IJednostkaProduktuBll
    {       
        public JednostkaProduktuBll(ISolexBllCalosc calosc) : base(calosc)
        {
        }
      
        public  string CacheNameProduktyJednostkiLista(int jezyk)
        {
            return string.Format("jednostki_jezyk_{0}",jezyk);
        }

        public  Dictionary<long, Jednostka> PobierzJednostki(int jezyk)
        {
            Dictionary<long, Jednostka> wynik = SolexBllCalosc.PobierzInstancje.Cache.PobierzObiekt<Dictionary<long, Jednostka>>(CacheNameProduktyJednostkiLista(jezyk));
            if (wynik == null)
            {
                wynik=new Dictionary<long, Jednostka>();
                var baza = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<Jednostka>(null).ToList();
                for (int i = 0; i < baza.Count; i++)
                {
                    var tmp = baza[i];
                    //wywalaka tlumaczenia przez lokalizacjabll
                    wynik.Add(tmp.Id,tmp);
                }
                Calosc.Cache.DodajObiekt(CacheNameProduktyJednostkiLista(jezyk),wynik);
            }
            return wynik;

        }
        
        public void PoprawJednostkiPrzedAktualizacja(IList<Jednostka> obj)
        {
            if (obj.Any(a => string.IsNullOrEmpty(a.Nazwa.Trim())) )
            {
                throw new Exception("Próba zapisania pustych jednostek");
            }

            //var obecneJednostki = this.PobierzJednostki(Calosc.Konfiguracja.JezykIDDomyslny);

            //foreach (var j in obj)
            //{
            //    if (obecneJednostki.Values.Any(x => x.Id != j.Id && (x.Nazwa == j.Nazwa || x.Nazwa.TrimEnd('.') == j.Nazwa.TrimEnd('.')) && x.Aktywna==j.Aktywna))
            //    {
            //        throw new Exception("Próba zapisania zdublowanej nazwy jednostki: " + j.Nazwa);
            //    }
            //}
        }
     
        public void UsunCache(IList<object> obj)
        {
            foreach (var jezyk in SolexBllCalosc.PobierzInstancje.Konfiguracja.JezykiWSystemie)
            {
                Calosc.Cache.UsunObiekt(CacheNameProduktyJednostkiLista(jezyk.Key));
            }
            Calosc.Cache.UsunGdzieKluczRozpoczynaSieOd(Calosc.DostepDane.KluczCacheTypDanych<ProduktBazowy>());
        }
    }
}
