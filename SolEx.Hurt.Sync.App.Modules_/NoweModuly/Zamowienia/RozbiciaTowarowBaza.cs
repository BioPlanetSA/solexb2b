using System.Linq;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using System.Collections.Generic;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces.Sync;
using SolEx.Hurt.Model.Interfaces.SyncModuly;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Zamowienia
{
    public abstract class RozbiciaTowarowBaza : SyncModul, IModulZamowienia
    {
        [FriendlyName("Wartość dodawana do numeru zamówniena z rozbicia")]
        [WidoczneListaAdmin(false, false, true, false)]
        public SkadPowodyRozbicia Powod { get; set; }

        [FriendlyName("Ilość znaków dla numeru rozbicia zamówienia")]
        [WidoczneListaAdmin(false, false, true, false)]
        public int DlugoscNumeru { get; set; }

        public ISyncProvider Provider;
        protected RozbiciaTowarowBaza()
        {
            FormatUwag = "{1} {0}";
            DlugoscNumeru = 20;
        }

        public override string uwagi => "";

        [FriendlyName("Format uwag jakie mają być zapisane z info o rozbiciu",FriendlyOpis = "za {0} podstawiamy uwagi do zamówienia za {1} podstawiamy nazwę cechy po której jest rozbicie")]
        [WidoczneListaAdmin(false, false, true, false)]
        public string FormatUwag { get; set; }

        public abstract Dictionary<long, ZamowienieSynchronizacja> RozbijZamowienie(ref ZamowienieSynchronizacja zamowienieWejsciowe, List<Cecha> cechy, List<ProduktCecha> cechyProduktyNaPlatfromie, Dictionary<long, Produkt> produktyB2B);

        public void Przetworz(ZamowienieSynchronizacja zamowienieWejsciowe, ref List<ZamowienieSynchronizacja> wszystkie, ISyncProvider provider, Dictionary<long, Jednostka> jednostki, Dictionary<long, ProduktJednostka> laczniki, 
            Dictionary<long, Produkt> produktyB2B, List<Cecha> cechy, List<ProduktCecha> cechyProduktyNaPlatfromie)
        {
            if (!zamowienieWejsciowe.Rozbijaj)
                return;

            if (zamowienieWejsciowe.PochodziZRozbicia)
            {
                Log.InfoFormat($"Zamówienie {zamowienieWejsciowe.NumerZPlatformy} już pochodzi z rozbicia. Moduł przerwie działanie.");
                return;
            }

            Provider = provider;
            //rozbijamy zamówienie
            var slownikZamowien = RozbijZamowienie(ref zamowienieWejsciowe, cechy, cechyProduktyNaPlatfromie,produktyB2B);

            //została tylko dostawa więc też ją przenosimy do pierwszego zamówienia rozbitego
            if (zamowienieWejsciowe.pozycje.Count == 1)
            {
                Produkt p;
                if (produktyB2B.TryGetValue(zamowienieWejsciowe.pozycje[0].ProduktId, out p))
                {
                    if (p.Typ == TypProduktu.Usluga)
                    {
                        slownikZamowien.First().Value.pozycje.Add((zamowienieWejsciowe.pozycje[0]));
                        zamowienieWejsciowe.pozycje.RemoveAt(0);
                    }
                }
            }

            //wszystkie pozycje zostały rozbite do osobnych zamówień
            if (!zamowienieWejsciowe.pozycje.Any())
            {
                if (slownikZamowien.Any())
                {
                    var tmp = wszystkie.FirstOrDefault(a => a.NumerZPlatformy == zamowienieWejsciowe.NumerZPlatformy);
                    if (tmp != null)
                    {
                        wszystkie.Remove(tmp);
                        //tutaj nie można zrobić przepisania całego obiektu zamówienia bo zginie typ tzn potencjalnie tutaj może być typ zamówienia utworzony przez moduł comarchconnector 
                     // zamowienieWejsciowe = slownikZamowien.First().Value;
                     // wszystkie.Add(zamowienieWejsciowe);
                    }
                   // slownikZamowien = slownikZamowien.Skip(1).ToDictionary(a => a.Key, a => a.Value);
                }
            }

            if (wszystkie.Any())
            {
                var pierwsze = wszystkie.First();
                //dopisujemy nr z rozbicia jesli w zamóweniu nie ma, z nie ma jeśli w zamówieniu wejściowym zostały jakieś pozycje
                if (string.IsNullOrEmpty(wszystkie.First().NumerZRozbicia))
                {
                    wszystkie.First().NumerZRozbicia = $"{pierwsze.NumerTymczasowyZamowienia}/{slownikZamowien.Count + 1}/";
                }
                //sprawdzamy długość nr z rozbicia
                if (pierwsze.NumerZRozbicia.Length > DlugoscNumeru)
                {
                    pierwsze.NumerZRozbicia = wszystkie[0].NumerZRozbicia.Substring(0, DlugoscNumeru);
                }
            }
            
            wszystkie.AddRange(slownikZamowien.Values);
            //foreach (KeyValuePair<long, ZamowienieSynchronizacja> zamowienieSynchronizacja in slownikZamowien)
            //{
            //    wszystkie.Add(zamowienieSynchronizacja.Value);
            //}

            if (!wszystkie.Any()) return;

            //przypisujemy liczbę łacznie rozbitych zamówień 
            //zamowienieWejsciowe.LacznieRozbitych = wszystkie.Count;
            foreach (var zamowienieSynchronizacja in wszystkie)
            {
                zamowienieSynchronizacja.LacznieRozbitych = wszystkie.Count;
            }
        }

        /// <summary>
        /// Pobieramy cechę wg id atrybutu lub jesli nie podamy to wg id cechy która jest przypisana do produktu
        /// </summary>
        /// <param name="atrybut">Id atrybutu szukanego</param>
        /// <param name="idCechy">Id cechy szukanej</param>
        /// <param name="cechyprodukty">Łaczniki cech i produktów</param>
        /// <param name="cechy">Lista cech</param>
        /// <param name="pId">Id produktu dla którego szukamy cechy</param>
        /// <returns></returns>
        protected Cecha Pobierzceche(int? atrybut, long? idCechy, List<ProduktCecha> cechyprodukty, List<Cecha> cechy, long pId)
        {
            List<long> idCechDlaProduktu = cechyprodukty.Where(x => x.ProduktId == pId).Select(x => x.CechaId).ToList();
            if (!idCechDlaProduktu.Any())
            {
              //  Log.Error($"Produkt o Id [{pId}] nie posiada przypisanych cech");
                return null;
            }

            Cecha cecha = null;
            if (atrybut.HasValue)
            {
                cecha = cechy.FirstOrDefault(x => x.AtrybutId == atrybut && idCechDlaProduktu.Contains(x.Id));
                return cecha;
            }

            if (!idCechy.HasValue)
            {
                return null;
            }

            cecha = cechy.FirstOrDefault(x => x.Id == idCechy.Value && idCechDlaProduktu.Contains(x.Id));
            return cecha;
        }
    }
}
