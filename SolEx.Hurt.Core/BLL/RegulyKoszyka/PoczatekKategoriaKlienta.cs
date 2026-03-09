using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Interfaces;
using System;
using System.Collections.Generic;

namespace SolEx.Hurt.Core.BLL.RegulyKoszyka
{
    [Obsolete("zamiaste tego użyj modułu KategoriaKlienta")]
    public class PoczatekKategoriaKlienta : RegulaKoszyka, IRegulaCalegoKoszyka, ITestowalna
    {
        public override string Opis
        {
            get { return "Czy klient należy do kategorii klientow o wybranym początku kategorii"; }
        }

        [FriendlyName("Początek kategorii klienta np. Dostawa_")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string PoczatekKategorii { get; set; }

        public bool KoszykSpelniaRegule(IKoszykiBLL koszyk)
        {
            throw new NotImplementedException();
            //   PoczatekKategorii = PoczatekKategorii.ToLower();
            //   IKlient k =SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<Klient>(koszyk.klient_id);
            //   return !string.IsNullOrEmpty(PoczatekKategorii) && k.Kategorie.Any(p => p.nazwa.ToLower().StartsWith(PoczatekKategorii));
        }

        public List<string> TestPoprawnosci()
        {
            List<string> listaBledow = new List<string>();
            IList<KategoriaKlienta> wszystkieKategorieKlientow = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<KategoriaKlienta>(null);
            foreach (var kategorie_Klientow in wszystkieKategorieKlientow)
            {
                if (kategorie_Klientow.Nazwa.StartsWith(PoczatekKategorii))
                {
                    return listaBledow;
                }
            }

            listaBledow.Add(string.Format("Żadna z kategori klienta nie rozpoczyna się od {0}", PoczatekKategorii));
            return listaBledow;
        }
    }
}