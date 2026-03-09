using System;
using System.Collections.Generic;
using System.Linq;
using ServiceStack.Common;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Web.Site2.Models.KontrolkiTresci.Bazowe;

namespace SolEx.Hurt.Web.Site2.Models.KontrolkiTresci
{
    public class WybraneProdukty : LosowaListaProduktowWybraneIdProduktow
    {
        public WybraneProdukty()
        {
            LiczbaProduktow = 8;
        }
        public override string Grupa => "Produkty";

        public override string Nazwa => "Losowa lista produktów";

        [Niewymagane]
        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Lista cech dla losowych produktów")]
        [PobieranieSlownika(typeof(SlownikCech))]
        public long[] ListaCech { get; set; }

        [Niewymagane]
        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Kategoria dla losowych produktów")]
        [PobieranieSlownika(typeof(SlownikKategoriiProduktow))]
        public long? ListaKategorii { get; set; }

        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Liczba produktów losowanych do wyświetlenia")]
        public int LiczbaProduktow { get; set; }

        public override HashSet<long> ListaProduktowId
        {
            get
            {
                Dictionary<string, long> wynik = new Dictionary<string, long>();
                if (ListaProduktow!=null && ListaProduktow.Any())
                {
                    return ListaProduktow;
                }
                if ((ListaCech == null || !ListaCech.Any()) && (!ListaKategorii.HasValue ))
                {
                    throw new Exception("Puste cechy i kategorie");
                }
                var slc = new Dictionary<int, HashSet<long>>();
                if (ListaCech != null && ListaCech.Any())
                {
                    slc.Add(0, new HashSet<long>( ListaCech ));
                }
                Dictionary<long, string> listaProduktow = SolexBllCalosc.PobierzInstancje.ProduktyKlienta.ProduktySpelniajaceKryteria(ListaKategorii, null, this.AktualnyKlient, this.AktualnyKlient.JezykId, slc,new Dictionary<int, HashSet<long>>(), null).ToDictionary(x=>x.Id,x=>string.IsNullOrEmpty(x.Rodzina)?x.Nazwa:x.Rodzina);

                Random r = new Random();
                HashSet<long> wymieszaneId =new HashSet<long>( listaProduktow.Keys.OrderBy(x => r.Next()) );

                foreach (var i in wymieszaneId)
                {
                    if (string.IsNullOrEmpty(listaProduktow[i]) || !wynik.ContainsKey(listaProduktow[i]))
                    {
                        wynik.Add(string.IsNullOrEmpty(listaProduktow[i])?"": listaProduktow[i], i);
                    }
                    if (wynik.Count >= LiczbaProduktow)
                    {
                        break;
                    }
                }
                return new HashSet<long>( wynik.Values );
            }
            set { ListaProduktow = value; }
        }

        

    }
}