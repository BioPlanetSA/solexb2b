using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolEx.Hurt.Model.Core;
using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Core.Helper
{
    public class SlownikTypow
    {
        public static Dictionary<Type,int> PobierzSlownikTypow()
        {
           return  new Dictionary<Type, int>()
                    {
                        {typeof(int), 0},
                        {typeof(string), 1},
                        {typeof(decimal), 2},
                        {typeof(double), 3},
                        {typeof(bool), 4},
                        {typeof(DateTime), 5},
                        {typeof(WartoscLiczbowaZaokraglana),7},
                        {typeof(decimal?), 8},
                        {typeof(int?), 9},
                        {typeof(double?), 10},
                        {typeof(DateTime?), 11},
                        {typeof(AccesLevel),12},
                        {typeof(TypProduktu),13},
                        {typeof(WartoscLiczbowa),14}
                    };
        }
    }
}
