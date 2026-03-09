using System.Reflection;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using System;
using System.Collections.Generic;
using System.Linq;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Interfaces.Sync;
using System.Data.SqlClient;
using SolEx.Hurt.Core.Sync;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Produkty
{
    public class PoleProduktuZSQL : SyncModul, Model.Interfaces.SyncModuly.IModulProdukty
    {

        public override string uwagi
        {
            get { return ""; }
        }

        [FriendlyName("Zapytanie SQL", FriendlyOpis = "W zapytaniu MUSI być zdefiniowanie pole 'PRODUCT' o typie INT rozróżniające produkty " +
            "np. SELECT PRODUCT=Towar_Id, Nazwa_Parsa=Towar_Pars FROM Towar")]
        [WidoczneListaAdmin(false, false, true, false)]
        public string Zapytanie { get; set; }

        [FriendlyName("Pole, do którego będzie skopiowana wartość z pola źródłowego")]
        [SyncSlownikNaPodstawieInnegoTypu(typeof(Produkt))]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string PoleDocelowe { get; set; }


        public override string Opis
        {
            get { return "Ustawienie pola produktu z SQL"; }
        }

        protected string NazwaKolumnyKluczaProduktu => "PRODUCT";

        [FriendlyName("Kolumna z SQL", FriendlyOpis = "Nazwa kolumny z zapytania SQL z której pobrać wartość do pola produktu")]
        [WidoczneListaAdmin(false, false, true, false)]
        public string NazwaKolumnySQL { get; set; }

        [Niewymagane]
        [FriendlyName("Połączenie do bazy danych (opcjonalne)", FriendlyOpis = "Jeśli nieuzupełnione, to wartość pobierana z ustawienia systemowego 'erp_cs'. " +
                       "Przykładowa wartość do wpisania: Data Source=appserwer; Initial Catalog=b2b-testowa; User Id=sa; Password=123")]
        [WidoczneListaAdmin(false, false, true, false)]
        public string ParametryPolaczenia { get; set; }

        public void Przetworz(ref List<Produkt> listaWejsciowa, ref List<Tlumaczenie> produktyTlumaczenia, Dictionary<long, Produkt> produktyNaB2B,
            ref List<JednostkaProduktu> jednostki, ref Dictionary<long, ProduktCecha> lacznikiCech, ref List<ProduktKategoria> lacznikiKategorii,
            ISyncProvider provider, ref List<ProduktUkryty> produktuUkryteErp, ref List<ProduktyZamienniki> zamienniki,
            Dictionary<long, KategoriaProduktu> kategorie, ref List<Cecha> cechy, ref List<Atrybut> atrybuty)
        {
            if (string.IsNullOrEmpty(Zapytanie) || string.IsNullOrEmpty(PoleDocelowe))
                return;

            List<PropertyInfo> propertisy = typeof(Produkt).Properties().Values.ToList();
            var akcesor = typeof(Produkt).PobierzRefleksja();

            var poledocelowe = propertisy.FirstOrDefault(a => a.Name == PoleDocelowe);

            if (poledocelowe == null)
            {
                Log.Error($"Brak pola o nazwie: {PoleDocelowe}");
                return;
            }

            int iloscWierszySQL = 0;

            using (SqlConnection con = new SqlConnection(this.ParametryPolaczenia ?? SyncManager.PobierzInstancje.Konfiguracja.ERPcs))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(this.Zapytanie, con))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ++iloscWierszySQL;

                            object kluczWWierszuSql = reader[this.NazwaKolumnyKluczaProduktu];
                            if (kluczWWierszuSql == null)
                            {
                                continue;
                            }

                            int produktID = (int)kluczWWierszuSql;

                            object nowaWartosc = reader[this.NazwaKolumnySQL];

                            if (nowaWartosc is null)
                            {
                                continue;
                            }

                            Produkt produktDoZmiany = listaWejsciowa.FirstOrDefault(x => x.Id == produktID);

                            if (produktDoZmiany == null)
                            {
                                LogiFormatki.PobierzInstancje.LogujInfo($"Nie ma produktu o ID: {produktID} - wiersz SQL numer: {iloscWierszySQL}");
                                continue;
                            }

                            try
                            {

                                akcesor[produktDoZmiany, poledocelowe.Name] = nowaWartosc;
                            }
                            catch (Exception ex)
                            {
                                Log.Error($"Błąd przy przetwarzaniu towaru id: {produktDoZmiany.Id}, próba wpisania wartości: {nowaWartosc} do pola: { poledocelowe.Name}, komunikat błędu:" + ex.Message, ex);
                            }
                        }

                    }
                }
            }
        }
    }
}
