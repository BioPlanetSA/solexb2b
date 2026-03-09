using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core.ModelBLL.ObiektyMaili
{
    public class PowitanieSzef : SzablonMailaBaza
    {
        public PowitanieSzef(IKlient klient) : base(klient) { }
        public PowitanieSzef() : base(null)
        {
        }

        public override string NazwaFormatu()
        {
            return "[TESTY - nie używać] Powitanie nowego klienta przez szefa firmy";
        }

        public override string OpisFormatu()
        {

            return null;
            //return "Mail powitalny od szefa wysyłany do nowych klientów. Mail wysyłany z opóźnieniem kilku godzin. " +
            //       "Mail wysyła moduł 'wyślij powiadomienia od szefa dla nowych klientów' lub przez " +
            //       "API: Api/klienci.powitanie.ashx.";
        }
        public override string OpisDlaKlienta()
        {
            return "Mail powitalny od szefa wysyłany do nowych klientów.";
        }
        public override TypyPowiadomienia[] ObslugiwaneRodzajePowiadomien
        {
            get { return new[] {TypyPowiadomienia.Klient}; }
        }

    }
}
