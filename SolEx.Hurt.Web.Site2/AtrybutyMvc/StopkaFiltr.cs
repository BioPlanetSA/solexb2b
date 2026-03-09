using System.IO;
using System.Text;
using SolEx.Hurt.Core;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Web.Site2.Controllers;

namespace SolEx.Hurt.Web.Site2.AtrybutyMvc
{
    public class StopkaFiltr : MemoryStream
    {
        private readonly Stream _outputStream;
        public StopkaFiltr(Stream outputStream, int jezyk)
        {
            _outputStream = outputStream;
            JezykId = jezyk;
        }
        public int JezykId { get; set; }
        public override void Write(byte[] buffer, int offset, int count)
        {

            string tresc=  Encoding.UTF8.GetString(buffer);
            
            string autor = string.Format("<meta name=\"author\" content=\"{0}\">", SolexBllCalosc.PobierzInstancje.TresciDostep.PobierzAutora(JezykId));
            string stopka = string.Format("<div class=\"stopka-solex\">{0}</div> ", SolexBllCalosc.PobierzInstancje.TresciDostep.PobierzStopke(JezykId));
            tresc = tresc.Replace("</body>", stopka + "</body>");
            tresc = tresc.Replace("</head>", autor + "</head>");
            var tmp = Encoding.UTF8.GetBytes(tresc);
            _outputStream.Write(tmp, 0, tmp.Length);
        }
    }

}