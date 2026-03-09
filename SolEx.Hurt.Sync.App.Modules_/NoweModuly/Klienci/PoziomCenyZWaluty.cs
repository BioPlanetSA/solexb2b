using System;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model;
using System.Collections.Generic;
using System.Linq;
using SolEx.Hurt.Model.Interfaces.Sync;
using SolEx.Hurt.Model.Web;
using Adres = SolEx.Hurt.Model.Adres;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Klienci
{
    
    class PoziomCenyZWaluty : SyncModul, Model.Interfaces.SyncModuly.IModulKlienci
    {
        public override string uwagi
        {
            get { return ""; }
        }


        public override string Opis
        {
            get { return "Ustawia poziom cenowy na podstawie waluty z karty klienta w ERP"; }
        }

        public void Przetworz(ref Dictionary<long, Klient> listaWejsciowa, Dictionary<long, Produkt> produktyB2B, ref Dictionary<Adres, KlientAdres> adresyWErp, List<KategoriaKlienta> kategorie, List<KlientKategoriaKlienta> laczniki, ref List<Sklep> sklepy, ref List<SklepKategoriaSklepu> sklpeylaczniki, ref List<KategoriaSklepu> sklepyKategorie, ref List<Kraje> kraje, ref List<Region> regiony, ref List<Magazyn> magazyny, ISyncProvider provider)
        {
            var poziomyCen = ApiWywolanie.PobierzPoziomyCen();

            foreach (Klient item in listaWejsciowa.Values)
            {
                if (item.WalutaId!=null)
                {
                    PoziomCenowy poziomCeny = poziomyCen.Values.FirstOrDefault(a => a.WalutaId==item.WalutaId);
                    if (poziomCeny != null)
                    {
                        item.PoziomCenowyId = poziomCeny.Id;
                    }
                }
            }
        }

    }

}
