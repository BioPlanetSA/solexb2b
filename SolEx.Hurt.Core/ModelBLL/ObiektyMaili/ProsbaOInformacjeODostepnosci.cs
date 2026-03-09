using System.Collections.Generic;
using System.Linq;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core.ModelBLL.ObiektyMaili
{
    public class ProsbaOInformacjeODostepnosci : SzablonMailaBaza
    {
        public ProsbaOInformacjeODostepnosci(): base(null){}
        public IProduktKlienta Produkt { get; set; }
        public ProsbaOInformacjeODostepnosci(IProduktKlienta produkt, IKlient klient) : base(klient)
        {
            Produkt = produkt;
        }
        public override string NazwaFormatu()
        {
            return "Prośba o informacje o dostępności produktów";
        }
         
        public override string OpisFormatu()
        {
            return "Mail z prośbą klienta o informacje kiedy towar będzie dostępny. Powiadomienie wysyłane natychmiast po kliknięciu przez klienta w przycisk 'powiadom o dostępności' " +
                   "(odpowiedz o pojawieniu się produktu na stanie jest wysyłana automatycznie przez powiadomienie mailowe " +
                   "'Pojawienie się produktów na które oczekiwałeś') ";
        }
        public override string OpisDlaKlienta()
        {
            return "Mail z prośbą klienta o informacje kiedy towar będzie dostępny.";
        }
        public override TypyPowiadomienia[] ObslugiwaneRodzajePowiadomien
        {
            get { return new[] { TypyPowiadomienia.Opiekun, TypyPowiadomienia.Przedstawiciel, TypyPowiadomienia.DrugiOpiekun,  };  }
        }

        public override TypyPowiadomienia[] PowiadomieniaDomyslnieAktywne
        {
            get { return null; }
        }
    }
}
