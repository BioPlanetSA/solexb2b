using SolEx.Hurt.Model;
using System.Collections.Generic;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Interfaces.Sync;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Pliki
{
    [FriendlyName("Zdjecia z povidera ksiegowego",FriendlyOpis = "Zapisuje zdjęcia z programu księgowego do wybranego katalogu. Obsługiwane systemy księgowe: Hermes, Optima, Subiekt, WFMAG, XL")]
    public class ZdjeciaZProvideraKsiegowego : PlikiZSystemuKsiegowegoBaza
    {
        public override void Przetworz(IDictionary<long, Produkt> produktyNaB2B, ref List<ProduktPlik> plikiLokalnePowiazania, ref List<Plik> plikiLokalne, ISyncProvider provider, ref List<Cecha> cechyB2B, ref List<KategoriaProduktu> kategorieB2B, ref List<Klient> klienciB2B)
        {
           if (provider is IEksportZdjecNadysk)
                (provider as IEksportZdjecNadysk).ZapiszZdjeciaNaDysk(Sciezka, Separator, Pole);
        }

    }
}
