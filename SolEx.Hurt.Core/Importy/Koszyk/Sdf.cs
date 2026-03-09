using System.Collections.Generic;
using System.Xml;
namespace SolEx.Hurt.Core.Importy.Koszyk
{
    public class Sdf : PlikiEdifact
    {
        public override string LadnaNazwa
        {
            get { return "Import plików w formacie sdf"; }
        }

        public override List<string> Rozszerzenia
        {
            get { return new List<string> { "sdf" }; }
        }
        protected override string WezlyProduktow
        {
            get { return "/document/purchaseorder/orderposition"; }
        }
        protected override XmlDocument StworzXml(string dane)
        {
          XmlDocument doc=new XmlDocument();
            doc.XmlResolver = null;
          doc.LoadXml(dane);
          return doc;
        }
        protected override string PrzetworzWezel(XmlNode produkt, out string ilosc)
        {      
            ilosc = "";
            string kod = "";
            XmlNode iloscw = produkt.SelectSingleNode("disposition/quantity1");
            if (iloscw != null)
            {
                ilosc = iloscw.InnerText;
            }
            XmlNode kodw = produkt.SelectSingleNode("eancode");
            if (kodw != null)
            {
                kod = kodw.InnerText.TrimStart('0');
            }
            return kod;
        }
    }
}
