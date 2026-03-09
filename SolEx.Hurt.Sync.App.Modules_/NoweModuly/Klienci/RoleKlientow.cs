using System;
using System.Linq;
using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using System.Collections.Generic;
using System.Threading.Tasks;
using ServiceStack.Common.Extensions;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;
using SolEx.Hurt.Model.Interfaces.Sync;
using Adres = SolEx.Hurt.Model.Adres;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Klienci
{
    [FriendlyName("Role klientów",FriendlyOpis = "Moduł, który automatycznie przydziela kontrahenów do określonej roli na podstawie cechy")]
    public class RoleKlientow : SyncModul, Model.Interfaces.SyncModuly.IModulKlienci, ITestowalna
    {
        [FriendlyName("Kategoria Klienta")]
        [PobieranieSlownika(typeof(SlownikKategoriiKlienta))]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public HashSet<int> KategoriaKlienta { get; set; }

        [FriendlyName("Rola, która będzie przypisana do klienta posiadającego wybraną cechę")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public RoleType RolaKlienta {get;set;}

        public RoleKlientow() 
        {
            RolaKlienta = RoleType.Klient;
        }

        public void Przetworz(ref Dictionary<long, Klient> listaWejsciowa, Dictionary<long, Produkt> produktyB2B, ref Dictionary<Adres, KlientAdres> adresyWErp, List<KategoriaKlienta> kategorie, List<KlientKategoriaKlienta> laczniki,
                              ref List<Sklep> sklepy, ref List<SklepKategoriaSklepu> sklpeylaczniki, ref List<KategoriaSklepu> sklepyKategorie, ref List<Kraje> kraje, ref List<Region> regiony, ref List<Magazyn> magazyny,ISyncProvider provider)
        {
            if (KategoriaKlienta == null || KategoriaKlienta.IsEmpty())
            {
                throw new NullReferenceException("W module RoleKlientow pole Kategoria klienta nie posiada wartości ");
            }

            HashSet<long> klienciZWlasciwymiLacznikami =new HashSet<long>( laczniki.Where(x => this.KategoriaKlienta.Contains(x.KategoriaKlientaId)).Select(x => x.KlientId) );

            Parallel.ForEach(listaWejsciowa, k =>
            {
                if (klienciZWlasciwymiLacznikami.Contains(k.Key))
                {
                    Log.DebugFormat($"Dodaje rolę: {RolaKlienta} dla Klienta :{k.Value.Email}");
                    if (!k.Value.Role.Contains(RolaKlienta))
                    {
                        k.Value.Role.Add(RolaKlienta);
                    }
                }
            });
        }


        public List<string> TestPoprawnosci()
        {
            List<string> bledy = new List<string>();
            if (KategoriaKlienta == null ||  KategoriaKlienta.IsEmpty())
            {
                bledy.Add("Pole kategoria klienta jest puste");
            }
            return bledy;
        }
    }
}
