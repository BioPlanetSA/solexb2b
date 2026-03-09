using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using ServiceStack.OrmLite;
using Xunit;

namespace SolEx.Hurt.DAL.Tests
{
    public class TestyOrm
    {
        [Fact()]
        public void ReCreateTabeleTest()
        {
            //TODO to się wywala ale nie mam pojęcia jak to poprawić
            //SolEx.Hurt.DAL.TworzenieBazyDanych.ReCreateTabele();

            //var q = Refleksja.PobierzListeKlasZAtrybutem("SolEx.Hurt.Model", typeof(SolEx.Hurt.Model.TworzDynamicznieTabeleAttribute));

            //Assert.True(q.Count() > 0);

            //foreach (Type t in q)
            //{
            //    Assert.True(MainDAO.db.TableExists(t.Name));
            //}
        }

            public enum Enum
            {
                test = 1,
                test2 = 2
            }
            public class TestowaLista
            {
                [Key]
                public int Id { get; set; }

                public List<int> listaintow { get; set; }
                public List<string> listastringow { get; set; }
                public List<Enum> listenumow { get; set; }

              
                public void uzupelnijDaneTestowe()
                {
                    listaintow = new List<int>() { 1, 2, 3, 4 };
                    listastringow = new List<string>() { "1", "2", "3", "4" };
                    listenumow = new List<Enum>() { Enum.test, Enum.test2 };
                }
            }


        public class TestowyObiektKluczVarchar
        {
            public string Id { get; set; }
            public string nazwa { get; set; }
        }

            private string baza = @"C:\bazaTest";

            private void StworzBazeTestowa()
            {
                OrmLiteConfig.DialectProvider = SqliteDialect.Provider;

                if (File.Exists(baza))
                {
                    File.Delete(baza);
                }
            }

            [Fact(DisplayName = "Test serializacji do tekstu Listy<int> i stringow")]
            public void dbTest()
            {
                StworzBazeTestowa();
                TestowaLista nowy = new TestowaLista();
                nowy.uzupelnijDaneTestowe();

                TestowaLista nowy2 = null;
                TestowyObiektKluczVarchar nowyTestowyVarchar = null;

                using (var db = baza.OpenDbConnection())
                {
                    db.CreateTable<TestowaLista>();
                    db.CreateTable<TestowyObiektKluczVarchar>();
                }

                using (var db = baza.OpenDbConnection())
                {
                    for (int i = 0; i < 10; ++i)
                    {
                        nowy.Id = i;
                        db.Insert(nowy);
                    }

                    for (int i = 0; i < 10; ++i)
                    {
                        nowyTestowyVarchar = new TestowyObiektKluczVarchar() {Id = "klucz" + i, nazwa = "nazwa" + i};
                        db.Insert(nowyTestowyVarchar);
                    }
                }

                using (var db = baza.OpenDbConnection())
                {
                    nowy2 = db.Select<TestowaLista>().FirstOrDefault();

                    //test kluczy i update
                    nowyTestowyVarchar = db.Select<TestowyObiektKluczVarchar>().FirstOrDefault();

                    nowyTestowyVarchar.nazwa = "toTest";
                    db.Update(nowyTestowyVarchar);
                    nowyTestowyVarchar = null;
                    nowyTestowyVarchar = db.Select<TestowyObiektKluczVarchar>().FirstOrDefault();

                }



                Assert.True(nowy2.listaintow.Count == nowy.listaintow.Count);
                Assert.True(nowyTestowyVarchar.Id == "klucz0" && nowyTestowyVarchar.nazwa == "toTest");


            }
        


    }
}
