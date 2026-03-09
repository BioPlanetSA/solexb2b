using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core.ModelBLL.ObiektyMaili
{
    public class GenerowanieKluczaApi : SzablonMailaBaza
    {
        public GenerowanieKluczaApi(IKlient klient) : base(klient) { }
        public GenerowanieKluczaApi()
            : base(null)
        {
        }
        public override string NazwaFormatu()
        {
            return "Generowanie klucza api";
        }

        public override string OpisFormatu()
        {
            return "Mail z nowym kluczem api wysyłany do nowych klientów. ";
        }
        public override string OpisDlaKlienta()
        {
            return "Mail z nowym kluczem api wysyłany do nowych klientów.";
        }
        public override TypyPowiadomienia[] ObslugiwaneRodzajePowiadomien
        {
            get { return new[] { TypyPowiadomienia.Klient }; }
        }
        public override TypyPowiadomienia[] PowiadomieniaDomyslnieAktywne
        {
            get { return new[] { TypyPowiadomienia.Klient }; }
        }
    }
}
