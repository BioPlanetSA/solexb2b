using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SolEx.Hurt.Model;

namespace SolEx.Hurt.Sync.App.Modules_.Provider
{
    /// <summary>
    /// Tworzy kategorie zrodlowe na podstawie niezerowych poziomów cen
    /// </summary>
    public class GrupyNaPodstawiePoziomuCenProvider : IImportDataModule
    {
        public void DoWork(System.Collections.Specialized.NameValueCollection configuration, Model.SourceDB db)
        {
            //string[] priceLevelsNames = configuration["GrupyNaPodstawiePoziomuPoziomyNazwy"].ToLower().Split(new string[]{";;"},StringSplitOptions.RemoveEmptyEntries);
            //string  prefix = configuration["GrupyNaPodstawiePoziomuPrefiks"];
            //for (int i = 0; i < db.Products.Count; i++)
            //{
            //    for (int j = 0; j < priceLevelsNames.Length; j++)
            //    {
            //        var pl =db.Products[i].PriceLevels.FirstOrDefault(p => p.poziom_id.ToLower() == priceLevelsNames[j]);
            //        if (pl!=null && pl.netto > 0)
            //        {
            //            int source_id = 20000 + pl.id;
            //            db.Products[i].CategoryIds.Add(source_id);
            //            if(!db.SourceCategories.Any(p=>p.Id==source_id))
            //            {
            //                Category tmp = new Category();
            //                tmp.Id = source_id;
            //                tmp.Name = prefix + priceLevelsNames[j];
            //                db.SourceCategories.Add(tmp);
            //            }
            //        }
            //    }
            //}
        }

        public event ProgressChangedEventHandler ProgresChanged;
    }
}
