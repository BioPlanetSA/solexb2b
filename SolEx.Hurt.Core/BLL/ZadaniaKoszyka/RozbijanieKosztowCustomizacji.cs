using System;
using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Web;
using System.Collections.Generic;
using System.Linq;

namespace SolEx.Hurt.Core.BLL.ZadaniaKoszyka
{
    public class RozbijanieKosztowCustomizacji : ZadanieCalegoKoszyka, IFinalizacjaKoszyka, IModulStartowy
    {
        [FriendlyName("Cena netto za customizacje - Jeśli pole niewypełnione to brak kosztu")]
        public decimal CenaNetto { get; set; }

        [FriendlyName("Produkty dodawany jako pozycja koszyka")]
        [PobieranieSlownika(typeof(SlownikProduktow))]
        [Niewymagane]
        public int IdProduktu { get; set; }

        [FriendlyName("Atrybut - Do indywidualizacji z wybranym atrybutem nie będzie doliczona dodatkowa kwota")]
        [PobieranieSlownika(typeof(SlownikAtrybutow))]
        public int IdAtrybutu { get; set; }

        public override bool Wykonaj(IKoszykiBLL koszyk)
        {
            throw new NotImplementedException();
            //decimal calkowitaNetto = 0;
            //AtrybutBll atrybut = null;
            //if (IdAtrybutu != 0)
            //{
            //    atrybut = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<AtrybutBll>(IdAtrybutu, koszyk.Klient.JezykId);

            //    foreach (var pozycja in koszyk.PobierzPozycje())
            //    {
            //        List<Cecha> listaCechProduktu = new List<Cecha>();
            //        if (pozycja.Produkt().Cechy != null && pozycja.Produkt().Cechy.Any())
            //        {
            //            listaCechProduktu = pozycja.Produkt().Cechy.Values.Select(x => x as Cecha).ToList();
            //        }
                   
            //        //var pars = ParametrDostosowywania.PrzetworzNaKolekcje(pozycja.Produkt().ParametryDostosowywania, listaCechProduktu);

            //        //if (pozycja.IndywidalneParametry != null && pozycja.IndywidalneParametry.Any())
            //        //{
            //        //    foreach (var ip in pozycja.IndywidalneParametry)
            //        //    {
            //        //        ParametrDostosowywania pd = pars.FirstOrDefault(x => x.Parametry == ip.Key);
            //        //        if (pd == null) continue;

            //        //        var nazwaAtrybutuZeStrony = pd.Cechy.FirstOrDefault(x => x.Key == ip.Value);

            //        //        foreach (var item in atrybut.ListaCech)
            //        //        {
            //        //            if (listaCechProduktu.Contains(item))
            //        //            {
            //        //                if (nazwaAtrybutuZeStrony.Value != item.Nazwa)
            //        //                {
            //        //                    calkowitaNetto += CenaNetto * pozycja.Ilosc;
            //        //                }
            //        //            }
            //        //        }
            //        //    }
            //        //}

            //    }
            //}
            //else
            //{
            //    foreach (var pozycja in koszyk.PobierzPozycje())
            //    {
            //        if (pozycja.IndywidalneParametry != null && pozycja.IndywidalneParametry.Any())
            //        {
            //            calkowitaNetto += (pozycja.IndywidalneParametry.Count * CenaNetto * pozycja.Ilosc);
            //        }
            //    }
            //}
            //if (calkowitaNetto != 0)
            //{
            //    SolexBllCalosc.PobierzInstancje.Koszyk.DodajPozycje(koszyk, null, new KoszykPozycje { ProduktId = IdProduktu, Ilosc = 1, WymuszonaCenaNettoPrzedstawiciel = calkowitaNetto, TypPozycji = TypPozycjiKoszyka.Automatyczny });
            //}
            //return true;
        }
    }
}