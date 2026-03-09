using ServiceStack.DesignPatterns.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using ServiceStack.DataAnnotations;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Model
{
    /// <summary>
    /// Model jêzyka
    /// </summary>
    public class Jezyk :IHasIntId , IPolaIDentyfikujaceRecznieDodanyObiekt, IPoleJezyk
    {
        [PrimaryKey]
        [AutoIncrement]
        
        [WidoczneListaAdmin(true, true, false, false)]
        public virtual int Id { get; set; }

        [FriendlyName("Nazwa")]
        [Lokalizowane]
        [WidoczneListaAdmin(true, true, true, true)]
        public string Nazwa { get; set; }

        [FriendlyName("Domyœlny")]
        [WidoczneListaAdmin(true, true, true, false)]
        public bool Domyslny { get; set; }

        [FriendlyName("Symbol")]
        [WidoczneListaAdmin(true, true, true, true)]
        public string Symbol { get; set; }

        [FriendlyName("Kultura", FriendlyOpis = "Sposób pokazywania znaków - np. kropka, przecinek itp.")]
        [WidoczneListaAdmin(false, false, true, true)]
        [PobieranieSlownika("SolEx.Hurt.Core.Helper.SlownikKultur,SolEx.Hurt.Core")]
        public string Kultura { get; set; }

        [FriendlyName("Obrazek")]
        [WidoczneListaAdmin(false, false, true, false)]
        [WymuszonyTypEdytora(TypEdytora.PoleZdjecie)]
        public int? ObrazekId { get; set; }

        [FriendlyName("Domyœlny dla brakuj¹cych t³umaczeñ:")]
        [WidoczneListaAdmin(true,false , true, false)]
        public bool DomyslnyDlaTlumaczen { get; set; }

        [Ignore]
        [WidoczneListaAdmin(true, true, false, false)]
        public string SymbolLokalizacji
        {
            get { return "Jezyk" + Id; }
        }

        [FriendlyName("Ukryty dla klienta:")]
        [WidoczneListaAdmin(false, false, true, false)]
        public bool UkrytyDlaKlienta { get; set; }

        public bool RecznieDodany()
        {
            return true;
        }
        [Ignore]
        public int JezykId { get; set; }
    }
}
