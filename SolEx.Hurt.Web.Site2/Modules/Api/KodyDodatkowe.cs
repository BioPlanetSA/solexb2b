using System;
using System.Collections.Generic;
using System.Linq;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Web.Site2.Helper;

namespace SolEx.Hurt.Web.Site2.Modules.Api
{
    /// <summary>
    /// Aktualizuje dodatkowe kody kreskowe produktów wysłane w obiekcie Data jako Lista<ProduktyKodyDodatkowe>
    /// </summary>
    public class AktualizujKodyDodatkowe : ApiSessionBaseHandler
    {
        protected override object Handle()
        {
            List<ProduktyKodyDodatkowe> doZmiany = (List<ProduktyKodyDodatkowe>)Data;
            int packageNr = 0;
            const int packegeSize = 1000;
            List<ProduktyKodyDodatkowe> paczka;
            do
            {
                paczka = doZmiany.Skip(packageNr * packegeSize).Take(packegeSize).ToList();
                ProduktyKodyDodatkoweBll.PobierzInstancje.Aktualizuj(paczka);
                packageNr++;
            }
            while (paczka.Count == packegeSize);

            return Helpers.Status.Utworz(Helpers.StatusApi.Ok);
        }
        public override Type PrzyjmowanyTyp
        {
            get { return typeof(List<ProduktyKodyDodatkowe>); }
        }
    }

    /// <summary>
    /// Pobiera dodatkowe kody kreskowe jako Lista<ProduktKodyDodatkowe>
    /// </summary>
    public class PobierzKodyDodatkowe : ApiSessionBaseHandler, Model.Core.IDocumentApiVisible
    {
        protected override object Handle()
        {
            return Core.BLL.ProduktyKodyDodatkoweBll.PobierzInstancje.WszystkieKody().Where(x => x.Id > 0);
        }
    }

    /// <summary>
    /// Usuwa dodatkowe kody kreskowe przekazane w obiekcie Data jako ListaID<int>
    /// </summary>
    class UsunKodyDodatkowe : ApiSessionBaseHandler
    {
        protected override object Handle()
        {
            Core.BLL.ProduktyKodyDodatkoweBll.PobierzInstancje.Usun((List<int>)Data);
            return Helpers.Status.Utworz(Helpers.StatusApi.Ok);
        }
        public override Type PrzyjmowanyTyp
        {
            get { return typeof(List<int>); }
        }
    }
}
