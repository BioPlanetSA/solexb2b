using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Interfaces.SyncModuly;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.ZadaniaOgolne
{
      [ModulStandardowy]
    public class WyslijPonownieBledneMaile : SyncModul, IZadaniaOglone
    {
        public override string uwagi
        {
            get { return "Wysyłanie ponownie błędnych mail"; }
        }

        public void Przetworz(Model.Interfaces.Sync.ISyncProvider provider)
        {
            ApiWywolanie.WyslijPonownieBledneMaile();
        }
    }
}