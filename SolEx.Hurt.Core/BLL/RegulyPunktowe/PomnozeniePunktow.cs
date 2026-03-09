using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model.AtrybutyKlas;

namespace SolEx.Hurt.Core.BLL.RegulyPunktowe
{
    public class PomnozeniePunktow : RegulaPunktowaCalegoDokumentu
    {
        public override string Opis
        {
            get
            {
                return "Mnoży punkty z poprzednich reguł razy określoną wartość. " +
                       "Jesli poprzednie reguły naliczyły 100 pkt, w module tym ustawimy przelicznik 0.5 to za ten dokument naliczne zostanie łacznie 150 pkt. " +
                       "Jesli poprzednie reguły naliczyły 100 pkt, w module tym ustawimy przelicznik 2 to za ten dokument naliczne zostanie łacznie 300 pkt.";
            }
        }

        [FriendlyName("Przelicznik")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public decimal Przelicznik { get; set; }

        public override int WyliczPunkty(DokumentyBll dokument, decimal punktyPoprzednieReguly)
        {
            return (int)(punktyPoprzednieReguly * Przelicznik);
        }
    }
}