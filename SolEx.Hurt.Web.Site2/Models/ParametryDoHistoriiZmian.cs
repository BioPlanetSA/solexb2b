using System;
using System.Collections.Generic;
using SolEx.Hurt.Core;
using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Model.Core;
using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Web.Site2.Models
{
  public class ParametryDoHistoriiZmian
    {
      public ParametryDoHistoriiZmian( Dictionary<string, List<SolEx.Hurt.Core.ModelBLL.ZmianaObiektu>> zmiany, Type typ, string id)
      {
          Zmiany = zmiany;
          Typ = typ;
          IdObj = id;
      }
    
        public Dictionary<string, List<SolEx.Hurt.Core.ModelBLL.ZmianaObiektu>> Zmiany { get; set; }
        public Type Typ { get; set; }
        public string IdObj { get; set; }
    }
}