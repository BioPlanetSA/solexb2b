using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model.Enums;
using System.Collections.Generic;
using Enumerable = System.Linq.Enumerable;

namespace SolEx.Hurt.Core.BLL.Testy
{
    public class TestSlabychHasel : TestKonfiguracjiBaza
    {
        public IKlienciDostep Klienci = SolexBllCalosc.PobierzInstancje.Klienci;

        public override string Opis
        {
            get { return "Test s³abych hase³"; }
        }

        /// <summary>
        /// Sprawdzanie slabych hasel dla ról innych ni¿ klient
        /// </summary>
        public override List<string> Test()
        {
            string haslo = "123";
            List<string> listaBledow = new List<string>();
            //List<IKlient> kl = new List<IKlient>();

            IList<Klient> listaKl = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<Klient>(null);
            haslo = Tools.PobierzInstancje.GetMd5Hash(haslo);

            foreach (var klient in listaKl)
            {
                if (klient.Role == null || !Enumerable.Any(klient.Role))
                {
                    continue;
                }
                if (!klient.Role.Contains(RoleType.Pracownik)) { continue; }
                if (string.Equals(klient.HasloKlienta, haslo))
                {
                    listaBledow.Add(string.Format("Pracownik z emailem: {0} posiada s³abe has³o", klient.Email));
                }
            }
            return listaBledow;
        }
    }
}