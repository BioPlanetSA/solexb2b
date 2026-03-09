using ServiceStack.Common;
using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Interfaces;
using SolEx.Hurt.Model.Web;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SolEx.Hurt.Core.BLL.ZadaniaKoszyka
{
    public class DodajPozycjeDoKoszyka : ZadanieCalegoKoszyka, IModulStartowy, IFinalizacjaKoszyka, ITestowalna
    {
        public override string Opis
        {
            get { return "Dodaje pozycję do koszyka."; }
        }

        [PobieranieSlownika(typeof(SlownikProduktow))]
        [FriendlyName("Produkt")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public int IdProduktow { get; set; }

        //[FriendlyName("Symbol produktu")]
        //[Niewymagane]
        //public string SymbolProduku { get; set; }

        [FriendlyName("Ilość w jednostce podstawowej")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public decimal Ilosc { get; set; }

        [FriendlyName("Cena netto, jeśli 0, to dodaje w cenie klienta")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public decimal Cena { get; set; }

        [FriendlyName("Rodzaj komunikatu")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public KomunikatRodzaj RodzajKomunikatu { get; set; }

        public List<string> TestPoprawnosci()
        {
            var produkty = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<ProduktBazowy>(null, x => x.Widoczny).ToList();
            List<string> listaBledow = Przedzial.SpradzWartosc(Ilosc, "Ilość");
            listaBledow.AddRange(Przedzial.SpradzWartosc(Cena, "Cena"));
            foreach (var produktBazowy in produkty)
            {
                if (produktBazowy.Id == IdProduktow)
                    return listaBledow;
            }
            listaBledow.Add(string.Format("Brak towaru o numerze id: {0}", IdProduktow));
            return listaBledow;
        }

        //protected  int IdPoz
        //{
        //    get
        //    {
        //        if (IdProduktow != 0) return IdProduktow;
        //        if (!string.IsNullOrWhiteSpace(SymbolProduku))
        //        {
        //            ProduktBazowy pb = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<ProduktBazowy>(SymbolProduku,  SolexBllCalosc.PobierzInstancje.Konfiguracja.JezykIDDomyslny);
        //                if (pb != null)
        //                {
        //                    return pb.produkt_id;
        //                }
        //        }
        //        return 0;
        //    }
        //}
        public override bool Wykonaj(IKoszykiBLL koszyk)
        {
            if (IdProduktow == 0)
            {
                throw new Exception("Nie znaleziono produktu. Bład w regule koszyka: DodajPozycjeDoKoszyka");
            }
            if (Ilosc > 0)
            {
                koszyk.DodajaAutomatyczny(IdProduktow, Ilosc, Cena, Id);
                WyslijWiadomosc(Komunikat, RodzajKomunikatu);
            }
            return true;
        }
    }
}