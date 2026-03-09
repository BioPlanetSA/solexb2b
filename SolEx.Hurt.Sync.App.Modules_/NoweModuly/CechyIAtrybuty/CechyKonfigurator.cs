using System;
using System.Collections.Generic;
using System.Linq;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.CechyIAtrybuty
{

    [FriendlyName("Import cech do konfiguratora", FriendlyOpis = "Import cech do konfiguratora")]
    public class CechyKonfigurator : SyncModul, SolEx.Hurt.Model.Interfaces.SyncModuly.IModulCechyIAtrybuty
    {
        private string _poczatekCechy;
        [FriendlyName("Początek cechy, po której ma tworzyć markę i model")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string PoczatekCechy
        {
            get { return _poczatekCechy; }

            set
            {
                _poczatekCechy = value;

                if (!string.IsNullOrEmpty(_poczatekCechy) && !_poczatekCechy.EndsWith(":"))
                    _poczatekCechy += ":";

            }
        }



        [FriendlyName("Separator oddzielający cechę nadrzędną od podrzędnej")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string Separator { get; set; }

        [FriendlyName("Cecha nadrzędna, do której są przypisane inne cechy np marki")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string CechaNadrzedna { get; set; }

        [FriendlyName("Cecha podrzędna, która jest przypisana do cechy nadrzędnej np model")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string CechaPodrzedna { get; set; }

        public CechyKonfigurator()
        {
            PoczatekCechy = string.Empty;
            Separator = string.Empty;
            CechaPodrzedna = string.Empty;
            CechaNadrzedna = string.Empty;
        }

        public void Przetworz(ref List<Atrybut> atrybuty, ref List<Cecha> cechy, Dictionary<long, Produkt> produktyNaB2B)
        {
            if (string.IsNullOrEmpty(CechaPodrzedna) || string.IsNullOrEmpty(CechaNadrzedna) || string.IsNullOrEmpty(PoczatekCechy))
                return;
            var nadrzedne = cechy.Where(p => p.Symbol.ToLower().StartsWith(CechaNadrzedna.ToLower())).ToList();

            var cechyPlatforma = ApiWywolanie.PobierzCechy().Values;
            var atrybutyplatforma = ApiWywolanie.PobierzAtrybuty().Values;

            foreach (var nadrzedna in nadrzedne)
            {
                string kompatybilnosciMArkiSymbol = PoczatekCechy + nadrzedna.Nazwa+Separator;
                var kompatybilnosci = cechy.Where(p => p.Symbol.ToLower().StartsWith(kompatybilnosciMArkiSymbol.ToLower())).ToList();
                foreach (var kompatybilnosc in kompatybilnosci)
                {
                    int separatorPozycja = kompatybilnosc.Symbol.LastIndexOf(Separator);
                    string nazwaPodrzednej = kompatybilnosc.Symbol.Substring(separatorPozycja + 1);
                    string symbolCechyPodrzednej = CechaPodrzedna + ":" + nazwaPodrzednej;

                    int separatordwukropek = kompatybilnosc.Symbol.IndexOf(':');
                     separatorPozycja = kompatybilnosc.Symbol.IndexOf(Separator);
                    string nazwaNadrzednej = kompatybilnosc.Symbol.Substring(separatordwukropek + 1, (separatorPozycja - separatordwukropek-1));
                    string symbolCechNadrzednej = CechaNadrzedna + ":" + nazwaNadrzednej;

                    var cechaNadrzednaerp = cechy.FirstOrDefault(a => a.Symbol.ToLower() == symbolCechNadrzednej.ToLower());
                    var cechaNadrzednab2b = cechyPlatforma.FirstOrDefault(a => a.Symbol.ToLower() == symbolCechNadrzednej.ToLower());
                    if (cechaNadrzednaerp == null && cechaNadrzednab2b == null)
                    {
                        Atrybut atr =
                            atrybuty.FirstOrDefault(a => a.Nazwa.ToLower().StartsWith(CechaNadrzedna.ToLower()));

                        Atrybut atrb2b =
                            atrybutyplatforma.FirstOrDefault(a => a.Nazwa.ToLower().StartsWith(CechaNadrzedna.ToLower()));


                        if (atr == null && atrb2b == null)
                        {
                            atr = dodajAtrybut(atrybuty, CechaNadrzedna);
                        }

                        if (atr != null)
                        {
                            Cecha c = new Cecha();
                            c.AtrybutId = atr.Id;
                            c.Symbol = symbolCechNadrzednej;
                            c.Nazwa = nazwaNadrzednej;
                            c.Widoczna = true;
                            c.Id = -Math.Abs(c.GetHashCode());
                            cechy.Add(c);
                        }
                    }

                    var podrzedna = cechy.FirstOrDefault(p => p.Symbol.ToLower() == symbolCechyPodrzednej.ToLower());
                    var podrzednab2b = cechyPlatforma.FirstOrDefault(a => a.Symbol.ToLower() == symbolCechyPodrzednej.ToLower());

                    if (podrzedna == null && podrzednab2b == null)
                    {
                        Atrybut atr = atrybuty.FirstOrDefault(a => a.Nazwa.ToLower().StartsWith(CechaPodrzedna.ToLower()));
                        Atrybut atrb2b = atrybutyplatforma.FirstOrDefault(a => a.Nazwa.ToLower().StartsWith(CechaPodrzedna.ToLower()));

                        if (atr == null && atrb2b == null)
                        {
                            atr = dodajAtrybut(atrybuty, CechaPodrzedna);
                        }

                        if (atr != null)
                        {
                            Cecha c = new Cecha();
                            c.AtrybutId = atr.Id;
                            c.Symbol = symbolCechyPodrzednej;
                            c.Nazwa = nazwaPodrzednej;
                            c.Widoczna = true;
                      
                            c.Id = -Math.Abs(c.GetHashCode());
                            cechy.Add(c);
                        }
                    }

                    else if(podrzedna != null && nadrzedna != null)
                    {
                      //  podrzedna.CechaNadrzednaId = nadrzedna.Id;
                    }
                }

            }

            var towary = produktyNaB2B.Values;
            var cechytowary = ApiWywolanie.PobierzCechyProdukty().Values.ToList();

            List<ProduktCecha> polaczeniacech = new List<ProduktCecha>();

            foreach (var towar in towary)
            {
                var cechytowaru1 =
                    cechy.Where(
                        a =>
                        a.Symbol.ToLower().StartsWith(PoczatekCechy.ToLower()))
                         .ToList();


                var cechytowar = cechytowary.Where(b => b.ProduktId == towar.Id).ToList();



                List<Cecha> cechytowaru = new List<Cecha>();

                foreach (var cp in cechytowar)
                {
                    var c = cechytowaru1.FirstOrDefault(a => a.Id == cp.CechaId);
                    if (c != null)
                        cechytowaru.Add(c);
                }

                foreach (var kompatybilnosc in cechytowaru)
                {
                    int separatorPozycja = kompatybilnosc.Symbol.LastIndexOf(Separator);
                    string nazwaPodrzednej = kompatybilnosc.Symbol.Substring(separatorPozycja + 1);
                    string symbolCechyPodrzednej = CechaPodrzedna + ":" + nazwaPodrzednej;

                    var cechaPodrzedna = cechyPlatforma.FirstOrDefault(a => a.Symbol == symbolCechyPodrzednej);
                    if (cechaPodrzedna != null)
                    {
                        dodajPowiazanie(cechytowary, cechaPodrzedna, towar);
                    }
                    int separatordwukropek = kompatybilnosc.Symbol.IndexOf(':');
                    separatorPozycja = kompatybilnosc.Symbol.IndexOf(Separator);
                    string nazwaNadrzednej = kompatybilnosc.Symbol.Substring(separatordwukropek + 1, (separatorPozycja - separatordwukropek-1));
                    string symbolCechNadrzednej = CechaNadrzedna + ":" + nazwaNadrzednej;

                    var cechaNadrzedna = cechyPlatforma.FirstOrDefault(a => a.Symbol == symbolCechNadrzednej);
                        if (cechaNadrzedna != null)
                        {
                            dodajPowiazanie(cechytowary, cechaNadrzedna, towar);
                        }
                    }
                
            }

            if (cechytowary.Count > 0)
            {
                Log.Debug("Połączeń cech do wysłania: " + cechytowary.Count);
                ApiWywolanie.AktualizujCechyProdukty(cechytowary);
            }
        }

        private static Atrybut dodajAtrybut(List<Atrybut> atrybutyDododania, string nazwa)
        {
            Atrybut a = new Atrybut(nazwa);
            a.Id = -Math.Abs(a.Nazwa.GetHashCode());            //TODO: zle !
            atrybutyDododania.Add(a);

            return a;
        }

        private static void dodajPowiazanie(List<ProduktCecha> CechyProduktyNaPlatfromie, Cecha nowaCechaMarki, Produkt p)
        {
            if (
                CechyProduktyNaPlatfromie.FirstOrDefault(
                    a => a.CechaId == nowaCechaMarki.Id && a.ProduktId == p.Id) ==
                null)
            {
                ProduktCecha cp = new ProduktCecha();
                cp.CechaId = nowaCechaMarki.Id;
                cp.ProduktId = p.Id;
                CechyProduktyNaPlatfromie.Add(cp);
            }
        }
    }
}
