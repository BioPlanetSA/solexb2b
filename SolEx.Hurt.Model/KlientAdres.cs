using ServiceStack.DesignPatterns.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Helpers;

namespace SolEx.Hurt.Model
{
    public class KlientAdres : IHasLongId
    {
        public KlientAdres()
        {
            TypAdresu = TypAdresu.Brak;
        }
        public long Id
        {
            get
            {
                return  (KlientId + "_" + AdresId+"_"+ TypAdresu).WygenerujIDObiektuSHAWersjaLong();
            }
        }

        [IdentyfikatorObiektuNadrzednego("SolEx.Hurt.Core.Klient,SolEx.Hurt.Core")]
        [PobieranieSlownika("SolEx.Hurt.Core.Helper.SlownikKlientow,SolEx.Hurt.Core")]
        [Niewymagane]
        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Klient")]
        public long KlientId { get; set; }
        public long AdresId { get; set; }
        public TypAdresu TypAdresu { get; set; }
    }
}