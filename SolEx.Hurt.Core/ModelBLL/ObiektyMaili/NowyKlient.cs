using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core.ModelBLL.ObiektyMaili
{
    public class NowyKlient : SzablonMailaBaza
    {
        public NowyKlient(IKlient klient) : base(klient)
        {
        }
        public NowyKlient()
            : base(null)
        {
        }
        public override string NazwaFormatu()
        {
            return "Nowy klient";
        }

        public override string OpisFormatu()
        {
            return "Mail powitalny wysyłany do nowych klientów natychmiast po utworzeniu konta w platformie. W mailu zawarte są instrukcje logowania (login, hasło, adres)";
        }
        public override string OpisDlaKlienta()
        {
            return "Mail powitalny wysyłany do nowych klientów natychmiast po utworzeniu konta na platformie.";
        }
        public override TypyPowiadomienia[] PowiadomieniaDomyslnieAktywne
        {
            get { return new[] {TypyPowiadomienia.Klient}; }
        }

    
    }
}
