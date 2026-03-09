using System.Collections.Generic;
using System.Text.RegularExpressions;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Interfaces.Sync;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.KategorieProduktow
{
    public class KolejnoscKategorii : SyncModul, Model.Interfaces.SyncModuly.IModulKategorieProduktow
    {
        private const string FormatDomyslny = @"\s*(?<kolejnosc>[0-9]{1,})\s*\.\s*(?<nazwa>\w{1,999})";
        public void Przetworz(ref Dictionary<long, KategoriaProduktu> listaWejsciowa, Dictionary<long, KategoriaProduktu> listaKategoriiB2B, ISyncProvider provider, List<Grupa> grupyPRoduktow)
        {
            string strRegex = string.IsNullOrEmpty(Format) ? FormatDomyslny : Format;
            const RegexOptions myRegexOptions = RegexOptions.None;
            Regex myRegex = new Regex(strRegex, myRegexOptions);

            foreach (int k in listaWejsciowa.Keys)
            {
                long grupaId = listaWejsciowa[k].GrupaId;
                if (Grupy.Count > 0 && !Grupy.Contains(grupaId)) continue; 
                foreach (Match myMatch in myRegex.Matches(listaWejsciowa[k].Nazwa))
                {
                    if (myMatch.Success)
                    {
                       if (myMatch.Groups["kolejnosc"].Success && myMatch.Groups["nazwa"].Success)
                       {
                           int kolejnosc = int.Parse(myMatch.Groups["kolejnosc"].Value);
                           listaWejsciowa[k].Kolejnosc = kolejnosc;
                           listaWejsciowa[k].Nazwa = myMatch.Groups["nazwa"].Value;
                       }
                    }
                }
            }
        }

        public override string uwagi
        {
            get { return ""; }
        }

        [FriendlyName("Format nazwy, parametr kolejnosci ma byc oznaczony kolejnosc,nazwa grupy ma być oznaczona: nazwa , domyslny format: " + FormatDomyslny)]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string Format { get; set; }

        [FriendlyName("ID grup, dla której będzie zastosowany moduł, oddzielone ; Jeśli puste to dotyczy wszystkich grup ")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string IDGrupy { get; set; }

        private List<long> Grupy
        {
            get
            {
                List<long> wynik=new List<long>();
                if (!string.IsNullOrWhiteSpace(IDGrupy))
                {
                    foreach (string i in IDGrupy.Split(';'))
                    {
                        if (string.IsNullOrWhiteSpace(i)) continue;
                        
                        wynik.Add(int.Parse(i));
                    }
                }
                return wynik;
            }
        }
        public override string Opis
        {
            get { return "Ustawia sortowanie wg formatu"; }
        }
    }
}
