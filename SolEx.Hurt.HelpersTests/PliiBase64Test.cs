using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model;
using Xunit;

namespace SolEx.Hurt.HelpersTests
{
    public class PliiBase64Test
    {
        [Fact(DisplayName = "Resize foto - do prawdiłowego działania trzeba ustawić scieżke do pliku")]
        public void ResizePhotoTest()
        {
            FileInfo info = new FileInfo("C:\\9120008993560.jpg");
            Plik plik = new Plik
            {
                Data = info.LastWriteTime.ToUniversalTime().AddMilliseconds(-info.LastWriteTime.ToUniversalTime().Millisecond),
                Nazwa = info.Name,
                nazwaLokalna = info.Name,
                Rozmiar = (int)info.Length,
                Sciezka = info.DirectoryName + "\\",
            };
            double rozmiarRzeczywisty = 0;
            string test = PlikiBase64.ResizePhoto(plik, out rozmiarRzeczywisty);

            PlikiBase64.Base64ToFile(test, $"c:\\test.jpg");
        }
    }
}
