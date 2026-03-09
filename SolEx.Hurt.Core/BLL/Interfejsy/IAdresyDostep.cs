using System.Collections;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using System.Collections.Generic;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core.BLL.Interfejsy
{
    public interface IAdresyDostep
    {
        IList<IAdres> PobierzAdresyKlienta(IKlient k);

       // void UsunLaczniki(IList<long> obj);

        void AktualizujLacznikiKlientow(IList<Adres> obj);

        IList<Adres> UzupelnijPozycjePoSelect(int jezykId, IKlient zadajacy, IList<Adres> objekty, object parametrDoMetodyPoSelect);

        //  List<Adres> Znajdz(int klient, bool pokazujJednorazowe, string kolumnaSortowania, Model.Enums.KolejnoscSortowania kierunek, string szukanieFraza);
    }
}