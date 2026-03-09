using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using SolEx.Hurt.Model;
using SolEx.Hurt.Sync.App.Modules_.NoweModuly.KategorieKlientow;
using Xunit;
namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.KategorieKlientow.Tests
{
    public class FiltrowanieKategoriKlientowTests
    {
        [Fact(DisplayName = "Test sprawdzajacy poprawnosc filtrowania kategori klienta")]
        public void PrzetworzTest()
        {
            string grupa = "Haslo";
            string grupa2 = "cos";
            KategoriaKlienta k1 = new KategoriaKlienta() { Grupa = "JACEK LIS", Id = 1, Nazwa = "rabat_pilchb_Kurier:20" };
            KategoriaKlienta k2 = new KategoriaKlienta() { Grupa = grupa, Id = 2, Nazwa = "Nazwa2" };
            KategoriaKlienta k3 = new KategoriaKlienta() { Grupa = "OPOCZNO", Id = 3, Nazwa = "rabat_incana_Platny kurier:40" };
            KategoriaKlienta k4 = new KategoriaKlienta() { Grupa = "TESTOWA", Id = 4, Nazwa = "rabat_tubądzin_Platny kurier:32" };
            KategoriaKlienta k5 = new KategoriaKlienta() { Grupa = "WIKTOR", Id = 5, Nazwa = "rabat_kwadro_Odbiór Świecie:25" };
            KategoriaKlienta k6 = new KategoriaKlienta() { Grupa = grupa2, Id = 6, Nazwa = "Nazwa6" };
            KategoriaKlienta k7 = new KategoriaKlienta() { Grupa = grupa, Id = 7, Nazwa = "Nazwa7" };
            KategoriaKlienta k8 = new KategoriaKlienta() { Grupa = "WYSYŁKA POLSKA", Id = 8, Nazwa = "rabat_incana_Odbiór Świecie:38" };
            KategoriaKlienta k9 = new KategoriaKlienta() { Grupa = grupa, Id = 9, Nazwa = "Nazwa9" };
            List<KategoriaKlienta> kategorie = new List<KategoriaKlienta>(){k1,k2,k3,k4,k5,k6,k7,k8,k9};

            FiltrowanieKategoriKlientow fkk = new FiltrowanieKategoriKlientow();
            fkk.Grupy = "ADRES DOSTAWY;BEZRABATÓW;DOSTAWA;DOSTAWY;ELECCIO;HASŁO;JACEK LIS;OPOCZNO;PŁATNOŚCI;PRIORYTET;SWIECIE;TESTOWA;WIKTOR;WYSYŁKA POLSKA";


            KlientKategoriaKlienta kk1 = new KlientKategoriaKlienta(){ KategoriaKlientaId = 1, KlientId = 1};
            KlientKategoriaKlienta kk2 = new KlientKategoriaKlienta() { KategoriaKlientaId = 6, KlientId = 1 };
            KlientKategoriaKlienta kk3 = new KlientKategoriaKlienta() {KategoriaKlientaId = 3, KlientId = 1 };
            

            List<KlientKategoriaKlienta>kk = new List<KlientKategoriaKlienta>(){kk1,kk2,kk3};
            fkk.Przetworz(ref kategorie,ref kk);
            Assert.True(kategorie.Count==4);
           

            Assert.True(kk.Count==1);
        }

        [Fact(DisplayName = "trade")]
        public void WykonajTest()
        {
            string stream = File.ReadAllText(@"C:\Projekty\SolexB2B\SolEx.Hurt.Web.Site2\static\obrazki_ustawienie.config", Encoding.UTF8);
            List<string> lista = new List<string>();
            using (XmlReader reader = new XmlTextReader(new StringReader(stream)))
            {
                while (reader.Read())
                {
                    if (reader.Name.Equals("preset"))
                    {
                        var name = reader.GetAttribute("name");
                        lista.Add(name);
                    } 
                }
            }
        }
    }
}
