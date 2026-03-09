using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Core.BLL.RegulyKoszyka;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Web;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SolEx.Hurt.Core.BLL.ZadaniaKoszyka
{
    [FriendlyName("Komunikat o darmowej dostawie", FriendlyOpis = "Moduł do darmowej dostawy pozostało. Uwzględnia tylko koszta dostawy, mające warunek LacznaWartoscKoszyka i cenę netto  równą 0")]
    public class KomunikatDarmowaDostawa : ZadanieCalegoKoszyka, IFinalizacjaKoszyka, IModulStartowy
    {
        public IZadaniaBLL ZadaniaBll = SolexBllCalosc.PobierzInstancje.ZadaniaBLL;

        [FriendlyName("Komunikat do pokazania, za {0} będzie wpisana brakująca wartość netto")]
        [Niewymagane]
        [Lokalizowane]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public new string Komunikat { get; set; }

        [FriendlyName("Id nie uwzglednianych sposobów dostawy, oddzielone ;")]
        [Niewymagane]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string IdDoPominiecia { get; set; }

        private List<int> DoPominiecia
        {
            get
            {
                List<int> wynik = new List<int>();
                if (!string.IsNullOrWhiteSpace(IdDoPominiecia))
                {
                    foreach (var i in IdDoPominiecia.Split(';'))
                    {
                        int id;
                        if (int.TryParse(i, out id))
                        {
                            wynik.Add(id);
                        }
                    }
                }
                return wynik;
            }
        }

        public override bool Wykonaj(IKoszykiBLL koszyk)
        {
            decimal kwota = decimal.MaxValue;
            foreach (var regula in ModulySpelniajaceWarunek(koszyk))
            {
                decimal tmp = regula.WartoscMinimalna.Value - regula.WyliczWartosc(koszyk);
                if (tmp > 0 && tmp < kwota)
                {
                    kwota = tmp;
                }
            }
            if (kwota != decimal.MaxValue && kwota > 0)
            {
                WyslijWiadomosc(string.Format(Komunikat, $"{kwota.ToString("0.00")} {koszyk.WalutaKoszyka().WalutaB2b}"), KomunikatRodzaj.info);
            }
            return true;
        }

        public List<LacznaWartoscKoszyka> ModulySpelniajaceWarunek(IKoszykiBLL koszyk)
        {
            List<LacznaWartoscKoszyka> wynik = new List<LacznaWartoscKoszyka>();

            var kd = SolexBllCalosc.PobierzInstancje.Koszyk.WszystkjeSposobyDostawy(koszyk);

            List<ZadanieCalegoKoszyka> koszta = new List<ZadanieCalegoKoszyka>();
            foreach (var x in kd)
            {
                if (DoPominiecia.Contains(x.Id))
                {
                    continue;
                }
                var tmp = x;
                if (tmp.WyliczCene(koszyk) != 0)
                {
                    continue;
                }
                koszta.Add((ZadanieCalegoKoszyka)x);
            }
            foreach (var zadanieKoszyka in koszta)
            {
                ZadanieBll regula = zadanieKoszyka.Warunki().FirstOrDefault(p => p.Modul().GetType() == typeof(LacznaWartoscKoszyka));
                if (regula != null)
                {
                    if (zadanieKoszyka.CzySpelniaKryteria(koszyk, new List<Type> { typeof(LacznaWartoscKoszyka) }) && ((LacznaWartoscKoszyka)regula.Modul()).WartoscMinimalna.HasValue && !zadanieKoszyka.CzySpelniaKryteria(koszyk)  )
                    {
                        wynik.Add((LacznaWartoscKoszyka)regula.Modul());
                    }
                }
            }
            return wynik;
        }
    }
}