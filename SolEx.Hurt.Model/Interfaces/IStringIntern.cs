using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace SolEx.Hurt.Model.Interfaces
{
    /// <summary>
    /// interfejs oznaczajacy ze w danej klasie sa pola stringowe ktore nalezy Internowac
    /// </summary>
    public interface IStringIntern
    {
    }

    /// <summary>
    /// atrybut nakazuje internowac string w tej zmiennej - klasa musi miec interfejs IStringIntern aby proces internacji zaszedl przy dodawaniu do cache
    /// </summary>
    public class StringInternuj : Attribute
    {

    }
}
