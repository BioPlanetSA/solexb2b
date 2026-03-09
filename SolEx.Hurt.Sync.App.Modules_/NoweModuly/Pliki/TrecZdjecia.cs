using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml;
using SolEx.Hurt.Core.Sync;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Interfaces.Sync;
using SolEx.Hurt.Model.Interfaces.SyncModuly;
using SolEx.Hurt.Sync.App.Modules_.Rozne;

using XnaFan.ImageComparison;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Pliki
{
    public class TrecZdjecia : TrecXmlBaza, IModulPliki
    {
        public override string Opis
        {
            get {return "Pobiera zdjęcia produktów do katalogu"; }
        }
        public TrecZdjecia()
        {
            Sciezka = "";
            Separator = "#";
            SeparatorNumer = "_";
            Pole = TypyPolDoDopasowaniaZdjecia.Idproduktu;
        }
        [FriendlyName("Ścieżka do katalogu z plikami - np. c:\\pliki\\")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string Sciezka { get; set; }
        
        [FriendlyName("Pojedyńczy znak który rozdziela nazwe pliku od symbolu produktu - np. jeśli mamy taka nazwe pliku: <b>zdjecie34#SKU123.jpg</b> gdzie kod produktu to SKU123 - naszym separatorem jest znak #")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string Separator { get; set; }
        
        [FriendlyName("Pojedyńczy znak który rozdziela nazwe pliku od numeru zdjecia")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string SeparatorNumer { get; set; }
        
        [FriendlyName("Pole z którego będą brane dane do nazwy pliku")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public TypyPolDoDopasowaniaZdjecia Pole { get; set; }

        protected void PrzeworzZdjecie(Produkt produkterp, List<string> sciezkaZjdeica, List<string> plikiDoKtorychSaZdjecia)
        {
            string bazanazwa = WygenerujNazwe(produkterp);
            int nr = 1;
            foreach (var z in sciezkaZjdeica)
            {


              string  s =bazanazwa+ SeparatorNumer+nr+Path.GetExtension(z);
                string sciezka = Path.Combine(Sciezka, s);
                plikiDoKtorychSaZdjecia.Add(sciezka);
                using (Image plik = PobierzZdjecie(z))
                {
                    if (NalezyZapisac(plik, sciezka))
                    {
                        try
                        {
                            plik.Save(sciezka);
                        }
                        catch (Exception)
                        {
                            Log.Error("Zapis zdjęcia:" + sciezka);
                        }
                    }
                }
                nr++;
            }
        }

        protected void PrzeworzZdjecie(Produkt produkterp, string sciezkaZjdeica, List<string> plikiDoKtorychSaZdjecia)
        {
          PrzeworzZdjecie(produkterp,new List<string>{sciezkaZjdeica},plikiDoKtorychSaZdjecia );
        }
        public virtual void Przetworz(IDictionary<long, Produkt> produktyNaB2B, ref List<ProduktPlik> plikiLokalnePowiazania, ref List<Plik> plikiLokalne, ISyncProvider provider, ref List<Cecha> cechyB2B, ref  List<KategoriaProduktu> kategorieB2B, ref List<Klient> klienciB2B)
        {
            List<string> plikiDoKtorychSaZdjecia=new List<string>();
            Dictionary<int, Tuple<string, XmlNode>> doRodzin = new Dictionary<int, Tuple<string, XmlNode>>();//kolekcja do budowy rodzin
            foreach (XmlNode produkt in WezlyProduktow)
            {
                XmlNode wezelKodKReskowy = produkt.SelectSingleNode("sku");
                XmlNode wezelnazwa = produkt.SelectSingleNode("name");
                XmlNode id = produkt.SelectSingleNode("id");
                if (wezelKodKReskowy != null) //tylko jak jest kod kreskowy to możemy dalej przetwarzać
                {
                    int idi = int.Parse(id.InnerText);
                    doRodzin.Add(idi, new Tuple<string, XmlNode>(wezelnazwa.InnerText, produkt));
                    string kodKreskowy = wezelKodKReskowy.InnerText;
                    Produkt produkterp = produktyNaB2B.Values.FirstOrDefault(x => x.KodKreskowy == kodKreskowy);
                    if (produkterp != null) //jest produkt w erp o tym kodzie kreskowym, tylko dla niego przetwarzamy zdjęcia
                    {
                        XmlNode img_Path = produkt.SelectSingleNode("img_path");
                        if (img_Path != null && !string.IsNullOrEmpty(img_Path.InnerText))
                        {

                            PrzeworzZdjecie(produkterp, img_Path.InnerText, plikiDoKtorychSaZdjecia);
                        }
                    }
                }
            }
            foreach (XmlNode produkt in WezlyProduktow) //druga pętla możemy ustawiać rodziców
            {
                XmlNode wezelKodKReskowy = produkt.SelectSingleNode("sku");
                if (wezelKodKReskowy != null) //tylko jak jest kod kreskowy to możemy dalej przetwarzać
                {
                    string kodKreskowy = wezelKodKReskowy.InnerText;
                    Produkt produkterp = produktyNaB2B.Values.FirstOrDefault(x => x.KodKreskowy == kodKreskowy);
                    if (produkterp != null) //jest produkt w erp o tym kodzie kreskowym
                    {
                        XmlNode parent_ID = produkt.SelectSingleNode("parent_id");
                        if (parent_ID != null)//rodzinę ustawiamy tylko jeśli jest ten atrybut
                        {
                            int parent;
                            if (int.TryParse(parent_ID.InnerText, out parent))
                            {
                                if (doRodzin.ContainsKey(parent))//czy w pierwszej pętli przeravialiśmy rodzica
                                {
                                    XmlNode img_Path = doRodzin[parent].Item2.SelectSingleNode("img_path");
                                    if (img_Path != null && !string.IsNullOrEmpty(img_Path.InnerText))
                                    {
                                        PrzeworzZdjecie(produkterp, img_Path.InnerText, plikiDoKtorychSaZdjecia);
                                    }
                                }
                                else
                                {
                                    Log.Debug(string.Format("Produkt {0} nie znaleziono rodzica {1}", produkterp.Id, parent));
                                }
                                Log.Debug(string.Format("Produkt {0} parent {1} rodzina {2}", produkterp.Id, parent, produkterp.Rodzina));
                            }
                        }

                    }
                }
            }

            LogiFormatki.PobierzInstancje.LogujInfo(string.Format("lokalnych powiązań po przetworzeniu przez moduł TrecZdjecia w ścieżce {0}: {1}", Sciezka, plikiDoKtorychSaZdjecia.Count));

            Usun(plikiDoKtorychSaZdjecia);
        }
        /// <summary>
        /// Usuwa zniepotrzebne zdjecia
        /// </summary>
        /// <param name="plikiZeZdjeciami"></param>
        protected void Usun(List<string> plikiZeZdjeciami)
        {
            string[] zdjecia=  Directory.GetFiles(Sciezka);
            foreach (string s in zdjecia)
            {
                 if (!plikiZeZdjeciami.Contains(s)) //nie jest to zdjecie zadnego z produktow
                 {
                    File.Delete(s);
                 }
            }
        }

        protected bool NalezyZapisac(Image plik, string sciezka)
        {
            if (plik == null) return false;
            if (!File.Exists(sciezka)) return true;//nie ma pliku na dysku, trzeba zapisać
            try
            {


            using (Image plikNaDysku = Image.FromFile(sciezka))
            {
                if (plik.Width != plikNaDysku.Width) return true; //roznia sie szerokoscia, trzeba zapisać
                if (plik.Height != plikNaDysku.Height) return true; //roznia sie wysokoscia, trzeba zapisać
                float roznica = plik.PercentageDifference(plikNaDysku, 0);
                return roznica > 0;
            }
            }
            catch (Exception)
            {
                Log.Error("Błąd porównania" +sciezka);
                File.Delete(sciezka);
                return true;
            }
        }
        protected Image PobierzZdjecie(string url)
        {
            try
            {
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse httpWebReponse = (HttpWebResponse)httpWebRequest.GetResponse();
            Stream stream = httpWebReponse.GetResponseStream();
                if (stream != null)
                {
                    return Image.FromStream(stream);
                }
                return null;
            }
            catch (Exception)
            {
                Log.Error("Pobieranie zdjęcia: "+url);
                return null;
            }
        }
        protected string WygenerujNazwe(Produkt produkterp)
        {
            string s = "";
            switch (Pole)
            {
                case TypyPolDoDopasowaniaZdjecia.Idproduktu:
                    s = produkterp.Id.ToString(CultureInfo.InvariantCulture);
                    break;
                case TypyPolDoDopasowaniaZdjecia.KodKreskowy:
                    s = produkterp.KodKreskowy;
                    break;
                case TypyPolDoDopasowaniaZdjecia.Kod:
                    s = produkterp.Kod;
                    break;
            }
         
            return TextHelper.PobierzInstancje.OczyscNazwePliku(s);
        }
    }
}
