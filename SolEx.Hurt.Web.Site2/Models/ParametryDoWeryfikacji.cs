using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Web.Site2.Models
{
  public class ParametryDoWeryfikacji
    {
      public ParametryDoWeryfikacji(string klucz,decimal kwota, int? numerPlatnosci,StatusPlatnosci status, string token="", string error="")
      {
          Kwota = kwota;
          NumerPlatnosci = numerPlatnosci;
          Error = error;
          Status = status;
          Token = token;
          KluczDokumentu = klucz;
      }

      public decimal Kwota { get; set; }
      public int? NumerPlatnosci { get; set; }
      public string Error { get; set; }
      public StatusPlatnosci Status { get; set; }
      public string KluczDokumentu { get; set; }
      public string Token { get; set; }

  
  }
}