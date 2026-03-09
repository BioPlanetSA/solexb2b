//using System.Collections.Generic;
//using System.Linq;
//using System.Xml.Linq;
//using SolEx.Hurt.Core.BLL.Interfejsy;
//using SolEx.Hurt.Model.Enums;
//using SolEx.Hurt.Model.Helpers;
//using Produkt = SolEx.Hurt.Model.Produkt;

//namespace SolEx.Hurt.Sync.App.Modules_.Rozne.Stany
//{
//    public class MasitaXML //: IModulyStanyZZewnatrz
//    {
//        public Dictionary<long, decimal> PobierzStanyProduktu(string zawartoscpliku, List<Produkt> produktyB2B)
//        {
//            Dictionary<long, decimal> stany = new Dictionary<long, decimal>();

//            XDocument doc = XDocument.Parse(zawartoscpliku);

//            var elementy = doc.Descendants("Art-Row");

//            foreach (var xElement in elementy)
//            {
//                string kod = xElement.Element("Product_Code").Value;
//                string ilosc = xElement.Element("Available").Value;
//                decimal iloscProduktu;

//                if (!string.IsNullOrEmpty(kod) && TextHelper.PobierzInstancje.SprobojSparsowac(ilosc, out iloscProduktu))
//                {
//                    kod = OczyscKodProduktu(kod);

//                    Produkt produkt = produktyB2B.FirstOrDefault(a => a.Kod == kod);

//                    if (produkt != null && !stany.ContainsKey(produkt.Id))
//                    {
//                        stany.Add(produkt.Id, iloscProduktu);
//                    }
//                }
//            }

//            return stany;
//        }

//        public string OczyscKodProduktu(string kod)
//        {
//            string nowyKod = kod;
//            for (int i = 0; i < 2; i++)
//            {
//                int indexMyslnika = nowyKod.IndexOf('-');
//                if(indexMyslnika > -1)
//                    nowyKod = nowyKod.Remove(indexMyslnika, 1);
//            }
//            return nowyKod.ToUpper().Trim();
//        }

//        public TypPlikuImportStanowZZewnatrz TypPliku => TypPlikuImportStanowZZewnatrz.MasitaXml;

//        //public void PrzetworzProdukty(string zawartoscpliku, List<Produkt> produkty)
//        //{
//        //    XDocument doc = XDocument.Parse(zawartoscpliku);

//        //    var elementy = doc.Descendants("Art-Row");

//        //    foreach (var xElement in elementy)
//        //    {
//        //        string kod = xElement.Element("Product_Code").Value;
//        //        string datadostawy = xElement.Element("Del-date").Value;
                
//        //        if (!string.IsNullOrEmpty(kod))
//        //        {
//        //            kod = OczyscKodProduktu(kod);

//        //            Produkt produkt = produkty.FirstOrDefault(a => a.Kod == kod);

//        //            if (produkt != null)
//        //            {
//        //                produkt.Dostawa = datadostawy;
//        //            }
//        //        }
//        //    }
//        //}
//    }
//}
