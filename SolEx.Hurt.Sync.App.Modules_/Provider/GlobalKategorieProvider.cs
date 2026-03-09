using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SolEx.Hurt.Model;
using System.Text.RegularExpressions;

namespace SolEx.Hurt.Sync.App.Modules_.Provider
{
    public class GlobalKategorieProvider : IImportDataModule
    {
        #region IImportDataModule Members

        public void DoWork(System.Collections.Specialized.NameValueCollection configuration, SourceDB db)
        {
            if (this.ProgresChanged != null)
            {
                ProgresChanged(this, new ProgressChangedEventArgs("Dodawanie kategorii"));
            }
            List<Category> tmp1 = db.SourceCategories.Where(p=>p.Id>0 && p.Id<10000).OrderBy(p => p.Name).ToList();
     
            List<Category> cats = new List<Category>();
            string child_Regexp_lvl1 = @"^([{0}]\.)([\s])*([0-9]*\.)([\s])*[A-ZA-z\.\s]+";
            string child_Regexp_lvl2 = @"^([{0}]\.)([\s])*([0-9]*\.)([0-9]*\.)+([\s])*[A-ZA-z\.\s]+";
            string parent_regexp = @"^([A-Z]\.)+\s*[A-Za-z\.\s]+";
            string child_name_junk_Regexp = @"^([A-Z]\.)([\s])*([0-9]*\.)+([\s])*";
            string parent_name_junk_regexp = @"^([A-Z]\.)+\s*";
            Regex parent = new Regex(parent_regexp);
            for (int i = 0; i < tmp1.Count; i++)
            {
                tmp1[i].Lp = i + 1;
            }
            for (int i = 0; i < tmp1.Count; i++)
            {
                Match parent_match = parent.Match(tmp1[i].Name);//wyszukujemy ojców
                if (parent_match.Success)
                {
                    string group_letter = tmp1[i].Name.Substring(0, 1); //litera na która zaczyna siê grupa,
                    if (!cats.Contains(tmp1[i]))
                    {
                        tmp1[i].Name = Regex.Replace(tmp1[i].Name, parent_name_junk_regexp, "");
                        cats.Add(tmp1[i]);
                    }
                    Regex childresn = new Regex(string.Format(child_Regexp_lvl1, group_letter)); // wyszukujemy wszyskie kategorie których ojca znalezliœmy w poprzednim kroku, poziom pierwszy
                    for (int j = 0; j < tmp1.Count; j++)
                    {
                        Match child_match = childresn.Match(tmp1[j].Name);
                        if (child_match.Success)
                        {
                            if (!cats.Contains(tmp1[j]))
                            {
                                tmp1[j].ParentId = tmp1[i].Id;
                                tmp1[j].Name = Regex.Replace(tmp1[j].Name, child_name_junk_Regexp, "");
                                cats.Add(tmp1[j]);
                                Regex childresn_lvl2 = new Regex(string.Format(child_Regexp_lvl2, group_letter)); // wyszukujemy wszyskie kategorie których ojca znalezliœmy w poprzednim kroku, poziom drugi
                                for (int z = 0; z < tmp1.Count; z++)
                                {
                                    Match child_match_lvl2 = childresn_lvl2.Match(tmp1[z].Name);
                                    if (child_match_lvl2.Success)
                                    {
                                        if (!cats.Contains(tmp1[z]))
                                        {
                                            tmp1[z].ParentId = tmp1[j].Id;
                                            tmp1[z].Name = Regex.Replace(tmp1[z].Name, child_name_junk_Regexp, "");
                                            cats.Add(tmp1[z]);
                                        }
                                    }
                                }
                            }
                        }

                    }
                }
            }
       
            db.SourceCategories.RemoveAll(p=>p.Id>0 && p.Id<10000);
            db.SourceCategories.AddRange(cats);
            if (this.ProgresChanged != null)
            {
                ProgresChanged(this, new ProgressChangedEventArgs("Dodawanie kategorii koniec, iloœæ " + db.SourceCategories.Count.ToString()));
            }

        }
        public event ProgressChangedEventHandler ProgresChanged;

        #endregion
    }
}
