using System;
using ServiceStack.Common;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Interfaces.Sync;
using SolEx.Hurt.Model.Interfaces.SyncModuly;
using SolEx.Hurt.Model;
using System.Collections.Generic;
using System.Linq;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.CechyIAtrybuty
{

    public abstract class CechyModulBaza : SyncModul, IModulCechyIAtrybuty, IModulProdukty
    {
        public void DodajBrakujaceLaczniki(long idCechy, long idProduktu, Dictionary<long, ProduktCecha> slownikLacznikow)//, List<ProduktCecha> laczniki)
        {
            var cp = new ProduktCecha
            {
                ProduktId = idProduktu,
                CechaId = idCechy
            };

            if (!slownikLacznikow.ContainsKey(cp.Id))
            {
                slownikLacznikow.Add(cp.Id, cp);
            }
        }

        public Atrybut ZnajdzAtrybut(string nazwaAtrybutu, List<Atrybut> listaatrybutow)
        {
            return listaatrybutow.FirstOrDefault(a => a.Nazwa.Equals(nazwaAtrybutu, StringComparison.OrdinalIgnoreCase));
        }

        public Cecha ZnajdzCeche(string symbolCechy, List<Cecha> listaCech)
        {
            Cecha wynik;
            wynik = listaCech.FirstOrDefault(a => a.Symbol.Equals(symbolCechy, StringComparison.OrdinalIgnoreCase));
            return wynik;
        }

        public Atrybut DodajBrakujacyAtrybut(string nazwaAtrybutu, List<Atrybut> listaatrybutow)
        {
            //Log.InfoFormat("Dodaje Atrybut o nazwie: {0}", nazwaAtrybutu);
            Atrybut nowyAtrybut = new Atrybut(nazwaAtrybutu) {Widoczny = true};
            nowyAtrybut.Id = nowyAtrybut.WygenerujIDObiektuSHA(1);
            listaatrybutow.Add(nowyAtrybut);
            return nowyAtrybut;
        }

        public Cecha DodajBrakujacaCeche(string symbolCechy, string nazwaCechy, int atrybutId, List<Cecha> listaCech)
        {
            symbolCechy = symbolCechy.ToLower();
            Cecha nowaCecha = new Cecha(nazwaCechy, symbolCechy)
            {
                Widoczna = true,
                AtrybutId = atrybutId
            };
            nowaCecha.Id = nowaCecha.WygenerujIDObiektuSHA(1);
            listaCech.Add(nowaCecha);
            
            return nowaCecha;
        }

        public Cecha DodajCecheNadrzedna(Cecha nowaCecha, HashSet<long> idCechyNadrz)
        {
            if(nowaCecha==null) return null;
            nowaCecha.CechyNadrzedne =new HashSet<long>( nowaCecha.CechyNadrzedne.Concat(idCechyNadrz) );
            return nowaCecha;
        }

        public abstract void Przetworz(ref List<Atrybut> atrybuty, ref List<Cecha> cechy, Dictionary<long, Produkt> produktyNaB2B);

        public abstract void Przetworz(ref List<Produkt> listaWejsciowa, ref List<Tlumaczenie> produktyTlumaczenia, Dictionary<long, Produkt> produktyNaB2B, 
            ref List<JednostkaProduktu> jednostki, ref Dictionary<long, ProduktCecha> lacznikiCech, ref List<ProduktKategoria> lacznikiKategorii, 
            ISyncProvider provider, ref List<ProduktUkryty> produktuUkryteErp, ref List<ProduktyZamienniki> zamienniki, Dictionary<long, KategoriaProduktu> kategorie, 
            ref List<Cecha> cechy, ref List<Atrybut> atrybuty);
    }
}
