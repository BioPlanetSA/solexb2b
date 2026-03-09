using System.Collections.Generic;

namespace SolEx.Hurt.Core.BLL.Interfejsy
{
    public interface ITestyKonfiguracji
    {
        List<KlasaOpakowanieTesty> WykonajTesty();

        List<KlasaOpakowanieTesty> WykonajTestyBazy();

        List<KlasaOpakowanieTesty> WykonajTestyKoszykowe();

        List<KlasaOpakowanieTesty> WykonajTestySkrzynekPocztowych();
    }
}