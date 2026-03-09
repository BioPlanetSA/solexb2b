using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.AtrybutyKlas;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SolEx.Hurt.Core.BLL.RegulyKoszyka
{
    [FriendlyName("Darmowa wysyłak dla regionu", FriendlyOpis = "Warunek dla jakiego regionu będzie darmowa wysyłka")]
    internal class DarmowaWysylkaRegion : RegulaKoszyka, IRegulaCalegoKoszyka, IRegulaPozycji
    {
        public DarmowaWysylkaRegion()
        {
            Regiony = new List<string>();
        }

        [FriendlyName("Wybierz państwa")]
        [PobieranieSlownika(typeof(SlownikRegionow))]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public List<string> Regiony { get; set; }

        public bool PozycjaSpelniaRegule(IKoszykPozycja pozycja, IKoszykiBLL koszyk)
        {
            return Regula(koszyk);
        }

        public bool KoszykSpelniaRegule(IKoszykiBLL koszyk)
        {
            return Regula(koszyk);
        }

        private bool Regula(IKoszykiBLL koszyk)
        {
            bool wynik = false;
            if (!Regiony.Any()) throw new InvalidOperationException("Brak wpisanych regionów w regule");
            if (koszyk.Adres == null)
            {
                return false;
            }
            if (Regiony.Any(x => int.Parse(x) == koszyk.Adres.RegionId))
            {
                wynik = true;
            }
            return wynik;
        }
    }
}