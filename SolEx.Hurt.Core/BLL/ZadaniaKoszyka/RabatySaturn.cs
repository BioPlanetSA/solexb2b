using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core.BLL.ZadaniaKoszyka
{
    public class RabatySaturn : ZadanieCalegoKoszyka, IModulStartowy, IFinalizacjaKoszyka
    {
        public override string Opis
        {
            get { return "Rabatowanie Saturna"; }
        }

        private static string sPattern = @"(?'reszta'[A-Za-zęóąśłżźćńĘÓĄŚŁŻŹĆŃ _]{1,})_(?'symbol'[A-Za-zęóąśłżźćńĘÓĄŚŁŻŹĆŃ ]{1,})([:0-9]{1,})$";
        private static Regex re = new Regex(sPattern, RegexOptions.ExplicitCapture);

        [FriendlyName("Nazwa atrybutu rabatowego")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string NazwaAtrybutuRabatowego { get; set; }

        [FriendlyName("Poczatek domyślnej dostawy")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string DomyslnaDostawa { get; set; }

        public ISolexBllCalosc ISolexBllCalosc = SolexBllCalosc.PobierzInstancje;

        public override bool Wykonaj(IKoszykiBLL koszyk)
        {
            List<KoszykPozycje> pozycje = koszyk.PobierzPozycje;
            if (koszyk.KosztDostawy() == null)
            {
                ZresetujRabaty(pozycje);
                return true;
            }

            DomyslnaDostawa = DomyslnaDostawa.ToLower();
            NazwaAtrybutuRabatowego = NazwaAtrybutuRabatowego.ToLower();

            string dostawa = koszyk.KosztDostawy().ProduktDostawy.Kod.ToLower();

            IKlient kl = koszyk.Klient;
            var kats = ISolexBllCalosc.DostepDane.Pobierz<KategoriaKlienta>(null, x => kl.Kategorie.Contains(x.Id));
            var staryRabat = kats.FirstOrDefault(a => a.Nazwa.StartsWith(DomyslnaDostawa));

            if (staryRabat == null)
            {
                ZresetujRabaty(pozycje);
                return true;
            }

            bool innaKategoria = re.Match(staryRabat.Nazwa).Groups["symbol"].Value.ToLower() == dostawa;
            if (!innaKategoria)
            {
                foreach (IKoszykPozycja pozycja in pozycje)
                {
                    if (pozycja.Produkt.FlatCeny.TypRabatu != (int)RabatTyp.Promocja)
                    {
                        var cecha = pozycja.Produkt.Cechy.FirstOrDefault(a => a.Value.NazwaAtrybutu.ToLower() == NazwaAtrybutuRabatowego);
                        if (cecha.Value != null)
                        {
                            var nowyRabat = kats.FirstOrDefault(
                                    a =>
                                    re.Match(a.Nazwa).Groups["symbol"].Value == dostawa && re.Match(a.Nazwa).Groups["reszta"].Value == cecha.Value.Symbol.ToLower());

                            if (nowyRabat != null && pozycja.Produkt.FlatCeny.CenaNetto > 0)
                            {
                                decimal wartoscNowegoRabatu = Convert.ToDecimal(nowyRabat.Nazwa.Split(':').Last());
                                var wartoscRabatu = pozycja.Produkt.FlatCeny.Rabat;
                                var nowyrabat = wartoscNowegoRabatu - wartoscRabatu;

                                decimal docelowaCena = (pozycja.Produkt.FlatCeny.CenaHurtowaNetto * (100 -
                                                       (pozycja.Produkt.FlatCeny.Rabat + nowyrabat)) / 100m);

                                decimal nowaWartoscRabatu = (100 -
                                                             ((docelowaCena * 100) / pozycja.Produkt.FlatCeny.CenaNetto));

                                pozycja.RabatDodatkowy = nowaWartoscRabatu;
                                //pozycja.Produkt.FlatCeny.rabat = wartoscNowegoRabatu;
                                if (nowaWartoscRabatu > 0)
                                    pozycja.PowodDodatkowegoRabatu = SolexBllCalosc.PobierzInstancje.Konfiguracja.PobierzTlumaczenie(pozycja.Klient.JezykId,"zniżkę za sposób dostawy");
                                else if (nowaWartoscRabatu < 0)
                                    pozycja.PowodDodatkowegoRabatu = SolexBllCalosc.PobierzInstancje.Konfiguracja.PobierzTlumaczenie(pozycja.Klient.JezykId, "zwyżkę za sposób dostawy");
                            }
                            else ZresetujRabat(pozycja);
                        }
                        else ZresetujRabat(pozycja);
                    }
                    else ZresetujRabat(pozycja);
                }
            }
            else ZresetujRabaty(pozycje);

            return true;
        }

        public static void ZresetujRabat(IKoszykPozycja pozycja)
        {
            if (pozycja.Produkt.FlatCeny.TypRabatu != (int)RabatTyp.Promocja)
            {
                var wartoscRabatu = pozycja.Produkt.FlatCeny.Rabat;
                pozycja.RabatDodatkowy = 0;
                pozycja.PowodDodatkowegoRabatu = "";
            }
        }

        private void ZresetujRabaty(List<KoszykPozycje> pozycje)
        {
            foreach (var pozycja in pozycje)
            {
                ZresetujRabat(pozycja);
            }
        }
    }
}