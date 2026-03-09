using System;
using System.Collections.Generic;
using System.Linq;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.CustomSearchCriteria;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Web.Site2.Helper;

namespace SolEx.Hurt.Web.Site2.Modules.Api
{
    /// <summary>
    /// Aktualizuje ustawienia systemowe wysłane w obiekcie Data jako Lista<ustawienia>
    /// </summary>
    public class AktualizujUstawieniaHandler : ApiSessionBaseHandler, Model.Core.IDocumentApiVisible
    {
        protected override object Handle()
        {
            IList<Ustawienie> data = (IList<Ustawienie>) Data;
            IList<long?> pracownikId = data.Select(p => p.OddzialId).ToList();
            IList<string> symbol = data.Select(p => p.Symbol).ToList();
            
            var existing = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<Ustawienie>(null).Where(u=> symbol.Contains(u.Symbol) && pracownikId.Contains(u.OddzialId));

            var propertisy = typeof(Ustawienie).Properties();
            var akcesor = typeof(Ustawienie).PobierzRefleksja();

            foreach (Ustawienie t in data)
            {
                Ustawienie c = existing.FirstOrDefault(p =>p.Symbol.Equals(t.Symbol, StringComparison.InvariantCultureIgnoreCase) && p.OddzialId == t.OddzialId);
                if (c == null)
                {
                    t.Nazwa = t.Nazwa ?? t.Symbol;
                    SolexBllCalosc.PobierzInstancje.DostepDane.AktualizujPojedynczy<Ustawienie>(t);
                    //db.Insert(t);
                }
                else
                {
                    if (!t.Porownaj(c, propertisy,akcesor))
                    {
                        SolexBllCalosc.PobierzInstancje.DostepDane.AktualizujPojedynczy<Ustawienie>(t);
                        //db.Update(t);
                    }
                }
            }
            return SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<Ustawienie>(null).Where(u => symbol.Contains(u.Symbol) && pracownikId.Contains(u.OddzialId));
        }

        public override Type PrzyjmowanyTyp
        {
            get { return typeof(List<Ustawienie>); }
        }
    }

    /// <summary>
    /// Pobiera listę ustawień systemowych jako Lista<ustawienia>
    /// </summary>
    [ApiUprawnioneRole(RoleType.Przedstawiciel, RoleType.Administrator)]
    public class PobierzUstawieniaHandler : ApiSessionBaseHandler, Model.Core.IDocumentApiVisible
    {
        protected override object Handle()
        {
            if (!Customer.CzyAdministrator)
            {
                return SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<Ustawienie>(null).Where(x => x.NadpisywanyPracownik == true).ToList();
            }

            return SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<Ustawienie>(null);

        }

        public override Type PrzyjmowanyTyp
        {
            get { return typeof(List<Ustawienie>); }
        }
    }
}
