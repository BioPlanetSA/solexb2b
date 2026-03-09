using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.Core;
using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Core.ModelBLL.ObiektyMaili
{
    public class NowyDokument : SzablonMailaBaza
    {
        public NowyDokument() : base(null)
        {
            this.ZgodaNaZmianyPrzezKlienta = true;
        }

        public NowyDokument(List<DokumentyBll> dokument) : base(dokument.First().DokumentPlatnik)
        {
            Dokumenty = dokument;
        }
        public override string NazwaFormatu()
        {
            return "Nowe faktury wystawione dla klienta";
        }
        public List<DokumentyBll> Dokumenty { get; set; }

        public override string OpisFormatu()
        {
            return "Mail informujący o pojawieniu się nowej faktury w platformie - wystarczy sam zapis o fakturze, powiadomienie nie czeka aż PDF będzie dostępny. " +
                   "W mailu wysyłana jest inforamcja czy plik PDF jest dostępny dla faktury. " +
                   "Powiadomienie wysyłane przez moduł synchronizacji 'Wyślij powiadomienie o nowych dokumentach' lub przez " +
                   "API: api/Dokumenty.elektroniczne.wysylanie.ashx.";
        }
        public override string OpisDlaKlienta()
        {
            return "Mail informujący o pojawieniu się nowej faktury w platformie.";
        }
        public string SciezkaDoPlikuPobierz(DokumentyBll dokument)
        {
            var pdf = SolexBllCalosc.PobierzInstancje.DokumentyDostep.PobierzDostepneFormatyDoPobrania(dokument).FirstOrDefault(x => x.Nazwa == "Pdf");
            return pdf != null ? string.Format("{0}/{1}",Konfiguracja.wlasciciel_AdresPlatformy, pdf.WygenerujLink()) : "";
        }

        public string SciezkaDoPlikuPokaz(DokumentyBll dokument)
        {
            return string.Format("{0}/Dokumenty/Pokaz/{1}", Konfiguracja.wlasciciel_AdresPlatformy, dokument.Id);
        }

        public WartoscLiczbowa CalkowitaWartoscBruttoDokumentow
        {
            get
            {
                return new WartoscLiczbowa(Dokumenty.Sum(dokument => dokument.DokumentWartoscBrutto), Dokumenty.First().walutaB2b); 
            }
        }
        public WartoscLiczbowa CalkowitaWartoscNettoDokumentow
        {
            get
            {
                return new WartoscLiczbowa(Dokumenty.Sum(dokument => dokument.DokumentWartoscNetto), Dokumenty.First().walutaB2b); 

            }
        }

        public override TypyPowiadomienia[] PowiadomieniaDomyslnieAktywne
        {
            get { return new[] {TypyPowiadomienia.Klient}; }
        }

    }
}
