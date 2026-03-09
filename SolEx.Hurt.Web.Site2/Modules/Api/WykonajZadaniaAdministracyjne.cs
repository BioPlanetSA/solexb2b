using SolEx.Hurt.Web.Site2.Helper;

namespace SolEx.Hurt.Web.Site2.Modules.Api
{
    /// <summary>
    /// Wywołanie powoduje uruchomienie wszystkich zadań administracyjnych
    /// </summary>
    public class WykonajZadaniaAdministracyjne : ApiSessionBaseHandler
    {
        protected override object Handle()
        {
          //  ZadanieAdmina.WykonajWszystkieZadaniaAdminia();
            return true;
        }
    }
}
