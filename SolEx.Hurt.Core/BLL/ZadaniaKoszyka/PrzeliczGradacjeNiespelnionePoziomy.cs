using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceStack.Common;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Core;
using SolEx.Hurt.Model.Web;

namespace SolEx.Hurt.Core.BLL.ZadaniaKoszyka
{
    [ModulStandardowy]
    public class PrzeliczGradacjeNiespelnionePoziomy : ZadanieCalegoKoszyka, IModulStartowy, IFinalizacjaKoszyka
    {
        public override string Opis
        {
            get { return ""; }
        }

        public override bool Wykonaj(IKoszykiBLL koszyk)
        {
            //czy w ogole gradacje sa aktywne i brane sa koszyki pod uwage
            if (!SolexBllCalosc.PobierzInstancje.Konfiguracja.GradacjeAktywne || !SolexBllCalosc.PobierzInstancje.Konfiguracja.ZCzegoLiczycGradacje.Contains(ZCzegoLiczycGradacje.Koszyk))
            {
                return true;
            }

            HashSet<IProduktKlienta> listaProduktowZGradacjami =new HashSet<IProduktKlienta>( koszyk.PobierzPozycje.Where(x => x.Produkt.GradacjePosortowane != null).Select(x => x.Produkt) );

            if (!listaProduktowZGradacjami.Any())
            {
                return true;
            }

            HashSet<long> idProduktowKtoreSaJuzPoliczone = new HashSet<long>();

            //sa gradacje - sprawdzam pozycja po pozycji odejmujac ilosci kupione w koszyku i sprawdzamy cene - ustawiamy taki rabat zeby byl wlasciwy
            foreach (IProduktKlienta produktKlienta in listaProduktowZGradacjami)
            {
                if (idProduktowKtoreSaJuzPoliczone.Contains(produktKlienta.Id))
                {
                    continue;
                }

                idProduktowKtoreSaJuzPoliczone.UnionWith(produktKlienta.GradacjeProduktyKtorychZakupyLiczycWspolnie);

                var pozycjeProduktowObrabianych = koszyk.PobierzPozycje.Where(x => produktKlienta.GradacjeProduktyKtorychZakupyLiczycWspolnie.Contains(x.ProduktId));

                var iloscRealnaJuzKupionaBezKoszyka = SolexBllCalosc.PobierzInstancje.Rabaty.WyliczIloscProduktow(koszyk.Klient, koszyk, produktKlienta, true);
                Dictionary<Konfekcje, decimal> slownikJakaIloscWJakiejCeniePojdzie = new Dictionary<Konfekcje, decimal>();

                decimal ileChceKupicObecnie = pozycjeProduktowObrabianych.Select(x=>x.IloscWJednostcePodstawowej).Sum(x => x);

                decimal sumaKwotyJakaPowinnaBycZaplacona = 0;

                for (int i =0; i< produktKlienta.GradacjePosortowane.Count;++i)
                {
                    decimal przedzialOd = produktKlienta.GradacjePosortowane[i].Ilosc;
                    if (i == 0)
                    {
                        //podmiana dla poziomu pierwszego - tu jest zazwyczaj minimalna ilosc sprzedazy, wolimy zero
                        przedzialOd = 0;
                    }

                    if (iloscRealnaJuzKupionaBezKoszyka > 0)
                    {
                        przedzialOd = przedzialOd + iloscRealnaJuzKupionaBezKoszyka;
                    }

                    decimal przedzialDo = int.MaxValue;

                    if (i < produktKlienta.GradacjePosortowane.Count - 1)
                    {
                        przedzialDo = produktKlienta.GradacjePosortowane[i + 1].Ilosc;
                    }

                    var przedzialKonfekcji = przedzialDo - przedzialOd;

                    //jesli jest mniej niz 0 to znaczy ze juz byly wczesniej kupione
                    if (przedzialKonfekcji < 0)
                    {
                        continue;
                    }

                    if (ileChceKupicObecnie < przedzialKonfekcji)
                    {
                        przedzialKonfekcji = ileChceKupicObecnie;
                    }

                    sumaKwotyJakaPowinnaBycZaplacona += przedzialKonfekcji*produktKlienta.GradacjePosortowane[i].PoliczCeneZGradacji(produktKlienta.FlatCeny.CenaNettoBezGradacji);
                  
                    slownikJakaIloscWJakiejCeniePojdzie.Add(produktKlienta.GradacjePosortowane[i], przedzialKonfekcji);
                    ileChceKupicObecnie = ileChceKupicObecnie - przedzialKonfekcji;

                    if (ileChceKupicObecnie <= 0)
                    {
                        break;
                    }
                }

                decimal cenaAktualnaSumarycznaPozycji = pozycjeProduktowObrabianych.Sum(x => x.Produkt.FlatCeny.CenaNetto*x.IloscWJednostcePodstawowej);

                WartoscLiczbowa koniecznaDoplataKlienta = new WartoscLiczbowa(sumaKwotyJakaPowinnaBycZaplacona - cenaAktualnaSumarycznaPozycji, produktKlienta.Klient.WalutaKlienta.WalutaB2b);

                if (koniecznaDoplataKlienta == 0)
                {
                    //nie ma nic do tego produktu
                    continue;
                }


                StringBuilder opisowaOdpowiedz = null;
                foreach (var s in slownikJakaIloscWJakiejCeniePojdzie)
                {
                    string rabatKwota = null;
                    rabatKwota = s.Key.PoliczCeneZGradacji(produktKlienta.FlatCeny.CenaNettoBezGradacji) + " " + produktKlienta.Klient.WalutaKlienta.WalutaB2b;

                    if (opisowaOdpowiedz == null)
                    {
                        opisowaOdpowiedz = new StringBuilder();
                    }
                    else
                    {
                        opisowaOdpowiedz.Append("<br/>");
                    }

                    opisowaOdpowiedz.AppendFormat("{0} {2} => {1}", s.Value.DoLadnejCyfry(), rabatKwota, produktKlienta.JednostkaPodstawowa.Nazwa);
                }

                //dodanie zwyżki
                ileChceKupicObecnie = pozycjeProduktowObrabianych.Select(x => x.IloscWJednostcePodstawowej).Sum(x => x); //jeszze raz ile chcemy kupić w sumie
                decimal doplataDoKazdejSztuki = koniecznaDoplataKlienta / ileChceKupicObecnie;

                foreach (var pozycja in pozycjeProduktowObrabianych)
                {
                    //pozycja.ZmienDodatkowyRabat(-(doplataDoKazdejSztuki*100)/pozycja.CenaNetto, $"Dzięki gradacji oszczędzasz <b>{-doplataDoKazdejSztuki * pozycja.IloscWJednostcePodstawowej} {pozycja.WartoscNetto.Waluta}</b> (wliczone już w cene)", TrybLiczeniaRabatuWKoszyku.NADPISZ);
                    decimal wartosc = -(doplataDoKazdejSztuki * 100) / pozycja.CenaNetto;
                    //Przyjmujemy na sztywno że jest to rabat (w tym module wartośc rabatu bedzie procentowa oraz teoretycznie bedzie to doplata gdyż cena będzie większa od prodgu gradacji gdyż konkretna cena bedzie dopiero dla produków które przekroczą próg)
                    pozycja.ZmienDodatkowyRabat(wartosc, "rabat", TrybLiczeniaRabatuWKoszyku.NADPISZ);
                    //    pozycja.ZmienDodatkowyRabat(-doplataDoKazdejSztuki * pozycja.IloscWJednostcePodstawowej, "rabat gradacji", TrybLiczeniaRabatuWKoszyku.NADPISZ);

                    pozycja.PowodDodatkowegoRabatu_DodatkoweInfoDymek = opisowaOdpowiedz.ToString();
                }
              //  WyslijWiadomosc(string.Format("Cena wynika z uśrednionych poziomów gradacji. {1}. Ceny produktów z gradacji: <br/>{0}", opisowaOdpowiedz, koniecznaDoplataKlienta), KomunikatRodzaj.warning);
            }

            return true;
        }
    }
}