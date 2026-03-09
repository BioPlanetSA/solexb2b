using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using ServiceStack.Common;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;
using SolEx.Hurt.Sync.App.Modules_.Helpers;

using System.Runtime.InteropServices;
using System.IO;
using System.Globalization;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Management;
using SolEx.Hurt.Helpers;
using InsERT;
using SolEx.ERP.SubiektGT;
using SolEx.Hurt.Model.Helpers;
using log4net;
using SolEx.Hurt.Core.Sync;
using Adres = SolEx.Hurt.Model.Adres;
using Produkt = SolEx.ERP.Model.Produkt;
using ServiceStack.Text;
using Waluta = SolEx.Hurt.Model.Waluta;

namespace SolEx.Hurt.Sync.ProviderSubiekt
{
    public class MainDAO
    {
        private static ILog log => LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private const int grupyoffset = 100000;
        private int dostawcy_offset = 10000000;
        private int krajPochodzeniaPrzesuniecie = 10000;
        private int kategorie_cechy_offset = 1000000;
        private int platnosci_offset = 100000;
        protected IConfigSynchro _config;
        private IAPIWywolania ApiWywolanie = APIWywolania.PobierzInstancje;


        private int rabat_offset_KategorieKlienta = 10000;

        private int rabat_offset_CechyProduktu = 100000000;


        public MainDAO(IConfigSynchro config)
        {
            _config = config;
        }

        private const string SELECT_HEADER = @" select a.*,status=case when b.flg_Id is null then dok_KatId+{1} else b.flg_Id end from (
                      select 
                      r.dok_KatId,
                      r.dok_OdbiorcaId,
                      r.dok_NrPelnyOryg,
                      r.dok_Id,
                      r.dok_DataWyst,
                      r.dok_NrPelny,
                      typ=2,
                      dokument_powiazany= dp.dok_NrPelny,
                      r.dok_DoDokId,
                      dok_Status= case when r.dok_Status=8 then 1 else 0 end,
                      r.dok_PlatnikId,
                      r.dok_Waluta,
                      dok_WalutaKurs =ISNULL( r.dok_WalutaKurs ,1),
                       dok_PlatTermin=r.dok_TerminRealizacji,
                      wartosc_netto =  CAST(case when (r.dok_WalutaKurs <> 1 and r.dok_WalutaKurs <> 0) then r.dok_WartNetto / r.dok_WalutaKurs else  r.dok_WartNetto end as decimal(15,2)),
                      wartosc_brutto =  CAST(case when (r.dok_WalutaKurs <> 1 and r.dok_WalutaKurs <> 0) then r.dok_WartBrutto / r.dok_WalutaKurs else  r.dok_WartBrutto end as decimal(15,2)),
                      nzf_WartoscWaluta= CAST(case when (r.dok_WalutaKurs <> 1 and r.dok_WalutaKurs <> 0) then r.dok_WartNetto / r.dok_WalutaKurs else  r.dok_WartNetto end as decimal(15,2)),
                      r.dok_Uwagi,
                       platnosc ='',
                      rezerwacja = r.dok_JestRuchMag
   ,opoznienie=0
 from dok__Dokument r left join dok__Dokument dp on r.dok_DoDokId=dp.dok_Id where r.dok_Typ in(16,15) and r.dok_PlatnikId is not null
 and (r.dok_Status = 8 or r.dok_Status = 7 or r.dok_Status = 6) and r.dok_DataWyst>=@from and r.dok_DataWyst<=GETDATE() and  r.dok_PlatnikId in ( {0} ) {2}
union
 select
r.dok_KatId,
r.dok_OdbiorcaId,
r.dok_NrPelnyOryg,
                    r.dok_Id,
                      r.dok_DataWyst,
                      r.dok_NrPelny,
                      typ=1,
                      dokument_powiazany = dp.dok_NrPelny,
                      r.dok_DoDokId,
                      dok_Status=0,
                      r.dok_PlatnikId,
                      r.dok_Waluta,
                      dok_WalutaKurs = isnull(r.dok_WalutaKurs, 1),
                      r.dok_PlatTermin,
                      wartosc_netto =  CAST(case when (r.dok_WalutaKurs <> 1 and r.dok_WalutaKurs <> 0) then r.dok_WartNetto / r.dok_WalutaKurs else  r.dok_WartNetto end as decimal(15,2)),
                      wartosc_brutto =  CAST(case when (r.dok_WalutaKurs <> 1 and r.dok_WalutaKurs <> 0) then r.dok_WartBrutto / r.dok_WalutaKurs else  r.dok_WartBrutto end as decimal(15,2)),
                      p.nzf_WartoscWaluta,
                      r.dok_Uwagi,
                      platnosc = isnull(pl.fp_Nazwa,''),
                      rezerwacja = r.dok_JestRuchMag
   , opoznienie=DATEDIFF (day, r.dok_PlatTermin, GETDATE())
 from vwFinanseRozDokumenty p join dok__Dokument r on p.nzf_IdDokumentAuto=r.dok_Id and p.nzf_IdObiektu=r.dok_PlatnikId
 left join sl_FormaPlatnosci pl on r.dok_PlatId=pl.fp_Id left join dok__Dokument dp on r.dok_DoDokId=dp.dok_Id 
 where (r.dok_Typ = 2 or r.dok_Typ = 6) and r.dok_PlatnikId is not null and r.dok_DataWyst>=@from and r.dok_DataWyst<=GETDATE()  and  r.dok_PlatnikId in ( {0} ) {2}
 
 union
 select      r.dok_KatId, r.dok_OdbiorcaId, r.dok_NrPelnyOryg,  r.dok_Id,
                      r.dok_DataWyst,
                      r.dok_NrPelny,
                      typ=1,
                      dokument_powiazany = dp.dok_NrPelny,
                      r.dok_DoDokId,
                      r.dok_Status,
                      r.dok_PlatnikId,
                      r.dok_Waluta,
                      dok_WalutaKurs = isnull(r.dok_WalutaKurs, 1),
                      r.dok_PlatTermin,
                      wartosc_netto =  CAST(case when (r.dok_WalutaKurs <> 1 and r.dok_WalutaKurs <> 0) then r.dok_WartNetto / r.dok_WalutaKurs else  r.dok_WartNetto end as decimal(15,2)),
                      wartosc_brutto =  CAST(case when (r.dok_WalutaKurs <> 1 and r.dok_WalutaKurs <> 0) then r.dok_WartBrutto / r.dok_WalutaKurs else  r.dok_WartBrutto end as decimal(15,2)),
                      nzf_WartoscWaluta =0,
                      r.dok_Uwagi,
                      platnosc = isnull(pl.fp_Nazwa,''),
                      rezerwacja = r.dok_JestRuchMag
   ,opoznienie=0
from vwFinanseRozRozliczone p 
                      join dok__Dokument r on p.nzf_IdDokumentAuto=r.dok_Id
 left join sl_FormaPlatnosci pl on r.dok_PlatId=pl.fp_Id 
 left join dok__Dokument dp on r.dok_DoDokId=dp.dok_Id
 where (r.dok_Typ = 2 or r.dok_Typ = 6) and r.dok_PlatnikId is not null and r.dok_DataWyst>=@from and r.dok_DataWyst<=GETDATE()  and  r.dok_PlatnikId in ( {0} )  {2}) a 
 left join (SELECT flw_IdObiektu,flg_Text,flg_Id
  FROM fl_Wartosc join fl__Flagi on flg_Id=flw_IdFlagi where flw_IdGrupyFlag in(4,5,8,9)) b on  flw_IdObiektu=dok_id 

  where dok_Id in(select ob_DokHanId from dok_Pozycja)  order by dok_id desc,dok_status desc
";

        const string SELECT_ITEMS = @"
select * from (select      brutto_przed =  p.ob_CenaBrutto /(case when d.dok_WalutaKurs <> 0 then d.dok_WalutaKurs else 1 end)* p.ob_Znak,
            netto_przed = p.ob_CenaNetto /(case when d.dok_WalutaKurs <> 0 then d.dok_WalutaKurs else 1 end)* p.ob_Znak,
            p.ob_DokHanId,
            netto_przed_rabatem_powiazany = d2.ob_CenaNetto/(case when dd.dok_WalutaKurs <> 0 then d.dok_WalutaKurs else 1 end),
            brutto_przed_rabatem_powiazany = d2.ob_CenaBrutto /(case when d.dok_WalutaKurs <> 0 then d.dok_WalutaKurs else 1 end),
            p.ob_Id,
            ilosc_powiazany = d2.ob_Ilosc,
            ilosc=p.ob_Ilosc * p.ob_Znak,
            p.ob_Jm,
            p.ob_VatProc,
            wartosc_brutto =  (p.ob_WartBrutto /(case when d.dok_WalutaKurs <> 0 then d.dok_WalutaKurs else 1 end))* p.ob_Znak,
            wartosc_netto =   (p.ob_WartNetto /(case when d.dok_WalutaKurs <> 0 then d.dok_WalutaKurs else 1 end))* p.ob_Znak,
            tw_Nazwa = ISNULL(t.tw_Nazwa, p.ob_Opis),
            tw_Symbol=ISNULL(t.tw_Symbol, ''),
            produkt_id = isnull(p.ob_TowId,0),
            rabat = p.ob_Rabat,
			odCenyNetto = d.dok_CenyTyp,
            p.ob_Opis
from dok_Pozycja p left join dok_Pozycja d2 on p.ob_DoId=d2.ob_Id  join dok__Dokument d on p.ob_DokHanId=d.dok_Id left join dok__Dokument dd on d2.ob_DokHanId=dd.dok_Id
left join tw__towar t on p.ob_TowId=t.tw_Id where (d.dok_Typ=16 or d.dok_Typ=2 or d.dok_Typ=15 or d.dok_Typ=6) 
and d.dok_Podtyp<>2 and  d.dok_Id  in ({0})
union
select 
            brutto_przed =(isnull(d2.ob_CenaBrutto,0)+isnull(p.ob_CenaBrutto,0))/(case when d.dok_WalutaKurs <> 0 then d.dok_WalutaKurs else 1 end),
            netto_przed = (isnull(d2.ob_CenaNetto,0)+isnull(p.ob_CenaNetto,0)) /(case when d.dok_WalutaKurs <> 0 then d.dok_WalutaKurs else 1 end),
            p.ob_DokHanId,
            netto_przed_rabatem_powiazany = d2.ob_CenaNetto/(case when dd.dok_WalutaKurs <> 0 then d.dok_WalutaKurs else 1 end),
            brutto_przed_rabatem_powiazany = d2.ob_CenaBrutto /(case when d.dok_WalutaKurs <> 0 then d.dok_WalutaKurs else 1 end),
            p.ob_Id,
            ilosc_powiazany = d2.ob_Ilosc,
            ilosc=isnull(d2.ob_Ilosc,0)+isnull(p.ob_Ilosc,0)*p.ob_Znak,
            p.ob_Jm,
            p.ob_VatProc,
            wartosc_brutto=(isnull(d2.ob_WartBrutto,0)+p.ob_Znak*isnull(p.ob_WartBrutto,0))/ (case when d.dok_WalutaKurs <> 0 then d.dok_WalutaKurs else 1 end) * p.ob_Znak  ,
            wartosc_netto=(isnull(d2.ob_WartNetto,0)+p.ob_Znak*isnull(p.ob_WartNetto,0))/ (case when d.dok_WalutaKurs <> 0 then d.dok_WalutaKurs else 1 end) * p.ob_Znak ,
            tw_Nazwa = ISNULL(t.tw_Nazwa, p.ob_Opis),
            tw_Symbol=ISNULL(t.tw_Symbol, ''),
            produkt_id = isnull(p.ob_TowId,0),
            rabat = p.ob_Rabat,
			odCenyNetto = d.dok_CenyTyp,
            p.ob_Opis
from dok_Pozycja p left join dok_Pozycja d2 on p.ob_DoId=d2.ob_Id join dok__Dokument d on p.ob_DokHanId=d.dok_Id left join dok__Dokument dd on d2.ob_DokHanId=dd.dok_Id
left join tw__towar t on p.ob_TowId=t.tw_Id where (d.dok_Typ=16 or d.dok_Typ=2 or d.dok_Typ=6 or d.dok_Typ=15) 
and d.dok_Podtyp=2 and  d.dok_Id  in ({0}) ) dok where dok.brutto_przed<>0 or dok.netto_przed<>0 or dok.ilosc<>0  or dok.wartosc_netto<>0";
            
        private MainDataContext _db = null;

        private MainDataContext DB
        {
            get
            {
                if (_db == null)
                {
                    _db = new MainDataContext(_config.ERPcs);

                }
                return _db;
            }
        }

        private Subiekt sub;
        const string SQL_PW = @"if not exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[solex_pola_klienta]') or id= object_id(N'[solex_pola_klienta]'))  exec('create view solex_pola_klienta as
Select kh_id=pwd_idobiektu,Pole=wartosc,pwp_nazwa from( 
select pwd_idobiektu,pwd_typobiektu,Wartosc,Pole
from (select  pwd_idobiektu,pwd_typobiektu
            ,pwd_liczba01 =cast(pwd_liczba01 as varchar(max))
            ,pwd_liczba02=cast(pwd_liczba02 as varchar(max))
            ,pwd_liczba03=cast(pwd_liczba03 as varchar(max))
            ,pwd_liczba04=cast(pwd_liczba04 as varchar(max))
            ,pwd_liczba05=cast(pwd_liczba05 as varchar(max))
            ,pwd_liczba06=cast(pwd_liczba06 as varchar(max))
            ,pwd_liczba07=cast(pwd_liczba07 as varchar(max))
            ,pwd_liczba08=cast(pwd_liczba08 as varchar(max))
            ,pwd_liczba09=cast(pwd_liczba09 as varchar(max))
            ,pwd_liczba10=cast(pwd_liczba10  as varchar(max))

            ,pwd_kwota01=cast(pwd_kwota01 as varchar(max))
            ,pwd_kwota02=cast(pwd_kwota02 as varchar(max))
            ,pwd_kwota03=cast(pwd_kwota03 as varchar(max))
            ,pwd_kwota04=cast(pwd_kwota04 as varchar(max))
            ,pwd_kwota05=cast(pwd_kwota05 as varchar(max))
            ,pwd_kwota06=cast(pwd_kwota06 as varchar(max))
            ,pwd_kwota07=cast(pwd_kwota07 as varchar(max))
            ,pwd_kwota08=cast(pwd_kwota08 as varchar(max))
            ,pwd_kwota09=cast(pwd_kwota09 as varchar(max))
            ,pwd_kwota10=cast(pwd_kwota10 as varchar(max))
           
            ,pwd_Tekst01 =cast(pwd_tekst01  as varchar(max))
            ,pwd_Tekst02 =cast(pwd_tekst02 as varchar(max))
            ,pwd_Tekst03=cast(pwd_tekst03 as varchar(max))
            ,pwd_Tekst04=cast(pwd_tekst04 as varchar(max))
            ,pwd_Tekst05=cast(pwd_tekst05 as varchar(max))
            ,pwd_Tekst06=cast(pwd_tekst06 as varchar(max))
            ,pwd_Tekst07=cast(pwd_tekst07 as varchar(max))
            ,pwd_Tekst08=cast(pwd_tekst08 as varchar(max))
            ,pwd_Tekst09=cast(pwd_tekst09 as varchar(max))
            ,pwd_Tekst10=cast(pwd_tekst10 as varchar(max))

			,pwd_Fk01 =cast(pwd_Fk01 as varchar(max))
			,pwd_Fk02=cast(pwd_Fk02 as varchar(max))
			,pwd_Fk03=cast(pwd_Fk03 as varchar(max))
			,pwd_Fk04=cast(pwd_Fk04 as varchar(max))
			,pwd_Fk05=cast(pwd_Fk05 as varchar(max))
			,pwd_Fk06=cast(pwd_Fk06 as varchar(max))
			,pwd_Fk07=cast(pwd_Fk07 as varchar(max))
			,pwd_Fk08=cast(pwd_Fk08 as varchar(max))
			,pwd_Fk09=cast(pwd_Fk09 as varchar(max))
			,pwd_Fk10=cast(pwd_Fk10  as varchar(max))
			
 from pw_dane )  a
unpivot
  (Wartosc for Pole in (pwd_liczba01,pwd_liczba02,pwd_liczba03,pwd_liczba04,pwd_liczba05,pwd_liczba06,pwd_liczba07,pwd_liczba08,pwd_liczba09,pwd_liczba10,
                            pwd_kwota01,pwd_kwota02,pwd_kwota03,pwd_kwota04,pwd_kwota05,pwd_kwota06,pwd_kwota07,pwd_kwota08,pwd_kwota09,pwd_kwota10,
                            pwd_tekst01,pwd_tekst02,pwd_tekst03,pwd_tekst04,pwd_tekst05,pwd_tekst06,pwd_tekst07,pwd_tekst08,pwd_tekst09,pwd_tekst10,
							pwd_Fk01,pwd_Fk02,pwd_Fk03,pwd_Fk04,pwd_Fk05,pwd_Fk06,pwd_Fk07,pwd_Fk08,pwd_Fk09,pwd_Fk10)
   ) as Amount ) a join pw_pole p on a.pole=p.pwp_pole where a.pwd_typobiektu=-12 and p.pwp_typobiektu=-12
   
union
select kh_id,klient.wartosc,Pole=pola.wartosc from 
(
select
      kh_id,
       Wartosc,Pole
from (select kh_id,  kh_Pole1 as khp_Nazwa1
      ,[kh_Pole2] as khp_Nazwa2
      ,[kh_Pole3] as khp_Nazwa3
      ,[kh_Pole4] as khp_Nazwa4
      ,[kh_Pole5] as khp_Nazwa5
      ,[kh_Pole6] as khp_Nazwa6
      ,[kh_Pole7] as khp_Nazwa7
      ,[kh_Pole8] as khp_Nazwa8 from kh__kontrahent) k  
unpivot
  (Wartosc for Pole in (
       [khp_Nazwa1]
      ,[khp_Nazwa2]
      ,[khp_Nazwa3]
      ,[khp_Nazwa4]
      ,[khp_Nazwa5]
      ,[khp_Nazwa6]
      ,[khp_Nazwa7]
      ,[khp_Nazwa8])
   ) as Amount ) klient join 
   (SELECT 
        Wartosc,Pole
  FROM [kh_ParametrG]
  unpivot
  (Wartosc for Pole in ( [khp_Nazwa1]
      ,[khp_Nazwa2]
      ,[khp_Nazwa3]
      ,[khp_Nazwa4]
      ,[khp_Nazwa5]
      ,[khp_Nazwa6]
      ,[khp_Nazwa7]
      ,[khp_Nazwa8])
   ) as Amount) pola on klient.pole=pola.pole where pola.wartosc <> '''' and klient.wartosc<>'''' ');";
        // Rodzaje kontrahentów
        // 0 - dostawca odbiorca
        // 1 - dostawca
        // 2 - odbiorca
        //
        // TODO
        // 1. odbiorcy
        // 2. grupy kontrahentów
        internal Dictionary<long, Klient> PobierzKlientow(out Dictionary<Adres, KlientAdres> adresy)
        {

            const string sqlKlienci = @"select telefonPracownik = 
(select top 1 tel_Numer from tel__Ewid where tel_Podstawowy = 1 and tel_IdAdresu in (
	select adr_Id from adr__Ewid a where a.adr_IdObiektu = pracownik.pk_Id and a.adr_TypAdresu = 12)), 
	imieNazwiskoPracownika = pracownik.pk_Imie + ' ' + pracownik.pk_Nazwisko, 
p.adr_IdWojewodztwo ,kh_www,krajue=a.pa_CzlonekUE,kh_skype,kh_gadugadu,p.adr_IdPanstwo, p.adr_Adres,p.adr_Kod,p.adr_Miejscowosc,p.adr_NazwaPelna,p.adr_NIP, p.adr_Telefon,k.kh_EMail,
k.kh_Id,opiekun=uz.uz_Identyfikator,k.kh_Symbol,
k.kh_Cena,k.kh_IdFormaP,k.kh_PodVATZarejestrowanyWUE   {0},
rabat=isnull(rt_Procent,0),rt_Nazwa,
k.kh_PlatOdroczone,k.kh_MaxWartKred,
adres = af.adr_Adres ,
kod = af.adr_Kod,
panstwo = af.adr_IdPanstwo ,
miasto =af.adr_Miejscowosc,
wojewdztwo2=af.adr_IdWojewodztwo,
telefon2=af.adr_telefon,
adresnazwa2=af.adr_Nazwa, 

p.adr_Nazwa,
waluta = cp.WALUTA,
k.kh_ZgodaEMail,
k.kh_AdresDostawy,k.kh_Zablokowany

 from kh__kontrahent k left join  adr__Ewid af on k.kh_Id = af.adr_IdObiektu and af.adr_TypAdresu = 11  and ( af.adr_Symbol <> '********************')  
 left  join  vwPoziomyCenWaluty  cp on k.kh_Cena = cp.IDENT
 join  adr__Ewid p on k.kh_Id = p.adr_IdObiektu and p.adr_TypAdresu = 1  and ( p.adr_Symbol <> '********************')  
 left join sl_Panstwo a on p.adr_IdPanstwo = a.pa_Id 
 left join pd_Uzytkownik uz on k.kh_IdOpiekun=uz.uz_Id  
 left join kh_Pracownik pracownik on (pracownik.pk_IdKh = k.kh_Id and pracownik.pk_Podstaw = 1) 
 left join sl_Rabat rabat on k.kh_IdRabat=rabat.rt_id  
 where (k.kh_Rodzaj = 0 or k.kh_Rodzaj = 2 or k.kh_Rodzaj = 1)  order by p.adr_nazwa";
            //przełącznik czy ma wyciągnąć tylko kontrahentów z ustawionym hasłem

            int hId = _config.GetPriceLevelHurt;
            string defCurrency = null;

            vwPoziomyCenWaluty domyslnawaluta = DB.vwPoziomyCenWaluties.FirstOrDefault(p => p.IDENT == (hId));
            if (domyslnawaluta != null) defCurrency = domyslnawaluta.WALUTA;

            // parametry
            string sql = @"select isnull(sum(nzf_WartoscWaluta),0) from nz__Finanse where nzf_typ=39 and nzf_Wartosc>0 and nzf_Idobiektu = {0} and nzf_typobiektu = 1";


            adresy = new Dictionary<Adres, KlientAdres>();
            Dictionary<long, Klient> items = new Dictionary<long, Klient>();
            SqlConnection conn = null;
            SqlCommand cmd = null;
            SqlDataReader rd = null;
            Dictionary<int, Dictionary<string, string>> customersFields = PobierzPolaWlasneKlientow();
            log.Debug($"Pola własne klientów : {customersFields.ToJson()}");
            List<string> polaWlasneNazwy = new List<string>();
            foreach (var customersField in customersFields.Values)
            {
                foreach (var filed in customersField.Keys)
                {
                    if (!polaWlasneNazwy.Contains(filed))
                    {
                        polaWlasneNazwy.Add(filed);
                    }
                }
            }
            try
            {
                bool czyJestLokalizacja = DataHelper.CzyKolumnaIstnieje("kh__Kontrahent", "kh_Lokalizacja", _config.ERPcs);

                string klienciLokalizacjaSql = "";

                if (czyJestLokalizacja) klienciLokalizacjaSql = ", kh_Lokalizacja";
                conn = new SqlConnection(_config.ERPcs);
                conn.Open();
                cmd = new SqlCommand(string.Format(sqlKlienci, klienciLokalizacjaSql), conn);
                rd = cmd.ExecuteReader();
                while (rd.Read()) //pobieranie  klientów
                {
                    string adr_Adres = DataHelper.dbs("adr_Adres", rd);
                    string adr_Kod = DataHelper.dbs("adr_Kod", rd);
                    string adr_Miejscowosc = DataHelper.dbs("adr_Miejscowosc", rd);
                    string adr_NazwaPelna = DataHelper.dbs("adr_NazwaPelna", rd);
                    string adr_Nip = DataHelper.dbs("adr_NIP", rd);
                    string adr_Telefon = DataHelper.dbs("adr_Telefon", rd);
                    string kh_EMail = DataHelper.dbs("kh_EMail", rd);
                    int kh_Id = DataHelper.dbi("kh_Id", rd);
                    string opiekun = DataHelper.dbs("opiekun", rd);
                    string kh_Symbol = DataHelper.dbs("kh_Symbol", rd);
                    int? kh_Cena = DataHelper.dbin("kh_Cena", rd);
                    bool kh_PodVatZarejestrowanyWue = DataHelper.dbb("kh_PodVATZarejestrowanyWUE", rd);
                    decimal rabat = DataHelper.dbd("rabat", rd);
                    decimal kh_MaxWartKred = DataHelper.dbd("kh_MaxWartKred", rd);
                    string adres = DataHelper.dbs("adres", rd);
                    string kh_Www = DataHelper.dbs("kh_www", rd);

                    string kh_Lokalizacja = czyJestLokalizacja ? DataHelper.dbs("kh_Lokalizacja", rd) : "";
                    string kod = DataHelper.dbs("kod", rd);
                    int? panstwo = DataHelper.dbin("panstwo", rd);
                    string miasto = DataHelper.dbs("miasto", rd);
                    int? adr_IdWojewodztwo = DataHelper.dbin("adr_IdWojewodztwo", rd);
                    int? wojewdztwo2 = DataHelper.dbin("wojewdztwo2", rd);
                    string adr_Nazwa = DataHelper.dbs("adr_Nazwa", rd);
                    string waluta = DataHelper.dbs("waluta", rd);
                    bool kh_AdresDostawy = DataHelper.dbb("kh_AdresDostawy", rd);
                    bool kh_Zablokowany = DataHelper.dbb("kh_Zablokowany", rd);
                    int adr_IdPanstwo = DataHelper.dbi("adr_IdPanstwo", rd);
                    string rt_Nazwa = DataHelper.dbs("rt_Nazwa", rd);
                    string kh_Skype = DataHelper.dbs("kh_skype", rd);
                    string kh_Gadugadu = DataHelper.dbs("kh_gadugadu", rd);
                    string telefon2 = DataHelper.dbs("telefon2", rd);
                    string adresnazwa2 = DataHelper.dbs("adresnazwa2", rd);
                    bool krajue = DataHelper.dbb("krajue", rd);

                    string telefonPracownika = DataHelper.dbs("telefonPracownik", rd);
                    string imieNazwiskoPracownika = DataHelper.dbs("imieNazwiskoPracownika", rd);


                    Dictionary<string, string> pars;
                    if (!customersFields.TryGetValue(kh_Id, out pars))
                    {
                        pars = new Dictionary<string, string>();

                    }
                    foreach (var pole in polaWlasneNazwy)
                    {
                        if (!pars.ContainsKey(pole))
                        {
                            pars.Add(pole, "");
                        }
                    }

                    pars.Add("kh_MaxWartKred", kh_MaxWartKred.ToString());
                    pars.Add("rt_Nazwa", rt_Nazwa);
                    pars.Add("kh_www", kh_Www);
                    pars.Add("kh_skype", kh_Skype);
                    pars.Add("kh_gadugadu", kh_Gadugadu);
                    if (czyJestLokalizacja) pars.Add("kh_Lokalizacja", kh_Lokalizacja);

                    Klient item = new Klient(kh_Id);

                    item.Aktywny = !kh_Zablokowany;

                    item.Nazwa = (_config.ImieNazwiskoKlienta ? adr_Nazwa : adr_NazwaPelna).Trim().Replace(Environment.NewLine, " ");
                    item.Nip = adr_Nip.Trim();
                    item.Telefon = adr_Telefon;
                    item.Symbol = kh_Symbol.Trim();
                    item.Rabat = rabat;
                    item.LimitKredytu = kh_MaxWartKred;


                    decimal kredyt_Wykorzystano = DB.ExecuteQuery<Decimal>(sql, item.Id).FirstOrDefault();
                    pars.Add("nzf_Wartosc", kredyt_Wykorzystano.ToString());

                    decimal kredytPozostalo = item.LimitKredytu - kredyt_Wykorzystano;
                    pars.Add("kredytPozostalo", kredytPozostalo.ToString());
                    //item.kredyt_wykorzystano = 
                    _config.SynchronizacjaPobierzPoleHasloZrodlowe(item, "", pars);
                    if (_config.EksportTylkoKontZHaslem && string.IsNullOrEmpty(item.HasloZrodlowe))
                    {
                        continue;
                    }

                    _config.SynchronizacjaPobierzDostepneMagazyny(item, "", pars);
                    _config.SynchronizacjaPobierzKredytWykorzystano(item, "nzf_Wartosc", pars);
                    _config.SynchronizacjaPobierzKredytPozostalo(item, "kredytPozostalo", pars);
                    _config.SynchronizacjaPobierzPoleJezyk(item, "", pars);

                    //B2B-1355
                        //_config.SynchronizacjaPobierzPoleGaduGadu(item, "kh_gadugadu", pars); 
                        //_config.SynchronizacjaPobierzPoleSkype(item, "kh_skype", pars);
                    if (!string.IsNullOrEmpty(telefonPracownika))
                    {
                        item.GaduGadu = telefonPracownika;
                    }

                    if (!string.IsNullOrEmpty(imieNazwiskoPracownika))
                    {
                        item.Skype = imieNazwiskoPracownika.Trim();
                    }

                    _config.SynchronizacjaPobierzPoleTekst1(item, "", pars);
                    _config.SynchronizacjaPobierzPoleTekst2(item, "", pars);
                    _config.SynchronizacjaPobierzPoleTekst3(item, "", pars);
                    _config.SynchronizacjaPobierzPoleTekst4(item, "", pars);
                    _config.SynchronizacjaPobierzPoleTekst5(item, "", pars);
                    _config.SynchronizacjaPobierzPoleMagazynDomyslny(item, "", pars);
                    _config.SynchronizacjaPobierzPoleIndywidualnaStawaVat(item, "", pars, kh_PodVatZarejestrowanyWue, krajue);
                    _config.SynchronizacjaPobierzPoleDomyslnaWaluta(item, "", string.IsNullOrEmpty(waluta) ? defCurrency : waluta, pars);

                    _config.SynchronizacjaPobierzPoleBlokadaZamowien(item, "", pars);
                    _config.SynchronizacjaPobierzMinimalnaWartoscZamowienia(item, "", pars);
                    _config.SynchronizacjaPobierzPoleOpiekun(item, "", pars, opiekun);
                    _config.SynchronizacjaPobierzPoleDrugiOpiekun(item, "", pars);
                    _config.SynchronizacjaPobierzPolePrzedstawiciel(item, "", pars);
                    _config.SynchronizacjaPobierzPoleKlientNadrzedny(item, "", pars);
                    _config.SynchronizacjaPobierzPoleEmail(item, kh_EMail);
                    _config.SynchronizacjaUstawPoziomCeny(item, kh_Cena != null && kh_Cena != 0 ? kh_Cena.Value : (int?) null);
                    items.Add(item.Id, item);

                    Adres it = new Adres(kh_Id*10000 + 1, adr_Adres, adr_Kod, adr_Miejscowosc, adr_IdPanstwo, adr_IdWojewodztwo, adr_Telefon, item.Nazwa, kh_EMail);
                    it.AutorId = kh_Id;
                    it.TypAdresu = TypAdresu.Glowny;
                    adresy.Add(it, new KlientAdres {AdresId = it.Id, KlientId = kh_Id, TypAdresu = TypAdresu.Glowny});
                    if (!string.IsNullOrEmpty(adres) && !string.IsNullOrEmpty(miasto) && kh_AdresDostawy)
                    {
                        Adres it2 = new Adres(kh_Id*10000 + 2, adres, kod, miasto, panstwo, wojewdztwo2, telefon2, adresnazwa2, kh_EMail);

                        adresy.Add(it2, new KlientAdres {AdresId = it2.Id, KlientId = kh_Id, TypAdresu = TypAdresu.Brak});
                    }
                }
                rd.Close();
                rd.Dispose();
                cmd.Dispose();
                log.Debug($"Pętla wyciągania klientów koniec {DateTime.Now}. Liczba klientów {items.Count}");
            } finally
            {
                if (rd != null)
                {
                    rd.Close();
                    rd.Dispose();
                }
                if (cmd != null) cmd.Dispose();
                if (conn != null)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
            return items;
        }

        private Dictionary<int, Dictionary<string, string>> PobierzPolaWlasneKlientow()
        {
            Dictionary<int, Dictionary<string, string>> customersFields = new Dictionary<int, Dictionary<string, string>>();
            SqlConnection conn = null;
            SqlCommand cmd = null;
            SqlDataReader rd = null;
            try
            {
                conn = new SqlConnection(_config.ERPcs);
                conn.Open();

                var slownikZWartosciami = PobierzSlownikZWartosciami(conn, out cmd, out rd, -12);

                log.Debug($"Pętla wyciągania pol wlasnych klientow początek {DateTime.Now}");

                cmd = new SqlCommand(SQL_PW, conn);
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                cmd = new SqlCommand("SELECT kh_id,pole,pwp_nazwa  FROM solex_pola_klienta", conn);
                rd = cmd.ExecuteReader();
                while (rd.Read()) //pobieranie pol prostych i rozszerzonych produktow
                {
                    int kh_id = DataHelper.dbi("kh_id", rd);
                    string field = DataHelper.dbs("pwp_nazwa", rd);
                    string fieldValue = DataHelper.dbs("pole", rd);
                    log.Debug($"Pobieram Pole:{field} o wartości:{fieldValue}");
                    //jeśli pole własne jest w słowniku to podmieniam wartość
                    Dictionary<string,string> slownik;
                    //czy pole ma przypisany słownik
                    if (slownikZWartosciami.TryGetValue(field, out slownik))
                    {
                        //pobieranie wartości dla pola
                        string wartoscSlownikowa;
                        if (slownik.TryGetValue(fieldValue, out wartoscSlownikowa))
                        {
                            fieldValue = wartoscSlownikowa;
                        }
                    }

                    Dictionary<string, string> data;
                    if (!customersFields.TryGetValue(kh_id, out data))
                    {
                        data = new Dictionary<string, string>();
                        customersFields.Add(kh_id, data);
                    }
                    data[field] = fieldValue;
                }
                rd.Close();
                rd.Dispose();
                cmd.Dispose();
            } finally
            {
                if (rd != null)
                {
                    rd.Close();
                    rd.Dispose();
                }
                if (cmd != null) cmd.Dispose();
                if (conn != null)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
            return customersFields;
        }

        private bool CzyWyciagamyProdukty()
        {
            return _config.SubiektWidocznoscTowarow != WidcznoscProduktowWSubiekcie.ZadenProduktNieJestPobierany;
        }

        internal List<ProduktCecha> GetProductTraits()
        {
            Dictionary<long, ProduktCecha> result = new Dictionary<long, ProduktCecha>();
            if (!CzyWyciagamyProdukty())
            {
                throw new Exception("Nie uzupełnione ustawienie \"Widocznośc produktów w subiekcie\"");
                //return result;
            }
            SqlConnection conn = null;
            SqlDataReader rd = null;
            SqlCommand cmd = null;

            int visibility = (int) _config.SubiektWidocznoscTowarow;

            try
            {
                conn = new SqlConnection(_config.ERPcs);
                conn.Open();
                cmd = new SqlCommand(@"select ctw.cht_idtowar,ctw.cht_idcecha from dbo.tw_CechaTw ctw join tw__Towar t  
                    on ctw.cht_idtowar=t.tw_id  where t.tw_zablokowany=0 and t.tw_rodzaj in(1,2,8) 
                      and 1=(case when @visible=1 then t.tw_SklepInternet  
                      else (case when @visible=2 then t.tw_SerwisAukcyjny  
                      else (case when @visible=3 then t.tw_SprzedazMobilna  
                      else 1 end) end) end)
                     union
                      select t.tw_id ,@przesuniecie+tw_IdKrajuPochodzenia from  tw__Towar t  
                      where t.tw_IdKrajuPochodzenia is not null and t.tw_zablokowany=0 and t.tw_rodzaj in(1,2,8) 
                      and 1=(case when @visible=1 then t.tw_SklepInternet  
                      else (case when @visible=2 then t.tw_SerwisAukcyjny  
                      else (case when @visible=3 then t.tw_SprzedazMobilna  
                      else 1 end) end) end)
                    ", conn);
                cmd.Parameters.AddWithValue("@visible", visibility);
                cmd.Parameters.AddWithValue("@przesuniecie", krajPochodzeniaPrzesuniecie);
                rd = cmd.ExecuteReader();
                while (rd.Read()) //pobieranie cech produktów
                {
                    int product_Id = DataHelper.dbi("cht_idtowar", rd);
                    int trait = DataHelper.dbi("cht_idcecha", rd);
                    var cp = new ProduktCecha {ProduktId = product_Id, CechaId = trait};
                    result.Add(cp.Id, cp);
                }
            } finally
            {
                rd.Close();
                rd.Dispose();
                cmd.Dispose();
                conn.Close();
                conn.Dispose();
            }

            string[] fields = _config.PolaWlasneCechy;



            Dictionary<int, Dictionary<string, string>> productsFields = PobierzPolaWlasne(conn, cmd, rd);
            log.Debug("pobranych pól własnych " + productsFields.Count);
            log.Debug("połączeń przed sprawdzaniem pól własnych: " + result.Count);
            try
            {
                foreach (KeyValuePair<int, Dictionary<string, string>> nameValueCollection in productsFields)
                {
                    foreach (string pwnaceche in fields)
                    {

                        if (nameValueCollection.Value.ContainsKey(pwnaceche))
                        {
                            var pole = nameValueCollection.Value[pwnaceche];
                            string symbol = (pwnaceche + ":" + pole).Trim().ToLower();

                            var atrybut = new Atrybut(pwnaceche);
                            atrybut.Id = pwnaceche.WygenerujIDObiektu();

                            Cecha c = new Cecha(pole, symbol) {AtrybutId = atrybut.Id, Widoczna = true};
                            c.Id = c.WygenerujIDObiektu();

                            ProduktCecha cp = new ProduktCecha();
                            cp.CechaId = c.Id;
                            cp.ProduktId = nameValueCollection.Key;

                            result.Add(cp.Id, cp);
                        }
                    }
                }
            } catch (Exception ex)
            {
                log.Error(new Exception("Błąd przy pobieraniu pól własnych produktów: ", ex));
            }

            try
            {
                const string SELECT_PRODUCTS = @"SELECT         
                         t.tw_Id,
                         t.tw_IdGrupa,
                          t.tw_IdProducenta
  FROM tw__Towar t 
  where t.tw_zablokowany=0 and t.tw_rodzaj in(1,2,8) 
  and 1=(case when @visible=1 then t.tw_SklepInternet  
  else (case when @visible=2 then t.tw_SerwisAukcyjny  
  else (case when @visible=3 then t.tw_SprzedazMobilna  
  else 1 end) end) end)";
                List<Grupa> grupy = ApiWywolanie.PobierzGrupy();
                try
                {
                    conn = new SqlConnection(_config.ERPcs);
                    conn.Open();
                    cmd = new SqlCommand(SELECT_PRODUCTS, conn);
                    cmd.Parameters.AddWithValue("@visible", visibility);
                    rd = cmd.ExecuteReader();
                    while (rd.Read()) //pobieranie cech produktów
                    {
                        int tw_Id = DataHelper.dbi("tw_id", rd);
                        int? tw_IdGrupa = DataHelper.dbin("tw_IdGrupa", rd);
                        if (tw_IdGrupa != null && grupy.Any(x => !string.IsNullOrEmpty(x.Parametry)))
                        {
                            var cp = new ProduktCecha {ProduktId = tw_Id, CechaId = grupyoffset + tw_IdGrupa.Value};
                            result.Add(cp.Id, cp);
                        }
                    }

                } finally
                {
                    rd.Close();
                    rd.Dispose();
                    cmd.Dispose();
                }
            } catch (Exception ex)
            {
                log.Error("nie udało się pobrać połączeń z grup produktów z powodu błędu: " + ex.Message, ex);
            }

            Dictionary<int, int> lacznikiProducentow = PobierzProducentowLaczniki();
            foreach (KeyValuePair<int, int> lacznikProducenta in lacznikiProducentow)
            {
                ProduktCecha cp = new ProduktCecha();
                cp.CechaId = lacznikProducenta.Value;
                cp.ProduktId = lacznikProducenta.Key;

                result.Add(cp.Id, cp);
            }
            DodajLacznikiPromocji(ref result);
            log.Debug("połączeń po " + result.Count);
            return result.Values.ToList();
        }

        private void DodajLacznikiPromocji(ref Dictionary<long, ProduktCecha> result)
        {
            using (DbConnection conn = new SqlConnection(_config.ERPcs))
            {
                conn.Open();

                using (DbCommand command = new SqlCommand("select pct_IdPromocja, pct_IdTowaru from dok_PromocjaTowar where pct_IdPromocja in ( select pc_Id from dok_Promocja where isnull( pc_Nieaktywna,0) = 0 AND  (pc_Do is null or pc_Do >= GetDate() or pc_OgraniczonaCzasowo=0) )"))
                {
                    command.Connection = conn;
                    using (DbDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int idproduktu = DataHelper.dbi("pct_IdTowaru", reader);
                            int idPromocji = DataHelper.dbi("pct_IdPromocja", reader) + rabat_offset_CechyProduktu;
                            ProduktCecha kk = new ProduktCecha(idproduktu, idPromocji);
                            if (!result.ContainsKey(kk.Id))
                            {
                                result.Add(kk.Id, kk);
                            }
                        }
                    }
                }
                conn.Close();
            }
        }




        const string POLA_PRODUKTOW = @"if  exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[solex_pola_produktow]') or id = object_id(N'[solex_pola_produktow]'))

exec('DROP VIEW [solex_pola_produktow]; ');
exec('create  view solex_pola_produktow as 
Select pwd_idobiektu,wartosc,pwp_nazwa from( 
select pwd_idobiektu,pwd_typobiektu,Wartosc,Pole
from (select  pwd_idobiektu,pwd_typobiektu
,pwd_liczba01 =cast(pwd_liczba01 as varchar(max))
,pwd_liczba02=cast(pwd_liczba02 as varchar(max))
,pwd_liczba03=cast(pwd_liczba03 as varchar(max))
,pwd_liczba04=cast(pwd_liczba04 as varchar(max))
,pwd_liczba05=cast(pwd_liczba05 as varchar(max))
,pwd_liczba06=cast(pwd_liczba06 as varchar(max))
,pwd_liczba07=cast(pwd_liczba07 as varchar(max))
,pwd_liczba08=cast(pwd_liczba08 as varchar(max))
,pwd_liczba09=cast(pwd_liczba09 as varchar(max))
,pwd_liczba10=cast(pwd_liczba10  as varchar(max))
,pwd_data01 =convert( varchar(max),pwd_data01,104)
,pwd_data02=convert( varchar(max),pwd_data02,104)
,pwd_data03=convert( varchar(max),pwd_data03,104)
,pwd_data04=convert( varchar(max),pwd_data04,104)
,pwd_data05=convert( varchar(max),pwd_data05,104)
,pwd_data06=convert( varchar(max),pwd_data06,104)
,pwd_data07=convert( varchar(max),pwd_data07,104)
,pwd_data08=convert( varchar(max),pwd_data08,104)
,pwd_data09=convert( varchar(max),pwd_data09,104)
,pwd_data10=convert( varchar(max),pwd_data10,104)
,pwd_kwota01=cast(pwd_kwota01 as varchar(max))
,pwd_kwota02=cast(pwd_kwota02 as varchar(max))
,pwd_kwota03=cast(pwd_kwota03 as varchar(max))
,pwd_kwota04=cast(pwd_kwota04 as varchar(max))
,pwd_kwota05=cast(pwd_kwota05 as varchar(max))
,pwd_kwota06=cast(pwd_kwota06 as varchar(max))
,pwd_kwota07=cast(pwd_kwota07 as varchar(max))
,pwd_kwota08=cast(pwd_kwota08 as varchar(max))
,pwd_kwota09=cast(pwd_kwota09 as varchar(max))
,pwd_kwota10=cast(pwd_kwota10 as varchar(max))
,pwd_Tekst01 =cast(pwd_tekst01  as varchar(max))
,pwd_Tekst02 =cast(pwd_tekst02 as varchar(max))
,pwd_Tekst03=cast(pwd_tekst03 as varchar(max))
,pwd_Tekst04=cast(pwd_tekst04 as varchar(max))
,pwd_Tekst05=cast(pwd_tekst05 as varchar(max))
,pwd_Tekst06=cast(pwd_tekst06 as varchar(max))
,pwd_Tekst07=cast(pwd_tekst07 as varchar(max))
,pwd_Tekst08=cast(pwd_tekst08 as varchar(max))
,pwd_Tekst09=cast(pwd_tekst09 as varchar(max))
,pwd_Tekst10=cast(pwd_tekst10 as varchar(max))
,pwd_fk01 =cast(pwd_fk01  as varchar(max))
,pwd_fk02 =cast(pwd_fk02 as varchar(max))
,pwd_fk03=cast(pwd_fk03 as varchar(max))
,pwd_fk04=cast(pwd_fk04 as varchar(max))
,pwd_fk05=cast(pwd_fk05 as varchar(max))
,pwd_fk06=cast(pwd_fk06 as varchar(max))
,pwd_fk07=cast(pwd_fk07 as varchar(max))
,pwd_fk08=cast(pwd_fk08 as varchar(max))
,pwd_fk09=cast(pwd_fk09 as varchar(max))
,pwd_fk10=cast(pwd_fk10 as varchar(max))
 from pw_dane )  a
unpivot
  (Wartosc for Pole in (pwd_liczba01,pwd_liczba02,pwd_liczba03,pwd_liczba04,pwd_liczba05,pwd_liczba06,pwd_liczba07,pwd_liczba08,pwd_liczba09,pwd_liczba10,
 pwd_data01,pwd_data02,pwd_data03,pwd_data04,pwd_data05,pwd_data06,pwd_data07,pwd_data08,pwd_data09,pwd_data10,pwd_kwota01,
 pwd_kwota02,pwd_kwota03,pwd_kwota04,pwd_kwota05,pwd_kwota06,pwd_kwota07,pwd_kwota08,pwd_kwota09,pwd_kwota10,pwd_tekst01
 ,pwd_tekst02,pwd_tekst03,pwd_tekst04,pwd_tekst05,pwd_tekst06,pwd_tekst07,pwd_tekst08,pwd_tekst09,pwd_tekst10
 ,pwd_fk01,pwd_fk02,pwd_fk03,pwd_fk04,pwd_fk05,pwd_fk06,pwd_fk07,pwd_fk08,pwd_fk09,pwd_fk10)
   ) as Amount ) a join pw_pole p on a.pole=p.pwp_pole where a.pwd_typobiektu=-14 and p.pwp_typobiektu=-14
union
select tw_id,klient.wartosc,Pole=pola.wartosc from 
(
select
      tw_id,
       Wartosc,Pole
from (select tw_id,  tw_Pole1 as twp_Nazwa1
      ,[tw_Pole2] as twp_Nazwa2
      ,[tw_Pole3] as twp_Nazwa3
      ,[tw_Pole4] as twp_Nazwa4
      ,[tw_Pole5] as twp_Nazwa5
      ,[tw_Pole6] as twp_Nazwa6
      ,[tw_Pole7] as twp_Nazwa7
      ,[tw_Pole8] as twp_Nazwa8 from tw__towar) k  
unpivot
  (Wartosc for Pole in (
      [twp_Nazwa1]
      ,[twp_Nazwa2]
      ,[twp_Nazwa3]
      ,[twp_Nazwa4]
      ,[twp_Nazwa5]
      ,[twp_Nazwa6]
      ,[twp_Nazwa7]
      ,[twp_Nazwa8])
   ) as Amount ) klient join 
   (SELECT 
        Wartosc,Pole
  FROM [tw_Parametr]
  unpivot
  (Wartosc for Pole in ( [twp_Nazwa1]
      ,[twp_Nazwa2]
      ,[twp_Nazwa3]
      ,[twp_Nazwa4]
      ,[twp_Nazwa5]
      ,[twp_Nazwa6]
      ,[twp_Nazwa7]
      ,[twp_Nazwa8])
   ) as Amount) pola on klient.pole=pola.pole   ');";

        private Dictionary<int, Dictionary<string, string>> PobierzPolaWlasne(SqlConnection conn, SqlCommand cmd, SqlDataReader rd)
        {
            Dictionary<int, Dictionary<string, string>> productsFields = new Dictionary<int, Dictionary<string, string>>();

            conn = new SqlConnection(_config.ERPcs);
            conn.Open();
            log.Info($"Pętla wyciągania pól własnych towarów początek {DateTime.Now}");
            var slownikZWartosciami = PobierzSlownikZWartosciami(conn, out cmd, out rd, -14);
            cmd = new SqlCommand(POLA_PRODUKTOW, conn);
            cmd.ExecuteNonQuery();
            cmd.Dispose();
            HashSet<string> polalista = new HashSet<string>();
            cmd = new SqlCommand("SELECT distinct pwp_nazwa  FROM solex_pola_produktow", conn);
            rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                string field = DataHelper.dbs("pwp_nazwa", rd);
                polalista.Add(field);
            }
            rd.Close();
            rd.Dispose();
            cmd.Dispose();

            cmd = new SqlCommand("SELECT pwd_idobiektu,wartosc,pwp_nazwa  FROM solex_pola_produktow", conn);
            rd = cmd.ExecuteReader();
            while (rd.Read()) //pobieranie pol prostych i rozszerzonych produktow
            {
                int product_Id = DataHelper.dbi("pwd_idobiektu", rd);
                string field = DataHelper.dbs("pwp_nazwa", rd);
                string fieldValue = DataHelper.dbs("wartosc", rd);
                //jeśli pole własne jest w słowniku to podmieniam wartość
                Dictionary<string, string> slownik;
                //czy pole ma przypisany słownik
                if (slownikZWartosciami.TryGetValue(field, out slownik))
                {
                    //pobieranie wartości dla pola
                    string wartoscSlownikowa;
                    if (slownik.TryGetValue(fieldValue, out wartoscSlownikowa))
                    {
                        fieldValue = wartoscSlownikowa;
                    }
                }
                Dictionary<string, string> data;
                if (!productsFields.TryGetValue(product_Id, out data))
                {
                    data = new Dictionary<string, string>();
                    foreach (var p in polalista)
                    {
                        data.Add(p, "");
                    }
                    productsFields.Add(product_Id, data);
                }
                data[field] = fieldValue;
            }
            rd.Close();
            rd.Dispose();
            cmd.Dispose();


            log.Info($"Pętla wyciągania pol wlasnych towarów koniec {DateTime.Now} Liczba produktów z polami {productsFields.Count}");


            return productsFields;
        }

        /// <summary>
        /// Pobiera słownikowe pola własne z subiekta, domysnie dla produktów 
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="cmd"></param>
        /// <param name="rd"></param>
        /// <param name="typ">Czy dla produktów (-14) czy dla klientów (-12)</param>
        /// <returns></returns>
        private static Dictionary<string, Dictionary<string, string>> PobierzSlownikZWartosciami(SqlConnection conn, out SqlCommand cmd, out SqlDataReader rd, int typ = -14)
        {
            if (typ != -12 && typ != -14)
            {
                throw new Exception($"Niepoprawny typ obiektu({typ}). Dozwolone wartości to -12 i -14.");
            }
            Dictionary<string, string> polaslownikowe = new Dictionary<string, string>();
            string sqlslowniki = $"SELECT pole=pwp_Nazwa,zapytanie=pwp_Select FROM pw_Pole where pwp_Pole like 'pwd_Fk%' and pwp_TypObiektu={typ} and ISNULL(pwp_select,'')<>''";
            cmd = new SqlCommand(sqlslowniki, conn);
            rd = cmd.ExecuteReader();
            while (rd.Read()) //pobieranie pól słownikowych
            {
                string pole = DataHelper.dbs("pole", rd);
                string zapytanie = DataHelper.dbs("zapytanie", rd);
                polaslownikowe.Add(pole, zapytanie);
            }
            rd.Close();
            rd.Dispose();
            cmd.Dispose();
            Dictionary<string, Dictionary<string, string>> slownikZWartosciami = new Dictionary<string, Dictionary<string, string>>();
            foreach (var ps in polaslownikowe)
            {
                Dictionary<string, string> wynik = new Dictionary<string, string>();
                cmd = new SqlCommand(ps.Value, conn);
                rd = cmd.ExecuteReader();
                while (rd.Read()) //pobieranie pol prostych i rozszerzonych produktow
                {
                    string pole = rd[0].ToString();
                    string zapytanie = rd[1].ToString();
                    wynik.Add(pole, zapytanie);
                }
                rd.Close();
                rd.Dispose();
                cmd.Dispose();
                slownikZWartosciami.Add(ps.Key, wynik);
            }
            return slownikZWartosciami;
        }

        const string SelectProducts = @"SELECT         
                         t.tw_Id,
                         t.tw_Objetosc,
                         t.tw_IdGrupa,
                         t.tw_JednMiary,
                         t.tw_Nazwa,
                         t.tw_Opis,
                         t.tw_PKWiU,
                         t.tw_PodstKodKresk,
                         t.tw_StanMin,
                         t.tw_Symbol,
                         sv.vat_Stawka,
                         sv.vat_Symbol,
                         t.tw_Charakter,
                         t.tw_WWW,
                         t.tw_Masa,
                          t.tw_IdProducenta,
             marka_nazwa=p.adr_NazwaPelna,
                         t.tw_Uwagi,
                         t.tw_UrzNazwa,
                         t.tw_DostSymbol,
                         t.tw_CzasDostawy,
                         t.tw_Rodzaj,
                         kp.krp_Nazwa,
                         tw_KodTowaru
  FROM tw__Towar t join sl_StawkaVAT sv on t.tw_IdVatSp=sv.vat_Id left join sl_krajpochodzenia kp on t.tw_IdKrajuPochodzenia=kp.krp_Id 
  left join  adr__Ewid p on t.tw_IdProducenta = p.adr_IdObiektu and p.adr_TypAdresu = 1  and ( p.adr_Symbol <> '********************')  
  where t.tw_zablokowany=0 and t.tw_rodzaj in(1,2,8) 
  and 1=(case when @visible=1 then t.tw_SklepInternet  
  else (case when @visible=2 then t.tw_SerwisAukcyjny  
  else (case when @visible=3 then t.tw_SprzedazMobilna  
  else 1 end) end) end)";

        // rodzaje towarów
        // 1 - towar
        // 2 - usługa
        // 4 - opakowanie
        // 8 - komplet
        internal List<Model.Produkt> PobierzProdukty(out List<Tlumaczenie> tlumaczenia, out List<JednostkaProduktu> jednostki, HashSet<string> magazyny)
        {
            tlumaczenia = new List<Tlumaczenie>();
            jednostki = new List<JednostkaProduktu>();
            List<Model.Produkt> items = new List<Model.Produkt>(5000);
            if (!CzyWyciagamyProdukty())
            {

                return items;
            }
            HashSet<int> ids = new HashSet<int>();
            foreach (string s in magazyny)
            {
                try
                {


                    ids.Add(PobierzIdMagazynu(s));
                } catch (Exception)
                {}
            }


            Dictionary<int, string> zestawystany = PobierzStanyZestawow();
            SqlConnection conn = null;
            SqlCommand cmd = null;
            SqlDataReader rd = null;
            Dictionary<int, Dictionary<string, string>> productsFields = PobierzPolaWlasne(conn, cmd, rd);
            Dictionary<int, List<KeyValuePair<int, string>>> productsTraits = new Dictionary<int, List<KeyValuePair<int, string>>>();
            Dictionary<int, List<KeyValuePair<string, decimal>>> productsQuantities = new Dictionary<int, List<KeyValuePair<string, decimal>>>();
            Dictionary<int, List<Tuple<decimal, DateTime>>> produktydostawy = new Dictionary<int, List<Tuple<decimal, DateTime>>>();
            Dictionary<int, Tuple<decimal, decimal>> stany = new Dictionary<int, Tuple<decimal, decimal>>();
            try
            {

                conn = new SqlConnection(_config.ERPcs);
                conn.Open();
                log.InfoFormat($"Pętla wyciągania cech  towarow początek {DateTime.Now}");
                cmd = new SqlCommand("select cht_idtowar,slc.ctw_id,slc.ctw_nazwa from dbo.tw_CechaTw ctw join sl_cechaTw slc on ctw.cht_idcecha=slc.ctw_id order by ctw.cht_idtowar", conn);
                rd = cmd.ExecuteReader();
                while (rd.Read()) //pobieranie cech produktów
                {
                    int product_Id = DataHelper.dbi("cht_idtowar", rd);
                    int trait = DataHelper.dbi("ctw_id", rd);
                    string traitName = DataHelper.dbs("ctw_nazwa", rd);
                    List<KeyValuePair<int, string>> data;
                    if (!productsTraits.TryGetValue(product_Id, out data))
                    {
                        data = new List<KeyValuePair<int, string>>();
                        productsTraits.Add(product_Id, data);
                    }
                    data.Add(new KeyValuePair<int, string>(trait, traitName));
                }
                rd.Close();
                rd.Dispose();
                cmd.Dispose();

                if (ids.Any())
                {
                    log.Debug($"Pętla wyciągania cech  towarow koniec {DateTime.Now}  Liczba produktów z cechami {productsTraits.Count}");
                    const string sql = @"select  ob_TowId,d.dok_TerminRealizacji,ob_Ilosc from dok_Pozycja dp join dok__dokument d on dp.ob_DokHanId=d.dok_Id where 
                          d.dok_Typ = 15    and dok_magid in({0})
                            and d.dok_Status <> 8 and dp.ob_Ilosc>0 and dok_TerminRealizacji>GETDATE() order by ob_towid,dok_TerminRealizacji";
                    log.InfoFormat($"Pętla wyciągania rezerwacji  towarow początek {DateTime.Now}");

                    cmd = new SqlCommand(string.Format(sql, Serializacje.PobierzInstancje.SerializeList(ids)), conn);
                    rd = cmd.ExecuteReader();
                    while (rd.Read()) //pobieranie cech produktów
                    {
                        int product_Id = DataHelper.dbi("ob_TowId", rd);
                        DateTime dok_TerminRealizacji = DataHelper.dbdt("dok_TerminRealizacji", rd);
                        decimal ob_Ilosc = DataHelper.dbd("ob_Ilosc", rd);
                        List<Tuple<decimal, DateTime>> data;
                        if (!produktydostawy.TryGetValue(product_Id, out data))
                        {
                            data = new List<Tuple<decimal, DateTime>>();
                            produktydostawy.Add(product_Id, data);
                        }
                        data.Add(new Tuple<decimal, DateTime>(ob_Ilosc, dok_TerminRealizacji));
                    }
                    rd.Close();
                    rd.Dispose();
                    cmd.Dispose();
                    log.Debug($"Pętla wyciągania rezerwacji  towarow koniec {DateTime.Now}  Liczba produktów z dostawami {produktydostawy.Count}");

                    const string sqlstany = @"select st_TowId,lacznie_na_stanie=sum( st_Stan) ,lacznie_rezerwacji=sum(st_StanRez) from tw_Stan 
                                    where st_magid in(1) group by st_towid  ";
                    log.InfoFormat($"Pętla wyciągania stanów  towarow początek {DateTime.Now}");

                    cmd = new SqlCommand(string.Format(sqlstany, Serializacje.PobierzInstancje.SerializeList(ids)), conn);
                    rd = cmd.ExecuteReader();
                    while (rd.Read()) //pobieranie cech produktów
                    {
                        int product_Id = DataHelper.dbi("st_TowId", rd);
                        decimal lacznie_Na_Stanie = DataHelper.dbd("lacznie_na_stanie", rd);
                        decimal lacznie_Rezerwacji = DataHelper.dbd("lacznie_rezerwacji", rd);
                        stany.Add(product_Id, new Tuple<decimal, decimal>(lacznie_Na_Stanie, lacznie_Rezerwacji));
                    }
                    rd.Close();
                    rd.Dispose();
                    cmd.Dispose();
                    log.Debug($"Pętla wyciągania stanów  towarow koniec {DateTime.Now}  Liczba produktów z stanów {produktydostawy.Count}");
                }

                log.Debug($"Pętla wyciągania jednostek  towarow początek {DateTime.Now}");
                cmd = new SqlCommand("select jm_idtowar,jm_idjednMiary,jm_przelicznik from tw_JednMiary", conn);
                rd = cmd.ExecuteReader();
                while (rd.Read()) //pobieranie jednostek produktów
                {
                    int product_Id = DataHelper.dbi("jm_idtowar", rd);
                    decimal quantity = DataHelper.dbd("jm_przelicznik", rd);
                    string quantityUnit = DataHelper.dbs("jm_idjednMiary", rd);
                    List<KeyValuePair<string, decimal>> data;
                    if (!productsQuantities.TryGetValue(product_Id, out data))
                    {
                        data = new List<KeyValuePair<string, decimal>>();
                        productsQuantities.Add(product_Id, data);
                    }
                    data.Add(new KeyValuePair<string, decimal>(quantityUnit, quantity));
                }
                rd.Close();
                rd.Dispose();
                cmd.Dispose();
                log.DebugFormat("Pętla wyciągania jednostek  towarow koniec {0}  Liczba produktów z jednostkami {1}", DateTime.Now, productsQuantities.Count);

                log.DebugFormat("Pętla wyciągania towarów początek {0}", DateTime.Now);

                int visibility = (int) _config.SubiektWidocznoscTowarow;

                cmd = new SqlCommand(SelectProducts, conn);
                cmd.Parameters.AddWithValue("@visible", visibility);
                cmd.CommandTimeout = 600;
                rd = cmd.ExecuteReader();
                while (rd.Read()) //pobieranie  produktów
                {
                    try
                    {
                        int tw_id = DataHelper.dbi("tw_id", rd);
                        string tw_JednMiary = DataHelper.dbs("tw_JednMiary", rd);
                        string tw_Nazwa = DataHelper.dbs("tw_Nazwa", rd);
                        string tw_Opis = DataHelper.dbs("tw_Opis", rd);
                        string tw_PKWiU = DataHelper.dbs("tw_PKWiU", rd);
                        string tw_PodstKodKresk = DataHelper.dbs("tw_PodstKodKresk", rd);
                        decimal tw_StanMin = DataHelper.dbd("tw_StanMin", rd);
                        decimal lacznie_rezerwacji = 0;
                        decimal lacznie_na_stanie = 0;
                        if (stany.ContainsKey(tw_id))
                        {
                            lacznie_na_stanie = stany[tw_id].Item1;
                            lacznie_rezerwacji = stany[tw_id].Item2;
                        }
                        string tw_Symbol = DataHelper.dbs("tw_Symbol", rd);
                        decimal vat_Stawka = DataHelper.dbd("vat_Stawka", rd);
                        string vat_symbol = DataHelper.dbs("vat_Symbol", rd);
                        string tw_Charakter = DataHelper.dbs("tw_Charakter", rd);
                        string tw_WWW = DataHelper.dbs("tw_WWW", rd);
                        decimal tw_Masa = DataHelper.dbd("tw_Masa", rd);
                        string tw_Uwagi = DataHelper.dbs("tw_Uwagi", rd);
                        string tw_UrzNazwa = DataHelper.dbs("tw_UrzNazwa", rd);
                        string tw_DostSymbol = DataHelper.dbs("tw_DostSymbol", rd);
                        int tw_CzasDostawy = DataHelper.dbi("tw_CzasDostawy", rd);
                        int tw_rodzaj = DataHelper.dbi("tw_Rodzaj", rd);
                        string kraj_pochodzenia = DataHelper.dbs("krp_Nazwa", rd);
                        string marka_nazwa = DataHelper.dbs("marka_nazwa", rd);
                        DateTime? data_dostawy = WyliczTerminDostawy(tw_id, produktydostawy, lacznie_rezerwacji, lacznie_na_stanie);
                        decimal tw_Objetosc = DataHelper.dbd("tw_Objetosc", rd);
                        Dictionary<string, string> pars;
                        List<KeyValuePair<int, string>> productTraits;
                        if (!productsTraits.TryGetValue(tw_id, out productTraits))
                        {
                            productTraits = new List<KeyValuePair<int, string>>();
                        }
                        if (!productsFields.TryGetValue(tw_id, out pars))
                        {
                            pars = new Dictionary<string, string>();
                        }

                        string kodCN = DataHelper.dbs("tw_KodTowaru", rd);

                        pars["tw_KodTowaru_kodCN"] = kodCN;

                        pars["lacznie_rezerwacji"] = lacznie_rezerwacji.ToString();
                        pars["lacznie_na_stanie"] = lacznie_na_stanie.ToString();
                        pars["marka_nazwa"] = marka_nazwa;
                        pars["tw_Uwagi"] = TextHelper.PobierzInstancje.GetHTMLBodyContent((tw_Uwagi ?? ""));
                        pars["tw_Opis"] = TextHelper.PobierzInstancje.GetHTMLBodyContent((tw_Opis ?? ""));
                        pars["tw_WWW"] = tw_WWW;
                        pars["tw_CzasDostawy"] = tw_CzasDostawy.ToString();
                        pars["tw_Symbol"] = tw_Symbol;
                        pars["tw_PKWiU"] = tw_PKWiU;
                        pars["tw_Charakter"] = TextHelper.PobierzInstancje.GetHTMLBodyContent(tw_Charakter ?? "");

                        pars["tw_Nazwa"] = tw_Nazwa;
                        pars["tw_DostSymbol"] = tw_DostSymbol;
                        pars["tw_UrzNazwa"] = tw_UrzNazwa;
                        pars["tw_id"] = tw_id.ToString();
                        pars["tw_StanMin"] = tw_StanMin.ToString();
                        pars["tw_Objetosc"] = tw_Objetosc <= 0m ? null : tw_Objetosc.ToString(CultureInfo.InvariantCulture);
                        pars["tw_PodstKodKresk"] = tw_PodstKodKresk;
                        pars["kraj_pochodzenia"] = kraj_pochodzenia;
                        pars["tw_Masa"] = tw_Masa <= 0m ? null : tw_Masa.ToString(CultureInfo.InvariantCulture);

                        Model.Produkt item = new Model.Produkt();
                        item.UstawWidocznoscProduktu(true);
                        item.KodKreskowy = !string.IsNullOrEmpty( tw_PodstKodKresk ) ? tw_PodstKodKresk : null;
                        item.Id = tw_id;

                        if (tw_rodzaj == 2) item.Typ = TypProduktu.Usluga;

                        item.StanMin = tw_StanMin;
                        item.PKWiU = tw_PKWiU;
                        item.Www = tw_WWW;
                        item.Vat = vat_Stawka;
                        List<JednostkaProduktu> tmpj = new List<JednostkaProduktu>();
                        JednostkaProduktu jp = new JednostkaProduktu {Podstawowa = true, Id = tw_JednMiary.WygenerujIDObiektu(), Nazwa = tw_JednMiary, ProduktId = tw_id, Przelicznik = 1};
                        tmpj.Add(jp);

                        if (vat_symbol == "oo")
                        {
                            item.VatOdwrotneObciazenie = true;
                        }

                        List<KeyValuePair<string, decimal>> quan;
                        if (!productsQuantities.TryGetValue(tw_id, out quan))
                        {
                            quan = new List<KeyValuePair<string, decimal>>();
                        }

                        foreach (var keyValuePair in quan)
                        {
                            JednostkaProduktu jp2 = new JednostkaProduktu {Podstawowa = false, Id = keyValuePair.Key.WygenerujIDObiektu(), Nazwa = keyValuePair.Key, ProduktId = tw_id, Przelicznik = keyValuePair.Value};

                            if (!tmpj.Any(a => a.ProduktId == jp2.ProduktId && a.Id == jp2.Id))
                            {
                                tmpj.Add(jp2);
                            }
                        }
                        if (zestawystany.Any())
                        {
                            string stan;
                            zestawystany.TryGetValue((int) item.Id, out stan);
                            pars.Add("StanZestawu", stan ?? "");
                        }
                        jednostki.AddRange(tmpj);
                        _config.SynchronizacjaPobierzObjetoscProduktu(item, "tw_Objetosc", pars);
                        _config.SynchronizacjaPobierzWageProduktu(item, "tw_Masa", pars);

                        _config.SynchronizacjaPobierzWidocznoscProduktuZPola(item, "", pars);
                        _config.SynchronizacjaPobierzPoleLiczba1(item, "", pars);
                        _config.SynchronizacjaPobierzPoleLiczba2(item, "", pars);
                        _config.SynchronizacjaPobierzPoleLiczba3(item, "tw_KodTowaru_kodCN", pars);
                        _config.SynchronizacjaPobierzPoleLiczba4(item, "", pars);
                        _config.SynchronizacjaPobierzPoleLiczba5(item, "", pars);
                        _config.SynchronizacjaPobierzKolumnaLiczba1(item, "", pars);
                        _config.SynchronizacjaPobierzKolumnaLiczba2(item, "", pars);
                        _config.SynchronizacjaPobierzKolumnaLiczba3(item, "", pars);
                        _config.SynchronizacjaPobierzKolumnaLiczba4(item, "", pars);
                        _config.SynchronizacjaPobierzKolumnaLiczba5(item, "", pars);
                        _config.SynchronizacjaPobierzIloscWOpakowaniu(item, "", pars);
                        _config.SynchronizacjaPobierzPoleDostawa(item, "tw_CzasDostawy", pars, data_dostawy);
                        _config.SynchronizacjaPobierzPoleOjciec(item, "", pars);
                        //wymiary dla opakowań
                        _config.SynchronizacjaPobierzGlebokoscOpakowaniaJednostkowego(item, "", pars);
                        _config.SynchronizacjaPobierzWysokoscOpakowaniaJednostkowego(item, "", pars);
                        _config.SynchronizacjaPobierzSzerokoscOpakowaniaJednostkowego(item, "", pars);

                        _config.SynchronizacjaPobierzGlebokoscOpakowaniaZbiorczego(item, "", pars);
                        _config.SynchronizacjaPobierzWysokoscOpakowaniaZbiorczego(item, "", pars);
                        _config.SynchronizacjaPobierzSzerokoscOpakowaniaZbiorczego(item, "", pars);
                        _config.SynchronizacjaPobierzWageOpakowaniaZbiorczego(item, "", pars);
                        _config.SynchronizacjaPobierzObjetoscOpakowaniaZbiorczego(item, "", pars);
                        _config.SynchronizacjaPobierzIloscWOpakowaniuZbiorczym(item, "", pars);

                        _config.SynchronizacjaPobierzGlebokoscPalety(item, "", pars);
                        _config.SynchronizacjaPobierzWysokoscPalety(item, "", pars);
                        _config.SynchronizacjaPobierzSzerokoscPalety(item, "", pars);
                        _config.SynchronizacjaPobierzWagePalety(item, "", pars);
                        _config.SynchronizacjaPobierzObjetoscPalety(item, "", pars);
                        _config.SynchronizacjaPobierzLiczbaSztukNaPalecie(item, "", pars);
                        _config.SynchronizacjaPobierzLiczbaSztukNaWarstwie(item, "", pars);

                        foreach (Jezyk j in _config.JezykiWSystemie.Values)
                        {
                            _config.SynchronizacjaPobierzLokalizacjeNazwa(item, j, "tw_Nazwa", pars, ref tlumaczenia);
                            _config.SynchronizacjaPobierzLokalizacjeKod(item, j, "tw_Symbol", pars, ref tlumaczenia);
                            _config.SynchronizacjaPobierzLokalizacjeOpis(item, j, "", pars, ref tlumaczenia);
                            _config.SynchronizacjaPobierzLokalizacjeOpis2(item, j, "", pars, ref tlumaczenia);
                            _config.SynchronizacjaPobierzLokalizacjeOpis3(item, j, "", pars, ref tlumaczenia);
                            _config.SynchronizacjaPobierzLokalizacjeOpis4(item, j, "", pars, ref tlumaczenia);
                            _config.SynchronizacjaPobierzLokalizacjeOpis5(item, j, "", pars, ref tlumaczenia);
                            _config.SynchronizacjaPobierzLokalizacjeOpisKrotki(item, j, "", pars, ref tlumaczenia);
                            _config.SynchronizacjaPobierzLokalizacjeOpisKrotki2(item, j, "", pars, ref tlumaczenia);
                            _config.SynchronizacjaPobierzLokalizacjeOpisKrotki3(item, j, "", pars, ref tlumaczenia);
                            _config.SynchronizacjaPobierzLokalizacjeOpisKrotki4(item, j, "", pars, ref tlumaczenia);
                            _config.SynchronizacjaPobierzLokalizacjeOpisKrotki5(item, j, "", pars, ref tlumaczenia);
                            _config.SynchronizacjaPobierzLokalizacjePoleTekst1(item, j, "", pars, ref tlumaczenia);
                            _config.SynchronizacjaPobierzLokalizacjePoleTekst2(item, j, "", pars, ref tlumaczenia);
                            _config.SynchronizacjaPobierzLokalizacjePoleTekst3(item, j, "", pars, ref tlumaczenia);
                            _config.SynchronizacjaPobierzLokalizacjePoleTekst4(item, j, "", pars, ref tlumaczenia);
                            _config.SynchronizacjaPobierzLokalizacjePoleTekst5(item, j, "", pars, ref tlumaczenia);
                            _config.SynchronizacjaPobierzLokalizacjeKolumnaTekst1(item, j, "", pars, ref tlumaczenia);
                            _config.SynchronizacjaPobierzLokalizacjeKolumnaTekst2(item, j, "", pars, ref tlumaczenia);
                            _config.SynchronizacjaPobierzLokalizacjeKolumnaTekst3(item, j, "", pars, ref tlumaczenia);
                            _config.SynchronizacjaPobierzLokalizacjeKolumnaTekst4(item, j, "", pars, ref tlumaczenia);
                            _config.SynchronizacjaPobierzLokalizacjeKolumnaTekst5(item, j, "", pars, ref tlumaczenia);
                            _config.SynchronizacjaPobierzLokalizacjeMetaOpis(item, j, "", pars, ref tlumaczenia);
                            _config.SynchronizacjaPobierzLokalizacjeMetaSlowaKluczowe(item, j, "", pars, ref tlumaczenia);
                            _config.SynchronizacjaPobierzLokalizacjeRodzina(item, j, "", pars, ref tlumaczenia);
                        }
                        //   log.DebugFormat("Mapowienie pól koniec {0}", item.produkt_id);
                        if (string.IsNullOrWhiteSpace(item.Nazwa))
                        {
                            item.Nazwa = tw_Nazwa;
                        }
                        if (string.IsNullOrWhiteSpace(item.Kod))
                        {
                            item.Kod = tw_Symbol;
                        }
                        items.Add(item);

                    } catch (Exception xx)
                    {
                        log.ErrorFormat($"Błąd pobierania produktu id: {DataHelper.dbi("tw_id", rd)}");
                        log.Error(xx);
                    }
                }
                rd.Close();
                rd.Dispose();
                cmd.Dispose();
                log.InfoFormat($"Pętla wyciągania towarów koniec {DateTime.Now}. Liczba produktów {items.Count}");
            } finally
            {
                if (rd != null)
                {
                    rd.Close();
                    rd.Dispose();
                }
                if (cmd != null) cmd.Dispose();
                if (conn != null)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
            return items;
        }

        private Dictionary<int, string> PobierzStanyZestawow()
        {
            string sql = @"  select a.kpl_IdKomplet, 
  STUFF((
    SELECT '; ' +REPLACE(CAST(cast(kpl_Liczba as decimal(14,4)) as varchar(20)) ,'.',',') + '*' + CAST(kpl_IdSkladnik AS VARCHAR(20)) 
    FROM tw_Komplet
    WHERE kpl_IdKomplet = a.kpl_IdKomplet
    FOR XML PATH(''),TYPE).value('.','VARCHAR(MAX)')
  ,1,2,'') AS sklad  
  from tw_Komplet a group by a.kpl_IdKomplet;";
            Dictionary<int, string> wynik = new Dictionary<int, string>();
            using (SqlConnection conn = new SqlConnection(_config.ERPcs))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    using (SqlDataReader rd = cmd.ExecuteReader())
                    {
                        while (rd.Read())
                        {


                            int produkt = DataHelper.dbi("kpl_IdKomplet", rd);
                            string stan = DataHelper.dbs("sklad", rd);
                            wynik.Add(produkt, stan);
                        }
                    }
                }
            }
            return wynik;

        }

        /// <summary>
        /// Wylicza termin dostawy
        /// </summary>
        /// <param name="produkt">Id produktu</param>
        /// <param name="dostawy">Wszystkie terminy dostaw z zamówień do dostawców</param>
        /// <param name="laczneRezerwacje">Lączne rezerwacje produktu</param>
        /// <param name="laczneStany">Łaczny stan produktu</param>
        /// <returns></returns>
        public static DateTime? WyliczTerminDostawy(int produkt, Dictionary<int, List<Tuple<decimal, DateTime>>> dostawy, decimal laczneRezerwacje, decimal laczneStany)
        {
            if (!dostawy.ContainsKey(produkt))
            {
                return null; //koniec produkt nie jest na żadnym zamówieniu
            }
            decimal iloscbrakujaca = laczneRezerwacje - laczneStany;
            if (iloscbrakujaca < 0)
            {
                return null; //na stanie jest więcej niż w rezerwacji
            }
            var dos = dostawy[produkt];
            foreach (var d in dos)
            {
                if (iloscbrakujaca < d.Item1)
                {
                    return d.Item2; //ilosć w zamówieniu jest mniejsza nie brakujace- czyli po tej dacie będą dostępne na stanie
                }
                iloscbrakujaca -= d.Item1;
            }
            return null; //wszystkie z zamówień są zarezerwowane
        }


        internal Dictionary<long, Model.Waluta> PobierzDostepneWaluty()
        {
            var wynik = new Dictionary<long, Model.Waluta>();

            const string sql = @"select [wl_Symbol] FROM [sl_Waluta] WHERE [wl_Aktywna] = 1";
            SqlConnection conn = null;
            SqlCommand cmd = null;
            SqlDataReader r = null;

            try
            {
                conn = new SqlConnection(_config.ERPcs);
                conn.Open();
                cmd = new SqlCommand(sql, conn);
                r = cmd.ExecuteReader();
                while (r.Read())
                {
                    Model.Waluta waluta = new Model.Waluta();
                    waluta.WalutaErp = DataHelper.dbs("wl_Symbol", r).Trim();
                    waluta.WalutaB2b = DataHelper.dbs("wl_Symbol", r);
                    waluta.Id = waluta.WalutaErp.ToLower().WygenerujIDObiektuSHAWersjaLong();
                    if (!wynik.ContainsKey(waluta.Id)) wynik.Add(waluta.Id, waluta);
                }
            } finally
            {
                if (r != null) r.Dispose();
                if (cmd != null) cmd.Dispose();
                if (conn != null)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }

            return wynik;
        }

        internal List<PoziomCenowy> PobierzDostepnePoziomyCen()
        {
            List<PoziomCenowy> prices = new List<PoziomCenowy>();
            var e = DB.vwPoziomyCenWaluties.OrderBy(p => p.IDENT).ToList();
            for (int i = 0; i < 10; ++i)
            {
                vwPoziomyCenWaluty v = e.ElementAtOrDefault(i);
                PoziomCenowy price = new PoziomCenowy();
                if (v != null)
                {
                    price.Id = v.IDENT == null ? 0 : v.IDENT.Value;
                    price.Nazwa = v.NAZWA;
                    price.WalutaId = v.WALUTA.ToLower().WygenerujIDObiektuSHAWersjaLong();
                }
                prices.Add(price);
            }
            return prices.Where(p => !string.IsNullOrEmpty(p.Nazwa)).ToList();
        }

        internal List<CenaPoziomu> GetProductsPriceLevels()
        {
            List<CenaPoziomu> items = new List<CenaPoziomu>();
            if (!CzyWyciagamyProdukty())
            {
                return items;
            }
            List<PoziomCenowy> pc = PobierzDostepnePoziomyCen();
            const string sql = @"select  distinct
       p.tc_CenaBrutto1,
                         p.tc_CenaBrutto2,
                         p.tc_CenaBrutto3,
                         p.tc_CenaBrutto4,
                         p.tc_CenaBrutto5,
                         p.tc_CenaBrutto6,
                         p.tc_CenaBrutto7,
                         p.tc_CenaBrutto8,
                         p.tc_CenaBrutto9,
                         p.tc_CenaBrutto10,
                         p.tc_CenaNetto1,
                         p.tc_CenaNetto2,
                         p.tc_CenaNetto3,
                         p.tc_CenaNetto4,
                         p.tc_CenaNetto5,
                         p.tc_CenaNetto6,
                         p.tc_CenaNetto7,
                         p.tc_CenaNetto8,
                         p.tc_CenaNetto9,
                         p.tc_CenaNetto10,
                         p.tw_Id

 from vwTowar p
                     where p.st_MagId is not null 
                     and  p.tw_zablokowany=0 and p.tw_rodzaj in(1,2,8) 
  and 1=(case when @visible=1 then p.tw_SklepInternet  
  else (case when @visible=2 then p.tw_SerwisAukcyjny  
  else (case when @visible=3 then p.tw_SprzedazMobilna  
  else 1 end) end) end)";
            int visibility = (int) _config.SubiektWidocznoscTowarow;

            SqlConnection conn = null;
            SqlCommand cmd = null;
            SqlDataReader r = null;

            try
            {
                conn = new SqlConnection(_config.ERPcs);

                conn.Open();
                cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@visible", visibility);
                r = cmd.ExecuteReader();


                while (r.Read())
                {
                    foreach (PoziomCenowy p in pc)
                    {
                        decimal netto = DataHelper.dbd("tc_CenaNetto" + p.Id, r);
                        items.Add(new CenaPoziomu(p.Id, Decimal.Round(netto, 2), DataHelper.dbi("tw_Id", r)));
                    }
                }
            } finally
            {
                if (r != null) r.Dispose();
                if (cmd != null) cmd.Dispose();
                if (conn != null)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
            return items;
        }

        public List<KeyValuePair<int, string>> ListaKategoriiZCech(Grupa grupa)
        {
            var c = DB.sl_CechaTws.Where(a => a.ctw_Nazwa.ToLower().Contains(grupa.Parametry.ToLower())).Select(a => new KeyValuePair<int, string>(a.ctw_Id, a.ctw_Nazwa));
            return c.ToList();
        }

        internal List<KategoriaKlienta> GetCustomerCategories()
        {
            var q = DB.sl_GrupaKhs.ToList();
            Dictionary<int, KategoriaKlienta> items = new Dictionary<int, KategoriaKlienta>();
            foreach (var x in q)
            {
                KategoriaKlienta item = new KategoriaKlienta();
                item.Id = x.grk_Id;
                item.Nazwa = x.grk_Nazwa;
                item.Grupa = "Grupa główna";
                items.Add(item.Id, item);
            }
            var platnosci = DB.sl_FormaPlatnoscis;
            foreach (var x in platnosci)
            {
                KategoriaKlienta item = new KategoriaKlienta {Id = platnosci_offset + x.fp_Id, Nazwa = x.fp_Nazwa + ":" + x.fp_Termin.ToString(CultureInfo.InvariantCulture), Grupa = "Platnosci"};
                items.Add(item.Id, item);
            }
            if (_config.SeparatorGrupKlientow.Any())
            {
                var c = (from p in DB.sl_CechaKhs
                         //  where p.ckh_Nazwa.ToLower().Trim().Contains(separator.ToLower().Trim())
                         select new {p.ckh_Nazwa, p.ckh_Id}).ToList();

                foreach (var v in c)
                {
                    if (KategorieKlientowHelper.SprawdzCzyPoprawnySeparator(v.ckh_Nazwa, _config.SeparatorGrupKlientow))
                    {
                        KategoriaKlienta item = new KategoriaKlienta();
                        item.Id = kategorie_cechy_offset + v.ckh_Id;

                        int a = v.ckh_Nazwa.IndexOfAny(_config.SeparatorGrupKlientow);
                        if (a > 0)
                        {
                            item.Nazwa = v.ckh_Nazwa.Substring(a + 1).Trim();
                            item.Grupa = v.ckh_Nazwa.ToLower().Substring(0, a).Trim();
                        }
                        else
                        {
                            item.Nazwa = v.ckh_Nazwa;
                        }
                        items.Add(item.Id, item);
                    }
                    else
                    {
                        LogiFormatki.PobierzInstancje.LogujInfo($"Cecha o nazwie: {v.ckh_Nazwa} zawiera więcej niż jeden separator z listy: {_config.SeparatorGrupKlientow.ToCsv()}. " +
                                                                "Zmień ustawienia separatorów lub cechę.");
                    }
                }
            }

            var polaKlientow = PobierzPolaWlasneKlientow();
            foreach (var nameValueCollection in polaKlientow)
            {
                foreach (var pole in nameValueCollection.Value)
                {
                    string nazwa = pole.Value;
                    if (string.IsNullOrWhiteSpace(nazwa))
                    {
                        continue;
                    }
                    string grupa = pole.Key;
                    KategoriaKlienta kk = StworzKategorieKlientow(nazwa, grupa);
                    if (!items.ContainsKey(kk.Id))
                    {
                        items.Add(kk.Id, kk);
                    }
                }
            }
            StworzKategorieZRabatow(ref items);
            return items.Values.ToList();
        }

        private KategoriaKlienta StworzKategorieKlientow(String nazwa, string grupa)
        {
            KategoriaKlienta kk = new KategoriaKlienta {Grupa = grupa, Nazwa = nazwa.ToLower()};
            kk.Id = kk.WygenerujIDObiektu();
            return kk;
        }

        internal List<Cecha> GetAttributes(out List<Atrybut> listaAtrybutow)
        {
            string cechaAuto = _config.CechaAuto;
            char[] separat = _config.SeparatorAtrybutowWCechach;
            bool atrybutZCechy = _config.AtrybutZCechy;
            Dictionary<int, Atrybut> atrybuty = new Dictionary<int, Atrybut>();
            List<Cecha> items = new List<Cecha>();
            var kp = DB.sl_KrajPochodzenias.ToList();
            string nazwaKrajPochodzenia = "Kraj pochodzenia";
            Atrybut kpatrybut = new Atrybut(nazwaKrajPochodzenia);
            kpatrybut.Id = nazwaKrajPochodzenia.WygenerujIDObiektu();
            atrybuty.Add(kpatrybut.Id, kpatrybut);
            foreach (var k in kp)
            {
                Cecha item = new Cecha(k.krp_Nazwa, k.krp_Kod);
                item.Id = k.krp_Id + krajPochodzeniaPrzesuniecie;
                item.AtrybutId = kpatrybut.Id;
                item.Widoczna = true;
                items.Add(item);
            }

            var q = (from p in DB.sl_CechaTws select new {p.ctw_Id, p.ctw_Nazwa}).ToList();

            foreach (var v in q)
            {
                string symbol = v.ctw_Nazwa;
                Cecha item = new Cecha(v.ctw_Nazwa, symbol);
                item.Id = v.ctw_Id;
                item.Widoczna = true;
                string nazwaCechy = item.Nazwa; //.Split(new string[]{separator}, StringSplitOptions.RemoveEmptyEntries).Last();
                Atrybut tmp = null;
                if (atrybutZCechy)
                {
                    tmp = AtrybutyWyszukiwanie.PobierzInstancje.WyciagnijAtrybutZCechy(ref nazwaCechy, ref symbol, separat, cechaAuto);
                    //log.Debug($"Stworzenie atrybutu ID: [{tmp.Id}] dla cechy o nazwie: [{nazwaCechy}].");
                }
                else
                {
                    log.Info("Wyłaczone ustawienie wyciagania atrybutow z cechy");
                }

                if (tmp != null)
                {
                    if (!atrybuty.ContainsKey(tmp.Id))
                    {
                        atrybuty.Add(tmp.Id, tmp);
                    }
                    item.AtrybutId = tmp.Id;
                    item.Nazwa = nazwaCechy;
                }
                item.Symbol = symbol;
                if (string.IsNullOrEmpty(item.Nazwa))
                {
                    item.Nazwa = item.Symbol;
                }
                item.Nazwa = item.Nazwa;
                if (item.AtrybutId == null) continue;
                item.Symbol = item.Symbol.Trim().ToLower();
                if (CzyCechaJuzJestNaLiscie(items, item, symbol)) continue;
                items.Add(item);
            }

            string[] fields = _config.PolaWlasneCechy;


            SqlConnection conn = null;
            SqlCommand cmd = null;
            SqlDataReader rd = null;
            Dictionary<int, Dictionary<string, string>> productsFields = PobierzPolaWlasne(conn, cmd, rd);
            try
            {
                foreach (KeyValuePair<int, Dictionary<string, string>> nameValueCollection in productsFields)
                {
                    foreach (string pwnaceche in fields)
                    {
                        if (nameValueCollection.Value.ContainsKey(pwnaceche))
                        {
                            var pole = nameValueCollection.Value[pwnaceche];

                            if (string.IsNullOrEmpty(pole)) continue;

                            string symbol = (pwnaceche + ":" + pole).Trim().ToLower();
                            var cecha = items.FirstOrDefault(p => p.Symbol.Trim().ToLower() == symbol);
                            var atrybut = new Atrybut(pwnaceche);
                            atrybut.Id = pwnaceche.WygenerujIDObiektu();
                            if (!atrybuty.ContainsKey(atrybut.Id))
                            {
                                atrybuty.Add(atrybut.Id, atrybut);
                            }

                            if (cecha == null)
                            {
                                Cecha c = new Cecha(pole, symbol) {AtrybutId = atrybut.Id, Widoczna = true};
                                c.Id = c.WygenerujIDObiektu();
                                items.Add(c);
                            }
                        }
                    }
                }

            } catch (Exception ex)
            {
                log.Error(new Exception("Błąd przy pobieraniu pól własnych produktów: ", ex));
            }

            try
            {
                var grupy = PobierzGrupy();
                foreach (var v in grupy)
                {
                    Cecha item = new Cecha();
                    item.Id = grupyoffset + v.Key;

                    char separator = v.Value.Contains("_") ? '_' : ':';
                    string[] obiekty = v.Value.Replace("/", "\\").Split(separator);
                    item.Nazwa = obiekty.Last();
                    string symbol = ((obiekty.Length == 1 ? _config.AtrybutKategoriiZERP : obiekty.First()) + separator + obiekty.Last()).ToLower();
                    string nazwaatrybutu;
                    if (obiekty.Length == 1)
                    {
                        nazwaatrybutu = _config.AtrybutKategoriiZERP + separator + item.Nazwa;
                    }
                    else nazwaatrybutu = v.Value;

                    Atrybut tmp = atrybutZCechy ? AtrybutyWyszukiwanie.PobierzInstancje.WyciagnijAtrybutZCechy(ref nazwaatrybutu, ref symbol, separat, cechaAuto) : null;

                    if (tmp != null)
                    {
                        if (!atrybuty.ContainsKey(tmp.Id))
                        {
                            atrybuty.Add(tmp.Id, tmp);
                        }
                        item.AtrybutId = tmp.Id;
                    }
                    item.Symbol = symbol;
                    if (CzyCechaJuzJestNaLiscie(items, item, symbol)) continue;

                    items.Add(item);
                }
            } catch (Exception ex)
            {
                log.Error("Nie udało się pobrać grup z powodu błędu: " + ex.Message, ex);
            }


            var w = PobierzProducentow();
            foreach (var v in w)
            {
                Model.Cecha item = new Model.Cecha();
                item.Id = v.Key;
                item.Nazwa = v.Value;
                string symbol = _config.AtrybutProducentaZERP + ":" + item.Nazwa.ToLower();
                item.Widoczna = true;
                string nazwaatrybutu = symbol;

                Atrybut tmp = atrybutZCechy ? AtrybutyWyszukiwanie.PobierzInstancje.WyciagnijAtrybutZCechy(ref nazwaatrybutu, ref symbol, separat, cechaAuto) : null;

                if (tmp != null)
                {
                    if (!atrybuty.ContainsKey(tmp.Id))
                    {
                        atrybuty.Add(tmp.Id, tmp);
                    }
                    item.AtrybutId = tmp.Id;
                }
                item.Symbol = symbol;
                if (CzyCechaJuzJestNaLiscie(items, item, symbol)) continue;
                items.Add(item);
            }

            StworzCechyZPromocji(ref atrybuty, ref items);

            listaAtrybutow = atrybuty.Values.ToList();

           
            return items;
        }


        private void StworzCechyZPromocji(ref Dictionary<int, Atrybut> atrybuty, ref List<Cecha> items)
        {
            int idAtrybutu = nazwaAtrybutuRabatu.WygenerujIDObiektu();
            Atrybut at = new Atrybut(nazwaAtrybutuRabatu, idAtrybutu);
            if (!atrybuty.ContainsKey(idAtrybutu))
            {
                atrybuty.Add(idAtrybutu, at);
            }

            using (DbConnection conn = new SqlConnection(_config.ERPcs))
            {
                conn.Open();

                using (DbCommand command = new SqlCommand(sqlPromocje))
                {
                    command.Connection = conn;
                    using (DbDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = DataHelper.dbi("pc_Id", reader) + rabat_offset_CechyProduktu;
                            string nazwa = DataHelper.dbs("pc_Nazwa", reader);

                            string nazwaKat = nazwa + "_" + id;
                            
                            string symbol = $"{nazwaAtrybutuRabatu}{_config.SeparatorAtrybutowWCechach[0]}{nazwaKat}";
                            items.Add(new Cecha(nazwaKat, symbol) { Id = id, AtrybutId = at.Id});
                        }
                    }
                }
                conn.Close();
            }
        }

        private static bool CzyCechaJuzJestNaLiscie(List<Cecha> items, Cecha item, string symbol)
        {
            if (items.Any(x => x.Symbol.Equals(item.Symbol, StringComparison.InvariantCultureIgnoreCase)))
            {
                log.Error("Pomijanie cechy, już istnieje" + symbol);
                return true;
            }
            return false;
        }

        private Dictionary<int, string> PobierzProducentow()
        {

            var w =
            (DB.kh__Kontrahents.Where(a => a.tw__Towars1.Count > 0)
                .Join(DB.vwKliencis, post => post.kh_Id, meta => meta.kh_Id, (post, meta) => new {IDProducenta = dostawcy_offset + post.kh_Id, Nazwa = meta.adr_Nazwa, Typ = meta.kh_Typ})
                .Where(a => a.Typ == 1)).ToDictionary(a => a.IDProducenta, a => a.Nazwa);

            return w;
        }

        private Dictionary<int, int> PobierzProducentowLaczniki()
        {
            //TODO przerobić to na normalny LINQ - ja póki co nie wiem jak to zrobić
            var w = (from p in DB.tw__Towars join k in DB.vwKliencis on p.tw_IdProducenta equals k.kh_Id where k.kh_Typ == 1 select new KeyValuePair<int, int>(p.tw_Id, dostawcy_offset + k.kh_Id)).ToDictionary(a => a.Key, a => a.Value);
            return w;
        }

        private string CreateMM(List<ZamowienieProdukt> suBraki, string mag, string docelowy, string nr, string uwagi = "")
        {
            List<Produkt> produkty = new List<Produkt>(suBraki.Count);

            foreach (ZamowienieProdukt item in suBraki)
            {
                Produkt p = new Produkt();
                p.Id = (int) item.ProduktId;
                p.Ilosc = item.Ilosc;
                p.PodstJmiary = item.Jednostka;
                produkty.Add(p);
                //    log.Info(string.Format("Produkt: {0}, Ilość: {1}", p.Id, p.Ilosc));
            }
            Subiekt sub = GetSubiekt(_config.ERPLogin, _config.ERPHaslo, _config.ERPcs);
            //  log.Info(string.Format("Tworzenie MM, z : {0} - {2}, do {1} - {3}, uwagi: {4}", mag, docelowy, Magazyny.PobierzIdMagazynu(docelowy), Magazyny.PobierzIdMagazynu(mag), uwagi));
            string nrMM = ERP.SubiektGT.Dokumenty.DodajDokument(sub, SubiektGTDokumentEnum.gtaSubiektDokumentMM, Magazyny.PobierzIdMagazynu(docelowy), produkty, nr, Magazyny.PobierzIdMagazynu(mag), uwagi, true, null);
            return nrMM;
        }

        private long PobierzPlatnika(long klient)
        {
            string sqldopobraniasql = @"SELECT top 1 sqlPlatnik = dkp_PlaOdbSql FROM dok_Parametr where dkp_Typ=16 and cast(dkp_PlaOdbSql as varchar(maX)) <>''";
            SqlConnection conn = null;
            SqlCommand cmd = null;
            SqlDataReader rd = null;
            long? idplatnika = null;
            try
            {
                conn = new SqlConnection(_config.ERPcs);
                conn.Open();
                cmd = new SqlCommand(sqldopobraniasql, conn);
                string zapytanie = (string) cmd.ExecuteScalar();
                if (!string.IsNullOrEmpty(zapytanie))
                {
                    cmd = new SqlCommand(zapytanie.Replace("%1", "@klient"), conn);
                    cmd.Parameters.AddWithValue("@klient", klient);
                    
                    rd = cmd.ExecuteReader();
                    while (rd.Read())
                    {
                        if (rd.IsDBNull(0))
                        {
                            break;
                        }
                        // idplatnika = rd.GetInt64(0);

                        int wartosc = rd.GetInt32(0);
                        idplatnika = wartosc;
                       
                        log.Debug($"Wartość płatnika {idplatnika}");

                        break;
                    }
                }
            } catch (Exception ex)
            {
                log.Error("ustawienie płatnika", ex);
            } finally
            {
                if (rd != null)
                {
                    rd.Close();
                    rd.Dispose();
                }
                if (cmd != null) cmd.Dispose();
                if (conn != null)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
            return idplatnika.GetValueOrDefault(klient);
        }

        private int PobierzIdMagazynu(string symbol = null)
        {
            sl_Magazyn m;
            if (!string.IsNullOrEmpty(symbol))
            {
                m = DB.sl_Magazyns.FirstOrDefault(p => symbol == p.mag_Symbol);
            }
            else
            {
                m = DB.sl_Magazyns.First(p => p.mag_Glowny);
            }

            if (m == null && !string.IsNullOrEmpty(symbol))
            {
                throw new Exception("W subiekcie nie znaleziono magazynu o symbolu " + symbol);
            }
            return m.mag_Id;
        }

        public string przesunMazazyn(List<ZamowienieProdukt> suBraki, string mag, string docelowy, string nr, string uwagi = "")
        {
            return CreateMM(suBraki, mag, docelowy, nr, uwagi);
        }

        internal List<ZamowienieSynchronizacja> SetOrders(List<ZamowienieSynchronizacja> list, Dictionary<long, Klient> wszyscy)
        {
            List<ZamowienieSynchronizacja> docs = new List<ZamowienieSynchronizacja>();
            var jednostki = ApiWywolanie.PobierzJednostki();
            var waluty = PobierzDostepneWaluty();
            bool liczonyOdNetto = _config.LiczonyOdCenyNetto;
            Subiekt subiekt;

            try
            {
                subiekt = GetSubiekt(_config.ERPLogin, _config.ERPHaslo, _config.ERPcs);
            }
            catch (Exception ex)
            {
                foreach (ZamowienieSynchronizacja t in list)
                {
                    t.StatusId = StatusImportuZamowieniaDoErp.Błąd;
                    t.BladKomunikat = ex.Message;
                    docs.Add(t);
                }
                throw new SaveOrderException("Inicjalizacja sfery " + ex.Message, null, ex);
            }

            for (int j = 0; j < list.Count; j++)
            {
                ZamowienieSynchronizacja o = list[j];

                SuDokument dokument2 = null;
                try
                {
                    log.Debug("Rozpoczynanie importu zamówienia o ID " + o.NumerZPlatformy + " ilość pozycji: " + o.pozycje.Count);

                    //sprawdzanie czy klient jest w subiekcie
                    Kontrahent knt = subiekt.Kontrahenci.Wczytaj(o.KlientId);
                    if (knt == null || !knt.Aktywny)
                    {
                        o.StatusId = StatusImportuZamowieniaDoErp.Błąd;
                        o.BladKomunikat = "Klient nie znaleziony lub nieaktywny. Pomijanie zamówienia B2B nr " + o.NumerZPlatformy;
                        log.Error($"Klient: {knt?.Symbol} nie znaleziony lub nieaktywny. Pomijanie zamówienia B2B nr {o.NumerZPlatformy}");
                        continue;
                    }

                    //ustawiamy magazyn realizujący
                    int idMagazynuRealizujacego = PobierzIdMagazynu(o.MagazynRealizujacy);
                    subiekt.MagazynId = idMagazynuRealizujacego;

                    if (o.IdZamowieniaDoPolaczenia.HasValue)
                    {
                        try
                        {
                            dokument2 = subiekt.SuDokumentyManager.WczytajDokument(o.IdZamowieniaDoPolaczenia);
                        }
                        catch (Exception e)
                        {
                            log.Error($"Błąd wczytywania dokumentu o id: {o.IdZamowieniaDoPolaczenia} bład {e.Message}. Nie mogę połaczyć zamówienia. Tworze nowe zamówienie.");
                        }
                    }
                    //jesli dokument2 jest null czyli albo nie ma id do połaczneia albo był błąd przy próbie wczytania zamówienia dlatego tworze nowe zamówienie
                    if (dokument2 == null)
                    {
                        if (o.IdZamowieniaDoPolaczenia.HasValue)
                        {
                            log.Error($"Zamówienie miało być połączone z zamoówieniem o id:{o.IdZamowieniaDoPolaczenia}, ale nie można zaczytać tego zamówienia może zostało usunięte.");
                            log.Error("Tworze nowe zamówienie.");
                        }
                        DodajNowyDokument(ref dokument2, wszyscy, subiekt, o, liczonyOdNetto, waluty);
                    }
                    
                    List<ZamowienieProdukt> braki = new List<ZamowienieProdukt>();

                    List<ZamowienieProdukt> pozycjePosortowane = GrupujPoMM(o);

                    //DodajePozycje do dokumentu
                    DodajPozycjeDoDokumentu(pozycjePosortowane, subiekt, dokument2, jednostki, liczonyOdNetto, idMagazynuRealizujacego, o.MagazynRealizujacy, o.MagazynDlaMm, braki);

                    log.Info("Dodano wszystkie pozycje...");

                    dokument2.StatusDokumentu = (SubiektDokumentStatusEnum) _config.SubiektStatusDokumentu;
                    dokument2.Rezerwacja = _config.ZamowieniaTworzRezerwacje;

                    //modyfikujemy uwagi dokumnetu i zmieniamy ewentualną flagę.
                    if (o.IdZamowieniaDoPolaczenia.HasValue)
                    {
                        try
                        {
                            dokument2 = o.ZdarzeniePrzetworzZamowienie(dokument2, dokument2.Uwagi,o);
                        }
                        catch (Exception e)
                        {
                            log.Error($"wystąpił bład przy przetwarzaniu zamówienia po połączeniu: {e.Message}");
                        }
                    }

                    log.Info($"Zaczynam zapis w Subiekt - pozycji w dokumencie: {dokument2.Pozycje.Liczba}");
                    try
                    {
                        dokument2.Zapisz();
                    }catch(Exception e)
                    {
                        log.Error(e);
                        dokument2.Wyswietl();
                        throw;
                    }

                    o.StatusId = StatusImportuZamowieniaDoErp.Zaimportowane; //_config.StatusyZamowien.Values.First(p => p.Symbol == "Zapisane").Id;
                    o.ListaDokumentowZamowienia = new List<ZamowienieDokumenty>() {new ZamowienieDokumenty(o.Id, dokument2.Identyfikator, dokument2.NumerPelny)};
                    if (braki.Count > 0)
                    {
                        CreateMM(braki, o.MagazynDlaMm, o.MagazynRealizujacy, dokument2.NumerPelny);
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                    o.StatusId = StatusImportuZamowieniaDoErp.Błąd; //_config.StatusyZamowien.Values.First(p => p.Symbol == "Błąd").Id;
                    o.BladKomunikat = ex.Message;
                }
                finally
                {
                    if (dokument2 != null)
                    {
                        log.Debug("Zakończono import zamówienia o ID " + o.NumerZPlatformy + " Zamowienie ma numer " + dokument2.NumerPelny);
                        dokument2.Zamknij();
                    }
                }
                docs.Add(o);
                log.Info("Koniec zamówienia");
            }
            return docs;
        }

        /// <summary>
        /// Tworzymy nowy dokument
        /// </summary>
        /// <param name="wszyscy"></param>
        /// <param name="dokument2"></param>
        /// <param name="subiekt"></param>
        /// <param name="o"></param>
        /// <param name="liczonyOdNetto"></param>
        /// <param name="waluty"></param>
        /// <returns></returns>
        private void DodajNowyDokument(ref SuDokument dokument2, Dictionary<long, Klient> wszyscy,  Subiekt subiekt, 
            ZamowienieSynchronizacja o, bool liczonyOdNetto, Dictionary<long, Waluta> waluty)
        {
            dokument2 = subiekt.SuDokumentyManager.DodajZK();
            dokument2.KontrahentId = PobierzPlatnika(o.KlientId);
            dokument2.OdbiorcaId = o.KlientId;
            Klient k = wszyscy[o.KlientId];
            if (o.TerminDostawy != null)
            {
                dokument2.TerminRealizacji = o.TerminDostawy;
            }
            dokument2.LiczonyOdCenNetto = liczonyOdNetto;
            dokument2.DoDokumentuNumerPelny = o.NumerZPlatformy;
            dokument2.NumerOryginalny = o.NumerZPlatformy;
            int? idKategorii = GetCategoryId(o.KategoriaZamowienia);
            if (idKategorii.HasValue)
            {
                dokument2.KategoriaId = idKategorii;
            }
            string desc = (o.Uwagi ?? "").UsunFormatowanieHTML();
            try
            {
                dokument2.Uwagi = (desc.Length > 490) ? desc.Substring(0, 490) : desc;
            }
            catch
            {
                //log.ErrorFormat($"Błąd zapisania uwag. Próbowano wpisać uwagi: {desc}");
                //throw;
                //zamiast wywalania uwag tylko info ze uwagi nieczytelne
                dokument2.Uwagi = "Nie udało się odczytać uwag z B2B - sprawdz uwagi na SOLEX B2B";
            }

            dokument2.Tytul = "Zamówienie z platformy B2B";
            dokument2.Podtytul = o.Podtytul;
            if (k.KlientEu)
            {
                dokument2.TransakcjaRodzajOperacjiVat = 2;
                if (_config.SubiektKodTransakcjiDlaKlientowEU > -1)
                {
                    dokument2.TransakcjaKod = _config.SubiektKodTransakcjiDlaKlientowEU;
                }
            }
            else if (!k.KlientEu && k.Eksport)
            {
                dokument2.TransakcjaRodzajOperacjiVat = 1;
            }
            if (o.PoziomCenyId != null) dokument2.PoziomCenyId = o.PoziomCenyId.Value;

            //BARTEK zmienił ze domyślnie jest PLN - będzie problem jak ktoś nie ma domyślnie PLN, ale takich to chyba nie ma?
            if (o.WalutaId != null && waluty[(long) o.WalutaId].WalutaErp.ToUpper() != "PLN")
            {
                decimal kurs = 1;
                DateTime? dataKursu = null;
                var x = DB.sl_WalutaKurs.OrderByDescending(p => p.wk_Data).FirstOrDefault(p => p.wk_Symbol == waluty[(long) o.WalutaId].WalutaErp && p.wk_Data != DateTime.Now.Date);
                if (x != null)
                {
                    //if (_config.RodzajKursuWalut == 2)
                    //    kurs = x.wk_Sprzedaz;
                    //else
                    //    if (_config.RodzajKursuWalut == 3)
                    //        kurs = x.wk_Zakup;
                    //    else
                    log.DebugFormat($"Kurs {x.wk_Symbol} z dnia {x.wk_Data} wynosi: {x.wk_Sredni}");
                    kurs = x.wk_Sredni;
                    dataKursu = x.wk_Data;
                }
                dokument2.WalutaSymbol = waluty[(long) o.WalutaId].WalutaErp;
                dokument2.KursCeny = kurs; //kurs > kurs0 ? 1 / kurs : kurs0;
                dokument2.WalutaKurs = kurs;

                //do tego by poprawnie można było zrealizować fakturę w walucie nalezy uzupełnić poniższe rzeczy 
                int idBankuKursu = DB.ExecuteQuery<int>("SELECT TOP 1 wbn_ID FROM sl_WalutaBank WHERE [wbn_Podstawowy] = 1").FirstOrDefault();
                dokument2.KursCenyDataKursu = dataKursu;
                dokument2.KursCenyTabelaBanku = idBankuKursu;
                dokument2.WalutaDataKursu = dataKursu;
                dokument2.WalutaTabelaBanku = idBankuKursu;
            }
        }

        /// <summary>
        /// Dodajemy pozycje do importowanego dokumentu
        /// </summary>
        /// <param name="pozycjePosortowane"></param>
        /// <param name="subiekt"></param>
        /// <param name="dokument2"></param>
        /// <param name="jednostki"></param>
        /// <param name="liczonyOdNetto"></param>
        /// <param name="magazynDlaMm"></param>
        /// <param name="idMagazynuRealizujacego"></param>
        /// <param name="braki"></param>
        /// <param name="magazynRealizujacy"></param>
        private void DodajPozycjeDoDokumentu(List<ZamowienieProdukt> pozycjePosortowane, Subiekt subiekt, SuDokument dokument2, Dictionary<long, Jednostka> jednostki, bool liczonyOdNetto, int idMagazynuRealizujacego, string magazynRealizujacy, string magazynDlaMm, List<ZamowienieProdukt> braki)
        {
            dokument2.AutoPrzeliczanie = false;

            foreach (ZamowienieProdukt i in pozycjePosortowane) //o.pozycje.OrderBy(p => p.id))
            {
                SuPozycja powiazany = null;
                log.Debug($"Wczytywanie towaru o id: {i.ProduktIdBazowy}");
                
                SuPozycja pozycja2 = (SuPozycja) dokument2.Pozycje.Dodaj(i.ProduktIdBazowy);
                if (pozycja2.TowarId != i.ProduktIdBazowy) //produkt miał towar poziwązany, rzeczywisty produkt jest na przedostatniej pozycji
                {
                    powiazany = pozycja2;
                    pozycja2 = dokument2.Pozycje[dokument2.Pozycje.Liczba - 1];
                }

                pozycja2.Jm = jednostki[i.JednostkaMiary].Nazwa;
                pozycja2.IloscJm = i.Ilosc;
                pozycja2.Opis = i.Opis;
                if (i.CenaNetto != -1)
                {
                    if (liczonyOdNetto)
                    {
                        if (pozycja2.CenaNettoPrzedRabatem == 0)
                        {
                            pozycja2.CenaNettoPrzedRabatem = i.CenaNetto;
                        }
                        pozycja2.CenaNettoPoRabacie = i.CenaNetto;
                    }
                    else
                    {
                        if (pozycja2.CenaBruttoPrzedRabatem == 0)
                        {
                            pozycja2.CenaBruttoPrzedRabatem = i.CenaBrutto;
                        }
                        pozycja2.CenaBruttoPoRabacie = i.CenaBrutto;
                    }
                }
                
                if (powiazany != null)
                {
                    if ( (powiazany.TowarNazwa as string).ToLower().Contains("kaucja") || (powiazany.TowarSymbol as string).ToLower().Contains("kaucja"))
                    {
                        //na kaucje rabat 0
                        powiazany.RabatProcent = 0;
                        powiazany.Opis = "Kaucja";
                    }
                    else
                    {
                        //rabat tylko jesli NIE jest kaucja
                        powiazany.RabatProcent = pozycja2.RabatProcent;
                    }
                }

                if (!string.IsNullOrEmpty(magazynDlaMm) && !string.IsNullOrEmpty(magazynRealizujacy) && magazynRealizujacy != magazynDlaMm)
                {
                    decimal iloscWymagana = (i.Ilosc*i.JednostkaPrzelicznik);
                    decimal magazynPodstawowyId = DB.sl_Magazyns.First(p => magazynDlaMm == p.mag_Symbol).mag_Id;
                    var v = DB.vwTowars.Select(p => new {p.tw_Id, p.st_MagId, p.tw_Rodzaj, p.Stan}).FirstOrDefault(p => p.tw_Id == i.ProduktId && p.st_MagId == idMagazynuRealizujacego);
                    var v2 = DB.vwTowars.Select(p => new {p.tw_Id, p.st_MagId, p.tw_Rodzaj, p.Stan}).FirstOrDefault(p => p.tw_Id == i.ProduktId && p.st_MagId == magazynPodstawowyId);
                    var t = v == null ? 0 : v.Stan.GetValueOrDefault();
                    var iloscWPodstawowym = v2 == null ? 0 : v2.Stan.GetValueOrDefault();
                    if (v.tw_Rodzaj == 2) t = 10000;
                    if (t < iloscWymagana && iloscWymagana <= iloscWPodstawowym)
                    {
                        braki.Add(i);
                    }
                }
            }

            dokument2.Przelicz();
        }

        private static List<ZamowienieProdukt> GrupujPoMM(ZamowienieSynchronizacja o)
        {
            List<ZamowienieProdukt> pozycjePosortowane = new List<ZamowienieProdukt>();
            List<ZamowienieProdukt> zMm = new List<ZamowienieProdukt>();
            List<ZamowienieProdukt> bezMm = new List<ZamowienieProdukt>();

            if (o.pozycje == null || !o.pozycje.Any())
            {
                log.ErrorFormat($"Brak pozycji dla zamówienia: {o.NumerZPlatformy}.");
                return null;
            }
            //  log.InfoFormat("Ilosc pozycji:{0}", o.pozycje.Count);
            foreach (ZamowienieProdukt i in o.pozycje.OrderBy(p => p.Id))
            {
                if (!string.IsNullOrEmpty(i.Opis) && i.Opis.Contains(" MM_"))
                {
                    zMm.Add(i);
                }
                else
                {
                    bezMm.Add(i);
                }
            }

            //  log.InfoFormat("Ilosc pozycji zmm:{0}, ilosc bezMM:{1}", zMm.Count, bezMm.Count);
            if (zMm.Count > 0)
            {
                pozycjePosortowane.AddRange(zMm);
            }
            if (bezMm.Count > 0)
            {
                pozycjePosortowane.AddRange(bezMm);
            }

            return pozycjePosortowane;
        }

        private int? GetCategoryId(string kategoria)
        {

            var x = DB.sl_Kategorias.FirstOrDefault(p => p.kat_Nazwa == kategoria);
            return (x != null) ? x.kat_Id : (int?) null;
        }

        public Subiekt GetSubiekt(string agentName, string agentPassword, string cs)
        {
            if (sub == null)
            {
                SprawdzanieSubiekta();
                try
                {
                    Polaczenie.UstawParametryPolaczenia(agentName, agentPassword, cs, _config.SubiektPodmiot, _config.SubiektSzyfrujHaslo);
                    sub = Polaczenie.GetSubiekt;
                } catch (COMException ex)
                {
                    uint kod = (uint) ex.ErrorCode;
                    switch (kod)
                    {
                        case 2147750699:
                            throw new SaveOrderException($"Nie poprawne dane użyte do logowania, aktualne login {agentName} hasło {agentPassword} ");
                        case 2147750498:
                            throw new SaveOrderException("Licencja sfery wygasła");
                        case 2147750386:
                            throw new SaveOrderException("Brak ważnej licencji Sfery i/lub stowarzyszonego produktu w podmiocie.");
                        case 2147750590:
                            throw new SaveOrderException("Zostało osiągnięte ograniczenie licencji Sfery na liczbę stanowisk.");
                        case 2147750646:
                            throw new SaveOrderException("Osiągnięty limit aktywnych pracowników. Rozszerz licencję.");
                        case 2147750869:
                            throw new SaveOrderException("Nie można zalogować wybranego użytkownika, ponieważ został przekroczony limit wykupionych licencji.");
                        default:
                            throw;
                    }
                } catch (Exception ex)
                {
                    throw new SaveOrderException("Inicjalizacja sfery: " + ex.Message + $" Dane użyte do łączenia: {agentName}, {agentPassword}", null, ex);
                }
            }
            return sub;
        }
        private void SprawdzanieSubiekta()
        {
            var processes = Process.GetProcessesByName("Subiekt");
            foreach (var process in processes)
            {
                if((DateTime.Now - process.StartTime).Minutes > 5)
                {
                    using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT CommandLine FROM Win32_Process WHERE ProcessId = " + process.Id))
                    using (ManagementObjectCollection objects = searcher.Get())
                    {
                        string commandLine = objects.Cast<ManagementBaseObject>().SingleOrDefault()?["CommandLine"]?.ToString();
                        if (commandLine != null && commandLine.Contains(_config.ERPLogin))
                        {
                            process.Kill();
                            log.Info($"Proces o id został zatrzymany: {process.Id}");
                        }
                    }
                }
            }
        }
        internal void CleanUp()
        {
            try
            {
                _db = null;
                Polaczenie.KillSubiekt();
                sub = null;
            } catch
            {}
        }

        internal List<Rabat> PobierzRabaty(Dictionary<long, Klient> dlaKogoliczyc)
        {

            List<Rabat> items = new List<Rabat>();

            using (DbConnection conn = new SqlConnection(_config.ERPcs))
            {
                conn.Open();

                using (DbCommand command = new SqlCommand("select pc_Id, pc_TwGrId, pc_KhGrId, pc_CenyPoziom, pc_Od,pc_do, pc_Rabat,pc_WyborKh, pc_WyborTw from dok_Promocja where   isnull( pc_Nieaktywna,0) = 0 AND  ( pc_Do is null or pc_Do >= GetDate() or pc_OgraniczonaCzasowo=0) "))
                {
                    command.Connection = conn;
                    using (DbDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int price_Level = DataHelper.dbi("pc_CenyPoziom", reader);
                            int? customerCategory = DataHelper.dbin("pc_KhGrId", reader);
                            int? productCategory = DataHelper.dbin("pc_TwGrId", reader);
                            long? cechyProduktu = null;
                            DateTime? dateTo = DataHelper.dbdtn("pc_do", reader);
                            DateTime? dateFrom = DataHelper.dbdtn("pc_Od", reader);
                            decimal discount = DataHelper.dbd("pc_Rabat", reader);

                            if (dateTo > DateTime.Now.AddYears(2))
                            {
                                dateTo = null;
                            }

                            bool wybraneProdukty = DataHelper.dbb("pc_WyborTw", reader);
                            bool wyborKh = DataHelper.dbb("pc_WyborKh", reader);

                            if (productCategory.HasValue)
                            {
                                productCategory += grupyoffset;
                            }
                            else if (wybraneProdukty)
                            {
                                cechyProduktu = DataHelper.dbi("pc_Id", reader) + rabat_offset_CechyProduktu;
                            }
                            if (!customerCategory.HasValue && wyborKh)
                            {
                                customerCategory = DataHelper.dbi("pc_Id", reader) + rabat_offset_KategorieKlienta;
                            }

                            Rabat d = new Rabat();
                            d.Aktywny = true;
                            d.KategoriaKlientowId = customerCategory;
                            d.CechaId = cechyProduktu;
                            d.KategoriaProduktowId = productCategory;
                            d.OdKiedy = dateFrom;
                            d.DoKiedy = dateTo;
                            d.TypRabatu = RabatTyp.Zaawansowany;
                            d.TypWartosci = RabatSposob.Procentowy;
                            d.Wartosc1 = d.Wartosc2 = d.Wartosc3 = discount;
                            d.Wartosc4 = d.Wartosc5 = discount;
                            d.PoziomCenyId = price_Level;
                            items.Add(d);
                        }
                    }
                }
                conn.Close();
            }


            log.InfoFormat($"Koniec promocje {DateTime.Now}. Wyciągnietych promocji: {items.Count}");

            return items;
        






    }

        internal Rejestracja CreateCustomers(Rejestracja items)
        {

            string symbol = "b2b_" + items.Id;
            Subiekt subiekt;
            try
            {
                subiekt = GetSubiekt(_config.ERPLogin, _config.ERPHaslo, _config.ERPcs);
            } catch (Exception ex)
            {
                throw new Exception("Inicjalizacja sfery, " + ex.Message);
            }
            string shortname = items.Nazwa.Length > 20 ? items.Nazwa.Substring(0, 20) : items.Nazwa;
            Kontrahent k;

            if (!string.IsNullOrEmpty(items.Nip) && SolEx.ERP.SubiektGT.Kontrahenci.IstniejeNIP(items.Nip))
            {
                int id = ERP.SubiektGT.Kontrahenci.PobierzID(items.Nip).GetValueOrDefault(0);
                k = subiekt.KontrahenciManager.WczytajKontrahenta(id);
                symbol = k.Symbol.ToString();
            }
            else
            {
                if (ERP.SubiektGT.Kontrahenci.IstniejeMail(items.Email))
                {
                    throw new Exception($"Istnieje kontrahent o mailu: {items.Email}");
                }
                while (ERP.SubiektGT.Kontrahenci.IstniejeSymbol(symbol))
                {
                    symbol += "@";
                }
                k = subiekt.KontrahenciManager.DodajKontrahenta();
                k.NazwaPelna = items.Nazwa;
                k.Nazwa = shortname;
                k.KodPocztowy = items.KodPocztowy;
                k.Ulica = items.Ulica;
                k.Miejscowosc = items.Miasto;
                k.Typ = KontrahentTypEnum.gtaKontrahentTypOdbiorca;
                if (!string.IsNullOrEmpty(items.Telefon))
                {
                    KhTelefon t = (KhTelefon) k.Telefony.Dodaj(items.Telefon);
                    t.Numer = items.Telefon;
                    t.Podstawowy = true;
                    t.Rodzaj = false;
                    t.Typ = TelefonTypEnum.gtaTelefonTypSluzbowy;
                }
                //      k.Panstwo = items.Panstwo;
                k.NIP = items.Nip;
                k.OsobaImie = items.ImieNazwisko;
                k.Symbol = symbol;
            }
            k.Email = items.Email;
            k.PoleWlasne[_config.PoleHasloPobierz("", new Dictionary<string, string>())] = string.IsNullOrEmpty(items.HasloJednorazowe) ? new Random().Next(1000, 999999).ToString(CultureInfo.InvariantCulture) : items.HasloJednorazowe;

            k.Zapisz();
            int klient_Id = (int) k.Identyfikator;
            k.Zamknij();

            int cechaId = ERP.SubiektGT.Kontrahenci.GetIDCecha("b2b-nowy-klient");
            ERP.SubiektGT.Kontrahenci.DodajCeche(klient_Id, cechaId);
            items.StatusEksportu = RegisterExportStatus.Exported;
            items.KlientId = klient_Id;
            return items;
        }



        internal List<int> GetDocumentToDelete(Dictionary<int, long> dokumentyNaPlatformie, HashSet<long> idKlientowB2b)
        {

            Dictionary<int, long> dokumentyNaPlatformie2 = new Dictionary<int, long>(dokumentyNaPlatformie);
            string sql = @"
select  dok_id,dok_PlatnikId from(
select 
 r.dok_Id,
 r.dok_DataWyst,
                      r.dok_NrPelny,
                      typ=2,
                      dokument_powiazany= dp.dok_NrPelny,
                      r.dok_DoDokId,
                      dok_Status= case when r.dok_Status=8 then 1 else 0 end,
                      r.dok_PlatnikId,
                      r.dok_Waluta,
                      dok_WalutaKurs =ISNULL( r.dok_WalutaKurs ,1),
                      r.dok_PlatTermin,
                      wartosc_netto =  CAST(case when (r.dok_WalutaKurs <> 1 and r.dok_WalutaKurs <> 0) then r.dok_WartNetto / r.dok_WalutaKurs else  r.dok_WartNetto end as decimal(15,2)),
                      wartosc_brutto =  CAST(case when (r.dok_WalutaKurs <> 1 and r.dok_WalutaKurs <> 0) then r.dok_WartBrutto / r.dok_WalutaKurs else  r.dok_WartBrutto end as decimal(15,2)),
                      nzf_WartoscWaluta= CAST(case when (r.dok_WalutaKurs <> 1 and r.dok_WalutaKurs <> 0) then r.dok_WartNetto / r.dok_WalutaKurs else  r.dok_WartNetto end as decimal(15,2)),
                      r.dok_Uwagi,
                       platnosc ='',
                      rezerwacja = r.dok_JestRuchMag
   ,opoznienie=0
 from dok__Dokument r left join dok__Dokument dp on r.dok_DoDokId=dp.dok_Id where r.dok_Typ in (16,15) and r.dok_PlatnikId is not null
 and (r.dok_Status = 8 or r.dok_Status = 7 or r.dok_Status = 6)
union
 select
                    r.dok_Id,
                      r.dok_DataWyst,
                      r.dok_NrPelny,
                      typ=1,
                      dokument_powiazany = dp.dok_NrPelny,
                      r.dok_DoDokId,
                      dok_Status=0,
                      r.dok_PlatnikId,
                      r.dok_Waluta,
                      dok_WalutaKurs = isnull(r.dok_WalutaKurs, 1),
                      r.dok_PlatTermin,
                      wartosc_netto =  CAST(case when (r.dok_WalutaKurs <> 1 and r.dok_WalutaKurs <> 0) then r.dok_WartNetto / r.dok_WalutaKurs else  r.dok_WartNetto end as decimal(15,2)),
                      wartosc_brutto =  CAST(case when (r.dok_WalutaKurs <> 1 and r.dok_WalutaKurs <> 0) then r.dok_WartBrutto / r.dok_WalutaKurs else  r.dok_WartBrutto end as decimal(15,2)),
                      p.nzf_WartoscWaluta,
                      r.dok_Uwagi,
                      platnosc = isnull(pl.fp_Nazwa,''),
                      rezerwacja = r.dok_JestRuchMag
   , opoznienie=DATEDIFF (day, r.dok_PlatTermin, GETDATE())
 from vwFinanseRozDokumenty p join dok__Dokument r on p.nzf_IdDokumentAuto=r.dok_Id
 left join sl_FormaPlatnosci pl on r.dok_PlatId=pl.fp_Id left join dok__Dokument dp on r.dok_DoDokId=dp.dok_Id
 where (r.dok_Typ = 2 or r.dok_Typ = 6) and r.dok_PlatnikId is not null
 
 union
 select          r.dok_Id,
                      r.dok_DataWyst,
                      r.dok_NrPelny,
                      typ=1,
                      dokument_powiazany = dp.dok_NrPelny,
                      r.dok_DoDokId,
                      r.dok_Status,
                      r.dok_PlatnikId,
                      r.dok_Waluta,
                      dok_WalutaKurs = isnull(r.dok_WalutaKurs, 1),
                      r.dok_PlatTermin,
                      wartosc_netto =  CAST(case when (r.dok_WalutaKurs <> 1 and r.dok_WalutaKurs <> 0) then r.dok_WartNetto / r.dok_WalutaKurs else  r.dok_WartNetto end as decimal(15,2)),
                      wartosc_brutto =  CAST(case when (r.dok_WalutaKurs <> 1 and r.dok_WalutaKurs <> 0) then r.dok_WartBrutto / r.dok_WalutaKurs else  r.dok_WartBrutto end as decimal(15,2)),
                      nzf_WartoscWaluta =0,
                      r.dok_Uwagi,
                      platnosc = isnull(pl.fp_Nazwa,''),
                      rezerwacja = r.dok_JestRuchMag
   ,opoznienie=0
from vwFinanseRozRozliczone p 
                      join dok__Dokument r on p.nzf_IdDokumentAuto=r.dok_Id
 left join sl_FormaPlatnosci pl on r.dok_PlatId=pl.fp_Id 
 left join dok__Dokument dp on r.dok_DoDokId=dp.dok_Id
 where (r.dok_Typ = 2 or r.dok_Typ = 6) and r.dok_PlatnikId is not null) a ";

            log.Debug(string.Format("Początek pobierania dokumentów do usunięcia - {0}", DateTime.Now));
            //StringBuilder sb = new StringBuilder(dokumentyNaPlatformie.Count * 3);
            //foreach (KeyValuePair<int, string> k in dokumentyNaPlatformie)
            //{
            //    sb.AppendFormat("insert into @table values({0});", k.Key.ToStringDoSerializacji());
            //}
            //string sql_all = string.Format(sql, sb.ToString());
            //  log.Error(sql_all);
            //    List<int> result = new List<int>();

            SqlConnection conn = null;
            SqlCommand cmd = null;
            SqlDataReader rd = null;
            try
            {

                conn = new SqlConnection(_config.ERPcs);
                conn.Open();
                cmd = new SqlCommand(sql, conn) {CommandTimeout = 10000};
                rd = cmd.ExecuteReader();
                while (rd.Read())
                {
                    int id = DataHelper.dbi("dok_id", rd);
                    int klient = DataHelper.dbi("dok_PlatnikId", rd);
                    if (dokumentyNaPlatformie2.ContainsKey(id) && idKlientowB2b.Contains(klient)) //dokumenty mogą zostać na b2b tylko wtedy jeśli zgadza się id i płatnik dokumentu
                    {
                        dokumentyNaPlatformie2.Remove(id);
                    }
                    // result.Add(id);
                }
            } finally
            {
                if (conn != null)
                {
                    conn.Close();
                    conn.Dispose();
                }
                if (cmd != null)
                {
                    cmd.Dispose();
                }
                if (rd != null)
                {
                    rd.Dispose();
                }
            }
            log.DebugFormat("Koniec pobierania dokumentów do usunięcia - {0}", DateTime.Now);
            return dokumentyNaPlatformie2.Keys.ToList();
        }


        private string UtworzWarunekDoNazwy(HashSet<string> slowaWymagane, HashSet<string> slowaZakazane)
        {
            string filtrowaniePoNazwie = string.Empty;
            if (slowaWymagane != null && slowaWymagane.Any())
            {
                foreach (var slowo in slowaWymagane)
                {
                    if (string.IsNullOrEmpty(slowo))
                    {
                        continue;
                    }
                    if (string.IsNullOrEmpty(filtrowaniePoNazwie))
                    {
                        filtrowaniePoNazwie += $" and ( r.dok_NrPelny like '%{slowo}%'";
                    }
                    else
                    {
                        filtrowaniePoNazwie += $" or r.dok_NrPelny like '%{slowo}%'";
                    }
                }
                filtrowaniePoNazwie += ")";
            }
            if (slowaZakazane != null && slowaZakazane.Any())
            {
                foreach (var slowo in slowaZakazane)
                {
                    if (string.IsNullOrEmpty(slowo))
                    {
                        continue;
                    }
                    if (string.IsNullOrEmpty(filtrowaniePoNazwie))
                    {
                        filtrowaniePoNazwie += $" and (r.dok_NrPelny not like '%{slowo}%'";
                    }
                    else
                    {
                        filtrowaniePoNazwie += $" and r.dok_NrPelny not like '%{slowo}%'";
                    }
                }
                filtrowaniePoNazwie += ")";
            }
            return filtrowaniePoNazwie;
        }

        internal Dictionary<HistoriaDokumentu, List<HistoriaDokumentuProdukt>> GetDocumentToSend(Dictionary<int, long> dokumentyNaPlatformie, DateTime from, List<Klient> klienci)
        {
            var jednostkiSlownik = APIWywolania.PobierzInstancje.PobierzJednostki().Values.Where(x => x.Aktywna).ToDictionary(x => x.Nazwa, x => x.Id);
            Dictionary<string, Jednostka> jednostkiDoDodania = new Dictionary<string, Jednostka>();
            int dokumentowWPaczce = _config.MaksimumDokumentowWPaczce;
            HashSet<int> doWywaleniaBrakJednostki = new HashSet<int>();

            HashSet<string> slowaWymagane = _config.SlowaWymaganeWDokumencie;
            HashSet<string> slowaZakazane = _config.SlowaZakazaneWDokumencie;

            string filtrowaniePoNazwie = UtworzWarunekDoNazwy(slowaWymagane, slowaZakazane);

            //   LogiFormatki.PobierzInstancje.LogujDebug(string.Format("ilosc {0} ostatni {1}",dokumentyNaPlatformie.Count,dokumentyNaPlatformie.Last().Key));
            if (klienci.Count == 0) return new Dictionary<HistoriaDokumentu, List<HistoriaDokumentuProdukt>>();
            Dictionary<int, HistoriaDokumentu> elems = new Dictionary<int, HistoriaDokumentu>();
            Dictionary<int, List<HistoriaDokumentuProdukt>> subitems = new Dictionary<int, List<HistoriaDokumentuProdukt>>();
            SqlConnection conn = null;
            SqlCommand cmd = null;
            SqlDataReader rd = null;
            //  int dokumentowWPaczce = _config.MaksimumDokumentowWPaczce;
            Dictionary<HistoriaDokumentu, List<HistoriaDokumentuProdukt>> items = new Dictionary<HistoriaDokumentu, List<HistoriaDokumentuProdukt>>();
            HashSet<int> przerobione = new HashSet<int>();
            LogiFormatki.PobierzInstancje.LogujDebug("Dok na b2b: " + dokumentyNaPlatformie.Count);
            try
            {
                conn = new SqlConnection(_config.ERPcs);
                conn.Open();

                cmd = new SqlCommand(string.Format(SELECT_HEADER, Serializacje.PobierzInstancje.SerializeList(klienci.Where(x => x.Aktywny).Select(x => x.Id)), presuniestatuskat, filtrowaniePoNazwie), conn);
                cmd.Parameters.AddWithValue("@from", from);
                cmd.CommandTimeout = 60000;

                //string zapytanie = cmd.CommandText;
                //log.Debug($"Zapytanie sql: {zapytanie}");

                rd = cmd.ExecuteReader();
                int j = 0;
                while (rd.Read())
                {
                    int id = DataHelper.dbi("dok_Id", rd);
                    if (przerobione.Contains(id))
                    {
                        continue;
                    }
                    przerobione.Add(id);
                    long cusid = DataHelper.dbl("dok_PlatnikId", rd);
                    DateTime date = DataHelper.dbdt("dok_DataWyst", rd);
                    string name = DataHelper.dbs("dok_NrPelny", rd);
                    string relatedName = DataHelper.dbs("dokument_powiazany", rd);
                    RodzajDokumentu type = (RodzajDokumentu) DataHelper.dbi("typ", rd);
                    string dok_NrPelnyOryg = DataHelper.dbs("dok_NrPelnyOryg", rd);
                    int? relatedId = DataHelper.dbin("dok_DoDokId", rd);
                    string desc = DataHelper.dbs("dok_Uwagi", rd);
                    decimal unpaid = DataHelper.dbd("nzf_WartoscWaluta", rd);
                    decimal valueNetto = DataHelper.dbd("wartosc_netto", rd);
                    decimal valueBrutto = DataHelper.dbd("wartosc_brutto", rd);
                    string currency = DataHelper.dbs("dok_Waluta", rd);
                    long walutaId = currency.ToLower().WygenerujIDObiektuSHAWersjaLong();
                    string paymentName = DataHelper.dbs("platnosc", rd);
                    int? status = DataHelper.dbin("status", rd);
                    DateTime paymentDate = DataHelper.dbdtn("dok_PlatTermin", rd).GetValueOrDefault(date);
                    bool booked = DataHelper.dbb("rezerwacja", rd);
                    bool paid = (DataHelper.dbi("dok_Status", rd) == 1) || (type == RodzajDokumentu.Faktura && unpaid == 0);
                    int? odbiorca = DataHelper.dbin("dok_OdbiorcaId", rd);
                    decimal pozostalo = valueNetto < 0 ? (unpaid*-1) : unpaid;
                    HistoriaDokumentu d = new HistoriaDokumentu(id, cusid, type, name, date, paid, relatedId, relatedName, desc, pozostalo, valueNetto, valueBrutto, walutaId, paymentName, paymentDate, booked);
                    d.OdbiorcaId = odbiorca;
                    d.NumerObcy = dok_NrPelnyOryg;
                    d.StatusId = status;
                    long hash = Tools.PobierzInstancje.PoliczHashDokumentu(d, out string ciagHasha);
                    
                    if (!dokumentyNaPlatformie.ContainsKey(d.Id) || dokumentyNaPlatformie[d.Id] != hash)
                    {
                        if(date < DateTime.Now.AddMonths(-3) && dokumentyNaPlatformie.ContainsKey(d.Id))
                        {
                            log.Warn($"Zmiana na dokumencie sprzed 3 miesięcy - nie powinno takie cos sie zdarzań normalnie. Dokument id: {id}, hash z b2b: {dokumentyNaPlatformie[d.Id]}, hash nowy: {hash}, zrzut danych dokumentu z erp:\r\n" +
                                $"{d.ToJson()} \r\n" +
                                $"Ciąg hasha:\r\n" +
                                $"{ciagHasha}");
                        }

                        //LogiFormatki.PobierzInstancje.LogujDebug($"Dokument {d.Id} hash {hash} jest {dokumentyNaPlatformie.ContainsKey(d.Id)} hash2 {(dokumentyNaPlatformie.ContainsKey(d.Id) ? dokumentyNaPlatformie[d.Id].ToString() : "")}");
                        if (!elems.ContainsKey(d.Id))
                        {
                            elems.Add(d.Id, d);
                        }
                    }

                    if (j%5000 == 0)
                    {
                        log.DebugFormat("przetwarzanie dokumentu {0}, do wysłania {1}", j, elems.Count);
                    }
                    if (elems.Count > dokumentowWPaczce) break; //przy jednej dużej paczecze max 10k dokumentów(inaczej żre mnóstwo ramu
                    j++;
                }
                log.DebugFormat("przetwarzanie dokumentów koniec {0}, do wysłania {1}", j, elems.Count);
                rd.Dispose();
                cmd.Dispose();
                //items = elems.Values.ToDictionary(x => x, x => new List<HistoriaDokumentuProdukt>());
                HashSet<int> docIDs = new HashSet<int>( elems.Keys);
                if (docIDs.Count == 0) return new Dictionary<HistoriaDokumentu, List<HistoriaDokumentuProdukt>>();
                cmd = new SqlCommand(string.Format(SELECT_ITEMS, Serializacje.PobierzInstancje.SerializeList(docIDs)), conn);
                cmd.CommandTimeout = 600;
                rd = cmd.ExecuteReader();

                while (rd.Read())
                {
                    int obid = DataHelper.dbi("ob_Id", rd);
                    int id = DataHelper.dbi("ob_DokHanId", rd);
                    if (doWywaleniaBrakJednostki.Contains(id))
                    {
                        continue;
                    }
                    string symbol = DataHelper.dbs("tw_Symbol", rd);
                    string name = DataHelper.dbs("tw_Nazwa", rd);
                    decimal quantity = DataHelper.dbd("ilosc", rd, 4);
                    string unit = DataHelper.dbs("ob_Jm", rd).Trim();
                    long jednostkaId = 0;
                    if (!string.IsNullOrEmpty(unit))
                    {
                        if (jednostkiSlownik.ContainsKey(unit))
                        {
                            jednostkaId = jednostkiSlownik[unit];
                        }
                        else
                        {
                            long? jed = jednostkiSlownik.Where(x => x.Key.TrimEnd(".") == unit.TrimEnd(".")).Select(x => x.Value).FirstOrDefault();
                            if (jed != null && jed != 0)
                            {
                                jednostkaId = jed.Value;
                            }
                            else
                            {
                                if (jednostkiDoDodania.ContainsKey(unit))
                                {
                                    jednostkaId = jednostkiDoDodania[unit].Id;
                                }
                                else
                                {
                                    Jednostka jednostka = new Jednostka();
                                    jednostka.Nazwa = unit;
                                    jednostka.JezykId = _config.JezykIDDomyslny;
                                    jednostka.Id = unit.WygenerujIDObiektu();
                                    jednostka.JezykId = _config.JezykIDDomyslny;
                                    jednostkiDoDodania.Add(unit, jednostka);

                                    jednostkaId = jednostka.Id;
                                }
                            }
                        }
                    }
                    else
                    {
                        doWywaleniaBrakJednostki.Add(id);
                        log.Debug($"Dokument o numerze: {elems[id].NazwaDokumentu} nie mozna pobrać takiej pozycji bo nie ma jednostki w systemie o nazwie: [{unit}]");
                        continue;
                    }
                    decimal netto = DataHelper.dbd("netto_przed", rd);
                    decimal brutto = DataHelper.dbd("brutto_przed", rd);
                    decimal priceNetto= netto == 0? DataHelper.dbd("netto_przed_rabatem_powiazany", rd): netto;
                    decimal priceBrutto = brutto == 0? DataHelper.dbd("brutto_przed_rabatem_powiazany", rd): brutto;
                    decimal vat = DataHelper.dbd("ob_VatProc", rd);
                    decimal rabat = DataHelper.dbd("rabat", rd);

                    bool czyLiczymyOdCenyNetto = DataHelper.dbi("odCenyNetto", rd)==1;

                    decimal nettoPoRabacie = 0;//czyLiczymyOdCenyNettoMath.Round(DataHelper.dbd("netto_porabacie", rd),2,MidpointRounding.AwayFromZero);
                    decimal bruttoPoRabacie = 0;

                    decimal wartoscNetto = DataHelper.dbd("wartosc_netto", rd);
                    decimal wartoscBrutto = DataHelper.dbd("wartosc_brutto", rd);


                    //Jest to faktura gdzie zmieniła się tylko kwota
                    if (netto == 0 && quantity == 0)
                    {

                        decimal iloscPowiazana = DataHelper.dbd("ilosc_powiazany", rd, 4);
                        quantity = iloscPowiazana;
                        rabat = 0;
                        priceNetto = nettoPoRabacie = Math.Round(wartoscNetto / iloscPowiazana, 2, MidpointRounding.AwayFromZero); 
                        priceBrutto = bruttoPoRabacie = Math.Round(wartoscBrutto / iloscPowiazana, 2, MidpointRounding.AwayFromZero);
                    }
                    else
                    {

                        if (czyLiczymyOdCenyNetto)
                        {
                            nettoPoRabacie = Math.Round(priceNetto * (1 - (rabat / 100)), 2, MidpointRounding.AwayFromZero);
                            bruttoPoRabacie = Math.Round(nettoPoRabacie * (1 + (vat / 100)), 2, MidpointRounding.AwayFromZero);
                        }
                        else
                        {
                            bruttoPoRabacie = Math.Round(priceBrutto * (1 - (rabat / 100)), 2, MidpointRounding.AwayFromZero);
                            nettoPoRabacie = Math.Round((bruttoPoRabacie * 100) / (100 + vat), 2, MidpointRounding.AwayFromZero);
                        }
                    }

                   

                    int productId = DataHelper.dbi("produkt_id", rd);
                    string opis = DataHelper.dbs("ob_Opis", rd);


                    //decimal bruttoPoRabacie = Math.Round(nettoPoRabacie*(1+(vat/100m)),2);
                    //nettoPoRabacie = Math.Round(nettoPoRabacie, 2);
                    decimal wartoscVat = Math.Round(wartoscNetto*(vat/100m),2, MidpointRounding.AwayFromZero);
                    HistoriaDokumentuProdukt it = new HistoriaDokumentuProdukt()
                    {
                        Id = obid,
                        DokumentId = id,
                        KodProduktu = symbol,
                        NazwaProduktu = name,
                        Ilosc = quantity,
                        JednostkaMiary = jednostkaId,
                        CenaNetto = priceNetto,
                        CenaBrutto = priceBrutto,
                        WartoscNetto = wartoscNetto,
                        WartoscBrutto = wartoscBrutto,
                        Vat = vat,
                        WartoscVat = wartoscVat,
                        Rabat = rabat,
                        ProduktId = productId,
                        Opis = opis,
                        CenaNettoPoRabacie = nettoPoRabacie,
                        CenaBruttoPoRabacie = bruttoPoRabacie,
                        ProduktIdBazowy = productId
                    };
                    if (!subitems.ContainsKey(it.DokumentId))
                    {
                        subitems.Add(it.DokumentId, new List<HistoriaDokumentuProdukt>());
                    }
                    subitems[it.DokumentId].Add(it);
                }
                rd.Dispose();
                cmd.Dispose();

                foreach (var item in elems)
                {
                    if (!doWywaleniaBrakJednostki.Contains(item.Key) && subitems.ContainsKey(item.Key))
                    {
                        items.Add(item.Value, subitems[item.Key]);
                    }
                }
            } finally
            {
                if (conn != null)
                {
                    conn.Close();
                    conn.Dispose();
                }
                if (cmd != null)
                {
                    cmd.Dispose();
                }
                if (rd != null)
                {
                    rd.Dispose();
                }
            }
            if (jednostkiDoDodania.Any())
            {
                LogiFormatki.PobierzInstancje.LogujInfo("Ilość jednostek do dodania: {0}", jednostkiDoDodania.Count);
                ApiWywolanie.AktualizujJednostki(jednostkiDoDodania.Values.ToList());
            }
            return items;
        }

        private const string POLADOKUMENTOW = @"if not exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[solex_pola_dokumentow]') or id = object_id(N'[solex_pola_dokumentow]'))
                exec('create view solex_pola_dokumentow as 
                Select pwd_idobiektu,wartosc,pwp_nazwa from( 
                select pwd_idobiektu,pwd_typobiektu,Wartosc,Pole
                from (select  pwd_idobiektu,pwd_typobiektu
                ,pwd_liczba01 =cast(pwd_liczba01 as varchar(max))
                ,pwd_liczba02=cast(pwd_liczba02 as varchar(max))
                ,pwd_liczba03=cast(pwd_liczba03 as varchar(max))
                ,pwd_liczba04=cast(pwd_liczba04 as varchar(max))
                ,pwd_liczba05=cast(pwd_liczba05 as varchar(max))
                ,pwd_liczba06=cast(pwd_liczba06 as varchar(max))
                ,pwd_liczba07=cast(pwd_liczba07 as varchar(max))
                ,pwd_liczba08=cast(pwd_liczba08 as varchar(max))
                ,pwd_liczba09=cast(pwd_liczba09 as varchar(max))
                ,pwd_liczba10=cast(pwd_liczba10  as varchar(max))
                ,pwd_data01 =cast(pwd_data01 as varchar(max))
                ,pwd_data02=cast(pwd_data02 as varchar(max))
                ,pwd_data03=cast(pwd_data03 as varchar(max))
                ,pwd_data04=cast(pwd_data04 as varchar(max))
                ,pwd_data05=cast(pwd_data05 as varchar(max))
                ,pwd_data06=cast(pwd_data06 as varchar(max))
                ,pwd_data07=cast(pwd_data07 as varchar(max))
                ,pwd_data08=cast(pwd_data08 as varchar(max))
                ,pwd_data09=cast(pwd_data09 as varchar(max))
                ,pwd_data10=cast(pwd_data10  as varchar(max))
                ,pwd_kwota01=cast(pwd_kwota01 as varchar(max))
                ,pwd_kwota02=cast(pwd_kwota02 as varchar(max))
                ,pwd_kwota03=cast(pwd_kwota03 as varchar(max))
                ,pwd_kwota04=cast(pwd_kwota04 as varchar(max))
                ,pwd_kwota05=cast(pwd_kwota05 as varchar(max))
                ,pwd_kwota06=cast(pwd_kwota06 as varchar(max))
                ,pwd_kwota07=cast(pwd_kwota07 as varchar(max))
                ,pwd_kwota08=cast(pwd_kwota08 as varchar(max))
                ,pwd_kwota09=cast(pwd_kwota09 as varchar(max))
                ,pwd_kwota10=cast(pwd_kwota10 as varchar(max))
                ,pwd_Tekst01 =cast(pwd_tekst01  as varchar(max))
                ,pwd_Tekst02 =cast(pwd_tekst02 as varchar(max))
                ,pwd_Tekst03=cast(pwd_tekst03 as varchar(max))
                ,pwd_Tekst04=cast(pwd_tekst04 as varchar(max))
                ,pwd_Tekst05=cast(pwd_tekst05 as varchar(max))
                ,pwd_Tekst06=cast(pwd_tekst06 as varchar(max))
                ,pwd_Tekst07=cast(pwd_tekst07 as varchar(max))
                ,pwd_Tekst08=cast(pwd_tekst08 as varchar(max))
                ,pwd_Tekst09=cast(pwd_tekst09 as varchar(max))
                ,pwd_Tekst10=cast(pwd_tekst10 as varchar(max))
                 from pw_dane )  a
                unpivot
                  (Wartosc for Pole in (pwd_liczba01,pwd_liczba02,pwd_liczba03,pwd_liczba04,pwd_liczba05,pwd_liczba06,pwd_liczba07,pwd_liczba08,pwd_liczba09,pwd_liczba10,
                 pwd_data01,pwd_data02,pwd_data03,pwd_data04,pwd_data05,pwd_data06,pwd_data07,pwd_data08,pwd_data09,pwd_data10,pwd_kwota01,pwd_kwota02,pwd_kwota03,pwd_kwota04,pwd_kwota05,pwd_kwota06,pwd_kwota07,pwd_kwota08,pwd_kwota09,pwd_kwota10,pwd_tekst01,pwd_tekst02,pwd_tekst03,pwd_tekst04,pwd_tekst05,pwd_tekst06,pwd_tekst07,pwd_tekst08,pwd_tekst09,pwd_tekst10)
                   ) as Amount ) a join pw_pole p on a.pole=p.pwp_pole where a.pwd_typobiektu = p.pwp_typobiektu and p.pwp_typobiektu in
                   (-2,-26,-37,-32,-33,-34)
                    ');";

        internal string PobierzPoleDokumentu(int dokumentId, string pole)
        {
            var pola = PobierzPolaDokumentu(new HashSet<int> {dokumentId});

            if (pola.ContainsKey(dokumentId))
            {
                if (pola[dokumentId].ContainsKey(pole))
                {
                    return pola[dokumentId][pole];
                }
            }
            return "";
        }

        internal Dictionary<int, Dictionary<string, string>> PobierzPolaDokumentu(HashSet<int> dokumentyId)
        {
            if (dokumentyId == null || !dokumentyId.Any()) return new Dictionary<int, Dictionary<string, string>>();
            Dictionary<int, Dictionary<string, string>> wynik = new Dictionary<int, Dictionary<string, string>>();
            SqlConnection conn = null;
            SqlCommand cmd = null;
            SqlDataReader rd = null;
            try
            {
                conn = new SqlConnection(_config.ERPcs);
                conn.Open();
                ZbudujPerespektywePolDokumentow(conn);
                cmd = new SqlCommand(string.Format("Select * from solex_pola_dokumentow"), conn);
                rd = cmd.ExecuteReader();
                while (rd.Read())
                {
                    Dictionary<string, string> tmp;
                    int iddokumentu = DataHelper.dbi("pwd_idobiektu", rd);
                    if (!dokumentyId.Contains(iddokumentu))
                    {
                        continue;
                    }
                    if (wynik.ContainsKey(iddokumentu))
                    {
                        tmp = wynik[iddokumentu];
                    }
                    else
                    {
                        tmp = new Dictionary<string, string>();
                        wynik.Add(iddokumentu, tmp);
                    }
                    string wartosc = DataHelper.dbs("wartosc", rd);
                    string klucz = DataHelper.dbs("pwp_nazwa", rd);
                    tmp.Add(klucz, wartosc);
                }
                rd.Close();
                rd.Dispose();
                cmd.Dispose();

                cmd = new SqlCommand(string.Format("Select * from dok__Dokument"), conn);
                rd = cmd.ExecuteReader();
                while (rd.Read())
                {
                    Dictionary<string, string> tmp;
                    int idKlienta = DataHelper.dbi("dok_id", rd);
                    if (!dokumentyId.Contains(idKlienta))
                    {
                        continue;
                    }
                    if (wynik.ContainsKey(idKlienta))
                    {
                        tmp = wynik[idKlienta];
                    }
                    else
                    {
                        tmp = new Dictionary<string, string>();
                        wynik.Add(idKlienta, tmp);
                    }
                    for (int i = 0; i < rd.FieldCount; i++)
                    {
                        if (!rd.IsDBNull(i))
                        {
                            string wartosc = rd[i].ToString();
                            string klucz = rd.GetName(i);
                            tmp.Add(klucz, wartosc);
                        }
                    }
                }
            } finally
            {
                if (conn != null)
                {
                    conn.Close();
                    conn.Dispose();
                }
                if (cmd != null)
                {
                    cmd.Dispose();
                }
                if (rd != null)
                {
                    rd.Close();
                    rd.Dispose();
                }
            }
            return wynik;
        }

        private void ZbudujPerespektywePolDokumentow(SqlConnection conn)
        {
            SqlCommand cmd = new SqlCommand(POLADOKUMENTOW, conn);
            cmd.ExecuteNonQuery();
        }

        internal List<KlientKategoriaKlienta> PobierzKategorieKlientowPolaczenia()
        {
            Dictionary<long, KlientKategoriaKlienta> items = new Dictionary<long, KlientKategoriaKlienta>();

            var grc = DB.kh__Kontrahents.Where(p => p.kh_IdGrupa != null).Select(x => new KlientKategoriaKlienta {KlientId = x.kh_Id, KategoriaKlientaId = x.kh_IdGrupa.GetValueOrDefault()}).ToList();
            grc.ForEach(x => items.Add(x.Id, x));
            var pla = DB.kh__Kontrahents.Where(p => p.kh_IdFormaP != null).Select(x => new KlientKategoriaKlienta {KlientId = x.kh_Id, KategoriaKlientaId = platnosci_offset + x.kh_IdFormaP.GetValueOrDefault()}).ToList();
            pla.ForEach(x => items.Add(x.Id, x));
            bool gr = _config.SeparatorGrupKlientow.Any();
            if (gr)
            {
                var c = (from p in DB.kh_CechaKhs select new {p.ck_IdKhnt, p.ck_IdCecha}).ToList();
                foreach (var v in c)
                {
                    KlientKategoriaKlienta item = new KlientKategoriaKlienta();
                    item.KategoriaKlientaId = kategorie_cechy_offset + v.ck_IdCecha;
                    item.KlientId = v.ck_IdKhnt;
                    if (!items.ContainsKey(item.Id)) items.Add(item.Id, item);
                }
            }

            var polaKlientow = PobierzPolaWlasneKlientow();
            foreach (var nameValueCollection in polaKlientow)
            {
                foreach (var p in nameValueCollection.Value)
                {
                    string nazwa = p.Value;
                    if (string.IsNullOrWhiteSpace(nazwa))
                    {
                        continue;
                    }
                    string grupa = p.Key;
                    KategoriaKlienta kk = StworzKategorieKlientow(nazwa, grupa);
                    KlientKategoriaKlienta item = new KlientKategoriaKlienta(nameValueCollection.Key, kk.Id);
                    if (!items.ContainsKey(item.Id)) items.Add(item.Id, item);
                }
            }
            PobierzPolaczenieZPromocjami(ref items);
            return items.Values.ToList();
        }

        private string sqlPromocje = "select pc_Nazwa, pc_Id from dok_Promocja where isnull( pc_Nieaktywna,0) = 0 AND  (  pc_Do is null or pc_Do >= GetDate() or pc_OgraniczonaCzasowo=0)  ";

        string nazwaAtrybutuRabatu = "Rabat";
        private void StworzKategorieZRabatow(ref Dictionary<int, KategoriaKlienta> items)
        {
            using (DbConnection conn = new SqlConnection(_config.ERPcs))
            {
                conn.Open();

                using (DbCommand command = new SqlCommand(sqlPromocje))
                {
                    command.Connection = conn;
                    using (DbDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string nazwa = DataHelper.dbs("pc_Nazwa", reader);
                            int id = DataHelper.dbi("pc_Id", reader) + rabat_offset_KategorieKlienta;

                            string nazwaKat = nazwa + "_" + id;

                            if (!items.ContainsKey(id))
                            {
                                items.Add(id, new KategoriaKlienta { Grupa = nazwaAtrybutuRabatu, Nazwa = nazwaKat.ToLower(), Id = id });
                            }
                        }
                    }
                }
                conn.Close();
            }
        }

        private void PobierzPolaczenieZPromocjami(ref Dictionary<long, KlientKategoriaKlienta> items)
        {
            using (DbConnection conn = new SqlConnection(_config.ERPcs))
            {
                conn.Open();

                using (DbCommand command = new SqlCommand("select pck_IdPromocja, pck_IdKontrahent from dok_PromocjaKontrahent where pck_IdPromocja in ( select pc_Id from dok_Promocja where isnull( pc_Nieaktywna,0) = 0 AND ( pc_Do is null or pc_Do >= GetDate() or pc_OgraniczonaCzasowo=0)   )"))
                {
                    command.Connection = conn;
                    using (DbDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int idKlienta = DataHelper.dbi("pck_IdKontrahent", reader);
                            int idPromocji = DataHelper.dbi("pck_IdPromocja", reader) + rabat_offset_KategorieKlienta;
                            KlientKategoriaKlienta kk = new KlientKategoriaKlienta(idKlienta, idPromocji);
                            if (!items.ContainsKey(kk.Id))
                            {
                                items.Add(kk.Id, kk);
                            }
                        }
                    }
                }
                conn.Close();
            }
        }

        private bool drukowanieWgCustomowegoSzablonu(SuDokument toPrintDoc, string sciezka)
        {
            if (_config.SqlDoWyciaganiWzorcaWydrukuSubiektBioPlanet.IsNullOrEmpty())
            {
                return false;
            }

            string korekta = toPrintDoc.Typ == 6 ? "1" : "0";

            string sql = _config.SqlDoWyciaganiWzorcaWydrukuSubiektBioPlanet.Replace("@korekta", korekta).Replace("@clientId", toPrintDoc.KontrahentId.ToString());

            log.Debug($"SQL do pobierania wydruku: {sql} ");

            int? konfiguracjaWydruku = DB.ExecuteQuery<int?>(sql).FirstOrDefault();

            if (konfiguracjaWydruku == null)
            {
                return false;
            }

            LogiFormatki.PobierzInstancje.LogujInfo($"Drukowanie faktury z użyciem szablony NIE standardowego, szablon id = {konfiguracjaWydruku.Value} - dokument: {toPrintDoc.NumerPelny.ToString()}, klient id: {toPrintDoc.KontrahentId.ToString()}");
            try
            {
                toPrintDoc.DrukujDoPlikuWgWzorca(konfiguracjaWydruku.Value, sciezka, TypPlikuEnum.gtaTypPlikuPDF);
            }
            catch
            {
                LogiFormatki.PobierzInstancje.LogujInfo($"Błąd druku dokumentu wg. innego szablonu niż standardowy - dokument numer: {toPrintDoc.NumerPelny.ToString()}, wydruk id: {konfiguracjaWydruku.Value}, korekta: {korekta}, klient id: {toPrintDoc.KontrahentId.ToString()}");
                log.Error($"Sql wyciągania szablonu wydruku: {sql} ");
                throw;
            }
            return true;
        }



        internal bool DrukujPDFDokument(int id, string sciezka)
        {
            if (string.IsNullOrEmpty(_config.ERPLogin) || string.IsNullOrEmpty(_config.ERPcs)) throw new Exception("Niepoprawne parametry konfiguracyjne Subiekta");
            Subiekt subiekt;
            try
            {
                subiekt = GetSubiekt(_config.ERPLogin, _config.ERPHaslo, _config.ERPcs);
            } catch (Exception ex)
            {
                throw new SaveOrderException("Inicjalizacja sfery: " + ex.Message, null, ex);
            }

            SuDokument toPrintDoc = (SuDokument) subiekt.Dokumenty.Wczytaj(id);
            if (toPrintDoc != null)
            {
                //jak sie nie uda drukownie customa to idziem do normalnego wydruuku
                if (! this.drukowanieWgCustomowegoSzablonu(toPrintDoc, sciezka) )
                {
                    try
                    {
                        toPrintDoc.DrukujDoPliku(sciezka, TypPlikuEnum.gtaTypPlikuPDF);
                    }
                    catch (COMException ex)
                    {
                        string komunikat = string.Format("Dokument {0} nie poprawny szablon wydruku. Spróbuj wyeksportować pdf bezpośrednio z programu.", toPrintDoc.NumerPelny);
                        if (ex.HResult == 0x800416FB)
                        {
                            komunikat += " Prawdopodobnie niepoprawna czcionka w szablonie wydruku";
                        }
                        throw new Exception(komunikat);
                    }                
                }

            }
            else
            {
                throw new InvalidOperationException(string.Format("Próba drukowania nieistniejącego dokumentu: {0}", id));
            }

            if (File.Exists(sciezka))
            {
                return true;
            }
            string nazwa = Path.GetFileNameWithoutExtension(sciezka);
            if (string.IsNullOrEmpty(nazwa))
            {
                return false;
            }
            string katalog = Path.GetDirectoryName(sciezka);
            if (string.IsNullOrEmpty(katalog))
            {
                return false;
            }
            sciezka = Directory.GetFiles(katalog).FirstOrDefault(x => Path.GetFileNameWithoutExtension(x).StartsWith(nazwa, StringComparison.InvariantCultureIgnoreCase));
            if (sciezka == null)
            {
                return false;
            }
            return true;
        }


        public List<KeyValuePair<int, string>> PobierzGrupy()
        {
            var grupy = DB.sl_GrupaTws.Select(a => new KeyValuePair<int, string>(a.grt_Id, a.grt_Nazwa)).ToList();

            return grupy;
        }

        public int? DodajNowaGrupe(string nazwaGrupy)
        {
            return Grupy.DodajNowaGrupe(nazwaGrupy);
        }



        internal int? DodajNowaCecheDlaKategorii(string nowaKategoria)
        {
            int? idCechy = ERP.SubiektGT.Towary.GetIDCecha(nowaKategoria);
            if (idCechy == null) return ERP.SubiektGT.Towary.DodajNowaCeche(nowaKategoria);

            return idCechy;
        }

        internal List<ProduktyKodyDodatkowe> PobierzAlternatywneKodyKreskowe()
        {
            List<ProduktyKodyDodatkowe> items = new List<ProduktyKodyDodatkowe>();
            if (!CzyWyciagamyProdukty())
            {
                return items;
            }

            const string sql = @"
            SELECT id=1000000+kk.kk_Id,t.tw_id,kod=kk.kk_Kod FROM tw__Towar t join tw_KodKreskowy kk on t.tw_Id=kk.kk_IdTowar
            where t.tw_zablokowany=0 and t.tw_rodzaj in(1,2,8)  and 1=(case when @visible=1 then t.tw_SklepInternet  
            else (case when @visible=2 then t.tw_SerwisAukcyjny   else (case when @visible=3 then t.tw_SprzedazMobilna   else 1 end) end) end)";

            SqlConnection conn = null;
            SqlCommand cmd = null;
            SqlDataReader rd = null;
            try
            {

                conn = new SqlConnection(_config.ERPcs);
                conn.Open();
                int visibility = (int) _config.SubiektWidocznoscTowarow;

                cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@visible", visibility);
                rd = cmd.ExecuteReader();
                while (rd.Read()) //pobieranie  produktów
                {
                    int id = DataHelper.dbi("id", rd);
                    long twid = DataHelper.dbl("tw_id", rd);
                    string kod = DataHelper.dbs("kod", rd);
                    items.Add(new ProduktyKodyDodatkowe() {Id = id, ProduktId = twid, Kod = kod});
                }
            } finally
            {
                if (rd != null)
                {
                    rd.Close();
                    rd.Dispose();
                }
                if (cmd != null) cmd.Dispose();
                if (conn != null)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
            return items;
        }

        private int presuniestatuskat = 10000;

        internal List<StatusZamowienia> PobierzStatusyDokumnetow()
        {
            string sql = "SELECT id=flg_Id,symbol=flg_Text FROM fl__Flagi where flg_IdGrupy in(4,5,8,9)";
            List<StatusZamowienia> items = new List<StatusZamowienia>();
            SqlConnection conn = null;
            SqlCommand cmd = null;
            SqlDataReader rd = null;
            try
            {

                conn = new SqlConnection(_config.ERPcs);
                conn.Open();


                cmd = new SqlCommand(sql, conn);
                rd = cmd.ExecuteReader();
                while (rd.Read()) //pobieranie  produktów
                {
                    int id = DataHelper.dbi("id", rd);

                    string symbol = DataHelper.dbs("symbol", rd);
                    items.Add(new StatusZamowienia {Id = id, Symbol = symbol});
                }
                rd.Close();
                rd.Dispose();
                cmd.Dispose();
                cmd = new SqlCommand("SELECT kat_Id,  kat_Nazwa FROM sl_Kategoria", conn);
                rd = cmd.ExecuteReader();
                while (rd.Read()) //pobieranie  produktów
                {
                    int id = DataHelper.dbi("kat_Id", rd) + presuniestatuskat;

                    string symbol = DataHelper.dbs("kat_Nazwa", rd);
                    items.Add(new StatusZamowienia {Id = id, Symbol = symbol});
                }
            } finally
            {
                if (rd != null)
                {
                    rd.Close();
                    rd.Dispose();
                }
                if (cmd != null) cmd.Dispose();
                if (conn != null)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
            return items;
        }

        internal string WystawWz(ZamowienieSynchronizacja zamowienie, int dlakogo, string dlakogoSymbol, string magazy, TypDokumentu typ, out string braki)
        {
            CleanUp();
            Subiekt subiekt = GetSubiekt(_config.ERPLogin, _config.ERPHaslo, _config.ERPcs);
            List<Produkt> produkty = new List<Produkt>(zamowienie.pozycje.Count);

            foreach (ZamowienieProdukt item in zamowienie.pozycje)
            {
                Produkt p = new Produkt();
                p.Id = (int) item.ProduktId;
                p.Ilosc = item.Ilosc;
                p.PodstJmiary = item.Jednostka;
                p.Symbol = item.ProduktId.ToString();
                produkty.Add(p);
            }
            if (dlakogo == 0)
            {
                dlakogo = ERP.SubiektGT.Kontrahenci.PobierzIDPoSymbolu(dlakogoSymbol);
            }
            SubiektGTDokumentEnum rodzaj;
            switch (typ)
            {
                case TypDokumentu.Pz:
                    rodzaj = SubiektGTDokumentEnum.gtaSubiektDokumentPZ;
                    break;
                case TypDokumentu.Wz:
                    rodzaj = SubiektGTDokumentEnum.gtaSubiektDokumentWZ;
                    break;
                default:
                    throw new Exception("Nieobsługiwany typ dokumenu");
            }
            return Dokumenty.DodajDokument(subiekt, rodzaj, Magazyny.PobierzIdMagazynu(magazy), produkty, zamowienie.NumerZPlatformy, dlakogo, "", false, null, out braki, true);

        }

        internal List<Kraje> PobierzKraje()
        {
            const string @select = "SELECT pa_id,pa_Nazwa,pa_KodPanstwaUE FROM sl_Panstwo";

            List<Kraje> items = new List<Kraje>();
            SqlConnection conn = null;
            SqlCommand cmd = null;
            SqlDataReader rd = null;
            try
            {

                conn = new SqlConnection(_config.ERPcs);
                conn.Open();


                cmd = new SqlCommand(select, conn);
                rd = cmd.ExecuteReader();
                while (rd.Read()) //pobieranie  produktów
                {
                    int paId = DataHelper.dbi("pa_id", rd);
                    string paKod = DataHelper.dbs("pa_KodPanstwaUE", rd);
                    string paNazwa = DataHelper.dbs("pa_Nazwa", rd);
                    items.Add(new Kraje(paId, paNazwa, paKod));
                }
            } finally
            {
                if (rd != null)
                {
                    rd.Close();
                    rd.Dispose();
                }
                if (cmd != null) cmd.Dispose();
                if (conn != null)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
            return items;
        }

        internal List<Magazyn> PobierzMagazyny()
        {
            List<Magazyn> magazyny = new List<Magazyn>();
            using (DbConnection conn = new SqlConnection(_config.ERPcs))
            {
                conn.Open();

                using (DbCommand command = new SqlCommand("select mag_Id, mag_Nazwa, mag_Symbol from sl_Magazyn;")) //pobieramy symbole magazynow - to tylko potrzebne
                {
                    command.Connection = conn;
                    using (DbDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string symbolMagazynu = DataHelper.dbs("mag_Symbol", reader);
                            int idMagazynu = DataHelper.dbi("mag_Id", reader);
                            string nazwaMagazynu = DataHelper.dbs("mag_Nazwa", reader);
                            if (!string.IsNullOrEmpty(symbolMagazynu))
                            {
                                magazyny.Add(new Magazyn() {Id = idMagazynu, Symbol = symbolMagazynu, Nazwa = nazwaMagazynu});
                            }
                        }
                    }
                }
                conn.Close();
            }
            return magazyny;
        }

        internal Dictionary<long, decimal> PobierzStanyProduktow(int idMagazynu, bool minusRezerwacje)
        {
            string stantmp = " st_Stan " + (minusRezerwacje ? "- st_StanRez  " : "");
            string sql = $"select st_towId towId, {stantmp} as stan from tw_Stan where st_MagId = {idMagazynu}";
            Dictionary<long, decimal> slownikStanow = new Dictionary<long, decimal>();
            SqlConnection conn = null;
            SqlCommand cmd = null;
            SqlDataReader rd = null;
            try
            {
                conn = new SqlConnection(_config.ERPcs);
                conn.Open();
                cmd = new SqlCommand(sql, conn);
                rd = cmd.ExecuteReader();
                while (rd.Read()) //pobieranie  produktów
                {
                    int tmp = DataHelper.dbi("towId", rd);
                    long idProduktu = DataHelper.dbl("towId", rd);
                    decimal stan = DataHelper.dbd("stan", rd);
                    if(!slownikStanow.ContainsKey(idProduktu)) { slownikStanow.Add(idProduktu, stan);}
                }
            }
            finally
            {
                if (rd != null)
                {
                    rd.Close();
                    rd.Dispose();
                }
                cmd?.Dispose();
                if (conn != null)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
            return slownikStanow;
        }

        internal List<Region> PobierzRegiony()
        {
            string select = "SELECT woj_Id,woj_Nazwa,kraj=(select top 1 pa_Id from sl_Panstwo where pa_Nazwa='Polska')  FROM sl_Wojewodztwo w ";

            List<Region> items = new List<Region>();
            SqlConnection conn = null;
            SqlCommand cmd = null;
            SqlDataReader rd = null;
            try
            {
                conn = new SqlConnection(_config.ERPcs);
                conn.Open();
                cmd = new SqlCommand(select, conn);
                rd = cmd.ExecuteReader();
                while (rd.Read()) //pobieranie  produktów
                {
                    int woj_Id = DataHelper.dbi("woj_Id", rd);
                    int kraj = DataHelper.dbi("kraj", rd);
                    string woj_Nazwa = DataHelper.dbs("woj_Nazwa", rd);
                    items.Add(new Region(woj_Id, woj_Nazwa, kraj));
                }
            } finally
            {
                if (rd != null)
                {
                    rd.Close();
                    rd.Dispose();
                }
                if (cmd != null) cmd.Dispose();
                if (conn != null)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
            return items;
        }

        public void UsunFlage(int idObiektu, int grupaFlagi)
        {
            string sqlDelete = $"DELETE FROM fl_Wartosc WHERE flw_IdObiektu = {idObiektu} AND flw_IdGrupyFlag = {grupaFlagi}";
            SqlConnection conn = null;
            try
            {
                conn = new SqlConnection(_config.ERPcs);
                conn.Open();

                using (SqlCommand command = new SqlCommand(sqlDelete))
                {
                    command.Connection = conn;
                    command.ExecuteNonQuery();
                }
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
        }

        public void DodajFlageDlaDokumentu(int idDokumentu, int idFlagi, int grupaFlagi)
        {
            //ZK = 8, MM, WZ = 12, ZD = 9,
            string sqlInsert = $"INSERT INTO fl_Wartosc (flw_IdGrupyFlag, flw_TypObiektu, flw_IdObiektu, flw_IdFlagi, flw_Komentarz, flw_IdUzytkownika, flw_CzasOstatniejZmiany) VALUES ( {grupaFlagi}, 0, {idDokumentu}, {idFlagi}, 'Automatycznie SOLEX', 1, '{DateTime.Now}' )";
            
            SqlConnection conn = null;
            try
            {
                conn = new SqlConnection(_config.ERPcs);
                conn.Open();

                using (SqlCommand command = new SqlCommand(sqlInsert))
                {
                    command.Connection = conn;
                    command.ExecuteNonQuery();
                }
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
        }

        public void ZmienFlageDlaDokumentu(int idDokumentu, int idFlagi, int grupaFlagi)
        {
            string sqlUpdate = $"UPDATE fl_Wartosc SET flw_IdFlagi = {idFlagi}, flw_CzasOstatniejZmiany = {DateTime.Now.ToString("yyyy-MM-dd")} WHERE flw_IdObiektu = {idDokumentu} AND flw_IdGrupyFlag = {grupaFlagi}";
            log.Debug("sqlUpdate: "+ sqlUpdate);

            SqlConnection conn = null;
            try
            {
                conn = new SqlConnection(_config.ERPcs);
                conn.Open();

                using (SqlCommand command = new SqlCommand(sqlUpdate))
                {
                    command.Connection = conn;
                    command.ExecuteNonQuery();
                }
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
        }


        public int PobierzIdFlagi(string nazwa, int grupaFlagi)
        {
            string sqlSelect = $"SELECT [flg_Id] FROM [fl__Flagi] WHERE [flg_Text] like '{nazwa}' AND flg_IdGrupy = {grupaFlagi}";
            int wynik=0;
            SqlConnection conn = null;
            SqlDataReader rd=null;
            try
            {
                conn = new SqlConnection(_config.ERPcs);
                conn.Open();

                using (SqlCommand command = new SqlCommand(sqlSelect))
                {
                    command.Connection = conn;
                    rd = command.ExecuteReader();
                    while (rd.Read()) //pobieranie  produktów
                    {
                        wynik = DataHelper.dbi("flg_Id", rd);
                    }
                }
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
            return wynik;
        }

        public bool SprawdzWpis(int dokIdentyfikator, int idFlagi, int grupa)
        {
            string sqlSelect = $"SELECT top 1 *  FROM [fl_Wartosc] WHERE flw_IdObiektu = {dokIdentyfikator} AND flw_IdGrupyFlag = {grupa}";
            bool jestWpis = false;
            SqlConnection conn = null;
            SqlDataReader rd = null;
            try
            {
                conn = new SqlConnection(_config.ERPcs);
                conn.Open();

                using (SqlCommand command = new SqlCommand(sqlSelect))
                {
                    command.Connection = conn;
                    rd = command.ExecuteReader();
                    while (rd.Read()) //pobieranie  produktów
                    {
                        jestWpis = DataHelper.dbi("flw_IdGrupyFlag", rd)!=0;
                    }
                }
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
            return jestWpis;
        }
    }
}
