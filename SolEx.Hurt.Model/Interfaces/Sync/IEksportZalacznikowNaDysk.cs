using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Model.Interfaces.Sync
{
    public interface IEksportZalacznikowNaDysk
    {
        void ZapiszZalacznikiNaDysk(string sciezka, string separator, TypyPolDoDopasowaniaZdjecia polaZapisuZalacznika);
    }
}
