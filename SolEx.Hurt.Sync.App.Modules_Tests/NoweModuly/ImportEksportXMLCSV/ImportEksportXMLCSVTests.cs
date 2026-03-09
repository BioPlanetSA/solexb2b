using System;
using System.Collections.Generic;
using System.IO;
using FakeItEasy;
using log4net;
using Xunit;
namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.ImportEksportXMLCSV.Tests
{
    public class ImportEksportXMLCSVTests
    {
        [Fact(DisplayName = "Test sprawdzajacy czy zostana wyłapane blędy")]
        public void PrzetworzTest()
        {
            string SciezkaLokalnaImport = "jakas";
            
            var logi = A.Fake<ILog>();
            A.CallTo(() => logi.Error("Nie podano ID szablonu!")).Throws(() => new NullReferenceException());
            A.CallTo(() => logi.Error("Nie podano ścieżki lokalnej importu")).Throws(() => new NullReferenceException());
            A.CallTo(() =>logi.Error(string.Format("Podana ścieżka lokalna importu nie istnieje. Ścieżka- {0}",SciezkaLokalnaImport))).Throws(() => new DirectoryNotFoundException());


            ImportEksportXMLCSV imp1 = new ImportEksportXMLCSV() { SciezkaLokalnaImport = SciezkaLokalnaImport, IDszablonu = "1"};
      

            ImportEksportXMLCSV imp2 = new ImportEksportXMLCSV() { IDszablonu = "1" };
   

            ImportEksportXMLCSV imp3 = new ImportEksportXMLCSV() { SciezkaLokalnaImport = SciezkaLokalnaImport};
       

            try
            {
                imp1.Przetworz();
                Assert.False(true, "Błąd ściezki");
            }
            catch (DirectoryNotFoundException ) { }
            try
            {
                imp2.Przetworz();
                Assert.False(true, "Nie podano ścizki");
            }
            catch (NullReferenceException ) { }
            try
            {
                imp3.Przetworz();
                Assert.False(true, "Nie podano id szablonu");
            }
            catch (NullReferenceException) { }
        }
    }
}
