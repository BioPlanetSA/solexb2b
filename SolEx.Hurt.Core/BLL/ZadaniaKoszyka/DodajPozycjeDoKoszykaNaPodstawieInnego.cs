using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using System.Collections.Generic;
using System.Linq;

namespace SolEx.Hurt.Core.BLL.ZadaniaKoszyka
{
    public class DodajPozycjeDoKoszykaNaPodstawieInnego : DodawaniePozycjiBaza
    {
        public override string Opis
        {
            get { return "Za każdą wielokrotność X produktów Y dodaj produkt  Z w ilości V i cenie W"; }
        }

        [FriendlyName("Produkty wymagane Y")]
        [PobieranieSlownika(typeof(SlownikProduktow))]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public List<string> IdProduktuWymaganego { get; set; }

        public override decimal WyliczDodawanaIlosc(IKoszykiBLL koszyk)
        {
            //string[] listaid = IdProduktuWymaganego.Split(new string[] {";"}, StringSplitOptions.RemoveEmptyEntries);
            List<int> listaIDProduktow = new List<int>();

            foreach (string id in IdProduktuWymaganego)
            {
                int tmpid;
                if (int.TryParse(id, out tmpid))
                {
                    listaIDProduktow.Add(tmpid);
                }
            }

            decimal dodana = koszyk.PobierzPozycje.Where(x => listaIDProduktow.Any(a => a == x.ProduktId) && x.TypPozycji == TypPozycjiKoszyka.Zwykly).Sum(x => x.IloscWJednostcePodstawowej);
            int iloscpaczek = (int)(dodana / IloscWielokrotnosc);
            return iloscpaczek * Ilosc;
        }

        public new List<string> TestPoprawnosci()
        {
            List<string> listaBledow = new List<string>();
            foreach (var produkt in IdProduktuWymaganego)
            {
                int produktId;
                if (int.TryParse(produkt, out produktId))
                {
                    ProduktBazowy prod = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<ProduktBazowy>(produktId);
                    if (prod == null)
                    {
                        listaBledow.Add(string.Format("Brak towaru o id:{0}", produktId));
                    }
                }
                else
                {
                    listaBledow.Add(string.Format("Brak towaru: {0}", produkt));
                }
            }
            return listaBledow;
        }
    }
}