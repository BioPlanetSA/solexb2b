namespace SolEx.Hurt.Model.Interfaces.Sync
{
    
    public interface IDrukowanieDokumentowPdf
    {

        bool DrukujPdfDokument(StatusDokumentuPDF status, ref string sciezka);
    }
}
