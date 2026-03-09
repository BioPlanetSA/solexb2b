using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.AtrybutyKlas;
using System;

namespace SolEx.Hurt.Core.BLL.RegulyKoszyka
{
    [FriendlyName("Aktualna data", FriendlyOpis = "Czy aktualna data jest z przedziału")]
    public class AktualnaData : RegulaKoszyka, IRegulaCalegoKoszyka, IRegulaPozycji
    {
        [FriendlyName("Data początkowa")]
        [Niewymagane]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public DateTime? PoczątekPrzedzialu { get; set; }

        [FriendlyName("Data końcowa")]
        [Niewymagane]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public DateTime? KoniecPrzedzialu { get; set; }

        public bool KoszykSpelniaRegule(IKoszykiBLL koszyk)
        {
            return SprawdzDate();
        }

        public bool PozycjaSpelniaRegule(IKoszykPozycja pozycja, IKoszykiBLL koszyk)
        {
            return SprawdzDate();
        }

        private bool SprawdzDate()
        {
            return (!PoczątekPrzedzialu.HasValue || PoczątekPrzedzialu.Value <= DateTime.Now) && (!KoniecPrzedzialu.HasValue || KoniecPrzedzialu.Value >= DateTime.Now);
        }
    }
}