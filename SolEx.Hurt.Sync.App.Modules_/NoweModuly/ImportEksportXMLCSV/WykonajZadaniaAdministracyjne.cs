using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model.AtrybutyKlas;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.ImportEksportXMLCSV
{
      [ModulStandardowy]
   public class WykonajZadaniaAdministracyjne:SyncModul, Model.Interfaces.SyncModuly.IModulEksportImportXMLCSV
    {
        public override string Opis
        {
            get {return "Wywołuje wykonanie zadań administracyjnych"; }
        }

        public void Przetworz()
        {
            ApiWywolanie.WykonajZadaniaAdministracyjne();
        }

       public override string uwagi
       {
           get { return ""; }
       }
    }
}
