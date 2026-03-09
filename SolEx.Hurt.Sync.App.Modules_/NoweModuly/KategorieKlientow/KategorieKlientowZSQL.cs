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
using SolEx.Hurt.Model.Interfaces.SyncModuly;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.KategorieKlientow
{
    public class KategorieKlientowZSQL : Rozne.KategorieKlientow, IModulKategorieKlientow
    {
        [FriendlyName("Zapytanie SQL", FriendlyOpis = "W zapytaniu MUSI być zdefiniowanie pole 'KlientID' o typie LONG, oraz 'Nazwa' np. SELECT KlientID, Nazwa FROM ....")]
        [WidoczneListaAdmin(false, false, true, false)]
        public string Zapytanie { get; set; }

        [FriendlyName("Nazwa tworzonej grupy")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string NazwaGrupy { get; set; }

        public override string Opis
        {
            get { return "Pobranie kategorii klientów z SQL"; }
        }

        [Niewymagane]
        [FriendlyName("Połączenie do bazy danych (opcjonalne)", FriendlyOpis = "Jeśli nieuzupełnione, to wartość pobierana z ustawienia systemowego 'erp_cs'. " +
                       "Przykładowa wartość do wpisania: Data Source=serwer; Initial Catalog=b2b-testowa; User Id=sa; Password=123")]
        [WidoczneListaAdmin(false, false, true, false)]
        public string ParametryPolaczenia { get; set; }


        public virtual IEnumerable<Klient> WszyscyKlienci()
        {
            return ApiWywolanie.PobierzKlientow().Values;
        }


        public void Przetworz(ref List<KategoriaKlienta> kategorie, ref List<KlientKategoriaKlienta> laczniki)
        {
            int iloscWierszySQL = 0;

            Dictionary<long, Klient> klienci = WszyscyKlienci().ToDictionary(x => x.Id);

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

                            object kluczWWierszuSql = reader[0];
                            if (kluczWWierszuSql == null)
                            {
                                continue;
                            }

                            int klientID = (int)kluczWWierszuSql;

                            object kategoria = reader[1];

                            if (kategoria is null)
                            {
                                continue;
                            }


                            if (!klienci.TryGetValue(klientID, out var klient))
                            {
                                LogiFormatki.PobierzInstancje.LogujInfo($"Brak klienta o ID: {klientID} - wiersz SQL: {iloscWierszySQL}");
                                continue;
                            }

                            DodajKategorie(kategorie, laczniki, klient, NazwaGrupy, kategoria.ToString());
                        }

                    }
                }
            }

         

        }








    }
}
