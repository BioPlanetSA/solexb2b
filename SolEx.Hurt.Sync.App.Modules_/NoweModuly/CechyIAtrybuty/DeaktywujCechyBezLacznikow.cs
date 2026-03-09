using System;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model.Interfaces.SyncModuly;
using SolEx.Hurt.Model;
using System.Collections.Generic;
using System.Linq;
using ServiceStack.Common;
using SolEx.Hurt.Core.BLL.ZadaniaKoszyka;
using SolEx.Hurt.Core.Sync;
using SolEx.Hurt.Model.AtrybutyKlas;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.CechyIAtrybuty
{
    [ModulStandardowy]
    [FriendlyName("Deaktywuj cechy bez łaczników", FriendlyOpis = "Deaktywuje cechy które nie są przypisane do produktów.")]
    public class DeaktywujCechyBezLacznikow : SyncModul, IModulCechyIAtrybuty, IModulStartowy
    {
        private string _deaktywacjaAutomatycznaOpis = "-DEAKTYWOWANA-AUTOMATYCZNIE";
        public void Przetworz(ref List<Atrybut> atrybuty, ref List<Cecha> cechy, Dictionary<long, Produkt> produktyNaB2B)
        {
            Dictionary<long, Cecha> cechyNaPlatformie = ApiWywolanie.PobierzCechy();
            HashSet<string> grupyNaB2B = new HashSet<string>( ApiWywolanie.PobierzGrupy().Where(x => !string.IsNullOrEmpty(x.Parametry)).SelectMany(x => x.ParametryTablica()) );
            HashSet<long> idCechZLacznikow = new HashSet<long>( ApiWywolanie.PobierzCechyProdukty().Select(x => x.Value.CechaId) );
          
            SprawdzCechy(cechyNaPlatformie, grupyNaB2B,idCechZLacznikow,ref cechy);
        }
        private string nazwaAtrybutuRabatu = "Rabat";
        public void SprawdzCechy(Dictionary<long, Cecha> cechyNaPlatformie, HashSet<string> grupyNaB2B, HashSet<long> idCechZLacznikow, ref List<Cecha> cechy)
        {
            //sprawdzanie czy cecha jest przypisana do jakiegokolwiek produktu i jelsi nie jest to cecha będzie niewidoczna 
            List<Cecha> cechKtoreSaJuzNaPlatformieIZostalyDeaktywowaneAutoamtycznie = cechy.Where(x => x.Id > 0 && cechyNaPlatformie.ContainsKey(x.Id)).Select(x => x).ToList();
            foreach (Cecha cecha in cechKtoreSaJuzNaPlatformieIZostalyDeaktywowaneAutoamtycznie)
            {
                if (cecha.Nazwa.Equals(nazwaAtrybutuRabatu, StringComparison.InvariantCultureIgnoreCase))
                {
                    continue;
                }
                //jest w laczniakch i byla poprzednia deaktywowana
                if (grupyNaB2B.Any(x => cecha.Symbol.StartsWith(x, StringComparison.InvariantCultureIgnoreCase)) || (idCechZLacznikow.Contains(cecha.Id) && cecha.Nazwa.EndsWith(_deaktywacjaAutomatycznaOpis)))
                {
                    cecha.Widoczna = true;
                    if (cecha.Nazwa.EndsWith(_deaktywacjaAutomatycznaOpis))
                    {
                        cecha.Nazwa = cecha.Nazwa.TrimEnd(_deaktywacjaAutomatycznaOpis);
                    }
                    continue;
                }
                if (idCechZLacznikow.Contains(cecha.Id)) continue;
                cecha.Widoczna = false;
                if (!cecha.Nazwa.EndsWith(_deaktywacjaAutomatycznaOpis))
                {
                    cecha.Nazwa = cecha.Nazwa + _deaktywacjaAutomatycznaOpis;
                }
            }
        }
    }
}
