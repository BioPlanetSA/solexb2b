using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using SolEx.Hurt.Core.Sync;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Helpers;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Stany
{
   
    [FriendlyName("Pobieranie stanów z SQL", FriendlyOpis = "Mapuje stany w wybranym magazynie na podstawie zapytania sql, zapytanie w pierwszej kolumnie ma zwracać kod kreskowy produktu(po nim mapowieni)," +
                                     " w drugiej ilość w jednostce podstawowej. Zapytanie musi zagwarantować unikalnośc kodów krekowych")]
    public class StanySql : StanyBaza
    {
        [FriendlyName("Parametry połączenia")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string ParametryLaczenia { get; set; }
        
        [FriendlyName("Zapytanie")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string Zapytanie { get; set; }
      
        public override void Przetworz(ref Dictionary<int,List<ProduktStan>> listaWejsciowa, List<Magazyn> magazyny, List<Produkt> produktyB2b )
        {

            if (!listaWejsciowa.ContainsKey(IdMagazynu))
            {
                throw new Exception($"Brak magazynu {magazyny[IdMagazynu].Nazwa}[{IdMagazynu}] do importu z erpa.");
            }
            var mag = magazyny.FirstOrDefault(x => x.Id == IdMagazynu);
            if (mag == null)
            {
                LogiFormatki.PobierzInstancje.LogujInfo("Brak magazynu od id {0}, moduł kończy działanie");
                return;
            }
            //pobieranie stanów z SQL
            var wynik = PobierzStanyZSql();
            Dictionary<string, long> slownikProduktow = SlownikProduktow(produktyB2b);
            
            //var produktyNaB2BWgId = produktyB2b.ToDictionary(x => x.Id, x => x);

            var lwpoid = listaWejsciowa[IdMagazynu].ToDictionary(x => x.ProduktId, x => x);
            foreach (var lw in listaWejsciowa[IdMagazynu])
            {
                if (!slownikProduktow.ContainsValue(lw.ProduktId))
                {
                    continue;
                }
                string kod = (slownikProduktow.First(x=>x.Value == lw.ProduktId).Key??"");
                if (string.IsNullOrEmpty(kod))
                {
                    continue;
                }
                if (!wynik.ContainsKey(kod))
                {
                    continue;
                }
                decimal nowy = wynik[kod];
                lw.Stan = NowyStan(lw.Stan, nowy);
            }


            foreach (var lw in slownikProduktow)
            {
                if (lwpoid.ContainsKey(lw.Value))
                {
                    continue; //w poprzedniej petli byl juz zrobiony
                }
                string kod = lw.Key;
                if (string.IsNullOrEmpty(kod))
                {
                    continue;
                }
                if (!wynik.ContainsKey(kod))
                {
                    continue;
                }
                decimal nowy = wynik[kod];
                var stannowy = new ProduktStan {MagazynId = IdMagazynu, ProduktId = lw.Value, Stan = nowy};
                lwpoid.Add(lw.Value, stannowy);
                listaWejsciowa[IdMagazynu].Add(stannowy);
            }
            Log.Debug("koniec mapowanie");
        }

        private Dictionary<string, decimal> PobierzStanyZSql()
        {
            Log.Debug("Początek sql");
            SqlDataReader rd = null;
            SqlCommand cmd = null;
            SqlConnection conn = null;
            Dictionary<string, decimal> wynik = new Dictionary<string, decimal>();
            try
            {
                conn = new SqlConnection(ParametryLaczenia);
                conn.Open();
                cmd = new SqlCommand(Zapytanie, conn);
                rd = cmd.ExecuteReader();
                while (rd.Read())
                {
                    wynik.Add(rd[0].ToString(), (decimal)rd[1]);
                }
            }
            catch (Exception ex)
            {
                LogiFormatki.PobierzInstancje.LogujInfo("Bład zapytania sql, moduł kończy działanie");
                LogiFormatki.PobierzInstancje.LogujError(ex);
                return wynik;
            }
            finally
            {
                if (rd != null)
                {
                    rd.Close();
                    rd.Dispose();
                }
                if (cmd != null)
                {
                    cmd.Dispose();
                }
                if (conn != null)
                {
                    conn.Close();
                    conn.Dispose();
                }

            }
            Log.Debug("konice sql");
            return wynik;
        }

        public override string uwagi => "Mapuje stany w wybranym magazynie na podstawie zapytania sql, zapytanie w pierwszej kolumnie ma zwracać kod kreskowy produktu(po nim mapowieni), w drugiej ilość w jednostce podstawowej. Zapytanie musi zagwarantować unikalnośc kodów krekowych";
    }

}
