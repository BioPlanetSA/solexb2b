using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core.ModelBLL.ObiektyMaili
{
    public abstract class PowiadomienieZZamowieniem : SzablonMailaBaza
    {
        public PowiadomienieZZamowieniem() { }
        protected PowiadomienieZZamowieniem(ZamowieniaBLL zamowienia, IKlient klient) : base(klient)
        {
            Zamowienie = zamowienia;
        }

        public override string NazwaSzablonu()
        {
            return "NoweZamowienie";
        }

        public ZamowieniaBLL Zamowienie { get; set; }

        public string PelnyAdres()
        {
            Model.Adres adres = Zamowienie.Adres;
            string wynik = string.Empty;
            if (!string.IsNullOrEmpty(adres.UlicaNr))
            {
                wynik += adres.UlicaNr + ", ";
            }
            if (!string.IsNullOrEmpty(adres.Miasto))
            {
                wynik += adres.Miasto + " ";
            }
            if (!string.IsNullOrEmpty(adres.KodPocztowy))
            {
                wynik += adres.KodPocztowy + ", ";
            }
            if (adres.KrajId.HasValue)
            {
                var kraj = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<Kraje>(adres.KrajId.Value);
                wynik += kraj.Nazwa + ", ";
            }
            if (!string.IsNullOrEmpty(adres.Telefon))
            {
                wynik += adres.Telefon + ", ";
            }
            return wynik.Trim().TrimEnd(',');
        }
    }
}