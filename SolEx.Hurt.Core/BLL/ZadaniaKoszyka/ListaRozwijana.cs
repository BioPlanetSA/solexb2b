using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace SolEx.Hurt.Core.BLL.ZadaniaKoszyka
{
    internal class ListaRozwijana : ZadanieCalegoKoszyka, IModulStartowy, IPoleWlasneKoszyka, ITestowalna
    {
        public ListaRozwijana()
        {
            Pozycja=PozycjaNaWidokuKoszyka.ZPrawej;
        }
        public override string Opis
        {
            get { return "Dodaje w koszyku listę rozwijaną"; }
        }

        public override bool Wykonaj(ModelBLL.Interfejsy.IKoszykiBLL koszyk)
        {
            return true;
        }

        [FriendlyName("Symbol")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string Symbol { get; set; }

        [FriendlyName("Nazwa dla klienta")]
        [Lokalizowane]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string NazwaDlaKlienta { get; set; }

        [FriendlyName("Opcje listy - wartości oddzielone średnikami")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string OpcjeList { get; set; }

        public string[] PobierzOpcje()
        {
            return OpcjeList.Split(';').ToArray();
        }

        public List<string> TestPoprawnosci()
        {
            List<string> listaBledow = new List<string>();
            var query = PobierzOpcje().GroupBy(x => x)
              .Where(g => g.Count() > 1)
              .Select(y => y.Key)
              .ToList();

            if (query.Count > 0)
            {
                listaBledow.Add(string.Format("Zawiera zduplikowane elementy"));
            }
            return listaBledow;
        }

        public TypDodatkowegoPolaKoszykowego TypPola
        {
            get { return TypDodatkowegoPolaKoszykowego.Select; }
        }

        [FriendlyName("Klient ma obowiązek wybrać wartość")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public bool Wymagane { get; set; }

        [FriendlyName("Lista wielokrotnego wyboru")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public bool Multiwybor { get; set; }

        [FriendlyName("Pozycja kontrolki w koszyku")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public PozycjaNaWidokuKoszyka Pozycja { get; set; }
    }
}