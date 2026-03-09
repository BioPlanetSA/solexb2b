using SolEx.Hurt.Core.BLL.RegulyKoszyka;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.AtrybutyKlas;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SolEx.Hurt.Core.BLL.ZadaniaKoszyka
{
    [Obsolete]
    public class DomyslnaDostawaZKategorii : ZadanieCalegoKoszyka, IModulStartowy
    {
        public override List<Type> WykluczoneWarunki
        {
            get
            {
                List<Type> wynik = new List<Type>();
                wynik.Add(typeof(SposobDostawy));
                return wynik;
            }
        }

        public override string PokazywanaNazwa
        {
            get { return "Domyślna dostawa pobierana z kategorii klientów"; }
        }

        public override string Opis
        {
            get { return "Moduł ustawia domyślną dostawe na podstawie wybranej kategorii klienta"; }
        }

        [FriendlyName("Początek kategorii klientów z której będzie pobierana forma dostawy")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string PoczatekKategoriiDostawy { get; set; }

        private static string sPattern = @"(?'reszta'[A-Za-zęóąśłżźćńĘÓĄŚŁŻŹĆŃ _]{1,})_(?'symbol'[A-Za-zęóąśłżźćńĘÓĄŚŁŻŹĆŃ ]{1,})([:0-9]{1,})$";
        private static Regex re = new Regex(sPattern, RegexOptions.ExplicitCapture);

        public override bool Wykonaj(IKoszykiBLL koszyk)
        {
            if (koszyk.KosztDostawyId == null)
            {
                //IKlient k = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<Klient>(koszyk.klient_id);
                //var staryRabat =k.Kategorie.FirstOrDefault(a => a.nazwa.StartsWith(PoczatekKategoriiDostawy));

                //if (staryRabat != null)
                //{
                //    var wszystkie = SolexBllCalosc.PobierzInstancje.ZadaniaBLL.PobierzZadaniaCalegoKoszykaKtorePasuja<ISposobDostawy>(koszyk);
                //    foreach (ZadanieCalegoKoszyka zadanieKoszyka in wszystkie)
                //    {
                //        ISposobDostawy tmp = zadanieKoszyka as ISposobDostawy;
                //        if (tmp != null &&
                //            re.Match(staryRabat.nazwa).Groups["symbol"].Value.ToUpper() == tmp.SymbolProduktu)
                //        {
                //            koszyk.KosztDostawyID =zadanieKoszyka.powiazaneZadanie.id;
                //        }
                //    }
                //}
            }

            return true;
        }
    }
}