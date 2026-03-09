using System;
using System.Collections.Generic;
using System.Linq;
using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces.Sync;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Zamowienia
{
    [FriendlyName("Rozbijanie towarów zamówienia po atrybucie",FriendlyOpis = "Moduł, który rozbija towary na kilka zamówień na podstawie atrybutu.")]
    public class RozbijanieTowarowPoAtrybucie : RozbiciaTowarowBaza
    {
        [FriendlyName("Atrybuty")]
        [PobieranieSlownika(typeof (SlownikAtrybutow))]
        [WidoczneListaAdmin(false, false, true, false)]
        public List<string> Atrybuty { get; set; }
        
        [FriendlyName("Próbuj ustawić magazyn realizujący na taki jak nazwa cechy rozbijanej")]
        [WidoczneListaAdmin(false, false, true, false)]
        public bool CzyUstawiacMagazyn { get; set; }

        public RozbijanieTowarowPoAtrybucie():base()
        {
            CzyUstawiacMagazyn = false;
        }

        public override string uwagi => "";
        /// <summary>
        ///  Rozbijanie pojedyńczej pozycji dla zamówienia
        /// </summary>
        /// <param name="pozycja"></param>
        /// <param name="atrybut"></param>
        /// <param name="cechyProduktyNaPlatfromie"></param>
        /// <param name="cechy"></param>
        /// <param name="zamowienieWejsciowe">Zamówienie wejściowe</param>
        /// <param name="slownikZamowien">Słownik z aktualnie rozbitymi zamówieniami</param>
        /// <param name="i">Nr pozycji</param>
        public void RozbijaniePozycji(ZamowienieProdukt pozycja, int atrybut, List<ProduktCecha> cechyProduktyNaPlatfromie, List<Cecha> cechy, ref ZamowienieSynchronizacja zamowienieWejsciowe, ref Dictionary<long, ZamowienieSynchronizacja> slownikZamowien, ref int i )
        {
            string powodRozbicia = string.Empty;
            Cecha cecha = Pobierzceche(atrybut,null, cechyProduktyNaPlatfromie, cechy, pozycja.ProduktIdBazowy);
            if (cecha != null)
            {
                if (!slownikZamowien.ContainsKey(cecha.Id))
                {
                    switch (Powod)
                    {
                        case SkadPowodyRozbicia.NazwaCechy:
                            powodRozbicia = cecha.Nazwa;
                            break;
                        case SkadPowodyRozbicia.SymbolCechy:
                            powodRozbicia = cecha.Symbol;
                            break;
                        case SkadPowodyRozbicia.NieUmieszczaj:
                            break;
                    }
                    
                    ZamowienieSynchronizacja z = zamowienieWejsciowe.StworzZamowieniaRozbite(slownikZamowien.Count + 1, powodRozbicia, DlugoscNumeru);
                    if (CzyUstawiacMagazyn && SymboleMagazynow.Any())
                    {
                        string magazyn = SymboleMagazynow.FirstOrDefault(x => x == cecha.Nazwa);
                        if (!string.IsNullOrEmpty(magazyn))
                        {
                            z.MagazynRealizujacy = magazyn;
                            Log.DebugFormat($"Dla zamowienia o numerze rozbicia: {z.NumerZRozbicia} przypisano magazyn realizujący o symbolu: {z.MagazynRealizujacy}");
                        }
                        else
                        {
                            Log.DebugFormat($"Włączone ustawienie: CzyUstawiacMagazyn natomiast nie znaleziono magazynu o symbolu: {cecha.Nazwa}");
                        }
                    }
                    Log.DebugFormat($"Stworzenie nowego zamówienia rozbijanego z cechy: {cecha} o numerze: {z.NumerZPlatformy}.");
                    slownikZamowien.Add(cecha.Id, z);
                }

                ZamowienieSynchronizacja zamowienie = slownikZamowien[cecha.Id];
                if (zamowienie.Uwagi != null && !zamowienie.Uwagi.StartsWith(cecha.Symbol))
                    zamowienie.Uwagi = string.Format(FormatUwag, zamowienie.Uwagi, cecha.Symbol);

                zamowienie.pozycje.Add(pozycja);
                zamowienieWejsciowe.pozycje.RemoveAt(i);
                i--;
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
            if (cechy.Count == 0 || cechyProduktyNaPlatfromie.Count == 0)
                throw new Exception("Nie udało się pobrać cech bądź cech produktów z platformy");

            if (!Atrybuty.Any())
            {
                Log.Error($"Brak atrubutów wybranych w module do rozbijania");
                return null;
            }

            Dictionary<long, ZamowienieSynchronizacja> slownikZamowien = new Dictionary<long, ZamowienieSynchronizacja>();
            cechy.ForEach(x => x.Symbol = x.Symbol.ToLower());
          
            if (Atrybuty.Any())
            {
                foreach (var a in Atrybuty)
                {
                    int idAtrybutu = int.Parse(a);
                    for (int i = 0; i < zamowienieWejsciowe.pozycje.Count; i++)
                    {
                        RozbijaniePozycji(zamowienieWejsciowe.pozycje[i], idAtrybutu, cechyProduktyNaPlatfromie, cechy, ref zamowienieWejsciowe, ref slownikZamowien, ref i);
                    }
                }
            }
            return slownikZamowien;
        }

        private List<string> _symboleMagazynow;
        private List<string> SymboleMagazynow => _symboleMagazynow ?? (_symboleMagazynow = Provider.PobierzMagazynyErp().Select(x => x.Symbol).ToList());

    }
}
