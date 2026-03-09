using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Interfaces.Modele;
using System.Collections.Generic;

namespace SolEx.Hurt.Core.BLL
{
    public interface ISklepy
    {
        void Zapisz(ISklepyBll item, IKlienci zapisujacy);

        bool UzupelnijWspolrzedne(Model.Adres sklep);

        void Zapisz(IList<Sklep> doZmiany, IKlienci zapisujacy);

        /// <summary>
        /// zwraca posortowana miasta dla sklepow bedacych w określonych kategoriach
        /// </summary>
        /// <param name="kategorieId"></param>
        /// <returns></returns>
        List<string> MiastaDlaWybranychKategorii(HashSet<long> kategorieId);

        string PobierzIkoneNaMape(ISklepyBll v, int jezykId);

        string PobierzIkoneKategorii(KategoriaSklepu kat);

        List<KategoriaSklepu> PobierzKategorieNiepusteIPoprawneKoordynaty(HashSet<long> kategoriaid, int jezyk);

        void AktualizujSklepMapaSklepow();

        void DodajAdresDoSklepu(IList<SklepyBll> obj);
        void UsunAdresSklepu(IList<long> obj);

        void ZarzadzajLacznikamiKategoriiSklepu(IList<SklepyBll> obj);
        List<SklepyBll> ListaSklepowDlaWybranychKategorii(HashSet<long> kategorieId, bool tylkoZPoprawnymoKoordynatami = true, string miasto = null, bool zaweajPunktyNaMapie=true);
        Dictionary<long, string> PobierzIkonyNaMape(int jezykId);
    }
}