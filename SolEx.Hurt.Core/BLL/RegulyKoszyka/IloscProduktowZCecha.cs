using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.AtrybutyKlas;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SolEx.Hurt.Core.BLL.RegulyKoszyka
{
    public class IloscProduktowZCecha : IloscLacznaBaza
    {
        public IloscProduktowZCecha()
        {
            Ilosc = IleProduktow.NieWszystkie;
        }

        public override string Opis
        {
            get { return "Czy określona liczba produktów ma określone cechy"; }
        }

        [Obsolete("Pole wycofane - korzystać z pola Cecha")]
        [FriendlyName("Symbol Cechy")]
        [Niewymagane]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string Symbol { get; set; }

        [FriendlyName("Cecha")]
        [PobieranieSlownika(typeof(SlownikCech))]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public long[] Cecha { get; set; }

        public override List<string> TestPoprawnosci()
        {
            return null;
        }

        public override decimal Wylicz(IKoszykiBLL koszyk, out int ilosc)
        {
            decimal liczba = 0;
            ilosc = 0;

            foreach (KoszykPozycje poz in koszyk.PobierzPozycje)
            {
                if (this.Ilosc == IleProduktow.NieWszystkie)
                {
                    if (poz.Produkt.PosiadaCechy(this.Cecha, false))
                    {
                        ilosc++;
                        liczba += poz.IloscWJednostcePodstawowej;
                    }
                }
                else
                {
                    if (!poz.Produkt.PosiadaCechy(this.Cecha, true))
                    {
                        continue;
                    }
                    ilosc++;
                    liczba += poz.IloscWJednostcePodstawowej;
                }
            }

            return liczba;
        }
    }
}