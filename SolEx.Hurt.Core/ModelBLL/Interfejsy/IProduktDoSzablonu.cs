using System.Collections.Generic;
using ServiceStack.DataAnnotations;
using SolEx.Hurt.Model.Core;

namespace SolEx.Hurt.Core.ModelBLL.Interfejsy
{
    public interface IProduktDoSzablonu : IProduktKlienta
    {
        [Ignore]
        decimal Ilosc { get; }

        [Ignore]
        string Jednostka { get; }

        [Ignore]
        WartoscLiczbowa WartoscNetto { get; }

        [Ignore]
        WartoscLiczbowa WartoscBrutto { get; }

        [Ignore]
        string ZamawianaIloscTekst { get; }
    }
}