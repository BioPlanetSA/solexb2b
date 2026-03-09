using ServiceStack.Common;
using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SolEx.Hurt.Core.BLL.RegulyKoszyka
{
    public class RegulaKwoty : RegulaKoszyka, IRegulaCalegoKoszyka, IRegulaPozycji, ITestowalna
    {
        public override string Opis
        {
            get { return "Rozbudowany moduł wartości koszyka"; }
        }

        [FriendlyName("Wartość koszyka")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public decimal Kwota { get; set; }

        [FriendlyName("Czy liczymy wg netto czy brutto. Nie - wartość liczona wg cen nettto, Tak - wg cen brutto")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public bool CzyBrutto { get; set; }

        [FriendlyName("Wartość koszyka ma być")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public Wartosc WartoscWarunek { get; set; }

        [Niewymagane]
        [FriendlyName("Cechy produktów które mają być sumowane, jeśli puste, to sumuje wszystkie.")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        [PobieranieSlownika(typeof(SlownikCech))]
        public List<long> Cechy { get; set; }

        public List<string> TestPoprawnosci()
        {
            List<string> listaBledow = new List<string>();
            if (Cechy!=null && Cechy.Any())
            {
                List<CechyBll> listaWszystkich = SolexBllCalosc.PobierzInstancje.CechyAtrybuty.PobierzWszystkieCechy(SolexBllCalosc.PobierzInstancje.Konfiguracja.JezykIDPolski).Values.ToList();
                foreach (var c in Cechy)
                {
                    CechyBll cecha = SolexBllCalosc.PobierzInstancje.CechyAtrybuty.PobierzWszystkieCechy(SolexBllCalosc.PobierzInstancje.Konfiguracja.JezykIDPolski)[c];
                    if (cecha == null || !listaWszystkich.Contains(cecha))
                    {
                        listaBledow.Add(string.Format("Brak cechy o id: {0}", c));
                    }
                }
            }
            return listaBledow;
        }
        public bool KoszykSpelniaRegule(IKoszykiBLL koszyk)
        {
            decimal wartosc = 0;

            if (Cechy==null || !Cechy.Any())
            {
                wartosc = CzyBrutto ? koszyk.CalkowitaWartoscHurtowaBruttoPoRabacie() : koszyk.CalkowitaWartoscHurtowaNettoPoRabacie();
            }
            else
            {
                foreach (var p in koszyk.PobierzPozycje)
                {
                    foreach (var c in p.Produkt.IdCechPRoduktu)
                    {
                        if (Cechy.Contains(c))
                        {
                            wartosc += (CzyBrutto ? p.WartoscBrutto : p.WartoscNetto);
                            break;
                        }
                    }
                }
            }
            return wartosc.PorownajWartosc(Kwota, WartoscWarunek);
        }

        public bool PozycjaSpelniaRegule(IKoszykPozycja pozycja, IKoszykiBLL koszyk)
        {
            return KoszykSpelniaRegule(koszyk);
        }
    }
}