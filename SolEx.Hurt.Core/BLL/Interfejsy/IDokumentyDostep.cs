using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.CustomSearchCriteria;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Web;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using SolEx.Hurt.Model.Core;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core.BLL.Interfejsy
{
    public interface IDokumentyDostep
    {
        bool CzyKlientPosiadaJakiesPrzeterminowaneFaktury(long klient);


        bool IstniejeZalacznik(int dokument, string rozszerzenie);

        string PobierzSciezkePliku(int dokumentId, string rozszerzenie);


        void Aktualizuj(List<KlasaOpakowujacaDokumentyDoWyslania> paczka);

        Dictionary<HistoriaDokumentu, List<HistoriaDokumentuProdukt>> PobierzSlownikDokumentowIPozycji(Klient klient, Expression<Func<HistoriaDokumentu, bool>> warunek );

        Dictionary<int, long> PobierzSumyKontrolneDokumentow();

        string KatalogDokumentow { get; }

        bool CzyDokumentNalezyDoKlienta(DokumentyBll dokument, IKlient k, HashSet<long> subkonta = null);

        bool CzyKlientMaDostep(DokumentyBll dokument, IKlient k, HashSet<long> subkonta = null);

        List<ParametryPobieraniaDokumentu> PobierzDostepneFormatyDoPobrania(DokumentyBll dokument);

        byte[] PobierzPlik(DokumentyBll dokument, string modul, IKlient zadajacy, out Encoding kodowanie, out string nazwa, string parametry = null);

        IList<HistoriaDokumentu> PobierzNiezaplaconeFakturyZTerminemPlatnosci();

        Dictionary<HistoriaDokumentu, bool> PobierzFakturyNiezaplaconeWzgledemDaty(int? ileDniPonowneWyslanie, int dokumentPrzeterminujesieWCiagu);

        Dictionary<HistoriaDokumentu, bool> PobierzDokumentyPrzeterminowaneWzgledemDaty(int? ileDniPonowneWyslanie,int mineloConajmniejOdTerminuPlatnosi);

        List<DokumentyBll> PobierzWyfiltrowaneDokumenty(IKlient zalogowany, IKlient klient, RodzajDokumentu rodzaj,
            DateTime odKiedy, DateTime doKiedy, string szukanieFraza = null, bool sortuj = false, bool UzupelnijODokumentyPowiazane = false);

        DocumentSummary PobierzPodsumowanieFakturKlient(IKlient klient);

        DocumentSummary WygenerujeDaneDoWykresuFaktur(List<DokumentyBll> dokumenty);

        DocumentSummary WygenerujeDaneDoWykresuZamowien(List<DokumentyBll> dokumenty);

        void WysylaniePowiadomienONowychFakturach(HashSet<int> kategorie);

        void WyslijMailaOPrzeterminowanychFakturach(int dokumentPrzeterminujesieWCiagu, int mineloConajmniejOdTerminuPlatnosi, int? ileDniPonowneWyslanie, int[] kategoriaKlientaNieWysylaj, int[] kategoriaKlientaWysylaj);

        IList<GenerowanieDokumentu> DostepneFormaty();

        Dictionary<int, long> PobierzSumyKontrolnePozycjiDokumentow(HashSet<int> idDokumentow);

        void UsunNiepotrzebne();
        
        List<DokumentyStawkiVAT> DokumentyStawkiVat(DokumentyBll dokument);
        bool GetMozliwaPlatnosc(DokumentyBll dokument);

        IList<T> PobierzElementyPoSelect<T>(int jezykId, IKlient zadajacy, IList<T> obj, object parametrDoMetodyPoSelect=null);


        IList<T> UzupelnijPozycjePoSelect<T>(int jezykID, IKlient zadajacy, IList<T> objekty, object parametrDoMetodyPoSelect);
        DokumentyBll PobierzDokumentIDUwzgledniajacSztuczneZamowienia(long id, int jezyk, IKlient klient);
        bool CzySaDokumentyDlaKlienta(RodzajDokumentu rodzaj, IKlient idKlietna);
        void UsunZamowienieDokument(IList<int> obj);
        void UsunDokumentDlaUsunietegoZamowienia(IList<ZamowieniaBLL> obj);
        HashSet<long> PobierzKlientowKtorymWyslacMaileOPrzeterminowaniu(int dokumentPrzeterminujesieWCiagu, int mineloConajmniejOdTerminuPlatnosi, int? ileDniPonowneWyslanie, string warunek, string odJakiejDatySprawdzacPonowneWysylanie, out Dictionary<long, List<HistoriaDokumentu>> slownikDokumentowPogrupowanychPoKliencie);
    }
}