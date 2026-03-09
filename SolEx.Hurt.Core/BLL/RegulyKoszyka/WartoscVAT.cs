using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.AtrybutyKlas;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SolEx.Hurt.Core.BLL.RegulyKoszyka
{
    [FriendlyName("Wartość VAT", FriendlyOpis = "Warunek na podstawie stawki VAT")]
    public class WartoscVAT : RegulaKoszyka, IRegulaCalegoKoszyka
    {
        [FriendlyName("Dozwolone stawki VAT oddzielone ; bez znaku %")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string DozwoloneWartosciVat { get; set; }

        private List<decimal> ZwrocListe(string wejscie)
        {
            if (string.IsNullOrEmpty(wejscie)) return new List<decimal>();
            var tmp = wejscie.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            List<decimal> wynik = new List<decimal>(); 
            foreach (string s in tmp)
            {
                decimal war;
                if (decimal.TryParse(s, out war))
                {
                    wynik.Add(war);
                }
            }
            return wynik;
        }

        public bool KoszykSpelniaRegule(IKoszykiBLL koszyk)
        {
            List<decimal> listavat = ZwrocListe(DozwoloneWartosciVat);
            decimal wartosc = koszyk.PobierzPozycje.First().Produkt.Vat;

            return listavat.Any(a => a == wartosc);
        }
    }
}