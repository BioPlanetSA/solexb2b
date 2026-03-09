using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Web;
using System;
using System.Linq;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core.BLL.ZadaniaKoszyka
{
    [ModulStandardowy]
    public class BlokadaFinalizacjiKoszykaZaPunkty : ZadanieCalegoKoszyka, IFinalizacjaKoszyka, IModulStartowy
    {
        public BlokadaFinalizacjiKoszykaZaPunkty()
        {
            Komunikat = "Niestety nie posiadasz wystarczającej ilosci punktow,. AKtualnie posiadasz {0} punktów, a chesz wydać {1}. Brakuje Ci {2} punktów aby zamówić wszystkie nagrody";
            KomunikatGdySpelnia = "Posiadasz {0} punktów, wykorzystujesz aktualnie {1} na nagrody, pozostanie {2} punktów na Twoim koncie.";
        }

        public override string Opis
        {
            get { return "Blokada finalizacji koszyka za punkty"; }
        }

        [FriendlyName("Komunikat z informacją ile klient poasiada punktów oraz ile wykorzystano.")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        [Lokalizowane]
        public string KomunikatGdySpelnia { get; set; }

        public override bool Wykonaj(IKoszykiBLL koszyk)
        {
            bool czySpelnia = true;
            decimal wartPunktow = 0;
            decimal iloscPunktow = 0;
            if (koszyk != null)
            {
                wartPunktow = WyliczWartoscPunktow(koszyk);
                iloscPunktow = WyliczIloscPunktowKlienta(koszyk.Klient);
                czySpelnia = iloscPunktow >= wartPunktow;
            }

            if (wartPunktow == 0) return true;

            if (!string.IsNullOrEmpty(KomunikatGdySpelnia) && czySpelnia)
            {
                WyslijWiadomosc(string.Format(KomunikatGdySpelnia, iloscPunktow.DoLadnejCyfry(), wartPunktow.DoLadnejCyfry(), (iloscPunktow - wartPunktow).DoLadnejCyfry()), KomunikatRodzaj.success);
            }
            else if (!string.IsNullOrEmpty(Komunikat) && !czySpelnia)
            {
                WyslijWiadomosc(string.Format(Komunikat, iloscPunktow.DoLadnejCyfry(), wartPunktow.DoLadnejCyfry(), (wartPunktow - iloscPunktow).DoLadnejCyfry()), KomunikatRodzaj.danger);
            }
            return czySpelnia;
        }

        public decimal WyliczIloscPunktowKlienta(IKlient idKlienta)
        {
            return SolexBllCalosc.PobierzInstancje.PunktyDostep.PobierzPunktyKlientaLacznie(idKlienta);
        }

        public decimal WyliczWartoscPunktow(IKoszykiBLL koszyk)
        {
            return koszyk.PobierzPozycje.Where(x => x.TypPozycji == TypPozycjiKoszyka.ZaPunkty).Sum(x => x.Produkt.CenaWPunktach * x.Ilosc);
        }
    }
}