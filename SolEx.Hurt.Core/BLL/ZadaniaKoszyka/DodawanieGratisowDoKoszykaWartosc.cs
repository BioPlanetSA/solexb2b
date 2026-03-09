using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Core;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;
using SolEx.Hurt.Model.Web;
using System.Collections.Generic;
using System.Linq;

namespace SolEx.Hurt.Core.BLL.ZadaniaKoszyka
{
    public class DodawanieGratisowDoKoszyka : DodawanieGratisowDoKoszykaBaza, ITestowalna
    {
        public IConfigBLL Konfiguracja = SolexBllCalosc.PobierzInstancje.Konfiguracja;

        public override string Opis
        {
            get { return "Dodaje gratisów moduł głowny. Wymaga modułów pozwalających na wybrór produktów."; }
        }

        [FriendlyName("Jaki procent wartości netto koszyk jest przeznaczony na gratisy")]
        [Niewymagane]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public decimal? ProcentKwoty { get; set; }

        [Niewymagane]
        [FriendlyName("Jaka wartość jest przeznaczona na gratisy")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public decimal? WartoscNetto { get; set; }

        [FriendlyName("Cecha produktów które mają być sumowane")]
        [Niewymagane]
        [PobieranieSlownika(typeof(SlownikCech))]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public List<string> Cecha { get; set; }

        private List<long> ListaCech
        {
            get
            {
                if (Cecha != null && Cecha.Any() && Cecha[0] != "")
                {
                    return Cecha.Select(long.Parse).ToList();
                }
                return null;
            }
        }

        protected override decimal Wartosc(IKoszykiBLL koszyk)
        {
            List<KoszykPozycje> pozycje = koszyk.PobierzPozycje;
            if (WartoscNetto.HasValue)
            {
                return WartoscNetto.Value;
            }
            decimal wartoscpoz = pozycje.Where(x => x.TypPozycji == TypPozycjiKoszyka.Zwykly).Sum(x => x.WartoscNetto.Wartosc);
            if (ListaCech != null)
            {
                wartoscpoz = pozycje.Where(x => x.TypPozycji == TypPozycjiKoszyka.Zwykly && x.Produkt.Cechy.Any(y => ListaCech.Contains(y.Key))).Sum(x => x.WartoscNetto.Wartosc);
            }
            return wartoscpoz * (ProcentKwoty.GetValueOrDefault() / 100M);
        }

        protected override decimal WartoscPozycji(IKoszykiBLL koszyk)
        {
            return koszyk.PobierzPozycje.Where(x => x.TypPozycji == TypPozycjiKoszyka.Gratis).Sum(x => x.Produkt.FlatCeny.CenaHurtowaNetto * x.IloscWJednostcePodstawowej);
        }

        public List<string> TestPoprawnosci()
        {
            List<string> bledy = new List<string>();
            if (ProcentKwoty.HasValue && WartoscNetto.HasValue)
            {
                bledy.Add("Nalezy wybrać albo wartość netto albo procent wartości koszyka");
            }
            if (!ProcentKwoty.HasValue && !WartoscNetto.HasValue)
            {
                bledy.Add("Nalezy wypełnić przynajmniej jedno z pół albo wartość netto albo procent wartości koszyka");
            }
            return bledy;
        }

        protected override Komunikat KomunikatGenreracja(IKoszykiBLL koszyk)
        {
            decimal laczna = WartoscPozycji(koszyk);
            decimal calkowita = Wartosc(koszyk);
            decimal pozostala = calkowita - laczna;

            string k = pozostala > 0 ? string.Format("pozostało: {0}", new WartoscLiczbowa(pozostala, koszyk.WalutaKoszyka().WalutaB2b)) : string.Format("przekroczyłeś o: {0}", new WartoscLiczbowa((pozostala * -1), koszyk.WalutaKoszyka().WalutaB2b));

            string komunikat = Konfiguracja.PobierzTlumaczenie(koszyk.Klient.JezykId,"Wartość produktów gratisowych do wykorzystania: {1}, wykorzystano: {0}, {2}", new WartoscLiczbowa(laczna, koszyk.WalutaKoszyka().WalutaB2b), new WartoscLiczbowa(calkowita, koszyk.WalutaKoszyka().WalutaB2b), k);
            return new Komunikat(komunikat, SpelniaRegule(koszyk) ? KomunikatRodzaj.success : KomunikatRodzaj.danger, "");
        }
    }
}