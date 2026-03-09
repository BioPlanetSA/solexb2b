using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Interfaces;
using System;
using System.Collections.Generic;

namespace SolEx.Hurt.Core.BLL.ZadaniaKoszyka
{
    public class EksportZamowieniaDoCSV : ZadanieCalegoKoszyka, IZadaniePoZapisieZamowienia, ITestowalna
    {
        [FriendlyName("ID szablonu")]
        [PobieranieSlownika(typeof(SlownikSzablonow))]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public int IdSzablonu { get; set; }

        public List<string> TestPoprawnosci()
        {
            throw new NotImplementedException();
            //var szablony = ImportyDostep.PobierzInstancje.Pobierz( SolexBllCalosc.PobierzInstancje.Konfiguracja.JezykIDPolski);
            //List<string> listaBledow = new List<string>();
            //foreach (var szablon in szablony)
            //{
            //    if (szablon.id == IdSzablonu)
            //    {
            //        return listaBledow;

            //    }
            //}
            //listaBledow.Add(string.Format("Brak szablonu o id {0}",IdSzablonu));
            // return listaBledow;
        }

        public override bool Wykonaj(IKoszykiBLL koszyk)
        {
            try
            {
                throw new NotImplementedException();
                //ImportyBll schemat = ImportyDostep.PobierzInstancje.Pobierz(IdSzablonu,  SolexBllCalosc.PobierzInstancje.Konfiguracja.JezykIDPolski);

                //string nazwaPliku = "";
                //string dane = ImportyDostep.PobierzInstancje.Wykonaj(schemat, SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<Klient>(SesjaHelper.PobierzInstancje.KlientID), out nazwaPliku, new List<object> {koszyk.SlownikParametrow.IdZamowienia});
                //string sciezka = string.Format(@"{0}\\Zasoby\\WydrukiZamowien\CSV\\", AppDomain.CurrentDomain.BaseDirectory);
                //Directory.CreateDirectory(sciezka);
                //File.WriteAllText(sciezka + string.Format("zamowienie-{0}.csv", koszyk.SlownikParametrow.IdZamowienia), dane, Encoding.Default);
                //return true;
            }
            catch (Exception e)
            {
                Log.Error(e);
                return false;
            }
        }
    }
}