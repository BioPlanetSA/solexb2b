using System;
using System.Linq;
using System.Text;

namespace SolEx.Hurt.Core.BLL.ZadaniaKoszyka
{
    internal class KopiowanieParametrowIndywidualizacjiDoUwag : ZadanieCalegoKoszyka, IZadaniePoFinalizacji
    {
        public override bool Wykonaj(ModelBLL.Interfejsy.IKoszykiBLL koszyk)
        {
            throw new NotImplementedException();
            //StringBuilder sb = new StringBuilder();
            //foreach (var p in koszyk.PobierzPozycje())
            //{
            //    if (!p.IndywidalneParametry.Any())
            //    {
            //        continue;
            //    }
            //    StringBuilder sbpoz = new StringBuilder();
            //    foreach (var par in p.IndywidalneParametry)
            //    {
            //        sbpoz.AppendFormat("{0} - {1}; ", par.Key, par.Value);
            //    }
            //    sb.AppendFormat("{0} - {1} {2} - {3} ", p.Produkt().Kod, p.Ilosc.ToString("0.##"), p.Jednostka().Nazwa, sbpoz);
            //}
            //koszyk.Uwagi += " " + sb;
            //return true;
        }
    }
}