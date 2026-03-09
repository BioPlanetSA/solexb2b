using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceStack.OrmLite;
using SolEx.Hurt.Core;
using SolEx.Hurt.Core.DostepDane;
using SolEx.Hurt.DAL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using Xunit;
namespace SolEx.Hurt.DAL.Tests
{
    public class OrmLiteWriteConnectionExtensionsExtensionsTests
    {

        [Fact(DisplayName = "Test dodawania różnych obiektów i sprawdzania zwracanych kluczy")]
        public void SaveAll_UzupelnijKluczeTest()
        {
            List<Produkt> lista = new List<Produkt>(3);
            Produkt item = new Produkt();
            item.Nazwa = "test";
            item.Kod = "test";
            item.Id = 0;
            lista.Add(item);

            item = new Produkt();
            item.Nazwa = "test2";
            item.Kod = "test2";
            item.Id = 0;
            lista.Add(item);

            item = new Produkt();
            item.Nazwa = "test3";
            item.Kod = "test3";
            item.Id = 0;
            lista.Add(item);

            //var temp = DAL.MainDAO.db.FirstOrDefault<produkty>(a => a.widoczny == true && a.ilosc_minimalna < 4);

            //DAL.MainDAO.db.SaveAll_UzupelnijKlucze(lista);

            //sprawdzy czy id zostały nadane - ujemne bo automatyczne ID
            foreach (Produkt p in lista)
            {
                Assert.True(p.Id < 0, "Produkt z dodatnim ID!!");
            }

            //czy po drugim zapisie ID zostanie zachowane a obiekty tylko z udatowane

            lista[0].Nazwa = "dfgdgerteter";
            lista[0].Kod = "dsdd";

            lista[1].Nazwa = "1111";
            lista[1].Kod = "111";

            lista[2].Nazwa = "2212121";
            lista[2].Kod = "221212121";

            item = new Produkt();
            item.Nazwa = "10000id10-erp";
            item.Kod = "erp-2produkt";
            item.Id = 10;   //dodanie ID
            lista.Add(item);

            item = new Produkt();
            item.Nazwa = "erp produtk duzy";
            item.Kod = "erp-3 produkt";
            item.Id = 6450;   //dodanie ID
            lista.Add(item);

            //pierwszy zapis
            //DAL.MainDAO.db.SaveAll_UzupelnijKlucze(lista);
            //drugi zapis
            //DAL.MainDAO.db.SaveAll_UzupelnijKlucze(lista);


            //pobranie listy
            //List<produkty> listaPobranaSQL = DAL.MainDAO.db.Select<produkty>();

            //foreach (produkty p in listaPobranaSQL)
            //{
            //    produkty pStary = lista.FirstOrDefault(x => x.Id == p.Id);
            //    Assert.NotNull(pStary);

            //    Assert.True(pStary.kod == p.kod && pStary.nazwa == p.nazwa);
            //}

        }
    }
}
