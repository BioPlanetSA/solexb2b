using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolEx.Hurt.Model;
using SolEx.Hurt.Sync.App.Modules_.NoweModuly.KategorieKlientow;
using Xunit;
namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.KategorieKlientow.Tests
{
    public class StworzGrupeZKateogriiTests
    {
        [Fact(DisplayName = "Test sprawdzajacy poprawnosc modulu przenoszacego kategorie do nowej grupy")]
        public void PrzetworzTest()
        {
            string kategorieDoPrzeniesienia = "JakasNazwa1;JakasNazwa3;JakasNazwa5;JakasNazwa7";
            KategoriaKlienta kk1 = new KategoriaKlienta() { Id = 1, Nazwa = "JakasNazwa1", Grupa = "jakasGrupa1" };
            KategoriaKlienta kk2 = new KategoriaKlienta() { Id = 2, Nazwa = "JakasNazwa2", Grupa = "jakasGrupa2" };
            KategoriaKlienta kk3 = new KategoriaKlienta() { Id = 3, Nazwa = "JakasNazwa3", Grupa = "jakasGrupa3" };
            KategoriaKlienta kk4 = new KategoriaKlienta() { Id = 4, Nazwa = "JakasNazwa4", Grupa = "jakasGrupa4" };
            KategoriaKlienta kk5 = new KategoriaKlienta() { Id = 5, Nazwa = "JakasNazwa5", Grupa = "jakasGrupa5" };
            KategoriaKlienta kk6 = new KategoriaKlienta() { Id = 6, Nazwa = "JakasNazwa6", Grupa = "jakasGrupa6" };
            KategoriaKlienta kk7 = new KategoriaKlienta() { Id = 7, Nazwa = "JakasNazwa7", Grupa = "jakasGrupa7" };

            List<KategoriaKlienta> listaKategoriKlientow = new List<KategoriaKlienta>(){kk1,kk2,kk3,kk4,kk5,kk6,kk7};

            StworzGrupeZKateogrii sgzk = new StworzGrupeZKateogrii();
            sgzk.Kategorie = kategorieDoPrzeniesienia;
            sgzk.Grupa = "NowaGrupa";
            List<KlientKategoriaKlienta> lista = new List<KlientKategoriaKlienta>();

            sgzk.Przetworz(ref listaKategoriKlientow, ref lista);

            Assert.True(listaKategoriKlientow[0].Grupa == "NowaGrupa");
            Assert.True(listaKategoriKlientow[2].Grupa == "NowaGrupa");
            Assert.True(listaKategoriKlientow[4].Grupa == "NowaGrupa");
            Assert.True(listaKategoriKlientow[6].Grupa == "NowaGrupa");
        }
    }
}
