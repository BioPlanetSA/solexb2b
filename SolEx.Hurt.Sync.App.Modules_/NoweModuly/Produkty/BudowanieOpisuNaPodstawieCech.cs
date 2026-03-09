using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces.Sync;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Produkty
{
    public class BudowanieOpisuNaPodstawieCech : SyncModul, Model.Interfaces.SyncModuly.IModulProdukty
    {
        [FriendlyName("Pole, do którego będzie wpisany opis")]
        [SyncSlownikNaPodstawieInnegoTypu(typeof (Produkt))]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string Pole { get; set; }

        [Niewymagane]
        [FriendlyName("Nagłówek, dodany będzie wyłacznie jeśli jest jakakolwiek cecha")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string Naglowek { get; set; }

        [Niewymagane]
        [FriendlyName("Stopka, dodana będzie wyłacznie jeśli jest jakakolwiek cecha")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string Stopka { get; set; }

        [FriendlyName("Opis produktu w formie {idAtrybutu&&Przed wartość dodana przed cechą && Wartość dodana po cesze}")]
        [WymuszonyTypEdytora(TypEdytora.PoleTekstoweMultiLine)]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string OpisFormat { get; set; }

        public override string Opis
        {
            get { return "Tworzy opis automatycznie na podstawie wybranych atrybutów"; }
        }

        public void Przetworz(ref List<Produkt> listaWejsciowa, ref List<Tlumaczenie> produktyTlumaczenia, Dictionary<long, Produkt> produktyNaB2B,
            ref List<JednostkaProduktu> jednostki, ref Dictionary<long, ProduktCecha> lacznikiCech, ref List<ProduktKategoria> lacznikiKategorii, 
            ISyncProvider provider, ref List<ProduktUkryty> produktuUkryteErp, ref List<ProduktyZamienniki> zamienniki, Dictionary<long, KategoriaProduktu> kategorie,
            ref List<Cecha> cechy, ref List<Atrybut> atrybuty)
        {
            if (String.IsNullOrEmpty(OpisFormat) || String.IsNullOrWhiteSpace(Pole)) return;
            Type prTyp = typeof (Produkt);
            PropertyInfo pi = prTyp.GetProperty(Pole);
            if (pi.PropertyType != typeof (string))
            {
                throw new InvalidOperationException("Pole do którego wpiszemy opis musi być typu tekstowego");
            }
            List<Cecha> cechyTmp = cechy.Where(x => x.AtrybutId != null).ToList();
            IEnumerable<ProduktCecha> laczniki = ApiWywolanie.PobierzCechyProdukty().Values;
            string strRegex = @"(?<atrybut>\{[^\{\}]*\})";
            RegexOptions myRegexOptions = RegexOptions.IgnoreCase;
            Regex myRegex = new Regex(strRegex, myRegexOptions);

            Parallel.ForEach(listaWejsciowa, produkt =>
            {
                List<long> cechyProduktu = laczniki.Where(x => x.ProduktId == produkt.Id).Select(x => x.CechaId).ToList();
                string calyOpis = OpisFormat;
                Match myMatch = myRegex.Match(calyOpis);

                while (myMatch.Success)
                {
                    string fragment = myMatch.Groups["atrybut"].Value;
                    fragment = fragment.Replace("{", "").Replace("}", "");
                    string[] elementy = fragment.Split(new[] {"&&"}, StringSplitOptions.None);
                    int id;
                    string s = "";
                    if (elementy.Length > 0 && int.TryParse(elementy[0], out id)) //jest id i jest liczbą
                    {
                        string przed = elementy.Length > 1 ? elementy[1] : "";
                        string po = elementy.Length > 2 ? elementy[2] : "";
                        Cecha cecha = cechyTmp.FirstOrDefault(x => x.AtrybutId == id && cechyProduktu.Contains(x.Id));

                        if (cecha != null)
                        {
                            s = przed + cecha.Nazwa + po;
                        }
                    }
                    string pocz = calyOpis.Substring(0, myMatch.Index);
                    string koniec = calyOpis.Substring(myMatch.Index + myMatch.Length);
                    calyOpis = pocz + s + koniec;
                    myMatch = myRegex.Match(calyOpis);
                }
                calyOpis = calyOpis.Trim();
                if (!string.IsNullOrWhiteSpace(calyOpis))
                {
                    calyOpis = Naglowek + calyOpis + Stopka;
                    pi.SetValue(produkt, calyOpis, null);
                }
            });
        }


    }
}
