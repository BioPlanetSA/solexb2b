using ServiceStack.DataAnnotations;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;

namespace SolEx.Hurt.Core
{
    internal interface IPozycjaDokumentuBll: IDokumentuPozycjaBazowa
    {
        /// <summary>
        /// ustawiane przy wyciaganiu automatycznie
        /// </summary>
        [Ignore]
        ProduktBazowy ProduktBazowy { get; set; }
    }
}