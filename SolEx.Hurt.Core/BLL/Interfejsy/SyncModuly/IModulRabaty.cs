using System.Collections.Generic;
using SolEx.Hurt.Model;

namespace SolEx.Hurt.Core.BLL.Interfejsy.SyncModuly
{
    [SyncJakaOperacja(Model.Enums.ElementySynchronizacji.Rabaty)]
    public interface IModulRabaty
    {
        void Przetworz(ref List<Rabat> rabatyNaB2B, ref List<ProduktUkryty> produktyUkryteNaB2B, ref List<Konfekcje> konfekcjaNaB2B, IDictionary<int, Model.Klient> kliencib2B, Dictionary<int, Produkt> produkty, List<PoziomCenowy> ceny, List<Cecha> cechy, Dictionary<long, ProduktCecha> cechyProdukty, Dictionary<int, KategoriaProduktu> kategorie, List<ProduktKategoria> produktyKategorie, ref IDictionary<int, KategoriaKlienta> kategorieKlientow, ref IDictionary<string, KlientKategoriaKlienta> klienciKategorie);
    }}
