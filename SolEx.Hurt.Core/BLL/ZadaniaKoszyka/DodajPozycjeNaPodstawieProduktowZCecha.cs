using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using System.Collections.Generic;
using System.Linq;

namespace SolEx.Hurt.Core.BLL.ZadaniaKoszyka
{
    public class DodajPozycjeNaPodstawieProduktowZCecha : DodawaniePozycjiBaza
    {
        public override string Opis
        {
            get { return "Za każdą wielokrotność X produktów z cechą Y dodaj produkt  Z w ilości V i cenie W"; }
        }

        [FriendlyName("Wymagana cecha - Y")]
        [PobieranieSlownika(typeof(SlownikCech))]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public int IdWymaganejCechy { get; set; }

        public override decimal WyliczDodawanaIlosc(IKoszykiBLL koszyk)
        {
            decimal dodana = koszyk.PobierzPozycje.Where(x => x.Produkt.Cechy.Values.Any(z => z.Id == IdWymaganejCechy) && x.TypPozycji == TypPozycjiKoszyka.Zwykly).Sum(x => x.IloscWJednostcePodstawowej);
            int iloscpaczek = (int)(dodana / IloscWielokrotnosc);
            return iloscpaczek * Ilosc;
        }

        public new List<string> TestPoprawnosci()
        {
            List<string> listaBledow = new List<string>();
            if (IdWymaganejCechy != 0)
            {
                CechyBll cecha = SolexBllCalosc.PobierzInstancje.CechyAtrybuty.PobierzWszystkieCechy(SolexBllCalosc.PobierzInstancje.Konfiguracja.JezykIDPolski)[IdWymaganejCechy];
                if (cecha == null)
                {
                    listaBledow.Add(string.Format("Brak cechy o id: {0}", IdWymaganejCechy));
                }
            }
            return listaBledow;
        }
    }
}