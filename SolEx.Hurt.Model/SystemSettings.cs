using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Core;

namespace SolEx.Hurt.Model
{
    [FriendlyClassName("Ustawienia")]
    public class SystemSettings
    {
        [FriendlyName("Nazwa systemu")]
        public string SystemName { get; set; }

        [FriendlyName("Adres internetowy systemu")]
        public string AdresHttp{ get; set; }
    }
}
