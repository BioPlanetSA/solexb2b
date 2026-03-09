using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolEx.Hurt.Model;
using SolEx.Hurt.Sync.App.Modules_.NoweModuly.CechyIAtrybuty;
using Xunit;
namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.CechyIAtrybuty.Tests
{
    public class FiltrowanieCechDoWysylkiTests
    {
        [Fact]
        public void PrzetworzTest()
        {
            Cecha c1 = new Cecha() { Id = 1, AtrybutId = 1};
            Cecha c2 = new Cecha() { Id = 1, AtrybutId = 2 };
            Cecha c3 = new Cecha() { Id = 1, AtrybutId = 1 };
            Cecha c4 = new Cecha() { Id = 1, AtrybutId = 2 };
            Cecha c5 = new Cecha() { Id = 1, AtrybutId = 3 };
            Cecha c6 = new Cecha() { Id = 1, AtrybutId = 4 };
            List<Cecha> listaCech = new List<Cecha>(){c1,c2,c3,c4,c5,c6};

            FiltrowanieCechDoWysylki filtr = new FiltrowanieCechDoWysylki();
            filtr.ListaAtrybutow=new List<string>(){"1","2"};
            Dictionary<long, Produkt> produktyNaB2B = new Dictionary<long, Produkt>();
            List<Atrybut> listaAtrybutow = new List<Atrybut>();
            filtr.Przetworz(ref listaAtrybutow, ref listaCech, produktyNaB2B);
            Assert.True(listaCech.Count == 4);

        }
    }
}
