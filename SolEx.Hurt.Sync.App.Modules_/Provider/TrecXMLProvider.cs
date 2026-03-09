using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Xml.Linq;
using SolEx.Hurt.Model;
using System.Drawing;

namespace SolEx.Hurt.Sync.App.Modules_.Provider
{
    public class TrecXMLProvider : IImportDataModule
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public static string PobierzXMLZeStrony(string adres)
        {
            if (string.IsNullOrEmpty(adres))
            {
                return "";
            }
            try
            {
                // used to build entire input
                StringBuilder sb = new StringBuilder();
                byte[] buf = new byte[8192];

                // prepare the web page we will be asking for
                HttpWebRequest request = (HttpWebRequest)
                    WebRequest.Create(adres);

                CookieContainer cookieContainer = new CookieContainer();
                request.CookieContainer = cookieContainer;

                // execute the request
                HttpWebResponse response = (HttpWebResponse)
                    request.GetResponse();

                // we will read data via the response stream
                Stream resStream = response.GetResponseStream();

                string tempString = null;
                int count = 0;

                do
                {
                    // fill the buffer with data
                    count = resStream.Read(buf, 0, buf.Length);

                    // make sure we read some data
                    if (count != 0)
                    {
                        // translate from bytes to ASCII text
                        tempString = Encoding.UTF8.GetString(buf, 0, count);
                        //tempString = Encoding.ASCII.GetString(buf, 0, count);

                        // continue building the string
                        sb.Append(tempString);
                    }
                }
                while (count > 0); // any more data to read?

                // print out page source
                return sb.ToString();
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                log.Error(ex.StackTrace);
            }
            return "";
        }


        public void DoWork(System.Collections.Specialized.NameValueCollection configuration, Model.SourceDB db)
        {
            string img_dir = configuration["TrecXML_img_dir"];
            string[] langs = configuration["TrecXML_langs"].Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
            db.SourceCategories.Clear();
            try
            {
                for (int i = 0; i < langs.Length; i++)
                {
                    string towary = PobierzXMLZeStrony(string.Format("http://katalog.trec.pl/katalog/export/xml/lng/{0}.xml", langs[i]));
                    XElement towaryXML = XElement.Parse(towary);
                    var p = towaryXML.Descendants("product").ToList();
                    foreach (var prod in p)
                    {
                        string barcode = prod.Attribute("code").Value;
                        int id = Math.Abs(barcode.GetHashCode());
                        produkty item = db.Products.FirstOrDefault(pr => pr.produkt_id == id);
                        //id wygenerowany z hasha powinno być unikalne, jeśli się powtórzy to trzeba bezwględnie przerwać działanie
                        if (item != null)
                        {
                            throw new Exception(string.Format("Produkt o kodzie kreskowym {0} otrzymał takie samo ID = {1}", barcode, id));
                        }

                        else item = new produkty();

                        if (i == 0) //język zrodlowy
                        {
                            item.produkt_id = id;
                            item.rodzina = prod.Element("name").Value;
                            item.nazwa = item.rodzina;
                            item.kod = barcode;
                            item.kod_kreskowy = barcode;
                            item.opis = prod.Element("details").Value;
                            item.opis_krotki = prod.Element("short").Value;
                            item.widoczny = true;
                            var cat = prod.Element("category");
                            int cid = 10000 + int.Parse(cat.Attribute("cid").Value);
                            item.CategoryIds.Clear();
                            item.CategoryIds.Add(cid);
                            if (!db.SourceCategories.Any(x => x.id == cid)) //kategoria zrodlowa
                            {
                                Category tmp = new Category();
                                tmp.id = cid;
                                tmp.nazwa = "kat_" + cat.Value;
                                db.SourceCategories.Add(tmp);
                            }
                            var img = prod.Element("img"); //zdjecie
                            if (img != null)
                            {
                                string img_src = img.Attribute("src").Value;
                                string ext = "png";
                                string fileName = String.Format("{0}\\{1}.{2}", img_dir, item.kod, ext);
                                if (!File.Exists(fileName) && !string.IsNullOrEmpty(img_src))
                                {
                                    Image img_d = DownloadImage(img_src);
                                    img_d.Save(fileName);
                                }
                            }
                            var traits = prod.Element("entities").Elements(); //cechy
                            foreach (var tra in traits)
                            {
                                int atr_id = int.Parse(tra.Attribute("eid").Value);
                                string atr_name = tra.Attribute("name").Value;
                                string trait_name = tra.Value;

                                if (string.IsNullOrEmpty(trait_name))
                                    continue;
                                if (atr_name == "smak" && trait_name == "0")
                                    continue;

                                //if (atr_name == "opakowanie")
                                //    item.FamilyName += " " + trait_name;

                                if (atr_name == "gramatura")
                                    //w razie jak by nazwa zawierała cechę np bez spacji czy coś
                                    if (!item.rodzina.Replace(" ", "").Contains(trait_name.Replace(" ", "")))
                                        item.nazwa = item.rodzina += " " + trait_name;

                                AddAttribute(db, item, atr_id, atr_name, trait_name);
                            }

                            db.Products.Add(item);
                        }
                        else
                        {
                            //string langn = langs[i];
                            //jezyki l = new jezyki(langn);
                            //l.FieldName = "name";
                            //l.symbol = langn;
                            //l.TypeName = "produkt";
                            //l.Value = prod.Element("name").Value;
                            //item.Languages.Add(l);
                            //jezyki l2 = new jezyki(langn);
                            //l2.FieldName = "desc";
                            //l2.symbol = langn;
                            //l2.TypeName = "produkt";
                            //l2.Value = prod.Element("details").Value;
                            //item.Languages.Add(l2);
                            //jezyki l3 = new jezyki(langn);
                            //l3.FieldName = "desc-short";
                            //l3.symbol = langn;
                            //l3.TypeName = "produkt";
                            //l3.Value = prod.Element("short").Value;
                            //item.Languages.Add(l3);
                        }
                    }

                    var fam = db.Products.Where(a => !a.AttributeSymbols.Exists(b => b.StartsWith("smak")) && a.AttributeSymbols.Exists(b => b.StartsWith("gramatura")));

                    foreach (var f in fam)
                    {
                        string gramatura = f.AttributeSymbols.FirstOrDefault(a => a.StartsWith("gramatura"));

                        if (gramatura != null)
                        {
                            string trait_name = gramatura.Split('_').Last();
                            f.rodzina = f.rodzina.Replace(trait_name, "").Trim();
                            AddAttribute(db, f, -327989, "gramatura bez smaku", trait_name);
                        }
                    }

                    foreach (var v in db.Products.GroupBy(pr => pr.rodzina).Where(pr => !string.IsNullOrEmpty(pr.Key)))
                    {
                        var o = v.FirstOrDefault(pr => !string.IsNullOrEmpty(pr.opis) && pr.Pictures.Count > 0);

                        if (o == null)
                            o = v.FirstOrDefault(pr => pr.Pictures.Count > 0);

                        if (o == null)
                            o = v.FirstOrDefault();

                        o.ojciec = true;

                        foreach (var z in v.Where(pr => pr.ojciec == false && string.IsNullOrEmpty(pr.opis)))
                        {
                            z.opis = o.opis;
                        }
                    }

                }
            }
            catch (Exception ex) { log.Error(ex.Message + "__" + ex.StackTrace); }
        }

        private static void AddAttribute(Model.SourceDB db, produkty item, int atr_id, string atr_name, string trait_name)
        {
            string trais_symbol = atr_name + "_" + trait_name;
            item.AttributeSymbols.Add(trais_symbol);
            if (!db.Attributes.Any(pr => pr.symbol == trais_symbol))
            {
                cechy trait = new cechy();
                trait.symbol = trais_symbol;
                trait.nazwa = trait_name;
                trait.cecha_id = 5000 + atr_id;
                trait.nazwa = atr_name;
                db.Attributes.Add(trait);
            }
        }
        public Image DownloadImage(string _URL)
        {
            Image _tmpImage = null;

            try
            {
                // Open a connection
                System.Net.HttpWebRequest _HttpWebRequest = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(_URL);

                _HttpWebRequest.AllowWriteStreamBuffering = true;

                // You can also specify additional header values like the user agent or the referer: (Optional)
                _HttpWebRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1)";
                _HttpWebRequest.Referer = "http://www.google.com/";

                // set timeout for 20 seconds (Optional)
                _HttpWebRequest.Timeout = 20000;

                // Request response:
                System.Net.WebResponse _WebResponse = _HttpWebRequest.GetResponse();

                // Open data stream:
                System.IO.Stream _WebStream = _WebResponse.GetResponseStream();

                // convert webstream to image
                _tmpImage = Image.FromStream(_WebStream);

                // Cleanup
                _WebResponse.Close();
                _WebResponse.Close();
            }
            catch (Exception _Exception)
            {
                // Error
                Console.WriteLine("Exception caught in process: {0}", _Exception.ToString());
                return null;
            }

            return _tmpImage;
        }
        public event ProgressChangedEventHandler ProgresChanged;
    }
}
