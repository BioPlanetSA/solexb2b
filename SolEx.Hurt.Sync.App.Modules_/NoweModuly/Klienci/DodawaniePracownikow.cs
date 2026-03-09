using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using System.Collections.Generic;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces.Sync;

using Adres = SolEx.Hurt.Model.Adres;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Klienci
{
    [FriendlyName("Dodawanie pracowników",FriendlyOpis = "Moduł, który na podstawie domeny maila przydziela pracowników lub klientów do określonej roli")]
    public class DodawaniePracownikow : SyncModul, Model.Interfaces.SyncModuly.IModulKlienci
    {       
        [FriendlyName("Domena, po której zostanie nadana rola dla pracownika lub klienta")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string Domena { get; set; }

        [FriendlyName("Rola, która będzie przypisana do klienta lub pracownika będącego w wybranej domenie")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public RoleType RolaKlienta {get;set;}

        public DodawaniePracownikow() 
        {
            Domena = string.Empty;
            RolaKlienta = RoleType.Klient;
        }

        public void Przetworz(ref Dictionary<long, Klient> listaWejsciowa, Dictionary<long, Produkt> produktyB2B, ref Dictionary<Adres, KlientAdres> adresyWErp, List<KategoriaKlienta> kategorie, List<KlientKategoriaKlienta> laczniki, ref List<Sklep> sklepy, ref List<SklepKategoriaSklepu> sklpeylaczniki, ref List<KategoriaSklepu> sklepyKategorie, ref List<Kraje> kraje, ref List<Region> regiony, ref List<Magazyn> magazyny, ISyncProvider provider)
        {
            if (string.IsNullOrEmpty(Domena))
                return;

            Domena = Domena.ToLower();
           
            foreach (Klient k in listaWejsciowa.Values)
            {
                if (!k.Aktywny || string.IsNullOrEmpty(k.Email))
                {
                    continue;
                }
                if (k.Email.ToLower().EndsWith(Domena))
                {
                    if(!k.Role.Contains(RolaKlienta))
                        k.Role.Add(RolaKlienta);
                }
            }
        }
    }
}
