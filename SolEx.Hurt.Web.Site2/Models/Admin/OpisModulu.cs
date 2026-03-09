using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ServiceStack.Common;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Web.Site2.Models.Admin
{
    public class OpisModulu
    {
        private string _opis;
        private string _nazwa;
        private string _grupa;
        private Type _typ;
        private readonly int? _parent;

        public string Nazwa
        {
            get { return _nazwa; }
        }

        public string Opis
        {
            get { return _opis; }

        }
        public string Grupa
        {
            get { return _grupa; }

        }

        public Type Typ
        {
            get { return _typ; }

        }

        public int? Parent
        {
            get { return _parent; }
        }

        public OpisModulu(Type t, int? parent)
        {
            _typ = t;
            _parent = parent;
            ModulStowrzonyNaPodstawieZadania modul = (ModulStowrzonyNaPodstawieZadania) Activator.CreateInstance(t);
            FriendlyNameAttribute opisy = modul.GetType().GetCustomAttribute<FriendlyNameAttribute>();
            _nazwa = (opisy != null) ? opisy.FriendlyName : modul.Nazwa;
            _opis = (opisy != null) ? opisy.FriendlyOpis : modul.Opis;

            var moduly = modul.JakiejOperacjiSynchronizacjiDotyczy;

            if (moduly != null && moduly.Any())
            {
                HashSet<ElementySynchronizacji> grupa = new HashSet<ElementySynchronizacji>( moduly );
                if (grupa != null && grupa.Any())
                {
                    foreach (ElementySynchronizacji elementySynchronizacji in grupa)
                    {
                        _grupa += elementySynchronizacji + ", ";
                    }
                    _grupa = _grupa.Trim().TrimEnd(",");
                }
            }
        }
    }
}