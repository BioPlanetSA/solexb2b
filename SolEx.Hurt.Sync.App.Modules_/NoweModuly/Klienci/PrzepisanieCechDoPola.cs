using System.Reflection;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Extensions;
using log4net;
using SolEx.Hurt.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ServiceStack.Common.Extensions;
using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.Sync;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Interfaces.Sync;

using Adres = SolEx.Hurt.Model.Adres;
using StringExtensions = ServiceStack.Text.StringExtensions;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Klienci
{

    [FriendlyName("Przepisanie cechy do pola klienta", FriendlyOpis = "Określa które atrybuty mają być przepisane do wybranego pola w kliencie. Cechy, które mają wartość tak/nie lub yes/no mogą być użyte dla pól boolowskich.")]
    public class PrzepisanieCechDoPola : SyncModul, Model.Interfaces.SyncModuly.IModulKlienci
    {
        public PrzepisanieCechDoPola()
        {
            Pola = new List<string>();
            Atrybut = string.Empty;
            PrzepisujJesliWartoscNieJestPusta = false;
        }

        public SyncManager SyncManager = SyncManager.PobierzInstancje;

        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        [FriendlyName("Pola, do których będą przepisane wybrane atrybuty")]
        [SyncSlownikNaPodstawieInnegoTypu(typeof(Klient))]
        [WidoczneListaAdmin(false, false, true, false)]
        public List<string> Pola { get; set; }

        [FriendlyName("Atrybut kategorii klientów, z którego wartość będzie wpisana do wybranego pola do klienta")]
        [PobieranieSlownika(typeof(SlownikGrupyKategoriiKlienta))]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string Atrybut { get; set; }

        [FriendlyName("Przepisuj atrybut tylko jeśli wartość atrybutu jest nie pusta. W przypadku pustych wartości nie przepisuj atrybutu (pomijaj).")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public bool PrzepisujJesliWartoscNieJestPusta { get; set; }

        public void Przetworz(ref Dictionary<long, Klient> listaWejsciowa, Dictionary<long, Produkt> produktyB2B, ref Dictionary<Adres, KlientAdres> adresyWErp, List<KategoriaKlienta> kategorie, List<KlientKategoriaKlienta> laczniki,
                              ref List<Sklep> sklepy, ref List<SklepKategoriaSklepu> sklpeylaczniki, ref List<KategoriaSklepu> sklepyKategorie, ref List<Kraje> kraje, ref List<Region> regiony, ref List<Magazyn> magazyny,
                              ISyncProvider provider)
        {
            if (Pola.IsEmpty() || string.IsNullOrEmpty(Atrybut))
            {
                throw new Exception("Brak pola do wpisania do klienta, lub atrybutu z ktorego pobierać cechy.");
            }

            List<PropertyInfo> propertisy = typeof(Klient).Properties().WhereKeyIsIn(this.Pola);

            if (propertisy.IsEmpty())
            {
                throw new Exception("Brak pól dla klienta do wpisania - w modułe są złe pole wpisane (które już nie istnieją), popraw konfigurację modułu.");
            }

            var akcesor = typeof(Klient).PobierzRefleksja();

            Dictionary<int, KategoriaKlienta> listaKategoriiKlientowPasujacych = KategorieKlientowWyszukiwanie.PobierzInstancje.FiltrujKategorieWgGrupyLubCechy(kategorie, Atrybut, true);
            Dictionary<long, List<KlientKategoriaKlienta>> lacznikiPoFiltracji = laczniki.Where(x => listaKategoriiKlientowPasujacych.ContainsKey(x.KategoriaKlientaId)).GroupBy(x => x.KlientId).ToDictionary(x => x.Key, x => x.ToList());

            LogiFormatki.PobierzInstancje.LogujInfo($"Dopasowano {lacznikiPoFiltracji.Count} klientó do zmiany. Pasujących kategorii wg. filtra atrybutów: {Atrybut}  - ilość: {listaKategoriiKlientowPasujacych.Count}");

            Parallel.ForEach(listaWejsciowa.Values, klient =>
            {
                List<KlientKategoriaKlienta> kategorieKlienta;
                string doWpisaniaWartosc = null;
                if (!lacznikiPoFiltracji.TryGetValue(klient.Id, out kategorieKlienta))
                {
                    //brak lacznika - brak kategorii dla klienta
                    if (PrzepisujJesliWartoscNieJestPusta)
                    {
                        return;
                    }
                }
                else
                {
                    //jelsi jest wybranych wiecej niz 1 kategoria to blad - bo nie ma jak tego wpisac
                    if (kategorieKlienta.Count > 1)
                    {
                        LogiFormatki.PobierzInstancje.LogujInfo(
                            $"Błąd dla klienta id: {klient.Id} mail: {klient.Email}, na podstawie atrybuty: {Atrybut}, pasuje kilka kategorii - moduł wymaga aby klient miał tylko jedną cechę w danym atrybucie. Wyciągnięte kategorie: {StringExtensions.ToCsv(kategorieKlienta)}. Klient zostanie pominięty.");
                        return;
                    }
                    doWpisaniaWartosc = listaKategoriiKlientowPasujacych[kategorieKlienta[0].KategoriaKlientaId].Nazwa;
                }

                foreach (var p in propertisy)
                {
                    try
                    {
                        akcesor[klient, p.Name] = doWpisaniaWartosc;
                    } catch (Exception e)
                    {
                        throw new Exception($"Nie udało się ustawić pola dla klienta: {p.Name}. Błąd refleksji: {e.Message}", e);
                    }
                }
            });
        }
    }
}
