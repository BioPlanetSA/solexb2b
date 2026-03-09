using System;
using System.IO;
using SolEx.Hurt.Sync.App.Modules_.NoweModuly.Zamowienia;
using SolEx.Hurt.Sync.App.Modules_.Rozne;
using Xunit;

namespace SolEx.Hurt.Sync.App.Modules_Tests.Rozne
{
    public class ImportZPlikuTests
    {
        [Fact(DisplayName = "Testowanie przenoszenia pliku")]
        public void PrzeniesPlikTest()
        {
            ImportZPliku import = new ComarchConnector();
            string katalog = "C:\\tmptestImport\\";
            string nazwaPliku = "jakisplik.xml";
            string plik = $"{katalog}{nazwaPliku}";
            if(!Directory.Exists(katalog))Directory.CreateDirectory(katalog);
            var tmp = File.Create(plik);
            tmp.Close();

            string katalogarchiwum = "archiwum";
            import.PrzeniesPlik(plik, katalogarchiwum);
            string plikDocelowy = $"{katalog}{katalogarchiwum}\\{DateTime.Now.ToString("yyyy-MM")}\\{nazwaPliku}";
            bool wynik = File.Exists(plikDocelowy);
            
            //usuwanie plików i katakogów po testach
            if(File.Exists(plik))File.Delete(plik);
            if(File.Exists(plikDocelowy)) File.Delete(plikDocelowy);
            if (Directory.Exists($"{katalog}{katalogarchiwum}\\{DateTime.Now.ToString("yyyy-MM")}")) Directory.Delete($"{katalog}{katalogarchiwum}\\{DateTime.Now.ToString("yyyy-MM")}");
            if (Directory.Exists($"{katalog}{katalogarchiwum}")) Directory.Delete($"{katalog}{katalogarchiwum}");
            if (Directory.Exists(katalog)) Directory.Delete(katalog);

            Assert.True(wynik, $"Brak pliku:{plikDocelowy}");
        }
        [Fact(DisplayName = "Testowanie przenoszenia pliku gdy nie ma pliku")]
        public void PrzeniesPlikTest2()
        {
            ImportZPliku import = new ComarchConnector();
            string katalog = "C:\\tmptestImport\\";
            string nazwaPliku = "jakisplik.xml";
            string plik = $"{katalog}{nazwaPliku}";
            if (!Directory.Exists(katalog)) Directory.CreateDirectory(katalog);
            //var tmp = File.Create(plik);
            //tmp.Close();

            string katalogarchiwum = "archiwum";
            bool wynik=false;
            try
            {
                import.PrzeniesPlik(plik, katalogarchiwum);
            }
            catch (FileNotFoundException)
            {
                wynik = true;
            }
           
            //usuwanie plików i katakogów po testach
            if (File.Exists(plik)) File.Delete(plik);
            if (Directory.Exists(katalog)) Directory.Delete(katalog);

            Assert.True(wynik, "Powinien zostać wyrzucony wjątek typu FileNotFoundException");
        }
    }
}
