using System.Collections.Generic;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core.BLL.Interfejsy
{
    public interface IBlogDostep
    {
        void AktualizujLacznikiKategorii(IList<BlogWpisBll> obj);
        IList<BlogWpisBll> BindingPoSelect(int jezykId, IKlient zadajacyKlient, IList<BlogWpisBll> listaWpisowZBazy, object parametryDoMetodyPoSelect);
    }
}