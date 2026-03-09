using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Interfaces.SyncModuly;
using SolEx.Hurt.Sync.App.Modules_.Rozne;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.CechyIAtrybuty
{
    public class TrecCechy : TrecXmlBaza, IModulCechyIAtrybuty
    {
        public override string Opis
        {
            get { return "Pobiaranie cech"; }
        }
   
        public void Przetworz(ref List<Atrybut> atrybuty, ref List<Cecha> cechy, Dictionary<long, Produkt> produktyNaB2B)
        {
                foreach (XmlNode produkt in WezlyProduktow)
                {
                    Dictionary<string, XmlNode> znalezione = ZnajdzWezelDoTlumaczen(produkt);     
                    foreach (var zn in ZnaneAtrybuty)
                    {
                        Atrybut a = new Atrybut(zn.Key);
                        a.Id = a.WygenerujIDObiektu();
                        if (atrybuty.All(x => x.Id != a.Id))
                        {
                            atrybuty.Add(a);
                        }
                        XmlNode cechyAtrybutu = produkt.SelectSingleNode(zn.Key);
                        if (cechyAtrybutu != null)
                        {

                            string[] cechyZAtrybutu = cechyAtrybutu.InnerText.Split(new[] { "," },
                                                                            StringSplitOptions.RemoveEmptyEntries);
                            List<long> ids=new List<long>();
                                foreach (string s in cechyZAtrybutu)
                                {
                                    Cecha tmp = ZrobCeche(a.Nazwa, s, a.Id);
                                    
                                    ids.Add(tmp.Id);
                                    if (cechy.All(x => x.Id != tmp.Id))
                                    {
                                        cechy.Add(tmp);
                                    }
                                }
                                foreach (var z in znalezione)
                                {
                                    XmlNode n = z.Value.SelectSingleNode(zn.Key);
                                    if (n != null)
                                    {
                                        string[] frazy = cechyAtrybutu.InnerText.Split(new[] { "," },
                                                                           StringSplitOptions.RemoveEmptyEntries);
                                        for (int i = 0; i < frazy.Length && i<ids.Count; i++)
                                        {
                                            DodajFraze(z.Key, ids[i], "nazwa", frazy[i], typeof(Cecha));
                                        }
                                        
                                    }
                                }
                        }
                    }
                }
                AktualizacjaTlumaczen();
            }

       
        }
}
