using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core.ModelBLL.ObiektyMaili
{
    public class ResetHasla : SzablonMailaBaza
    {
        public ResetHasla(IKlient klient) : base(klient)
        {
            
        }
        public ResetHasla() : base(null)
        {
        }
        public override string NazwaFormatu()
        {
            return "Resetowanie hasła";
        }

        public override string OpisFormatu()
        {
            return "Mail informujący o zmianie hasła - w mailu jest link aktywacyjny";
        }
        public override string OpisDlaKlienta()
        {
            return "Mail informujący o zmianie hasła - w mailu jest link aktywacyjny";
        }
        public override TypyPowiadomienia[] ObslugiwaneRodzajePowiadomien
        {
            get
            {
                return new[] { TypyPowiadomienia.Klient };
            }
        }

        public override TypyPowiadomienia[] PowiadomieniaDomyslnieAktywne
        {
            get { return new[] { TypyPowiadomienia.Klient }; }
        }

        public string LinkZmianyHaslaKlienta(IKlient klient = null)
        {
            string link;
            if (klient != null && !string.IsNullOrEmpty(klient.Gid))
            {
                link = string.Format("{0}/zmiana-hasla?gid={1}", Konfiguracja.wlasciciel_AdresPlatformy, klient.Gid);
            }
            else
            {
                link = string.Format("{0}/zmiana-hasla", Konfiguracja.wlasciciel_AdresPlatformy);
            }
            return link;
        }
    }
}
