using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using System.Collections.Generic;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core.BLL.Interfejsy
{
    public interface IRabatyBll
    {
        decimal WyliczIloscProduktow(IKlient klient, IKoszykiBLL koszyk, IProduktKlienta produkt, bool wymuszajPomijanieAktualnegoKoszyka = false);

        List<GradacjaWidok> WyliczoneGradacje(IProduktKlienta produkt, IKlient klient, IFlatCeny flatCenaDlaProduktu, out decimal zakupionaDotychczasIlosc);

        List<decimal> PobierzPrzedzialyCenowe();


        RabatBLL Znajdz(long produktId, HashSet<long> cechyid, IKlient klient, IEnumerable<int> kategorieklienta, List<RabatTyp> rt, long? waluta);

        //    Dictionary<RabatTyp,long[]> ZnajdzWszystkieKlienta(IEnumerable<int> kategorieKlienta, int klientId);
        IFlatCenyBLL PobierzCeneProduktuDlaKlienta(IProduktKlienta produktKlienta);


        void UsunCache(IList<object> obj);
        IDictionary<long, RabatBLL> SlownikRabatow();
        //decimal WyliczIloscProduktow(IKlient klient, HashSet<long> idProduktow, bool wymuszajPomijanieAktualnegoKoszyka = false);

        /// <summary>
        /// liczenie ceny TYLKO dla zalogowanych klientów
        /// </summary>
        /// <param name="klient"></param>
        /// <param name="produkt"></param>
        /// <returns></returns>
        FlatCenyBLL WyliczCeneDlaKlientaZalogowanegoZGradacja(IKlient klient, IProduktKlienta produkt);

        FlatCenyBLL WyliczCeneDlaKlientaZalogowanego(IKlient klient, IProduktKlienta produkt);
        void WyczyscGradacjePoZmianieKoszyka(IKlient klient);
        void WyczyscCacheFlatCenKlienta(long idKlienta);
    }
}