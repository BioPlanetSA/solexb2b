using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Xml;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model;
using log4net;
using SolEx.Hurt.Core.Sync;
using SolEx.Hurt.Model.Helpers;


namespace SolEx.Hurt.Sync.App.Modules_.Rozne
{
    public abstract class TrecXmlBaza:SyncModul
    {
      protected static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
      private readonly Dictionary<long, Tlumaczenie> _frazyDoLokalizacji;
       private Dictionary<string, XmlDocument> _xmlTlumaczenia;
       protected Cecha ZrobCeche(string atrybut, string cecha, int atrybutid)
       {
           string symbol = atrybut + ":" + cecha;
           Cecha tmp = new Cecha
           {
               AtrybutId = atrybutid,
               Nazwa = cecha,
               Symbol = symbol,
               Widoczna = true
           };
           tmp.Id = symbol.WygenerujIDObiektu();
           return tmp;
       }
        protected void DodajFraze(string symboljezyka, long obiekt, string pole, string fraza, Type typ)
        {
            int idjezyka = SyncManager.PobierzInstancje.Konfiguracja.JezykiWSystemieSlownikPoSymbolu[symboljezyka].Id;

            Tlumaczenie s = new Tlumaczenie { JezykId = idjezyka, ObiektId = obiekt, Pole = pole, Wpis = fraza, Typ = typ.PobierzOpisTypu() };
            long hash = s.Id;
            if (!_frazyDoLokalizacji.ContainsKey(hash))
            {
                _frazyDoLokalizacji.Add(hash, s);
            }
            else
            {
                _frazyDoLokalizacji[hash] = s;
            }
        }

        protected List<Tlumaczenie> PobierzTlumaczenia()
        {
            return _frazyDoLokalizacji.Values.ToList();
        }
        protected void AktualizacjaTlumaczen()
        {
           Log.Debug("Poczatek aktualizacji tlumaczen");
            //SlownikiSearchCriteria kryteria=new SlownikiSearchCriteria();
            //kryteria.jezyk_id.AddRange(_frazyDoLokalizacji.Values.Select(x => x.jezyk_id).Distinct());
            //kryteria.typ_id.AddRange(_frazyDoLokalizacji.Values.Select(x => x.typ_id).Distinct());
            //IEnumerable<slowniki> b2B = ApiWywolanie.GetSlowniki(kryteria).Values;
            var jezykId = _frazyDoLokalizacji.Values.Select(x => x.JezykId).Distinct();
            var typId = _frazyDoLokalizacji.Values.Select(x => x.Typ).Distinct();
            IEnumerable<Tlumaczenie> b2B = ApiWywolanie.GetSlowniki().Values.Where(x=> jezykId.Contains(x.JezykId) && typId.Contains(x.Typ));
         
            Dictionary<long, Tlumaczenie> tlumaczeniaNaB2B = b2B.ToDictionary(x => x.Id, x => x);
            Dictionary<long, Tlumaczenie> tlumaczeniaErp = _frazyDoLokalizacji.Values.ToDictionary(x => x.Id, x => x);
            HashSet<long> doAktualizacji = new HashSet<long>();
            HashSet<long> doUsuniecia = new HashSet<long>();
            HashSet<long> doDodania = new HashSet<long>();
            tlumaczeniaNaB2B.Porownaj(tlumaczeniaErp, ref doAktualizacji, ref doDodania, ref doUsuniecia);
            if (doDodania.Count > 0)
            {
                List<Tlumaczenie> poziomyDoDodania = tlumaczeniaErp.WhereKeyIsIn(doDodania);
                ApiWywolanie.DodajTlumaczenia(poziomyDoDodania);
            }
            if (doAktualizacji.Count > 0)
            {
                List<Tlumaczenie> poziomyDoAktualizacji = tlumaczeniaErp.WhereKeyIsIn(doAktualizacji);
                ApiWywolanie.DodajTlumaczenia(poziomyDoAktualizacji);
            }
            Log.Debug("Koniec aktualizacji tlumaczen");
        }
        private XmlDocument _xmlZrodlowy;
        protected XmlDocument XmlJezykZrodlowy
        {
            get
            {
                if (_xmlZrodlowy == null)
                {
                    _xmlZrodlowy = PobierzXml(SymbolJezykaZrodlowego);
                }
                return _xmlZrodlowy;
            }
        }
        protected Dictionary<string, XmlNode> ZnajdzWezelDoTlumaczen(XmlNode produkt)
        {
            int id;
            Dictionary<string, XmlNode> wynik = new Dictionary<string, XmlNode>();
            XmlNode xmlid = produkt.SelectSingleNode("id");
            if (xmlid != null && int.TryParse(xmlid.InnerText, out id))
            {
                string xpath = string.Format("/items/item[id='{0}']", id);
                foreach (var jezyki in XmlDoTlumaczen)
                {
                    XmlNode wezel = jezyki.Value.SelectSingleNode(xpath);
                    if (wezel != null)
                    {
                        wynik.Add(jezyki.Key, wezel);
                    }
                }
            }
            return wynik;
        }
        protected Dictionary<string, XmlDocument> XmlDoTlumaczen
        {
            get
            {
                if (_xmlTlumaczenia == null)
                {
                    _xmlTlumaczenia=new Dictionary<string, XmlDocument>();
                    foreach (string s in SymboleJezykowDoTlumaczen)
                    {
                        _xmlTlumaczenia.Add(s,PobierzXml(s));
                    }
                }
                return _xmlTlumaczenia;
            }
        }
        public XmlDocument PobierzXml(string symbolJezyka)
        {
            Log.DebugFormat("Pobieranie xml jezyk {0}",symbolJezyka);
            const string url = "http://www.trec.pl/media/xmlFeedExport/custom/products_{0}.xml ";
            string xml;
            Log.DebugFormat("Pobiera nie xml jezyk {0}",symbolJezyka);
            using (var webClient = new WebClient())
            {
                webClient.Encoding = Encoding.UTF8;

                xml = webClient.DownloadString(String.Format(url,symbolJezyka));
            }
            XmlDocument doc =new XmlDocument();
            doc.LoadXml(xml);
            return doc;
        }
             protected readonly Dictionary<string,string> ZnaneAtrybuty=new Dictionary<string, string>();
             public TrecXmlBaza()
        {
            _frazyDoLokalizacji = new Dictionary<long, Tlumaczenie>();
            ZnaneAtrybuty = new Dictionary<string, string>
            {
                {"size", "/items/item/size"},
                {"gender", "/items/item/gender"},
                {"portions", "/items/item/portions"},
                {"flavour", "/items/item/flavour"}
            };
        }
        public override string uwagi
        {
            get { return "Aby system działał trzeba włączyć wszystkie moduły z nazwą Trec"; }
        }

        private string _zrodlowy;
        protected string SymbolJezykaZrodlowego
        {
            get
            {
                if (String.IsNullOrEmpty(_zrodlowy))
                {
                    foreach (var v in SyncManager.PobierzInstancje.Konfiguracja.JezykiWSystemie)
                    {
                        if (v.Value.Domyslny)
                        {
                            _zrodlowy = v.Value.Symbol;
                        }
                    }
                }
                return _zrodlowy;
            }
        }
        private List<string> _jezykiDoTlumaczen;
        protected List<string> SymboleJezykowDoTlumaczen
        {
            get
            {
                if (_jezykiDoTlumaczen==null)
                {
                    _jezykiDoTlumaczen=new List<string>();
                    foreach (var v in SyncManager.PobierzInstancje.Konfiguracja.JezykiWSystemie)
                    {
                        if (!v.Value.Domyslny)
                        {
                           _jezykiDoTlumaczen.Add(v.Value.Symbol);
                        }
                    }
                }
                return _jezykiDoTlumaczen;
            }
        }
        protected XmlNodeList WezlyProduktow
        {
            get
            {
                XmlNodeList xmlNodeList = XmlJezykZrodlowy.SelectNodes("/items/item");
                if (xmlNodeList != null)
                {
                    return xmlNodeList;
                }
                throw new InvalidOperationException("Brak węzłów");
            }
        }
    }
}
