using System;
using System.Linq;
using SolEx.Hurt.Model;
using System.Collections.Generic;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Sync.App.Modules_.Helpers;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Zamowienia
{
    [FriendlyName("Rozbijanie towarów wg stanu i dostępności",FriendlyOpis = "Moduł, który rozbija towary na kilka zamówień na podstawie ich stanów w magazynie.")]
    public class RozbiciaTowarowOdDostepnosci : RozbiciaTowarowBaza
    {
        [FriendlyName("Minimalny stan poniżej którego towar zostanie dodany do osobnego zamówienia.", FriendlyOpis = "Minimalny stan magazynowy towaru poniżej którego towar jest zawsze przenoszony do nowego zamówienia")]
        [WidoczneListaAdmin(false, false, true, false)]
        public decimal MinimalnyStan { get; set; }

        [FriendlyName("Suffix dodawany do numeru zamówienia i uwag opisujący powód rozbicia.")]
        [WidoczneListaAdmin(false, false, true, false)]
        public string Suffix { get; set; }
        
        [Niewymagane]
        [FriendlyName("ID magazynów z których zostaną pobrane stany - oddzielone średnikiem", FriendlyOpis = "Pole niewymagane. <br/>Jeśli puste pobiera stany z wszystkich magazynów z B2B.")]
        [WidoczneListaAdmin(false, false, true, false)]
        public string IdMagazynow { get; set; }

        public RozbiciaTowarowOdDostepnosci():base()
        {
            MinimalnyStan = -1;
            Suffix = "DOST";
            IdMagazynow = "";
        }
        /// <summary>
        /// Rozbijanie pojedyńczej pozycji dla zamówienia
        /// </summary>
        /// <param name="pozycja"></param>
        /// <param name="produktyB2B"></param>
        /// <param name="listastany"></param>
        /// <param name="zamowienieWejsciowe">Zamówienie wejściowe</param>
        /// <param name="slownikZamowien">Słownik z aktualnie rozbitymi zamówieniami</param>
        /// <param name="i">Nr pozycji</param>
        public void RozbijaniePozycji(ZamowienieProdukt pozycja, Dictionary<long, Produkt> produktyB2B,Dictionary<long, decimal> listastany, ref ZamowienieSynchronizacja zamowienieWejsciowe, ref Dictionary<long, ZamowienieSynchronizacja> slownikZamowien, ref int i)
        {
            Produkt produkt;
            if (!produktyB2B.TryGetValue(pozycja.ProduktId, out produkt))
            {
                return;
            }
            //jeśli pobrany produkt jest usługą to pomijamy pozycję
            if (produkt.Typ == TypProduktu.Usluga) return;
            if (!slownikZamowien.ContainsKey(1))
            {
                ZamowienieSynchronizacja z = zamowienieWejsciowe.StworzZamowieniaRozbite(slownikZamowien.Count + 1, Suffix, DlugoscNumeru);
                Log.DebugFormat($"Stworzenie nowego zamówienia rozbijanego  o numerze: {z.NumerZPlatformy}.");
                slownikZamowien.Add(1, z);
            }

            ZamowienieSynchronizacja nowezamowienie = slownikZamowien[1];
            if ((nowezamowienie.Uwagi != null && !nowezamowienie.Uwagi.StartsWith(Suffix)) || nowezamowienie.Uwagi == null)
            {
                nowezamowienie.Uwagi = string.Format(FormatUwag, nowezamowienie.Uwagi, Suffix);
            }
            //pobieramy stan dla produktu
            decimal stan;
            listastany.TryGetValue(produkt.Id, out stan);

            if ((MinimalnyStan > -1 && stan < MinimalnyStan) || stan < pozycja.Ilosc)
            {
                decimal staraIlosc = pozycja.Ilosc;
                decimal nowaIlosc = staraIlosc - (stan < 0 ? 0 : stan);
                //tworzenie pozycji zamówienia
                ZamowienieProdukt nowyProdukt = new ZamowienieProdukt(pozycja)
                {
                    Ilosc = nowaIlosc
                };
                //dodawanie pozycji do zamówenia
                nowezamowienie.pozycje.Add(nowyProdukt);

                if (nowaIlosc > 0 && stan > 0)
                {
                    pozycja.Ilosc = stan;
                }
                else
                {
                    zamowienieWejsciowe.pozycje.RemoveAt(i);
                    i--;
                }
            }
        }
        /// <summary>
        /// Rozbijanie zamówienia
        /// </summary>
        /// <param name="zamowienieWejsciowe"></param>
        /// <param name="cechy"></param>
        /// <param name="cechyProduktyNaPlatfromie"></param>
        /// <param name="produktyB2B"></param>
        /// <returns></returns>
        public override Dictionary<long, ZamowienieSynchronizacja> RozbijZamowienie(ref ZamowienieSynchronizacja zamowienieWejsciowe, List<Cecha> cechy, List<ProduktCecha> cechyProduktyNaPlatfromie, Dictionary<long, Produkt> produktyB2B)
        {
            Dictionary<long, decimal> listastany = PobierzStanyZMagazynow();

            Dictionary<long, ZamowienieSynchronizacja> slownikZamowien = new Dictionary<long, ZamowienieSynchronizacja>();
            for (int i = 0; i < zamowienieWejsciowe.pozycje.Count; i++)
            {
                RozbijaniePozycji(zamowienieWejsciowe.pozycje[i], produktyB2B, listastany, ref zamowienieWejsciowe, ref slownikZamowien, ref i);

            }
            return slownikZamowien;
        }

        /// <summary>
        /// Pobieranie stanów z wybranych magazynów 
        /// </summary>
        /// <returns></returns>
        public Dictionary<long, decimal> PobierzStanyZMagazynow()
        {
            //pobieramy id magazynów wybranych w module
            string[] magazynyId = IdMagazynow.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
            //pobieranie magazynów z platformy
            List<Magazyn> listaMagazynowPlatformy = ApiWywolanie.PobierzMagazyny().Where(m => (magazynyId.Length > 0 && magazynyId.Any(mi => mi == m.Id.ToString())) || (magazynyId.Length == 0)).ToList();

            Dictionary<long, decimal> wynik = new Dictionary<long, decimal>();

            foreach (Magazyn mag in listaMagazynowPlatformy)
            {
                //jeśli magazyn mamy importować z erpa to stany pobieramy z erp-a przez provider
                //w przeciwnym wypadku stany pobieramy przez api z platformy
                Dictionary<long, decimal> stany = mag.ImportowacZErp
                    ? StanyHelpers.PobierzStanyDlaMagazynow(Provider, mag)
                    : ApiWywolanie.PobierzStanyProduktow(mag).ToDictionary(a => a.ProduktId, b => b.Stan);
                //dodajemy stan do wyniku
                foreach (KeyValuePair<long, decimal> keyValuePair in stany)
                {
                    if (!wynik.ContainsKey(keyValuePair.Key))
                    {
                        wynik.Add(keyValuePair.Key, keyValuePair.Value);
                    }
                    else
                    {
                        wynik[keyValuePair.Key] += keyValuePair.Value;
                    }
                }
            }
            return wynik;
        }
    }
}


