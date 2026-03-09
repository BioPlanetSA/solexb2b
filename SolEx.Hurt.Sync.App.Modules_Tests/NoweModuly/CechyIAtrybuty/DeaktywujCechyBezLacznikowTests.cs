using System;
using System.Collections.Generic;
using System.Linq;
using SolEx.Hurt.Model;
using SolEx.Hurt.Sync.App.Modules_.NoweModuly.CechyIAtrybuty;
using Xunit;

namespace SolEx.Hurt.Sync.App.Modules_Tests.NoweModuly.CechyIAtrybuty
{
    public class DeaktywujCechyBezLacznikowTests
    {

        [Fact]
        public void SprawdzCechyTest()
        {
            string deaktywacjaAutomatycznaOpis = "-DEAKTYWOWANA-AUTOMATYCZNIE";
            Cecha c1 = new Cecha("Cecha 1", "atr:cecha 1") {Id = 1};
            Cecha c2 = new Cecha("Cecha 2", "atr:cecha 2") { Id = 2 };
            Cecha c3 = new Cecha("Cecha 3", "atr:cecha 3") { Id = 3 };
            Cecha c4 = new Cecha("Cecha 4", "atr:cecha 4") { Id = 4 };
            Cecha c5 = new Cecha("Cecha 5", "atr:cecha 5") { Id = 5 };
            Cecha c6 = new Cecha("Cecha 6", "atr:cecha 6") { Id = 6 };

            Cecha p1 = new Cecha("Cecha p1", "atr:cecha p1") { Id = 7 };
            Cecha p2 = new Cecha("Cecha p2", "atr:cecha p2") { Id = 8 };
            Cecha p3 = new Cecha("Cecha p3", "atr:cecha p3") { Id = 9 };
            Cecha p4 = new Cecha("Cecha p4", "atr:cecha p4") { Id = 10 };
            Cecha p5 = new Cecha("Cecha p5", "atr:cecha p5") { Id = 11 };
            Cecha p6 = new Cecha("Cecha p6", "atr:cecha p6") { Id = 12 };

            List<Cecha> listaCech = new List<Cecha>() {c1,c2,c3,c4,c5,c6};
            List<Cecha> listaCechNaPlatformie = new List<Cecha>() { c1, c2, c3, c4, c5, c6 };
            Dictionary<long, Cecha> cechyNaPlatformie = listaCechNaPlatformie.ToDictionary(x => x.Id, x => x);
            HashSet<string> grupy = new HashSet<string>() {"atr"};
            HashSet<long> idCechZLacznikow = new HashSet<long>() {1,2,4,6,8,10,12};

            DeaktywujCechyBezLacznikow deaktywuj = new DeaktywujCechyBezLacznikow();
            deaktywuj.SprawdzCechy(cechyNaPlatformie,grupy,idCechZLacznikow,ref listaCech);
            //Assert.True(listaCech.Any(x=>x.Nazwa.IndexOf(deaktywacjaAutomatycznaOpis, StringComparison.InvariantCultureIgnoreCase)>0), "Powinna być jakakolwiec cecha deaktywowana");

        }
    }
}
