using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Extensions;
using SolEx.Hurt.Model.Helpers;
using Xunit;


namespace SolEx.Hurt.Helpers.Tests
{
    public class PropertyInfoExtensionTests
    {
        [Fact(DisplayName = "Poprawność zapisu propertisów metodą SetValueExt dla typu bool")]
        public void SetValueExtBoolTest()
        {
            SetValueTest(true, true, "BlokadaZamowien", false);
            SetValueTest(false, false, "BlokadaZamowien", false);
            SetValueTest("trUe", true, "BlokadaZamowien", false);
            SetValueTest("fAlse", false, "BlokadaZamowien", false);
            SetValueTest("tAk", true, "BlokadaZamowien", false);
            SetValueTest("nIe", false, "BlokadaZamowien", false);
            SetValueTest(1, true, "BlokadaZamowien", false);
            SetValueTest(0, false, "BlokadaZamowien", false);
            SetValueTest(3, false, "BlokadaZamowien", true);
            SetValueTest("T", true, "BlokadaZamowien", false);
            SetValueTest("N", false, "BlokadaZamowien", false);
            SetValueTest("Y", true, "BlokadaZamowien", false);
            SetValueTest("f", false, "BlokadaZamowien", false);
            SetValueTest("Y0", false, "BlokadaZamowien", true);
            SetValueTest(true, true, "BlokadaZamowien", false);
            SetValueTest("cisowianka", false, "BlokadaZamowien", true);
        }

        [Fact(DisplayName = "Poprawność zapisu propertisów metodą SetValueExt dla typu bool?")]
        public void SetValueExtBoolNullTest()
        {
            SetValueTest(true, true, "WidziPunkty", false);
            SetValueTest(false, false, "WidziPunkty", false);
            SetValueTest("trUe", true, "WidziPunkty", false);
            SetValueTest("fAlse", false, "WidziPunkty", false);
            SetValueTest("tAk", true, "WidziPunkty", false);
            SetValueTest("nIe", false, "WidziPunkty", false);
            SetValueTest(1, true, "WidziPunkty", false);
            SetValueTest(0, false, "WidziPunkty", false);
            SetValueTest(3, null, "WidziPunkty", true);
            SetValueTest("T", true, "WidziPunkty", false);
            SetValueTest("N", false, "WidziPunkty", false);
            SetValueTest("Y", true, "WidziPunkty", false);
            SetValueTest("f", false, "WidziPunkty", false);
            SetValueTest("Y0", null, "WidziPunkty", true);
            SetValueTest(true, true, "WidziPunkty", false);
            SetValueTest("cisowianka", null, "WidziPunkty", true);
        }


        [Fact(DisplayName = "Poprawność zapisu propertisów metodą SetValueExt dla enum")]
        public void SetValueExtEnum()
        {
            Klient klient = new Model.Klient();
            string pole = new { klient.PowodBlokady}.Properties().First().Key;
            SetValueTest(BlokadaPowod.BrakFaktur, BlokadaPowod.BrakFaktur, pole, false);
            SetValueTest("BrakFaktur", BlokadaPowod.BrakFaktur, pole, false);
            SetValueTest("bezsensu", BlokadaPowod.Brak, pole, false);
        }
        [Fact(DisplayName = "Poprawność zapisu propertisów metodą SetValueExt dla datetime")]
        public void SetValueExtDateTime()
        {
            Klient klient = new Model.Klient();
            string pole = new {klient.DataDodatnia}.Properties().First().Key;
            SetValueTest(DateTime.Now.Date, DateTime.Now.Date, pole, false);
            SetValueTest("1.1.2014",new DateTime(2014,1,1), pole, false);
            SetValueTest("bezsensu",null, pole, true);
        }

        public void SetValueTest(object wartoscWejsciowa, object wartoscOczekiwana, string pole, bool czyMaBycWyjatek)
        {
            Klient klient = new Model.Klient();

            bool blad = SetValue(klient, pole, wartoscWejsciowa);
            //czy powinno zwrócić wyjątek
            Assert.Equal(czyMaBycWyjatek, blad);

            object wartoscZwrocona = GetValue(klient, pole);

            //sprawdzanie czy zwrócona wartość jest prawidłowa
            Assert.Equal(wartoscOczekiwana, wartoscZwrocona);

        }

        private object GetValue(object obiekt, string nazwaPola)
        {
            var typ = obiekt.GetType();

            PropertyInfo propertis = typ.Properties().FirstOrDefault(x => x.Key == nazwaPola).Value;
            if (propertis != null)
            {
                return propertis.GetValue(obiekt);
            }
            throw new Exception("Nie znaleziono podanego pola!");
        }

        private bool SetValue(object obiekt, string nazwaPola, object wartosc)
        {
            var typ = obiekt.GetType();

            PropertyInfo propertis = typ.Properties().FirstOrDefault(x => x.Key == nazwaPola).Value;

            try
                    {
                        propertis.SetValueExt(obiekt, wartosc);
                    }
                    catch(Exception )
                    {
                        return true;
                    }
            
            return false;
        }

    }
}
