using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;

namespace SolEx.Hurt.Core.BLL.RegulyPunktowe
{
    public class PunktyZaWartoscPozycji : RegulaPunktowaPozycjiDokumentu
    {
        public override string Opis
        {
            get { return "Dodaje określoną liczbę punktów z każdą złotówkę na pozycji dokumentu"; }
        }

        [FriendlyName("Czy liczymy wg netto czy brutto. Nie - wartość liczona wg cen nettto, Tak - wg cen brutto")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public bool CzyBrutto { get; set; }

        [FriendlyName("Zmniejsz ilość punktów o wartość rabatu")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public bool ZmiejszORabat { get; set; }

        [FriendlyName("Za każdą złotówkę na pozycycji daj x punktow")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public decimal IlePunktow { get; set; }

        public override decimal WyliczPunkty(DokumentyPozycje pozycjaWyliczna, DokumentyBll dokumentNaKtorymJestPozycja, decimal punktyPoprzednieReguly)
        {
            decimal wartosc = CzyBrutto ? pozycjaWyliczna.PozycjaDokumentuWartoscBrutto : pozycjaWyliczna.PozycjaDokumentuWartoscNetto;
            decimal wynik = wartosc * IlePunktow;

            if (ZmiejszORabat)
            {
                wynik *= ((100M - pozycjaWyliczna.Rabat) / 100M);
            }
            return wynik;
        }
    }
}