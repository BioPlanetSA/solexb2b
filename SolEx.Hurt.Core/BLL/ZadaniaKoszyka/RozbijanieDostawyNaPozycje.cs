using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using System.Linq;

namespace SolEx.Hurt.Core.BLL.ZadaniaKoszyka
{
    public class RozbijanieDostawyNaPozycje : ZadanieCalegoKoszyka, IFinalizacjaKoszyka
    {
        public override string Opis
        {
            get { return "Usuwa koszt dostawy z koszyka, a jego wartość rozdziela na poszczególne pozycje"; }
        }

        public override bool Wykonaj(IKoszykiBLL koszyk)
        {
            var pozycje = koszyk.PobierzPozycje;
            if (koszyk.KosztDostawyId != null)
            {
                decimal cenaDostawy = koszyk.KosztDostawy().WyliczCene(koszyk);
                decimal sumaBezDostawy = 0;
                decimal pozostalo = cenaDostawy;
                foreach (var item in pozycje)
                {
                    var cenaPozycji = item.CenaNetto * item.IloscWJednostcePodstawowej;
                    sumaBezDostawy += cenaPozycji;
                }

                foreach (IKoszykPozycja kp in pozycje)
                {
                    decimal procentUdzialu = decimal.Round((kp.CenaNetto * kp.IloscWJednostcePodstawowej) / sumaBezDostawy, 4);
                    decimal kosztDostawyLiczonyzProcent = cenaDostawy * procentUdzialu;
                    decimal kosztDostawyJednaSztuka = kosztDostawyLiczonyzProcent / kp.IloscWJednostcePodstawowej;
                    pozostalo -= kosztDostawyLiczonyzProcent;
                    if (kp.RabatDodatkowy > 0)
                    {
                        kp.WymuszonaCenaNettoModul = kp.CenaNetto + kosztDostawyJednaSztuka;
                        decimal? rabat = kp.WymuszonaCenaNettoModul * (kp.RabatDodatkowy / 100);
                        kp.WymuszonaCenaNettoModul += rabat;
                    }
                    else
                    {
                        kp.WymuszonaCenaNettoModul = kp.CenaNetto + kosztDostawyJednaSztuka;
                    }
                }
                if (pozostalo != 0)
                {
                    IKoszykPozycja ostatniaPozycja = pozycje.Last();
                    ostatniaPozycja.WymuszonaCenaNettoModul += pozostalo / ostatniaPozycja.IloscWJednostcePodstawowej;
                }

                koszyk.NieDodawajDostawyDoKoszyka = true;
            }

            return true;
        }
    }
}