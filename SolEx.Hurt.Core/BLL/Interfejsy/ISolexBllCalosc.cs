using log4net;
using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Core.DostepDane;

namespace SolEx.Hurt.Core.BLL
{
    public interface ISolexBllCalosc
    {
        IDokumentyDostep DokumentyDostep { get; }
        IMaileBLL MaileBLL { get; }
        IPunktyDostep PunktyDostep { get; }
        IProduktyJednostkiDostep ProduktyJednostkiDostep { get; }
        IProduktyKategorieDostep ProduktyKategorieDostep { get; }
        IAdresyDostep AdresyDostep { get; }
        IRegionyDostep RegionyDostep { get; }
        ICechyProduktyDostep CechyProduktyDostep { get; }
        IKlienciDostep Klienci { get; }
        IProfilKlientaBll ProfilKlienta { get; }
        IStatystyki Statystyki { get; }
        IPoziomyCenBll CenyPoziomy { get; }
        IProduktyBazowe ProduktyBazowe { get; }
        IPliki Pliki { get; }
        IProduktyStanBll ProduktyStanBll { get; }
        IZadaniaBLL ZadaniaBLL { get; }
        ISklepy Sklepy { get; }
        IRabatyBll Rabaty { get; }
        IConfigBLL Konfiguracja { get; }
        ILog Log { get; }
        IProduktyKlienta ProduktyKlienta { get; }
        ICechyAtrybuty CechyAtrybuty { get; }
        IListyPrzewozoweBll ListyPrzewozoweBll { get; }
        IKategorieDostep KategorieDostep { get; }
        IZamowieniaDostep ZamowieniaDostep { get; }
        IDaneDostep DostepDane { get; }
        SposobyPokazywaniaStanowBLL SposobyPokazywaniaStanowBll { get; }
        ICacheBll Cache { get; }
        IWidocznosciTypowBLL WidocznosciTypowBll { get; }

        //   IAtrybutyDostep AtrybutyDostep { get; }
        ITresciDostep TresciDostep { get; }

        IKoszykiDostep Koszyk { get; }

        ProduktyUkryteBll ProduktyUkryteBll { get; }

        KomunikatyBll KomunikatyBll { get; }
        Szukanie Szukanie { get; }
        KupowaneIlosciBLL KupowaneIlosciBLL { get; }
    }
}