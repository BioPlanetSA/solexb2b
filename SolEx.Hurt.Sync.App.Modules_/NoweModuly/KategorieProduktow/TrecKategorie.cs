using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Interfaces.Sync;
using SolEx.Hurt.Model.Interfaces.SyncModuly;
using SolEx.Hurt.Sync.App.Modules_.Rozne;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.KategorieProduktow
{
   public class TrecKategorie : TrecXmlBaza, IModulKategorieProduktow
    {
       public void Przetworz(ref Dictionary<long, KategoriaProduktu> listaWejsciowa, Dictionary<long, KategoriaProduktu> listaKategoriiB2B, ISyncProvider provider, List<Grupa> grupyPRoduktow)
       {
           const string nazwaWezla = "category";
           Grupa grupa = grupyPRoduktow.FirstOrDefault(x => x.Id == Grupa);
            if (grupa == null)
            {
                throw new InvalidOperationException("Brak grupy produktów o nazwie "+Grupa);
            }
            List<long> kluczeDoWywalenia = new List<long>();
            foreach (var kategory in listaWejsciowa)//usuwanie kategorii z grupy do której wpiszemy nowe kategorie
            {
                if (kategory.Value.GrupaId == grupa.Id)
                {
                    kluczeDoWywalenia.Add(kategory.Key);
                }
            }
            foreach (int i in kluczeDoWywalenia)
            {
                listaWejsciowa.Remove(i);
            }
                foreach (XmlNode produkt in WezlyProduktow)
                {
                    Dictionary<string, XmlNode> znalezione = ZnajdzWezelDoTlumaczen(produkt);                 
                    XmlNode w = produkt.SelectSingleNode(nazwaWezla);
                    if (w != null)
                    {
                        KategoriaProduktu nowa = new KategoriaProduktu();
                        nowa.Nazwa = w.InnerText;
                        nowa.Widoczna = true;
                        nowa.GrupaId = grupa.Id;
                        nowa.Id = nowa.WygenerujIDObiektu();
                        if (!listaWejsciowa.ContainsKey(nowa.Id))
                        {
                            listaWejsciowa.Add(nowa.Id, nowa);
                        }
                        foreach (var z in znalezione)
                        {
                            XmlNode n = z.Value.SelectSingleNode(nazwaWezla);
                            if (n != null)
                            {
                                DodajFraze(z.Key, (int)nowa.Id, "nazwa", n.InnerText, nowa.GetType());
                            }
                        }
                    }
                }
           AktualizacjaTlumaczen();
       }
        

       [FriendlyName("Id grupy, do której należy przypisać kategorie. Istniejące zostaną usnięte")]
       [WidoczneListaAdminAttribute(false, false, true, false)]
        public int Grupa { get; set; }
        public override string Opis
        {
            get { return "Kategorie produktów"; }
        }
    }
}
