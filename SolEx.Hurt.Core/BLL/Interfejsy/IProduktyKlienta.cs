using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using System;
using System.Collections.Generic;
using SolEx.Hurt.Core.BLL.WirtualneProduktyProvidery;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core.BLL.Interfejsy
{
    public interface IProduktyKlienta
    {
        //Dictionary<long, bool> PobierzZamiennikiWidoczneDlaKlienta(ProduktKlienta pk, IKlient klient, HashSet<long> IdProduktowRzeczywiscieDostepnychDlaKlienta);

        Dictionary<IProduktKlienta, KategorieBLL> WybierzProduktyDoPokazaniaWgStronyISortowania(long? kategorie, IList<ProduktKlienta> pasujaceprodukty, 
            IKlient customer, int jezykId, List<SortowaniePole> sortowanie,
            bool wylaczGrupowanie, int pominacIle, int ilePobrac, out int lacznie, out HashSet<long> wszystkieidsrodzinowe);

        HashSet<long> PobierzIdProduktowRzeczywiscieDostepnychDlaKlienta(IKlient klient);
        HashSet<long> PobierzIdProduktowRzeczywiscieDostepnychDlaKlienta(IKlient klient, out IList<ProduktKlienta> produktyWirtualne, bool zwracajTylkoIdProduktowBazowych = false);
        //HashSet<long> PobierzIdPRoduktowWRodzinieDostepnychDlaKlienta(IProduktKlienta pk);

        void WyczyscCacheProduktyKlienta(IKlient klient = null);

        IList<ProduktKlienta> ProduktySpelniajaceKryteria(long? katIDs, string wyszukiwane, IKlient klient, int lang, Dictionary<int, HashSet<long>> filtry,
            Dictionary<int, HashSet<long>> staleFiltyIds, string wyszukiwanieWewnatrzKategorii, IList<long> idProduktow = null);

        IList<ProduktKlienta> PobierzProduktyKlientaZCache(int jezykID, IKlient klientID);
    }
}