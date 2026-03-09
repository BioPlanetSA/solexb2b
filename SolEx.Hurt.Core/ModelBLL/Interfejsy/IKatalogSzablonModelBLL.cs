using System.Collections.Generic;
using ServiceStack.DataAnnotations;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core.ModelBLL.Interfejsy
{
    public interface IKatalogSzablonModelBLL: IPoleJezyk
    {
        [PrimaryKey]
        [UpdateColumnKey()]
        [AutoIncrement]
        long Id { get; set; }

        string Nazwa { get; set; }
        string ParametrTekstowy1 { get; set; }
        string ParametrTekstowy2 { get; set; }
        string ParametrTekstowy3 { get; set; }
        string ParametrTekstowy4 { get; set; }
        string ParametrTekstowy5 { get; set; }
        string ParametrTekstowy6 { get; set; }
        string ParametrTekstowy7 { get; set; }
        int? Obrazek0Id { get; set; }
        int? Obrazek1Id { get; set; }
        int? Obrazek2Id { get; set; }
        int? Obrazek3Id { get; set; }
        int? Obrazek4Id { get; set; }
        int? Obrazek5Id { get; set; }

        int? PlikSzablonuId { get; set; }

        [Ignore]
        Plik PlikSzablonu { get; }

        bool Aktywny { get; set; }
        HashSet<KatalogFormatZapisu> DozwoloneFormatyWydruku { get; set; }
        HashSet<RoleType> DostepnyDla { get; set; }

        [Ignore]
        IObrazek Obrazek0 { get; }

        [Ignore]
        IObrazek Obrazek1 { get; }

        [Ignore]
        IObrazek Obrazek2 { get; }

        [Ignore]
        IObrazek Obrazek3 { get; }

        [Ignore]
        IObrazek Obrazek4 { get; }

        [Ignore]
        IObrazek Obrazek5 { get; }
    }
}