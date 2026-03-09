using System.Collections.Generic;
using System.Linq;
using SolEx.Hurt.Model;

namespace SolEx.Hurt.Core.Sync
{
    public class SubiektBaza : BLL.BllBaza<SubiektBaza>
    {
        public Cecha StworzCecheZGrupy(out Atrybut atrybut, int grupyoffset, KeyValuePair<int,string>grupa,string atrybutZkategorii,string cechaAuto,char[] separatorUstawienie, bool atrybutZCechy)
        {
            Cecha item = new Cecha();
            item.Id = grupyoffset + grupa.Key;

            char separator = grupa.Value.Contains("_") ? '_' : ':';
            string[] obiekty = grupa.Value.Replace("/", "\\").Split(separator);
            item.Nazwa = obiekty.Last();
            string symbol = ((obiekty.Length == 1 ? atrybutZkategorii : obiekty.First()) + separator + obiekty.Last()).ToLower();
            string nazwaatrybutu;
            if (obiekty.Length == 1)
            {
                nazwaatrybutu = atrybutZkategorii + separator + item.Nazwa;
            }
            else nazwaatrybutu = grupa.Value;

            Atrybut tmp = atrybutZCechy ? AtrybutyWyszukiwanie.PobierzInstancje.WyciagnijAtrybutZCechy(ref nazwaatrybutu, ref symbol,separatorUstawienie, cechaAuto): null;
            item.Symbol = symbol;
            atrybut = tmp;

            return item;
        }
    }
}
