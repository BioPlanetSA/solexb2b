using System.Collections.Generic;
using System.Reflection;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Interfaces.SyncModuly;
using SolEx.Hurt.Sync.App.Modules_.Rozne;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.PoziomyCenowe
{
        [ModulStandardowy]
    [SynchronizowanePola(typeof(PoziomCenowy))]
 
    public class KtorePolaSynchronizowacPoziomyCen : KtorePolaSynchonizowacBaza, IModulPoziomyCen, IModulPola
    {
        public void Przetworz(ref Dictionary<int, PoziomCenowy> listaPoziomowCen, ref List<CenaPoziomu> ceny, Dictionary<int, PoziomCenowy> poziomyNaB2B, Dictionary<long, CenaPoziomu> cenyPoziomyB2B)
        {
            if (Pola==null || Pola.Count == 0)
            {
                return;
            }
            PropertyInfo[] propertisy = typeof(PoziomCenowy).GetProperties();

            foreach (PoziomCenowy wzorzec in poziomyNaB2B.Values)
            {
                PoziomCenowy docelowy = listaPoziomowCen.ContainsKey(wzorzec.Id) ? listaPoziomowCen[wzorzec.Id] : null;
                UstawPola(docelowy, wzorzec, propertisy, Pola);
            }
        }

        [Niewymagane]
        [FriendlyName("Wybierz pola które będziesz edytował w adminie B2B. Pola nie wybrane będa nadpisywane wartościami z ERP (są oznaczone na zielono w adminie)")]
        [SyncSlownikNaPodstawieInnegoTypu(typeof(PoziomCenowy))]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public List<string> Pola { get; set; }
        
        public List<string> PobierzDostepnePola()
        {
            return Pola;
        }
    }
}
