using ServiceStack.Common;
using SolEx.Hurt.Model;
using System.Collections.Generic;

namespace SolEx.Hurt.Core.BLL.Testy
{
    public class CzyWyslanyNewsletter : TestKonfiguracjiBaza
    {
        public ISolexBllCalosc Bll = SolexBllCalosc.PobierzInstancje;

        public override string Opis
        {
            get { return "Test czy przynajmniej jeden newsletter zosta³ wys³any"; }
        }

        /// <summary>
        /// Sprawdzenie czy przynajmniej jeden newsletter zosta³ wys³any
        /// </summary>
        /// <returns></returns>
        public override List<string> Test()
        {
            List<string> listaBledow = new List<string>();
            IList<NewsletterKampania> kampania = Bll.DostepDane.Pobierz<NewsletterKampania>(null);
            if (kampania.IsEmpty())
            {
                listaBledow.Add("¯aden newsletter nie zosta³ wys³any");
            }
            return listaBledow;
        }
    }
}