//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using CsvHelper;
//using SolEx.Hurt.Core.BLL.Interfejsy;
//using SolEx.Hurt.Model.Enums;
//using SolEx.Hurt.Model.Helpers;
//using Produkt = SolEx.Hurt.Model.Produkt;

//namespace SolEx.Hurt.Sync.App.Modules_.Rozne.Stany
//{
//    public class Csv : IModulyStanyZZewnatrz
//    {
//        public Dictionary<long, decimal> PobierzStanyProduktu(string zawartoscpliku, List<Produkt> produktyB2B)
//        {
//            Dictionary<long, decimal> stany = new Dictionary<long, decimal>();
            
//            CsvReader csv = new CsvReader(new StringReader(zawartoscpliku));
//            csv.Configuration.Delimiter = ";";
//            csv.Configuration.HasHeaderRecord = true;
//            while (csv.Read()) //petla wyciagajaca naglowki kolumna
//            {
//                string kod = csv[0];
//                string stan = csv[1];
//                decimal iloscProduktu;
//                if (string.IsNullOrEmpty(stan)) continue;
//                if (!string.IsNullOrEmpty(kod) && TextHelper.PobierzInstancje.SprobojSparsowac(stan, out iloscProduktu))
//                {
//                    Produkt produkt = produktyB2B.FirstOrDefault(a => a.Kod == kod);

//                    if (produkt != null && !stany.ContainsKey(produkt.Id))
//                    {
//                        stany.Add(produkt.Id, iloscProduktu);
//                    }
//                }
//            }
//            return stany;
//        }
//        public TypPlikuImportStanowZZewnatrz TypPliku => TypPlikuImportStanowZZewnatrz.Csv;

       
//    }
//}
