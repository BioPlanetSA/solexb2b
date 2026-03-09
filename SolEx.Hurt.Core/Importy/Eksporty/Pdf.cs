using System;
using System.IO;
using System.Text;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core.Importy.Eksporty
{
    [LinkDokumentacji("http://solexb2b.com")]
    [FriendlyName("Umożliwia pobranie dokumentów w formacie pdf")]
    public class Pdf : GenerowanieDokumentu
    {
        public override bool MoznaGenerowac(DokumentyBll dokument)
        {
            bool? czyJestPlik = SolexBllCalosc.PobierzInstancje.Cache.PobierzObiekt<bool?>("pdf{0}", dokument.Id);
            if (!czyJestPlik.HasValue)
            {
                //ta operacja jest dosyć czasochłonna więc mały cache tu jest
                czyJestPlik = SolexBllCalosc.PobierzInstancje.DokumentyDostep.IstniejeZalacznik(dokument.Id, "pdf");
                SolexBllCalosc.PobierzInstancje.Cache.DodajObiekt("pdf{0}", dokument.Id, czyJestPlik.Value);
            }
            return czyJestPlik.Value;
        }

        public override Licencje? WymaganaLicencja
        {
            get { return Licencje.DokumentyPDF;}
        }

        public override Encoding Kodowanie
        {
            get { return Encoding.UTF8; }
        }

        public override string Nazwa
        {
            get { return "Pdf"; }
        }

        public override string PobierzNazwePliku(DokumentyBll dokument)
        {
            return NazwaPliku(dokument) + ".pdf";
        }

        protected override byte[] PobierzDane(DokumentyBll dokument, IKlient klient)
        {           
            return File.ReadAllBytes(SolexBllCalosc.PobierzInstancje.DokumentyDostep.PobierzSciezkePliku(dokument.Id, "pdf"));
        }
    }
}
