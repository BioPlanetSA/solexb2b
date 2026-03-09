using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model.AtrybutyKlas;

namespace SolEx.Hurt.Web.Site2.Models.KontrolkiTresci
{
    public class ResetHasla : KontrolkaTresciBaza
    {
        public override string Grupa
        {
            get { return "Klienci"; }
        }
        [Lokalizowane]
        public override string Nazwa
        {
            get { return "Reset hasła"; }
        }

        public override string Kontroler
        {
            get { return "MojeDane"; }
        }

        public override string Akcja
        {
            get { return "ResetHasla"; }
        }

        public override string Opis
        {
            get { return "Kontrolka do resetowania hasła"; }
        }
    }
}