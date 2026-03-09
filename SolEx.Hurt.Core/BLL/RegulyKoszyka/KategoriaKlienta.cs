using System;
using ServiceStack.Common;
using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.AtrybutyKlas;
using System.Collections.Generic;
using System.Linq;

namespace SolEx.Hurt.Core.BLL.RegulyKoszyka
{
    [FriendlyName("Kategoria klienta", FriendlyOpis = "Reguła należności klienta do kategorii klientow o wybranym id")]
    public class KategoriaKlienta : RegulaKoszyka, IRegulaCalegoKoszyka, IRegulaPozycji
    {
        public IKlienciDostep klienciDostep = SolexBllCalosc.PobierzInstancje.Klienci;

        public KategoriaKlienta()
        {
            if ((int)NaleznoscKlienta == 0)
            {
                NaleznoscKlienta = Naleznosc.NalezyDoWszystkich;
            }
            KategoriaID = new List<string>();
        }

        [FriendlyName("Kategorii klientow")]
        [PobieranieSlownika(typeof(SlownikKategoriiKlienta))]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public List<string> KategoriaID { get; set; }

        [FriendlyName("Należność klienta do kategorii")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public Naleznosc NaleznoscKlienta { get; set; }

        public List<int> KategorieID => KategoriaID?.Select(int.Parse).ToList();

        public bool KoszykSpelniaRegule(IKoszykiBLL koszyk)
        {
            if (KategoriaID == null || !KategoriaID.Any())
            {
                Log.Error("Brak podanych kategorii klientów");
                return false;
            }
            return Regula(koszyk);
        }

        public bool PozycjaSpelniaRegule(IKoszykPozycja pozycja, IKoszykiBLL koszyk)
        {
            return Regula(koszyk);
        }

        public bool Regula(IKoszykiBLL koszyk)
        {
            bool zawiera = true;
            if (NaleznoscKlienta == Naleznosc.NalezyDoWszystkich)
            {
                HashSet<int> listIDKategoriKlienta = new HashSet<int>( koszyk.Klient.Kategorie );
                foreach (var kategoria in KategorieID)
                {
                    if (!listIDKategoriKlienta.Contains(kategoria))
                    {
                        zawiera = false;
                        break;
                    }
                }
            }
            else
            {
                zawiera = false;
                foreach (var kategoria in koszyk.Klient.Kategorie)
                {
                    if (KategorieID.Contains(kategoria))
                    {
                        zawiera = true;
                        break;
                    }
                }
            }

            return NaleznoscKlienta == Naleznosc.NieNalezy ? !zawiera : zawiera;
        }

        public enum Naleznosc
        {
            NieNalezy = 1,
            NalezyDoWszystkich = 10,
            NalezyDoKoregokolwiek = 20
        }
    }
}