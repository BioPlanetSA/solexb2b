using System.Collections.Generic;
using System.Linq;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Interfaces.Sync;

using Adres = SolEx.Hurt.Model.Adres;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Klienci
{
    [ModulStandardowy]
    [FriendlyName("Dodaj brakujące magazyny", FriendlyOpis = "Dodaje brakujace magazyny używane przez klientów do B2b")]
    public class DodajBrakujaceMagazyny : SyncModul, Model.Interfaces.SyncModuly.IModulKlienci
    {
        public void Przetworz(ref Dictionary<long, Klient> listaWejsciowa, Dictionary<long, Produkt> produktyB2B, ref Dictionary<Adres, KlientAdres> adresyWErp, List<KategoriaKlienta> kategorie, List<KlientKategoriaKlienta> laczniki, ref List<Sklep> sklepy, ref List<SklepKategoriaSklepu> sklpeylaczniki, ref List<KategoriaSklepu> sklepyKategorie, ref List<Kraje> kraje, ref List<Region> regiony, ref List<Magazyn> magazyny, ISyncProvider provider)
        {
            Dictionary<int, Magazyn> wynik = new Dictionary<int, Magazyn>();
            var magazynyNaPlatformie = ApiWywolanie.PobierzMagazyny();
            Dictionary<string, int> symboleNaPlatformie = new Dictionary<string, int>();

            foreach (var mag in magazynyNaPlatformie)
            {
                var magaz = mag.Symbol.Split('+');
                foreach (var m in magaz)
                {
                    symboleNaPlatformie.Add(m, mag.Id);
                }
            }

            var magazynyErp = provider.PobierzMagazynyErp().ToDictionary(x=>x.Symbol.Trim(),x=>x);
            
            foreach (var klient in listaWejsciowa.Values)
            {
                if (klient.DostepneMagazyny == null)
                {
                    continue;
                }
                foreach (var s in klient.DostepneMagazyny)
                {
                    var symbol = s.Trim();
                    Magazyn mag = magazynyErp[symbol];
                    if (symboleNaPlatformie.ContainsKey(symbol) && symboleNaPlatformie[symbol] <= 0)
                    {
                        continue;
                    }
                    
                    if (!wynik.ContainsKey(mag.Id))
                    {
                        wynik.Add(mag.Id,mag);
                    }
                }
            }
            magazyny = wynik.Values.ToList();
        }
    }
}
