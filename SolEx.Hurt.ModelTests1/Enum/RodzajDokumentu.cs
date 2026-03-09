using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolEx.Hurt.Model.Enums;
using Xunit;

namespace SolEx.Hurt.ModelTests1.Enum
{
    public class RodzajDokumentuTests
    {
        [Fact(DisplayName = "Test rodzaju dokumentu")]
        public void ZadaniaTest()
        {

            RodzajDokumentu r = (RodzajDokumentu) 2;
            Assert.Equal(r,RodzajDokumentu.Zamowienie);

            RodzajDokumentu r2 = (RodzajDokumentu)1;
            Assert.Equal(r2, RodzajDokumentu.Faktura);



            RodzajDokumentu r3 = (RodzajDokumentu) 3;

            Assert.True((int)r3==3);
        }
    }
}
