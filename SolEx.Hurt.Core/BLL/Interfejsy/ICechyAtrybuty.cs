using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Web;
using System;
using System.Collections.Generic;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core.BLL.Interfejsy
{
    public interface ICechyAtrybuty
    {
        List<AtrybutBll> PobierzWyfiltrowane(Dictionary<int, HashSet<long>> wybrane, long? kategorie, Dictionary<int, HashSet<long>> stale, string szukane, IKlient klient, int jezyk, string szukanieWKategorii);

        /// <summary>
        /// Buduje slownik filtrow na podstawie ciagu znakowego Key-id atrybutu, Value - Hashset z numerami id cech
        /// </summary>
        /// <param name="filtry"></param>
        /// <returns></returns>
        Dictionary<int, HashSet<long>> SlownikFiltrow(string filtry);
        
       Dictionary<long, CechyBll> PobierzWszystkieCechy(int lang);

        CechyBll PobierzCecheOSymbolu(string symbol, int lang);

        HashSet<CechyBll> PobierzCechyOId(HashSet<long> idCech, int langId);

        List<CechyBll> PobierzCecheDlaAtrybutu(int atrybutId, int lang);

        List<Cecha> AktualizujLubZapiszCechy(List<Cecha> data);

        void GenerujStandardoweCechy();

        List<CechyBll> PobierzMetkiLista(MetkaPozycjaLista pozycja, HashSet<long> idCech, int jezykId, bool zalogowany);

        List<CechyBll> PobierzMetkaPozycjaKoszykGratisy(MetkaPozycjaKoszykGratisy pozycja, HashSet<long> idCech, int jezykId, bool zalogowany);

        List<CechyBll> PobierzMetkaPozycjaKoszykGratisyPopUp(MetkaPozycjaKoszykGratisyPopUp pozycja, HashSet<long> idCech, int jezykId, bool zalogowany);

        List<CechyBll> PobierzMetkaPozycjaKoszykAutomatyczne(MetkaPozycjaKoszykAutomatyczne pozycja, HashSet<long> idCech, int jezykId, bool zalogowany);

        List<CechyBll> PobierzMetkiRodzina(MetkaPozycjaRodziny pozycja, HashSet<long> idCech, int jezykId, bool zalogowany);

        List<CechyBll> PobierzMetkiSzczegoly(MetkaPozycjaSzczegoly pozycja, HashSet<long> idCech, int jezykId, bool zalogowany);

        List<CechyBll> PobierzMetkiSzczegolyWarianty(MetkaPozycjaSzczegolyWarianty pozycja, HashSet<long> idCech, int jezykId, bool zalogowany);
        List<CechyBll> PobierzMetkiKafleProduktu(MetkaPozycjaKafle pozycja, HashSet<long> idCech, int jezykId, bool zalogowany);
        List<CechyBll> PobierzMetkaPozycjaKoszykProdukty(MetkaPozycjaKoszykProdukty pozycja, HashSet<long> idCech, int jezykId, bool zalogowany);

        void PrzedAktualizacjaAtrybutow(IList<AtrybutBll> obj);
        string PobierzPrzyjaznaNazweDlaTypuStanu(TypStanu typ);
        void UsunCacheAtrybutyICechy(IList<object> obj);

        List<AtrybutBll> PobierzAtrybutyListyProduktow(IList<ProduktKlienta> wybraneProdukty, int jezykId, IKlient klient, string kluczCacheListyProduktow);
        IList<AtrybutBll> BindingAtrybutyPoSelect(int jezykID, IKlient zajadacyKlient, IList<AtrybutBll> listaAtrybutow, object daneDoSelecta);
        void UstawSlownikiMetek();

        IList<RodzinyCechyUnikalne> BindPoSelectCechUnikatowych(int jezykID, IKlient zajadacyKlient, IList<RodzinyCechyUnikalne> listaCechUnikatowych, object daneDoSelecta);
        string ZbudujKluczCacheDlaFiltrowListyProduktow(long? wybraneKategorie, IKlient klient, string szukaneGlobalne, string szukanieWKategorii, Dictionary<int, HashSet<long>> staleFiltryWgAtrybutow, Dictionary<int, HashSet<long>> wybraneJuzWczesniejCechyWgAtrybutow, int iloscProduktow);
        Dictionary<int, long[]> SlownikIdAtrybutowIIdCech { get; }
    }
}