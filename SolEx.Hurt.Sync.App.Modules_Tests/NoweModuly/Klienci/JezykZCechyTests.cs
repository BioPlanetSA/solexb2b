using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Interfaces;
using SolEx.Hurt.Sync.App.Modules_.NoweModuly.Klienci;
using Xunit;
namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Klienci.Tests
{
    public class JezykZCechyTests
    {
        [Fact(DisplayName = "Test sprawdzajacy poprawnosci przypisywania jezyka klientowi na podstawie cechy z platformy b2b")]
        public void PrzetworzTest()
        {
            string grupa = "JakasGrupa";


            Dictionary<int, Jezyk> Jezyki = new Dictionary<int, Jezyk>();
            Jezyk j1= new Jezyk(){Id=1, Nazwa = "PierwszyJezyk", Symbol = "PJ"};
            Jezyk j2= new Jezyk(){Id=2, Nazwa = "DrugiJezyk", Symbol = "DJ"};
            Jezyk j3= new Jezyk(){Id=3, Nazwa = "TrzeciJezyk", Symbol = "TJ"};
            Jezyki.Add(j1.Id, j1);
            Jezyki.Add(j2.Id, j2);
            Jezyki.Add(j3.Id, j3);

            var config = A.Fake<IConfigBLL>();
            A.CallTo(() => config.JezykiWSystemie).Returns(Jezyki);


            Klient k1 = new Klient() { Id = 1, Nazwa = "Klient1" };
            Klient k2 = new Klient() { Id = 2, Nazwa = "Klient2" };
            Klient k3 = new Klient() { Id = 3, Nazwa = "Klient3" };
            Klient k4 = new Klient() { Id = 4, Nazwa = "Klient4" };
            Dictionary<long, Klient> listaWejsciowa = new Dictionary<long, Klient>();
            listaWejsciowa.Add(k1.Id, k1);
            listaWejsciowa.Add(k2.Id, k2);
            listaWejsciowa.Add(k3.Id, k3);
            listaWejsciowa.Add(k4.Id, k4);
            KategoriaKlienta kk1 = new KategoriaKlienta(){Id=1, Nazwa = "jakasNazwa1", Grupa = grupa};
            KategoriaKlienta kk2 = new KategoriaKlienta(){Id=2, Nazwa = "jakasNazwa2", Grupa = "JakasNowaGrupa"};
            KategoriaKlienta kk3 = new KategoriaKlienta(){Id=3, Nazwa = "jakasNazwa3", Grupa = grupa};



            var klienciDostep = A.Fake<IKlienciDostep>();
            //A.CallTo(()=>klienciDostep.PobierzKategorieKlienta())




            JezykZCechy jzc = new JezykZCechy();
            jzc.GrupaKlienta = grupa;
            jzc.WybranePoleJezyka =JezykZCechy.PolaJezyka.Nazwa;
        }
    }
}
