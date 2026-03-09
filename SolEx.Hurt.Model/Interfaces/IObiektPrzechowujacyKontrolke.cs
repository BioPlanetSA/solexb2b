using System.Collections.Generic;

namespace SolEx.Hurt.Model.Interfaces
{
    public interface IObiektPrzechowujacyKontrolke
    {
        string TypKontrolki();
        string ParametrySerializowane();
        Dictionary<string, object> ParametryLokalizowane();
        void UstawParametrySerializowane(string parametry);
    }
}
