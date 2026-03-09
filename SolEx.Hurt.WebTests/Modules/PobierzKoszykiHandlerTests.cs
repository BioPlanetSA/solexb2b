using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using FakeItEasy;
using SolEx.Hurt.Core;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model;
using SolEx.Hurt.Web.Modules;
using Xunit;
using Klient = SolEx.Hurt.Core.Klient;

namespace SolEx.Hurt.Web.Modules.Tests
{
    public class PobierzKoszykiHandlerTests
    {
        private List<KoszykBll> WygenerujKoszyki(int ile, Klient klient)
        {
            List<KoszykBll> listakoszykow = new List<KoszykBll>(ile);
            for (int i = 0; i < ile; i++)
            {
                KoszykBll k = new KoszykBll();
                k.Id = i;
                k.KlientId = 69;

                //List<IKoszykPozycja> pozycjeKoszyka = new List<IKoszykPozycja>(10);
                //KoszykBll koszyk = new KoszykBll(k, 1, pozycjeKoszyka);
                //for (int j = 0; j < 10; j++)
                //{
                //    KoszykPozycje pozycja = new KoszykPozycje();
                //    pozycja.Id = j;
                //    pozycja.ProduktId = j;
                //    pozycja.KoszykId = i;
                //    pozycja.Ilosc = i + j;


                //    JednostkaProduktu jednostka = new JednostkaProduktu();
                //    jednostka.ProduktId = pozycja.ProduktId;
                //    jednostka.Id = 1;
                //    jednostka.Nazwa = "szt.";

                //    //pozycjeKoszyka.Add(new KoszykPozycje(pozycja, 1, klient, jednostka, koszyk));
                //}
                //listakoszykow.Add(koszyk);
            }
            return listakoszykow;
        }
    }
}
