using SolEx.Hurt.Core.BLL.Interfejsy;
using System.Collections.Generic;

namespace SolEx.Hurt.Core.BLL.Testy
{
    public class TestPrzechwytywanieMaili : TestKonfiguracjiBaza
    {
        public IConfigBLL Konfiguracja = SolexBllCalosc.PobierzInstancje.Konfiguracja;

        public override string Opis
        {
            get { return "Test czy w³¹czona jest opcja wysy³anie wiadomoœci do klienta"; }
        }

        /// <summary>
        /// Sprawdzanie czy jest w³¹czone przechwytywanie maili
        /// </summary>
        public override List<string> Test()
        {
            List<string> listaBledow = new List<string>();
            if (Konfiguracja.MaileTylkoSolex)
            {
                listaBledow.Add("Opcja wysy³ania tylko do Solex jest w³¹czona");
                return listaBledow;
            }
            return listaBledow;
        }
    }
}