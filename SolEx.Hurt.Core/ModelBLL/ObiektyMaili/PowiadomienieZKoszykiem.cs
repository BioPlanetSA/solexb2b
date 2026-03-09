using System.Collections.Generic;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core.ModelBLL.ObiektyMaili
{
    public abstract class PowiadomienieZKoszykiem : SzablonMailaBaza
    {
        protected PowiadomienieZKoszykiem(IKoszykiBLL koszyk, IKlient klient) : base(klient)
        {
            Koszyk = koszyk;
        }

        public IKoszykiBLL Koszyk { get; set; }

        public PowiadomienieZKoszykiem() { }

        public IKlient KlientKtoryOdrzucil { get; set; }

        public List<IKlient> KlienciMogacyAkceptowac { get; set; }
        public override string NazwaSzablonu()
        {
            return "SubkontaKoszyk";
        }
    }
}