using System;
using ServiceStack.Common;
using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Interfaces.Sync;
using SolEx.Hurt.Model.Interfaces.SyncModuly;
using SolEx.Hurt.Model;
using System.Collections.Generic;
using System.Linq;
using SolEx.Hurt.Core.Sync;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.CechyIAtrybuty
{
    [FriendlyName("Dodaj brakujące cechy",FriendlyOpis = "Dodaje brakujące cechy w drzewie - rozbiaja cechy z wybranego atrybuty na podcechy wzgledem separatora /\\.")]
    public class DodajBrakujaceCechy  : SyncModul, IModulCechyIAtrybuty, IModulProdukty
    {
        public IConfigSynchro Konfiguracja = SyncManager.PobierzInstancje.Konfiguracja;
        //public override string Opis
        //{
        //    get { return "Dodaje brakujące cechy w drzewie - rozbiaja cechy z wybranego atrybuty na podcechy wzgledem separatora /\\."; }
        //}

        [FriendlyName("Atrybut dla którego będą dodawane brakujące cechy")]
        [PobieranieSlownika(typeof(SlownikAtrybutow))]
        [WidoczneListaAdmin(false, false, true, false)]
        public string Atrybut { get; set; }

        [FriendlyName("Separator w cechach")]
        [WidoczneListaAdmin(false, false, true, false)]
        public string Separator { get; set; }

        public void Przetworz(ref List<Atrybut> atrybuty, ref List<Cecha> cechy, Dictionary<long, Produkt> produktyNaB2B)
        {
            Dictionary<long, ProduktCecha> lacznikiCech = ApiWywolanie.PobierzCechyProdukty();

            Przetworz(atrybuty, cechy, produktyNaB2B.Values.ToList(), lacznikiCech);
        }
        public void Przetworz(List<Atrybut> atrybuty, List<Cecha> cechy, List<Produkt> produkty, Dictionary<long, ProduktCecha> lacznikiCech)
        {
            char separatorAtrybutu = Konfiguracja.SeparatorAtrybutowWCechach[0];
            HashSet<string> listaSymbolowCech = new HashSet<string>( cechy.Select(x => x.Symbol.Trim().ToLower()) );
            var atrybut = atrybuty.FirstOrDefault(x => x.Id == int.Parse(Atrybut));
            if (atrybut == null)
            {
                throw new Exception($"Brak atrybutu: {Atrybut} pobranego z ERP który jest wybrany w module!!!!");
            }

            var cechyZAtrybutem = cechy.Where(x => x.AtrybutId == atrybut.Id).ToList();
            if (!cechyZAtrybutem.Any())
            {
                return;
            }
            const char pusty = '\0';
            for (int i = 0; i < cechyZAtrybutem.Count(); i++)
            {
                Cecha c = cechyZAtrybutem[i];
                char separator = c.Nazwa.Contains('\\') ? '\\' : c.Nazwa.Contains('/') ? '/' : pusty;
                //brak separatora omijamy ceche
                if (separator == pusty)
                {
                    continue;
                }
                //tworzymy tablice wzgledem separatora
                string[] podzielonaNazwa = cechyZAtrybutem[i].Nazwa.Split(separator);
                string atrybytZSeparatorem = $"{atrybut.Nazwa}{separatorAtrybutu}";
                string nazwaCechy = String.Empty;
                foreach (var s in podzielonaNazwa)
                {
                    nazwaCechy += s;
                    string symbolNowejCechy = $"{atrybytZSeparatorem}{nazwaCechy}".ToLower();
                    if (listaSymbolowCech.Contains(symbolNowejCechy))
                    {
                        nazwaCechy += separator;
                        continue;
                    }
                    Cecha nowaCecha = new Cecha(nazwaCechy, symbolNowejCechy)
                    {
                        AtrybutId = atrybut.Id,
                        Widoczna = true
                    };
                    Log.DebugFormat("Dodajemy cechy o nazwie: {0} i symbolu {1}",nowaCecha.Nazwa,nowaCecha.Symbol);
                    nowaCecha.Id = nowaCecha.WygenerujIDObiektuSHAWersjaLong(1);
                    cechy.Add(nowaCecha);
                    listaSymbolowCech.Add(nowaCecha.Symbol);
                    nazwaCechy += separator;
                }
            }

        }

        public void Przetworz(ref List<Produkt> listaWejsciowa, ref List<Tlumaczenie> produktyTlumaczenia, Dictionary<long, Produkt> produktyNaB2B, 
            ref List<JednostkaProduktu> jednostki, ref Dictionary<long, ProduktCecha> lacznikiCech, ref List<ProduktKategoria> lacznikiKategorii,
            ISyncProvider provider, ref List<ProduktUkryty> produktuUkryteErp, ref List<ProduktyZamienniki> zamienniki, Dictionary<long, KategoriaProduktu> kategorie,
            ref List<Cecha> cechy, ref List<Atrybut> atrybuty)
        {
            Przetworz(atrybuty, cechy, listaWejsciowa, lacznikiCech);
        }
    }
}
