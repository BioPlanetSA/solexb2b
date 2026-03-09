using ServiceStack.DataAnnotations;
using ServiceStack.DesignPatterns.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Core;

namespace SolEx.Hurt.Model
{
    public interface IFlatCeny:IHasLongId
    {
        [Ignore]
        WartoscLiczbowa CenaNettoPrzedPromocja { get; set; }

        long KlientId { get; set; }

        [FriendlyName("Numer id produkt")]
        long ProduktId { get; set; }

        [FriendlyName("cena netto po rabacie")]
        [RealSortColumnName("FlatCeny.cena_netto")]
        decimal CenaNetto { get; set; }

        [RealSortColumnName("FlatCeny.cena_hurtowa_netto")]
        [FriendlyName("cena netto przed rabatem")]
        decimal CenaHurtowaNetto { get; set; }

        int TypRabatu { get; set; }

        [FriendlyName("waluta")]
        long WalutaId { get; set; }

        [FriendlyName("rabat")]
        [RealSortColumnName("FlatCeny.rabat")]
        decimal Rabat { get; set; }

        decimal CenaNettoDokladna { get; set; }
        decimal PrzeliczenieWaluty_Kurs { get; set; }
        decimal PrzeliczenieWaluty_CenaNettoBazowa { get; set; }
        long PrzeliczenieWaluty_WalutaIdBazowa { get; set; }
        decimal CenaNettoBezGradacji { get; set; }
    }
}