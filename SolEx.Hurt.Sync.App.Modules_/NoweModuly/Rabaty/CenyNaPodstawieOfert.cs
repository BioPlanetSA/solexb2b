using System;
using System.Collections.Generic;
using System.Linq;
using ServiceStack.Common;
using SolEx.Hurt.Core.Sync;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Interfaces.SyncModuly;
using SolEx.Hurt.Sync.App.Modules_.Rozne;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Rabaty
{
    [FriendlyName("Cena na podstawie ofert", FriendlyOpis = "Moduł, ustawia cenę produkt na podstawie oferty")]
    public class CenyNaPodstawieOfert : BazaModulyDokumentyStatus, IModulRabaty
    {
        public CenyNaPodstawieOfert()
        {
            JednakoweCenyDlaWszystkichDzieci = true;
        }

        [FriendlyName("Jeśli w ofercie jest cena dla jednego dziecka - to przepisz cene dla wszystkich dzieci w tej samej rodzinie")]
        [WidoczneListaAdmin(false, false, true, false)]
        public bool JednakoweCenyDlaWszystkichDzieci { get; set; }

        public void Przetworz(ref List<Rabat> rabatyNaB2B, ref List<ProduktUkryty> produktyUkryte, ref Dictionary<long, Konfekcje> konfekcjaNaB2B, IDictionary<long, Klient> kliencib2B,
            Dictionary<long, Produkt> produkty, List<PoziomCenowy> ceny, List<Cecha> cechyNaPlatformie, Dictionary<long, ProduktCecha> cechyProdukty, Dictionary<long, KategoriaProduktu> kategorie,
            List<ProduktKategoria> produktyKategorie, ref IDictionary<int, KategoriaKlienta> kategorieKlientow, ref IDictionary<long, KlientKategoriaKlienta> klienciKategorie)
        {
            if (kliencib2B == null || kliencib2B.IsEmpty())
            {
                LogiFormatki.PobierzInstancje.LogujInfo($"Brak klientów na B2B do przetowrzenia.");
                return;
            }

            long[] klienciDoPobrania = kliencib2B.Values.Where(x => x.Aktywny).Select(x => x.Id).ToArray();
            if (klienciDoPobrania.IsEmpty())
            {
                LogiFormatki.PobierzInstancje.LogujInfo($"Brak klientów aktywnych na B2B do przetowrzenia.");
            }

            Dictionary<string, IGrouping<string, Produkt>> rodzinowe = produkty.Values.Where(x => !string.IsNullOrEmpty(x.Rodzina)).GroupBy(x => x.Rodzina).ToDictionary(x => x.Key, x => x);
            Dictionary<long, List<HistoriaDokumentuProdukt>> pobrane = PasujaceDokumenty(klienciDoPobrania);

            foreach (var d in pobrane)
            {
                Klient klientDokumentu;
                if (kliencib2B.TryGetValue(d.Key, out klientDokumentu))
                {
                    foreach (var p in d.Value)
                    {
                        if (!produkty.ContainsKey(p.ProduktIdBazowy))
                        {
                            continue; //nie ma na b2b
                        }
                        var pr = produkty[p.ProduktIdBazowy];



                        if (!JednakoweCenyDlaWszystkichDzieci || (JednakoweCenyDlaWszystkichDzieci && string.IsNullOrEmpty(pr.Rodzina)))
                        {
                            Rabat r = new Rabat(d.Key, p.ProduktIdBazowy, p.CenaNettoPoRabacie);
                            r.WalutaId = klientDokumentu.WalutaId;
                            rabatyNaB2B.Add(r);
                        }
                        else
                        {
                            foreach (var pwr in rodzinowe[pr.Rodzina])
                            {
                                Rabat r = new Rabat(d.Key, pwr.Id, p.CenaNettoPoRabacie);
                                r.WalutaId = klientDokumentu.WalutaId;
                                if (rabatyNaB2B.All(x => x.ProduktId != pwr.Id))
                                {
                                    rabatyNaB2B.Add(r);
                                }
                            }

                        }
                    }

                }
                
            }
        }
    }
}

