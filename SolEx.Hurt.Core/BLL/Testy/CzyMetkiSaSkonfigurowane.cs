using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Model.Enums;
using System.Collections.Generic;
using System.Linq;

namespace SolEx.Hurt.Core.BLL.Testy
{
    public class CzyMetkiSaSkonfigurowane : TestKonfiguracjiBaza
    {
        public IConfigBLL Konfiguracja = SolexBllCalosc.PobierzInstancje.Konfiguracja;
        public ICechyAtrybuty CechyAtrybuty = BLL.SolexBllCalosc.PobierzInstancje.CechyAtrybuty;

        public override string Opis
        {
            get { return "Test sprawdzaj¹cy czy s¹ skonfigurowane s¹ przynajmniej 2 metki"; }
        }

        /// <summary>
        /// Spradzenie czy przynajmniej dwie metki sa skonfigurowane
        /// </summary>
        public override List<string> Test()
        {
            List<string> listaBledow = new List<string>();
            var runiwe = CechyAtrybuty.PobierzWszystkieCechy(Konfiguracja.JezykIDPolski)
                .Values.Where(
                    a =>
                        (a.MetkaKatalog != null || a.MetkaOpis != null) && (a.MetkaPozycjaLista != MetkaPozycjaLista.Brak || a.MetkaPozycjaRodziny != MetkaPozycjaRodziny.Brak || a.MetkaPozycjaSzczegoly != MetkaPozycjaSzczegoly.Brak || a.MetkaPozycjaSzczegolyWarianty != MetkaPozycjaSzczegolyWarianty.Brak)).ToList();
            if (runiwe.Count() < 2)
            {
                listaBledow.Add(string.Format("Nie ma 2 skonfigurowanych metek, ich iloœæ wynosi wynosi: {0}", runiwe.Count()));
            }
            return listaBledow;
        }
    }
}