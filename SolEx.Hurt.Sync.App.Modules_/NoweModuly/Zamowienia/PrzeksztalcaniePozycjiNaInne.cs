using System;
using System.Collections.Generic;
using System.Reflection;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Interfaces.Sync;
using SolEx.Hurt.Model.Interfaces.SyncModuly;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Zamowienia
{
    public class PrzeksztalcaniePozycjiNaInne : SyncModul, IModulZamowienia
    {
        public override string uwagi
        {
            get { return "Przekształca pozycję na zamówieniu na inną, zachowuje pozostałe parametry"; }
        }
         
        [FriendlyName("Pole, w którym szukamy id produktu na który mamy podmienic")]
        [SyncSlownikNaPodstawieInnegoTypu(typeof(Produkt))]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string PoleZrodlowe { get; set; }
        public void Przetworz(ZamowienieSynchronizacja listaWejsciowa, ref List<ZamowienieSynchronizacja> wszystkie, ISyncProvider provider, Dictionary<long, Jednostka> jednostki, Dictionary<long, ProduktJednostka> laczniki, Dictionary<long, Produkt> produktyB2B, List<Cecha> cechyB2B, List<ProduktCecha> lacznikiCech)
        {
            PropertyInfo propertisy = typeof(Produkt).GetProperty(PoleZrodlowe);
            var produkty = ApiWywolanie.PobierzProdukty();
            foreach (var p in listaWejsciowa.pozycje)
            {
                Produkt pr = produkty[p.ProduktId];
                object o = propertisy.GetValue(pr);
                if (o != null)
                {
                p.ProduktId=  Convert.ToInt32(o);
                    p.Opis2 = pr.Nazwa;
                }
            }
        }
    }
}
