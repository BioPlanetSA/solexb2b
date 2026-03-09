using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceStack.DataAnnotations;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Core.ModelBLL
{
    [Alias("Klient")]
    public class AdministratorBLL : Klient
    {
        public AdministratorBLL()
        {
        }

        public AdministratorBLL(Klient x) : base(x)
        {
        }
    }
}
