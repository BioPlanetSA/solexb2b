using System.Collections.Generic;
using System.Linq;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core.ModelBLL.ObiektyMaili
{
    public class PojawienieSieProduktow : SzablonMailaBaza
    {
        public PojawienieSieProduktow() : base(null)
        {
            
        }
        public PojawienieSieProduktow(IList<IProduktKlienta> listaProduktow, IKlient klient): base(klient)
        {
            ListaProduktow = listaProduktow;
        }

        public override string NazwaFormatu()
        {
            return "Pojawienie się produktów na które oczekiwał klient";
        }

        public IList<IProduktKlienta> ListaProduktow { get; set; }

        public override string OpisFormatu()
        {
            return "Mail informujący o pojawieniu się produktów na które oczekiwał klient. Powiadomienia wysyłane przez moduł 'Wyślij powiadomienia o pojawieniu się produktów na stanie' lub " +
                   "przez API: Api/produkty.stany.powiadomienie.ashx";
        }
        public override string OpisDlaKlienta()
        {
            return "Mail informujący o pojawieniu się produktów na które oczekiwał klient.";
        }
        public override TypyPowiadomienia[] PowiadomieniaDomyslnieAktywne
        {
            get { return null; }
        }
    }
}