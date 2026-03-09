using SolEx.Hurt.Core.BLL.Interfejsy;
using System.Collections.Generic;

namespace SolEx.Hurt.Core.BLL.Testy
{
    public class TestKonfiguracjiSkrzynkiNewsletterow : TestKonfiguracjiBaza
    {
        public IConfigBLL Konfiguracja = SolexBllCalosc.PobierzInstancje.Konfiguracja;

        public override string Opis
        {
            get { return "Test skrzynki newsletter"; }
        }

        /// <summary>
        /// Test skrzynki pocztowej newsletterów
        /// </summary>
        public override List<string> Test()
        {
            List<string> listaBledow = new List<string>();
            if (string.IsNullOrEmpty(Konfiguracja.MailingEmailHost))
            {
                listaBledow.Add("Brak hosta");
            }
            if (string.IsNullOrEmpty(Konfiguracja.MailingEmailNazwaUzytkownika))
            {
                listaBledow.Add("Brak u¿ytkownika");
            }
            if (string.IsNullOrEmpty(Konfiguracja.MailingEmailHaslo))
            {
                listaBledow.Add("Brak has³a u¿ytkownika");
            }
            if (string.IsNullOrEmpty(Konfiguracja.MailingEmailFrom))
            {
                listaBledow.Add("Brak nadawcy");
            }
            return listaBledow;
        }
    }
}