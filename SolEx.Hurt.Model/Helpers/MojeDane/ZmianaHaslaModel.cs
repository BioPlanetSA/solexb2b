using SolEx.Hurt.Model.CustomSearchCriteria;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Model.Helpers.MojeDane
{
    public class ZmianaHaslaModel 
    {
        public string Haslo { get; set; }
        public string Haslo2 { get; set; }
        public string GUID { get; set; }
        public bool HasloZmienione { get; set; }
        public string komunikatDlaKlienta { get; set; }
    }
}