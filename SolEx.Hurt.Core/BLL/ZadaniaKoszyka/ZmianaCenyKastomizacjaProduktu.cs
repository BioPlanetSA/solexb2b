using System;
using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Interfaces;
using SolEx.Hurt.Model.Web;
using System.Collections.Generic;
using System.Linq;

namespace SolEx.Hurt.Core.BLL.ZadaniaKoszyka
{
    public class ZmianaCenyKastomizacjaProduktu : ZadaniePozycjiKoszyka, IModulStartowy, ITestowalna, IFinalizacjaKoszyka
    {
        public override string Opis
        {
            get { return "Moduł umożliwia zmiene ceny za kastomizacje produktu, dla wybranych cech. Moduł działa tak że szuka wartości wybranej opcji kastomizacji wśród cech w wybranych atrybutach. Moduł współpracuje z modułem ProduktyIndywidualne."; }
        }

        [Wymagane]
        [PobieranieSlownika(typeof(SlownikAtrybutow))]
        [FriendlyName("Atrybuty w którym szukać wartości kastomizacji")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public List<string> Atrybuty { get; set; }

        [Niewymagane]
        [FriendlyName("Cena kastomizacji którą odejmujemy od produktu jeśli wybrana wartość kastomizacji znajduje sie na liście podanych atrybutów")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public decimal CenaZnajdujeSie { get; set; }

        //[Niewymagane]
        //[FriendlyName("Cena kastomizacji którą odejmujemy od produktu jeśli wybrana wartość kastomizacji NIE znajduje sie na liście podanych atrybutów")]
        //public decimal CenaNieZnajdujeSie { get; set; }

        protected HashSet<int> PobierzAtrybutyId()
        {
            return new HashSet<int>(Atrybuty.Select(int.Parse));
        }

        public override bool Wykonaj(IKoszykPozycja pozycja, IKoszykiBLL koszyk)
        {
            throw new NotImplementedException();

           // //lista atrybutów wybranych przez użytkownika
           // var atrybuty = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<AtrybutBll>(koszyk.Klient.Id, null, x => PobierzAtrybutyId().Contains(x.Id));

           // List<Cecha> cechyProduktu = null;
           // if (pozycja.Produkt().Cechy != null && pozycja.Produkt().Cechy.Any())
           // {
           //     cechyProduktu = pozycja.Produkt().Cechy.Values.Select(x => x as Cecha).ToList();
           // }
           //// var pars = pozycja.Produkt().ParametryDostosowywania;
           // foreach (var ip in pozycja.IndywidalneParametry)
           // {
                
           //     //pobieram wartość kastomizowaną pobraną z listy
           //     //ParametrDostosowywania pd = pars.FirstOrDefault(x => x.Parametry == ip.Key);
           //     //if (pd == null) continue;

           //     ////jezeli są 2 atrybuty na stronie, oba zaznaczone zostaną pobrane
           //     //var nazwaAtrybutuZeStrony = pd.Cechy.FirstOrDefault(x => x.Key == ip.Value);

           //     //foreach (var atrybut in atrybuty)
           //     //{
           //     //    //iterujemy po wszystkich nazwach z danego atrybutu
           //     //    foreach (var item in atrybut.ListaCech)
           //     //    {
           //     //        if (cechyProduktu.Contains(item))
           //     //        {
           //     //            if (nazwaAtrybutuZeStrony.Value == item.Nazwa)
           //     //            {
           //     //                decimal nowacena = pozycja.Produkt().FlatCeny.CenaNetto - CenaZnajdujeSie;
           //     //                decimal nowy = Kwoty.WyliczRabatDlaUzyskaniaCeny(pozycja.Produkt().FlatCeny.CenaNetto, nowacena);
           //     //                pozycja.ZmienDodatkowyRabat(nowy, Komunikat, TrybLiczeniaRabatuWKoszyku.SUMUJ);
           //     //            }
           //     //        }
           //     //    }
           //     //}
           // }

           // return true;
        }

        public List<string> TestPoprawnosci()
        {
            List<string> listaBledow = new List<string>();
            listaBledow.AddRange(Przedzial.SpradzWartosc(CenaZnajdujeSie, "CenaZnajdujeSie"));

            return listaBledow;
        }
    }
}