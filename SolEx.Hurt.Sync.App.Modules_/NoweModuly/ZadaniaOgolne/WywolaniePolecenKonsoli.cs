using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ServiceStack.Common;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.Sync;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Interfaces.SyncModuly;


namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.ZadaniaOgolne
{
    public class WywolaniePolecenKonsoli : SyncModul, IZadaniaOglone
    {
        public WywolaniePolecenKonsoli()
        {
            TimeOut = 300;
        }
        [FriendlyName("Polecenia do wywołania")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string Polecenia { get; set; }

        [FriendlyName("Czas na pojedyńcze polecenie w ms")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public int TimeOut { get; set; }

        public void Przetworz(Model.Interfaces.Sync.ISyncProvider provider)
        {
            foreach (var p in Polecenia.Split(new []{Environment.NewLine},StringSplitOptions.RemoveEmptyEntries))
            {
              var proces=  System.Diagnostics.Process.Start("CMD.exe","/c "+ p);
                if (proces != null)
                {
                Thread.Sleep(TimeOut);
                    try
                    {
                        if (!proces.HasExited)
                        {
                            proces.Kill();
                        }
                    }
                    catch (Exception ex)
                    {
                       LogiFormatki.PobierzInstancje.LogujError(ex);
                    }
                }
            }
        }
        
        
        
        public override string uwagi
        {
            get { return "Polecenia wykonywane są linia po linii"; }
        }
    }
}
