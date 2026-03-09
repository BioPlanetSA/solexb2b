using ServiceStack.DataAnnotations;
using ServiceStack.DesignPatterns.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Core;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Model
{
    /// <summary>
    /// Reprezentuje ceny klienta
    /// </summary>
    public class FlatCeny: IFlatCeny
    {
        [PrimaryKey]
        public long Id => FlatCeny.WyliczId(KlientId, ProduktId);

        public static long WyliczId(long klientId, long produktId)
        {
            return (klientId + "_" + produktId).WygenerujIDObiektuSHAWersjaLong(1);
        }

        public FlatCeny()
        {
        }

        public FlatCeny(long klientId, long towarId,decimal cenaHurtowaNetto1, decimal rabat1, long waluta,decimal dokladnaCenaNetto) : base()
        {
            this.KlientId = klientId;
            ProduktId = towarId;
            CenaNetto = decimal.Round(dokladnaCenaNetto, 2);
            CenaHurtowaNetto = cenaHurtowaNetto1;
            Rabat = rabat1;
            WalutaId = waluta;
            CenaNettoDokladna = dokladnaCenaNetto;
        }

        public FlatCeny(long klient, decimal rabat, CenaPoziomu poziopm, decimal dokladnaCenaNetto):base()
        {
            KlientId = klient;
            ProduktId = poziopm.ProduktId;
            CenaNetto = decimal.Round(dokladnaCenaNetto, 2);
            CenaHurtowaNetto = poziopm.Netto;
            this.Rabat = rabat;
            WalutaId = poziopm.WalutaId.Value;
            CenaNettoDokladna = dokladnaCenaNetto;
        }

        public FlatCeny(IFlatCeny baza) : base()
        {
            CenaNettoPrzedPromocja = baza.CenaNettoPrzedPromocja;
            KlientId = baza.KlientId;
            ProduktId = baza.ProduktId;
            CenaNetto = baza.CenaNetto;
            CenaHurtowaNetto = baza.CenaHurtowaNetto;
            TypRabatu = baza.TypRabatu;
            WalutaId = baza.WalutaId;
            Rabat = baza.Rabat;
            CenaNettoDokladna = baza.CenaNettoDokladna;

            PrzeliczenieWaluty_Kurs = baza.PrzeliczenieWaluty_Kurs;
            PrzeliczenieWaluty_CenaNettoBazowa = baza.PrzeliczenieWaluty_CenaNettoBazowa;
            PrzeliczenieWaluty_WalutaIdBazowa = baza.PrzeliczenieWaluty_WalutaIdBazowa;
            this.CenaNettoBezGradacji = baza.CenaNettoBezGradacji;
        }

        [Ignore]
        public WartoscLiczbowa CenaNettoPrzedPromocja { get; set; }
        public long KlientId { get; set; }
        [FriendlyName("Numer id produkt")]
        public long ProduktId { get; set; }

        [FriendlyName("cena netto po rabacie")]
        [RealSortColumnName("FlatCeny.cena_netto")]
        public decimal CenaNetto { get; set; }
        
        [RealSortColumnName("FlatCeny.cena_hurtowa_netto")]
        [FriendlyName("cena netto przed rabatem")]
        public decimal CenaHurtowaNetto { get; set; }
         
        public int TypRabatu { get; set; }
          
        [FriendlyName("waluta")]
        public long WalutaId { get; set; }

        public decimal CenaNettoDokladna { get; set; }

        [FriendlyName("rabat")]
        [RealSortColumnName("FlatCeny.rabat")]
        public decimal Rabat { get; set; }

        [Ignore]
        public decimal PrzeliczenieWaluty_Kurs { get; set; }

        //todo: zmiana dwoch pozniej proeprtisow na WartoscLiczbowa
        [Ignore]
        public decimal PrzeliczenieWaluty_CenaNettoBazowa { get; set; }

        [Ignore]
        public long PrzeliczenieWaluty_WalutaIdBazowa { get; set; }

        [Ignore]
        public decimal CenaNettoBezGradacji { get; set; }
    }
}
