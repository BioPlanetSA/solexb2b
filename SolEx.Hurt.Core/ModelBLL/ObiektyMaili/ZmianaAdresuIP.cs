using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core.ModelBLL.ObiektyMaili
{
    public class ZmianaAdresuIP : SzablonMailaBaza
    {
        public ZmianaAdresuIP(IKlient klient, string noweIP, string stareIP) : base(klient)
        {
            NoweIp = noweIP;
            StareIp = stareIP;
        }

        public ZmianaAdresuIP() : base(null)
        {
            NoweIp = "0.0.0.0";
            StareIp = "0.0.0.0";
        }
        public override string NazwaFormatu()
        {
            return "Zmiana adresu IP";
        }

        protected ISolexBllCalosc Calosc = SolexBllCalosc.PobierzInstancje;

        public string StareIp { get; private set; }

        public string NoweIp { get; private set; }

        public override string OpisFormatu()
        {
            return "Mail informujący o zmianie adresu IP w stosunku do adresu wykorzystywanego poprzednio - " +
                   "w mialu jest link do potwierdzenia nowego adresu IP. Powiadomnie wysyłane przez funkcje sprawdzającą adresy IP logujących się klientów " +
                   "- funkcja aktywowana licencją";
        }
        public override string OpisDlaKlienta()
        {
            return "Mail informujący o zmianie adresu IP w stosunku do adresu wykorzystywanego poprzednio.";
        }
        public override TypyPowiadomienia[] PowiadomieniaDomyslnieAktywne
        {
            get { return new[] { TypyPowiadomienia.Klient }; }
        }
    }
}
