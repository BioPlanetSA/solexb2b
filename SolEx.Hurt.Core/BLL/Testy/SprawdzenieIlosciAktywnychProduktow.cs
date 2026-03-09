using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Core.ModelBLL;
using System.Collections.Generic;

namespace SolEx.Hurt.Core.BLL.Testy
{
    public class SprawdzenieIlosciAktywnychProduktow : TestKonfiguracjiBaza
    {
        public IProduktyBazowe ProduktyBazowe = BLL.SolexBllCalosc.PobierzInstancje.ProduktyBazowe;

        public override string Opis
        {
            get { return "Test sprawdzaj¹cy czy jest wiêcej ni¿ 10 aktywnych produktów"; }
        }

        /// <summary>
        /// Sprawdza czy ilosc aktywnych produktow jest wieksza niz 10
        /// </summary>
        public override List<string> Test()
        {
            List<string> listaBledow = new List<string>();
            int i = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<ProduktBazowy>(null, x => x.Widoczny).Count;

            if (i < 10)
            {
                listaBledow.Add(string.Format("Nie ma wiêcej ni¿ 10 aktywnych produktów, ich iloœæ wynosi - {0}", i));
            }
            return listaBledow;
        }
    }
}