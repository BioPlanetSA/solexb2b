using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SolEx.Hurt.Core.BLL.ZadaniaKoszyka
{
    public class DodajPozycjeNaPodstawieZestawu : DodawaniePozycjiBaza
    {
        [FriendlyName("Skład zestawu w formacie idProduktu@iloscwjednostcepodstawowej;idproduktu@iloscwjednostcepodstawowej - Y")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string Zestaw { get; set; }

        public override string Opis
        {
            get { return "Za każdą wielokrotność zestawu X produktów Y dodaj produkt  Z w ilości V i cenie W"; }
        }

        private List<Tuple<int, decimal>> PobierzZestawy()
        {
            List<Tuple<int, decimal>> wynik = new List<Tuple<int, decimal>>();
            string[] elementy = Zestaw.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string s in elementy)
            {
                string[] wiersz = s.Split(new[] { '@' }, StringSplitOptions.None);
                int id = int.Parse(wiersz[0]);
                decimal ilosDecimal;
                TextHelper.PobierzInstancje.SprobojSparsowac(wiersz[1], out ilosDecimal);
                wynik.Add(new Tuple<int, decimal>(id, ilosDecimal));
            }
            return wynik;
        }

        public override decimal WyliczDodawanaIlosc(IKoszykiBLL koszyk)
        {
            int liczbazestawow = int.MaxValue;
            foreach (Tuple<int, decimal> elementZestawu in PobierzZestawy())
            {
                decimal iloscPozycji = koszyk.PobierzPozycje.Where(x => x.ProduktId == elementZestawu.Item1 && x.TypPozycji == TypPozycjiKoszyka.Zwykly).Sum(x => x.IloscWJednostcePodstawowej);
                int pelnych = (int)(iloscPozycji / elementZestawu.Item2);
                if (pelnych < liczbazestawow)
                {
                    liczbazestawow = pelnych;
                }
            }
            return Ilosc * liczbazestawow;
        }
    }
}