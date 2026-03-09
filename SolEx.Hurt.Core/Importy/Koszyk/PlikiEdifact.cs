using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using EDIFACT;
using SolEx.Hurt.Core.Importy.Model;
using SolEx.Hurt.Model.Web;

namespace SolEx.Hurt.Core.Importy.Koszyk
{
    public class PlikiEdifact : ImportBaza
    {
        public override string LadnaNazwa
        {
            get { return "Import plików w formacie edifact"; }
        }

        public override List<string> Rozszerzenia
        {
            get { return new List<string> { "edifact" }; }
        }

        protected virtual string WezlyProduktow
        {
            get { return "/*[local-name()='D96AORDERS']/*[local-name()='ORDERS']/*[local-name()='GRP25']/*[local-name()='LIN']"; }
        }
        protected virtual XmlDocument StworzXml(string dane)
        {
            EDIMessage msg = new EDIMessage(dane, true);
            return msg.SerializeToXml().First(); ;
        }
        public override List<PozycjaKoszykaImportowana> Przetworz(string dane, out List<Komunikat> bledy, Stream stumien)
        {
            bledy=new List<Komunikat>();
            List<PozycjaKoszykaImportowana> wynik=new List<PozycjaKoszykaImportowana>();

            XmlDocument xDoc = StworzXml(dane);
            XmlNodeList produkty = xDoc.SelectNodes(WezlyProduktow);
            if (produkty != null)
            {
                foreach (XmlNode produkt in produkty)
                {
                    if (produkt == null) continue;
                    string ilosc;
                  string kod=  PrzetworzWezel(produkt,out ilosc);
                  if (string.IsNullOrEmpty(kod) || string.IsNullOrEmpty(ilosc))
                  {
                      bledy.Add(new Komunikat("Nie znaleziono ilości lub kodu kreskowego, węzeł: " + produkt.InnerXml, KomunikatRodzaj.danger, GetType().Name));
                      continue;
                  }
                  ZnajdzProdukt(kod, ilosc, produkt.InnerXml, wynik, bledy);
                    if (ZaDuzoElementow)
                    {
                        break;
                    }
                }
            }
            return wynik;
        }
        protected virtual string PrzetworzWezel(XmlNode produkt,out string ilosc)
        {
            ilosc = "";
            XmlNode wezelIlosc = produkt.SelectSingleNode("*[local-name()='QTY']");
            if (produkt.Attributes != null)
            {
                XmlAttribute kodKreskowy = produkt.Attributes["itemNumber"];
                if (wezelIlosc != null)
                {
                    if (wezelIlosc.Attributes != null)
                    {
                        XmlAttribute iloscStr = wezelIlosc.Attributes["quantity"];
                        if (iloscStr == null || kodKreskowy == null)
                        {
                            return "";
                        }
                        ilosc = iloscStr.Value;
                        return kodKreskowy.Value;
                    }
                }
            }
            return "";
        }
    }
}
