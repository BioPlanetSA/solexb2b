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
    public class ZamowienieDoAkceptacji : PowiadomienieZKoszykiem
    {
        public ZamowienieDoAkceptacji() { }
        public ZamowienieDoAkceptacji(IKoszykiBLL koszyk, IKlient klient)
            : base(koszyk, klient)
        {
          
        }
  
        public override string NazwaFormatu()
        {
            return "Subkonta - zamówienie do akceptacji dla konta akceptujacego";
        }

        public override string OpisFormatu()
        {
            return "Mail informujący o zamówieniu do akceptacji dla konta akceptujacego - w mailu są linki do akceptacji lub odrzucenia zamówienia. Klient oznacza w tym wypadku konta akceptujące zamówienia";
        }
        public override string OpisDlaKlienta()
        {
            return "Mail informujący o zamówieniu do akceptacji dla konta akceptujacego.";
        }
        public override TypyPowiadomienia[] ObslugiwaneRodzajePowiadomien
        {
            get
            {
                return new[] { TypyPowiadomienia.Klient  };
            }
        }
        public override TypyPowiadomienia[] PowiadomieniaDomyslnieAktywne => new[] { TypyPowiadomienia.Klient };

    }
}
