using System.Collections.Generic;
using ServiceStack.DataAnnotations;
using SolEx.Hurt.Core.BLL.RegulyPunktowe;
using SolEx.Hurt.Core.BLL.ZadaniaKoszyka;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Core.ModelBLL
{
    [Alias("Zadanie")]
    [RepreznetowyTypModulow(typeof(SyncModul))]
    [FriendlyName("Moduł Synchronizacji", FriendlyOpis = "")]
    public class ModulSynchronizacji:ZadanieBll
    {
        public ModulSynchronizacji()
        {
        }

        public ModulSynchronizacji(ZadanieBll x):base(x)
        {
        }
    }
  
    [Alias("Zadanie")]
    [RepreznetowyTypModulow(typeof(ZadanieKoszyka))]
    [FriendlyName("Moduł Koszyka")]
    public class ModulKoszyka : ZadanieBll
    {
        public ModulKoszyka()
        {
        }

        public ModulKoszyka(ZadanieBll x)
            : base(x)
        {
        }
        [Ignore]
        [WidoczneListaAdmin(false,false,true,false)]
        [WymuszonyTypEdytora(TypEdytora.ListaWarunkow, typeof(ZadanieKoszyka))]
        public IList<ZadanieBll> Warunki
        {
            get { return BLL.SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<ZadanieBll>(null, x => x.ZadanieNadrzedne == Id); }
        }
    }
    [Alias("Zadanie")]
    [RepreznetowyTypModulow(typeof(RegulaPunktowa))]
    [FriendlyName("Moduł Punktowy", FriendlyOpis = "")]
    public class ModulPunktowy : ZadanieBll
    {
        public ModulPunktowy()
        {
        }

        public ModulPunktowy(ZadanieBll x)
            : base(x)
        {
        }
        [Ignore]
        [WidoczneListaAdmin(false, false, true, false)]
        [WymuszonyTypEdytora(TypEdytora.ListaWarunkow, typeof(RegulaPunktowa))]
        public IList<ZadanieBll> Warunki
        {
            get { return BLL.SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<ZadanieBll>(null, x => x.ZadanieNadrzedne == Id); }
        }
    }
}
