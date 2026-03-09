using System;
using ServiceStack.DataAnnotations;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Model.Interfaces
{
    public interface IAdres
    {
        bool MoznaEdytowac { get; set; }

        [FriendlyName("Miasto")]
        string Miasto { get; set; }

        [FriendlyName("Kraj")]
        int? KrajId { get; set; }
        //int KlientId { get; set; }
        string Kraj { get; }
        [PrimaryKey]
        [UpdateColumnKey]
        long Id { get; set; }

        //bool Glowny { get; set; }
        //bool Domyslny { get; set; }
        string Nazwa { get; set; }

        [FriendlyName("Kod pocztowy")]
        string KodPocztowy { get; set; }

        [FriendlyName("Ulica")]
        string UlicaNr { get; set; }

        //bool Jednorazowy { get; set; }
        string Telefon { get; set; }

        [FriendlyName("Województwo")]
        int? RegionId { get; set; }

        bool CzyPoprawneKoordynaty { get; }
        TypAdresu TypAdresu { get; set; }
        string KrajSymbol { get; set; }
        string Region { get; set; }
        string Email { get; set; }
        decimal Lat { get; set; }
        decimal Lon { get; set; }
        DateTime? DataDodania { get; set; }
        long? AutorId { get; set; }
    }
}