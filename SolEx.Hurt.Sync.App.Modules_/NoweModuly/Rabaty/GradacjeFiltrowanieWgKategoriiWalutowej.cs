using System;
using System.Collections.Generic;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.Sync;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;


namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Rabaty
{
    public   class GradacjeFiltrowanieWgKategoriiWalutowej: SyncModul, Model.Interfaces.SyncModuly.IModulRabaty
    {
        public GradacjeFiltrowanieWgKategoriiWalutowej()
        {
            NazwaGrupy = "WALUTA";
        }
        public override string uwagi
        {
            get { return "Filtruje gradacje wg waluty. Usuwa jeśli rabat jest na kategorie walutową i waluta się różni"; }
        }
        
        [FriendlyName("Nazwa grupy walutowej")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string NazwaGrupy { get; set; }
        
        public void Przetworz(ref List<Rabat> rabatyNaB2B, ref List<ProduktUkryty> produktyUkryteNaB2B, ref Dictionary<long, Konfekcje> konfekcjaNaB2B, IDictionary<long, Klient> kliencib2B, Dictionary<long, Produkt> produkty, List<PoziomCenowy> ceny, List<Cecha> cechy, Dictionary<long, ProduktCecha> cechyProdukty, Dictionary<long, KategoriaProduktu> kategorie, List<ProduktKategoria> produktyKategorie, ref IDictionary<int, KategoriaKlienta> kategorieKlientow, ref IDictionary<long, KlientKategoriaKlienta> klienciKategorie)
        {
            int przed = konfekcjaNaB2B.Count;
            var waluty = ApiWywolanie.PobierzWaluty();
            HashSet<long>konfekcjaDoWywalenia = new HashSet<long>();
            foreach (var konfekcje in konfekcjaNaB2B)
            {
                if (konfekcje.Value.KategoriaKlientowId.HasValue)
                {
                    var kategoria = kategorieKlientow.ContainsKey(konfekcje.Value.KategoriaKlientowId.Value) ? kategorieKlientow[konfekcje.Value.KategoriaKlientowId.Value] : null;
                    if (kategoria != null)
                    {
                        if (kategoria.Grupa.Equals(NazwaGrupy, StringComparison.InvariantCultureIgnoreCase))
                        {
                            string waluta = waluty[konfekcje.Value.WalutaId].WalutaB2b;
                            if (!string.IsNullOrEmpty(waluta) && !kategoria.Nazwa.Equals(waluta, StringComparison.InvariantCultureIgnoreCase))
                            {
                                konfekcjaDoWywalenia.Add(konfekcje.Key);
                            }
                        }
                    }
                }

            }

            foreach (var konfekcje in konfekcjaDoWywalenia)
            {
                konfekcjaNaB2B.Remove(konfekcje);
            }
            
            int po = konfekcjaNaB2B.Count;
            LogiFormatki.PobierzInstancje.LogujInfo("Filtrowanie gradacji, przed {0}, po {1}, różnica {2}",przed,po,przed-po);
        }
    }
}
