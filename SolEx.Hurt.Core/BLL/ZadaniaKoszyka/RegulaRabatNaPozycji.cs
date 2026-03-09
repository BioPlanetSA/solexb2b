using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;
using System;
using System.Collections.Generic;

namespace SolEx.Hurt.Core.BLL.ZadaniaKoszyka
{
    public enum OdCzegoLiczyc
    {
        OdCenyPrzedRabatem,
        OdCenyPoRabacie,
        OdCenyPoRabacieBezGradacji
    }
    [FriendlyName("Reguła rabatu na pozycje",FriendlyOpis = "Uwaga. Opcja TypLiczeniaRabatu słuzy do tego, jaki ma być rabat gdy klient spełnia warunki kilku modułów RegulaRabatNaPozycji," +
                                                            "np pierwszy daje 2%, drugi 5%. Opcja sumuj - ostateczny rabat wyniesie 7%. NADPISZ - rabat brany jest z ostatniego modułu w tym przypadku 5%")]
    public class RegulaRabatNaPozycji : ZadaniePozycjiKoszyka, IModulStartowy, IFinalizacjaKoszyka, ITestowalna
    {
       public RegulaRabatNaPozycji()
        {
            WarunekNaliczeniaRabatu = Wartosc.Dowolna;
        }

        [FriendlyName("Ustaw rabat tylko jeśli rabat z modułu  jest od rabatu z rabatów")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public Wartosc WarunekNaliczeniaRabatu { get; set; }

        [FriendlyName("Jeśli jest kilka modułów rabatowych w koszyku to ich rabaty")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public TrybLiczeniaRabatuWKoszyku TypLiczeniaRabatu { get; set; }

        [FriendlyName("Wartość rabatu procentowa")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public decimal Rabat { get; set; }

        [FriendlyName("Od czego liczyć rabat")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public OdCzegoLiczyc JakLiczyc { get; set; }

        public List<string> TestPoprawnosci()
        {
            List<string> listaBledow = Przedzial.SpradzWartosc(Rabat, "Rabat");
            return listaBledow;
        }

        public override bool Wykonaj(IKoszykPozycja pozycja, IKoszykiBLL koszyk)
        {
            if (Rabat == 0)
            {
                return true;
            }

            decimal wartosc = Rabat;
            if (JakLiczyc == OdCzegoLiczyc.OdCenyPrzedRabatem && pozycja.Produkt.FlatCeny.CenaNetto > 0)
            {
                decimal x = pozycja.Produkt.FlatCeny.CenaHurtowaNetto;
                decimal rabatpelen = pozycja.Produkt.FlatCeny.Rabat + Rabat;
                decimal y = pozycja.Produkt.FlatCeny.CenaNetto;

                decimal wynik = (x * (100 - rabatpelen) / 100) / y;
                wartosc = (1 - wynik) * 100;
            }
            if (JakLiczyc == OdCzegoLiczyc.OdCenyPoRabacieBezGradacji)
            {
              //  var cenyPrzedGradacja = ;// SolexBllCalosc.PobierzInstancje.Rabaty.WyliczCeneDlaKlientaZalogowanego(koszyk.Klient, pozycja.Produkt);
                decimal x = pozycja.Produkt.FlatCeny.CenaNettoBezGradacji;
                decimal y = pozycja.Produkt.FlatCeny.CenaNetto;

                //decimal rabatpelen = pozycja.Produkt.FlatCeny.Rabat + Rabat;
                decimal rabatwartosc = x - (x * (100 - Rabat) / 100);
                var a = y - rabatwartosc;
                decimal wynik = (a * 100) / y;
                wartosc = (1 - wynik / 100) * 100;
            }
            if (wartosc.PorownajWartosc(pozycja.Produkt.FlatCeny.Rabat, WarunekNaliczeniaRabatu))
            {
                pozycja.ZmienDodatkowyRabat(wartosc, Komunikat, TypLiczeniaRabatu);
            }
            return true;
        }
    }
}