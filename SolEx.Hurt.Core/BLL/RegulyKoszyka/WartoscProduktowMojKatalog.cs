using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using System;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core.BLL.RegulyKoszyka
{
    public class WartoscProduktowMojKatalog : RegulaKoszyka, IRegulaCalegoKoszyka
    {
        public override string Opis
        {
            get { return "Sprawdza kryterium wartości produktów z mojego katalogu"; }
        }

        [FriendlyName("Wartość koszyka")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public decimal Kwota { get; set; }

        [FriendlyName("Czy liczymy wg netto czy brutto")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public bool CzyBrutto { get; set; }

        [FriendlyName("Wartość koszyka ma być")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public Wartosc WartoscWarunek { get; set; }

        public bool KoszykSpelniaRegule(IKoszykiBLL koszyk)
        {
            decimal wartosc = 0;
            IKlient k = koszyk.Klient;
            foreach (var p in koszyk.PobierzPozycje)
            {
                if (k.MojKatalog.Contains(p.ProduktId))
                {
                    wartosc += (CzyBrutto ? p.WartoscBrutto : p.WartoscNetto);
                }
            }
            return wartosc.PorownajWartosc(Kwota, WartoscWarunek);
        }
    }
}