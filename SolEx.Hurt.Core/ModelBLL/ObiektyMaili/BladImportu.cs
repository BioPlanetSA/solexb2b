using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core.ModelBLL.ObiektyMaili
{
    public class BladImportu : PowiadomienieZZamowieniem
    {
        public BladImportu() { }
        public BladImportu(ZamowieniaBLL zamowienia, IKlient klient) : base(zamowienia, klient)
        {
          
        }

        public override string NazwaFormatu()
        {
            return "Błąd importu zamówienia";
        }
        public override string OpisFormatu()
        {
            return "Mail informujący o błędzie importu zamówienia do systemu ERP";
        }
        public override string OpisDlaKlienta()
        {
            return "Mail informujący o błędzie importu zamówienia.";
        }
        public override TypyPowiadomienia[] ObslugiwaneRodzajePowiadomien
        {
            get
            {
                return new[] { TypyPowiadomienia.Opiekun, TypyPowiadomienia.Przedstawiciel, TypyPowiadomienia.DrugiOpiekun };
            }
        }
        public override TypyPowiadomienia[] PowiadomieniaDomyslnieAktywne
        {
            get { return new[] { TypyPowiadomienia.Opiekun }; }
        }
    }
}
