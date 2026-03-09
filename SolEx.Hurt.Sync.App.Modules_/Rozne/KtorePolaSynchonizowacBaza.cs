using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Helpers;

namespace SolEx.Hurt.Sync.App.Modules_.Rozne
{
    public abstract class KtorePolaSynchonizowacBaza : SyncModul
    {
        public override string Opis
        {
            get { return "Określa które pola z ERP mają być nadpisane wartością pól z B2B. Zaznaczone pola będą zawsze nadpisywane wartością z B2B."; }
        }
        public override string uwagi
        {
            get { return ""; }
        }

        public void UstawPola(object docelowy, object wzor, PropertyInfo[] wszystkiepropertisy,List<string> pola)
        {
            if (docelowy == null || wzor == null)
            {
                return;
            }

            var akcesorCelowy = docelowy.GetType().PobierzRefleksja();
            var akcesorWzor = wzor.GetType().PobierzRefleksja();

            foreach (var p in wszystkiepropertisy)
            {
                string nazwa = p.Name;
                if (pola.Contains(nazwa) || pola.Contains(p.Name))
                {
                    akcesorCelowy[docelowy, p.Name] = akcesorWzor[wzor, p.Name];
                }
            }

        }
    }
}
