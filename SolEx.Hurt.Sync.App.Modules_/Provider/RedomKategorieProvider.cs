using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using SolEx.Hurt.Model;

namespace SolEx.Hurt.Sync.App.Modules_.Provider
{
    public class RedomKategorieProvider : IImportDataModule
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public event ProgressChangedEventHandler ProgresChanged;
        int add = 100000; //dodawane do id podkategorii
        public void DoWork(NameValueCollection configuration, SourceDB db)
        {
            if (this.ProgresChanged != null)
            {
                this.ProgresChanged(this, new ProgressChangedEventArgs("Kategorie  z cech"));
            }
            List<int> attrIds = new List<int>(200);
            for (int i = 0; i < db.Attributes.Count; i++)
            {
               
                if (db.Attributes[i].symbol.Trim().StartsWith("-"))//kategorie z myslnikiem oznaczaja podkategorie - woda perfumowana damska alternatywna
                {
                    attrIds.Add(db.Attributes[i].cecha_id);
                }
            }
            for (int i = 0; i < db.Products.Count; i++)
            {
                if(db.Products[i].widoczny) //spawdzamy tylko aktywne produkty
                {
                    for (int j = 0; j < db.Products[i].AttributeIds.Count; j++)
                    {
                        if (attrIds.Contains(db.Products[i].AttributeIds[j])) //produkt ma jedną z cech zaczynaących się od -
                        {
                            var a = db.Attributes.First(p => p.cecha_id == db.Products[i].AttributeIds[j]); // pobieramy atrybut o tym id
                            int? parent=db.Products[i].CategoryIds.FirstOrDefault(p => p > 0);
                            if (!parent.HasValue ) continue;
                            int source_id = add + a.cecha_id*100+parent.Value; //id nowej kategorii zrodlowej
                            log.Error(source_id.ToString() + " " + a.symbol + " " + a.cecha_id.ToString() + " "+ parent.GetValueOrDefault(-1).ToString()); 
                            if (!db.SourceCategories.Any(p => p.id == source_id)) //czy istaniej kategorii o tym id, jesli nie to ją tworzymy
                            {
                                Category tmp = new Category();
                                tmp.nazwa = a.symbol.Trim();
                                tmp.id = source_id;
                                tmp.ParentId = parent.Value;
                                db.SourceCategories.Add(tmp);
                            }
                            db.Products[i].CategoryIds.Add(source_id);
                        }
                    }
                }
            }
        }
    }
}

