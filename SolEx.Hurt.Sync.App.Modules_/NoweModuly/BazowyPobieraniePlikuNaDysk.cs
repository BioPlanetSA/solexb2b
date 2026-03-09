using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolEx.Hurt.Model.AtrybutyKlas;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly
{
    public abstract class BazowyPobieraniePlikuNaDysk: BazowyParsowanieWWW
    {
        [WidoczneListaAdmin(false, false, true, false)]
        public string SciezkaPlikuNaDysku { get; set; }

        public bool PobierzPlik()
        {
            File.Delete(SciezkaPlikuNaDysku);
           var dok = PobierzStrone();
            dok.Save( SciezkaPlikuNaDysku );

            if (File.Exists(SciezkaPlikuNaDysku) && new FileInfo(SciezkaPlikuNaDysku).Length > 10)
            {
                return true;
            }
            return false;
        }
    }
}
