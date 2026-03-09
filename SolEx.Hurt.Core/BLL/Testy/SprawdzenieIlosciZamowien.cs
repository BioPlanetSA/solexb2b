using SolEx.Hurt.Core.BLL.Interfejsy;
using System.Collections.Generic;
using System.Linq;
using ServiceStack.OrmLite;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Core.BLL.Testy
{
    public class SprawdzenieIlosciZamowien : TestKonfiguracjiBaza
    {
        public IZamowieniaDostep Zamowienia = SolexBllCalosc.PobierzInstancje.ZamowieniaDostep;

        public override string Opis
        {
            get { return "Test sprawdzaj¹cy czy jest wiêcej ni¿ 5 zamówieñ"; }
        }

        /// <summary>
        /// Sprawdza czy ilosc zamowien jest wieksza niz 5
        /// </summary>
        public override List<string> Test()
        {
            List<string> listaBledow = new List<string>();
            long ilosc= SolexBllCalosc.PobierzInstancje.DostepDane.DbORM.Count<HistoriaDokumentu>(x => x.Rodzaj == RodzajDokumentu.Zamowienie);
            if (ilosc <= 5)
            {
                listaBledow.Add(string.Format("Nie ma wiêcej ni¿ 5 zamowieñ, ich iloœæ wynosi: {0}", ilosc));
            }
            return listaBledow;
        }
    }
}