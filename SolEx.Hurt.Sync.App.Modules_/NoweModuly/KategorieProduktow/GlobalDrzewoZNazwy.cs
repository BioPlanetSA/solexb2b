using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.Sync;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Interfaces.Sync;


namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.KategorieProduktow
{
    
    class GlobalDrzewoZNazwy: SyncModul, Model.Interfaces.SyncModuly.IModulKategorieProduktow
    {
        public override string uwagi
        {
            get { return ""; }
        }

        [FriendlyName("ID grupy kategorii którą przerobić")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public int IdGrupyKtorejDotyczy { get; set; }

        public void Przetworz(ref Dictionary<long, Model.KategoriaProduktu> listaWejsciowa, Dictionary<long, Model.KategoriaProduktu> listaKategoriiB2B, ISyncProvider provider, List<Model.Grupa> grupyPRoduktow)
        {

            List<Model.KategoriaProduktu> tmp1 = listaWejsciowa.Values.Where(a => a.GrupaId == IdGrupyKtorejDotyczy).ToList();

            List<Model.KategoriaProduktu> cats = new List<Model.KategoriaProduktu>();
            const string childRegexpLvl1 = @"^([{0}]\.)([\s])*(?<numer>\d*)\.([\s])*[\D]+";
            const string childRegexpLvl2 = @"^([{0}]\.)([\s])*({1})([\s])*\.([\s])*(\d*\.)+([\s])*[\D]+";
            const string parentRegexp = @"^([A-Z]\.)+(\s)*[\D]+";
            const string childNazwaJunkRegexp = @"^([A-Z]\.)([\s])*(\d*\.)+([\s])*";
            const string parentNazwaJunkRegexp = @"^([A-Z]\.)+(\s)*";
            Regex parent = new Regex(parentRegexp,RegexOptions.IgnoreCase);
            for (int i = 0; i < tmp1.Count; i++)
            {
                tmp1[i].Kolejnosc = i + 1;
            }
            foreach (KategoriaProduktu t2 in tmp1)
            {
                Match parent_Match = parent.Match(t2.Nazwa);//wyszukujemy ojców
                if (!parent_Match.Success)
                {
                    continue;
                }
                string group_letter = t2.Nazwa.Substring(0, 1); //litera na która zaczyna się grupa,
                if (!cats.Contains(t2))
                {
                    t2.Nazwa = Regex.Replace(t2.Nazwa, parentNazwaJunkRegexp, "");
                    cats.Add(t2);
                }       
                Regex childresn = new Regex(string.Format(childRegexpLvl1, group_letter), RegexOptions.IgnoreCase); // wyszukujemy wszyskie kategorie których ojca znalezliśmy w poprzednim kroku, poziom pierwszy
                foreach (KategoriaProduktu t1 in tmp1)
                {
                    Match child_match = childresn.Match(t1.Nazwa);
                    if (!child_match.Success)
                    {
                        continue;
                    }
                    if (!cats.Contains(t1))
                    {
                        t1.ParentId = t2.Id;
                        t1.Nazwa = Regex.Replace(t1.Nazwa, childNazwaJunkRegexp, "");
                        string nr = child_match.Groups["numer"].Value;
                        cats.Add(t1);
                        string wyr = string.Format(childRegexpLvl2, group_letter, nr);
                        Regex childresn_lvl2 = new Regex(wyr, RegexOptions.IgnoreCase); // wyszukujemy wszyskie kategorie których ojca znalezliśmy w poprzednim kroku, poziom drugi
                        LogiFormatki.PobierzInstancje.LogujDebug(string.Format("Kategoria {0}, wyrazenie {1}", t1.Nazwa, wyr));
                        foreach (KategoriaProduktu t in tmp1)
                        {
                            Match child_match_lvl2 = childresn_lvl2.Match(t.Nazwa);
                            if (child_match_lvl2.Success)
                            {
                                if (!cats.Contains(t))
                                {
                                    t.ParentId = t1.Id;
                                    t.Nazwa = Regex.Replace(t.Nazwa, childNazwaJunkRegexp, "");
                                    cats.Add(t);
                                }
                            }
                            LogiFormatki.PobierzInstancje.LogujDebug(string.Format("Kategoria {0}, dopasowanie {1}", t.Nazwa, child_match_lvl2.Success));
                        }
                    }
                }
            }
            cats.AddRange( listaWejsciowa.Values.Where(a => a.GrupaId != IdGrupyKtorejDotyczy) );
            listaWejsciowa = cats.ToDictionary(a => a.Id, a => a);
        }

        public override string Opis
        {
            get { return "Przerabia wpisy typu A.1.1 na drzewo kategorii, gdzie A to kategoria główna"; }
        }
    }

}
