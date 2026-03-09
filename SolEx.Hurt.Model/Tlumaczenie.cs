using ServiceStack.DesignPatterns.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Model
{
    public class Tlumaczenie : IModelSearcharable, IHasLongId,IStringIntern
    {  
        [StringInternuj]
        public string Typ { get; set; }
        public int JezykId { get; set; }
        public long ObiektId { get; set; }

        [StringInternuj]
        [WidoczneListaAdmin(false,false,true,true)]
        public string Wpis { get; set; }

        [StringInternuj]
        public string Pole { get; set; }
        
        public long Id
        {
            get { return string.Format("{0}{1}{2}{3}", Typ, JezykId, ObiektId, Pole).WygenerujIDObiektuSHAWersjaLong(); }
        }
    }
}
