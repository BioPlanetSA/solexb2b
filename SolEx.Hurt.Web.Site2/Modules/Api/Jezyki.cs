using SolEx.Hurt.Web.Site2.Helper;

namespace SolEx.Hurt.Web.Site2.Modules.Api
{
    /// <summary>
    /// Pobiera wszystkie dostępne w systemie języki jako Słownik<klucz int ID, wartość jezyki>
    /// </summary>
    public class PobierzJezykiHandler : ApiSessionBaseHandler, SolEx.Hurt.Model.Core.IDocumentApiVisible
    {
        protected override object Handle()
        {
            return Core.BLL.Languages.JezykiWSystemie();
        }
        public override bool WymagajAktywnejSesji
        {
            get { return false; }
        }
    }
}
