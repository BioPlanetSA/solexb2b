using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.AtrybutyKlas;

namespace SolEx.Hurt.Core.BLL.ZadaniaKoszyka
{
    public class DostawaUbezpieczona : DostawaBaza
    {
        public override string Opis
        {
            get { return "Koszt dostawy z narzutem ubezpieczenia. Wyliczne cena netto + narzut ubezpieczenia"; }
        }

        [FriendlyName("Mimalna cena ubezpieczenia netto")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public decimal MinimalnaCena { get; set; }

        [FriendlyName("Jaki procent wartości netto koszyk stanowi ubezpieczenie, podawane w procentach")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public decimal ProcentCeny { get; set; }

        [FriendlyName("Cena dostawy netto")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public decimal CenaNetto { get; set; }

        public override decimal WyliczCene(IKoszykiBLL koszyk)
        {
            decimal cena = koszyk.CalkowitaWartoscHurtowaNettoPoRabacie() * ((ProcentCeny / 100M));
            if (cena < MinimalnaCena)
            {
                cena = MinimalnaCena;
            }
            return decimal.Round(cena + CenaNetto, 2);
        }
    }
}