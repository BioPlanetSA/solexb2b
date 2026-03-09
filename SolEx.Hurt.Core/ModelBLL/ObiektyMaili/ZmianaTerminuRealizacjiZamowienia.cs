using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Core.ModelBLL.ObiektyMaili
{
    public class ZmianaTerminuRealizacjiZamowienia : SzablonMailaBaza
    {
        public ZmianaTerminuRealizacjiZamowienia(DokumentyBll dokument):base(dokument.DokumentPlatnik)
        {
            Dokument = dokument;
        }
        public ZmianaTerminuRealizacjiZamowienia() : base(null)
        {
            this.ZgodaNaZmianyPrzezKlienta = true;
        }
        public DokumentyBll Dokument { get; set; }
        public override string NazwaFormatu()
        {
            return "Zmiana terminu realizacji zamówienia";
        }

        public override string OpisFormatu()
        {
            return "Mail informujący o zmianie terminu realizacji zamówienia - powiadomienie wysyłane tylko dla zamówień dotychczas nie zrealizowanych";
        }
        public override string OpisDlaKlienta()
        {
            return "Mail informujący o zmianie terminu realizacji zamówienia.";
        }
        public string NowyTermin
        {
            get
            {
                var data = Dokument.DokumentTerminRealizacji.HasValue ? Dokument.DokumentTerminRealizacji.Value.ToShortDateString() : "";
                return data;
            }
        }

        public override TypyPowiadomienia[] PowiadomieniaDomyslnieAktywne
        {
            get { return new[] { TypyPowiadomienia.Klient }; }
        }
    }
}
