using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SolEx.Hurt.Model;
using Xunit;
namespace System.Tests
{
    public class ListExtensionTests
    {
        [Fact()]
        public void ZerowaniePolZPustymiStringamiTest()
        {
            Produkt p = new Produkt() {KodKreskowy = "", Kod = "a"};
            List<PropertyInfo> listaStringowychPol = typeof(Produkt).GetProperties().Where(x => x.PropertyType == typeof(string) && x.CanRead && x.CanWrite).ToList();
            IList<Produkt> produkty = new List<Produkt>();
            produkty.Add(p);
            produkty.ZerowaniePolZPustymiStringami(listaStringowychPol);
            Assert.NotNull(p.Kod);
            Assert.Null(p.KodKreskowy);
        }

        [Fact()]
        public void aaaa()
        {
            Klient k = new Klient(null);
            Dictionary<long, Klient> lista1 = new Dictionary<long, Klient>();
            Klient k1 = new Klient() { Id = 1, Nazwa = "k1", Email = "k1@k1.pl", Symbol = "k2sl1" };
            Klient k2 = new Klient() { Id = 2, Nazwa = "k2", Email = "k2@k2.pl", Symbol = "k2sl2" };
            lista1.Add(k1.Id, k1);
            lista1.Add(k2.Id, k2);



            Dictionary<long, Klient> lista2 = new Dictionary<long, Klient>();
            Klient k3 = new Klient() { Id = 1, Nazwa = "k3", Email = "k3@k3.pl", Symbol = "k1s" };
            Klient k4 = new Klient() { Id = 3, Nazwa = "k4", Email = "k4@k4.pl", Symbol = "k2s" };
            lista2.Add(k3.Id, k3);
            lista2.Add(k4.Id, k4);


            lista2.KopiujPolaIstniejaceObiekty(lista1,new {email = k.Email, k.Nazwa});

            Assert.True(lista2.Values.First().Email == "k3@k3.pl");
            Assert.True(lista2.Values.First().Symbol == "k1s");
            Assert.True(lista2.Values.Last().Email == "k4@k4.pl");
            Assert.True(lista2.Values.Last().Symbol == "k2s");
        }

        [Fact()]
        public void ZerowaniePolStringZeZnakowSpecjalnychTest()
        {
            string teks1 = ConvertHexToString("0c086162630b", System.Text.Encoding.ASCII);
            string teks2 = ConvertHexToString("610b0062", System.Text.Encoding.ASCII);
            string teks3 = ConvertHexToString("63620c00611b", System.Text.Encoding.ASCII);


            var test = ConvertStringToHex("abc", System.Text.Encoding.ASCII);
            Produkt p1 = new Produkt() { KodKreskowy = "&kod_kreskowy&", Kod = "", Dostawa = null };
            Produkt p2 = new Produkt();
            p2.KodKreskowy = teks1;
            p2.Kod = teks2;
            p2.Dostawa = teks3;

            List<PropertyInfo> listaStringowychPol = typeof(Produkt).GetProperties().Where(x => x.PropertyType == typeof(string) && x.CanRead && x.CanWrite).ToList();
            IList<Produkt> produkty = new List<Produkt>();
            produkty.Add(p1);
            produkty.Add(p2);
            produkty.ZerowaniePolStringZeZnakowSpecjalnych(listaStringowychPol);
            
            Assert.True(p1.Kod == "");
            Assert.True(p1.KodKreskowy == "&kod_kreskowy&");
            Assert.True(p1.Dostawa == null);

            Assert.True(p2.KodKreskowy == "abc");
            Assert.True(p2.Kod == "ab");
            Assert.True(p2.Dostawa == "cba");
        }

        public static string ConvertStringToHex(String input, System.Text.Encoding encoding)
        {
            Byte[] stringBytes = encoding.GetBytes(input);
            StringBuilder sbBytes = new StringBuilder(stringBytes.Length * 2);
            foreach (byte b in stringBytes)
            {
                sbBytes.AppendFormat("{0:X2}", b);
            }
            return sbBytes.ToString();
        }

        public static string ConvertHexToString(String hexInput, Encoding encoding)
        {
            int numberChars = hexInput.Length;
            byte[] bytes = new byte[numberChars / 2];
            for (int i = 0; i < numberChars; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hexInput.Substring(i, 2), 16);
            }
            return encoding.GetString(bytes);
        }
    }
}
