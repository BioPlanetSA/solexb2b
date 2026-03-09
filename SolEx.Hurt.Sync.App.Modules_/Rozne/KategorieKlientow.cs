using System.Collections.Generic;
using System.Linq;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Helpers;

namespace SolEx.Hurt.Sync.App.Modules_.Rozne
{
    public abstract class KategorieKlientow : SyncModul
    {
        internal void DodajKategorie(List<KategoriaKlienta> kategorie, List<KlientKategoriaKlienta> laczniki, Klient k, string grupa, string nazwakategorii)
        {
            var kat = kategorie.FirstOrDefault(x => x.Nazwa == nazwakategorii && x.Grupa == grupa);
            if (kat == null)
            {
                kat = new KategoriaKlienta {Grupa = grupa, Nazwa = nazwakategorii};
                kat.Id = kat.WygenerujIDObiektu();
                kategorie.Add(kat);
            }
            if (!laczniki.Any(x => x.KlientId == k.Id && x.KategoriaKlientaId == kat.Id))
            {
                laczniki.Add(new KlientKategoriaKlienta { KlientId = k.Id, KategoriaKlientaId = kat.Id });
            }
        }
    }
}