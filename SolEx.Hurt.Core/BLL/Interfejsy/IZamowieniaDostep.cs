using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model;
using System.Collections.Generic;
using System.Linq;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core.BLL.Interfejsy
{
    public interface IZamowieniaDostep
    {
        IEnumerable<ZamowienieSynchronizacja> PobierzZamowieniaOczekujaceNaImportDoERP(IKlient idZadajacego);

        int AktualizujZamowienia(ZamowieniaBLL update, List<ZamowieniaProduktyBLL> pozycje = null);

        string GenerujNumerZamowieniaDlaOddzialu(IKlient klient, int rok);


        List<StatusZamowienia> AktulizujStatusyZamowienien(List<StatusZamowienia> list);

        void SprawdzStatusy_UtworzStatusySystemowe();


        bool SpradzCzyKlientMaPrawoDoZamowienia(ZamowieniaBLL arg1, IKlient arg2);

       void UsunStatusyCache(IList<object> list);

      
    }
}