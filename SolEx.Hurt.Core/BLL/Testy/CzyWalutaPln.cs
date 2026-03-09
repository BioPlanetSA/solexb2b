using ServiceStack.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SolEx.Hurt.Core.BLL.Testy
{
    public class CzyWalutaPln : TestKonfiguracjiBaza
    {
        public ISolexBllCalosc Bll = SolexBllCalosc.PobierzInstancje;

        public override string Opis
        {
            get { return "Test sprawdzający czy poziom cen ma walute PLN"; }
        }

        public override List<string> Test()
        {
            List<string> listaBledow = new List<string>();
            //bool cenyPoziom = Bll.DostepDane.Pobierz<ceny_poziomy>(null,a => !a.Waluta.IsNullOrEmpty() && a.Waluta.Equals("PLN", StringComparison.InvariantCultureIgnoreCase)).Any();
            //if (cenyPoziom)
            //{
            //    listaBledow.Add("PLN powinny zostać zmienione na zł");
            //}
            bool poziomCeny = Bll.DostepDane.Pobierz<Model.Waluta>(null, a => !a.WalutaB2b.IsNullOrEmpty() && a.WalutaB2b.Equals("PLN", StringComparison.InvariantCultureIgnoreCase)).Any();
            if (poziomCeny)
            {
                listaBledow.Add("PLN powinny zostać zmienione na zł");
            }
            return listaBledow;
        }
    }
}