using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Model.AtrybutyKlas;
using System;
using System.Collections.Generic;
using System.Linq;
using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Core.BLL.RegulyKoszyka
{
    public class RegionKlienta : RegulaKoszyka, IRegulaCalegoKoszyka
    {
        public RegionKlienta()
        {
            Relacja = RelacjaJestNieJest.Jest;
            Regiony = new List<string>();
        }

        [FriendlyName("Czy klient jest/ nie jest z wybranych regionów")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public RelacjaJestNieJest Relacja { get; set; }

        [FriendlyName("Wybierz region")]
        [PobieranieSlownika(typeof(SlownikRegionow))]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public List<string> Regiony { get; set; }

        public bool KoszykSpelniaRegule(ModelBLL.Interfejsy.IKoszykiBLL koszyk)
        {
            bool wynik = false;
            if (!Regiony.Any()) throw new InvalidOperationException("Brak wpisanych państw w regule");
            if (koszyk.Adres == null)
            {
                return false;
            }
            if (Regiony.Any(x => int.Parse(x) == koszyk.Adres.RegionId))
            {
                wynik = true;
            }
            return Relacja == RelacjaJestNieJest.NieJest ? !wynik : wynik;
        }
    }
}