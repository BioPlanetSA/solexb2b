using System;
using ServiceStack.DataAnnotations;
using SolEx.Hurt.Core.BLL;
using System.Linq;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core.ModelBLL
{
    [Alias("Rabat")]
    public class RabatBLL:Rabat
    {
        public RabatBLL(Rabat bazowy): base(bazowy){}
        public RabatBLL(){}
        
        [FriendlyName("Dla kogo")]
        [Ignore]
        [WidoczneListaAdmin(true, true, false, false)]
        public string DlaKogo
        {
            get
            {
                if (KategoriaKlientowId.HasValue)
                {
                  //  return "Klienci z kategorii " + SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<KategoriaKlienta>(KategoriaKlientowId.Value, SolexBllCalosc.PobierzInstancje.Konfiguracja.JezykIDPolski).Nazwa;
                    return "Kategorie klientów";
                }

                if (KlientId.HasValue)
                {
                    //return "Klient " +SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<Klient>(KlientId.Value).Nazwa;
                    return "Wybranych klientów";
                }
                return "Wszyscy";
            }
        }

        [FriendlyName("Na co")]
        [Ignore]
        [WidoczneListaAdmin(true, true, false, false)]
        public string NaCo
        {
           get
           {
               if (KategoriaProduktowId.HasValue)
               {
                //    return string.Format("Kategoria {0}", SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<KategorieBLL>(KategoriaProduktowId.Value).Nazwa);                  
                   return "Kategorie produktów";
               }
               if (ProduktId.HasValue)
               {
                   //ProduktBazowy pr = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<ProduktBazowy>(ProduktId.Value,  SolexBllCalosc.PobierzInstancje.Konfiguracja.JezykIDPolski);
                   //return string.Format("Produkt {0}  [ {1} ]", pr.Nazwa, pr.Kod);
                    return "Wybrane produkty";
                }
               if (CechaId.HasValue)
               {
                   //return string.Format("Cecha {0}", SolexBllCalosc.PobierzInstancje.CechyAtrybuty.PobierzWszystkieCechy(SolexBllCalosc.PobierzInstancje.Konfiguracja.JezykIDPolski)[CechaId.Value].Nazwa);
                   return "Wybrane cechy";
               }
               return "Wszystkie produkty";
           }
        }

        //[Ignore]
        //public string NaCoBezNazw
        //{
        //    get
        //    {
        //        if (KategoriaProduktowId.HasValue)
        //        {
        //            return SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<KategorieBLL>(KategoriaProduktowId.Value).Nazwa.Trim();
        //        }
        //        if (ProduktId.HasValue)
        //        {
        //            return SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<ProduktBazowy>(ProduktId.Value).Nazwa.Trim();
        //        }
        //        if (CechaId.HasValue)
        //        {
        //            return SolexBllCalosc.PobierzInstancje.CechyAtrybuty.PobierzWszystkieCechy(SolexBllCalosc.PobierzInstancje.Konfiguracja.JezykIDPolski)[CechaId.Value].Nazwa.Trim();
        //        }
        //        return "Wszystkie produkty";
        //    }
        //}

        public decimal? PobierzWartoscRabatu(decimal cenaNetto)
        {
            if (SolexBllCalosc.PobierzInstancje.Konfiguracja.PrzedzialyCenowe == null)
            {
                return Wartosc1;
            }


            throw  new NotImplementedException("Trzeba zmienić implemetnacje dlatego że wartosci moga byc juz NULLami - wtedy bierzemy wartosc z poprzedniego poziomu");

            int cena = 1 +SolexBllCalosc.PobierzInstancje.Rabaty.PobierzPrzedzialyCenowe().TakeWhile(przedzial => przedzial > 0).TakeWhile(przedzial => cenaNetto >= przedzial).Count();
            switch (cena)
            {
                case 1:
                    return Wartosc1;
                case 2:
                    return Wartosc2;
                case 3:
                    return Wartosc3;
                case 4:
                    return Wartosc4;
                case 5:
                    return Wartosc5;
                default:
                    return 0;
            }
        }


             
    }
}
