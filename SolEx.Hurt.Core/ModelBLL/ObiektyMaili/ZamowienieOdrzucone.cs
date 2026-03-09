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
    public class ZamowienieOdrzucone : PowiadomienieZKoszykiem
    {
        public ZamowienieOdrzucone() { }
        public ZamowienieOdrzucone(IKoszykiBLL koszyk, IKlient klient, IKlient odrzucil)
            : base(koszyk, klient)
        {

        }
        public override string NazwaFormatu()
        {
            return "Subkonta - zamówienie odrzucone";
        }
      
        public override string OpisFormatu()
        {
            return "Mail informujący o odrzuceniu zamówienia";
        }
        public override string OpisDlaKlienta()
        {
            return "Mail informujący o odrzuceniu zamówienia.";
        }
        public override TypyPowiadomienia[] PowiadomieniaDomyslnieAktywne
        {
            get { return null; }
        }
        
    }
}
