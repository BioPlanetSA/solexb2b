using System;
using System.Collections.Generic;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Interfaces.Sync;
using SolEx.Hurt.Model.Interfaces.SyncModuly;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.WyliczenieGotowychCen
{
    [FriendlyName("Pobieranie wyliczonych cen z ERP-a", FriendlyOpis = "Zwraca wyliczone ceny z providera księgowego")]
    public class PobieranieWyliczonychCenZProvideraKsiegowego : SyncModul, IModulWyliczanieGotowychCen
    {
        public void Przetworz(ref List<FlatCeny> wynik, Dictionary<long, Klient> dlaKogoLiczyc,  ISyncProvider aktualnyProvider)
        {
            IWyliczanieGotowychCen prov = aktualnyProvider as IWyliczanieGotowychCen;
            if (prov == null)
            {
                throw new Exception("Aktualny provider nie dziedziczy po interfejsie IWyliczanieGotowychCen");
            }
            foreach (var klienci in dlaKogoLiczyc)
            {
                var wyliczne = prov.WyliczCenyKlienta(klienci.Key);
                wynik.AddRange(wyliczne);
                Log.DebugFormat("dla klienta {0} wyliczono {1} cen",klienci.Key, wyliczne.Count);
            }

            Log.DebugFormat("koniec modułu 'Pobieranie wyliczonych cen z ERP-a' wyliczonych cen {0} ",  wynik.Count);
        }
    }
}
