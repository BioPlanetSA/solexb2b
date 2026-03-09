using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.Enums;
using System.Linq;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core.BLL.RegulyKoszyka
{
    public class LacznaIloscPunktow : RegulaKoszyka, IRegulaCalegoKoszyka
    {
        public bool KoszykSpelniaRegule(IKoszykiBLL koszyk)
        {
            if (koszyk != null)
            {
                decimal wartPunktow = WyliczWartoscPunktow(koszyk);
                decimal iloscPunktow = WyliczIloscPunktowKlienta(koszyk.Klient);
                return iloscPunktow <= wartPunktow;
            }
            return false;
        }

        public decimal WyliczIloscPunktowKlienta(IKlient idKlienta)
        {
            return SolexBllCalosc.PobierzInstancje.PunktyDostep.PobierzPunktyKlientaLacznie(idKlienta);
        }

        public decimal WyliczWartoscPunktow(IKoszykiBLL koszyk)
        {
            return koszyk.PobierzPozycje.Where(x => x.TypPozycji == TypPozycjiKoszyka.ZaPunkty).Sum(x => x.Produkt.CenaWPunktach);
        }
    }
}