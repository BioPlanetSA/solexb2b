using SolEx.Hurt.Model;
using System.Collections.Generic;

namespace SolEx.Hurt.Core.BLL.Interfejsy
{
    public interface IProduktyStanBll
    {
        /// <summary>
        /// wyciaganie pojedynczego stanu dla produktu z danego magazynu
        /// </summary>
        /// <param name="produktId"></param>
        /// <param name="magazynId"></param>
        /// <returns></returns>
        //ProduktStan StanProduktuPobierz(int produktId, int magazynId);

        Dictionary<int, Dictionary<long, decimal>> WszystkieStanyDlaMagazynow();

        List<ProduktStan> PobierzStanyProduktowNieZerowe(Magazyn mag);
        Magazyn MagazynDomyslny();
        void ZmniejszStany(long produktID, decimal ilosc, string magazynPostawowy);

        void UstawMagazynRealizujacy(IList<Magazyn> obj);
       // decimal PobierzStanyDlaProduktu(HashSet<string> nazwyMagazynow, long idProduktu);
        //decimal PobierzStanyDlaProduktu(HashSet<int> nazwyMagazynow, long idProduktu);
        void UsunCache(IList<object> obj);
    }
}