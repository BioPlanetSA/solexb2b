using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using SolEx.Hurt.Model.AtrybutyKlas;

namespace SolEx.Hurt.Model.Enums
{
    public enum SubkontaRodzajAdministratora
    {
        [FriendlyName("Brak uprawnień")] Brak,
        [FriendlyName("Głowny administrator")] Glowny,
        [FriendlyName("Administrator oddziałowy")] Oddzialowy
    }
}
