using System.Collections.Generic;

namespace SolEx.Hurt.Core.BLL.Interfejsy
{
    public interface IWysylanieWiadomosciEmail
    {
        //WiadomoscEmail GenerujPodlad(long id, int jezyk, TypyPowiadomienia dokogogo);
        void InicjalizacjaPowiadomien();

        string ParsujKolekcjeProduktow(IList<ProduktKlienta> produkty, string szablon);
    }
}