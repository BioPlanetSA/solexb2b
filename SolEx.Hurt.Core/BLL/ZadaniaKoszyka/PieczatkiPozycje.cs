using System;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using System.Linq;

namespace SolEx.Hurt.Core.BLL.ZadaniaKoszyka
{
    public class PieczatkiPozycje : ZadaniePozycjiKoszyka, IModulStartowy, IFinalizacjaKoszyka
    {
        public override bool Wykonaj(IKoszykPozycja pozycja, IKoszykiBLL koszyk)
        {
            throw new NotImplementedException();
            //var json = pozycja.IndywidalneParametry.FirstOrDefault(x => x.Key == "json");
            //var pdf = pozycja.IndywidalneParametry.FirstOrDefault(x => x.Key == "pdf");

            //var png64Base = pozycja.IndywidalneParametry.FirstOrDefault(x => x.Key == "png").Value;
            //pozycja.Produkt().ZdjecieGlowne = new Obrazek(null);
            //pozycja.Produkt().ZdjecieGlowne.DanePlikBase64 = png64Base;
            //pozycja.Produkt().ZdjecieGlowne.Id = pozycja.Id;
            //pozycja.PieczatkaGrafika = png64Base;

            //return true;
        }
    }
}