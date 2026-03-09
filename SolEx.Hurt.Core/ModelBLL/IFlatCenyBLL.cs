using System.Collections.Generic;
using ServiceStack.DataAnnotations;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Core;

namespace SolEx.Hurt.Core
{
    public interface IFlatCenyBLL :IFlatCeny
    {
      
        [RealSortColumnName("FlatCeny.cena_brutto")]
        [FriendlyName("cena brutto po rabacie")]
        WartoscLiczbowa CenaBrutto { get; }


        [FriendlyName("cena brutto przed rabatem")]
        [RealSortColumnName("FlatCeny.cena_hurtowa_brutto")]
        WartoscLiczbowa CenaHurtowaBrutto { get; }

        [FriendlyName("zysk klienta netto")]
        [RealSortColumnName("FlatCeny.zysk_klienta_netto")]
        WartoscLiczbowa ZyskKlientaNetto { get; }

        [FriendlyName("cena detaliczna netto")]
        [RealSortColumnName("FlatCeny.cena_detaliczna_netto")]
        WartoscLiczbowa CenaDetalicznaNetto { get; set; }

        [FriendlyName("cena detaliczna brutto")]
        [RealSortColumnName("FlatCeny.cena_detaliczna_brutto")]
        WartoscLiczbowa CenaDetalicznaBrutto { get; }

        WartoscLiczbowa CenaBruttoPrzedPromocja { get; }


        [Ignore]
        WartoscLiczbowa CenaNettoPrzedPromocja { get; set; }


        [FriendlyName("Numer id produkt")]
        long ProduktId { get; set; }

        [FriendlyName("cena netto po rabacie")]
        [RealSortColumnName("FlatCeny.cena_netto")]
        WartoscLiczbowa CenaNetto { get; set; }

        [RealSortColumnName("FlatCeny.cena_hurtowa_netto")]
        [FriendlyName("cena netto przed rabatem")]
        WartoscLiczbowa CenaHurtowaNetto { get; set; }

        int TypRabatu { get; set; }

        [FriendlyName("waluta")]
        long WalutaId { get; set; }

        [FriendlyName("rabat")]
        [RealSortColumnName("FlatCeny.rabat")]
        decimal Rabat { get; set; }

        WartoscLiczbowa CenaBruttoPrzedPrzewalutowaniem { get; }
        List<GradacjaWidok> GradacjaUzytaDoLiczeniaCeny_Poziomy { get; set; }
        decimal GradacjaUzytaDoLiczeniaCeny_KupioneIlosci { get; set; }
    }
}