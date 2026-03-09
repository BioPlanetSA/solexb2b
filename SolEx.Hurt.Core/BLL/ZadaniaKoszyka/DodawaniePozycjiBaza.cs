using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Interfaces;
using SolEx.Hurt.Model.Web;
using System;
using System.Collections.Generic;

namespace SolEx.Hurt.Core.BLL.ZadaniaKoszyka
{
    public abstract class DodawaniePozycjiBaza : ZadanieCalegoKoszyka, IModulStartowy, IFinalizacjaKoszyka, ITestowalna
    {
        protected DodawaniePozycjiBaza()
        {
            Rodzaj = KomunikatRodzaj.success;
        }

        [FriendlyName("Produkty do dodania - Z ")]
        [PobieranieSlownika(typeof(SlownikProduktow))]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public int IdProduktuDoDodania { get; set; }

        [FriendlyName("Ilość do dodania za każdą wielokrotność w jednostce podstawowej - V")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public decimal Ilosc { get; set; }

        [Niewymagane]
        [FriendlyName("Cena netto, jeśli 0, to dodaje w cenie klienta - W")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public decimal Cena { get; set; }

        [FriendlyName("Wielokrotność na podstawie której dodajemy - X")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public decimal IloscWielokrotnosc { get; set; }

        public abstract decimal WyliczDodawanaIlosc(IKoszykiBLL koszyk);

        [FriendlyName("Rodzaj komunikatu")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public KomunikatRodzaj Rodzaj { get; set; }

        public List<string> TestPoprawnosci()
        {
            var produkty = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<ProduktBazowy>(null, x => x.Widoczny);
            List<string> listaBledow = Przedzial.SpradzWartosc(Ilosc, "Ilość");
            listaBledow.AddRange(Przedzial.SpradzWartosc(Cena, "Cena"));
            listaBledow.AddRange(Przedzial.SpradzWartosc(IloscWielokrotnosc, "Ilość Wielokrotności"));
            if (IdProduktuDoDodania != 0)
            {
                foreach (var prod in produkty)
                {
                    if (prod.Id == IdProduktuDoDodania)
                        return listaBledow;
                }
                listaBledow.Add(string.Format("Brak towaru o id: {0}", IdProduktuDoDodania));
            }
            return listaBledow;
        }

        public override bool Wykonaj(IKoszykiBLL koszyk)
        {
            if (IloscWielokrotnosc <= 0)
            {
                throw new Exception("Wielokrotność musi być >0");
            }
            if (Ilosc <= 0)
            {
                throw new Exception("Dodawana ilość musi być >0");
            }
            decimal dododania = WyliczDodawanaIlosc(koszyk);
            if (dododania > 0)
            {
                if (!string.IsNullOrEmpty(Komunikat))
                {
                    WyslijWiadomosc(Komunikat, Rodzaj);
                }
                koszyk.DodajaAutomatyczny(IdProduktuDoDodania, dododania, Cena, Id);
            }
            return true;
        }
    }
}