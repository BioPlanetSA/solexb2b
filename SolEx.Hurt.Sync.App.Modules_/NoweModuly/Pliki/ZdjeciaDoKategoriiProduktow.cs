using System.Collections.Generic;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces.Sync;
using SolEx.Hurt.Model.Interfaces.SyncModuly;
using SolEx.Hurt.Sync.App.Modules_.Helpers;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Pliki
{

    public class ZdjeciaDoKategoriiProduktow : SyncModul, IModulPliki
    {
        public ZdjeciaDoKategoriiProduktow()
        {
            KomuPrzypisacZdjecie=KomuPrzypisywac.PierwzemuZnalezionemu;
        }
        public override string uwagi
        {
            get { return "Zdjęcia do kategorii. Trzeba włączyć KtorePolaSynchronizowacKategorie"; }
        }
        
        [FriendlyName("Pole wg ktorego mapujemy kategorie produktów")]
        [SyncSlownikNaPodstawieInnegoTypu(typeof(KategoriaProduktu))]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string Pole { get; set; }
        
        [FriendlyName("Ścieżka do katalogu z plikami - np. c:\\pliki\\")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string Sciezka { get; set; }
        
        [FriendlyName("Komu przypisywac pasujące zdjecie")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public KomuPrzypisywac KomuPrzypisacZdjecie{ get; set; }
        
        public void Przetworz(IDictionary<long, Produkt> produktyNaB2B, ref List<ProduktPlik> plikiLokalnePowiazania, ref List<Plik> plikiLokalne, ISyncProvider provider, ref List<Cecha> cechyB2B, ref List<KategoriaProduktu> kategorieB2B, ref List<Klient> klienciB2B)
        {
            PlikiHelper.PrzetworzPlikiDlaTypu(Sciezka, plikiLokalne, Pole, Wypelnij, kategorieB2B, true, KomuPrzypisacZdjecie);
        }

        private bool Wypelnij(object c,Plik p)
        {
            KategoriaProduktu kategoria = (KategoriaProduktu)c;
            kategoria.ObrazekId = p.Id;
            return true;
        }
    }
}
