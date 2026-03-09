using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Web;
using System.Collections.Generic;
using System.Linq;

namespace SolEx.Hurt.Core.BLL.ZadaniaKoszyka
{
    public abstract class DodawanieGratisowDoKoszykaBaza : ZadanieCalegoKoszyka, IModulStartowy, IModulKoszykGratisy, IFinalizacjaKoszyka
    {
        protected abstract Komunikat KomunikatGenreracja(IKoszykiBLL koszyki);

        [FriendlyName("Komunikat gdy koszyk ma przekroczone warunki")]
        [Niewymagane]
        [Lokalizowane]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string KomunikatPrzekroczone { get; set; }

        public override bool Wykonaj(IKoszykiBLL koszyk)
        {
            bool kom = SpelniaRegule(koszyk);
            decimal wartoscmaksproduktow = Wartosc(koszyk);

            if (!kom && !string.IsNullOrEmpty(KomunikatPrzekroczone))
            {
                WyslijWiadomosc(KomunikatPrzekroczone, KomunikatRodzaj.danger);
            }
            if (kom && !string.IsNullOrEmpty(Komunikat) && wartoscmaksproduktow != 0)
            {
                WyslijWiadomosc(Komunikat, KomunikatRodzaj.success);
            }

            return kom;
        }

        protected abstract decimal Wartosc(IKoszykiBLL koszyk);

        protected abstract decimal WartoscPozycji(IKoszykiBLL koszyk);

        public Komunikat KomunikatWarunku(IKoszykiBLL koszyk)
        {
            decimal calkowita = Wartosc(koszyk);
            if (calkowita == 0)
            {
                return null;
            }
            return KomunikatGenreracja(koszyk);
        }

        public bool SpelniaRegule(IKoszykiBLL koszyk)
        {
            decimal wartoscmaksproduktow = Wartosc(koszyk);
            bool kom = WartoscPozycji(koszyk) <= wartoscmaksproduktow;
            return kom;
        }
    }

    public class DodawanieGratisowDoKoszykaIlosc : DodawanieGratisowDoKoszykaBaza
    {
        public IConfigBLL Konfiguracja = SolexBllCalosc.PobierzInstancje.Konfiguracja;

        public override string Opis
        {
            get { return "Dodaje gratisów moduł głowny. Wymaga modułów pozwalających na wybrór produktów. ZA kazde X produktów z cechą Y można wybrać Z gratisów"; }
        }

        [FriendlyNameAttribute("X produktów")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public decimal X { get; set; }

        [FriendlyNameAttribute("Y nazwa cechy")]
        [PobieranieSlownika(typeof(SlownikCech))]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public List<string> Y { get; set; }

        [FriendlyNameAttribute("Z gratisów")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public decimal Z { get; set; }

        public List<int> IdY
        {
            get { return Y.Select(int.Parse).ToList(); }
        }

        protected override decimal Wartosc(IKoszykiBLL koszyk)
        {
            decimal liczba = IdY.Sum(i => koszyk.PobierzPozycje.Where(x => x.TypPozycji == TypPozycjiKoszyka.Zwykly && x.Produkt.Cechy.Any(y => y.Key == i)).Sum(x => x.IloscWJednostcePodstawowej));
            return X != 0 ? decimal.Floor((liczba / X) * Z) : 0;
        }

        protected override decimal WartoscPozycji(IKoszykiBLL koszyk)
        {
            return koszyk.PobierzPozycje.Where(x => x.TypPozycji == TypPozycjiKoszyka.Gratis).Sum(x => x.IloscWJednostcePodstawowej);
        }

        protected override Komunikat KomunikatGenreracja(IKoszykiBLL koszyk)
        {
            int laczna = (int)WartoscPozycji(koszyk);
            int calkowita = (int)Wartosc(koszyk);
            int pozostala = calkowita - laczna;

            string k = string.Empty;
            if (pozostala > 0)
            {
                k = string.Format("pozostało: {0}", pozostala);
            }
            else
            {
                if (pozostala != 0)
                {
                    k = string.Format(", zmniejsz ilość o: {0}", pozostala * -1);
                }
            }

            string komunikat = Konfiguracja.PobierzTlumaczenie(koszyk.Klient.JezykId,"Ilość produktów gratisowych do wykorzystania: {1}, wykorzystano: {0} {2}", laczna, calkowita, k);
            return new Komunikat(komunikat, SpelniaRegule(koszyk) ? KomunikatRodzaj.success : KomunikatRodzaj.danger, "");
        }

        public bool SpelniaRegule2(IKoszykiBLL koszyk)
        {
            decimal wartoscmaksproduktow = Wartosc(koszyk);
            bool kom = wartoscmaksproduktow >= X;
            return kom;
        }
    }
}