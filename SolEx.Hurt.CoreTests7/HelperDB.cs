using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ServiceStack.OrmLite;
using SolEx.Hurt.DAL;
using SolEx.Hurt.Model;
using Xunit;

namespace SolEx.Hurt.CoreTests
{
    public static class TestHelperDB
    {
        //dane testowe
        public static magazyny[] mags = new magazyny[] { 
        new magazyny { Id = -99999, importowac_z_ERP = false, Nazwa = "___testowy solex__", symbol = "testowy_solex", parametry = "" }, 
        new magazyny { Id = -99649, importowac_z_ERP = true, Nazwa = "test168jj  uf  u", symbol = "tets1", parametry = "" },
        new magazyny { Id = 1, importowac_z_ERP = true, Nazwa = "mag9", symbol = "MAG", parametry = "" },
        new magazyny { Id = 0, importowac_z_ERP = false, Nazwa = "sym", symbol = "PAT", parametry = "" } 
        };

        public static SolEx.Hurt.Model.produkty[] pr = new SolEx.Hurt.Model.produkty[] {
            new SolEx.Hurt.Model.produkty{ produkt_id = 217519, nazwa = "Drag DR10 17x7 ET40 5x100/114,3 Srebrna", kod= "DR10177054073S", stan_min = 0, 
                ilosc_w_opakowaniu =1, kod_kreskowy ="423423423", vat=23, PKWiU = "", opis = "", waga=10 } };

        public static Produkty_stany[] stany = new Produkty_stany[]{ 
            new Produkty_stany{produkt_id = 217519, magazyn_id = 1, stan = 4},
            new Produkty_stany{produkt_id = 217519, magazyn_id = 0, stan = 0},
            new Produkty_stany{produkt_id = 217519, magazyn_id = -99649, stan = 34},
            new Produkty_stany{produkt_id = 217519, magazyn_id = -99999, stan = 995}
        };

        public static artykuly_kategorie testowaKategoria = new artykuly_kategorie()
        {
            dodatkowe_klasy_css = "css",
            dostep_id = 1,
            Id = 0,
            kolejnosc = 2,
            link = "/admin/cos/tam.html",
            link_listy_produktow = "produkty?id=2",
            meta_opis = "meta1",
            meta_slowa_kluczowe = "kluczowe slowa",
            nazwa = "nazwa",
            symbol = "symbol",
            parent_id = null,
            pokazuj_tytul = false,
            pracownik_id = 34,
            przyjazna_nazwa = "sdfds",
            rozmiar_strony = 34,
            skrypt = "sdfrs",
            submenu_nazwa = "sdfsfs",
            systemowa = true,
        
            ukryj_submenu = true,
            ukryty = true,
            widoczna = true,
            zawiera_kontrolke = true
        };

        public static void WypelnijBazePrzykladowymiDanymi()
        {
             MainDAO.db.ExecuteSql("CREATE TABLE Produkty_stany (produkt_id INTEGER, magazyn_id INTEGER, stan REAL, PRIMARY KEY (produkt_id, magazyn_id))");

             MainDAO.db.Insert<magazyny>(mags);
             MainDAO.db.Insert<SolEx.Hurt.Model.produkty>(pr);
             MainDAO.db.Insert(testowaKategoria);

             MainDAO.db.Insert<Produkty_stany>(stany);

             //foreach (Produkty_stany s in stany)
             //{
             //    string sql = string.Format("insert into Produkty_stany values ( {0}, {1}, {2} )", s.produkt_id, s.magazyn_id, s.stan);
             //    MainDAO.db.ExecuteSql(sql);
             //}

             List<magazyny> aktual = MainDAO.db.Select<magazyny>();
             Assert.True(aktual.Count == mags.Count());
        }

    }
}
