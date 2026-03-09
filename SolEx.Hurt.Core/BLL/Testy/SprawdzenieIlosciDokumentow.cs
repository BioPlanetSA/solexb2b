using System.Collections.Generic;
using ServiceStack.OrmLite;
using SolEx.Hurt.Model;

namespace SolEx.Hurt.Core.BLL.Testy
{
    public class SprawdzenieIlosciDokumentow : TestKonfiguracjiBaza
    {
        public override string Opis
        {
            get { return "Test sprawdzajacy czy jest wiecej dokumentów ni¿ 10"; }
        }

        /// <summary>
        /// Sprawdza czy ilosc dokumentow jest wieksza niz 10
        /// </summary>
        public override List<string> Test()
        {
            List<string> listaBledow = new List<string>();
            if (SolexBllCalosc.PobierzInstancje.DostepDane.DbORM.Count<HistoriaDokumentu>() <= 10)
            {
                listaBledow.Add(string.Format("Nie ma wiêcej ni¿ 10 dokumentów."));
            }
            return listaBledow;
        }
    }
}