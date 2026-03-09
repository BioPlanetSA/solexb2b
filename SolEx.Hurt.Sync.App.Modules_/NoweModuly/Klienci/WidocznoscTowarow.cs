using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using System;
using System.Collections.Generic;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Interfaces.Sync;
using Adres = SolEx.Hurt.Model.Adres;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Klienci
{
    [FriendlyName("Widocznosc Towarow", FriendlyOpis = "Moduł, który ustawia widoczność towarów dla wszystykich klientów")]
    public class WidocznoscTowarow : SyncModul, Model.Interfaces.SyncModuly.IModulKlienci
    {
        public enum Widocznosc { PelnaOferta = 1, TylkoMojKatalog = 0 }

        [FriendlyName("Typ widoczności ustawiany dla każdego klienta")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public Widocznosc TypWidocznosci { get; set; }

        public override string uwagi
        {
            get { return ""; }
        }
            
        public WidocznoscTowarow() 
        {
            TypWidocznosci = Widocznosc.TylkoMojKatalog;
        }

     
        /// <summary>
        /// </summary>
        /// <param name="listaWejsciowa"></param>
        /// <param name="produktyB2B"></param>
        /// <param name="adresyWErp"></param>
        /// <param name="kategorie"></param>
        /// <param name="laczniki"></param>
        /// <param name="sklepy"></param>
        /// <param name="sklpeylaczniki"></param>
        /// <param name="sklepyKategorie"></param>
        /// <param name="kraje"></param>
        /// <param name="regiony"></param>
        /// <param name="magazyny"></param>
        /// <param name="provider"></param>
        public void Przetworz(ref Dictionary<long, Klient> listaWejsciowa, Dictionary<long, Produkt> produktyB2B, ref Dictionary<Adres, KlientAdres> adresyWErp, List<KategoriaKlienta> kategorie, List<KlientKategoriaKlienta> laczniki, ref List<Sklep> sklepy, ref List<SklepKategoriaSklepu> sklpeylaczniki, ref List<KategoriaSklepu> sklepyKategorie, ref List<Kraje> kraje, ref List<Region> regiony, ref List<Magazyn> magazyny, ISyncProvider provider)
        {
            foreach (Klient k in listaWejsciowa.Values) 
            {
                k.PelnaOferta = Convert.ToBoolean(TypWidocznosci);
            }
        }
    }
}
