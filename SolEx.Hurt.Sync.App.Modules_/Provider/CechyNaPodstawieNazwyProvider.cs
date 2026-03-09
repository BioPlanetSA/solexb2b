using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using SolEx.Hurt.Model;

namespace SolEx.Hurt.Sync.App.Modules_.Provider
{
    public class CechyNaPodstawieNazwyProvider : IImportDataModule
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        #region IImportDataModule Members
        List<int> threenumbers = new List<int> { 18, 19 /*,16*/, 43, 44,
        59, 5, 17, 14, 8, 6, 25, 41, 3, 30, 31, 35, 42 /*,26*/, 34, 52, 33, 7 };
        List<int> twnumbers = new List<int> { 2, 63 /*, 29 */, 26, 70 };
        List<int> twnumbersOneText = new List<int> { };
        List<int> OneNumberOneText = new List<int> { 4, 54, 27, 38, 39 };
        List<int> OneText = new List<int> { 69, 58, 47, 45, 57, 24, 48, 21 };
        List<int> OneNumber = new List<int> { 20 };
        private int GetActionType(List<int> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (threenumbers.Contains(list[i]))
                    return 1;
                if (twnumbers.Contains(list[i]))
                    return 2;
                if (twnumbersOneText.Contains(list[i]))
                    return 3;
                if (OneNumberOneText.Contains(list[i]))
                    return 4;
                if (OneText.Contains(list[i]))
                    return 5;
                if (OneNumber.Contains(list[i]))
                    return 6;

            }
            return -1;
        }
        public void DoWork(System.Collections.Specialized.NameValueCollection configuration, SourceDB db)
        {
            int i = 0;
            for (; i < db.Products.Count; i++)
            {
                try
                {
                    int actionType = GetActionType(db.Products[i].CategoryIds);
                    string name = db.Products[i].nazwa.ToLower();
                    string v1 = "";
                    string v2 = "";
                    string v3 = "";
                    string a1 = "";
                    string a2 = "";
                    string a3 = "";
                    string a1t = "";
                    string a2t = "";
                    string a3t = "";
                    ;
                    GetAtributes(db.Products[i].CategoryIds, out a1, out a1t, out a2, out a2t, out a3, out a3t);
                    bool result = false;
                    switch (actionType)
                    {
                        case 1:
                            result = GetValues(name, out  v1, out  v2, out  v3);
                            break;
                        case 2:
                            result = GetValues(name, out  v1, out  v2);
                            break;
                        case 3:
                            break;
                        case 4:
                            result = GetValues(name, out  v1, out  v2, "-");
                            break;
                        case 5:
                            result = GetValues(name, out  v1);
                            break;
                        case 6:
                            result = GetValues(name, out  v1);
                            break;
                    }
                    if (result)
                    {
                        if (!string.IsNullOrEmpty(a1))
                        {
                            string val = (a1t == "l") ? GetValues(v1) : v1;
                            if (!string.IsNullOrEmpty(val))
                            {
                                string symbol = GetSymbol(a1, val);
                                if (!db.Products[i].AttributeSymbols.Contains(symbol))
                                {
                                    db.Products[i].AttributeSymbols.Add(symbol);
                                }
                                if (!db.Attributes.Any(p => p.symbol == symbol))
                                {
                                    db.Attributes.Add(GetTrait(a1, symbol, val));
                                }
                            }
                        }
                        if (!string.IsNullOrEmpty(a2))
                        {
                            string val = (a2t == "l") ? GetValues(v2) : v2;
                            if (!string.IsNullOrEmpty(val))
                            {
                                string symbol = GetSymbol(a2, val);
                                if (!db.Products[i].AttributeSymbols.Contains(symbol))
                                {
                                    db.Products[i].AttributeSymbols.Add(symbol);
                                }
                                if (!db.Attributes.Any(p => p.symbol == symbol))
                                {
                                    db.Attributes.Add(GetTrait(a2, symbol, val));
                                }
                            }
                        }
                        if (!string.IsNullOrEmpty(a3))
                        {
                            string val = (a3t == "l") ? GetValues(v3) : v3;
                            if (!string.IsNullOrEmpty(val))
                            {
                                string symbol = GetSymbol(a3, val);
                                if (!db.Products[i].AttributeSymbols.Contains(symbol))
                                {
                                    db.Products[i].AttributeSymbols.Add(symbol);
                                }
                                if (!db.Attributes.Any(p => p.symbol == symbol))
                                {
                                    db.Attributes.Add(GetTrait(a3, symbol, val));
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.Error("Wywalka p=" + db.Products[i].nazwa + "c=" + (db.Products[i].CategoryIds.Count > 0 ? db.Products[i].CategoryIds[0].ToString() : "-1") + "e=" + ex.Message
                    + "st=" + ex.StackTrace);
                }
            }

        }


        private cechy GetTrait(string name, string symbol, string value)
        {
            cechy tmp = new cechy();
            tmp.cecha_id = GetID(name);
            tmp.nazwa = value;
            tmp.symbol = symbol;
            tmp.nazwa = name;
            return tmp;
        }

        private int GetID(string name)
        {
            int sum = 0;
            foreach (char c in name)
            {
                sum += (char)c;
            }
            return sum;
        }
        private string GetSymbol(string atribute, string value)
        {
            return "b2b_" + atribute + "_" + value;
        }
        private string GetValues(string v1)
        {

            string[] split = v1.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
            decimal tmp = 0;
            if (decimal.TryParse(split[0], out tmp))
            {
                return split[0];
            }
            return "";
        }

        private void GetAtributes(List<int> list, out string atribute1, out string atribute1type, out string atribute2, out string atribute2type, out string atribute3, out string atribute3type)
        {
            atribute1 = atribute1type = atribute2 = atribute2type = atribute3 = atribute3type = "";
            if (list.Contains(18))//18	CORTECO	T
            {
                atribute1type = atribute2type = atribute3type = "l";
                atribute1 = "Wew";
                atribute2 = "Zew";
                atribute3 = "Grubość";
            }
            else
                if (list.Contains(19))//19	DT1, DT2, DT3	T
                {
                    atribute1type = atribute2type = atribute3type = "l";
                    atribute1 = "Wew";
                    atribute2 = "Szerokość";
                    atribute3 = "Grubość";
                }
                //else if (list.Contains(16))//16	GIR, GER, OW	T
                //{
                //    atribute1type = atribute2type = atribute3type = "l";
                //    atribute1 = "Wew";
                //    atribute2 = "Zew";
                //    atribute3 = "Grubość";
                //}
                else
                    if (list.Contains(43))//43	K82, GPP	T
                    {
                        atribute1type = atribute2type = atribute3type = "l";
                        atribute1 = "Wew";
                        atribute2 = "Zew";
                        atribute3 = "Grubość";
                    }
                    else
                        if (list.Contains(69))//69	KINEX	T
                        {
                            atribute1type = "s";
                            atribute1 = "Typ";
                        }//        List<int> OneText = new List<int> { 69,58,47,45,57,24,48,21 };
                        else
                            if (list.Contains(44))//44	KLINY, WPUSTY	T
                            {
                                atribute1type = atribute2type = atribute3type = "l";
                                atribute1 = "Szerokość";
                                atribute2 = "Grubość";
                                atribute3 = "Długość";
                            }
                            else
                                if (list.Contains(59))//59	KOŁKI SPR	T
                                {
                                    atribute1type = atribute2type = "l";
                                    atribute1 = "Średnica";
                                    atribute2 = "Długość";
                                }
                                else
                                    if (list.Contains(5))//5	KOMPAKTY TŁOKOWE	T
                                    {
                                        atribute1type = atribute2type = atribute3type = "l";
                                        atribute1 = "Zew";
                                        atribute2 = "Wew";
                                        atribute3 = "Grubość";
                                    }
                                    else
                                        if (list.Contains(58))//69	KINEX	T
                                        {
                                            atribute1type = "s";
                                            atribute1 = "Typ";
                                        }//        List<int> OneText = new List<int> { 69,58,47,45,57,24,48,21 };
                                        else
                                            if (list.Contains(47))//69	KINEX	T
                                            {
                                                atribute1type = "s";
                                                atribute1 = "Typ";
                                            }//        List<int> OneText = new List<int> { 69,58,47,45,57,24,48,21 };
                                            else
                                                if (list.Contains(45))//69	KINEX	T
                                                {
                                                    atribute1type = "s";
                                                    atribute1 = "Typ";
                                                }//        List<int> OneText = new List<int> { 69,58,47,45,57,24,48,21 };
                                                else
                                                    if (list.Contains(57))//69	KINEX	T
                                                    {
                                                        atribute1type = "s";
                                                        atribute1 = "Typ";
                                                    }//        List<int> OneText = new List<int> { 69,58,47,45,57,24,48,21 };
                                                    else
                                                        if (list.Contains(24))//69	KINEX	T
                                                        {
                                                            atribute1type = "s";
                                                            atribute1 = "Typ";
                                                        }//        List<int> OneText = new List<int> { 69,58,47,45,57,24,48,21 };
                                                        else
                                                            if (list.Contains(48))//69	KINEX	T
                                                            {
                                                                atribute1type = "s";
                                                                atribute1 = "Typ";
                                                            }//        List<int> OneText = new List<int> { 69,58,47,45,57,24,48,21 };
                                                            else
                                                                if (list.Contains(21))//69	KINEX	T
                                                                {
                                                                    atribute1type = "s";
                                                                    atribute1 = "Typ";
                                                                }//        List<int> OneText = new List<int> { 69,58,47,45,57,24,48,21 };
                                                                else
                                                                    if (list.Contains(54))//4	SEGER	T
                                                                    {
                                                                        atribute1type = "s";
                                                                        atribute2type = "l";
                                                                        atribute1 = "Typ";
                                                                        atribute2 = "Rozmiar";
                                                                    }
                                                                    else
                                                                        if (list.Contains(2))//2	ORING	T
                                                                        {
                                                                            atribute1type = atribute2type = "l";
                                                                            atribute1 = "Wew";
                                                                            atribute2 = "Grubość";
                                                                        }
                                                                        else
                                                                            if (list.Contains(63))//63	ORINGI FPM	T
                                                                            {
                                                                                atribute1type = atribute2type = "l";
                                                                                atribute1 = "Wew";
                                                                                atribute2 = "Grubość";
                                                                            }
                                                                            else
                                                                                if (list.Contains(27))//4	SEGER	T
                                                                                {
                                                                                    atribute1type = "s";
                                                                                    atribute2type = "l";
                                                                                    atribute1 = "Typ";
                                                                                    atribute2 = "Rozmiar";
                                                                                }

                                                                                else
                                                                                    if (list.Contains(38))//4	SEGER	T
                                                                                    {
                                                                                        atribute1type = "s";
                                                                                        atribute2type = "l";
                                                                                        atribute1 = "Typ";
                                                                                        atribute2 = "Rozmiar";
                                                                                    }
                                                                                    else
                                                                                        if (list.Contains(39))//4	SEGER	T
                                                                                        {
                                                                                            atribute1type = "s";
                                                                                            atribute2type = "l";
                                                                                            atribute1 = "Typ";
                                                                                            atribute2 = "Rozmiar";
                                                                                        }
                                                                                        else
                                                                                            if (list.Contains(17))//17	PIERSCIENIE KASTAS	T
                                                                                            {
                                                                                                atribute1type = atribute2type = atribute3type = "l";
                                                                                                atribute1 = "Zew";
                                                                                                atribute2 = "Wew";
                                                                                                atribute3 = "Ilość";
                                                                                            }
                                                                                            else
                                                                                                if (list.Contains(7))
                                                                                                {
                                                                                                    atribute1type = atribute2type = atribute3type = "l";
                                                                                                    atribute1 = "Wew";
                                                                                                    atribute2 = "Zew";
                                                                                                    atribute3 = "Grubość";
                                                                                                }
                                                                                                else
                                                                                                    if (list.Contains(14))//14	PIERSCIENIE UP, UT	T
                                                                                                    {
                                                                                                        atribute1type = atribute2type = atribute3type = "l";
                                                                                                        atribute1 = "Wew";
                                                                                                        atribute2 = "Zew";
                                                                                                        atribute3 = "Grubość";
                                                                                                    }
                                                                                                    else
                                                                                                        if (list.Contains(8))//8	PIERŚCIENIE U - POLIURETANOWE	T
                                                                                                        {
                                                                                                            atribute1type = atribute2type = atribute3type = "l";
                                                                                                            atribute1 = "Wew";
                                                                                                            atribute2 = "Zew";
                                                                                                            atribute3 = "Grubość";
                                                                                                        }
                                                                                                        else
                                                                                                            if (list.Contains(6))//6	PIERŚCIENIE Z,ZZ	T
                                                                                                            {
                                                                                                                atribute1type = atribute2type = atribute3type = "l";
                                                                                                                atribute1 = "Wew";
                                                                                                                atribute2 = "Zew";
                                                                                                                atribute3 = "Grubość";
                                                                                                            }
                                                                                                            else
                                                                                                                if (list.Contains(25))//25	PODKLADKI	T
                                                                                                                {
                                                                                                                    atribute1type = atribute2type = atribute3type = "l";
                                                                                                                    atribute1 = "Wew";
                                                                                                                    atribute2 = "Zew";
                                                                                                                    atribute3 = "Grubość";
                                                                                                                }
                                                                                                                else
                                                                                                                    if (list.Contains(41))//41	PODKLADKI IMP	T
                                                                                                                    {
                                                                                                                        atribute1 = "Wew";
                                                                                                                        atribute2 = "Zew";
                                                                                                                        atribute3 = "Grubość";
                                                                                                                    }
                                                                                                                    else
                                                                                                                        if (list.Contains(4))//4	SEGER	T
                                                                                                                        {
                                                                                                                            atribute1type = "s";
                                                                                                                            atribute2type = "l";
                                                                                                                            atribute1 = "Typ";
                                                                                                                            atribute2 = "Rozmiar";
                                                                                                                        }
                                                                                                                        else
                                                                                                                            if (list.Contains(3))//3	SIMMERING	T
                                                                                                                            {
                                                                                                                                atribute1type = atribute2type = atribute3type = "l";
                                                                                                                                atribute1 = "Wew";
                                                                                                                                atribute2 = "Zew";
                                                                                                                                atribute3 = "Grubość";
                                                                                                                            }
                                                                                                                            else
                                                                                                                                if (list.Contains(30))//30	SIMMERINGI AGD	T
                                                                                                                                {
                                                                                                                                    atribute1type = atribute2type = atribute3type = "l";
                                                                                                                                    atribute1 = "Wew";
                                                                                                                                    atribute2 = "Zew";
                                                                                                                                    atribute3 = "Grubość";
                                                                                                                                }
                                                                                                                                else
                                                                                                                                    if (list.Contains(31))//31	SIMMERINGI IMPORT	T
                                                                                                                                    {
                                                                                                                                        atribute1 = "Wew";
                                                                                                                                        atribute2 = "Zew";
                                                                                                                                        atribute3 = "Grubość";
                                                                                                                                    }
                                                                                                                                    else
                                                                                                                                        if (list.Contains(35))//35	TTI IMPORT	T
                                                                                                                                        {
                                                                                                                                            atribute1type = atribute2type = atribute3type = "l";
                                                                                                                                            atribute1 = "Wew";
                                                                                                                                            atribute2 = "Zew";
                                                                                                                                            atribute3 = "Grubość";
                                                                                                                                        }
                                                                                                                                        else
                                                                                                                                            if (list.Contains(42))//42	U IMPORT	T
                                                                                                                                            {
                                                                                                                                                atribute1type = atribute2type = atribute3type = "l";
                                                                                                                                                atribute1 = "Wew";
                                                                                                                                                atribute2 = "Zew";
                                                                                                                                                atribute3 = "Grubość";
                                                                                                                                            }
                                                                                                                                            //else if (list.Contains(29))//29	V,VT	T
                                                                                                                                            //{
                                                                                                                                            //    atribute1type = "l";
                                                                                                                                            //    atribute1 = "Zew";
                                                                                                                                            //}
                                                                                                                                            else
                                                                                                                                                if (list.Contains(20))//20	VA, VS	T
                                                                                                                                                {
                                                                                                                                                    atribute1type = "l";
                                                                                                                                                    atribute1 = "Średnica";
                                                                                                                                                }
                                                                                                                                                else
                                                                                                                                                    if (list.Contains(26))//26	X-RING	T
                                                                                                                                                    {
                                                                                                                                                        atribute1type = atribute2type = atribute3type = "l";
                                                                                                                                                        //   atribute1 = "Zew";
                                                                                                                                                        atribute2 = "Wew";
                                                                                                                                                        atribute3 = "Grubość";
                                                                                                                                                    }
                                                                                                                                                    else
                                                                                                                                                        if (list.Contains(34))//34	Z IMPORT	T
                                                                                                                                                        {
                                                                                                                                                            atribute1type = atribute2type = atribute3type = "l";
                                                                                                                                                            atribute1 = "Wew";
                                                                                                                                                            atribute2 = "Zew";
                                                                                                                                                            atribute3 = "Grubość";
                                                                                                                                                        }
                                                                                                                                                        else
                                                                                                                                                            if (list.Contains(52))//52	Z+U	T
                                                                                                                                                            {
                                                                                                                                                                atribute1type = atribute2type = atribute3type = "l";
                                                                                                                                                                atribute1 = "Wew";
                                                                                                                                                                atribute2 = "Zew";
                                                                                                                                                                atribute3 = "Grubość";
                                                                                                                                                            }
                                                                                                                                                            else
                                                                                                                                                                if (list.Contains(33))//33	ZZ IMPORT	T
                                                                                                                                                                {
                                                                                                                                                                    atribute1type = atribute2type = atribute3type = "l";
                                                                                                                                                                    atribute1 = "Wew";
                                                                                                                                                                    atribute2 = "Zew";
                                                                                                                                                                    atribute3 = "Grubość";
                                                                                                                                                                }
                                                                                                                                                                else
                                                                                                                                                                    if (list.Contains(70))
                                                                                                                                                                    {
                                                                                                                                                                        atribute1type = atribute2type =  "l";
                                                                                                                                                                        atribute1 = "Wew";
                                                                                                                                                                        atribute2 = "Grubość";
                                                                                                                                                                    }
        }
        /*27	PASY	?
        38	PASY IMP.	?
        7	PIERSCIENIE U1,U2,UN	Nastapi rozdzielenie na dwie grupy
        9	SZNURY ORINGOWE	? 
        */
        private bool GetValues(string name, out string v1)
        {
            string nameDataModule = TrimData(name);
            if (!string.IsNullOrEmpty(nameDataModule))
            {
                v1 = nameDataModule.Trim();
                return true;
            }
            else
            {
                v1 = null;
                return false;
            }
        }
        private bool GetValues(string name, out string v1, out string v2, string split)
        {
            string nameDataModule = (split != "-") ? TrimData(name) : TrimData2(name);

            string[] data = nameDataModule.Split(new string[] { split }, StringSplitOptions.RemoveEmptyEntries);
            if (data.Length != 2)
            {
                v1 = v2 = null;
                return false;
            }
            else
            {
                v1 = data[0].Trim();
                v2 = data[1].Trim();
                return true;
            }
        }

        private bool GetValues(string name, out string v1, out string v2)
        {
            return GetValues(name, out v1, out v2, "x");
        }

        private bool GetValues(string name, out string v1, out string v2, out string v3)
        {
            string nameDataModule = TrimData(name);
            string[] data = nameDataModule.Split(new string[] { "x" }, StringSplitOptions.RemoveEmptyEntries);
            if (data.Length != 3)
            {
                v1 = v2 = v3 = null;
                return false;
            }
            else
            {
                v1 = data[0].Trim();
                v2 = data[1].Trim();
                v3 = data[2].Trim();
                return true;
            }
        }

        private string TrimData(string name)
        {
            Regex RX = new Regex("[a-wyzA-WYZ]");
            name = RX.Replace(name, " ");
            int first = name.IndexOf("  ");

            int last = name.LastIndexOf("    ");
            if (first > -1)
            {
                if (first < last && last - 3 > first)
                {
                    int len = last - first;
                    return name.Substring(first, len);
                }
                return name.Substring(first);
            }
            return "";
        }
        private string TrimData2(string name)
        {
            int first = name.IndexOf(" ");
            first = name.IndexOf(" ", first + 1);
            int last = name.LastIndexOf("    ");
            if (first > -1)
            {
                if (first < last && last - 3 > first)
                {
                    int len = last - first;
                    return name.Substring(first, len);
                }
                return name.Substring(first);
            }
            return "";
        }
        public event ProgressChangedEventHandler ProgresChanged;

        #endregion
    }
}
