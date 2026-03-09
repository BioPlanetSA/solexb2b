using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.Web;
using System.Collections.Generic;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core.BLL.Interfejsy
{
    public interface IKategorieDostep
    {
        void Usun(IEnumerable<long> ids);

        List<KategorieBLL> PobierzDzieci(KategorieBLL kategorie);

        HashSet<long> PobierzWszystkieKategorie(HashSet<long> katIDs, bool wszystkiepoziomy);

        IList<KategorieBLL> PobierzDrzewkoKategorii(long idGrupy, int jezykId, IKlient klient, HashSet<long> rozpartywanePRodukty = null);
       // bool JestWidocznaDlaKlienta(KategorieBLL kategoria, IKlient klient, bool dlaRabatu, HashSet<int> rozpartywanePRodukty);

        List<IProduktKlienta> PobierzProdukty(KategorieBLL kategoria, IKlient k = null, Dictionary<int, HashSet<long>> stale = null);

        KategorieBLL KategoriaNadrzedna(KategorieBLL kategoria);

        //List<KategorieBLL> PobierzKategorieZGrupy(long grupa, int jezyk, IKlient klient, bool tylkorabaty, HashSet<int> rozpatrywane);

        //List<KategorieBLL> PobierzKategorieZGrupy(long grupa, int jezyk);

       // bool RenderowacDzieci(long sprawdzana, IList<KategorieBLL> wszystkie, HashSet<long> wybrane, out List<KategorieBLL> dzieci);

      //  int LiczbaProduktow(KategorieBLL kategoria, IKlient k, Dictionary<int, HashSet<long>> stale, string szukane, int jezyk);

        KategorieBLL CzyNaPodstwieCechy(CechyBll cecha);
        void AktualizujKategorie(IList<KategoriaProduktu> list);

        bool JestWidocznaDlaKlienta(KategorieBLL kategoria, IKlient klient, HashSet<long> rozpartywanePRodukty);
        bool JestWidocznaDlaKlienta(KategorieBLL kategoria, IKlient klient);
        bool JestWidocznaDlaKlienta(GrupaBLL grupa, IKlient klient);

        GrupaBLL PobierzIdGrupyKomplementarnej(KategorieBLL kategoria, IKlient klient, int lang);
        void UsunCache(IList<object> obj);
        IList<KategorieBLL> MetodaPrzetwarzajacaPoSelect(int i, IKlient klient, IList<KategorieBLL> arg3, object arg4);
        Dictionary<long, KategorieBLL> PobierzKategorieDostepneDlaKlienta(IKlient kienta);
    }
}