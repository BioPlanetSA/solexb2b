using System;
using ServiceStack.DataAnnotations;
using ServiceStack.DesignPatterns.Model;
namespace SolEx.Hurt.Model
{
    public interface ISklep : IHasLongId
    {
        string Nazwa { get; set; }

       
        bool Aktywny { get; set; }
       
        DateTime DataUtworzenia { get; set; }
       
        AdresUrl LinkUrl { get; set; }
        
        long? AutorId { get; set; }

        long? AdresId { get; set; }

        int? ObrazekId { get; set; }

       string Opis { get; set; }
       
        bool AutomatyczneKoordynaty { get; set; }

        bool KoordynatyZERP { get; set; }

        bool Siedziba { get; set; }
    }
}