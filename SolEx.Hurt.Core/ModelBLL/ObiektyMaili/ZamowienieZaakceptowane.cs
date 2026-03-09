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
    public class ZamowienieZaakceptowane :  PowiadomienieZZamowieniem
    {
        public ZamowienieZaakceptowane() { }
        public ZamowienieZaakceptowane(ZamowieniaBLL zamowienia, IKlient klient)
            : base(zamowienia, klient)
        {

        }
   
        public override string NazwaFormatu()
        {
            return "Subkonta - zamówienie zaakceptowane";
        }

        public override string OpisFormatu()
        {
            return "Mail informujący o zaakceptowaniu zamówienia";
        }
        public override string OpisDlaKlienta()
        {
            return "Mail informujący o zaakceptowaniu zamówienia.";
        }
        public override TypyPowiadomienia[] PowiadomieniaDomyslnieAktywne
        {
            get { return new[] { TypyPowiadomienia.Klient }; }
        }
    }
}
