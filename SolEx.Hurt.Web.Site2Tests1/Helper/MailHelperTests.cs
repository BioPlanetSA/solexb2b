using Xunit;
using System;
using System.Collections.Generic;
using FakeItEasy;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Core.DostepDane;
using SolEx.Hurt.Core.ModelBLL.ObiektyMaili;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Web.Site2.Helper.Tests
{
    public class MailHelperTests: MailHelper
    {

        private UstawieniePowiadomienia GenerujUstawieniePowiadomienia()
        {
            UstawieniePowiadomienia ustawienie = A.Fake<UstawieniePowiadomienia>();

            ustawienie.ParametryWysylania = new List<ParametryWyslania>();

            ustawienie.ParametryWysylania.Add(new ParametryWyslania() { Aktywny = true, DoKogo = TypyPowiadomienia.Klient, EmailBcc = "testbcc@vsssolex.com;mail2@gmmm.com" });
            ustawienie.ParametryWysylania.Add(new ParametryWyslania() { Aktywny = true, DoKogo = TypyPowiadomienia.DrugiOpiekun, EmailBcc = "testbcc@vsssolex.com;mail2@gmmm.com" });
            ustawienie.ParametryWysylania.Add(new ParametryWyslania() { Aktywny = true, DoKogo = TypyPowiadomienia.Opiekun, EmailBcc = "testbcc@vsssolex.com;mail2@gmmm.com" });
            ustawienie.ParametryWysylania.Add(new ParametryWyslania() { Aktywny = true, DoKogo = TypyPowiadomienia.Przedstawiciel, EmailBcc = "testbcc@vsssolex.com;mail2@gmmm.com" });
            return ustawienie;
        }

        private Jezyk jezykTestowyEn = A.Fake<Jezyk>();
        private Jezyk jezykTestowyPl = A.Fake<Jezyk>();
        
        private ISolexBllCalosc calosc = A.Fake<ISolexBllCalosc>();
        private IDaneDostep dane = A.Fake<IDaneDostep>();

        IConfigBLL config = A.Fake<IConfigBLL>();

        private Dictionary<int, Jezyk> slownikJezykow = null;

        protected void konfiguracjaDlaWieluJezykow()
        {
            slownikJezykow = new Dictionary<int, Jezyk> { { jezykTestowyEn.Id, jezykTestowyEn }, { jezykTestowyPl.Id, jezykTestowyPl } };
            A.CallTo(() => config.WieleJezykowWSystemie).Returns(true);
            A.CallTo(() => config.JezykIDDomyslny).Returns(jezykTestowyEn.Id);
            A.CallTo(() => config.JezykiWSystemie).Returns(slownikJezykow);
            A.CallTo(() => config.AdresStronyZProduktem).Returns("p");

            A.CallTo(() => calosc.Konfiguracja).Returns(config);
            A.CallTo(() => calosc.DostepDane).Returns(dane);
        }

        Exception e;

        [Fact(DisplayName = "Wysyłanie maila")]
        public void WyslijMaileTest()
        {
            IKlient klient = A.Fake<IKlient>();
            klient.Id = 44;
            klient.Email = "test@solex.net.pl";
            klient.JezykId = 2;

            WiadomoscEmail wiadomosc = new WiadomoscEmail("muminkki@solex.net.pl", "fsdfs@soelx.cccc.com", null);

            SzablonMailaBaza szablonMaila = A.Fake<SzablonMailaBaza>();
            szablonMaila.DoKogoWysylany = TypyPowiadomienia.Klient;
            szablonMaila.Klient = klient;

            UstawieniePowiadomienia ustawienie = this.GenerujUstawieniePowiadomienia();
            ustawienie.ZgodaNaZmianyPrzezKlienta = false;

            ISolexBllCalosc calosc = A.Fake<ISolexBllCalosc>();
            IDaneDostep dostepDanych = A.Fake<IDaneDostep>();
            IMaileBLL maile = A.Fake<IMaileBLL>();

            A.CallTo(() => calosc.MaileBLL).Returns(maile);
            A.CallTo(() => calosc.DostepDane).Returns(dostepDanych);
            A.CallTo(() => maile.WyslijEmaila(wiadomosc, null, TypyUstawieniaSkrzynek.Newsletter, out e, true, true)).WithAnyArguments().Returns(true);
            A.CallTo(dostepDanych).Where(x => x.Method.Name == "PobierzPojedynczy").WithReturnType<UstawieniePowiadomienia>().WithAnyArguments().Returns(ustawienie);



            MailHelper mail = A.Fake<MailHelper>();
            mail.Calosc = calosc;
            
            A.CallTo(() => mail.ParsujSzablon(null, out e, true)).WithAnyArguments().Returns(wiadomosc);

            mail.WyslijMaile(szablonMaila);
            
            //sprawdzmay czy bylo wywolanie maila - jak nie to nie wyslalo sie nic
            A.CallTo(() => calosc.MaileBLL.WyslijEmaila(null, null, TypyUstawieniaSkrzynek.Ogolne, out e, true, true)).WithAnyArguments().MustHaveHappened();

            ustawienie.ZgodaNaZmianyPrzezKlienta = true;
            IProfilKlientaBll profil = A.Fake<IProfilKlientaBll>();
            A.CallTo(() => calosc.ProfilKlienta).Returns(profil);
            A.CallTo(() => profil.PobierzWartosc<bool>(klient, TypUstawieniaKlienta.PowiadomieniaMailowe, A<string>.Ignored)).Returns(true);

            mail.WyslijMaile(szablonMaila);
            A.CallTo(() => calosc.MaileBLL.WyslijEmaila(null, null, TypyUstawieniaSkrzynek.Ogolne, out e, true, true)).WithAnyArguments().MustHaveHappened();

            A.CallTo(() => profil.PobierzWartosc<bool>(klient, TypUstawieniaKlienta.PowiadomieniaMailowe, A<string>.Ignored)).Returns(false);

            mail.WyslijMaile(szablonMaila);
            A.CallTo(() => calosc.MaileBLL.WyslijEmaila(null, null, TypyUstawieniaSkrzynek.Ogolne, out e, true, true)).WithAnyArguments().MustHaveHappened();

            ustawienie.ParametryWysylania[0].EmailBcc=string.Empty;
            mail.WyslijMaile(szablonMaila);
            A.CallTo(() => calosc.MaileBLL.WyslijEmaila(null, null, TypyUstawieniaSkrzynek.Ogolne, out e, true, true)).MustNotHaveHappened();
        }
    }
}