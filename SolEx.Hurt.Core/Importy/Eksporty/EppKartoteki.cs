using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core.Importy.Eksporty
{
    [LinkDokumentacji("http://bok.solexb2b.com/index.php?/Knowledgebase/Article/View/89/0/import-pliku-epp")]
    [FriendlyName("Format plików EPP jest przygotowany tak aby umożliwić szybki i łatwy import faktur i zamówień do SubiektaGT")]
    public class EppKartoteki : Epp
    {
        protected override byte[] PobierzDane(DokumentyBll dokument, IKlient klient)
        {
            return Generuj(dokument, klient, true);
        }

        public override string Nazwa
        {
            get { return "Subiekt EPP - zakładnie kartotek"; }
        }
    }
}