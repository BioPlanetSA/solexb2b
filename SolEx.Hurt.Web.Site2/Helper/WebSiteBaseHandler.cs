using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model.Helpers;

namespace SolEx.Hurt.Web.Site2.Helper
{
    public abstract class WebSiteBaseHandler : IHttpHandler 
    {
        private HttpContext Context {get;set;}
        protected virtual bool Resusable
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Wysyła plik do klienta
        /// </summary>
        /// <param name="context">Kontekst wywołania</param>
        /// <param name="path">Scieżka do wysyłanego pliku</param>
        /// <param name="fileName">Nazwa pliku jaki ma zobaczyć klient</param>
        /// <returns></returns>
        public bool SendFile(HttpContext context,string path,string fileName)
        {
            context.Response.ContentEncoding = Encoding.UTF8;
            context.Response.AppendHeader("Content-Disposition", String.Format("attachment;filename=\"{0}\"", fileName));
            context.Response.TransmitFile(path);

            return true;
        }
        public void SendFileFromString(HttpContext context,string data,string nazwa,Encoding kododwanie)
        {
            string typ = Path.GetExtension(nazwa);
            byte[] bity = kododwanie.GetBytes(data);
            byte[] result = kododwanie.GetPreamble().Concat(bity).ToArray();
            string type = Tools.PobierzInstancje.GetMimeType(typ.ToLower());
            context.Response.ContentEncoding = kododwanie;
            context.Response.ContentType = type;
            context.Response.AddHeader("content-disposition", "attachment; filename=\"" + nazwa + "\"");
            context.Response.BinaryWrite(result);
        }
        /// <summary>
        /// Wysyła XML do klienta
        /// </summary>
        /// <param name="context"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool SendXML(HttpContext context, object data, bool minimalizowacXML=false)
        {
            context.Response.ContentEncoding = Encoding.UTF8;
    //           context.Response.ContentType = "text/plain";
            string respJson = JSonHelper.Serialize(data);

            XmlDocument doc = JSonHelper.DeserializeXML(respJson,"root");
            if (minimalizowacXML && doc.ChildNodes[0].ChildNodes.Count>2)
            {
                List<XmlNode> listanowych = new List<XmlNode>();
                for (int i = 0; i < doc.ChildNodes[0].ChildNodes[2].ChildNodes.Count; i++)
                {
                    int kluczID = 0;

                    if (int.TryParse(doc.ChildNodes[0].ChildNodes[2].ChildNodes[i].Name, out kluczID))
                    {
                        XmlNode dozmiany = doc.ChildNodes[0].ChildNodes[2].ChildNodes[i];
                        XmlElement nowy = doc.CreateElement("k" + kluczID);
                        nowy.InnerXml = dozmiany.InnerXml;
                        listanowych.Add(nowy);
                    }
                }

                if (listanowych.Count > 0)
                {
                    doc.ChildNodes[0].ChildNodes[2].RemoveAll();
                    foreach (var xmlNode in listanowych)
                    {
                        doc.ChildNodes[0].ChildNodes[2].AppendChild(xmlNode);
                    }
                }

                XDocument xdoc = XDocument.Parse(doc.OuterXml);

                Dictionary<string,string> zakazaneNazwy = new Dictionary<string, string>();

             //   var elements = xdoc.Descendants("Data").ToList();

             //   xdoc.Element("root").Element("Data").Name = "d";
             //   zakazaneNazwy.Add("Data", "d");

                foreach (XElement e in xdoc.Element("root").Descendants("Data"))
                {
                    string poprzedniaNazwaData = e.Name.LocalName;
                    if (zakazaneNazwy.ContainsKey(poprzedniaNazwaData))
                        e.Name = zakazaneNazwy[poprzedniaNazwaData];
                    else
                    {
                        e.Name = generujNazweSkroconaElementuXML(e.Name.LocalName, zakazaneNazwy);
                        zakazaneNazwy.Add(poprzedniaNazwaData, e.Name.LocalName);
                    }

                    foreach (XElement d in e.Descendants())
                    {
                        string poprzedniaNazwa = d.Name.LocalName;
                        if (zakazaneNazwy.ContainsKey(poprzedniaNazwa))
                            d.Name = zakazaneNazwy[poprzedniaNazwa];
                        else
                        {
                            d.Name = generujNazweSkroconaElementuXML(d.Name.LocalName, zakazaneNazwy);
                            zakazaneNazwy.Add(poprzedniaNazwa, d.Name.LocalName);
                        }
                    }
                }

                context.Response.Write((string) xdoc.ToString());
                return true;
            }
            context.Response.Write((string) doc.OuterXml);
            return true;
        }

        private string generujNazweSkroconaElementuXML(string nazwaElementu, Dictionary<string,string> zakazaneNazwy)
        {
            string znalezionaNazwa = string.Empty;
            bool poprawna = false;
            int i = 0;
            while (!poprawna)
            {
                if (i == nazwaElementu.Length - 1)
                    return znalezionaNazwa;

                znalezionaNazwa += nazwaElementu[i++];
                if (!zakazaneNazwy.ContainsValue(znalezionaNazwa))
                {
                    poprawna = true;
                }
            }
            return znalezionaNazwa;
        }

        /// <summary>
        /// Wysyła Jsona do klienta
        /// </summary>
        /// <param name="context">Kontekst wywołania</param>
        /// <param name="data">Dane do wysłania</param>
        /// <returns></returns>
        public bool SendJson(HttpContext context, object data)
        {
            context.Response.ContentEncoding = Encoding.UTF8;
            context.Response.ContentType = "application/json";

            string resp = JSonHelper.Serialize(data);
           
            context.Response.Filter = new System.IO.Compression.GZipStream(context.Response.Filter, System.IO.Compression.CompressionMode.Compress);
            context.Response.AppendHeader("Content-Encoding", "gzip");

            context.Response.Write(resp);
            return true;
        }
        /// <summary>
        /// Wysyła strumień danych do klienta
        /// </summary>
        /// <param name="context">Kontekst wywołania</param>
        /// <param name="data">Strumień danych</param>
        /// <param name="contentType">Typ danych</param>
        /// <returns></returns>
        public bool SendStream(HttpContext context, Stream data, string contentType)
        {
            context.Response.ContentEncoding = Encoding.UTF8;
            context.Response.ContentType = contentType;
            byte[] buffer = null;

            buffer = new byte[data.Length];
            data.Read(buffer, 0, buffer.Length);
         //   context.Response.AddHeader("Content-Disposition", "filename=" + reportName + ".rpt");
            context.Response.AddHeader("Content-Length", buffer.Length.ToString());
            context.Response.BinaryWrite(buffer);return true;
        }

        public void SendText(HttpContext context, string zawartoscPliku, string contentType)
        {
             context.Response.ContentEncoding = Encoding.UTF8;
            context.Response.ContentType = contentType;
            //context.Response.AddHeader("Content-Length", zawartoscPliku.Length.ToString());
            context.Response.Write(RemoveTroublesomeCharacters(zawartoscPliku));
        }
        /// <summary>
        /// Usuwa błędne znakich z stringa
        /// </summary>
        /// <param name="inString"></param>
        /// <returns></returns>
        public static string RemoveTroublesomeCharacters(string inString)
        {
            if (inString == null) return null;

            StringBuilder newString = new StringBuilder(inString.Length);
            char ch;

            for (int i = 0; i < inString.Length; i++)
            {
                ch = inString[i];
                // remove any characters outside the valid UTF-8 range as well as all control characters
                // except tabs and new lines
                if ( (ch < 383 && ch > 0x001F) || ch==10 || ch==13 )
                {
                    newString.Append(ch);
                }
            }
            return newString.ToString();
        }
        public bool IsReusable
        {
            get { return Resusable; }
        }

        public void ProcessRequest(HttpContext context)
        {
            Context = context;
            context.Response.Clear();
            HandleRequest(context);
        }
        public abstract void HandleRequest(HttpContext context);
    }
}
