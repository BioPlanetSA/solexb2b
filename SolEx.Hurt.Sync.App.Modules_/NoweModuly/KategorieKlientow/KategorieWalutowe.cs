using System.Collections.Generic;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Interfaces.SyncModuly;
using SolEx.Hurt.Sync.App.Modules_.Rozne;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.KategorieKlientow
{
    [FriendlyName("Kategorie walutowe", FriendlyOpis = "Moduł tworzy kategorie klientów na podstawie waluty klienta")]
    public class KategorieWalutowe : KategorieWalutoweBase,IModulKategorieKlientow
    {
        public void Przetworz(ref List<KategoriaKlienta> kategorie, ref List<KlientKategoriaKlienta> laczniki)
        {
            IEnumerable<Klient> wszyscy = ApiWywolanie.PobierzKlientow().Values;
            var waluty = ApiWywolanie.PobierzWaluty();
            foreach (Klient k in wszyscy)
            {
                if (k.Id < 0) continue;
                Waluta waluta;
                waluty.TryGetValue(k.WalutaId, out waluta);
                DodajKategorie(kategorie, laczniki, k, grupa, waluta!=null?waluta.WalutaErp:DomyslnaWaluta);
            }
        }
    }
}