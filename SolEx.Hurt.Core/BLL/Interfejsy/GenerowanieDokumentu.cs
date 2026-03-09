using System.Collections.Generic;
using System.Linq;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Helpers;
using System.Security;
using System.Text;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core.BLL.Interfejsy
{
    public abstract class GenerowanieDokumentu
    {
        public abstract bool MoznaGenerowac(DokumentyBll dokument);

        public abstract Licencje? WymaganaLicencja { get; }
        public abstract Encoding Kodowanie { get; }
        public abstract string Nazwa { get; }

        public abstract string PobierzNazwePliku(DokumentyBll dokument);

        protected string NazwaPliku(DokumentyBll dokument)
        {
            return TextHelper.PobierzInstancje.OczyscNazwePliku(dokument.NazwaDokumentu);
        }

        protected Owner seller
        {
            get
            {
                return SolexBllCalosc.PobierzInstancje.Konfiguracja.GetOwner();
            }
        }

        protected abstract byte[] PobierzDane(DokumentyBll dokument, IKlient klient);

        public byte[] PobierzDokumentDlaKlienta(DokumentyBll dokument, IKlient klient)
        {
            if (SolexBllCalosc.PobierzInstancje.DokumentyDostep.CzyKlientMaDostep(dokument, klient))
            {
                SolexBllCalosc.PobierzInstancje.Statystyki.ZdarzeniePobranieFaktury(dokument, klient, this.Nazwa);
                return PobierzDane(dokument, klient);
            }
            throw new SecurityException("Nie masz dostępu do tego dokumentu");
        }

        private Dictionary<long, int> _zaokraglenia = null;

        public Dictionary<long, int> Zaokraglenia
        {
            get { return _zaokraglenia ?? (_zaokraglenia = SolexBllCalosc.PobierzInstancje.JednostkaProduktuBll.PobierzJednostki(SolexBllCalosc.PobierzInstancje.Konfiguracja.JezykIDDomyslny).ToDictionary(x => x.Key, x => x.Value.Zaokraglenie)); }
        }


    }
}