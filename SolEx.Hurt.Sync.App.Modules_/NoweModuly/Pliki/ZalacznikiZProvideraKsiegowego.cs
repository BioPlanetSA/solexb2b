using System.Collections.Generic;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Interfaces.Sync;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Pliki
{
    [FriendlyName("Zalaczniki z providera ksiegowego", FriendlyOpis = "Zapisuje Zalaczniki z programu księgowego do wybranego katalogu. Obsługiwane systemy księgowe: Hermes, Optima, Subiekt, WFMAG, XL")]
    public class ZalacznikiZProvideraKsiegowego : PlikiZSystemuKsiegowegoBaza
    {
        public override void Przetworz(IDictionary<long, Model.Produkt> produktyNaB2B, ref List<ProduktPlik> plikiLokalnePowiazania, ref List<Plik> plikiLokalne, ISyncProvider provider, ref List<Cecha> cechyB2B, ref List<KategoriaProduktu> kategorieB2B, ref List<Klient> klienciB2B)
        {
            if (provider is IEksportZalacznikowNaDysk)
                (provider as IEksportZalacznikowNaDysk).ZapiszZalacznikiNaDysk(Sciezka, Separator, Pole);
        }
    }
}
