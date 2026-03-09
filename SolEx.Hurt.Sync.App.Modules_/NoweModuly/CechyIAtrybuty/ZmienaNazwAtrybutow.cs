using System;
using System.Collections.Generic;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Interfaces.SyncModuly;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.CechyIAtrybuty
{
    [FriendlyName("Zmień nazwe atrybutow", FriendlyOpis = "zmienia nazwę atrybutów")]
    public class ZmienaNazwAtrybutow : SyncModul, IModulCechyIAtrybuty
    {
        public void Przetworz(ref List<Atrybut> atrybuty, ref List<Cecha> cechy, Dictionary<long, Produkt> produktyNaB2B)
        {
            if (string.IsNullOrEmpty(Zamienniki))
            {
                return;
            }

            string[] zastepniki = Zamienniki.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (Atrybut kat in atrybuty)
            {
                foreach (var zastepnik in zastepniki)
                {
                    string[] nowe = zastepnik.Split(new[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                    if (kat.Nazwa == nowe[0])
                        kat.Nazwa = nowe[1];
                }
            }
        }
        [FriendlyName("Zastępniki atrybutow w formacie: nazwa atrybutu:czymzastąpić nazwę. Każdy nowy zastępnik musi być oddzielony ;")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string Zamienniki { get; set; }
      
    }
}
