using System;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces.Modele;

namespace SolEx.Hurt.Model.Helpers
{
    public static class Kwoty
    {
        public static decimal WyliczWartosc(decimal cena, decimal rabat, RabatSposob typWartosc)
        {
            decimal netto = 0;


            switch (typWartosc)
            {
                case RabatSposob.MinusCena:
                    netto = cena - rabat;
                    break;
                case RabatSposob.StalaCena:
                    netto = rabat;
                    break;
                case RabatSposob.Procentowy:
                    netto = Kwoty.WyliczCenePoRabacie(cena, rabat);
                    break;
            }
            return netto;
        }
        public static decimal WyliczRabatDlaUzyskaniaCeny(decimal cenaBazowa, decimal cenaDocelowa)
        {
            if (cenaBazowa == 0 || cenaBazowa==cenaDocelowa)
            {
                return 0;
            }
            decimal rabat = (100M*cenaDocelowa)/cenaBazowa;
            return 100M - rabat;
        }
        public static decimal WyliczCenePoRabacie(decimal cenaBazowa, decimal rabat)
        {
            if (cenaBazowa == 0 )
            {
                return 0;
            }
           return  cenaBazowa * (100M - rabat) / 100M;
        }
        public static decimal WyliczBrutto(decimal netto, decimal vat, IKlienci k)
        {
            decimal stawka = k.IndywidualnaStawaVat.GetValueOrDefault(vat);
            return Decimal.Round(netto * (100M + stawka) / 100M, 4); 
        }
        public static decimal WyliczBrutto(decimal netto, decimal vat)
        {
            return Decimal.Round(netto * (100M + vat) / 100M, 4);
        }
        public  static decimal WyliczRabat(decimal przed, decimal po,int zaokralanie=0)
        {
            decimal wyliczonyRabat = 0;
            if (przed > 0)
            {
                wyliczonyRabat = 100M - (100M * po / przed);
                wyliczonyRabat = wyliczonyRabat < 0 ? 0 : wyliczonyRabat;
            }
            return Decimal.Round(wyliczonyRabat,zaokralanie);
        }
    }
}
