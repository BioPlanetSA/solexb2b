using System.Collections;
using System.Collections.Generic;
using FakeItEasy;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.BLL.ZadaniaKoszyka;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;
using Xunit;

namespace SolEx.Hurt.CoreTests.BLL.ZadaniaKoszyka
{
    public class KonfigurowalneUwagiTest
    {
        [Fact(DisplayName = "Test sprwadzajacy poprawne działanie budowania slownika dla adresow")]
        public void DodajIndormacjeOAdresachTest()
        {
            KonfigurowalneUwagi kb = new KonfigurowalneUwagi();

            IKoszykiBLL koszyki = A.Fake<IKoszykiBLL>();

            Adres adres = new Adres();
            adres.Id = 1;
            adres.Nazwa = "Nazwa";
            adres.Telefon = "telefon";
            adres.KodPocztowy = "kodPocztowy";
            adres.Miasto = "miasto";
            adres.Kraj = "Polska";
            adres.TypAdresu = TypAdresu.Glowny;

            ISolexBllCalosc calosc = A.Fake<ISolexBllCalosc>();
            
            kb.Calosc = calosc;
            A.CallTo(calosc.DostepDane).Where(x => x.Method.Name == "Pobierz").WithReturnType<IList<DokumentyBll>>().WithAnyArguments().Returns(null);
            A.CallTo(() => calosc.DokumentyDostep.PobierzPodsumowanieFakturKlient(A<IKlient>.Ignored)).Returns(null);
            
            A.CallTo(() => koszyki.Adres).Returns(adres);

            kb.Wzor = "{adres_dostawy_nazwa}_{dodatkowe_parametry}_{adres_telefon}_{liczba_produktow}_{dokumenty_przeterminowane_liczba}_{sumaryczneIlosciPozcji}_{adres_dostawy_kraj_platforma}_{adres_dostawy_typ_platforma}_{adres_dostawy_kraj}";

            string oczekiwany = "Nazwa__telefon_0_____Polska";

            kb.Wykonaj(koszyki);
            Assert.Equal(koszyki.Uwagi, oczekiwany);


            adres.Id = -1;
            kb.Wykonaj(koszyki);
            oczekiwany = "___0___Polska_Glowny_";
            Assert.Equal(koszyki.Uwagi, oczekiwany);
        }



    }
}
