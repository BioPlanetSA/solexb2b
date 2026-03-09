using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using ServiceStack.Common;
using SolEx.Hurt.Core;
using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.DAL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;
using Xunit;
namespace SolEx.Hurt.DAL.Tests
{
    public class SettingCollectionTests
    {
        [Fact(DisplayName = "Pobieranie ustawienia zalogowanego - niezalogowanego")]
        public void GetSettingTest()
        {
            string nl = "nl";
            string zal = "zal";
            List<Ustawienie> ust = new List<Ustawienie>();
            ust.Add(new Ustawienie( "test", "", zal, "", "", TypUstawienia.Bool, true, "", null, null, "", false, nl, TypUstawieniaPodgrupa.Brak, null, null, false, null,null,null));
            IConfigDataProvider proc = A.Fake<IConfigDataProvider>();

            SettingCollection col = new SettingCollection(ust,proc);
            string wal = col.GetSetting("test", "def", TypUstawienia.Bool, "", "", ustawieniaGrupa.Wygląd, false, true, true);
          Assert.Equal(wal, zal);
          string wal2 = col.GetSetting("test", "def", TypUstawienia.Bool, "", "", ustawieniaGrupa.Wygląd, false, true, false);
          Assert.Equal(wal2, nl);
        }

        [Fact(DisplayName = "Test sprawdzajacy poprawnosc dzialania metosy zwracajacej hashset z ustawieniami")]
        public void GetSettingSlownikRefleksjaTest()
        {

            string nazwa = "jakas nazwa";
            HashSet<int> defaultValue = new HashSet<int>(){1,2,3,4};
            string wartosc = "2;3;4";

            var settingCollection = A.Fake<SettingCollection>();
            A.CallTo(
                () =>
                    settingCollection.GetSetting(nazwa, A<string>.Ignored, A<TypUstawienia>.Ignored,
                        A<string>.Ignored, A<string>.Ignored, A<ustawieniaGrupa>.Ignored, A<bool>.Ignored,
                        A<bool>.Ignored, A<bool>.Ignored, A<bool>.Ignored, A<TypUstawieniaPodgrupa>.Ignored, A<bool>.Ignored, A<string>.Ignored, A<string>.Ignored)).Returns(wartosc);
            HashSet<int> hashset = settingCollection.GetSettingSlownikRefleksja<SlownikPoziomuCen, int>(nazwa, defaultValue, SesjaHelper.PobierzInstancje.CzyKlientZalogowany, "Id detalicznego poziomu cen", ustawieniaGrupa.Systemowe);

            Assert.True(hashset.Count==3,string.Format("Oczekiwano 3, otrzymano{0}",hashset.Count));
            Assert.True(hashset.Contains(2), string.Format("Oczekiwano 3, otrzymano{0}", hashset.Count));
            Assert.True(hashset.Contains(3), string.Format("Oczekiwano 3, otrzymano{0}", hashset.Count));
            Assert.True(hashset.Contains(4), string.Format("Oczekiwano 3, otrzymano{0}", hashset.Count));


            //Assert.True(false, "not implemented yet");
        }

        [Fact(DisplayName = "Test sprawdzajacy poprawnoscia budowania slownika")]
        public void GetSettingSlownikTest()
        {
            HashSet<string> domyslne = new HashSet<string>() { "_DropDownWielokrotnegoWyboru" };
            List<string> listaFiltrow = new List<string>() { "_DropDownWielokrotnegoWyboru", "_CheckBox" };
            IConfigDataProvider proc = A.Fake<IConfigDataProvider>();
            List<Ustawienie> ust = new List<Ustawienie>();
            ust.Add(new Ustawienie("DomyslneUstawienie", "", null, "", "", TypUstawienia.Bool, true, "", null, null, "", false, null, TypUstawieniaPodgrupa.Brak, null, null, false, null,null,null));

            var set = A.Fake<SettingCollection>(x => x.WithArgumentsForConstructor(new object[] { ust,proc }));
            A.CallTo(
                () =>
                    set.GetSetting(A<string>.Ignored, A<string>.Ignored, A<TypUstawienia>.Ignored,
                        A<string>.Ignored, A<string>.Ignored, A<ustawieniaGrupa>.Ignored, A<bool>.Ignored,
                        A<bool>.Ignored, A<bool>.Ignored, A<bool>.Ignored, A<TypUstawieniaPodgrupa>.Ignored, A<bool>.Ignored, A<string>.Ignored, A<string>.Ignored)).Returns("JakasWartosc");
            HashSet<string> wynik = set.GetSettingSlownik("DomyslnyTypFiltru", domyslne, listaFiltrow, SesjaHelper.PobierzInstancje.CzyKlientZalogowany, "Typy filtrów w systemie", ustawieniaGrupa.Systemowe);
            Assert.True(wynik.Contains("JakasWartosc"));

            var set2 = A.Fake<SettingCollection>(x => x.WithArgumentsForConstructor(new object[] { ust, proc }));
            A.CallTo(
                () =>
                    set.GetSetting(A<string>.Ignored, A<string>.Ignored, A<TypUstawienia>.Ignored,
                        A<string>.Ignored, A<string>.Ignored, A<ustawieniaGrupa>.Ignored, A<bool>.Ignored,
                        A<bool>.Ignored, A<bool>.Ignored, A<bool>.Ignored, A<TypUstawieniaPodgrupa>.Ignored, A<bool>.Ignored, A<string>.Ignored, A<string>.Ignored)).Returns(null);
            HashSet<string> wynik2 = set2.GetSettingSlownik("DomyslnyTypFiltru", domyslne, listaFiltrow, SesjaHelper.PobierzInstancje.CzyKlientZalogowany, "Typy filtrów w systemie", ustawieniaGrupa.Systemowe);

            Assert.True(wynik2.Contains("_DropDownWielokrotnegoWyboru"));
        }

        [Fact(DisplayName = "Test sprawdzajacy czy dobre czy dobrze zwrcana jest wartosc ustawienia")]
        public void GetSettingTest1()
        {
            List<Ustawienie> ust = new List<Ustawienie>();
            ust.Add(new Ustawienie( "DomyslneUstawienie", "", null, "", "", TypUstawienia.Bool, true, "", null, null, "", false, null, TypUstawieniaPodgrupa.Brak, null, null, false, null, null,null));
            IConfigDataProvider proc = A.Fake<IConfigDataProvider>();

            SettingCollection col = new SettingCollection(ust, proc);
            string wal = col.GetSetting("DomyslneUstawienie", "domyslna", TypUstawienia.Combo, "", "", ustawieniaGrupa.Systemowe, false, true, true);
            Assert.True(wal=="domyslna");



            List<Ustawienie> ust2 = new List<Ustawienie>();
            ust2.Add(new Ustawienie("DomyslneUstawienie", "", "JakasWartosc", "", "", TypUstawienia.Bool, true, "", null, null, "", false, null, TypUstawieniaPodgrupa.Brak, null, null, false, null, null,null));
            IConfigDataProvider proc2 = A.Fake<IConfigDataProvider>();

            SettingCollection col2 = new SettingCollection(ust2, proc2);
            string wal2 = col2.GetSetting("DomyslneUstawienie", "domyslna", TypUstawienia.Combo, "", "", ustawieniaGrupa.Systemowe, false, true, true);
            Assert.True(wal2 == "JakasWartosc");
        }
    }
}
