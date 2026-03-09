using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Core.BLL.Testy;
using SolEx.Hurt.Core.DostepDane;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Core.ModelBLL.ObiektyMaili;

namespace SolEx.Hurt.Core.BLL
{
    public class SolexBllCalosc : BllBaza<SolexBllCalosc>, ISolexBllCalosc
    {
        private readonly CechyProduktyDostep _cechyProduktyDostep;
        private readonly KlienciDostep _klienci;
        private readonly Statystyki _statystyki;
        private readonly PoziomyCenBll _cenyPoziomy;
        private readonly ProduktyBazowe _produktyBazowe;
        private readonly PlikiBLL _pliki;
        private readonly ProduktyStanBll _produktyStanBll;
        private readonly ZadaniaBLL _zadaniaBLL;
        private readonly Sklepy _sklepy;
        private readonly RabatyBll _rabaty;
        private readonly ProduktyKlienta _produktyKlienta;
        private readonly CechyAtrybuty _cechyAtrybuty;
        private readonly MaileBLL _maileBLL;
        private readonly KategorieDostep _kategorieDostep;
        private readonly ProfilKlientaDostep _profilKlientaBll;
        private readonly RegionyDostep _regionyDostep;
        private readonly ZamowieniaDostep _zamowienia;
        private readonly AdresyBLL _adresyDostep;
        private readonly ProduktyKategorieDostep _produktyKategorieDostep;
        private readonly ProduktyJednostkiDostep _produktyJednostkiDostep;
        private readonly DostepDoDanych _dal;
        private readonly CacheBll _cache;
        private readonly ConfigBLL _konfiguracja;
        private readonly PunktyDostep _punktyDostep;
        private readonly WidocznosciTypowBLL _widocznosciTypowBll;
        private readonly BlogDostep _blog;
        private readonly ListyPrzewozoweBll _listyPrzewozoweBll;
        private readonly DokumentyDostep _dokumentyDostep;

        private readonly JednostkaProduktuBll _jednostkaProduktuBll;
        private readonly TresciDostep _tresciDostep;
        private readonly KoszykiDostep _koszyk;
        private readonly PieczatkiBll _pieczatki;
        private readonly SposobyPokazywaniaStanowBLL _sposobyPokazywaniaStanowBll;
        private readonly TestyKonfiguracji _testyKonfiguracji;
        private readonly ProduktyUkryteBll _produktyUkryteBll;
        private readonly KomunikatyBll _komunikatyBll;
        private readonly Szukanie _szukanie;
        private readonly KupowaneIlosciBLL _kupowaneIlosciBll;
        private readonly MailNoweProdukty _mailNoweProdukty;
        private readonly MailProduktyPrzyjeteNaMagazyn _mailProduktyPrzyjeteNaMagazyn;

        public SolexBllCalosc()
        {
            _konfiguracja = new ConfigBLL(this, new SqlSettingProvider());
            _cache = new CacheBll(this);
            _dal = new DostepDoDanych(this);

            _cechyProduktyDostep = new CechyProduktyDostep(this);
            _klienci = new KlienciDostep(this);
            _blog = new BlogDostep(this);
            _koszyk = new KoszykiDostep(this);
            _statystyki = new Statystyki(this);
            _cenyPoziomy = new PoziomyCenBll(this);
            _pliki = new PlikiBLL(this);
            _produktyStanBll = new ProduktyStanBll(this);
            _zadaniaBLL = new ZadaniaBLL(this);
            _sklepy = new Sklepy(this);
            _rabaty = new RabatyBll(this);
            _produktyBazowe = new ProduktyBazowe(this);
            _produktyKlienta = new ProduktyKlienta(this);
            _cechyAtrybuty = new CechyAtrybuty(this);
            _maileBLL = new MaileBLL(this);
            _kategorieDostep = new KategorieDostep(this);
            _zamowienia = new ZamowieniaDostep(this);
            _profilKlientaBll = new ProfilKlientaDostep(this);
            _regionyDostep = new RegionyDostep(this);
            _adresyDostep = new AdresyBLL(this);
            _produktyKategorieDostep = new ProduktyKategorieDostep(this);
            _produktyJednostkiDostep = new ProduktyJednostkiDostep(this);
            _widocznosciTypowBll = new WidocznosciTypowBLL(this);
            _listyPrzewozoweBll = new ListyPrzewozoweBll();
            _dokumentyDostep = new DokumentyDostep(this);
            _jednostkaProduktuBll = new JednostkaProduktuBll(this);
            _tresciDostep = new TresciDostep(this);
            _pieczatki = new PieczatkiBll(this);
            _sposobyPokazywaniaStanowBll = new SposobyPokazywaniaStanowBLL(this);
            _testyKonfiguracji = new TestyKonfiguracji(this);
            _punktyDostep = new PunktyDostep(this);
            _produktyUkryteBll = new ProduktyUkryteBll(this);
            _komunikatyBll = new KomunikatyBll(this);
            _szukanie = new Szukanie(this);
            _kupowaneIlosciBll = new KupowaneIlosciBLL(this);
            _mailNoweProdukty = new MailNoweProdukty(this);
            _mailProduktyPrzyjeteNaMagazyn = new MailProduktyPrzyjeteNaMagazyn(this);
        }

        public KupowaneIlosciBLL KupowaneIlosciBLL
        {
            get { return _kupowaneIlosciBll; }
        }

        public Szukanie Szukanie
        {
            get { return  this._szukanie;}
        }

        public ProduktyUkryteBll ProduktyUkryteBll
        {
            get { return _produktyUkryteBll; }
        }

        public IDaneDostep DostepDane
        {
            get { return _dal; }
        }

        public IPunktyDostep PunktyDostep
        {
            get { return _punktyDostep; }
        }

        public ITresciDostep TresciDostep
        {
            get { return _tresciDostep; }
        }

        public ICacheBll Cache
        {
            get { return _cache; }
        }

        public IMaileBLL MaileBLL
        {
            get { return _maileBLL; }
        }

        public IProduktyJednostkiDostep ProduktyJednostkiDostep
        {
            get { return _produktyJednostkiDostep; }
        }

        public IProduktyKategorieDostep ProduktyKategorieDostep
        {
            get { return _produktyKategorieDostep; }
        }

        public IAdresyDostep AdresyDostep
        {
            get { return _adresyDostep; }
        }

        public IRegionyDostep RegionyDostep
        {
            get { return _regionyDostep; }
        }

        public IProfilKlientaBll ProfilKlienta
        {
            get { return _profilKlientaBll; }
        }

        public ICechyProduktyDostep CechyProduktyDostep
        {
            get { return _cechyProduktyDostep; }
        }


        public IKlienciDostep Klienci
        {
            get { return _klienci; }
        }

        public IBlogDostep Blog
        {
            get { return _blog; }
        }

        public IKoszykiDostep Koszyk
        {
            get { return _koszyk; }
        }

        public IStatystyki Statystyki
        {
            get { return _statystyki; }
        }

        public IPoziomyCenBll CenyPoziomy
        {
            get { return _cenyPoziomy; }
        }

        public IProduktyBazowe ProduktyBazowe
        {
            get { return _produktyBazowe; }
        }

        public IPliki Pliki
        {
            get { return _pliki; }
        }

        public IProduktyStanBll ProduktyStanBll
        {
            get { return _produktyStanBll; }
        }

        public IZadaniaBLL ZadaniaBLL
        {
            get { return _zadaniaBLL; }
        }

        public ISklepy Sklepy
        {
            get { return _sklepy; }
        }

        public IRabatyBll Rabaty
        {
            get { return _rabaty; }
        }

        public IProduktyKlienta ProduktyKlienta
        {
            get { return _produktyKlienta; }
        }

        public ICechyAtrybuty CechyAtrybuty
        {
            get { return _cechyAtrybuty; }
        }

        public IKategorieDostep KategorieDostep
        {
            get { return _kategorieDostep; }
        }

        public IZamowieniaDostep ZamowieniaDostep
        {
            get { return _zamowienia; }
        }

        public IConfigBLL Konfiguracja
        {
            get { return _konfiguracja; }
        }

        public IWidocznosciTypowBLL WidocznosciTypowBll
        {
            get { return _widocznosciTypowBll; }
        }

        //public IAtrybutyDostep AtrybutyDostep
        //{
        //    get { return _atrybutyDostep; }
        //}
        public IListyPrzewozoweBll ListyPrzewozoweBll
        {
            get { return _listyPrzewozoweBll; }
        }
        public IDokumentyDostep DokumentyDostep
        {
            get { return _dokumentyDostep; }
        }

        public IJednostkaProduktuBll JednostkaProduktuBll
        {
            get { return _jednostkaProduktuBll; }
        }

        public IPieczatkiBll PieczatkiBll
        {
            get { return _pieczatki; }
        }

        public SposobyPokazywaniaStanowBLL SposobyPokazywaniaStanowBll
        {
            get { return _sposobyPokazywaniaStanowBll; }
        }
        public KomunikatyBll KomunikatyBll
        {
            get { return _komunikatyBll; }
        }
        public ITestyKonfiguracji TestyKonfiguracji
        {
            get { return _testyKonfiguracji; }
        }

        public MailNoweProdukty MailNoweProdukty
        {
            get { return _mailNoweProdukty; }
        }

        public MailProduktyPrzyjeteNaMagazyn MailProduktyPrzyjeteNaMagazyn => _mailProduktyPrzyjeteNaMagazyn;

    }

    public abstract class BllBazaCalosc : BazaDostepDane
    {
        protected ISolexBllCalosc Calosc;

        protected BllBazaCalosc(ISolexBllCalosc calosc)
        {
            Calosc = calosc;
        }

        public virtual void ZadaniaInicjacyjne()
        {
        }
    }
}