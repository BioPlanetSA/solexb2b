using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolEx.Hurt.Model;
using SolEx.Hurt.Sync.App.Modules_.NoweModuly.KategorieProduktow;
using Xunit;
namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.KategorieProduktow.Tests
{
    public class FiltrowanieDrzewkaTests
    {

        [Fact()]
        public void PrzetworzTest()
        {
            int grupa = 1;
            var wejsciowa = new Dictionary<long, KategoriaProduktu>();
            wejsciowa.Add(6175876, new KategoriaProduktu { Id = 6175876, GrupaId = grupa, Nazwa = "Anteny", ParentId = 1295251276 });
            wejsciowa.Add(6175876, new KategoriaProduktu { Id = 6175876, GrupaId = grupa, Nazwa = "Anteny", ParentId = 1295251276 });
            wejsciowa.Add(6175876, new KategoriaProduktu { Id = 6175876, GrupaId = grupa, Nazwa = "Anteny", ParentId = 1295251276 });
           FiltrowanieDrzewka fd=new FiltrowanieDrzewka();
            fd.Gałąź = "test";
            fd.IDGrupy = grupa.ToString();
            fd.Przetworz(ref wejsciowa, new Dictionary<long, KategoriaProduktu>(), null, new List<Grupa>());
            Assert.Equal(wejsciowa.Count,3);
            
        }
    }
}
