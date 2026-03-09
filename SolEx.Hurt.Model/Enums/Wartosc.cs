using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SolEx.Hurt.Model.AtrybutyKlas;

namespace SolEx.Hurt.Model.Enums
{
    public enum Wartosc
    {
        [FriendlyName("Równe/=")]
        Rowne,
        [FriendlyName("Większe/<")]
        Wieksze,
        [FriendlyName("Mniejsze/>")]
        Mniejsze,
        [FriendlyName("Różne/!=")]
        Rozne,
        [FriendlyName("Dowolna")]
        Dowolna,
        [FriendlyName("Większe lub równe/<=")]
        WiekszeRowne,
        [FriendlyName("Mniejszelub równe/>=")]
        MniejszeRowne,

    }
}
