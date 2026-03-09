using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model.Interfaces.Sync;
using log4net;
using SolEx.Hurt.Model;
using System.Collections.Generic;
using SolEx.Hurt.Model.AtrybutyKlas;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.KategorieProduktow
{
    [FriendlyName("Dodaj brakujące kategorie w Subiekcie",FriendlyOpis = "Dodaje brakujące kategorie w drzewie w osbługiwanym systemie ERP. Obsługuje tylko SubiektaGT")]
    class DodajBrakujaceKategorie: SyncModul, Model.Interfaces.SyncModuly.IModulKategorieProduktow
    {
        public void Przetworz(ref Dictionary<long, KategoriaProduktu> listaWejsciowa, Dictionary<long, KategoriaProduktu> listaKategoriiB2B, ISyncProvider provider, List<Grupa> grupyPRoduktow)
        {
            if (provider is ITworzenieKategorii)
            {
                (provider as ITworzenieKategorii).PrzetworzKategorie(grupyPRoduktow);
            }
            else Log.Error("Aktualny provider dla systemu ERP nie obsługuje tworzenia kategorii.");
        }
    }

}
