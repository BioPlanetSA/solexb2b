using Xunit;
using SolEx.Hurt.Core.BLL;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core.BLL.Tests
{
    public class StatystykiTests
    {
        [Fact(DisplayName = "rozpoznanie ktory klient jet nowy i wyslac mu maila powitalnego")]
        public void ZdarzenieNowyKlientTest()
        {
            Statystyki stat = A.Fake<Statystyki>();

            IList<IKlient> starzy = new List<IKlient>
            {
                new Klient() {Id = 5, Email = "fss@c.pl", Aktywny = true},
                new Klient() {Id = 3, Email = "fsds@c.pl", Aktywny = false},
                new Klient() {Id = 11, Email = "fssdsf@c.pl", Aktywny = false},
                new Klient() {Id = 1, Email = "fssdsf@c.pl", Aktywny = true}
            };

            IList<IKlient> nowi = new List<IKlient>
            {
                new Klient() {Id = 56, Email = "fss@c.pl", Aktywny = true},
                new Klient() {Id = 3, Email = "fsfs@c.pl", Aktywny = true},
                new Klient() {Id = 11, Email = "fsfds@c.pl", Aktywny = false},
                new Klient() {Id = 1, Email = "fssdsf@c.pl", Aktywny = true}
            };


            IList<Model.Klient> starzy2 = new List<Model.Klient>()
            {
                new Model.Klient() {Id = 5, Email = "fss@c.pl", Aktywny = true},
                new  Model.Klient() {Id = 3, Email = "fsds@c.pl", Aktywny = false},
                new  Model.Klient() {Id = 11, Email = "fssdsf@c.pl", Aktywny = false},
                new  Model.Klient() {Id = 1, Email = "fssdsf@c.pl", Aktywny = true}
            };
            IList<Model.Klient> nowi2 = new List<Model.Klient>
            {
                new Model.Klient() {Id = 56, Email = "fss@c.pl", Aktywny = true},
                new Model.Klient() {Id = 3, Email = "fsfs@c.pl", Aktywny = true},
                new Model.Klient() {Id = 11, Email = "fsfds@c.pl", Aktywny = false},
                new Model.Klient() {Id = 1, Email = "fssdsf@c.pl", Aktywny = true}
            };

            stat.AktualizacjaKlientow_RozpoznanieNowychKlientow(starzy, nowi);


            //powinni wybrać TYLKO: 56 i 3 - czyli dwa pierwsze wiersze z nowych

            A.CallTo(() => stat.AktualizacjaKlientow(nowi[0])).MustHaveHappened();
            A.CallTo(() => stat.AktualizacjaKlientow(nowi[1])).MustHaveHappened();

            //i tylko 2 powyższe wystąpienia
            A.CallTo(() => stat.AktualizacjaKlientow(null)).WithAnyArguments().MustHaveHappened(Repeated.Exactly.Twice);



            Statystyki stat2 = A.Fake<Statystyki>();

            IList<IKlient> starzyPoRzutowaniu = starzy2.Select(x => new Klient(x) as IKlient).ToList();
            IList<IKlient> nowiPoRzutowaniu = nowi2.Select(x => new Klient(x) as IKlient).ToList();

            stat2.AktualizacjaKlientow_RozpoznanieNowychKlientow(starzyPoRzutowaniu, nowiPoRzutowaniu);

            //powinni wybrać TYLKO: 56 i 3 - czyli dwa pierwsze wiersze z nowych

            A.CallTo(() => stat2.AktualizacjaKlientow(nowiPoRzutowaniu[0])).MustHaveHappened();
            A.CallTo(() => stat2.AktualizacjaKlientow(nowiPoRzutowaniu[1])).MustHaveHappened();

            //i tylko 2 powyższe wystąpienia
            A.CallTo(() => stat2.AktualizacjaKlientow(null)).WithAnyArguments().MustHaveHappened(Repeated.Exactly.Twice);
        }
    }
}