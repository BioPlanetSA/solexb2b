using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using SolEx.Hurt.Model;
using Xunit;
namespace SolEx.Hurt.Model.Tests
{
    public class ZadaniaTests
    {
        [Fact()]
        public void ZadaniaTest()
        {
            var zadanie = A.Fake<Zadanie>();
            zadanie.Aktywne = true;
            zadanie.MozeDzialacOdGodziny = 14;
            zadanie.MozeDzialacDoGodziny = 9;
            var biezacaGodzina = A.Fake<SolEx.Hurt.Model.Helpers.DateTimeHelper>();
            zadanie._biezacaData = biezacaGodzina;
//2. zrob test dla przypadku kiedy godziny dzialania sa ustawione od 14 do 9 - a godzina aktualna to:

//- 8:00 - ma sie uruchomic
//- 9:15 - NIE
//- 15:00 - TAK
//- 1:00 - TAK

            DateTime dataDoTestowania = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 8, 0, 0);
            A.CallTo(() => biezacaGodzina.BiezacaData()).Returns(dataDoTestowania);

            Assert.Equal(true, zadanie.CzyPowinnoBycUruchomioneTeraz);

            dataDoTestowania = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 9, 15, 0);
            A.CallTo(() => biezacaGodzina.BiezacaData()).Returns(dataDoTestowania);

            Assert.Equal(false, zadanie.CzyPowinnoBycUruchomioneTeraz);

            dataDoTestowania = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 15, 0, 0);
            A.CallTo(() => biezacaGodzina.BiezacaData()).Returns(dataDoTestowania);

            Assert.Equal(true, zadanie.CzyPowinnoBycUruchomioneTeraz);

            dataDoTestowania = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 1, 0, 0);
            A.CallTo(() => biezacaGodzina.BiezacaData()).Returns(dataDoTestowania);
            Assert.Equal(true, zadanie.CzyPowinnoBycUruchomioneTeraz);

            //teraz odwrotna sytuacja tzn zadanie ma się uruchamiać między 9 a 14

            //- 8:00 - NIE
            //- 9:15 - TAK
            //- 15:00 - NIE
            //- 1:00 - NIE
            zadanie.MozeDzialacOdGodziny = 9;
            zadanie.MozeDzialacDoGodziny = 14;

            dataDoTestowania = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 8, 0, 0);
            A.CallTo(() => biezacaGodzina.BiezacaData()).Returns(dataDoTestowania);

            Assert.Equal(false, zadanie.CzyPowinnoBycUruchomioneTeraz);

            dataDoTestowania = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 9, 15, 0);
            A.CallTo(() => biezacaGodzina.BiezacaData()).Returns(dataDoTestowania);

            Assert.Equal(true, zadanie.CzyPowinnoBycUruchomioneTeraz);

            dataDoTestowania = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 15, 0, 0);
            A.CallTo(() => biezacaGodzina.BiezacaData()).Returns(dataDoTestowania);

            Assert.Equal(false, zadanie.CzyPowinnoBycUruchomioneTeraz);

            dataDoTestowania = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 1, 0, 0);
            A.CallTo(() => biezacaGodzina.BiezacaData()).Returns(dataDoTestowania);
            Assert.Equal(false, zadanie.CzyPowinnoBycUruchomioneTeraz);


            zadanie.MozeDzialacOdGodziny = 0;
            zadanie.MozeDzialacDoGodziny = 0;

            dataDoTestowania = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 8, 0, 0);
            A.CallTo(() => biezacaGodzina.BiezacaData()).Returns(dataDoTestowania);

            Assert.Equal(true, zadanie.CzyPowinnoBycUruchomioneTeraz);

        }

    }
}
