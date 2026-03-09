using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Interfaces.SyncModuly;
using log4net;
using SolEx.Hurt.Model;
using System.Collections.Generic;
using System.Linq;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.CechyIAtrybuty
{

    [FriendlyName("Cechy na kategorie",FriendlyOpis = "Dla cech, które mają strukturę drzewa dodaje rodzica z nazwy atrybutu")]
    internal class CechyNaKategorie : SyncModul, IModulCechyIAtrybuty
    {

        public CechyNaKategorie()
        {
            Atrybut = "";
        }
     
        [FriendlyName("Nazwa atrybutu, który będzie doklejony do nazwy cechy")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string Atrybut { get; set; }

        public void Przetworz(ref List<Atrybut> atrybuty, ref List<Cecha> cechy, Dictionary<long, Produkt> produktyNaB2B)
        {
            if (string.IsNullOrEmpty(Atrybut))
            {
                Log.Debug("Pole Atrybut jest puste, moduł przerwie działanie");
                return;
            }

            Atrybut atr = atrybuty.FirstOrDefault(a => a.Nazwa == Atrybut);
            if (atr != null)
            {
                List<Cecha> listacech = cechy.Where(a => a.AtrybutId == atr.Id).ToList();
                foreach (Cecha cecha in listacech)
                {
                    cecha.Nazwa = string.Format(@"{0}\{1}", Atrybut, cecha.Nazwa);
                }

                Cecha cechaAtrybutu = cechy.FirstOrDefault(a => a.Nazwa == Atrybut);
                if (cechaAtrybutu == null)
                {
                    Cecha nowaCecha = new Cecha();
                    nowaCecha.Nazwa = Atrybut;
                    nowaCecha.Symbol = string.Format("{0}:{0}", Atrybut).ToLower();
                    nowaCecha.Widoczna = true;
                    nowaCecha.AtrybutId = atr.Id;
                    nowaCecha.Id = nowaCecha.Symbol.WygenerujIDObiektu();
                    cechy.Add(nowaCecha);
                }
            }
        }

    }
}
