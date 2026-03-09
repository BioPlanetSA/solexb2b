using ServiceStack.DataAnnotations;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model;
using System;
using SolEx.Hurt.Core.ModelBLL;

namespace SolEx.Hurt.Core.BLL
{
    [Alias("HistoriaDokumentuPlatnosciOnline")]
    public class DokumentPlatnosciOnline : HistoriaDokumentuPlatnosciOnline
    {
        public DokumentPlatnosciOnline(DokumentyBll dok)
        {
            IdDokumentu = dok.Id;
            NazwaDokumentu = dok.NazwaDokumentu;
            Kwota = dok.DokumentWartoscBrutto;
            throw new Exception("Sylwe zepsul - tu nie moze byc odwolan do sesjihelpoerw - to jest CORE!");
          //  PlatnikId = SesjaHelper.PobierzInstancje.KlientID;
          //  IpOperacji = SesjaHelper.PobierzInstancje.IpKlienta;
        }
    }
}