using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Core.Importy.Eksporty
{
   [FriendlyName("Format XML jest dedykowany do budowy własnych rozwiązań informatycznych. Plik jest czytelny tylko dla informatyków za pomocą specjalnych programów. Zawartość pliku to pozycje dokumentu oraz dane nagłówkowe dokumentu")]
   public  class Xml : OptimaXml
    {
   
       //protected override int IdSzablonu
       //{
       //    get
       //    {
       //        return  SolexBllCalosc.PobierzInstancje.Konfiguracja.XmlDomyslnySzablon;
       //    }
       //}
        
        public override Licencje? WymaganaLicencja
        {
            get {return Licencje.DokumentyXML;}
        }

   
        public override string Nazwa
        {
            get { return "Xml"; }
        }

        public override string PobierzNazwePliku(DokumentyBll dokument)
        {
            return NazwaPliku(dokument) + "-b2b.xml";
        }
    }
}
