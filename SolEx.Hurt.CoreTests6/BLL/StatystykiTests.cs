//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using FakeItEasy;
//using SolEx.Hurt.Core.BLL;
//using SolEx.Hurt.Core.ModelBLL;
//using SolEx.Hurt.Model.Enums;
//using SolEx.Hurt.Model.Interfaces;
//using SolEx.Hurt.Model.Interfaces.Modele;
//using Xunit;
//namespace SolEx.Hurt.Core.BLL.Tests
//{
//    public class StatystykiTests
//    {
//        [Fact(DisplayName = "Sprawdzanie czy klient może pobrać dane przez API")]
//        public void SprawdzCzestotliwoscPobieraniaPrzezApiTest()
//        {
//            //wygenerowanych jest 6 akcji klienta, w tym 4 dla API z czego najstarsze jest sprzed 10 minut a najnowsze sprzed 2 minut  

//            //klient może pobrać bo ma 10 pobrań na minutę
//            SprawdzCzestotliwoscPobieraniaPrzezApi(true, 1, 10);

//            //klient może pobrać bo ma 2 pobrania na minutę
//            SprawdzCzestotliwoscPobieraniaPrzezApi(true, 1, 2);

//            //klient nie może pobrać bo ma 1 pobranie na 10 minut
//            SprawdzCzestotliwoscPobieraniaPrzezApi(false, 10, 1);
//        }

//        public void SprawdzCzestotliwoscPobieraniaPrzezApi(bool oczekiwana, int limitokres, int pobrannaokres)
//        {
//            IConfigBLL config = A.Fake<IConfigBLL>();
//            A.CallTo(() => config.SferaPobieranieLimitOkres).Returns(limitokres);
//            A.CallTo(() => config.SferaMaksPobranNaOkres).Returns(pobrannaokres);

//            Statystyki statystyki = A.Fake<Statystyki>();
//            statystyki.Iconfigbll = config;

//            IKlienci klient = A.Fake<IKlienci>();
//            A.CallTo(() => klient.email).Returns("test@test.pl");

//            A.CallTo(() => statystyki.PobierzStatystyki()).Returns(PobierzStatystyki(klient.email));
//            string komunikat;
//            bool wynik = statystyki.SprawdzCzestotliwoscPobieraniaPrzezApi(klient, out komunikat, 1);
//            Assert.Equal(oczekiwana, wynik);
//        }

//        private List<AkcjaKlienta> PobierzStatystyki(string emailKlienta)
//        {
//            string ip = "92.68.1.100";
//            List<AkcjaKlienta> akcje = new List<AkcjaKlienta>();
//            akcje.Add(new AkcjaKlienta() { ZdarzenieGlowne = ZdarzenieGlowne.Sfera, Data = DateTime.Now.AddMinutes(-10),EmailKlienta  = emailKlienta, IpKlienta = ip});
//            akcje.Add(new AkcjaKlienta() { ZdarzenieGlowne = ZdarzenieGlowne.Sfera, Data = DateTime.Now.AddMinutes(-4), EmailKlienta = emailKlienta, IpKlienta = ip });
//            akcje.Add(new AkcjaKlienta() { ZdarzenieGlowne = ZdarzenieGlowne.Sfera, Data = DateTime.Now.AddMinutes(-3), EmailKlienta = emailKlienta, IpKlienta = ip });
//            akcje.Add(new AkcjaKlienta() { ZdarzenieGlowne = ZdarzenieGlowne.PobieranieDokumentu, Data = DateTime.Now.AddMinutes(-3), EmailKlienta = emailKlienta, IpKlienta = ip });
//            akcje.Add(new AkcjaKlienta() { ZdarzenieGlowne = ZdarzenieGlowne.Produkty, Data = DateTime.Now.AddMinutes(-3), EmailKlienta = emailKlienta, IpKlienta = ip });
//            akcje.Add(new AkcjaKlienta() { ZdarzenieGlowne = ZdarzenieGlowne.Sfera, Data = DateTime.Now.AddMinutes(-2), EmailKlienta = emailKlienta, IpKlienta = ip });

//            return akcje;

//        }
//    }
//}

