using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using SolEx.Hurt.Core.Importy.Model;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Web;

namespace SolEx.Hurt.Core.Importy.Koszyk
{
    public class OptimaXml:ImportBaza
    {
        public override string LadnaNazwa
        {
            get {return "Import w formacie Xml dla Optimy"; }
        }


        public override List<string> Rozszerzenia
        {
            get {return new List<string>{"xml"};}
        }

        public override List<PozycjaKoszykaImportowana> Przetworz(string dane, out List<Komunikat> bledy, Stream stumien)
        {
            bledy = new List<Komunikat>();
            List<PozycjaKoszykaImportowana> wynik = WykonajPrzetwarzanie(dane, bledy);
            return wynik;
        }

        protected List<PozycjaKoszykaImportowana> WykonajPrzetwarzanie(string tekst, List<Komunikat> bledy, string separator = ";")
        {
            tekst = tekst.Replace("xmlns=\"http://www.cdn.com.pl/optima/dokument\"", "");
            List <PozycjaKoszykaImportowana> wynik = new List<PozycjaKoszykaImportowana>();
            XDocument doc;
            try
            {
                doc = XDocument.Parse(tekst);
            }
            catch (Exception ex)
            {
                throw new Exception("Błąd pliku XML" + ex.Message);
            }
            var root = doc.Root;
            if (root == null)
            {
                bledy.Add(new Komunikat("Zły plik xml",
                                        KomunikatRodzaj.danger, GetType().Name + "NieZnaleziono"));
                return wynik;
            }
            var elem = root.Descendants("POZYCJE");
            var xElements = elem as IList<XElement> ?? elem.ToList();
            if (!xElements.Any())
            {
                bledy.Add(new Komunikat("Nie znaleziono elementu pozycje w pliku",
                                         KomunikatRodzaj.danger, GetType().Name + "NieZnaleziono"));
                return wynik;
            }
            var pozycje = xElements.Descendants("POZYCJA");
            var enumerable = pozycje as IList<XElement> ?? pozycje.ToList();
            if (!enumerable.Any())
            {
                bledy.Add(new Komunikat("Nie znaleziono pozycji w pliku",
                                         KomunikatRodzaj.danger, GetType().Name + "NieZnaleziono"));
                return wynik;
            }
            foreach (XElement n in enumerable)
            {
                XElement towar = n.Element("TOWAR");
                if (towar== null) continue;
                XElement eanTmp = towar.Element("EAN");
                XElement kodTmp = towar.Element("KOD");
                string kod = eanTmp!=null ? eanTmp.Value:null;
                if (string.IsNullOrEmpty(kod) && kodTmp!=null)
                {
                    kod = kodTmp.Value;
                }

                XElement iloscTmp = n.Element("ILOSC");
                string ilosc = iloscTmp != null ? iloscTmp.Value.Trim() : null;

                XElement jmTmp = n.Element("JM");
                string jednostka = jmTmp != null ? jmTmp.Value : null;
                Jednostka jednostkaProd = new Jednostka();
                jednostkaProd.Nazwa = jednostka.Trim();
                jednostkaProd.Id = jednostkaProd.Nazwa.WygenerujIDObiektu();

                //if (string.IsNullOrEmpty(ilosc)) continue;
                ZnajdzProdukt(kod, ilosc, n.ToString(), wynik, bledy, jednostkaProd);
                if (ZaDuzoElementow)
                {
                    break;
                }
            }

            return wynik;
        }

    }
}
