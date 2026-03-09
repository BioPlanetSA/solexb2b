using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Pliki
{
    [FriendlyName("Pliki do produktów z plików", FriendlyOpis = "Przeszukuje określony katalog i dopasowuje pliki jako załączniki do produktów")]
    public class PlikiDoProduktowZPlikow : ZdjeciaDoProduktowZPlikow
    {
        public override bool ZweryfikujTypPliku(Plik plik)
        {
            return plik.RodzajPliku ==RodzajPliku.Zalacznik;
        }
        protected override void PRzypiszTypPliku(Plik plik)
        {
            plik.RodzajPliku = RodzajPliku.Zalacznik;
        }
    }
}
