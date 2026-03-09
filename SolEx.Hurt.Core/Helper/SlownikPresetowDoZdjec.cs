using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using ServiceStack.Messaging.Rcon;
using ServiceStack.Text;
using SolEx.Hurt.Core.BLL;

namespace SolEx.Hurt.Core.Helper
{
    public class SlownikPresetowDoZdjec : SlownikBazowy
    {
        protected override Dictionary<string, object> WygenerujSlownik
        {
            get
            {
                var sciezka = HttpContext.Current.Server.MapPath(@"~/static");
                var plikxml = Path.Combine(sciezka, "obrazki_ustawienie.config");
                List<string> lista = new List<string>();
                if (File.Exists(plikxml))
                {
                    string stream = File.ReadAllText(plikxml, Encoding.UTF8);

                    using (XmlReader reader = new XmlTextReader(new StringReader(stream)))
                    {
                        while (reader.Read())
                        {
                            if (reader.Name.Equals("preset"))
                            {
                                var name = reader.GetAttribute("name");
                                lista.Add(name);
                            }
                        }
                    }
                }

                Dictionary<string, object> wynik = new Dictionary<string, object>();
                foreach (var k in lista)
                {
                    wynik.Add(k,k);
                }
                return wynik;
            }
        }
    }
}
