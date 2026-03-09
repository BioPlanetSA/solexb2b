using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Web;
using System.Collections.Generic;
using System.Linq;

namespace SolEx.Hurt.Core.BLL.ZadaniaKoszyka
{
    [FriendlyName("Przekroczone stany", FriendlyOpis = "Moduł pobiera stany z wskazanych magazynów lub magazynu wybranego przez klienta")]
    public class PrzekroczoneStany : PrzekroczoneStanyBaza, IModulStartowy, IFinalizacjaKoszyka
    {
        [FriendlyName("Kiedy blokować złożenie zamówienia")]
        [WidoczneListaAdmin(false, false, true, false)]
        public BlokadaKoszyka BlokadaPoPrzekroczeniu { get; set; }

        [Niewymagane]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        [FriendlyName("Kolor tla dla produktow dostepnch - należy wpisać w formie hex czyli np #ff0000. Przydkładowy edytor: http://www.colorpicker.com/")]
        public string KolorOk { get; set; }

        [Niewymagane]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        [FriendlyName("Kolor tla dla produktow przekroczonych - należy wpisać w formie hex czyli np #ff0000. Przydkładowy edytor: http://www.colorpicker.com/")]
        public string KolorPrzekroczone { get; set; }

        [Niewymagane]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        [FriendlyName("Kolor tla dla produktow  niedostepnch - należy wpisać w formie hex czyli np #ff0000. Przydkładowy edytor: http://www.colorpicker.com/")]
        public string KolorNiedostepne { get; set; }

        public override bool Wykonaj(IKoszykiBLL koszyk)
        {
            Dictionary<IKoszykPozycja, decimal> braki = null;
            base.Wykonaj(koszyk, ref braki);

            bool przekroczono = false;
            string magazynDoInformacjiWKomunikacie = null;
            //czy bierzemy magazyn z koszyka podstawowy
            if (this.PobieracStanyTylkoZMagazynuPodstawowegoKoszyka && !string.IsNullOrEmpty(koszyk.MagazynRealizujacy))
            {
                magazynDoInformacjiWKomunikacie = koszyk.MagazynRealizujacy;
            }

            foreach (IKoszykPozycja p in koszyk.PobierzPozycje.Where(x => x.TypPozycji != TypPozycjiKoszyka.ZaPunkty))
            {
                p.KolorTla = string.IsNullOrEmpty(p.KolorTla) ? KolorOk : p.KolorTla;
               
                if (p.StanKoszyk == StanKoszyk.Przekroczony)
                {
                    przekroczono = true;
                    p.KolorTla = KolorPrzekroczone;
                }

                if (p.StanKoszyk == StanKoszyk.Niedostepy)
                {//toco wyzej ale dodaotkowo
                    przekroczono = true;
                    p.KolorTla = KolorNiedostepne;
                }
            }
            bool blokuj = (przekroczono && BlokadaPoPrzekroczeniu == BlokadaKoszyka.BlokujGdyPrzekroczone) || (koszyk.PobierzPozycje.All(x => x.StanKoszyk == StanKoszyk.Niedostepy) && BlokadaPoPrzekroczeniu == BlokadaKoszyka.BlokujGdyWszystkieNiedostepne);
            if (przekroczono && !string.IsNullOrEmpty(Komunikat))
            {
                //czy mamy magazyn wybrany przez klienta
                if (!string.IsNullOrEmpty(magazynDoInformacjiWKomunikacie))
                {
                    WyslijWiadomosc(string.Format("[{0}] {1}", magazynDoInformacjiWKomunikacie, Komunikat), KomunikatRodzaj.danger);
                }
                else
                {
                    WyslijWiadomosc(Komunikat, KomunikatRodzaj.danger);
                }
            }
            return !blokuj;
        }
    }
}