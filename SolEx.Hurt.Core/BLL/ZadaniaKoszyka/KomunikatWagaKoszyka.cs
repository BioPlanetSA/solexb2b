using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Interfaces;
using SolEx.Hurt.Model.Web;
using System.Collections.Generic;

namespace SolEx.Hurt.Core.BLL.ZadaniaKoszyka
{
    public class KomunikatWagaKoszyka : ZadanieCalegoKoszyka, IModulStartowy, IFinalizacjaKoszyka, ITestowalna
    {
        public KomunikatWagaKoszyka()
        {
            IloscMiejscPoPrzecinku = 1;
        }

        [FriendlyName("Komunikat do pokazania, za {0} będzie wpisana waga koszyka")]
        [Lokalizowane]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public new string Komunikat { get; set; }

        [FriendlyName("Do ilu miejsc zaokrąglać wagę - Jeśli pole niewypełnione to brak zaokrąglenia")]
        [Niewymagane]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public int? IloscMiejscPoPrzecinku { get; set; }

        public List<string> TestPoprawnosci()
        {
            List<string> listaBledow = Przedzial.Sprawdzenie(IloscMiejscPoPrzecinku, null, "Ilość miejsc po przecinku");
            return listaBledow;
        }

        public override bool Wykonaj(IKoszykiBLL koszyk)
        {
            if (koszyk.WagaCalokowita() > 0)
            {
                decimal wynik = 0;
                wynik = IloscMiejscPoPrzecinku.HasValue ? decimal.Round(koszyk.WagaCalokowita().Wartosc, IloscMiejscPoPrzecinku.Value) : koszyk.WagaCalokowita().Wartosc;
                WyslijWiadomosc(string.Format(Komunikat, wynik), KomunikatRodzaj.info);
            }
            return true;
        }
    }
}