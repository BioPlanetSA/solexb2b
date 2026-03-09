using System.Collections.Generic;
using System.Linq;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Interfaces.Sync;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Zamowienia
{
    [FriendlyName("Zamowienia w jednoskach Podstawowych", FriendlyOpis = "Przekształca zamówienie tak, aby wszystkie pozycje zapisane były w jednostce podstawowej")]
    public class ZamowieniaWJednoskachPodstawowych : SyncModul, Model.Interfaces.SyncModuly.IModulZamowienia
    {
        public void Przetworz(ZamowienieSynchronizacja listaWejsciowa, ref List<ZamowienieSynchronizacja> wszystkie, ISyncProvider provider, Dictionary<long, Jednostka> jednostki, Dictionary<long, ProduktJednostka> laczniki, Dictionary<long, Produkt> produktyB2B, List<Cecha> cechy, List<ProduktCecha> cechyProduktyNaPlatfromie)
        {

            foreach (ZamowienieProdukt pozycja in listaWejsciowa.pozycje)
            {
                 if (pozycja.JednostkaPrzelicznik != 1 && pozycja.JednostkaPrzelicznik!=0)
                 {
                     ProduktJednostka lacznik= laczniki.Values.First(x => x.ProduktId == pozycja.ProduktId && x.Podstawowa);
                     Jednostka pdst = jednostki[lacznik.JednostkaId];
                     pozycja.UstawJednostke( pdst.Nazwa);//czyścimy jednostkę
                     pozycja.JednostkaMiary = pdst.Id;
                     pozycja.CenaNetto = pozycja.CenaNetto/pozycja.JednostkaPrzelicznik;
                     pozycja.CenaBrutto = pozycja.CenaBrutto / pozycja.JednostkaPrzelicznik;
                     pozycja.Ilosc = pozycja.Ilosc*pozycja.JednostkaPrzelicznik;
                     pozycja.JednostkaPrzelicznik = 1;
                 }
            }
        }

        public override string uwagi => "";
    }
}
