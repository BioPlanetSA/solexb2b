using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using System.Linq;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core.BLL
{
    public static class LimityIloscioweBLL
    {
        public static Licencje LicencjaLimity
        {
            get { return Licencje.limity_ilosciowe; }
        }

        public static bool CzyKlientMaLimity(int klient)
        {
            long? kid = klient;
            if (!SolexBllCalosc.PobierzInstancje.Konfiguracja.GetLicense(LicencjaLimity)) return false;
            do
            {
                IKlient k = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<Klient>(kid.GetValueOrDefault());
                if (SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<KlientLimitIlosciowy>(k).Any())//PobierzWszystkie().Values.Any(x => x.KlientId == kid))
                {
                    return true;
                }
                kid = k.KlientNadrzednyId;
            }
            while (kid != null);
            return false;
        }

        internal static decimal? WyliczLimit(long idKlienta, long produkt_id)
        {
            return null;
        }
    }
}