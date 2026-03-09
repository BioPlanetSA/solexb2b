using System.Reflection;
using ServiceStack.Common;
using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Interfaces;
using SolEx.Hurt.Model.Interfaces.Sync;
using log4net;
using SolEx.Hurt.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using SolEx.Hurt.Model.Interfaces.SyncModuly;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Produkty
{
    [FriendlyName("Przepisywanie cech do pola produktu", FriendlyOpis = "Określa które atrybuty mają być przepisane do wybranego pola w produkcie. Cechy, które mają wartość tak/nie lub yes/no mogą być użyte dla pól boolowskich.")]
    public class PrzepisanieCechDoPola : SyncModul, IModulProdukty, ITestowalna
    {
        public PrzepisanieCechDoPola()
        {
            Pola = new List<string>();
            Atrybuty = string.Empty;
            PrzepisujJesliWartoscNieJestPusta = false;
        }

        public override string uwagi
        {
            get { return ""; }
        }

        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        [FriendlyName("Pola, do których będą przepisane wybrane atrybuty")]
        [SyncSlownikNaPodstawieInnegoTypu(typeof(Produkt))]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public List<string> Pola { get; set; }

        [FriendlyName("Nazwa atrybutu, który będzie dodana do wybranego pola. Można podać wiele atrybutów oddzielając je średnikiem.")]
        [Obsolete("Pole wycofane zaleca się korzystać z ListaRozwijanaAtrybutow")]
        [Niewymagane]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string Atrybuty { get; set; }

        [FriendlyName("Przepisuj tylko jeśli wartość jest nie pusta. W przypadku pustych wartości nie przepisuj.")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public bool PrzepisujJesliWartoscNieJestPusta { get; set; }
        
        [FriendlyName("Nazwy atrybutu, które będą dodane do wybranego pola.")]
        [PobieranieSlownika(typeof(SlownikAtrybutow))]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public List<string> ListaRozwijanaAtrybutow { get; set; }

        public override string Opis
        {
            get { return "Określa które atrybuty mają być przepisane do wybranego pola w produkcie. Cechy, które mają wartość tak/nie lub yes/no mogą być użyte dla pól boolowskich."; }
        }

        public void Przetworz(ref List<Produkt> listaWejsciowa, ref List<Tlumaczenie> produktyTlumaczenia, Dictionary<long, Produkt> produktyNaB2B, ref List<JednostkaProduktu> jednostki, ref Dictionary<long, ProduktCecha> lacznikiCech, ref List<ProduktKategoria> lacznikiKategorii, ISyncProvider provider, ref List<ProduktUkryty> produktuUkryteErp, ref List<ProduktyZamienniki> zamienniki, Dictionary<long, KategoriaProduktu> kategorie, ref List<Cecha> cechy, ref List<Atrybut> atrybuty)
        {

            if (Pola.Count == 0)
            {
                return;
            }
            HashSet<int> atrybutyTmp;
           
            if (ListaRozwijanaAtrybutow != null && ListaRozwijanaAtrybutow.Any() && ListaRozwijanaAtrybutow[0] != "")
            {
                atrybutyTmp = new HashSet<int>( ListaRozwijanaAtrybutow.Select(int.Parse) );
            }
            else
            {
               atrybutyTmp = new HashSet<int>( atrybuty.Select(x=>x.Id) );
            }
            Dictionary<long, Cecha> cechyTmp = cechy.Where(x=>atrybutyTmp.Contains(x.AtrybutId.GetValueOrDefault())).ToDictionary(x=>x.Id,x=>x);


            Przetworz(ref listaWejsciowa, ref produktyTlumaczenia, produktyNaB2B, ref jednostki, ref lacznikiCech, ref lacznikiKategorii, provider, ref produktuUkryteErp, cechyTmp);
        }

        public void Przetworz(ref List<Produkt> listaWejsciowa, ref List<Tlumaczenie> produktyTlumaczenia, Dictionary<long, Produkt> produktyNaB2B,
            ref List<JednostkaProduktu> jednostki, ref Dictionary<long, ProduktCecha> lacznikiCech, ref List< ProduktKategoria> lacznikiKategorii, ISyncProvider provider, ref List<ProduktUkryty> produktuUkryteErp, Dictionary<long, Cecha>cechy)
        {
           var grupowanelaczniki= lacznikiCech.Values.GroupBy(x => x.ProduktId).ToDictionary(x => x.Key, x => x.Select(y => y.CechaId));

          
            PropertyInfo[] propertisy = typeof(Produkt).GetProperties().Where(x=>Pola.Contains(x.Name)).ToArray();
            foreach (Produkt prod in listaWejsciowa)
            {
                string nowaCecha = string.Empty;
                if (grupowanelaczniki.ContainsKey(prod.Id))
                {
                    foreach (int cechy1 in grupowanelaczniki[prod.Id])
                    {
                        var cecha = cechy.ContainsKey(cechy1)?cechy[cechy1]:null;
                        if (cecha != null)
                            nowaCecha += cecha.Nazwa + " ";
                    }
                }
                if (PrzepisujJesliWartoscNieJestPusta && string.IsNullOrWhiteSpace(nowaCecha))
                {
                    continue; 
                }
                nowaCecha = nowaCecha.Trim();
                foreach (var p in propertisy)
                {
                    //string nazwa = p.Name;
                    //var atribut =
                    //    p.GetCustomAttributes(true)
                    //     .FirstOrDefault(a => a.GetType() == typeof(FriendlyNameAttribute)) as
                    //    FriendlyNameAttribute;
                    //if (atribut != null)
                    //{
                    //    nazwa = atribut.FriendlyName;
                    //}

                    //if (Pola.Contains(nazwa) || Pola.Contains(p.Name))
                    //{
                        try
                        {
                            p.SetValueExt(prod, nowaCecha);
                        }
                        catch (Exception ex)
                        {
                            Log.Error(new Exception($"Nie udało się ustawić pola {p.Name} na wartość {nowaCecha} z powodu błędu.", ex));
                        }
                   // }
                }
            }
        }

        public List<string> TestPoprawnosci()
        {
            List<string> listaBledow = new List<string>();
            if (!string.IsNullOrEmpty(Atrybuty))
            {
                listaBledow.Add("Pole Atrybut jest wycofane użyj listy rozwijalnej atrybutów.");
            }

            if (ListaRozwijanaAtrybutow != null && ListaRozwijanaAtrybutow.Any() && ListaRozwijanaAtrybutow[0] != "")
            {
                HashSet<int> atrybuty = new HashSet<int>(ListaRozwijanaAtrybutow.Select(int.Parse) );

                if (ListaRozwijanaAtrybutow.Count > 0)
                {
                    if (atrybuty.Count == 0)
                    {
                        listaBledow.Add("Nie udało się pobrać atrybutów.");
                    }else if (atrybuty.Count != ListaRozwijanaAtrybutow.Count)
                    {
                        listaBledow.Add("Nie udało się pobrać wszystkich atrybutów.");
                    }
                }
            }
            return listaBledow;
        }
    }
}
