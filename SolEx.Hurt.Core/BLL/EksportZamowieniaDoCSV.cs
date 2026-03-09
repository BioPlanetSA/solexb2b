using SolEx.Hurt.Core.BLL.ZadaniaKoszyka;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model.AtrybutyKlas;
using System.IO;

namespace SolEx.Hurt.Core.BLL
{
    public class EksportZamowieniaDoCSV:  ZadanieKoszyka,IZadaniePoFinalizacji
    {
        [FriendlyName("ID szablonu")]
        public int IdSzablonu { get; set; }
        public override bool Wykonaj(KoszykiBLL koszyk)
        {
           string dane= APIWywolania.PobierzEksportowaneDane(IdSzablonu,new List<int>{koszyk.IdZamowienia});
            File.WriteAllText(string.Format("{0}\\zamowienie{1}.txt", AppDomain.CurrentDomain.BaseDirectory, koszyk.IdZamowienia),dane);
            return true;
        }
    }
}
