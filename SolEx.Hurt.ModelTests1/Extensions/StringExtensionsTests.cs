using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
namespace System.Tests
{
    public class StringExtensionsTests
    {
        [Fact(DisplayName = "Testy sprawdzania poprawności adresu email klienta")]
        public void PoprawnyAdresEmailTest()
        {
            //TODO Mateusz w #4924
            bool niepoprawny = "test@".PoprawnyAdresEmail();
            Assert.False(niepoprawny, "adres email powinien być wykryty jako niepoprawny");


            //wg Bartka ten adres jest poprawny (sic!)
            bool poprawny1 = "test@aa.p".PoprawnyAdresEmail();
            Assert.True(poprawny1, "adres email powinien być wykryty jako poprawny");

            bool poprawny2 = "test@aa.pl".PoprawnyAdresEmail();
            Assert.True(poprawny2, "adres email powinien być wykryty jako poprawny");
            
        }
        [Fact(DisplayName = "Testy sprawdzania poprawności usuwania znaków (hex - E281A0")]
        public void PreplaceHexadecimalSymbolsTest()
        {
            //string text = IO.File.ReadAllText(@"C:\znaczek.txt");
            //string wynik = text.ReplaceHexadecimalSymbols();
            //Assert.Equal(wynik, "ul. Ikara 7/27");

            string text = "TP Michał dorzucamy do jutrzejszej dostawy😉";
            string wynik = text.ReplaceHexadecimalSymbols();
            Assert.Equal(wynik, "TP Michał dorzucamy do jutrzejszej dostawy");
        }

        [Fact(DisplayName = "Testy sprawdzania poprawności ZamienZnakKoncaLiniNaWebowy usuwania \\r i \\n")]
        public void ZamienZnakKoncaLiniNaWebowyTest()
        {
            string text = string.Empty;
            string wynik = text.ZamienZnakKoncaLiniNaWebowy();
            Assert.Empty(wynik);
            text = "TP Michał dorzucamy do\n jutrzejszej \r dostawy\r\n";
            wynik = text.ZamienZnakKoncaLiniNaWebowy();
            Assert.Equal(wynik, "TP Michał dorzucamy do<br/> jutrzejszej <br/> dostawy<br/>");
        }
    }
}
