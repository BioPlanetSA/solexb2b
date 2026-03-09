using System.Collections.Generic;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces.Sync;
using SolEx.Hurt.Model.Interfaces.SyncModuly;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Pliki
{
    public abstract class PlikiZSystemuKsiegowegoBaza:SyncModul, IModulPliki
    {
        [FriendlyName("Ścieżka do katalogu z plikami - np. c:\\pliki\\")]
        [WidoczneListaAdmin(false, false, true, false)]
        public string Sciezka { get; set; }

        [FriendlyName("Pojedyńczy znak który rozdziela nazwe pliku od symbolu produktu - np. jeśli mamy taka nazwe pliku: <b>zdjecie34#SKU123.jpg</b> gdzie kod produktu to SKU123 - naszym separatorem jest znak #")]
        [WidoczneListaAdmin(false, false, true, false)]
        public string Separator { get; set; }

        [FriendlyName("Pole z którego będą brane dane do nazwy pliku")]
        [WidoczneListaAdmin(false, false, true, false)]
        public TypyPolDoDopasowaniaZdjecia Pole { get; set; }

        public override string uwagi
        {
            get
            {
                return "Dozolone znaki w nazwie pliku: 0-9a-zA-Z-,$_. Wszystkie pozostałe będą zastąpione znakiem: -";
            }
        }
        public PlikiZSystemuKsiegowegoBaza()
        {
            Sciezka = "";
            Separator = "_";
            Pole = TypyPolDoDopasowaniaZdjecia.Kod;
        }

        public abstract void Przetworz(IDictionary<long, Produkt> produktyNaB2B, ref List<ProduktPlik> plikiLokalnePowiazania, ref List<Plik> plikiLokalne, ISyncProvider provider,
            ref List<Cecha> cechyB2B, ref List<KategoriaProduktu> kategorieB2B, ref List<Klient> klienciB2B);
    }
}
