using Xunit;
using SolEx.Hurt.Web.Site2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Web.Site2.Models.Tests
{
    public class ParametryPrzekazywaneDoListyProduktowTests
    {
        [Fact()]
        public void KluczDoCachuFiltrowTest()
        {
            ISolexBllCalosc calosc = A.Fake<ISolexBllCalosc>();
            IKlient klient = A.Fake<IKlient>();
            klient.Id = 22;
            string staleFiltryKey = "12,23,34";
            long? kategoria=null;
            string szukane = "";
            A.CallTo(() => calosc.ProfilKlienta.PobierzStaleFiltryString(klient)).Returns(staleFiltryKey);
            Dictionary<int, HashSet<long>> filtry = new Dictionary<int, HashSet<long>>();
            filtry.Add(1,new HashSet<long>() {2,3,4});


            ParametryPrzekazywaneDoListyProduktow pr = new ParametryPrzekazywaneDoListyProduktow();
            pr.Calosc = calosc;
            pr.filtry = null;
            string klucz = $"Filtry__{staleFiltryKey}_{kategoria}_{szukane}";
            string wynik = pr.KluczDoCachuFiltrow(klient);
            Assert.True(wynik.Equals(klucz), $"Klucz {wynik} źle wyliczony");

            pr = new ParametryPrzekazywaneDoListyProduktow();
            pr.Calosc = calosc;
            pr.filtry = filtry;
            klucz = $"Filtry_2,3,4_{staleFiltryKey}_{kategoria}_{szukane}";
            wynik = pr.KluczDoCachuFiltrow(klient);
            Assert.True(wynik.Equals(klucz), $"Klucz z filtrami: {wynik} źle wyliczony");

            pr = new ParametryPrzekazywaneDoListyProduktow();
            pr.Calosc = calosc;
            pr.filtry = filtry;
            klient.OfertaIndywidualizowana = true;
            klucz = $"Filtry_2,3,4_{staleFiltryKey}_{kategoria}_{szukane}_{klient.Id}";
            wynik = pr.KluczDoCachuFiltrow(klient);
            Assert.True(wynik.Equals(klucz), $"Klucz z oferą: {wynik} źle wyliczony");

            kategoria = 11;
            klucz = $"Filtry_2,3,4_{staleFiltryKey}_{kategoria}_{szukane}_{klient.Id}";
            pr = new ParametryPrzekazywaneDoListyProduktow();
            pr.Calosc = calosc;
            pr.filtry = filtry;
            pr.kategoria = kategoria;
            klient.OfertaIndywidualizowana = true;
            wynik = pr.KluczDoCachuFiltrow(klient);
            Assert.True(wynik.Equals(klucz), $"Klucz z kategoria: {wynik} źle wyliczony");

            szukane = "costam";
            klucz = $"Filtry_2,3,4_{staleFiltryKey}_{kategoria}_{szukane}_{klient.Id}";
            pr = new ParametryPrzekazywaneDoListyProduktow();
            pr.Calosc = calosc;
            pr.filtry = filtry;
            pr.kategoria = kategoria;
            pr.szukane = szukane;
            klient.OfertaIndywidualizowana = true;
            wynik = pr.KluczDoCachuFiltrow(klient);
            Assert.True(wynik.Equals(klucz), $"Klucz z szukamym: {wynik} źle wyliczony");

        }
    }
}