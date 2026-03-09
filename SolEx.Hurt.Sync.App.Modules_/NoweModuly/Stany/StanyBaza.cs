using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Interfaces.SyncModuly;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Stany
{
    public abstract class StanyBaza : SyncModul, IModulStany
    {
        protected StanyBaza()
        {
            DomyslnieZero = false;
            SposobLaczenia = LaczenieStanow.Sumuj;
        }

        [FriendlyName("Pole produktu na po którym ³¹czyæ stany.")]
        [SyncSlownikNaPodstawieInnegoTypu(typeof(Produkt))]
        [WidoczneListaAdmin(false, false, true, false)]
        public string Pole { get; set; }

        [FriendlyName("Sposób ³¹czenia stanów")]
        [WidoczneListaAdmin(false, false, true, false)]
        public LaczenieStanow SposobLaczenia { get; set; }


        [FriendlyName("Domyœlny stan produktu ustaw na 0.", FriendlyOpis = "Dla produktów które nie pojawi³y siê w imporcie stanów, ustawiaj stan 0")]
        [WidoczneListaAdmin(false, false, true, false)]
        public bool DomyslnieZero { get; set; }

        [FriendlyName("ID magazynu z B2B do którego bêd¹ dodane nowe stany")]
        [PobieranieSlownika(typeof(SlownikMagazynow))]
        [WidoczneListaAdmin(false, false, true, false)]
        public int IdMagazynu { get; set; }
        
        /// <summary>
        /// Aktualizuje listê stanów wzgledem tego co pobra³
        /// </summary>
        /// <param name="stanyZPliku"></param>
        /// <param name="listaWejsciowa"></param>
        /// <param name="produktyB2B"></param>
        public void ZaktualizujListeStanowOStanyZPliku(Dictionary<long, decimal> stanyZPliku, ref Dictionary<int, List<ProduktStan>> listaWejsciowa, List<Produkt> produktyB2B)
        {
            //musimy przejsc po wszystkich produktach poniewa¿ w stytacji gdy produkt zniknie z pliku musimy mu wyzerowaæ iloœæ
            foreach (var prod in produktyB2B)
            {
                //dla brakuj¹cych produktów ustawiamy stan 0
                decimal stan;
                if (!stanyZPliku.TryGetValue(prod.Id, out stan))
                {
                    if (DomyslnieZero)
                    {
                        stan = 0;
                    }
                    else
                    {
                        continue;
                    }
                }
                
                //jezeli by³ juz stan dla danego produktu na podanym magazynie zwiekszamy jego stan o wartoœæ z pliku w przeciwnym razie dodajemy stan
                var stanProduktu = listaWejsciowa[IdMagazynu].FirstOrDefault(x => x.ProduktId == prod.Id);
                stan = NowyStan(stanProduktu?.Stan, stan);
                if (stanProduktu != null)
                {
                    stanProduktu.Stan = stan;
                }
                else
                {
                    //Tworzymu nowy stan 0 dla brakuj¹cego produktu
                    ProduktStan s = new ProduktStan
                    {
                        MagazynId = IdMagazynu,
                        ProduktId = prod.Id,
                        Stan = stan
                    };
                    listaWejsciowa[IdMagazynu].Add(s);
                }
            }
        }

        public Dictionary<string, long> SlownikProduktow(List<Produkt> produktyB2B)
        {
            Dictionary<string, long> wynik = new Dictionary<string, long>(StringComparer.InvariantCultureIgnoreCase);
            Type typ = typeof(Produkt);
            PropertyInfo property =typ.GetProperty(Pole);
            if (property == null) return null;
            foreach (var produkt in produktyB2B)
            {
                var wartosc = property.GetValue(produkt);
                if (wartosc == null)
                {
                    Log.Error($"Produkt:{produkt.Kod}[{produkt.Id}] w polu: {Pole} ma wartoœæ null. Pomijam ten produkt");
                    continue;
                }
                string wartoscStr = wartosc.ToString().Trim();
                if (!wynik.ContainsKey(wartoscStr))
                {
                    wynik.Add(wartoscStr, produkt.Id);
                }else
                {
                    Log.Error($"Produkt:{produkt.Kod}[{produkt.Id}] w polu: {Pole} ma wartoœæ {wartoscStr} podobnie jak produkt o id: {wynik[wartoscStr]}. Pomijamy dodanie dubla.");
                }
            }
            return wynik;
        }


        /// <summary>
        /// Wylicza nowy stan produktu wg ustawienia SposobLaczenia
        /// </summary>
        /// <param name="aktualny">Aktualny stan</param>
        /// <param name="nowy">Wartoœæ z pliku</param>
        /// <returns>Nowa wartoœæ stanu</returns>
        public decimal NowyStan(decimal? aktualny, decimal nowy)
        {
            decimal wynik= aktualny??0;
            switch (SposobLaczenia)
            {
                case LaczenieStanow.Podmieniaj:
                    wynik = nowy;
                    break;
                case LaczenieStanow.PodmieniajGdyWiekszy:
                    if (nowy > wynik)
                    {
                        wynik = nowy;
                    }
                    break;
                case LaczenieStanow.Sumuj:
                    wynik += nowy;
                    break;
            }
            return wynik;
        }

        public abstract void Przetworz(ref Dictionary<int, List<ProduktStan>> listaWejsciowa, List<Magazyn> magazyny, List<Produkt> produktyB2b);
    }


    public enum LaczenieStanow
    {
        [FriendlyName("Podmieniaj")]
        Podmieniaj,
        [FriendlyName("Podmieniaj gdy wiêkszy")]
        PodmieniajGdyWiekszy,
        [FriendlyName("Sumuj")]
        Sumuj
    }
}