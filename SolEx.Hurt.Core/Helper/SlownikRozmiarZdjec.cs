using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Xml;
using ServiceStack.Common;
using SolEx.Hurt.Core.BLL;

namespace SolEx.Hurt.Core.Helper
{
    public class SlownikRozmiarZdjec : SlownikBazowy
    {
        protected override Dictionary<string, object> WygenerujSlownik
        {
            
            get
            {
                Dictionary<string,object> wynik= new Dictionary<string, object>();
                XmlDocument conf = WczytajDokumentKonfiguracjaRozmiarZdjec();
                XmlElement root = conf.DocumentElement;
                XmlNodeList nodeList = root.GetElementsByTagName("preset");
                string klucz = string.Empty;
                string rozmiar = string.Empty;
                string nazwa = string.Empty;
                string[] tab;
                foreach (XmlNode item in nodeList)
                {
                    if (item.Attributes["name"] != null)
                    {
                        nazwa = item.Attributes["name"].Value;
                        tab = item.Attributes["defaults"] != null ? item.Attributes["defaults"].Value.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries) : item.Attributes["settings"].Value.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                        rozmiar = string.Format("{0}, {1}", tab[0], tab[1]);
                        klucz = string.Format("{0} - ({1})", nazwa, rozmiar);
                        wynik.Add(klucz, nazwa);
                    }
                }
                return wynik;
            }
        }

        private XmlDocument WczytajDokumentKonfiguracjaRozmiarZdjec()
        {
            string plik = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "/static/obrazki_ustawienie.config".TrimStart('/')).Replace("/", "\\");
            if (!File.Exists(plik))
            {
                throw new Exception("Błąd ustawienia SciezkaDoPlikuZRozmiareZdjec - błędna ścieżka");
            }
            XmlDocument doc;
            try
            {
                doc = new XmlDocument();
                doc.Load(plik);
            }
            catch
            {
                throw new Exception("Problem z odczytem pliku");
            }
            return doc;
        }
    }
}
