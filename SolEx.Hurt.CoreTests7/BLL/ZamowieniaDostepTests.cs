using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model;
using Xunit;
namespace SolEx.Hurt.Core.BLL.Tests
{
    public class ZamowieniaDostepTests
    {
        [Fact(DisplayName = "Numer dokumentu dla partnera (GLOBAL)")]
        public void PobierzNumerDokumentuPartneraTest()
        {
            int idPrzedstawiciela = 100;

            ZamowieniaDostep zamowieniaDostep = A.Fake<ZamowieniaDostep>();

            IKlient klient1 = A.Fake<IKlient>();
            A.CallTo(() => klient1.przedstawiciel_id).Returns(idPrzedstawiciela);

            IKlient klient2 = A.Fake<IKlient>();
            A.CallTo(() => klient2.przedstawiciel_id).Returns(idPrzedstawiciela+1);


            Dictionary<int, ZamowieniaBLL> zamowienia = WygenerujZamowienia(klient1, klient2);

            A.CallTo(() => zamowieniaDostep.PobierzZamowienia(A<IKlient>.Ignored)).Returns(zamowienia);

            int numer = zamowieniaDostep.PobierzNumerDokumentuPartneraGlownaMetoda(idPrzedstawiciela, 2014);

            Assert.Equal(2, numer);
        }


        [Fact()]
        public void GenerujNumerZamowieniaDlaOddzialuTest()
        {
            //int idPrzedstawiciela = 100;
            //string nazwaOddzialu = "DZIUBEK";
            //int rok = 2014;

            //ZamowieniaDostep zamowieniaDostep = A.Fake<ZamowieniaDostep>();
            
            //IKlient klient1 = A.Fake<IKlient>();
            //A.CallTo(() => klient1.przedstawiciel_id).Returns(idPrzedstawiciela);
            //A.CallTo(() => klient1.OddzialDoJakiegoNalezyKlient).Returns(idPrzedstawiciela);
            //A.CallTo(() => klient1.klient_id).Returns(69);
            //A.CallTo(() => klient1.OddzialDoJakiegoNalezyKlientNazwa).Returns(nazwaOddzialu);

            //KlienciDostep klienciDostep = A.Fake<KlienciDostep>();
            //zamowieniaDostep.Klienci = klienciDostep;
            //A.CallTo(() => klienciDostep.Pobierz(klient1.klient_id)).Returns(klient1);

            //A.CallTo(() => zamowieniaDostep.PobierzNumerDokumentuPartnera(klient1.przedstawiciel_id.Value, rok))
            //    .Returns(500);

            //string numer = zamowieniaDostep.GenerujNumerZamowieniaDlaOddzialu(1093, klient1.klient_id, rok);

            //Assert.Equal(string.Format("{0}/{1}/{2}", 500, nazwaOddzialu, rok), numer);

        }

        private Dictionary<int, ZamowieniaBLL> WygenerujZamowienia(IKlient klientWlasciwy, IKlient klientSmieciowy)
        {


            Dictionary<int, ZamowieniaBLL> slownik = new Dictionary<int, ZamowieniaBLL>();

            ZamowieniaBLL zamowienie1 = A.Fake<ZamowieniaBLL>();
            A.CallTo(() => zamowienie1.Klient).Returns(klientWlasciwy);
            A.CallTo(() => zamowienie1.data_utworzenia).Returns(new DateTime(2013,1,1));
            A.CallTo(() => zamowienie1.IdZamowienia).Returns(1);

            slownik.Add(zamowienie1.IdZamowienia, zamowienie1);

            ZamowieniaBLL zamowienie2 = A.Fake<ZamowieniaBLL>();
            A.CallTo(() => zamowienie2.Klient).Returns(klientWlasciwy);
            A.CallTo(() => zamowienie2.data_utworzenia).Returns(new DateTime(2014, 1, 1));
            A.CallTo(() => zamowienie2.IdZamowienia).Returns(2);

            slownik.Add(zamowienie2.IdZamowienia, zamowienie2);

            ZamowieniaBLL zamowienie3 = A.Fake<ZamowieniaBLL>();
            A.CallTo(() => zamowienie3.Klient).Returns(klientSmieciowy);
            A.CallTo(() => zamowienie3.data_utworzenia).Returns(new DateTime(2014, 1, 1));
            A.CallTo(() => zamowienie3.IdZamowienia).Returns(3);

            slownik.Add(zamowienie3.IdZamowienia, zamowienie3);

            ZamowieniaBLL zamowienie4 = A.Fake<ZamowieniaBLL>();
            A.CallTo(() => zamowienie4.Klient).Returns(klientSmieciowy);
            A.CallTo(() => zamowienie4.data_utworzenia).Returns(new DateTime(2014, 2, 1));
            A.CallTo(() => zamowienie4.IdZamowienia).Returns(4);

            slownik.Add(zamowienie4.IdZamowienia, zamowienie4);

            return slownik;
        }

    }
}
