using ServiceStack.DataAnnotations;
using ServiceStack.DesignPatterns.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using System.Collections.Generic;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Model
{
    [FriendlyClassName("Magazyn")]
    [TworzDynamicznieTabele]
    public class Magazyn : IEqualityComparer<Magazyn>, IHasIntId,  IPolaIDentyfikujaceRecznieDodanyObiekt
    {
        public Magazyn()
        {
            ImportowacZErp = true;
            //Parametry = string.Empty;
            MagazynRealizujacy = false;
        }
        [PrimaryKey]
        [UpdateColumnKey]
        [WidoczneListaAdmin(true, true, false, false)]
        public int Id { get; set; }

        [FriendlyName("Symbol magazynu", FriendlyOpis = "W przypadku gdy chemy żeby stany były łączone z kilku magazynów należy symbole półćzyć + np.: Mag1+Mag2")]
        [Index(Unique = true)]
        [WidoczneListaAdmin(true, true, true, false)]
        public string Symbol { get; set; }

        [FriendlyName("Importować z ERP")]
        [WidoczneListaAdmin(true, true, true, false)]
        public bool ImportowacZErp { get; set; }
        
        //nigdzie nie wykorzystywane
        //[WidoczneListaAdmin(true, true, true, false)]
        //[Niewymagane]
        //public string Parametry { get; set; }
        
        [FriendlyName("Nazwa magazynu")]
        [WidoczneListaAdmin(true, true, true, true)]
        public string Nazwa { get; set; }

        [FriendlyName("Magazyn realizujący dla ściagania stanu towarów po złożeniu zamówienia")]
        [WidoczneListaAdmin(true, true, true, false)]
        public bool MagazynRealizujacy { get; set; }

        public bool Equals(Magazyn x, Magazyn y)
        {
            //return x.Id == y.Id && x.Symbol == y.Symbol && x.ImportowacZErp == y.ImportowacZErp && x.Parametry == y.Parametry && x.Nazwa == y.Nazwa;
            return x.Id == y.Id && x.Symbol == y.Symbol && x.ImportowacZErp == y.ImportowacZErp  && x.Nazwa == y.Nazwa;
        }

        public int GetHashCode(Magazyn obj)
        {
            return obj.GetHashCode();
        }

        public override string ToString()
        {
            return Symbol;
        }

        public bool RecznieDodany()
        {
            return true;
        }
    }
}
