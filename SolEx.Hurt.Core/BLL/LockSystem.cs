using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces.Modele;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SolEx.Hurt.Core.BLL
{
    //TODO: DO WYWALENIA Calkowicie!!
    public class LockSystem
    {
        //Godzina przechowywana jest w MetaSlowaKluczowe
        //Powód blokady w MetaOpis
        private Tresc _tresc;

        public LockSystem()
        {
            _tresc = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<Tresc>(x => x.Symbol == "blokada-systemu", null);
        }

        public string Message
        {
            get
            {
                if (IsLock)
                {
                    return _tresc.MetaOpis;
                }
                return "";
            }
        }

        private HashSet<int> _ids;

        public HashSet<int> EnabledCategories
        {
            get
            {
                if (_ids == null)
                {
                    _ids = new HashSet<int>();
                    string[] date = (_tresc.MetaSlowaKluczowe ?? "").Replace("<div class=\"cfk_data\">", "").Replace("</div>", "").Split(';');
                    if (date.Length > 2)
                    {
                        string[] ids = date[2].Split(new[] { "^" }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string s in ids)
                        {
                            _ids.Add(int.Parse(s));
                        }
                    }
                }
                return _ids;
            }
        }

        public bool IsLock
        {
            get
            {
                DateTime startLock = DateTime.MinValue;
                DateTime endLock = DateTime.MinValue;
                if (_tresc != null)
                {
                    if (_tresc.Aktywny == false)
                    {
                        return false;
                    }
                    string[] date = (_tresc.MetaSlowaKluczowe ?? "").Replace("<div class=\"cfk_data\">", "").Replace("</div>", "").Split(';');
                    if (date.Length > 0)
                    {
                        if (!DateTime.TryParse(date[0], out startLock))
                        {
                            startLock = DateTime.MinValue;
                        }
                        if (date.Length > 1)
                        {
                            if (!DateTime.TryParse(date[1], out endLock))
                            {
                                endLock = DateTime.MinValue;
                            }
                        }
                    }
                    if (startLock <= DateTime.Now && DateTime.Now <= endLock)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public bool MozeSieZalogowac(Klient c)
        {
            if (!IsLock) return true;
            if (c.Role.Contains(RoleType.Administrator) || c.Role.Contains(RoleType.Przedstawiciel) || c.Role.Contains(RoleType.Pracownik)) return true;
            if (c.Kategorie.Any(x => EnabledCategories.Contains(x))) return true;
            return false;
        }
    }
}