using System.Collections.Generic;
using System.IO;
using System.Linq;
using log4net;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.Importy.Model;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Web;

namespace SolEx.Hurt.Core.Importy.Koszyk
{
    public abstract class ImportBaza
    {
        public ISolexBllCalosc Calosc = SolexBllCalosc.PobierzInstancje;
        public abstract string LadnaNazwa { get; }
        public abstract List<string> Rozszerzenia { get; }

        public abstract List<PozycjaKoszykaImportowana> Przetworz(string dane, out List<Komunikat> bledy, Stream stumien);

        protected void ZnajdzProdukt(string kod, string ilosctekst, string wiersz, List<PozycjaKoszykaImportowana> wynik, List<Komunikat> bledy, Jednostka jednostka = null)
        {
            decimal ilosc;
            int iloscMaxPozycjiWKoszyku = SolexBllCalosc.PobierzInstancje.Konfiguracja.MaksymalnaIloscPozycjiWKoszyku;
            TextHelper.PobierzInstancje.SprobojSparsowac(ilosctekst, out ilosc);
            if (string.IsNullOrWhiteSpace(kod) || ilosc == 0)
            {
                bledy.Add(new Komunikat("W wierszu nie znaleziono kodu kreskowego lub ilości, wiersz: " + wiersz, KomunikatRodzaj.danger,GetType().Name+"NieZnaleziono"));
                return;
            }
            kod = kod.Trim();
           
            List<ProduktBazowy> listaProduktow = Calosc.ProduktyBazowe.ZnajdzProdukty(kod);
            if (!listaProduktow.Any())
            {
                bledy.Add(new Komunikat($"Nie znaleziono produktu: {kod}", KomunikatRodzaj.danger, GetType().Name + "NieZnaleziono"));
                return;
            }
            foreach (ProduktBazowy produktBazowy in listaProduktow)
            {
                if (jednostka != null && produktBazowy.Jednostki.FirstOrDefault(x => x.Id == jednostka.Id) != null)
                {
                    wynik.Add(new PozycjaKoszykaImportowana(produktBazowy)
                    {
                        Ilosc = ilosc,
                        Jednostka = jednostka.Nazwa,
                        Produkt = produktBazowy.Id
                    });
                }
                else
                {
                    wynik.Add(new PozycjaKoszykaImportowana(produktBazowy) {Ilosc = ilosc, Produkt = produktBazowy.Id});
                }
                if (wynik.Count >= iloscMaxPozycjiWKoszyku)
                {
                    bledy.Add(new Komunikat($"Do koszyka można dodać maksymalnie: {iloscMaxPozycjiWKoszyku} pozycji", KomunikatRodzaj.danger, GetType().Name + "ZaDuzoElementow"));
                    ZaDuzoElementow = true;
                    break;
                }
            }
        }

        public bool ZaDuzoElementow { get; set; }

        protected ILog Log
        {
            get
            {
                return LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            }
        }
    }
}
