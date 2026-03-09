using System.Collections.Generic;
using ServiceStack.OrmLite;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core.BLL.Interfejsy
{
    public interface IPliki
    {
        IObrazek PobierzObrazek(string sciezkaPliku);

        /// <summary>
        /// Pobiera obrazek po id
        /// </summary>
        /// <param name="idObrazka">id obrazka</param>
        /// <returns></returns>
        IObrazek PobierzObrazek(int idObrazka);

        /// <summary>
        /// Pobieramy liste Obrazków na podstawie HashSet ze scie¿kami
        /// </summary>
        /// <param name="sciezkiObrazkow"></param>
        /// <returns></returns>
        List<IObrazek> PobierzListeObrazkow(HashSet<string> sciezkiObrazkow);

        void PlikiProduktuUsun(HashSet<string> ids);
        Plik DodajPlikUzytkownika(Plik p);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p">plik do zapisu</param>
        /// <param name="dodacDoBazyDanychPlikowB2B">jeœ³i false to nie dodajmy do bazy danych B2B</param>
        /// <returns></returns>
        Plik ImportPlikBase64(Plik p, bool dodacDoBazyDanychPlikowB2B = true);

        bool CzyPlikToZdjecie(Plik p);
        void UsunPlikiZBazyBezPlikowFizycznych();
        List<IObrazek> PobierzObrazkiProduktu(long produktID);

        /// <summary>
        /// lista ³¹czników pogrupowana wg. produktow id - laczniki w kolejnosci takiej jak powinny byc pokazywane na stronie
        /// </summary>
        /// <returns></returns>
        Dictionary<long, List<ProduktPlik>> Laczniki { get; set; }

        OrmLiteConnectionFactory DbFactory { get; set; }

        void UsunCache(IList<object> obj);
        void BindPoAktualizacji(IList<ProduktPlik> obj);
        void ZadaniaInicjacyjne();

        void UsunPlikiFizyczneBezPlikuWBazie();
        bool CzyPlikToZdjecie(string sciezka);
    }
}