using System.Globalization;
using System.Text.RegularExpressions;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Core;
using SolEx.Hurt.Model.CustomSearchCriteria;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Sync.Core;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Klienci
{
    public class UkrywajProduktyDlaKlientaJesliNieMaRabatuSaturn : Model.SyncModul, SolEx.Hurt.Model.Interfaces.SyncModuly.IModulKlienci
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public UkrywajProduktyDlaKlientaJesliNieMaRabatuSaturn()
        {

        }

        public override string uwagi
        {
            get { return ""; }
        }

        public override string Opis
        {
            get { return "Moduł dla Saturna ukrywający produkty jeśli klient nie ma na nie rabatu"; }
        }
     
        public void Przetworz(ref Dictionary<klienci, List<kategorie_klientow>> listaWejsciowa)
        {
            Dictionary<string, object> katparametry = new Dictionary<string, object>(1);
            katparametry.Add("wszystkie", true);
            var kategoriekl = APIWywolania.PobierzKategorieKlientow(katparametry);
            var kategorieklpolaczenia = APIWywolania.PobierzKlienciKategorie(katparametry);
            log.Debug(string.Format("Pobrano {0} kategorii klientów i {1} powiązań", kategoriekl.Count, kategorieklpolaczenia.Count));
            Dictionary<int, List<kategorie_klientow>> kategorieKlientow = new Dictionary<int, List<kategorie_klientow>>();
            List<kategorie> kategorie = APIWywolania.PobierzKategorie(new KategorieSearchCriteria());
            Dictionary<int, List<int>> grupy = kategorieklpolaczenia.GroupBy(a => a.klient_id).ToDictionary(p => p.Key, b => b.Select(c => c.kategoria_klientow_id).ToList());
        //    log.Debug("grup: " + grupy.Count);

            List<cechy> listaCech = APIWywolania.GetCechy(new CechySearchCriteria());

            foreach (var gr in grupy)
            {
                log.Debug(string.Format("kategorii w grupie {0}:{1}", gr.Key, gr.Value.Count));
                foreach (int idkategorii in gr.Value)
                {
                    var kategoriedanegoklienta = kategoriekl.FirstOrDefault(a => a.Id == idkategorii);
                    if (kategoriedanegoklienta != null)
                    {
                        if (!kategorieKlientow.ContainsKey(gr.Key))
                        {
                            kategorieKlientow.Add(gr.Key, new List<kategorie_klientow>());
                        }
                        
                        kategorieKlientow[gr.Key].Add(kategoriedanegoklienta);
                    }
                }

            }

            List<rabaty> rabatyNaB2B = APIWywolania.PobierzRabaty(new RabatySearchCriteria());
            List<produkty_ukryte> produktyUkryte = new List<produkty_ukryte>();

            List<produkty_ukryte> produktyUkryteB2B =
                APIWywolania.PobierzProduktyUkryte(new ProduktyUkryteSearchCriteria());

            foreach (var klient in listaWejsciowa)
            {

                klient.Key.pelna_oferta = false;
                var rabatyKlienta = rabatyNaB2B.Where(a => a.klient_id == klient.Key.klient_id).ToList();

                if (!kategorieKlientow.ContainsKey(klient.Key.klient_id))
                    continue;

                var kategorieRabatoweKlienta =
                    kategorieKlientow[klient.Key.klient_id].Where(a => rabatyKlienta.Any(b => b.kategoria_klientow_id == a.Id)).ToList();

                log.Debug(string.Format("klient {0} ma {1} rabatów i {2} kategorii", klient.Key.nazwa, rabatyKlienta.Count, kategorieRabatoweKlienta.Count));

                foreach (kategorie_klientow kategorie_Klientow in kategorieRabatoweKlienta)
                {
                    kategorie k = WyszukajKategorieZRabatu(kategorie_Klientow.nazwa, kategorie);

                    if (k != null)
                    {
                        produkty_ukryte pu = new produkty_ukryte();
                        pu.kategoria_id = k.kategoria_id;
                        pu.klient_zrodlo_id = klient.Key.klient_id;
                        pu.Tryb = KatalogKlientaTypy.Dostepne;

                        if (
                            !produktyUkryte.Any(
                                a => a.kategoria_id == pu.kategoria_id && a.klient_zrodlo_id == pu.klient_zrodlo_id) &&
                            !produktyUkryteB2B.Any(
                                a => a.kategoria_id == pu.kategoria_id && a.klient_zrodlo_id == pu.klient_zrodlo_id))
                        {
                            produktyUkryte.Add(pu);
                        }
                    }

                }
            }



            List<int> dousuniecia = new List<int>();

            foreach (produkty_ukryte pu in produktyUkryteB2B)
            {
                if (!produktyUkryte.Any(
                    a => a.kategoria_id == pu.kategoria_id && a.klient_zrodlo_id == pu.klient_zrodlo_id))
                    dousuniecia.Add(pu.id);
            }

            if (dousuniecia.Count > 0)
            {
                LogiFormatki.LogujInfo("Łącznie produktów ukrytych do usunięcia:  " + dousuniecia.Count);
                APIWywolania.UsunProduktyUkryte(dousuniecia);

            }
            LogiFormatki.LogujInfo("Łącznie produktów ukrytych do aktualizacji:  " + produktyUkryte.Count);
            APIWywolania.AktualizujProduktyUkryte(produktyUkryte);
        }

        kategorie WyszukajKategorieZRabatu(string nazwa, List<kategorie> listaKategorii)
        {
            string nowanazwa = nazwa.Replace("rabat_", "").Replace("dostawa_", "").ToLower();
            string[] nazwy = nowanazwa.Split(new string[]{"_"}, StringSplitOptions.RemoveEmptyEntries);

            if (nazwy.Length >= 2)
            {
                kategorie parent = listaKategorii.FirstOrDefault(a => a.nazwa.ToLower() == nazwy[0]);
                if (parent != null)
                {
                    kategorie dziecko =
                        listaKategorii.FirstOrDefault(a => a.nazwa.ToLower() == nazwy[1] && a.parent_id == parent.kategoria_id);

                    return dziecko;
                }
            }
            else if (nazwy.Length <= 2)
            {
                kategorie parent = listaKategorii.FirstOrDefault(a => a.nazwa.ToLower() == nazwy[0]);
                return parent;
            }

            return null;
        }
    }
}
