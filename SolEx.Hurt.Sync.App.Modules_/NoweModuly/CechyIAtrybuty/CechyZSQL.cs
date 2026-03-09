using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using ServiceStack.Text;
using SolEx.Hurt.Core.Sync;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Interfaces.Sync;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.CechyIAtrybuty
{
    [FriendlyName("Cechy z SQLa",FriendlyOpis = "Moduł tworzący cechy i atrybuty na podstawie wybranego pola produktu" )]
    public class CechyZSQL : CechyModulBaza
    {
        [FriendlyName("Atrybut, który będzie utworzony dla wybranego pola produkktu")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string Atrybut { get; set; }

        [FriendlyName("Zapytanie SQL", FriendlyOpis = "W zapytaniu MUSI być zdefiniowanie pole 'PRODUCT' o typie INT rozróżniające produkty " +
            "np. SELECT PRODUCT, CECHA FROM Towar")]
        [WidoczneListaAdmin(false, false, true, false)]
        public string Zapytanie { get; set; }

        protected string NazwaKolumnyKluczaProduktu => "PRODUCT";
        protected string NazwaKolumnyCecha => "CECHA";


        [Niewymagane]
        [FriendlyName("Połączenie do bazy danych (opcjonalne)", FriendlyOpis = "Jeśli nieuzupełnione, to wartość pobierana z ustawienia systemowego 'erp_cs'. " +
                      "Przykładowa wartość do wpisania: Data Source=appserwer; Initial Catalog=b2b-testowa; User Id=sa; Password=123")]
        [WidoczneListaAdmin(false, false, true, false)]
        public string ParametryPolaczenia { get; set; }


        public override void Przetworz(ref List<Atrybut> atrybuty, ref List<Cecha> cechy, Dictionary<long, Produkt> produktyNaB2B)
        {
            Dictionary<long, ProduktCecha> lacznikiCech = ApiWywolanie.PobierzCechyProdukty();
            Przetworz(ref atrybuty, ref cechy, produktyNaB2B.Values.ToList(), ref lacznikiCech);
        }
        
        public override void Przetworz(ref List<Produkt> listaWejsciowa, ref List<Tlumaczenie> produktyTlumaczenia, Dictionary<long, Produkt> produktyNaB2B, ref List<JednostkaProduktu> jednostki, ref Dictionary<long, ProduktCecha> lacznikiCech, ref List<ProduktKategoria> lacznikiKategorii, ISyncProvider provider, ref List<ProduktUkryty> produktuUkryteErp, ref List<ProduktyZamienniki> zamienniki, Dictionary<long, KategoriaProduktu> kategorie, ref List<Cecha> cechy, ref List<Atrybut> atrybuty)
        {
            Przetworz(ref atrybuty, ref cechy, listaWejsciowa, ref lacznikiCech);
        }

        public void Przetworz(ref List<Atrybut> atrybuty, ref List<Cecha> cechy, List<Produkt> produkty, ref Dictionary<long, ProduktCecha> lacznikiCech)
        {
            if (string.IsNullOrEmpty(Atrybut))
            {
                throw new Exception("Ustawienie Atrybut nie moze być puste. Popraw konfiguracje modułu");
            }

            if (string.IsNullOrEmpty(Zapytanie))
            {
                throw new Exception("Brak zapytania SQL");
            }

            Atrybut atrybut = ZnajdzAtrybut(Atrybut, atrybuty);
            if (atrybut == null)
            {
                atrybut = DodajBrakujacyAtrybut(Atrybut, atrybuty);
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

                            object nowaWartosc = reader[this.NazwaKolumnyCecha];

                            if (nowaWartosc is null)
                            {
                                continue;
                            }

                            Produkt produktDoZmiany = produkty.FirstOrDefault(x => x.Id == produktID);

                            if (produktDoZmiany == null)
                            {
                                LogiFormatki.PobierzInstancje.LogujInfo($"Nie ma produktu o ID: {produktID} - wiersz SQL numer: {iloscWierszySQL}");
                                continue;
                            }

                            try
                            {
                                string nazwaCechy = nowaWartosc.ToString();
                                string symbolCechy = $"{Atrybut}:{nazwaCechy.Trim()}".ToLower();

                                Cecha cechaProduktu = ZnajdzCeche(symbolCechy, cechy);
                                if (cechaProduktu == null)
                                {
                                    cechaProduktu = DodajBrakujacaCeche(symbolCechy, nazwaCechy, atrybut.Id, cechy);
                                }

                                DodajBrakujaceLaczniki(cechaProduktu.Id, produktDoZmiany.Id, lacznikiCech);
                            }
                            catch (Exception ex)
                            {
                                Log.Error($"Błąd przy przetwarzaniu towaru id: {produktDoZmiany.Id}, próba wpisania wartości: {nowaWartosc}, komunikat błędu:" + ex.Message, ex);
                            }
                        }

                    }
                }
            }


        }
    }
}
