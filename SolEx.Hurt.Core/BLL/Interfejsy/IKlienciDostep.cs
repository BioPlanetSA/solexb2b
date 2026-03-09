using System;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces.Modele;
using SolEx.Hurt.Model.Web;
using System.Collections.Generic;
using System.Linq.Expressions;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core.BLL.Interfejsy
{
    public interface IKlienciDostep
    {
        Expression<Func<Klient, IKlient, bool>> WalidatorKlientow { get; }

        //[FriendlyName(null)]
        //IKlient DomyslnyOpiekun { get; set; }

        //[FriendlyName(null)]
        //IKlient DomyslnyDrugiOpiekun { get; set; }

        //[FriendlyName(null)]
        //IKlient DomyslnyPrzedstawiciel { get; set; }

        Klient KlientNiezalogowany();

        /// <summary>
        /// Zwraca obiekt klienta wg podanego ID, lub jesli pusty to zalogowanego aktualnie klienta
        /// </summary>
        /// <param name="klientID"></param>
        /// <returns></returns>
        IKlient PobierzWgIdLubZalogowanyAktualnie(long klientID);
    //    List<IKlient> PobierzKlientowPolecajacych(long id, bool dontthrowex = false);
        Klient PobierzPologinie(string login, string hasloOdkryte = null, string hasloMd5 = null);
     //   IKlient Pobierz(string email);
        Klient Pobierz(long id, IKlient zadajacy);

        bool SprawdzenieMaila(IKlienci klient, IEnumerable<IKlienci> wszyscy);
    //    List<Model.Klient> UpdateCustomers(List<Model.Klient> update);
        bool ZmienHaslo(IKlient customer, string nowehasloodkryte);
        void ResetHasla(IKlient klient, string nowehasloodkryte = null);
        IList<Klient> PobierzKlientowRoli(RoleType rola, long? oddzialKlientNadrzedny = null);
        List<Podpowiedz> ZnajdzDoZarzadania(string searchString, int count, int lang, IKlient zadajacy);
        void WygenerujKluczeAdministratorom();
        void WygenerujKlucz(Klient customer);
        void WygenerujBrakujaceLoginy();
        void WyslijMailePowitalneOdSzefa(IKlienci wywolujacy);
        void SprawdzAdresIpKlienta(Klient c);
        bool ResetGid(string gid);
        bool JestOpiekunem(long odbiorca, IKlient opiekun);
        void DodajZdarzenieLogowannie(string login, bool ok);
        //bool MoznaLogowac(string username, string password);
        bool CzyMoznaZalogowacKlienta(string login, string haslo, string ipKlienta, out Klient klient);
        bool CzyKlientMozeSieZalogowac(Klient klient);
        List<IKlient> PobierzKontaDoZarzadania(IKlient klient);
        
        /// <summary>
        /// Lista uprawnień do adminia dla określonego klienta
        /// </summary>
        /// <param name="klient">Klient</param>
        /// <returns></returns>
        HashSet<string> UprawniniaKlienta(IKlient klient);

        /// <summary>
        /// Czy klient ma dostęp do wybranej strony
        /// </summary>
        /// <param name="klient">Klient</param>
        /// <param name="modul">Moduł</param>
        /// <returns></returns>
        bool MaDostepDoModuluAdmina(IKlient klient, NavigationItem modul);
        bool CzyKlientIstnieje(int id);
        IList<Klient> BindPoSelectKlieta(int jezykId, IKlient zadajacy, IList<Klient> obj, object parametrDoMetodyPoSelect = null);
        void UsunCacheKlienci(IList<object> obj);
        IList<ModelBLL.Pracownik> PobierzPracownikow(int jezyk, IKlient zadajacy);
        IList<AdministratorBLL> PobierzAdministratorow(int jezyk, IKlient zadajacy);
        void PrzedAktualizacjaPracownikow(IList<ModelBLL.Pracownik> obj);
        void PrzedAktualizacjaKlientow(IList<Model.Klient> obj);
        void PrzedAktualizacjaKlientow(IList<Klient> obj);
        SzablonLimitow PobierzCalkowityLimitWartosciZamowien(IKlient klient);
        SzablonLimitow PobierzCalkowityLimitIloscZamowien(IKlient klient);
        SzablonAkceptacjiBll PobierzSzablonAkceptacji(IKlient klient, bool limityprzekoczone);
        IKlient SztucznyAdministrator();
        string KluczDoKlientaWypisanieZapisaniaZNewsletera(IKlient klient);
        List<KatalogSzablonModelBLL> PobierzSzablonyWidoczneDlaKlienta(IKlient zadajacy);
        T PobierzWykorzystanyLimit<T>(IKlient klient, SzablonLimitow szablon, RodzajLimitu rodzajLimitu);
        DateTime WyliczOdKiedy(SzablonLimitow szablon);
        void UsunCacheLimitow(long idKlienta);
        bool CzyKlientNieMaMinimumLogistyczne(IKlient klient);
        bool CzyWymaganaZmianaHasla(IKlient klientDoZalogowania);
    }
}