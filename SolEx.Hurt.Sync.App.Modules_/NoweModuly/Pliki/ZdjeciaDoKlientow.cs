using System.Collections.Generic;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Interfaces.Sync;
using SolEx.Hurt.Model.Interfaces.SyncModuly;
using SolEx.Hurt.Sync.App.Modules_.Helpers;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Pliki
{
    public class ZdjeciaDoKlientow : SyncModul, IModulPliki
    {
        public override string uwagi
        {
            get { return "Zdjęcia do klientów. Trzeba włączyć KtorePolaSynchronizowacKlienci"; }
        }
        [FriendlyName("Pole wg ktorego mapujemy klientów")]
        [SyncSlownikNaPodstawieInnegoTypu(typeof(Klient))]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string Pole { get; set; }
        
        [FriendlyName("Ścieżka do katalogu z plikami - np. c:\\pliki\\")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string Sciezka { get; set; }
        
        public void Przetworz(IDictionary<long, Produkt> produktyNaB2B, ref List<ProduktPlik> plikiLokalnePowiazania, ref List<Plik> plikiLokalne, ISyncProvider provider, ref List<Cecha> cechyB2B, ref List<KategoriaProduktu> kategorieB2B, ref List<Klient> klienciB2B)
        {
            PlikiHelper.PrzetworzPlikiDlaTypu(Sciezka, plikiLokalne, Pole, WypelnijKlienta, klienciB2B);
        }

        private bool WypelnijKlienta(object c, Plik p)
        {
            Klient klient = (Klient)c;
            klient.ZdjecieId = p.Id;
            return true;
        }
    }
}
