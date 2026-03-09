using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SolEx.Hurt.Model;
using SolEx.Hurt.Sync.Core;
using System.Data.SqlClient;
using SolEx.Hurt.Helpers;

namespace SolEx.Hurt.Sync.App.Modules_.Provider
{
    public class VariantListyPrzewozoweProvider : IImportDataModule
    {
        #region IImportDataModule Members

        public void DoWork(System.Collections.Specialized.NameValueCollection configuration, SourceDB db)
        {
            string SQL = @" 
select fs,listprzew from(
SELECT Sprzed.Opiekun, Sprzed.Akronim, Sprzed.Mag, Sprzed.Zam, Sprzed.TermReal, Sprzed.Wyst, Sprzed.DataZam, Sprzed.GodzPotw, Sprzed.Wz, Sprzed.GodzWz AS DataGodzWz, 
	CAST(Sprzed.GodzWZ-Sprzed.DataZam as float) AS Zam_WZ, ISNULL(Sprzed.Qgu,'NIE') AS Qgu, ' ' AS [*], Sprzed.TrN_SposobDostawy AS SposobWysylki, 
	Sprzed.OpisWysylki, Sprzed.FS, Sprzed.Kwota, Sprzed.DataGot, ' ' AS [+], 
	CASE WHEN SUBSTRING(Sprzed.OpisWysylki,1+ISNULL(CHARINDEX('<',Sprzed.OpisWysylki),0),1)='N' 
		THEN SUBSTRING(Sprzed.OpisWysylki,2+ISNULL(CHARINDEX('<',Sprzed.OpisWysylki),0),10) 
		ELSE SUBSTRING(Sprzed.OpisWysylki,1+ISNULL(CHARINDEX('<',Sprzed.OpisWysylki),0), ABS(CHARINDEX('>',Sprzed.OpisWysylki)-1-CHARINDEX('<',Sprzed.OpisWysylki)))
	END AS ListPrzew,
	DataNadania, Odbierajacy, DataOdbioru, CAST(DataOdbioru - Sprzed.DataZam  as float) AS CzasReal

FROM
(
SELECT MAG_Kod AS Mag, 'ZS ' + LTRIM(STR(ZaN_ZamNumer,10)) + '/' + RIGHT(ZaN_ZamRok,2) + CASE WHEN ZaN_ZamSeria='' THEN '' 
					ELSE '/' + ZaN_ZamSeria END AS Zam, 
	Prc_Nazwisko + ', ' + Prc_Imie1 AS Opiekun, Knt_Akronim AS Akronim, Ope_Nazwisko AS Wyst,
	DATEADD(ms,10*ZaN_GodzinaWystawienia, CONVERT(DateTime,ISNULL(ZaN_DataWystawienia,TraNag.TrN_Data2)-36163)) AS DataZam,
	DATEADD(ms,10*ZaN_GodzinaPotwierdzenia, '00:00') AS GodzPotw,
	CONVERT(DateTime,ZaN_DataRealizacji-36163) AS TermReal,
	Tranag.TrN_GIDNumer, TraNag.TrN_DokumentObcy AS Wz, CONVERT(DateTime, TraNag.TrN_Data2-36163) AS DataWZ,
	DATEADD(ms,10*TraNag.TrN_GodzinaWystawienia, CONVERT(DateTime, TraNag.TrN_Data2-36163)) AS GodzWZ, TraNag.TrN_RokMiesiac AS Mies,
	Atr_Wartosc AS Qgu, TraNag.TrN_SposobDostawy, LEFT(REPLACE(REPLACE(TnO_Opis,CHAR(10),''), CHAR(13),''),38) AS OpisWysylki,
	SpiNag.TrN_DokumentObcy AS FS, CONVERT(DateTime,SpiNag.TrN_Data3-36163) AS DataFS, 
	CASE ISDATE(ISNULL(SUBSTRING(TnO_Opis,1,4)+'-'+SUBSTRING(TnO_Opis,6,2)+'-'+SUBSTRING(TnO_Opis,9,8),'TT')) 
		WHEN 0 THEN CASE WHEN SpiNag.TrN_Stan >2 
					THEN DATEADD(ss, SpiNag.TrN_LastMod,CONVERT(DATETIME,'19900101',11))
					ELSE NULL END
		ELSE CONVERT(DateTime,SUBSTRING(TnO_Opis,1,4)+'-'+SUBSTRING(TnO_Opis,6,2)+'-'+SUBSTRING(TnO_Opis,9,8),20) 
	END AS DataGot,
	SUM(TrS_KosztKsiegowy) AS Kwota
FROM CDN.TraNag AS TraNag 
	INNER JOIN CDN.TraElem ON TraNag.TrN_GIDTyp = TrE_GIDTyp AND TraNag.TrN_GIDNumer = TrE_GIDNumer
	INNER JOIN CDN.TraSElem ON TrE_GIDLp = TrS_GIDLp AND TrE_GIDNumer = TrS_GIDNumer AND TrE_GIDTyp = TrS_GIDTyp
	INNER JOIN CDN.TwrKarty ON Twr_GIDNumer=TrE_TwrNumer
	INNER JOIN CDN.KntKarty ON Knt_GIDNumer=TrN_KntNumer
	LEFT JOIN CDN.KntOpiekun ON Knt_GIDTyp = KtO_KntTyp AND KtO_KntNumer = Knt_GIDNumer 
	LEFT JOIN CDN.PrcKarty ON KtO_PrcNumer = Prc_GIDNumer
	LEFT JOIN CDN.Rezerwacje ON TrS_RezTyp=Rez_GIDTyp AND TrS_RezNumer=Rez_GIDNumer AND TrS_RezLp=Rez_GIDLp
	LEFT JOIN CDN.ZamNag ON ZaN_GIDNumer = (
			CASE WHEN TraNag.TrN_ZaNNumer=0 
				THEN 	CASE TrS_RezTyp WHEN 2576 THEN Rez_ZrdNumer 
							WHEN 960 THEN TrS_RezNumer 
					END 
				ELSE TrN_ZaNNumer 
			END)
	LEFT JOIN CDN.TraNag AS SpiNag ON TraNag.TrN_SpiNumer = SpiNag.TrN_GIDNumer AND TraNag.TrN_SpiTyp = SpiNag.TrN_GIDTyp
	LEFT JOIN CDN.TrNOpisy ON SpiNag.TrN_GIDNumer = TnO_TrnNumer AND SpiNag.TrN_GIDTyp = TnO_TrnTyp
	LEFT JOIN CDN.OpeKarty ON ZaN_OpeNumerW=Ope_GIDNumer
	LEFT JOIN CDN.Atrybuty ON TraNag.TrN_GIDTyp=Atr_ObiTyp AND TraNag.TrN_GIDNumer=Atr_ObiNumer AND Atr_AtkId=8
	INNER JOIN CDN.Magazyny ON MAG_GIDNumer = TraNag.TrN_MagZNumer

WHERE CONVERT(DateTime,TraNag.TrN_Data3-36163)>=@OdDnia AND TraNag.TrN_GIDTyp IN(2001,2033) AND Twr_Typ IN(1,2)
	AND TraNag.TrN_TrNSeria<>'E'  AND TraNag.TrN_TrNSeria<>'U'
GROUP BY MAG_Kod, ZaN_ZamNumer, ZaN_ZamRok, ZaN_ZamSeria, ZaN_DataWystawienia, ZaN_GodzinaWystawienia, ZaN_GodzinaPotwierdzenia, ZaN_DataRealizacji,
	Knt_Akronim, Prc_Nazwisko, Prc_Imie1, Atr_Wartosc, TraNag.Trn_GIDNumer, TraNag.TrN_DokumentObcy, TraNag.TrN_Data2, TraNag.TrN_GodzinaWystawienia, TraNag.TrN_SposobDostawy, TraNag.TrN_RokMiesiac,
	SpiNag.TrN_DokumentObcy, SpiNag.TrN_Data3, SpiNag.TrN_Stan, SpiNag.TrN_LastMod, 
	TnO_Opis, Ope_Nazwisko
) AS Sprzed
LEFT JOIN DODATKI.dbo.RaportSCHENKER 
	ON Numer=	CASE WHEN SUBSTRING(Sprzed.OpisWysylki,1+ISNULL(CHARINDEX('<',Sprzed.OpisWysylki),0),1)='N' 
				THEN SUBSTRING(Sprzed.OpisWysylki,2+ISNULL(CHARINDEX('<',Sprzed.OpisWysylki),0),10) 
				ELSE SUBSTRING(Sprzed.OpisWysylki,1+ISNULL(CHARINDEX('<',Sprzed.OpisWysylki),0), ABS(CHARINDEX('>',Sprzed.OpisWysylki)-1-CHARINDEX('<',Sprzed.OpisWysylki)))
			END ) x where isnull(listprzew,'')<>'' and len(listprzew)=10


";
            DateTime start;
            if (!DateTime.TryParse(configuration["program_import_start_date"], out start))
            {
                start = CoreManager.GetDocumentsStartDate( null);
            }

            if (this.ProgresChanged != null)
            {
                ProgresChanged(this, new ProgressChangedEventArgs("Pobieranie listów przewozowych"));
            }

            SqlConnection conn = null;
            SqlCommand cmd = null;
            SqlDataReader rd = null;
            try
            {

                conn = new SqlConnection(configuration["erp_cs"]);
                conn.Open();

                List<KeyValuePair<string, string>> data = new List<KeyValuePair<string, string>>(10000);
        

                cmd = new SqlCommand(string.Format(SQL), conn);
                cmd.Parameters.AddWithValue("@OdDnia", start);
                rd = cmd.ExecuteReader();
                while (rd.Read())
                {
                    KeyValuePair<string, string> tmp = new KeyValuePair<string, string>(DataHelper.dbs("fs", rd), DataHelper.dbs("listprzew", rd));
                    data.Add(tmp);
                }
                if (this.ProgresChanged != null)
                {
                    ProgresChanged(this, new ProgressChangedEventArgs("Liczba listów " + data.Count.ToString()));
                }
                string fileName = db == null ? "csv_dokumenty_listy_mini.csv" : "csv_dokumenty_listy.csv";
                SyncManager.SaveFile(fileName, CreateCSV(data)
                , AppDomain.CurrentDomain.BaseDirectory + (configuration["program_mode"] == "-auto" ? "paczka_temp_auto\\" : "paczka_temp\\"));
            }
            catch (Exception ex)
            {
                if (this.ProgresChanged != null)
                {
                    ProgresChanged(this, new ProgressChangedEventArgs("Błąd " + ex.Message + " " + ex.StackTrace));
                }
            }
            finally
            {

                if (conn != null) { conn.Close(); conn.Dispose(); }
                if (cmd != null) { cmd.Dispose(); }
                if (rd != null) { rd.Dispose(); }
            }
            if (this.ProgresChanged != null)
            {
                ProgresChanged(this, new ProgressChangedEventArgs("Pobieranie listów przewozowych koniec"));
            }
        }

        private string CreateCSV(List<KeyValuePair<string, string>> data)
        {
            StringBuilder sb = new StringBuilder();

            foreach (KeyValuePair<string, string> d in data)
            {
                sb.Append(d.Key);
                sb.Append("|");
                sb.Append(d.Value);
                sb.Append(";");
            }

            return sb.ToString();
        }

        public event ProgressChangedEventHandler ProgresChanged;

        #endregion
    }
}
