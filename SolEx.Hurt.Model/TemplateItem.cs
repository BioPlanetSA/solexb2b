using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SolEx.Hurt.Model
{
    /// <summary>
    /// Model elementu szablonu katalogowego, wykorzystywany podczas drukowania oferty detalicznej
    /// </summary>
    public class TemplateItem
    {
        public int ProductId { get; set; }
        public string ProductSymbol { get; set; }
        public string ProductBarCode { get; set; }
        public string ProductName { get; set; }
        public Plik ProductImage { get; set; }
        public string Desc { get; set; }
        public string DescShort { get; set; }
        
        public string QuantityUnit { get; set; }
        public string InStockText { get; set; }
        public decimal InStock { get; set; }
        public decimal InBox { get; set; }
        public string InPaletteText { get; set; }

        public string PriceText { get; set; }
        public decimal PriceNetto { get; set; }
        public decimal PriceBrutto { get; set; }
        public decimal PriceDetalNetto { get; set; }
        public decimal PriceDetalBrutto { get; set; }
        public string Currency { get; set; }
        public string CurrencySymbol { get; set; }
        public string BrandName { get; set; }
        public Plik BrandImage { get; set; }
        public string CategoryName { get; set; }
        public string CategoryWWW { get; set; }

        public string Param1 { get; set; }
        public string Param2 { get; set; }
        public string Param3 { get; set; }
        public string Param4 { get; set; }
        public string Param5 { get; set; }
        public string Param6 { get; set; }
        public string Param7 { get; set; }
        public string Param8 { get; set; }
        public string Param9 { get; set; }
        public string Param10 { get; set; }

        public string GetCategoryLevel(int i)
        {
            if (string.IsNullOrEmpty(CategoryName))
            {
                return "";
            }

            var list = CategoryName.Split('/');
            if (i > list.Length - 1)
            {
                return "";
            }

            string cat = list[i].Trim();
            if (cat.IndexOf('(') != -1)
            {
                cat = cat.Remove(cat.IndexOf('('), cat.IndexOf(')') - cat.IndexOf('(') + 1 );
            }

            return cat.ToUpper();
        }
        public string PKWiU { get; set; }

        public decimal InBoxMin { get; set; }
        public decimal VAT { get; set; }

        public decimal Discount { get; set; }
    }
}

