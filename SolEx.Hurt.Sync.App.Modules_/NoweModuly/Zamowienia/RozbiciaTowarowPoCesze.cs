using System;
using System.Linq;
using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Model;
using System.Collections.Generic;
using ServiceStack.Common.Extensions;
using SolEx.Hurt.Core.Sync;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Zamowienia
{
    [FriendlyName("Rozbijanie pozycji zamówień po cesze.", FriendlyOpis = "Moduł, który rozbija towary na kilka zamówień na podstawie cech.")]
    public class RozbiciaTowarowPoCesze : RozbiciaTowarowBaza
    {
        [FriendlyName("Cechy")]
        [PobieranieSlownika(typeof(SlownikCech))]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public List<string> Cechy { get; set; }

        public RozbiciaTowarowPoCesze():base()
        {
            Powod = SkadPowodyRozbicia.NazwaCechy;
        }
        
        /// <summary>
        /// Rozbijanie zamówienia
        /// </summary>
        /// <param name="zamowienieWejsciowe"></param>
        /// <param name="cechy"></param>
        /// <param name="cechyProduktyNaPlatfromie"></param>
        /// <param name="produktyB2B"></param>
        /// <returns></returns>
        public override Dictionary<long, ZamowienieSynchronizacja> RozbijZamowienie(ref ZamowienieSynchronizacja zamowienieWejsciowe, List<Cecha> cechy, 
            List<ProduktCecha> cechyProduktyNaPlatfromie, Dictionary<long, Produkt> produktyB2B)
        {
            if (!cechy.Any() || !cechyProduktyNaPlatfromie.Any())
                throw new Exception("Nie udało się pobrać cech bądź cech produktów z platformy");
            if (!Cechy.Any())
            {
                Log.Error($"Brak cech wybranych w module do rozbijania");
                return null;
            }

            long[] wybraneCechyId = Cechy.Select(x => long.Parse(x)).ToArray();

            Dictionary<Cecha, HashSet<long>> slownikProduktowICech = cechyProduktyNaPlatfromie.Where(x => wybraneCechyId.Contains(x.CechaId))
                .GroupBy(x => x.CechaId).ToDictionary(x => cechy.First(z=> z.Id == x.Key), x => new HashSet<long>( x.Select(z=> z.ProduktId) ));
         
            Dictionary<long, ZamowienieSynchronizacja> slownikZamowien = new Dictionary<long, ZamowienieSynchronizacja>();
            cechy.ForEach(x => x.Symbol = x.Symbol.ToLower());
            
            foreach (KeyValuePair<Cecha, HashSet<long>> c in slownikProduktowICech)
            {
                for (int i = 0; i < zamowienieWejsciowe.pozycje.Count; i++)
                {
                    //czy ten produkt ma ta ceche
                    if (c.Value.Contains(zamowienieWejsciowe.pozycje[i].ProduktIdBazowy))
                    {
                        RozbijaniePozycji(zamowienieWejsciowe.pozycje[i], c.Key, ref zamowienieWejsciowe, ref slownikZamowien, ref i);
                    }
                }
            }
            return slownikZamowien;
        }

        /// <summary>
        ///  Rozbijanie pojedyńczej pozycji dla zamówienia
        /// </summary>
        /// <param name="pozycja"></param>
        /// <param name="idCechy"></param>
        /// <param name="cechyProduktyNaPlatfromie"></param>
        /// <param name="cechy"></param>
        /// <param name="zamowienieWejsciowe">Zamówienie wejściowe</param>
        /// <param name="slownikZamowien">Słownik z aktualnie rozbitymi zamówieniami</param>
        /// <param name="i">Nr pozycji</param>
        public void RozbijaniePozycji(ZamowienieProdukt pozycja, Cecha cechaRozbijana, ref ZamowienieSynchronizacja zamowienieWejsciowe, 
            ref Dictionary<long, ZamowienieSynchronizacja> slownikZamowien, ref int i)
        {
            string powodRozbicia = string.Empty;
            if (!slownikZamowien.ContainsKey(cechaRozbijana.Id))
            {
                switch (Powod)
                {
                    case SkadPowodyRozbicia.NazwaCechy:
                        powodRozbicia = cechaRozbijana.Nazwa;
                        break;
                    case SkadPowodyRozbicia.SymbolCechy:
                        powodRozbicia = cechaRozbijana.Symbol;
                        break;
                    case SkadPowodyRozbicia.NieUmieszczaj:
                        break;
                }

                ZamowienieSynchronizacja noweZamowienieRozbite = null;

                if (zamowienieWejsciowe is ZamowieniaImport zamowienieZPliku)
                {
                    noweZamowienieRozbite = zamowienieZPliku.StworzZamowieniaRozbite(slownikZamowien.Count + 1, powodRozbicia, DlugoscNumeru);
                    if (!(noweZamowienieRozbite is ZamowieniaImport))
                    {
                        throw new Exception($"Zamówienie wejściowe jest typu ZamowieniaImport wiec rozbite też ma być takiego!!");
                    }
                }
                else
                {
                    noweZamowienieRozbite = zamowienieWejsciowe.StworzZamowieniaRozbite(slownikZamowien.Count + 1, powodRozbicia, DlugoscNumeru);
                }

                LogiFormatki.PobierzInstancje.LogujInfo($"Stworzenie nowego zamówienia rozbijanego z cechy: {cechaRozbijana.Nazwa} o numerze: {noweZamowienieRozbite.NumerZPlatformy} - numer z rozbicia: " +
                                                        $"{noweZamowienieRozbite.NumerZRozbicia}, tymczasowy numer: {noweZamowienieRozbite.NumerTymczasowyZamowienia}. Pierwotny numer: {zamowienieWejsciowe.NumerZPlatformy}");
                
                slownikZamowien.Add(cechaRozbijana.Id, noweZamowienieRozbite);
            }

            ZamowienieSynchronizacja zamowienie = slownikZamowien[cechaRozbijana.Id];
            if ((zamowienie.Uwagi != null && !zamowienie.Uwagi.StartsWith(cechaRozbijana.Symbol)) || zamowienie.Uwagi == null)
            {
                zamowienie.Uwagi = string.Format(FormatUwag, zamowienie.Uwagi, cechaRozbijana.Symbol);
            }
            zamowienie.pozycje.Add(pozycja);
            zamowienieWejsciowe.pozycje.RemoveAt(i);
            i--;
        }
    }
}
