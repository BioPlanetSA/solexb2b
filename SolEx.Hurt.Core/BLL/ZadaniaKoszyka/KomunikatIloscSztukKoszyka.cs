using System;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Web;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SolEx.Hurt.Core.BLL.ZadaniaKoszyka
{
    [FriendlyName("Komunikat o ilości sztuk w koszyku", FriendlyOpis = "Moduł zlicza ilości dla poszczegółnych pozycji poszyka i sumuje je wg jednostki.")]
    public class KomunikatIloscSztukKoszyka : ZadanieCalegoKoszyka, IModulStartowy, IFinalizacjaKoszyka
    {
        public KomunikatIloscSztukKoszyka()
        {
            Komunikat = "Suma pozycji wg. jednostek miary:{0}";
        }

        [FriendlyName("Komunikat do pokazania, za {0} będzie wpisana liczba sztuk koszyka")]
        [Lokalizowane]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public new string Komunikat { get; set; }

     
        public override bool Wykonaj(IKoszykiBLL koszyk)
        {
            List<KoszykPozycje> pozycje = koszyk.PobierzPozycje;
            if (pozycje!= null && pozycje.Any())
            {
                IEnumerable<IGrouping<long?, IKoszykPozycja>> jednostki = pozycje.GroupBy(x=>x.JednostkaId);

                StringBuilder iloscWgJednostki = new StringBuilder();
                foreach (var jednostkaProduktu in jednostki)
                {
                    decimal wynik;
                    wynik = jednostkaProduktu.Sum(x => x.Ilosc);
                    IKoszykPozycja elementAtOrDefault = jednostkaProduktu.ElementAtOrDefault(0);
                    if (elementAtOrDefault != null)
                    {
                        iloscWgJednostki.AppendFormat(" {0} {1},", wynik.DoLadnejCyfry("0.####"),elementAtOrDefault.Jednostka().Nazwa);
                    }
                }
                
                WyslijWiadomosc(string.Format(Komunikat, iloscWgJednostki.ToString().TrimEnd(',')), KomunikatRodzaj.info);
            }
            return true;
        }
    }
}