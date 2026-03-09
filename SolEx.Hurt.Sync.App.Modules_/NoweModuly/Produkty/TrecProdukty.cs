using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using Newtonsoft.Json;
using ServiceStack.Common;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Interfaces;
using SolEx.Hurt.Model.Interfaces.Sync;
using SolEx.Hurt.Model.Interfaces.SyncModuly;
using SolEx.Hurt.Sync.App.Modules_.Rozne;
using SolEx.Hurt.Model.Helpers;
using Newtonsoft.Json.Linq;
using SolEx.Hurt.Core.Sync;


namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Produkty
{
   public class TrecProdukty : TrecXmlBaza, IModulProdukty
    {
        public override string Opis
        {
            get { return "Pobiera dane o produktach"; }
        }
        private readonly Dictionary<string,string> _slownikPol=new Dictionary<string, string>();
        
       [FriendlyName("ID grupy, do której należy przypisać kategorie")]
       [WidoczneListaAdminAttribute(false, false, true, false)]
        public int Grupa { get; set; }
        public TrecProdukty()
        {
            _slownikPol=new Dictionary<string, string>
            {
                {"opis2", "description"},
                {"opis", "short_description"},
                {"opis3", "supplement_facts"},
                {"opis4", "dosage"}
                //,{"nazwa", "name"}
            };
        }

        private IConfigSynchro _configbll = SyncManager.PobierzInstancje.Konfiguracja;

       public IConfigSynchro _ConfigBLL
       {
           get { return _configbll; }
           set { _configbll = value; }
       }

       private void UstawPola(Produkt produkterp, XmlNode produkt,  PropertyInfo[] props)
       {
           Dictionary<string, XmlNode> znalezione = ZnajdzWezelDoTlumaczen(produkt);        //do lokalizacji
           foreach (KeyValuePair<string, string> wezel in _slownikPol)
           {
               XmlNode dane = produkt.SelectSingleNode(wezel.Value);
               if (dane != null)
               {
                   PropertyInfo pi = props.FirstOrDefault(x => x.Name == wezel.Key);
                   if (pi != null)
                   {
                       string wartosc = dane.InnerText;
                       if (wezel.Key == "opis3")
                       {
                           wartosc = ZrobOpis(wartosc, _ConfigBLL.JezykiWSystemie.Values.First(x => x.Domyslny).Symbol);
                       }
                       pi.SetValue(produkterp, wartosc, null);
                   }
               }
               foreach (var z in znalezione)
               {
                   XmlNode n = z.Value.SelectSingleNode(wezel.Value);
                   if (n != null)
                   {
                       string wartosc = n.InnerText;
                       if (wezel.Key == "opis3")
                       {
                           wartosc = ZrobOpis(wartosc, z.Key);
                       }
                       DodajFraze(z.Key, (int)produkterp.Id, wezel.Key, wartosc, produkterp.GetType());
                   }
               }
           }
           XmlNode weight = produkt.SelectSingleNode("weight");
           if (weight != null)
           {
               decimal waga;
               if (TextHelper.PobierzInstancje.SprobojSparsowac(weight.InnerText, out waga))
               {
                   produkterp.Waga = waga;
               }
           }
       }

       public void Przetworz(ref List<Produkt> listaWejsciowa, ref List<Tlumaczenie> produktyTlumaczenia, Dictionary<long, Produkt> produktyNaB2B, ref List<JednostkaProduktu> jednostki, ref Dictionary<long, ProduktCecha> lacznikiCech, ref List<ProduktKategoria> lacznikiKategorii, ISyncProvider provider, ref List<ProduktUkryty> produktuUkryteErp, ref List<ProduktyZamienniki> zamienniki, Dictionary<long, KategoriaProduktu> kategorie, ref List<Cecha> cechy, ref List<Atrybut> atrybuty)
        {
           // Dictionary<int, Tuple<string, XmlNode>> doRodzin = new Dictionary<int, Tuple<string, XmlNode>>();//kolekcja do budowy rodzin
            Type typProduktu = typeof (Produkt);
         var props = typProduktu.GetProperties();
                foreach (XmlNode produkt in WezlyProduktow)
                {
                   // XmlNode wezelnazwa = produkt.SelectSingleNode("name");
                    XmlNode wezelKodKReskowy = produkt.SelectSingleNode("sku");
                    XmlNode id = produkt.SelectSingleNode("id");

                      //  int idi = int.Parse(id.InnerText);
                      //  doRodzin.Add(idi,new Tuple<string, XmlNode>( wezelnazwa.InnerText,produkt));
                    if (wezelKodKReskowy != null)//tylko jak jest kod kreskowy to możemy dalej przetwarzać
                    {
                        string kodKreskowy = wezelKodKReskowy.InnerText;
                        Produkt produkterp = listaWejsciowa.FirstOrDefault(x => x.KodKreskowy == kodKreskowy);
                        if (produkterp != null)//jest produkt w erp o tym kodzie kreskowym
                        {
                           
                            UstawPola(produkterp,produkt,props);

                            XmlNode category = produkt.SelectSingleNode("category");//wiązanie z kategorią
                            if (category != null)
                            {
                             //   Log.Debug(string.Format("Mapowanie kategorii{0} proukt {1}", category.InnerText,produkterp.nazwa));
                                KategoriaProduktu nowa = new KategoriaProduktu
                                {
                                    Nazwa = category.InnerText,
                                    Widoczna = true,
                                    GrupaId = Grupa
                                };
                                nowa.Id = nowa.WygenerujIDObiektu();
                                ProduktKategoria nowyLacznik = new ProduktKategoria
                                {
                                    KategoriaId = nowa.Id,
                                    ProduktId = produkterp.Id
                                };
                              
                                lacznikiKategorii.Add(nowyLacznik);
                            }
                            else
                            {
                                Log.Debug(string.Format("Mapowanie kategorii brak produktu {0}", produkterp.Nazwa));
                            }
                            foreach (var zn in ZnaneAtrybuty)
                            {
                                Atrybut a = new Atrybut(zn.Key);
                                a.Id = a.WygenerujIDObiektu();

                                XmlNode c = produkt.SelectSingleNode(zn.Key);
                                if (c == null)
                                {
                                    continue;
                                }
                                string[] cechyZAtrybutu = c.InnerText.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                                foreach (string s in cechyZAtrybutu)
                                {
                                    Cecha tmp = ZrobCeche(a.Nazwa, s, a.Id);
                                    var cp = new ProduktCecha {ProduktId = produkterp.Id, CechaId = tmp.Id};
                                    lacznikiCech.Add(cp.Id, cp);
                                }
                            }
                        }
                    }
                }

                //foreach (XmlNode produkt in WezlyProduktow) //druga pętla możemy ustawiać rodziców
                //{
                //    XmlNode wezelKodKReskowy = produkt.SelectSingleNode("sku");
                //    if (wezelKodKReskowy != null) //tylko jak jest kod kreskowy to możemy dalej przetwarzać
                //    {
                //        string kodKreskowy = wezelKodKReskowy.InnerText;
                //        produkty produkterp = listaWejsciowa.FirstOrDefault(x => x.kod_kreskowy == kodKreskowy);
                //        if (produkterp != null) //jest produkt w erp o tym kodzie kreskowym
                //        {
                //            XmlNode parent_ID = produkt.SelectSingleNode("parent_id");
                //            if (parent_ID != null)//rodzinę ustawiamy tylko jeśli jest ten atrybut
                //            {
                               
                //                int parent;
                //                if (int.TryParse(parent_ID.InnerText, out parent))
                //                {
                                  
                //                    if (doRodzin.ContainsKey(parent))//czy w pierwszej pętli przeravialiśmy rodzica
                //                    {
                //                        produkterp.rodzina = doRodzin[parent].Item1;
                //                        UstawPola(produkterp,doRodzin[parent].Item2,props);
                //                    }
                //                    else
                //                    {
                //                        Log.Debug(string.Format("Produkt {0} nie znaleziono rodzica {1}",produkterp.produkt_id, parent));
                //                    }
                //                    Log.Debug(string.Format("Produkt {0} parent {1} rodzina {2}", produkterp.produkt_id, parent,produkterp.rodzina));
                //                }
                //            }

                //        }
                //    }
                //}
           produktyTlumaczenia.Clear();
                produktyTlumaczenia.AddRange(PobierzTlumaczenia());
          //  AktualizacjaTlumaczen();
        }


        public string ZrobOpis(string tresc,string symboljezyka)
        {
            if (string.IsNullOrEmpty(tresc) || tresc.Length < 5)
            {
                return "";
            }

            try
            {
                int idjezyka = _ConfigBLL.JezykiWSystemieSlownikPoSymbolu[symboljezyka].Id;
                StringBuilder sb = new StringBuilder();
                sb.Append("<table class=\"wartosc-odzywcze table-condensed table-bordered \">");
                JToken o = (JToken)JsonConvert.DeserializeObject(tresc);
                JToken naglowki = o["informacje_ogolne"]["naglowki_tabeli"];
                sb.Append("<thead><tr><td></td>");
                foreach (var n in naglowki)
                {
                    sb.AppendFormat("<td>{0}</td>", _ConfigBLL.PobierzTlumaczenie(idjezyka, n.ToString()));
                }
                sb.Append("</thead></tr>");

                PRzetworzenergetyczne(sb, o["wartosci_energetyczne"],idjezyka);

                PRzetworzodzywcze(sb, o["wartosci_odzywcze"], idjezyka);
                PRzetworzskladniki(sb, o["skladniki"], idjezyka);
                //   PRzetworzskladniki(sb, o["zalecane"]);
                PRzetworzwitaminy(sb, o["witaminy"], idjezyka);
                sb.Append("</table>");

                return sb.ToString();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                LogiFormatki.PobierzInstancje.LogujInfo("Nie udało się sparsować opisu: '{0}' dla jezyka {1} czyszczę opis",tresc,symboljezyka);
                return "";
            }
        }

        private void PRzetworzwitaminy(StringBuilder sb, JToken jToken,int symbol)
        {
            if (jToken == null)
            {
                return;
            }
            List<string> nazwywitamin = new List<string>();
            foreach (var nazwy in jToken["nazwa"])
            {
                nazwywitamin.Add(nazwy.ToString());
            }
            int i = 0;
            if (nazwywitamin.Count != 0)
            {

                sb.AppendFormat("<tr class=\"amionkwasy\"><td >{0}</td></tr>", _ConfigBLL.PobierzTlumaczenie(symbol, "Aminokwasy w porcji dziennej"));
            }
            foreach (var wart in jToken["wartosc"])
            {
                sb.Append("<tr class=\"witaminy\">");
                sb.AppendFormat("<td colspan=\"3\">{0}</td>", _ConfigBLL.PobierzTlumaczenie(symbol, nazwywitamin[i]));
                sb.AppendFormat("<td>{0}<br/>{1} %</td>", wart["minwart"], wart["minproc"]);
                sb.Append("</tr>");
                i++;
            }
        }

        private void PRzetworzskladniki(StringBuilder sbc, JToken token, int symbol)
        {
            if (token == null)
            {
                return;
            }
            StringBuilder skl=new StringBuilder();
            foreach (var n in token)
            {
                StringBuilder sb = new StringBuilder();
                bool pusty = true;
                sb.Append("<tr class=\"skladniki\">");
                sb.AppendFormat("<td>{0}</td>", _ConfigBLL.PobierzTlumaczenie(symbol, n[0].ToString()));//, ConfigBLL.PobierzInstancje.PobierzTlumaczenie(symbol, n[1].ToString()));
                for (int i = 2; i < n.Count(); i++)
                {
                    sb.AppendFormat("<td>{0}</td>", n[i]);
                    if (!string.IsNullOrEmpty(n[i].ToString()))
                    {
                        pusty = false;
                    }
                }
                sb.Append("</tr>");
                if (!pusty)
                {
                    skl.Append(sb);
                }
            }
            if (!string.IsNullOrEmpty(skl.ToString()))
            {
                sbc.AppendFormat("<tr class='aktywne'><td>{0}</td></tr>", _ConfigBLL.PobierzTlumaczenie(symbol, "Składniki aktywne"));
                sbc.Append(skl);
            }
        }

        private void PRzetworzenergetyczne(StringBuilder sbc, JToken token, int symbol)
        {
            if (token == null)
            {
                return;
            }
            List<string> kalorie = new List<string>();
            List<string> kj = new List<string>();
            foreach (var n in token)
            {
                string nazwa= ((JProperty)n).Name;
                if (nazwa == "kcal")
                {
                    foreach (var x in n)
                    {
                        foreach (var el in x)
                        {
                           kalorie.Add(el.ToString());
                        }
                    }
                }
                else if (nazwa == "kj")
                {
                    foreach (var x in n)
                    {
                        foreach (var el in x)
                        {
                            kj.Add(el.ToString());
                        }
                    }
                }
            }
            StringBuilder sbenergetyjka = new StringBuilder();
            sbenergetyjka.AppendFormat("<tr class=\"energetyczne\"><td>{0}</td>", _ConfigBLL.PobierzTlumaczenie(symbol, "Wartość energetyczna"));
            for (int i = 0; i < kalorie.Count && i < kj.Count; i++)
            {
                sbenergetyjka.AppendFormat("<td> {0} kj / {1} kcal </td>", kj[i], kalorie[i]);
            }
            if (kalorie.Any() && kj.Any())
            {
                sbc.AppendFormat("<tr class=\"sekcja\"><td>{0}</td></tr>", _ConfigBLL.PobierzTlumaczenie(symbol, "Wartość odżywcza"));
                sbc.Append(sbenergetyjka);
            }
            sbenergetyjka.Append("</tr>");
        }

        private void PRzetworzodzywcze(StringBuilder sb, JToken token, int symbol)
        {
            if (token == null)
            {
                return;
            }
            foreach (var n in token)
            {
                string name = ((JProperty)n).Name;
                if (name.StartsWith("radio"))
                {
                    continue;
                }
                sb.Append("<tr class=\"odzywcze\">");

                foreach (var x in n)
                {
                    int i = -1;
                    foreach (var el in x)
                    {
                        i++;
                        if (i == 0)
                        {
                            sb.AppendFormat("<td>{0}</td>", _ConfigBLL.PobierzTlumaczenie(symbol, name));//, _ConfigBLL.PobierzTlumaczenie(symbol, el.ToString()));
                            continue;
                        }
                        sb.AppendFormat("<td>{0}</td>", el);

                    }
                }
                sb.Append("</tr>");
            }
        }
    }
}
