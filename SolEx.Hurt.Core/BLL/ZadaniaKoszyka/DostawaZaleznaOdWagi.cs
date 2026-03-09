using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.AtrybutyKlas;
using System;

namespace SolEx.Hurt.Core.BLL.ZadaniaKoszyka
{
    [FriendlyName("Dostawa zależna od Wagi", FriendlyOpis = "Za każde X kg ustaw cenę y")]
    public class DostawaZaleznaOdWagi : DostawaBaza
    {
        [FriendlyName("Waga jednostkowa x")]
        public decimal WagaPaczki { get; set; }

        [FriendlyName("Cena jednostkowa y")]
        public decimal CenaPaczki { get; set; }

        public override decimal WyliczCene(IKoszykiBLL koszyk)
        {
            if (WagaPaczki == 0) throw new Exception("Waga paczki musi być większa od 0");
            decimal liczbaPaczek = koszyk.WagaCalokowita() / WagaPaczki;
            liczbaPaczek = decimal.Ceiling(liczbaPaczek);
            return liczbaPaczek * CenaPaczki;
        }
    }
}