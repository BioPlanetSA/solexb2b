using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model.AtrybutyKlas;

namespace SolEx.Hurt.Core.BLL.RegulyPunktowe
{
    public class PunktyZaWartoscDokumentu : RegulaPunktowaCalegoDokumentu
    {
        public override string Opis
        {
            get { return "Dodaje określoną liczbę punktów z każdą złotówkę na dokumencie"; }
        }

        [FriendlyName("Czy liczymy wg netto czy brutto. Nie - wartość liczona wg cen nettto, Tak - wg cen brutto")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public bool CzyBrutto { get; set; }

        [FriendlyName("Za każdą złotówkę na dokumencie daj x punktow")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public decimal IlePunktow { get; set; }

        public override int WyliczPunkty(DokumentyBll dokument, decimal punktyPoprzednieReguly)
        {
            decimal wartosc = CzyBrutto ? dokument.DokumentWartoscBrutto : dokument.DokumentWartoscNetto;
            return (int)(wartosc * IlePunktow);
        }
    }
}