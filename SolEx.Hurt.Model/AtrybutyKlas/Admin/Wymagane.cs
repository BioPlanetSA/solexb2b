using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Model.AtrybutyKlas
{
    public class Wymagane : Attribute
    {
        public ERPProviderzy[] ERPProvider { get; private set; }
   
        public Wymagane(ERPProviderzy provider)
        {
            ERPProvider = new [] {provider};
        }

        public Wymagane(ERPProviderzy[] provider)
        {
            ERPProvider = provider;
        }

        public Wymagane()
        {
            ERPProvider = null;
        }

        public bool WymaganeDlaProvidera(ERPProviderzy provider)
        {
            if (ERPProvider == null || ERPProvider.Contains(provider))
            {
                return true;
            }

            return false;
        }
    }
}
