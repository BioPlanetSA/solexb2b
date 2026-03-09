using System;
using System.Collections.Generic;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Interfaces.SyncModuly;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.CechyIAtrybuty
{
    [FriendlyName("Zmiana nazwa cech", FriendlyOpis = "zmienia nazwę cechy na podstawie symbolu")]
    public class ZmianaNazwCech : SyncModul,IModulCechyIAtrybuty
    {
        public void Przetworz(ref List<Atrybut> atrybuty, ref List<Cecha> cechy, Dictionary<long, Produkt> produktyNaB2B)
        {
            if (string.IsNullOrEmpty(Zamienniki))
            {
                return;
            }

            string[] zastepniki = Zamienniki.Split(new[] { ";"}, StringSplitOptions.RemoveEmptyEntries);

            foreach (Cecha kat in cechy)
            {
                foreach (var zastepnik in zastepniki)
                {
                    string[] nowe = zastepnik.Split(new[] { Separator[0] }, StringSplitOptions.RemoveEmptyEntries);
                    if (kat.Symbol == nowe[0])
                        kat.Nazwa = nowe[1];
                }
            }
        }
        [FriendlyName("Zastępniki cech w formacie: np: symbol cechy:czymzastąpić nazwę (gdzie ':' to separator z ustawienia). Każdy nowy zastępnik musi być oddzielony ;")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string Zamienniki { get; set; }
        [FriendlyName("Separator odzielajacy cechę od frazy którą zastępujemy")]
        public string Separator { get; set; }
       
    }
}
