using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ServiceStack.Text;
using SolEx.Hurt.Core.Sync;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Interfaces.Sync;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Zamowienia
{
    [FriendlyName("Łączenie nowych zamówień z zamówieniami z ERP", FriendlyOpis = "Moduł łączy nowe zamówienia z niezrealizowanymi zamówieniami z Erp-a Subiekta")]
    public class LaczenieZamowienSubiekt : LaczenieZamowienBaza
    {
        [FriendlyName("Flaga zamówień do których nie będą dołączane nowe zamówienia - po średniku można dodać wiele flag")]
        [WidoczneListaAdmin(false, false, true, false)]
        public string FlagaWykluczjaca { get; set; }

        [FriendlyName("Flaga jaką ustawiać dla zamówień połączonych", FriendlyOpis = "USUN - Usuwa całkowicie flagę z zamówienia.")]
        [WidoczneListaAdmin(false, false, true, false)]
        public string FlagaPoPolaczeniu { get; set; }

        [FriendlyName("Uwagi zamówień do których nie będą dołączane nowe zamówienia", FriendlyOpis = "Uwagi które wykluczą zamówienie. Różne frazy mogą być rozdzielone spacją np.: <br/>" +
                                                                                                     "\"r:% Import%\" Wykluczy uwagi które mają r: lub Import na początku, \"%kotek% %Młotek\" Wykluczy uwagi które zawierają kotek lub Młotek na końcu<br/>" +
                                                                                                     "Mogą zawierać znaki wieloznaczności:<br/>" +
                                                                                                     "% -Dowolny ciąg znaków.<br/>_ (podkreślenie) - Dowolny pojedynczy znak.<br/>" +
                                                                                                     "[] - Dowolny pojedynczy znak w ramach określonego zakres([a-f]) lub zestaw([abcdef]).<br/>" +
                                                                                                     "[^] - Dowolny pojedynczy znak nie w ramach określonego zakres([^a-f]) lub zestaw([^abcdef]).")]
        [WidoczneListaAdmin(false, false, true, false)]
        public string UwagiWykluczenie { get; set; }

        public LaczenieZamowienSubiekt() : base()
        {
            FlagaPoPolaczeniu = "USUN";
        }

        private ISyncProvider _provider;

        public override void PrzetworzZamowienie(ref ZamowienieSynchronizacja zamowienieWejsciowe, ISyncProvider provider)
        {
            Log.Info($"Przetwarzam zamówienie: {zamowienieWejsciowe.NumerZPlatformy}");
            _provider = provider;
            string zapytanie = PrzygotujZapytanie(zamowienieWejsciowe);
            List<long> idDokumentow = PobierIdDokumnetow(zapytanie);
            if (idDokumentow.Any())
            {
                zamowienieWejsciowe.IdZamowieniaDoPolaczenia = idDokumentow.First();
                zamowienieWejsciowe.ZdarzeniePrzetworzZamowienie = PrzetworzZamowieniePoPolaczeniu;
                Log.Info($"Zamówienie o nr:{zamowienieWejsciowe.NumerZPlatformy} zostanie połączony z dokumentem o Id: {zamowienieWejsciowe.IdZamowieniaDoPolaczenia}");
                Log.Debug($"SQL użyty do wyszukania zamówienia do połączenia: {zapytanie} ");
            }
            else
            {
                Log.Info($"Nie znaleziono dokumentów do dołaczenia");
            }
            Log.Debug("Koniec przetwarzania zamówienie");
        }

        /// <summary>
        /// Przygotowywuje zapytanie w zależności od danych wejściowych do wyszukiwania zamówień do łaczenia.
        /// </summary>
        /// <param name="zamowienieWejsciowe"></param>
        /// <returns></returns>
        protected string PrzygotujZapytanie(ZamowienieSynchronizacja zamowienieWejsciowe)
        {
            string sqlRozbicia = string.Empty;
            //jesli zamówienie pochodzi z rozbicia musimy wyciągnąć powód rozbicia
            if (zamowienieWejsciowe.PochodziZRozbicia)
            {
                string cechaRozbicia = PobierzPowodRozbicia(zamowienieWejsciowe.NumerZPlatformy);
                if (string.IsNullOrEmpty(cechaRozbicia))
                {
                    Log.Error($"Zamówienie pochodzi z rozbicia ale z nr: ({zamowienieWejsciowe.NumerZPlatformy}) nie można odczytać cechy rozbicia. Pomijam Łączenie dla tego zamówienia.");
                    return sqlRozbicia;
                }
                sqlRozbicia = $"AND dok_NrPelnyOryg like 'B2B %/{cechaRozbicia} z%'";
            }
            else
            {
                sqlRozbicia = "AND dok_NrPelnyOryg not like 'B2B %/[A-Za-z]% z %'";
            }

            string sqlUwagi = string.Empty;
            if (!string.IsNullOrEmpty(UwagiWykluczenie))
            {
                foreach (string s in UwagiWykluczenie.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    sqlUwagi += $"AND dok_Uwagi not LIKE '{s}' ";
                }
            }

            string[] flagiWykluczajace = FlagaWykluczjaca.Split(new char[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries);

            List<string> sqlDoSzukaniaPoFlaga = new List<string>();

            foreach (string flaga in flagiWykluczajace)
            {
                sqlDoSzukaniaPoFlaga.Add($" (flg_Text not like '{flaga}') ");
            }

            string sqlDoSzukaniaPoFlagach = string.Join(" AND ", sqlDoSzukaniaPoFlaga);

            string sql = "SELECT [dok_Id] FROM [dok__Dokument] " +
                         (!string.IsNullOrEmpty(FlagaWykluczjaca) ? "LEFT JOIN fl_Wartosc ON flw_IdObiektu = dok_Id LEFT JOIN fl__Flagi ON flg_Id = flw_IdFlagi " : " ") +
                         "WHERE dok_Typ = 16 AND dok_Status IN(6, 7) " +
                         $"AND dok_OdbiorcaId = {zamowienieWejsciowe.KlientId} " +
                         (!string.IsNullOrEmpty(FlagaWykluczjaca) ? $" AND  ( flg_Text is null OR ( {sqlDoSzukaniaPoFlagach})  ) " : " ") +
                         sqlRozbicia + sqlUwagi +
                         $"AND dok_DataWyst BETWEEN '{DateTime.Now.AddDays(-LiczbaDni).ToString("yyyy-MM-dd")}' AND '{DateTime.Now.ToString("yyyy-MM-dd")}' " +
                         $" AND flg_Id IS NOT NULL AND flg_IdGrupy = 8 " +
                         " ORDER BY dok_DataWyst";
            
            return sql;
        }

        /// <summary>
        /// Pobiera z nr zamówienia powód rozbicia
        /// </summary>
        /// <param name="nrZamowienia"></param>
        /// <returns></returns>
        public string PobierzPowodRozbicia(string nrZamowienia)
        {
            var tmp = Regex.Match(nrZamowienia);
            var wynik = tmp.Groups["powod"].Value;
            return wynik;
        }

        private Regex _reg;

        private Regex Regex => _reg ?? (_reg = new Regex(@"^(?:\bB2B\b|\bimport\b)\s.*/(?<powod>.*)\sz\s\d+$", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant));

        /// <summary>
        /// Czynności wykonywane po polaczeniu zamówień
        /// </summary>
        public override object PrzetworzZamowieniePoPolaczeniu(object dokument, string uwagi, ZamowienieSynchronizacja zamowienie)
        {
            string noweUwagi = DopiszWyrazenieDoUwag(uwagi, zamowienie.NumerZPlatformy, zamowienie.PochodziZRozbicia, zamowienie.Uwagi);
            Log.Debug($"Nowe uwagi:{noweUwagi}");
            Dictionary<string, object> parametry = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase) { { "Uwagi", noweUwagi }, { "Flaga", FlagaPoPolaczeniu } };
            var dok = ((ILaczenieZamowien)_provider).PrzetorzZamowieniePoPolaczeniu(dokument, parametry);
            return dok;
        }

        /// <summary>
        /// Pobieramy id dokumnetów za pomocą wcześniej przygotowanego zapytania.
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        protected List<long> PobierIdDokumnetow(string sql)
        {
            List<long> items = new List<long>();

            if (string.IsNullOrEmpty(sql))
            {
                return items;
            }

            string cs = SyncManager.PobierzInstancje.Konfiguracja.ERPcs;
            if (string.IsNullOrEmpty(cs))
            {
                throw new Exception($"Brak danych do połaczenia z Subiektem.");
            }

            SqlConnection conn = null;
            SqlCommand cmd = null;
            SqlDataReader rd = null;
            try
            {
                conn = new SqlConnection(cs);
                conn.Open();
                cmd = new SqlCommand(sql, conn);
                rd = cmd.ExecuteReader();
                while (rd.Read()) //pobieranie  produktów
                {
                    long id = DataHelper.dbl("dok_Id", rd);
                    items.Add(id);
                }
                rd.Close();
                rd.Dispose();
                cmd.Dispose();
            }
            catch (Exception e){
                Log.Info($"Zapytanie SQL: {sql}");
                throw;
            }
            finally
            {
                if (rd != null)
                {
                    rd.Close();
                    rd.Dispose();
                }
                cmd?.Dispose();
                if (conn != null)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
            Log.Debug($"Pobranych id dokumentów:{items.Count}. {items.ToJson()}");
            return items;
        }
    }
}