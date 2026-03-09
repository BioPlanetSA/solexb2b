using System.Web.Routing;
using ServiceStack.DesignPatterns.Model;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core.ModelBLL
{
    public interface IKontrolkaTresciBaza: IPolaIDentyfikujaceRecznieDodanyObiekt, IHasIntId
    {
        IKlient AktualnyKlient { get; set; }
        string Nazwa { get; }
        string Grupa { get; }
        string Opis { get; }
        string Ikona { get; }
        string Kontroler { get; }
        string Akcja { get; }
        string AcordionNazwa { get; set; }
        bool AcordionZwiniety { get; set; }

        RouteValueDictionary Parametry();
    }

    //public interface IKontrolkaTresciBaza_Renderowana
    //{
    //    string Render(RequestContext requestContext);
    //}
}