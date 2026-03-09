using System.Collections.Generic;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using SposobPokazywaniaStanow = SolEx.Hurt.Core.ModelBLL.SposobPokazywaniaStanow;

namespace SolEx.Hurt.Core.BLL.Interfejsy
{
    public interface ISposobyPokazywaniaStanowBLL
    {
        bool SaStany(int idKlienta, PozycjaLista? pozycjaLista, PozycjaKarta? pozycjaKart,int? idPrzedstawiciela);
        SposobPokazywaniaStanow Get(int IdSposobu,int jezyk);
        Dictionary<int, SposobPokazywaniaStanow> WszystkieSposoby(int jezyk);
        //sposoby_pokazywania_stanow_reguly Save(Model.sposoby_pokazywania_stanow_reguly data);
        void RegulaUsun(int regulaID, int idSposobu);
        void WyczyscCache();
        void WyczyscCache(List<ProduktStan> lista );
        RegulyPokazywaniaStanow.BaseRegulyPokazywaniaStanow PobierzSilnikReguly(Model.SposobPokazywaniaStanowRegula regula);
        int? DomyslnySposobPokazywaniaNumer { get; }
        string PokazStanProduktuWgSposobu(int idSposobu, int produktId, int magazynId, int langId);
        void ZmienKolejnoscReguly(int idReguly, int nowaWartosc);
        Dictionary<int, SposobPokazywaniaStanow> WszystkieSposobyKlienta(IKlient klient, int lang,
            PozycjaLista? pozycjaLista, PozycjaKarta? pozycjaKarta, int? idPRzedstawiciela);
        List<SposobPokazywaniaStanowRegula> PobierzReguly(int idSposobu);
        void UstawParametryReguly(IList<SposobPokazywaniaStanowRegula> obj);
        SposobPokazywaniaStanowRegula PobierzRegule(int id);

        
    }
}