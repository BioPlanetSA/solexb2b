using System;
using System.Collections.Generic;
using System.Linq;
using ServiceStack.Common;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core.Sync
{
    public class KategorieProduktowSyncHelper : BllBaza<KategorieProduktowSyncHelper>
    {
        public IAPIWywolania ApiWywolanie = APIWywolania.PobierzInstancje;
        public IConfigSynchro Config = SyncManager.PobierzInstancje.Konfiguracja;

        public void KategorieProduktyZCechy(Grupa grupa, ref List<ProduktKategoria> listalacznikow, List<ProduktCecha> wszystkiecp)
        {
            var idslacz = new HashSet<long>( listalacznikow.Select(x => x.WygenerujIDObiektuSHAWersjaLong()) );
            LogiFormatki.PobierzInstancje.LogujDebug("Pobieranie kategorii");
            var kategorie_Z_Cech = PobierzKategorie(grupa);
            foreach (var t in kategorie_Z_Cech)
            {
                var cp = wszystkiecp.Where(x => x.CechaId == t.Id).ToList();
                foreach (var cechy_Produkty in cp)
                {
                    var pk = new ProduktKategoria {KategoriaId = cechy_Produkty.CechaId, ProduktId = cechy_Produkty.ProduktId, Rodzaj = 1};
                    var hash = pk.WygenerujIDObiektuSHAWersjaLong();
                    if (!idslacz.Contains(hash))
                    {
                        idslacz.Add(hash);
                        listalacznikow.Add(pk);
                    }
                }
            }
        }

        private const string UzywanySeparator = "\\";

        private string PodmienSeparatory(string[] dostepneSeparatory, string nazwaKategorii, string uzywanySeparator)
        {
            var nowaNazwa = "";
            var czesci = nazwaKategorii.Split(dostepneSeparatory, StringSplitOptions.RemoveEmptyEntries);
            for (var i = 0; i < czesci.Length; i++)
            {
                nowaNazwa += czesci[i].Trim();
                if (i < czesci.Length - 1)
                {
                    nowaNazwa += uzywanySeparator;
                }
            }
            return nowaNazwa.Trim();
        }

        public virtual List<KategoriaProduktu> PobierzKategorie(Grupa grupa)
        {
            var wszystkiechechy = ApiWywolanie.PobierzCechy().Values.ToList();
            LogiFormatki.PobierzInstancje.LogujDebug($"Pobrano {wszystkiechechy.Count} cech");
            string[] noweParametry = grupa.ParametryTablica();
            var dodawajAtrDoKategorii = Config.DadawanieAtrybutuDoKategorii;
            var kategorie_Z_Cech = new List<KategoriaProduktu>();
            var separatorywdrzewku = Config.SeparatoryDrzewkaKategorii;
            foreach (var nowyParametr in noweParametry)
            {
                foreach (var cechy in wszystkiechechy)
                {
                    if (!cechy.Symbol.StartsWith(nowyParametr, StringComparison.InvariantCultureIgnoreCase))
                    {
                        continue;
                    }
                    //domyślnie powinny być i tak wysyłane ze zmienionym znakiem więc tutaj nie należy tego robić
                    //w trójce jedna kategoria ma nazwę Panele ogrodzeniowe 5 / 6+ 6 mm i zmieniając to w taki sposób się rozjedzie cały mechanizm
                    //cechy.nazwa = cechy.nazwa.Replace("/", "\\");
                    var k = new KategoriaProduktu
                    {
                        Nazwa = PodmienSeparatory(separatorywdrzewku, (dodawajAtrDoKategorii ? cechy.Symbol : cechy.Nazwa).TrimStart('_').Trim(), UzywanySeparator),
                        Widoczna = true,
                        GrupaId = grupa.Id,
                        Dostep = AccesLevel.Wszyscy,
                        Id = cechy.Id,
                        ObrazekId = cechy.ObrazekId
                    };
                    kategorie_Z_Cech.Add(k);
                }
            }
            LogiFormatki.PobierzInstancje.LogujDebug($"Przefiltrowanych i utworzonych kategorii bez hierarchii: {kategorie_Z_Cech.Count} ");
            return kategorie_Z_Cech;
        }

        public void KategorieZCechy(Grupa grupa, Dictionary<long, KategoriaProduktu> kategorie)
        {
            //  string[] separatorywdrzewku = Config.SeparatoryDrzewkaKategorii;
            var kategorie_Z_Cech = PobierzKategorie(grupa);
            for (var i = 0; i < kategorie_Z_Cech.Count; i++)
            {
                var poziomy = kategorie_Z_Cech[i].Nazwa.Split(new[] {UzywanySeparator}, StringSplitOptions.RemoveEmptyEntries);
                if (poziomy.Length == 0)
                {
                    continue; //olewamy jesli pusty element
                }
                var symbolRodzica = "";
                for (var j = 0; j < poziomy.Length - 1; j++)
                {
                    symbolRodzica += poziomy[j].Trim();
                    if (j < poziomy.Length - 2)
                    {
                        symbolRodzica += UzywanySeparator;
                    }
                }
                if (!string.IsNullOrEmpty(symbolRodzica))
                {
                    var rodzic = kategorie_Z_Cech.FirstOrDefault(p => p.Nazwa.Equals(symbolRodzica, StringComparison.InvariantCultureIgnoreCase));
                    if (rodzic != null)
                    {
                        kategorie_Z_Cech[i].ParentId = rodzic.Id;
                    }
                }
            }
            foreach (var kc in kategorie_Z_Cech)
            {
                var poziomy = kc.Nazwa.Split(new[] {UzywanySeparator}, StringSplitOptions.RemoveEmptyEntries);
                //to ma tak zostać, inaczej efekty jak na saturnie
                // if (poziomy.Length > 0  && kc.parent_id!=null) lemir grupy, pierwszy poziom nie może się nazywać kategoria:oświetlenie z podziałem na style i typy\wszystkie kinkiety gdzie w grupie prefixem jest kategoria:oświetlenie z podziałem na style i typy\
                if (poziomy.Length > 0)
                {
                    kc.Nazwa = poziomy[poziomy.Length - 1].Trim(); //nazwa to ostatni element
                }
                if (kategorie.All(a => a.Key != kc.Id) && !string.IsNullOrEmpty(kc.Nazwa))
                {
                    kategorie.Add(kc.Id, kc);
                }
            }
            LogiFormatki.PobierzInstancje.LogujDebug($"Przefiltrowanych i utworzonych kategorii z hierarchią: {kategorie.Count} ");
        }
    }
}
