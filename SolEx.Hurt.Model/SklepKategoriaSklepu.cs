using ServiceStack.DesignPatterns.Model;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Model
{
    public class SklepKategoriaSklepu : IHasLongId, IPolaIDentyfikujaceRecznieDodanyObiekt
    {
       
        public long SklepId { get; set; }
        public long KategoriaSklepuId { get; set; }
        
        public long Id { get { return (SklepId + "||" + KategoriaSklepuId).WygenerujIDObiektuSHAWersjaLong(); }}

        public bool RecznieDodany()
        {
            if (SklepId < 0 )
            {
                return true;
            }
            return false;
        }
    }
}
