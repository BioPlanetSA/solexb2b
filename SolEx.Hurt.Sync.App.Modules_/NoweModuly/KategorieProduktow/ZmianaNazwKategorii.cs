using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Interfaces.Sync;
using SolEx.Hurt.Model;
using System;
using System.Collections.Generic;
using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.KategorieProduktow
{
    [FriendlyName("Zmiana nazwy kategorii",FriendlyOpis = "Zmienia nazwy kategorii na inne.")]
    class ZmianaNazwKategorii: SyncModul, Model.Interfaces.SyncModuly.IModulKategorieProduktow
    {

        public ZmianaNazwKategorii()
        {
            Kategorie = "";
        }

        public override string uwagi
        {
            get { return ""; }
        }

        [FriendlyName("Zastępniki kategorii w formacie: nazwa:czymzastąpić. Każdy nowy zastępnik musi być wpisany od nowej linijki.")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        [WymuszonyTypEdytora(TypEdytora.PoleTekstoweMultiLine)]
        public string Kategorie { get; set; }

        public void Przetworz(ref Dictionary<long, KategoriaProduktu> listaWejsciowa, Dictionary<long, KategoriaProduktu> listaKategoriiB2B, ISyncProvider provider, List<Grupa> grupyPRoduktow)
        {
            if (string.IsNullOrEmpty(Kategorie))
            {
                return;
            }

            string[] zastepniki = Kategorie.Split(new [] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);

            foreach (KeyValuePair<long, KategoriaProduktu> kat in listaWejsciowa)
            {
                foreach (var zastepnik in zastepniki)
                {
                    string[] nowe = zastepnik.Split(new [] {":"}, StringSplitOptions.RemoveEmptyEntries);
                    if (kat.Value.Nazwa == nowe[0])
                        kat.Value.Nazwa = nowe[1];
                }
            }
        }

    }

}
