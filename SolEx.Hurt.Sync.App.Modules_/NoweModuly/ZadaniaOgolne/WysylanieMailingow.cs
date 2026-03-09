using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Interfaces.SyncModuly;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.ZadaniaOgolne
{
      [ModulStandardowy]
    public class WysylanieMailingow : SyncModul, IZadaniaOglone
    {
        public override string uwagi
        {
            get { return "Wysyłanie newsletterów"; }
        }

        public void Przetworz(Model.Interfaces.Sync.ISyncProvider provider)
        {
            ApiWywolanie.WyslijNewslettery();
        }
    }
}