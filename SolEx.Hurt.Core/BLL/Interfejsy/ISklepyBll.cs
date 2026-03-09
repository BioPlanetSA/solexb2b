using ServiceStack.DataAnnotations;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model;
using System.Collections.Generic;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core.BLL.Interfejsy
{
    public interface ISklepyBll : ISklep
    {
        [Ignore]
        IAdres Adres { get; }

        [Ignore]
        string KodPocztowy { get; set; }

        [Ignore]
        string UlicaNr { get; set; }

        [Ignore]
        string Miasto { get; set; }

        [Ignore]
        string Telefon { get; set; }

        [Ignore]
        string Email { get; set; }

        [Ignore]
        string Autor { get; }

        [Ignore]
        IObrazek Obrazek { get; }

        IList<KategoriaSklepu> PobierzKategorie(int jezykId);
    }
}