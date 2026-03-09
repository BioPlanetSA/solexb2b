using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Core.BLL.RegulyKoszyka
{
    public class KrajKlienta : RegulaKoszyka, IRegulaCalegoKoszyka, IRegulaPozycji, ITestowalna
    {
        public KrajKlienta()
        {
            Relacja = RelacjaJestNieJest.Jest;
            Panstwa = new List<string>();
        }

        public override string Opis
        {
            get { return "Warunek z jakiego kraju jest klient"; }
        }

        [FriendlyName("Czy klient jest/ nie jest z wybranych krajów")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public RelacjaJestNieJest Relacja { get; set; }

        [FriendlyName("Wybierz państwa")]
        [PobieranieSlownika(typeof(SlownikPanstw))]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public List<string> Panstwa { get; set; }

        public bool KoszykSpelniaRegule(IKoszykiBLL koszyk)
        {
            return Regula(koszyk);
        }

        public bool PozycjaSpelniaRegule(IKoszykPozycja pozycja, IKoszykiBLL koszyk)
        {
            return Regula(koszyk);
        }

        private bool Regula(IKoszykiBLL koszyk)
        {
            bool wynik = false;
            if (!Panstwa.Any()) throw new InvalidOperationException("Brak wpisanych państw w regule");
            if (koszyk.Adres == null)
            {
                return false;
            }
            if (Panstwa.Any(x => int.Parse(x) == koszyk.Adres.KrajId))
            {
                wynik = true;
            }
            return Relacja == RelacjaJestNieJest.NieJest ? !wynik : wynik;
        }

        public List<string> TestPoprawnosci()
        {
            List<string> wynik = new List<string>();
            if (!Panstwa.Any())
            {
                wynik.Add("Brak wybranych Państw");
            }
            int tmp;
            if (Panstwa.Any(x => !int.TryParse(x, out tmp)))
            {
                wynik.Add("Moduł ma niepoprawne wartości, nalezy skonfigurować kraje na nowo");
            }
            return wynik;
        }
    }
}