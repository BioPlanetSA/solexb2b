using System;
using ServiceStack.DataAnnotations;

namespace SolEx.Hurt.Model.Interfaces
{
    public interface IObrazek
    {
        [PrimaryKey]
        int Id { get; set; }

        string Nazwa { get; set; }
        string Sciezka { get; set; }
        DateTime Data { get; set; }

        [Ignore]
        string DanePlikBase64 { get; set; }

        [Ignore]
        string NazwaBezRozszerzenia { get; set; }

        string Rozszerzenie { get; }

        [Ignore]
        string SciezkaWzgledna { get; }

        [Ignore]
        string SciezkaBezwzgledna { get; }

        int Rozmiar { get; set; }

        [Ignore]
        string nazwaLokalna { get; set; }

        [Ignore]
        bool DoPobrania { get; set; }

        /// <summary>
        /// Propertis stworzony by przesyłać wybranego preseta razem ze zdjeciem
        /// </summary>
        [Ignore]
        string DomyslnyPreset { get; set; }

        string LinkOryginal { get; }

        string LinkWWersji(string i = null);

        bool CzyTeSamePliki(Plik y);

        void PoprawNazwaPlikuDlaURL();
    }
}