using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ServiceStack.Common;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.Sync;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Interfaces.Sync;
using SolEx.Hurt.Model.Interfaces.SyncModuly;


namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Produkty
{
    public class PrzypisywanieMenagerowProduktow : SyncModul, IModulProdukty
    {
        public override string uwagi
        {
            get { return "Przypisanie menagerów produktów na podstawie cechy"; }
        }

        private List<Klient> _pracownicy;
        public virtual List<Klient> PracownicyNaPlatformie
        {
            get
            {
                if (_pracownicy == null)
                {
                    _pracownicy = ApiWywolanie.PobierzKlientow().Values.ToList();
                }
                return _pracownicy;
            }
        }
        private List<Cecha> _cechy;
        public virtual List<Cecha> CechyNaPlatformie
        {
            get { return _cechy ?? (_cechy = ApiWywolanie.PobierzCechy().Values.ToList()); }
        }
        [FriendlyName("Początek symbolu cech używanych do mapowania menagerów produktów")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string PoczatekCechy { get; set; }

        [FriendlyName("Pole, klienta w którym szukać nazwy cechy")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        [SyncSlownikNaPodstawieInnegoTypu(typeof(Klient))]
        public string Pole { get; set; }

        public void Przetworz(ref List<Produkt> listaWejsciowa, ref List<Tlumaczenie> produktyTlumaczenia, Dictionary<long, Produkt> produktyNaB2B, ref List<JednostkaProduktu> jednostki, ref Dictionary<long, ProduktCecha> lacznikiCech, ref List<ProduktKategoria> lacznikiKategorii, ISyncProvider provider, ref List<ProduktUkryty> produktuUkryteErp, ref List<ProduktyZamienniki> zamienniki, Dictionary<long, KategoriaProduktu> kategorie, ref List<Cecha> cechy, ref List<Atrybut> atrybuty)
        {
            Dictionary<long, HashSet<long>> cechyproduktywgproduktow = lacznikiCech.Values.GroupBy(x => x.ProduktId).ToDictionary(x => x.Key, x => new HashSet<long>( x.Select(y=>y.CechaId) ));
            Type kt = typeof (Klient);
            PropertyInfo pik = kt.GetProperties().First(x => x.Name == Pole);
            Dictionary<long, Cecha> pasujacecechy = CechyNaPlatformie.Where(x => x.Symbol.ToLower().StartsWith(PoczatekCechy.ToLower())).ToDictionary(x=>x.Id,x=>x);
            if (pasujacecechy.Count==0)
            {
                throw new Exception("Nie znaleziono cech których symbol zaczyna się od "+PoczatekCechy);
            }
            foreach (Produkt p in listaWejsciowa)
            {
                if (!cechyproduktywgproduktow.ContainsKey(p.Id))
                {
                    continue;//brak łączników pomijamy
                }
                List<Cecha> c = pasujacecechy.WhereKeyIsIn(cechyproduktywgproduktow[p.Id]);
                bool ustawiono = false;
                foreach (Cecha cecha in c)
                {
                    string szukane = cecha.Nazwa.ToLower();
                    foreach (Klient pr in PracownicyNaPlatformie)
                    {
                        object wart = pik.GetValue(pr);
                        if (wart == null)
                        {
                            continue; 
                        }
                        string wstr = wart.ToString().ToLower();
                        if (szukane == wstr)
                        {
                            p.MenagerId = pr.Id;
                            ustawiono = true;
                            break;
                        }
                    }
                    if (ustawiono)
                    {
                        break;
                    }
                    LogiFormatki.PobierzInstancje.LogujInfo(string.Format("Produkt {3} ma cechę o symbolu {0}, ale nie znaleziono pracownika który w polu {1} miałby wpisane {2}",cecha.Symbol,pik.Name,cecha.Nazwa,p.Kod));
                }
            }
        }
    }
}
