//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using SolEx.Hurt.Model;
//using SolEx.Hurt.Sync.App.Modules_.Rozne.Stany;
//using Xunit;
//namespace SolEx.Hurt.Sync.App.Modules_.Rozne.Stany.Tests
//{
//    public class MasitaXMLTests
//    {
//        [Fact(DisplayName = "Pobieranie stanów z pliku XML dla Masity")]
//        public void PobierzStanyProduktuTest()
//        {

//            string jakisXML =
//                "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?><Root xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"><Art-Row><Product_Code>114-1510-128</Product_Code><Available>114</Available><Del-date></Del-date><EAN_Code>8717666468311</EAN_Code></Art-Row><Art-Row><Product_Code>114-1510-140</Product_Code><Available>134</Available><Del-date></Del-date><EAN_Code>8717666468328</EAN_Code></Art-Row><Art-Row><Product_Code>6214-4010-g</Product_Code><Available>849</Available><Del-date/><EAN_Code>8717666454376</EAN_Code></Art-Row></Root>";
//            MasitaXML modul = new MasitaXML();

//            //masita ma w subiekcie kody bez myślników, w pliku xml są z myślnikami
//            List<Produkt> listaproduktow = new List<Produkt>() { new Produkt(1) { Kod = "1141510128" }, new Produkt(2) { Kod = "1141510140" }, new Produkt(3) { Kod = "62144010G" } };

//            Dictionary<long, decimal> stany = modul.PobierzStanyProduktu(jakisXML, listaproduktow);

//            Assert.Equal(3, stany.Count);
//            Assert.Equal(114, stany[1]);
//            Assert.Equal(134, stany[2]);
//            Assert.Equal(849, stany[3]);
//        }

//        [Fact(DisplayName = "Oczyszczanie kodu produktu z XML Masity - usuwanie dwóch pierwszych wystąpień znaku -")]
//        public void OczyscKodProduktuTest()
//        {
//            string kod1 = "4020-4000-37-40";
//            string kod1oczekiwany = "4020400037-40";

//            string kod2 = "402040003740";

//            string kod3 = "4020-4000-37-40-80";
//            string kod3oczekiwany = "4020400037-40-80";

//            string kod4 = "6214-4010-g";
//            string kod4oczekiwany = "62144010G";

//            string kod5 = "9096-0-10";
//            string kod5oczekiwany = "9096010";

//            MasitaXML modul = new MasitaXML();
//            string nowyKod1 = modul.OczyscKodProduktu(kod1);

//            Assert.Equal(kod1oczekiwany, nowyKod1);

//            string nowyKod2 = modul.OczyscKodProduktu(kod2);
//            Assert.Equal(kod2, nowyKod2);


//            string nowyKod3 = modul.OczyscKodProduktu(kod3);
//            Assert.Equal(kod3oczekiwany, nowyKod3);

//            string nowyKod4 = modul.OczyscKodProduktu(kod4);
//            Assert.Equal(kod4oczekiwany, nowyKod4);

//            string nowyKod5 = modul.OczyscKodProduktu(kod5);
//            Assert.Equal(kod5oczekiwany, nowyKod5);
//        }
//    }
//}
