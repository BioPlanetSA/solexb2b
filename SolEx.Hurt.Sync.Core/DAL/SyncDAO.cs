using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SolEx.Hurt.Model;
using SolEx.Hurt.Sync.Core.Configuration;
using System.Drawing;
using System.IO;
using System.Net;
using System.Xml;
using System.Configuration;
using System.Security.Cryptography;
using System.Xml.Linq;
using SolEx.Hurt.Core.Configuration;
using System.Data.SqlClient;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Core.Helpers;
using SolEx.Hurt.DAL;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Model.CustomSearchCriteria;
using ServiceStack.OrmLite;

namespace SolEx.Hurt.Sync.Core.DAL
{
    public class SyncDAO
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        string wrong_pics = "";
        string ss = "";
        private int packageSize = 0;
        private int PackageSize { 
            get 
            { 
                if (packageSize == 0) 
                    packageSize = Config.Settings.GetSettingInt("sync_package_size", 100);

                return packageSize;
            } 
        }

        public void CleanUp()
        {
        }

        internal void SynchronizePayments(List<Model.platnosci> items)
        {
            if (items == null || items.Count < 1)
                return;

          log.Info("start - platnosci");

            SqlCommand cmd = null;
            SqlConnection conn = null;
            StringBuilder sb = null;

            try
            {
                conn = new SqlConnection(Config.MainCS);
                conn.Open();

                List<int> ids = items.Select(w => w.zrodlo_id).ToList<int>();

                cmd = new SqlCommand(string.Format("delete from platnosci where zrodlo_id > 0 and zrodlo_id not in ({0})"
                    , ids == null || ids.Count == 0 ? "0" : Model.Helpers.Serializacje.SerializeList(ids, ',')), conn);
                cmd.ExecuteNonQuery();

                sb = new StringBuilder(500);
                foreach (var v in items)
                {
                    sb.AppendFormat(@" if exists(select * from platnosci where zrodlo_id = {0})
                        update platnosci set zrodlo_id = {0}, nazwa = {1}, termin = {2} where zrodlo_id = {0} 
                        else insert into platnosci(zrodlo_id, nazwa, termin) values({0}, {1}, {2}) ", v.zrodlo_id, v.nazwa.ToStringDoSerializacji(), v.termin.ToStringDoSerializacji());
                }
                if (!string.IsNullOrEmpty(sb.ToString()))
                {
                    cmd = new SqlCommand(sb.ToString(), conn);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
              log.Error( "synchronizowanie płatności",ex);
            }
            finally { if (cmd != null) cmd.Dispose(); if (conn != null) { conn.Close(); conn.Dispose(); } }       
        }

        public  void SynchronizeCustomers(List<klienci> items)
        {
            log.InfoFormat("ilośc klientów do synchronizacji: {0}", items.Count.ToString());
            //Sync.log.Error(string.Format("ilośc klientów do synchronizacji: {0}", items.Count.ToString()));
            int package=100;
            int package_nr=0;
            if (items == null || items.Count < 1)
                return;

            SqlCommand cmd = null;
            SqlConnection conn = null;


            try
            {

                // pobranie klientow
                SolEx.Hurt.Model.CustomSearchCriteria.CustomerSearchCriteria criteria = new SolEx.Hurt.Model.CustomSearchCriteria.CustomerSearchCriteria();
                criteria.AddtionalSQL = " and zrodlo_id>0 ";
                List<klienci> items_b2b = CustomerDAO.Get(criteria);
             
                // deaktywowanie klientow
                List<int> ids = items.Where(p => !string.IsNullOrEmpty(p.email) && !string.IsNullOrEmpty(p.PasswordUncoded) && p.aktywny).Select(p => p.zrodlo_id).ToList();
                log.Info(string.Format("Update klienci set aktywny = 0 where zrodlo_id > 0 and zrodlo_id not in ({0})", ids.Count == 0 ? "0" : Model.Helpers.Serializacje.SerializeList(ids, ',')));
                conn = new SqlConnection(Config.MainCS);
                conn.Open();
                cmd = new SqlCommand(string.Format("Update klienci set aktywny = 0 where zrodlo_id > 0 and zrodlo_id not in ({0})", ids.Count == 0 ? "0" : Model.Helpers.Serializacje.SerializeList(ids, ',')), conn);
                cmd.CommandTimeout = Config.Settings.TimeoutLong;
                cmd.ExecuteNonQuery();
                klienci domyslnyOpiekun = SolEx.Hurt.Core.CoreManager.GetDomyslnyOpiekun();
                klienci domyslnyDrugiOpiekun = SolEx.Hurt.Core.CoreManager.GetDomyslnyDrugiOpiekun();
                klienci domyslnyPrzedstawiciel = SolEx.Hurt.Core.CoreManager.GetDomyslnyPrzedstawiciel();
                List<Model.platnosci> payments = PlatnosciDAL.Get(new PlatnosciSearchCriteria());
                List<klienci> packagecust = new List<klienci>();
                do
                {

                    packagecust = items.Skip(package * package_nr).Take(package).ToList();
                    package_nr++;
                    for (int i = 0; i < packagecust.Count; i++)
                    {
                        if (packagecust[i].opiekun_id == null && domyslnyOpiekun != null)
                        {
                            packagecust[i].opiekun_id = domyslnyOpiekun.klient_id;
                        }
                        if (packagecust[i].przedstawiciel_id == null && domyslnyPrzedstawiciel != null)
                        {
                            packagecust[i].przedstawiciel_id = domyslnyPrzedstawiciel.klient_id;
                        }
                        if (packagecust[i].drugi_opiekun == null && domyslnyDrugiOpiekun != null)
                        {
                            packagecust[i].drugi_opiekun = domyslnyDrugiOpiekun.klient_id;
                        }
                        if (packagecust[i].platnosc_id != null)
                        {
                            packagecust[i].platnosc_id = payments.First(p => p.zrodlo_id == packagecust[i].platnosc_id).platnosc_id;
                        }
                    }
                    List<klienci> updated = SolEx.Hurt.Core.CoreManager.UpdateCustomers(packagecust);

                    List<int> customerSrcID = packagecust.Select(p => p.zrodlo_id).ToList();
                    SolEx.Hurt.Model.CustomSearchCriteria.AdressSearchCriteria criteriaAdresy = new SolEx.Hurt.Model.CustomSearchCriteria.AdressSearchCriteria();
                    criteriaAdresy.AddtionalSQL = string.Format(" and zrodlo_id>0 and klient_id in (select klient_id from klienci where zrodlo_id in ({0}) ) ", SolEx.Hurt.Model.Helpers.Serializacje.SerializeList(customerSrcID, ','));
                    List<Model.adresy> existsIDDB = AdressDAO.Get(criteriaAdresy);
                    List<Model.adresy> toUpdate = new List<Model.adresy>();
                    List<Model.adresy> toDelete = new List<Model.adresy>();

                    for (int i = 0; i < packagecust.Count; i++)
                    {

                        klienci fromDb = updated.First(p => p.zrodlo_id == packagecust[i].zrodlo_id);//obiekt klienta pobrany z bazy
                        List<Model.adresy> adresFromDB = existsIDDB.Where(p => p.klient_id == fromDb.klient_id).ToList();//adresy danego klienta pobrane z bazy;
                        for (int j = 0; j < packagecust[i].adresy.Count; j++)
                        {
                            packagecust[i].adresy[j].klient_id = fromDb.klient_id;
                            toUpdate.Add(packagecust[i].adresy[j]);
                        }
                        toDelete.AddRange(adresFromDB.Where(p => !packagecust[i].adresy.Select(x => x.zrodlo_id).Contains(p.zrodlo_id)));
                    }
                    AdressDAO.UpdateAdress(toUpdate);
                    AdressDAO.Delete(toDelete);

                }
                while (packagecust.Count == package);
            }
            catch (Exception ex) {log.Error( "Usługa - synchronizacja klientów",ex); }
            //odświeżanie statystyk klientów
            CustomerDAO.PrzeliczenieStatystykWszystkichKlientow();

            // mappowanie kategorii klientów
            if (items.Where(p=>p.aktywny && !string.IsNullOrEmpty(p.email) && !string.IsNullOrEmpty(p.PasswordUncoded)).Any(p => p.Categories != null && p.Categories.Count > 0))
            {
                StringBuilder sb = new StringBuilder(1000);

                foreach (var v in items.Where(p => p.Categories != null && p.Categories.Count > 0 && 
                    p.aktywny && !string.IsNullOrEmpty(p.email) && !string.IsNullOrEmpty(p.haslo_docelowe)))
                {
             
                    sb.AppendFormat(@" set @categoryId = null set @customerId = null
                                       select top 1 @customerId = klient_id from klienci where zrodlo_id = {0}
                                       delete from klienci_kategorie where klient_id = @customerId and kategoria_id in (select kategoria_id from kategorie where grupa_id in (select id from grupy where typ_id=2 and zrodlo_id<>0) and zrodlo_id<>0 ) "
                       , v.klient_id);
                    foreach (var cc in v.Categories)
                    {
                       // log.Error( "kz", cc.ToString(),new Exception(v.nazwa));
                        sb.AppendFormat(@" select top 1 @categoryId = kategoria_id from kategorie where zrodlo_id = {0} and grupa_id in (select id from grupy where typ_id=2)
                            if @categoryId is not null and @customerId is not null and not exists(select * from klienci_kategorie where kategoria_id = @categoryId and klient_id = @customerId)
                            insert into klienci_kategorie(kategoria_id, klient_id) values(@categoryId, @customerId) ", cc.ToStringDoSerializacji());
                    }
                }

                string sql = sb.ToString();
                sb = null;
                if (!string.IsNullOrEmpty(sql))
                {
                 //  SolEx.Hurt.Sync.log.Error(sql);

                    var sqlcmd= new SqlCommand("declare @categoryId int, @customerId int; " + sql, conn);
                    sqlcmd.CommandTimeout = Config.Settings.TimeoutLong;
                    sqlcmd.ExecuteNonQuery();
                }
            }

            List<int> idsa = items.Where(q => q.Categories == null || q.Categories.Count == 0 &&
                    q.aktywny && !string.IsNullOrEmpty(q.email) && !string.IsNullOrEmpty(q.haslo_docelowe)).Select(q => q.klient_id).ToList<int>();
            cmd = new SqlCommand(string.Format("delete kk from klienci_kategorie kk join klienci k on k.klient_id = kk.klient_id where {0}"
                , idsa.Count == 0 ? "1=0" : " k.zrodlo_id in ("+ Model.Helpers.Serializacje.SerializeList(idsa, ',') + ")"), conn);
            cmd.CommandTimeout = Config.Settings.TimeoutLong;
            cmd.ExecuteNonQuery();
        }

       
        private bool GetBool(string par)
        {
            if (par == null)
                return false;

            return par.Trim() == "1";
        }

        internal void SynchronizeDiscounts(List<DiscountItem> items)
        {
            if (items == null || items.Count < 1)
                return;

          log.Info( "start - rabaty");
          MainDataContext db = MainDAO.GetContext();
            try
            {
              //log.Error( "przed usuwaniem",new Exception(DateTime.Now.ToLongTimeString()));
                var q = db.rabaties.ToList();
                db.rabaties.DeleteAllOnSubmit(q.Where(p => (p.zrodlo_id != -2 && !items.Select(w => w.Id).ToList<int>().Contains(p.zrodlo_id))));
                db.SubmitChanges();
              //log.Error( "przed danych",new Exception(DateTime.Now.ToLongTimeString()));
                var customers = db.klienciLinqs.Where(p=>p.aktywny).Select(p => new { p.zrodlo_id, p.klient_id }).ToList();
                var products = db.produkties.Select(p => new { p.zrodlo_id, p.produkt_id }).ToList();
                var categoriesSource = db.kategorie_zrodlowes.Select(p => new { p.zrodlo_id, p.kategoria_id }).ToList();
                var categories = db.kategories.Select(p => new { p.zrodlo_id, p.kategoria_id }).ToList();
              //log.Error( "po danych",new Exception(DateTime.Now.ToLongTimeString()));
                foreach (DiscountItem d in items)
                {
                    bool upd = true;
                    var product = d.ProductId != 0 && d.ProductId != null ? products.SingleOrDefault(p => p.zrodlo_id == d.ProductId) : null;
                    var customer = d.CustomerId != 0 && d.CustomerId != null ? customers.SingleOrDefault(p => p.zrodlo_id == d.CustomerId) : null;
                    var productCategory = d.ProductCategoryId != 0 && d.ProductCategoryId != null ? categoriesSource.SingleOrDefault(p => p.zrodlo_id == d.ProductCategoryId) : null;
                    var customerCategory = d.CustomerCategoryId != 0 && d.CustomerCategoryId != null ? categories.SingleOrDefault(p => p.zrodlo_id == d.CustomerCategoryId) : null;

                    if (d.ProductId != 0 && d.ProductId != null && product == null)
                        continue;
                    if (d.CustomerId != 0 && d.CustomerId != null && customer == null)
                        continue;
                    if (d.CustomerCategoryId != 0 && d.CustomerCategoryId != null && customerCategory == null)
                        continue;
                    if (d.ProductCategoryId != 0 && d.ProductCategoryId != null && productCategory == null)
                        continue;

                    var v = d.Id == -2 || d.Id == -1 ? null : q.SingleOrDefault(p => (p.zrodlo_id == d.Id));

                    if (v == null)
                    {
                        if (product != null && customer != null && d.Id == -2)
                            v = q.SingleOrDefault(p => p.zrodlo_id == -2 && p.produkt_id == product.produkt_id && p.klient_id == customer.klient_id);

                        if (v == null)
                        {
                            v = new SolEx.Hurt.DAL.rabaty();
                            v.zrodlo_id = d.Id;
                            upd = false;
                        }
                    }
                    v.aktywny = true;
                    v.@do = d.ToDate;
                    var category = categories.FirstOrDefault(p => p.zrodlo_id == d.CustomerCategoryId);
                    if (category != null)
                        v.kategoria_klientow_id = category.kategoria_id;
                    var cs = categoriesSource.SingleOrDefault(p=>p.zrodlo_id == d.ProductCategoryId);
                    if (cs != null)
                        v.kategoria_zrodlowa_produktow_id = cs.kategoria_id;
                    if (customer != null)
                        v.klient_id = customer.klient_id;
                    v.od = d.FromDate;
                    if (product != null)
                        v.produkt_id = product.produkt_id;
                    v.typ_rabatu = (short)d.DiscountType;
                    v.typ_wartosci = (short)d.ValueType;
                    v.wartosc1 = d.Value1.GetValueOrDefault(0);
                    v.wartosc2 = d.Value2.GetValueOrDefault(0);
                    v.wartosc3 = d.Value3.GetValueOrDefault(0);
                    v.poziom_cen = d.PriceLevelId;

                    if (!upd)
                        db.rabaties.InsertOnSubmit(v);
                }
               // log.Error( "po petli",new Exception(DateTime.Now.ToLongTimeString()));
                db.SubmitChanges();
            }
            finally{ if(db!=null) db.Dispose();}
        }

        internal void SynchronizeCustomerCategories(List<Category> items)
        {
            if (items == null || items.Count < 1)
                return;

          log.Info("start - kategorie klientow");

            SqlCommand cmd = null;
            SqlConnection conn = null;
            StringBuilder sb = null;
            string sql = null;

            try
            {
                conn = new SqlConnection(Config.MainCS);
                conn.Open();

                List<int> ids = items.Select(w => w.Id).ToList<int>();

                cmd = new SqlCommand(string.Format("delete from kategorie where zrodlo_id <> 0 and grupa_id in (select id from grupy where typ_id=2) and zrodlo_id not in ({0})"
                 , ids == null || ids.Count == 0 ? "0" : Model.Helpers.Serializacje.SerializeList(ids, ',')), conn);
                cmd.ExecuteNonQuery();
               
                sb = new StringBuilder(500);
                //synchronizacja grup 
                List<string> groupNames = items.Select(p => p.GroupName).Distinct().ToList();
                sql = string.Format("delete from grupy where zrodlo_id <> 0 and  typ_id=2 and nazwa not in ({0})", groupNames == null || groupNames.Count == 0 ? "''" : Model.Helpers.Serializacje.SerializeList(groupNames, ','));
                cmd = new SqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
                foreach (string s in groupNames)
                {
                    sb.AppendFormat(@" if not exists(select * from grupy  where typ_id=2 and nazwa = {0} ) 
                                    insert into grupy(nazwa,typ_id,widoczna,kolejnosc_na_Stronie,producencka,lista_z_obrazkami,zrodlo_id) values ({0},2,1,0,0,0,-1)  ",s.ToStringDoSerializacji());
                }
                sql = sb.ToString();
                if (!string.IsNullOrEmpty(sql))
                {
                    cmd = new SqlCommand(sql, conn);
                    cmd.ExecuteNonQuery();
                }
                sb.Clear();
                foreach (var v in items)
                {
                    sb.AppendFormat(@" if exists(select * from kategorie  where zrodlo_id <> 0 and grupa_id in (select id from grupy where typ_id=2) and zrodlo_id={0})
                        update kategorie set   grupa_id = (select top 1 id from grupy where nazwa={1} and zrodlo_id<>0 and typ_id=2), nazwa = {2}  where zrodlo_id={0} 
                        else insert into kategorie(zrodlo_id, grupa_id, nazwa, widoczna) values({0},(select top 1 id from grupy where nazwa={1} and zrodlo_id<>0 and typ_id=2), {2}, 1) ", v.Id.ToStringDoSerializacji(), v.GroupName.ToStringDoSerializacji(), v.Name.ToStringDoSerializacji());

                }
                sql = sb.ToString();
                if (!string.IsNullOrEmpty(sql))
                {
                    cmd = new SqlCommand(sql, conn);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
              log.Error(  sql,ex);
            }
            finally { if (cmd != null) cmd.Dispose(); if (conn != null) { conn.Close(); conn.Dispose(); } }            
        }

        internal void SynchronizeSourceCategories(List<Category> items)
        {
          log.Info ("start - kategorie");

            if (items == null || items.Count < 1)
                return;

            MainDataContext db = MainDAO.GetContext();
            StringBuilder sb = null;
            string s = null;
            int a = 0;

            try
            {
                sb = new StringBuilder(1000);   

                // usunięcie kategorii źródłowych
                List<int> ids = items.Select(w => w.Id).ToList<int>();
                db.ExecuteCommand(string.Format("delete from kategorie_zrodlowe where zrodlo_id <> 0 and zrodlo_id > -9999990 and zrodlo_id not in ({0})"
                    , ids == null || ids.Count == 0 ? "0" : Model.Helpers.Serializacje.SerializeList(ids, ',')));

                // pobranie kategorii zrodlowych
                string SQL_KategorieZrodlowe = @"select Id = kz.zrodlo_id, Name = kz.nazwa, Lp = kz.kolejnosc, ParentId = (select zrodlo_id from kategorie_zrodlowe where kategoria_id = kz.parent_id) from kategorie_zrodlowe kz ";
                List<Category> items_b2b = db.ExecuteQuery<Category>(SQL_KategorieZrodlowe).ToList();

                string sql_update = " update kategorie_zrodlowe set zrodlo_id = {0}, nazwa = {1}, kolejnosc = {2}, parent_id = (select top 1 kategoria_id from kategorie_zrodlowe where zrodlo_id = {3}) where zrodlo_id = {0} ";
                string sql_insert = " insert into kategorie_zrodlowe(zrodlo_id, nazwa, kolejnosc, parent_id) values({0}, {1}, {2}, (select top 1 kategoria_id from kategorie_zrodlowe where zrodlo_id = {3})) ";

                foreach (Category i in items)
                {
                    s = null;
                    var v = items_b2b.FirstOrDefault(p => p.Id == i.Id);
                    if (v == null)
                        s = sql_insert;
                    else
                    {
                        v.CreateDate = i.CreateDate;
                        if (!SyncTools.PorownajObiekty(i, v))
                            s = sql_update;
                    }
                    if (!string.IsNullOrEmpty(s))
                    {
                        ++a;
                        sb.AppendFormat(s, i.Id, i.Name.ToStringDoSerializacji(), i.Lp, i.ParentId.ToStringDoSerializacji());
                    }
                }
                if(!string.IsNullOrEmpty(sb.ToString()))
                    db.ExecuteCommand(sb.ToString());

                bool b = true; // (DateTime.Now.Hour > 23 && DateTime.Now.Hour < 14);
                if (Config.Settings.GetSettingBool("category_auto_mapping", false) && Config.Settings.GetSettingBool("category_auto_mapping_delete", true) && b)
                {
                    db.ExecuteCommand("delete from kategorie where zrodlo_id > 0 ");
                }
                
                if ( Config.Settings.GetSettingBool("sync_kategorie_jako_kategorie_platformy", false) && b )
                {
                    var grupy = db.grupies.Where(p=>p.typ_id==1).ToList();

                    foreach (Category i in items)
                    {
                        if (i.Id == 0 || string.IsNullOrEmpty(i.Name ))
                            continue;

                        var v = db.kategories.FirstOrDefault(p => p.kategorie_kategorie_zrodlowes.Any() && p.kategorie_kategorie_zrodlowes.First().kategorie_zrodlowe.zrodlo_id == i.Id);

                        if (v == null)
                        {
                            v = new SolEx.Hurt.DAL.kategorie();
                            v.widoczna = true;
                            kategorie_kategorie_zrodlowe kkz = new kategorie_kategorie_zrodlowe();
                            kkz.kategorie = v;
                            kkz.kategoria_zrodlowa_id = db.kategorie_zrodlowes.Single(p => p.zrodlo_id == i.Id).kategoria_id;
                            db.kategorie_kategorie_zrodlowes.InsertOnSubmit(kkz);
                        }
                        if (i.Picture !=null)
                        {
                            obrazki o = v.obrazki;
                            if (o == null)
                            {
                                o = new obrazki();                            
                            }
                            o.miniaturka1 = i.Picture.T1;
                            o.miniaturka2 = i.Picture.T2;
                            o.miniaturka3 = i.Picture.T3;
                            o.oryginal = i.Picture.T4;
                            o.typ_mime = i.Picture.MimeType;
                            o.sciezka = i.Picture.Path;
                            o.zrodlo_id = i.Picture.Id;
                            v.obrazki = o;                                
                        }
                        v.kolejnosc = i.Lp; 
                        v.nazwa = i.Name;
                        v.grupa_id = (i.Id > 0) ? grupy.First(p => p.producencka == false).id : grupy.First(p => p.producencka == true).id;
                        v.zrodlo_id = i.Id;
                        v.kolejnosc = i.Lp != 0 ? i.Lp : v.kolejnosc;
                   }
                   db.SubmitChanges();

                   var kat_zrodlo = (from c in db.kategorie_kategorie_zrodlowes
                                     join k_z in db.kategorie_zrodlowes on c.kategoria_zrodlowa_id equals k_z.kategoria_id
                                     select new { c.kategoria_id, k_z.zrodlo_id }).ToList();
                   foreach (Category i in items)
                   {
                       if(i.ParentId.HasValue)
                       {
                            var w = kat_zrodlo.FirstOrDefault(x => x.zrodlo_id == i.Id);
                            if(w == null) continue;

                            var k = db.kategories.FirstOrDefault(p => p.kategoria_id == w.kategoria_id);
                            if (k == null) continue;

                            // log.Error( i.ParentCategory.Value.ToString(),"kat_par");

                            var r = kat_zrodlo.FirstOrDefault(p => p.zrodlo_id == i.ParentId.Value);
                            if(r != null)
                                k.parent_id = r.kategoria_id;
                       }
                   }
                   db.SubmitChanges();
                }
                
                if ( Config.Settings.GetSettingBool("category_auto_mapping", false) && b )
                {
                    db = MainDAO.GetContext();
                    db.CommandTimeout = Config.Settings.TimeoutMedium;

                    string sql = "Exec proc_kategorie_Mapowanie ;";

                    db.ExecuteCommand(sql);
                }

                if (Config.Settings.GetSettingBool("kategorie_specjalne", false))
                {
                  log.Error("start - kategorie-specjalne");
                    try
                    {
                        var special_names = Config.Settings.GetSettingsList().Where(p => p.Symbol.StartsWith("Promocja_typ_")).Select(p => p.Symbol).ToList();
                        if (special_names.Count > 0)
                        {
                            for (int i = 0; i < special_names.Count; i++)
                            {
                                int id = 0;
                                
                                int idx = special_names[i].LastIndexOf("_");
                                if (!int.TryParse(special_names[i].Substring(idx + 1), out id))
                                {
                                    continue;
                                }
                                var sc = db.kategorie_zrodlowes.FirstOrDefault(p => p.nazwa.ToLower() == (Config.Settings.GetSettingString(special_names[i], "") + "_platforma").ToLower());
                                if (sc == null)
                                {
                                    sc = new kategorie_zrodlowe();
                                    sc.nazwa = Config.Settings.GetSettingString(special_names[i], "") + "_platforma";
                                    db.kategorie_zrodlowes.InsertOnSubmit(sc);
                                }  
                            }
                            db.SubmitChanges();
                        }
                    }
                    catch (Exception ex) {log.Error( "Kategorie_specjalne",ex); }
                }
            }
            catch (Exception ex)
            {
               log.Error( "synchronizowanie kategorii źródłowych",ex);
            }
            finally
            {
                if(db != null) db.Dispose();
            }

          log.Info( " Liczba elementów: " + items.Count + " Zmieniono: " + a,new Exception("koniec - kategorie"));
        }

        internal void BulkInsert(string csvPath, string tableName, string where, int firstRow)
        {
            BulkInsert(csvPath, tableName, where, firstRow, "|", ";");
        }
        internal void BulkInsert(string csvPath, string tableName, string where, int firstRow, string fieldTerminator, string rowTerminator)
        {
            log.Error( "start - BULK " + tableName);
            SqlConnection conn = null;
            SqlCommand cmd = null;
            try
            {
                conn = new SqlConnection(Config.MainCS);
                conn.Open();
                DataHelper.ClearTable(conn, tableName,where);

                string sql = "Exec proc_BULK_INSERT @csv_path = '{0}', @table_name = '{1}', @first_row = {2}"
                    + ", @field_terminator = '{3}', @row_terminator = '{4}' ;";

                sql = string.Format(sql, csvPath, tableName, firstRow, fieldTerminator, rowTerminator);

                cmd = new SqlCommand(sql, conn);
                cmd.CommandTimeout = Config.Settings.TimeoutMedium;
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
              log.Error( "aktualizowanie BULK " + tableName,ex);
            }
            finally { if (cmd != null) cmd.Dispose(); if (conn != null) { conn.Close(); conn.Dispose(); } }


            if(tableName == "flat_stany")
               log.Error("koniec - BULK " + tableName);
            else
               log.Error( "czas",new Exception("koniec - BULK " + tableName));
        }
        
        internal void SynchronizeProducts(List<Model.produkty> items, out string error)
        {

            error = "";

            if (items == null || items.Count < 1)
            {
                return;
            }
            
          log.Info( "start - produkty");

            SqlCommand cmd = null;
            SqlConnection conn = null;
            string log_symbol = Config.Settings.GetSettingString("produkty_synchro_log_symbol", "");
            string sql_update = null, sql_insert, sql = null ;
            StringBuilder sb = new StringBuilder();
            int a = 0;

            try
            {
                var s_desc_update = Config.Settings.GetSettingString("aktualizuj_opisy_produktow", "opis1;opis2;opis3;opis4;opis5");
                bool b_desc_update = s_desc_update == null ? false : s_desc_update.Contains("opis1");

                // pobieranie produktów
                MainDataContext db = MainDAO.GetContext();
                List<Model.produkty> items_b2b = ProductDAO.Get(new ProduktySearchCriteria() { AddtionalSQL=" zrodlo_id>0 " });

                conn = new SqlConnection(Config.MainCS);
                conn.Open();

                // deaktywowanie
                if (string.IsNullOrEmpty(log_symbol))
                {
                    var list = items.Select(w => w.zrodlo_id).ToList();
                    var deleted = items_b2b.Where(p => !list.Contains(p.zrodlo_id)).Select(p => p.zrodlo_id).ToList();
                    cmd = new SqlCommand(string.Format("Update produkty set widoczny = 0 where zrodlo_id > 0 and zrodlo_id in ({0}) "
                        , deleted.Count == 0 ? "0" : Model.Helpers.Serializacje.SerializeList(deleted, ',')), conn);
                  log.Error( "deaktywowanie liczba",new Exception(deleted.Count.ToString()));
                    cmd.ExecuteNonQuery();
                }

                sql_update = @" Update produkty set ilosc_w_opakowaniu = {1}, jednostka_miary = {2}
                , kod = {3}, kod_kreskowy = {4}, nazwa = {6}, PKWiU = {7}, stan = {8}
                , stan_min = {9}, vat = {10}, widoczny = {11}, opis = {12}, opis_krotki = {13}, ojciec = {14}, rodzina = {15}
                , pole_tekst1 = {16}, pole_tekst2 = {17}, pole_tekst3 = {18}, waga = {19}, www = {20}, pole_tekst4 = {21}, pole_tekst5 = {22}
                , pole_liczba1 = {23}, pole_liczba2 = {24}, pole_liczba3 = {25}, pole_liczba4 = {26}, pole_liczba5 = {27}
                , kolumna_tekst1 = {28}, kolumna_tekst2 = {29}, kolumna_tekst3 = {30}, kolumna_tekst4 = {31}, kolumna_tekst5 = {32}
                , kolumna_liczba1 = {33}, kolumna_liczba2 = {34}, kolumna_liczba3 = {35}, kolumna_liczba4 = {36}, kolumna_liczba5 = {37}
                , jednostka_miary1 = {38}, jednostka_miary_przelicznik1 = {39}, jednostka_miary2 = {40}, jednostka_miary_przelicznik2 = {41}
                , jednostka_miary3 = {42}, jednostka_miary_przelicznik3 = {43}, ilosc_minimalna = {44}, dostawa = {45}
                , jednostka_miary_przelicznik_cena0 = {46}, jednostka_miary_przelicznik_cena1 = {47}, jednostka_miary_przelicznik_cena2 = {48}
                , jednostka_miary_przelicznik_cena3 = {49}, ilosc_na_palecie = {50}, dostepny_dla_wszystkich = {51}, historia_ceny_html = {52} , ilosc_w_opakowaniu_tryb = {53} 
                    where zrodlo_id = {0} ";
                sql_insert = @" Insert into produkty(ilosc_w_opakowaniu, jednostka_miary
                , kod, kod_kreskowy, nazwa, PKWiU, stan
                , stan_min, vat, widoczny, opis, opis_krotki, ojciec, rodzina
                , pole_tekst1, pole_tekst2, pole_tekst3, waga, www, zrodlo_id, pole_tekst4, pole_tekst5
                , pole_liczba1, pole_liczba2, pole_liczba3, pole_liczba4, pole_liczba5
                , kolumna_tekst1, kolumna_tekst2, kolumna_tekst3, kolumna_tekst4, kolumna_tekst5
                , kolumna_liczba1, kolumna_liczba2, kolumna_liczba3, kolumna_liczba4, kolumna_liczba5
                , jednostka_miary1, jednostka_miary_przelicznik1, jednostka_miary2, jednostka_miary_przelicznik2
                , jednostka_miary3, jednostka_miary_przelicznik3, ilosc_minimalna, dostawa
                , jednostka_miary_przelicznik_cena0, jednostka_miary_przelicznik_cena1, jednostka_miary_przelicznik_cena2, jednostka_miary_przelicznik_cena3
                , ilosc_na_palecie, dostepny_dla_wszystkich, historia_ceny_html,ilosc_w_opakowaniu_tryb) values ({1}, {2}, {3}, {4}
                , {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13}, {14}, {15}, {16}, {17}, {18}, {19}, {20}, {0}, {21}, {22}
                , {23}, {24}, {25}, {26}, {27}, {28}, {29}, {30}, {31}, {32}, {33}, {34}, {35}, {36}, {37}
                , {38}, {39}, {40}, {41}, {42}, {43}, {44}, {45}, {46}, {47}, {48}, {49}, {50}, {51}, {52}, {53}) ";

                items = items.OrderBy(p => p.zrodlo_id).ToList();


                for (int i = 0, j = 0; i < items.Count; ++i)
                {

                    items[i].ilosc_w_opakowaniu =  items[i].ilosc_w_opakowaniu <= 0 ? 1 : items[i].ilosc_w_opakowaniu;
                    items[i].ilosc_minimalna = items[i].ilosc_minimalna <= 0 ? 1 : items[i].ilosc_minimalna;
                    items[i].www = items[i].www == null ? "" : items[i].www;

                    sql = null;
                    Model.produkty item = GetProductToCompare(items, i, items_b2b, j, out j);
                    if (item != null)
                    {
                        items[i].produkt_id = item.produkt_id;
                    }
                    if (item == null)
                        sql = sql_insert;
                    else if (!SyncTools.PorownajObiekty(items[i], item)) // !string.IsNullOrEmpty(log_symbol) && items[i].symbol == log_symbol))
                        sql = sql_update;

                    if (sql != null)
                    {
                        ++a;
                        //if (items[i].symbol == "1002335")
                        //  log.Error( "aktualizuj_opisy2",new Exception(!b_desc_update && item != null ? "opis" : items[i].Uwagi.ToStringDoSerializacji()));
                        sb.AppendFormat(sql, items[i].zrodlo_id, items[i].ilosc_w_opakowaniu.ToStringDoSerializacji(), items[i].jednostka_miary.ToStringDoSerializacji(), items[i].kod.ToStringDoSerializacji(), items[i].kod_kreskowy.ToStringDoSerializacji(), ""
                            , items[i].nazwa.ToStringDoSerializacji(), items[i].PKWiU.ToStringDoSerializacji(),0.ToStringDoSerializacji(), items[i].ilosc_minimalna.ToStringDoSerializacji(), items[i].vat.ToStringDoSerializacji(), items[i].widoczny.ToStringDoSerializacji()
                            , !b_desc_update && item != null ? "opis" : items[i].opis.ToStringDoSerializacji(), !b_desc_update && item != null ? "opis_krotki" : items[i].opis_krotki.ToStringDoSerializacji()
                            , items[i].ojciec.ToStringDoSerializacji(), items[i].rodzina.ToStringDoSerializacji()
                            , items[i].pole_tekst1.ToStringDoSerializacji(), items[i].pole_tekst2.ToStringDoSerializacji(), items[i].pole_tekst3.ToStringDoSerializacji(), items[i].waga.ToStringDoSerializacji(), items[i].www.ToStringDoSerializacji(), items[i].pole_tekst4.ToStringDoSerializacji(), items[i].pole_tekst5.ToStringDoSerializacji()
                            , items[i].pole_liczba1.ToStringDoSerializacji(), items[i].pole_liczba2.ToStringDoSerializacji(), items[i].pole_liczba3.ToStringDoSerializacji(), items[i].pole_liczba4.ToStringDoSerializacji(), items[i].pole_liczba5.ToStringDoSerializacji()
                            , items[i].kolumna_tekst1.ToStringDoSerializacji(), items[i].kolumna_tekst2.ToStringDoSerializacji(), items[i].kolumna_tekst3.ToStringDoSerializacji(), items[i].kolumna_tekst4.ToStringDoSerializacji(), items[i].kolumna_tekst5.ToStringDoSerializacji()
                            , items[i].kolumna_liczba1.ToStringDoSerializacji(), items[i].kolumna_liczba2.ToStringDoSerializacji(), items[i].kolumna_liczba3.ToStringDoSerializacji(), items[i].kolumna_liczba4.ToStringDoSerializacji(), items[i].kolumna_liczba5.ToStringDoSerializacji()
                            , items[i].jednostka_miary1.ToStringDoSerializacji(), items[i].jednostka_miary_przelicznik1.ToStringDoSerializacji(), items[i].jednostka_miary2.ToStringDoSerializacji(), items[i].jednostka_miary_przelicznik2.ToStringDoSerializacji(), items[i].jednostka_miary3.ToStringDoSerializacji(), items[i].jednostka_miary_przelicznik3.ToStringDoSerializacji()
                            , items[i].ilosc_minimalna.ToStringDoSerializacji(),  items[i].jednostka_miary_przelicznik_cena0.ToStringDoSerializacji(), items[i].jednostka_miary_przelicznik_cena1.ToStringDoSerializacji(), items[i].jednostka_miary_przelicznik_cena2.ToStringDoSerializacji(), items[i].jednostka_miary_przelicznik_cena3.ToStringDoSerializacji()
                            , items[i].ilosc_na_palecie.ToStringDoSerializacji(), items[i].dostepny_dla_wszystkich.ToStringDoSerializacji(),"".ToStringDoSerializacji(),items[i].ilosc_w_opakowaniu_tryb.ToStringDoSerializacji());
                    }                    
                    if (a > 0 && a % PackageSize == 0)
                    {
                        ss = sb.ToString();
                        if (!string.IsNullOrEmpty(ss))
                        {
                            cmd = new SqlCommand(ss, conn);
                            cmd.CommandTimeout = Config.Settings.TimeoutMedium;
                            cmd.ExecuteNonQuery();
                        }
                        sb = new StringBuilder();
                    }

                }
                ss = sb.ToString();
                if (!string.IsNullOrEmpty(ss))
                {
                    cmd = new SqlCommand(ss, conn);
                    cmd.CommandTimeout = Config.Settings.TimeoutMedium;
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
              log.Error( "synchronizowanie produktow",ex);
              log.Error( "synchronizowanie produktow",ex);
            }
          log.Error( "czas "+ "Liczba elementów: "+ items.Count + " Zmieniono: " + a,new Exception("koniec - produkty"));

            if (!string.IsNullOrEmpty(log_symbol))
                return;

            SynchronizeQuantityUnits(items);

            SynchronizeProductsRelations(items);

            SynchronizeProductsLanguages(items);

            SynchronizeProductsAttributesConnections(items);

            SynchronizeProductsCategoriesConnections(items);

          

            if (!string.IsNullOrEmpty(wrong_pics))
            {
                error += wrong_pics;
            }
        }

        private void SynchronizeQuantityUnits(List<Model.produkty> items)
        {
            Model.jezyki defaultLanguage = Config.Languages.FirstOrDefault(p => p.domyslny);
                foreach (Model.produkty i in items)
                {
                    if(!string.IsNullOrEmpty(i.jednostka_miary))
                    {
                        Config.GetSystemName(defaultLanguage.id, "Jednostka_" + i.jednostka_miary, i.jednostka_miary);
                    }
                    if (!string.IsNullOrEmpty(i.jednostka_miary1))
                    {
                        Config.GetSystemName(defaultLanguage.id, "Jednostka_" + i.jednostka_miary1, i.jednostka_miary1);
                    }
                    if (!string.IsNullOrEmpty(i.jednostka_miary2))
                    {
                        Config.GetSystemName(defaultLanguage.id, "Jednostka_" + i.jednostka_miary2, i.jednostka_miary2);
                    }
                    if (!string.IsNullOrEmpty(i.jednostka_miary3))
                    {
                        Config.GetSystemName(defaultLanguage.id, "Jednostka_" + i.jednostka_miary3, i.jednostka_miary3);
                    }
                }
        }

        private Model.produkty GetProductToCompare(List<Model.produkty> items, int i, List<Model.produkty> items_b2b, int j, out int jj)
        {
            Model.produkty item = null;
            while (true)
            {
                if (items_b2b.Count <= j)
                    break;
                if (items[i].zrodlo_id == items_b2b[j].zrodlo_id)
                {
                    item = items_b2b[j];
                    ++j;
                    break;
                }
                if (items[i].zrodlo_id < items_b2b[j].zrodlo_id)
                    break;
                ++j;
            }
            jj = j;
            return item;
        }

        /// <summary>
        /// Synchronizowanie relacji pomiędzy produktami
        /// </summary>
        /// <param name="items"></param>
        private void SynchronizeProductsRelations(List<Model.produkty> items)
        {
            SqlCommand cmd = null;
            SqlConnection conn = null;
            
            try
            {
                conn = new SqlConnection(Config.MainCS);
                conn.Open();

                if (items.Any(p => p.Relations != null) && items.Any(p => p.Relations.Count > 0))
                {
                    int a = 0;
                    string st = "";
                    StringBuilder sb = new StringBuilder();
                    string types = string.IsNullOrEmpty(Config.Settings.GetSettingString("relacje_typy_zrodlowe", "")) ? " 1=1 " : " typ in (" + Config.Settings.GetSettingString("relacje_typy_zrodlowe","") + ")";

                    cmd = new SqlCommand("Update produkty_produkty set aktywny = 0 where " + types, conn);
                    cmd.ExecuteNonQuery();

                    foreach (Model.produkty i in items.Where(p => p.Relations != null && p.Relations.Count > 0))
                    {
                        string sql = string.Format(" declare @productId int; select top 1 @productId = produkt_id from produkty where zrodlo_id = {0} ", i.zrodlo_id);

                        int k = 0;
                        foreach (var r in i.Relations)
                        {
                            sql += string.Format(@" declare @relationId{0} int; select top 1 @relationId{0} = produkt_id from produkty where zrodlo_id = {1} 
                                if exists(select * from produkty_produkty where ojciec_id = @productId and dziecko_id = @relationId{0} and typ = {2}) 
                                Update produkty_produkty set aktywny = 1 where ojciec_id = @productId and dziecko_id =  @relationId{0} and typ = {2} 
                                else if  @relationId{0} is not null
                                Insert into produkty_produkty(ojciec_id, dziecko_id, typ, aktywny) values(@productId, @relationId{0}, {2}, 1) ", k++, r.Id2, r.Type);
                        }

                        cmd = new SqlCommand(sql, conn);
                        cmd.ExecuteNonQuery();

                        //if (++a % PackageSize == 0)
                        //{
                        //    ss = sb.ToString();
                        //    if (!string.IsNullOrEmpty(ss))
                        //        new SqlCommand(ss, conn).ExecuteNonQuery();
                        //    sb = new StringBuilder();
                        //}
                    }

                    //ss = sb.ToString();
                    //if (!string.IsNullOrEmpty(ss))
                    //    new SqlCommand(ss, conn).ExecuteNonQuery();

                    // Usuwanie relacji 
                    cmd = new SqlCommand("Delete from produkty_produkty where aktywny = 0 and " + types, conn);
                    cmd.ExecuteNonQuery(); 

                }
            }
            catch (Exception ex)
            {
              log.Error( "synchronizowanie relacji produktow",ex);
            }
            finally { if (cmd != null) cmd.Dispose(); if (conn != null) { conn.Close(); conn.Dispose(); } }

            try
            {
                if (Config.Settings.GetSettingBool("product_sets", false) && items.Any(p => p.Sets.Count > 0))
                {
                    int a = 0;
                    string st = "";
                    StringBuilder sb = new StringBuilder();

                    conn = new SqlConnection(Config.MainCS);
                    conn.Open();

                    cmd = new SqlCommand("Update produkty_zestawy set aktywny = 'false';", conn);
                    cmd.ExecuteNonQuery();

                    foreach (Model.produkty i in items.Where(p => p.Sets.Count > 0))
                    {
                        string sql = string.Format(" declare @productId int; select top 1 @productId = produkt_id from produkty where zrodlo_id = {0} ", i.zrodlo_id);

                        int k = 0;
                        foreach (var r in i.Sets)
                        {
                            sql += string.Format(@" declare @setId{0} int; select top 1 @setId{0} = produkt_id from produkty_zestawy where nazwa = {1} and produkt_id = @productId 
                                if exists(select * from produkty_zestawyy where id = @setId{0}) 
                                Update produkty_zestawy set nazwa = {1}, aktywny = 1 where id = @setId{0} 
                                else if @setId{0} is not null
                                Insert into produkty_zestawy(nazwa, produkt_id, aktywny) values({1}, @productId, 1) ", k++, r.ToStringDoSerializacji());
                        }

                        cmd = new SqlCommand(sql, conn);
                        cmd.ExecuteNonQuery();
                    }
                    cmd = new SqlCommand("Delete from produkty_zestawy where aktywny = 'false'", conn);
                    cmd.ExecuteNonQuery();

                }
            }
            catch (Exception ex)
            {
              log.Error( "synchronizowanie kompletów ",ex);
            }
            finally { if (cmd != null) cmd.Dispose(); if (conn != null) { conn.Close(); conn.Dispose(); } }
        }

        /// <summary>
        /// Synchronizowanie połączeń produktów z kategoriami
        /// </summary>
        /// <param name="items"></param>
        public void SynchronizeProductsCategoriesConnections(List<Model.produkty> items)
        {
            SqlCommand cmd = null;
            SqlConnection conn = null;
            SqlDataReader r = null;
            StringBuilder sb = new StringBuilder();
            List<int> brands = new List<int>();
            string sql = null;
            int list_index = 0;
            int a = 0;
            List<int> notActiveConnections = new List<int>();



            try
            {
                conn = new SqlConnection( Config.MainCS);
                conn.Open();

                // pobieranie listy aktualnych polączeń
                List<ProductConnection> products_categories = new List<ProductConnection>();
                cmd = new SqlCommand(@"select pkz.id, produkt_zrodlo_id = p.zrodlo_id, kategoria_zrodlo_id = kz.zrodlo_id from produkty_kategorie_zrodlowe pkz, kategorie_zrodlowe kz, produkty p 
                    where pkz.produkt_id = p.produkt_id and pkz.kategoria_zrodlowa_id = kz.kategoria_id and kz.zrodlo_id <> 0 and kz.zrodlo_id > -9999990 order by p.zrodlo_id", conn);
                cmd.CommandTimeout = Config.Settings.TimeoutMedium;
                r = cmd.ExecuteReader();
                while (r.Read())
                    products_categories.Add(new ProductConnection() { Id = (int)r[0], ProductSourceId = (int)r[1], ItemSourceId = (int)r[2] });
                r.Close();

                notActiveConnections = products_categories.Select(p => p.Id).ToList();

                // pobieranie produktow
                List<BasicItem> products = new List<BasicItem>();
                cmd = new SqlCommand("select produkt_id, zrodlo_id from produkty", conn);
                cmd.CommandTimeout = Config.Settings.TimeoutMedium;
                r = cmd.ExecuteReader();
                while (r.Read())
                    products.Add(new BasicItem() { Id = (int)r[0], Number = (int)r[1] });
                r.Close();
                // pobieranie cech
                List<BasicItem> categories = new List<BasicItem>();
                cmd = new SqlCommand("select kategoria_id, zrodlo_id from kategorie_zrodlowe", conn);
                cmd.CommandTimeout = Config.Settings.TimeoutMedium;
                r = cmd.ExecuteReader();
                while (r.Read())
                    categories.Add(new BasicItem() { Id = (int)r[0], Number = (int)r[1] });
                r.Close();

                // aktualizowanie połączeń
                foreach (Model.produkty v in items.Where(p => p.CategoryIds.Count > 0).OrderBy(p => p.zrodlo_id))
                {
                    int productId = products.Where(p => p.Number == v.zrodlo_id).Select(p => p.Id).FirstOrDefault();

                    foreach (var vv in v.CategoryIds)
                    {
                        if (vv < 0)
                            brands.Add(vv);

                        int id = FindProductConnectionId(products_categories, v.zrodlo_id, vv, null, list_index, out list_index);
                        if (id == 0)
                        {
                            ++a;
                            var c=categories.Where(p => p.Number == vv).Select(p => p.Id).FirstOrDefault();
                            if (c == 0)
                            {
                              log.Error( "kz równa 0",new Exception(productId.ToString()));
                                continue;
                            }
                            sb.AppendFormat(" insert into produkty_kategorie_zrodlowe(produkt_id, kategoria_zrodlowa_id, rodzaj) values({0}, {1}, -1) "
                                , productId,c );
                        }
                        else
                            notActiveConnections.Remove(id);
                    }

                    if (a > 0 && a % PackageSize == 0)
                    {
                        ss = sb.ToString();
                        if (!string.IsNullOrEmpty(ss))
                        {
                          //log.Error( "update",new Exception(ss.ToString()));
                            cmd = new SqlCommand(ss, conn);
                            cmd.CommandTimeout = Config.Settings.TimeoutMedium;
                            cmd.ExecuteNonQuery();
                        }
                        sb.Clear();
                    }
                }
                ss = sb.ToString();
                if (!string.IsNullOrEmpty(ss))
                {
                 // log.Error( "update",new Exception(ss.ToString()));
                    cmd = new SqlCommand(ss, conn);
                    cmd.CommandTimeout = Config.Settings.TimeoutMedium;
                    cmd.ExecuteNonQuery();
                }

                // usuwanie nieaktualnych połączeń
                if (notActiveConnections.Count > 0)
                {
                    cmd = new SqlCommand("delete pkz from produkty_kategorie_zrodlowe pkz, kategorie_zrodlowe kz where pkz.rodzaj <> -1 and pkz.kategoria_zrodlowa_id = kz.kategoria_id and pkz.id in ("
                        + Model.Helpers.Serializacje.SerializeList(notActiveConnections, ',') + ")"
                        + " update produkty_kategorie_zrodlowe set rodzaj = 1 where rodzaj = -1 ", conn);
                    cmd.CommandTimeout = Config.Settings.TimeoutLong;
                    cmd.ExecuteNonQuery();
                }

                // deaktywowanie pustych marek
                brands = brands.Distinct().ToList();
                cmd = new SqlCommand(string.Format(@"update kategorie_zrodlowe set aktywny = 0 where zrodlo_id < 0 and zrodlo_id > -9999990 and zrodlo_id not in ({0}) 
                    update kategorie set widoczna = 0 where zrodlo_id < 0 and zrodlo_id > -9999990 and zrodlo_id not in ({0})"
                    , brands.Count == 0 ? "0" : Model.Helpers.Serializacje.SerializeList(brands, ',')), conn);
                cmd.CommandTimeout = Config.Settings.TimeoutMedium;
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
              log.Error(  sql,ex);
            }
            finally { if (cmd != null) cmd.Dispose(); if (conn != null) { conn.Close(); conn.Dispose(); } if (r != null) { r.Close(); r.Dispose(); } }



            MainDataContext db = MainDAO.GetContext();
            try
            {
                if (Config.Settings.GetSettingBool("kategorie_specjalne", false))
                {
                    // Zapisywanie zaktualizowanych połączeń z kategoriami specjalnymi
                    var special_names = Config.Settings.GetSettingsList().Where(p => p.Symbol.StartsWith("Promocja_typ_")).Select(p => p.Symbol).ToList();
                    var cas = db.produkty_kategorie_zrodlowes.ToList();

                    if (special_names.Count > 0)
                    {
                        for (int i = 0; i < special_names.Count; i++)
                        {
                            int id = 0;
                            int idx = special_names[i].LastIndexOf("_");
                            if (!int.TryParse(special_names[i].Substring(idx + 1), out id))
                            {
                                continue;
                            }
                            var sc = db.kategorie_zrodlowes.FirstOrDefault(p => p.nazwa.ToLower() == (Config.Settings.GetSettingString(special_names[i], "") + "_platforma").ToLower());
                            if (sc == null) { log.Error( "sc"); continue; }
                            db.produkty_kategorie_zrodlowes.DeleteAllOnSubmit(db.produkty_kategorie_zrodlowes.Where(p => p.kategoria_zrodlowa_id == sc.kategoria_id));
                            db.SubmitChanges();
                            var prods = db.rabaties.Where(p => p.typ_promocji == id && p.produkt_id != null
                                && (!p.@do.HasValue || p.@do.Value >= DateTime.Now)
                                )
                                .Select(p => p.produkt_id).Distinct().ToList();
                            foreach (int product_id in prods)
                            {
                                produkty_kategorie_zrodlowe pk = cas.SingleOrDefault(p => p.kategoria_zrodlowa_id == sc.kategoria_id && p.produkt_id == product_id);
                                if (pk == null)
                                {
                                    pk = new produkty_kategorie_zrodlowe();
                                    pk.produkt_id = product_id;
                                    pk.kategoria_zrodlowa_id = sc.kategoria_id;

                                    db.produkty_kategorie_zrodlowes.InsertOnSubmit(pk);
                                }
                            }
                        }
                        db.SubmitChanges();
                    }
                }

                // mapowanie kagorii wykorzystujących produkty podczas mapowania
                if (Config.Settings.GetSettingBool("category_auto_mapping2", false))
                {
                    try
                    {
                        if (db != null)
                            db.Dispose();

                        db = MainDAO.GetContext();
                        db.CommandTimeout = Config.Settings.TimeoutMedium;

                        sql = "Exec proc_kategorie_Mapowanie ;";

                        db.ExecuteCommand(sql);
                    }
                    catch (Exception ex) { log.Error( "mapowanie kategorii 2 ",ex); }

                }
            }
            catch (Exception ex)
            {
              log.Error( "kategorie specjalne",ex);
            }
            finally { if (db != null) db.Dispose(); }
        }
        
        /// <summary>
        /// Synchronizowanie połączeń produktów z cechami
        /// </summary>
        /// <param name="items"></param>
        public void SynchronizeProductsAttributesConnections(List<Model.produkty> items)
        {
            SqlCommand cmd = null;
            SqlConnection conn = null;
            SqlDataReader r = null;
            StringBuilder sb = new StringBuilder();
            int a = 0;
            string sql = null;
            int list_index = 0;
            bool dot = Config.Settings.GetSettingBool("attributes_replace_coma", false);
            List<int> notActiveConnections = new List<int>();


            try
            {
                conn = new SqlConnection( Config.MainCS);
                conn.Open();
                
                // pobieranie listy aktualnych polączeń
                List<ProductConnection> products_traits = new List<ProductConnection>();
                cmd = new SqlCommand(@"select cp.id, produkt_zrodlo_id = p.zrodlo_id, cecha_zrodlo_id = c.zrodlo_id, cecha_symbol = c.symbol from cechy_produkty cp, cechy c, produkty p 
                    where cp.produkt_id = p.produkt_id and cp.cecha_id = c.cecha_id and c.zrodlo_id < 10000 and c.zrodlo_id > -1 order by p.zrodlo_id", conn);
                cmd.CommandTimeout = Config.Settings.TimeoutMedium;
                r = cmd.ExecuteReader();
                while (r.Read())
                {
                    products_traits.Add(new ProductConnection() { Id = (int)r[0], ProductSourceId = (int)r[1], ItemSourceId = (int)r[2], ItemSymbol = r[3].ToString() });
                }
                r.Close();

                notActiveConnections = products_traits.Select(p => p.Id).ToList();

                // pobieranie produktow
                List<BasicItem> products = new List<BasicItem>();
                cmd = new SqlCommand("select produkt_id, zrodlo_id from produkty", conn);
                cmd.CommandTimeout = Config.Settings.TimeoutMedium;
                r = cmd.ExecuteReader();
                while (r.Read())
                    products.Add(new BasicItem() { Id = (int)r[0], Number = (int)r[1] });
                r.Close();
                // pobieranie cech
                List<BasicItem> traits = new List<BasicItem>();
                cmd = new SqlCommand("select cecha_id, zrodlo_id, symbol from cechy", conn);
                cmd.CommandTimeout = Config.Settings.TimeoutMedium;
                r = cmd.ExecuteReader();
                while (r.Read())
                    traits.Add(new BasicItem() { Id = (int)r[0], Number = (int)r[1], Symbol = r[2] == null || r[2] == DBNull.Value ? null : r[2].ToString() });
                r.Close();

                // aktualizowanie połączeń
                foreach (Model.produkty v in items.Where(p => p.AttributeIds.Count > 0 || p.AttributeSymbols.Count > 0).OrderBy(p => p.zrodlo_id))
                {
                    int productId = products.Where(p => p.Number == v.zrodlo_id).Select(p => p.Id).FirstOrDefault();

                    foreach (var vv in v.AttributeIds)
                    {
                        int id = FindProductConnectionId(products_traits, v.zrodlo_id, vv, null, list_index, out list_index);
                        if (id == 0)
                        {
                            ++a;
                            sb.AppendFormat(" insert into cechy_produkty(produkt_id, cecha_id, rodzaj) values({0}, {1}, -1) "
                                , productId, traits.Where(p => p.Number == vv).Select(p => p.Id).FirstOrDefault());
                        }
                        else
                            notActiveConnections.Remove(id);
                    }

                    foreach (var vv in v.AttributeSymbols)
                    {
                        int id = FindProductConnectionId(products_traits, v.zrodlo_id, -1, vv, list_index, out list_index);
                        if (id == 0)
                        {
                            ++a;
                            sb.AppendFormat(" insert into cechy_produkty(produkt_id, cecha_id, rodzaj) values({0}, {1}, -1) "
                                , productId, traits.Where(p => p.Symbol == (dot && vv != null ? vv.Replace(",", ".") : vv)).Select(p => p.Id).FirstOrDefault());
                        }
                        else
                            notActiveConnections.Remove(id);
                    }

                    if (a > 0 && a % PackageSize == 0)
                    {
                        ss = sb.ToString();
                        if (!string.IsNullOrEmpty(ss))
                        {
                            cmd = new SqlCommand(ss, conn);
                            cmd.CommandTimeout = Config.Settings.TimeoutLong;
                            cmd.ExecuteNonQuery();
                        }
                        sb.Clear();
                    }
                }

                ss = sb.ToString();
                if (!string.IsNullOrEmpty(ss))
                {
                    cmd = new SqlCommand(ss, conn);
                    cmd.CommandTimeout = Config.Settings.TimeoutLong;
                    cmd.ExecuteNonQuery();
                }

                // usuwanie nieaktualnych połączeń
                if (notActiveConnections.Count > 0) 
                {
                    cmd = new SqlCommand("delete cp from cechy_produkty cp, cechy c where cp.rodzaj <> -1 and cp.cecha_id = c.cecha_id and cp.id in (" 
                        + Model.Helpers.Serializacje.SerializeList(notActiveConnections, ',') + ")"
                        + " update cechy_produkty set rodzaj = 1 where rodzaj = -1 ", conn);
                    cmd.CommandTimeout = Config.Settings.TimeoutLong;
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
              log.Error( sql,ex);
            }
            finally { if (cmd != null) cmd.Dispose(); if (conn != null) { conn.Close(); conn.Dispose(); } if (r != null) {r.Close(); r.Dispose();} }

        }

        private int FindProductConnectionId(List<ProductConnection> list, int productId, int itemId, string itemSymbol, int list_index, out int list_index_out)
        {
            list_index_out = list_index;
            for (int i = list_index; i < list.Count; ++i)
            {
                if (list[i].ProductSourceId > productId)
                    return 0;
                if (list[i].ProductSourceId == productId && list[list_index_out].ProductSourceId != productId)
                    list_index_out = i; 
                if (list[i].ProductSourceId == productId && (list[i].ItemSourceId == itemId || (itemSymbol != null && list[i].ItemSymbol == itemSymbol)))
                    return list[i].Id;
            }
            return 0;
        }

        /// <summary>
        /// Synchronizowanie tłumaczeń nazw i opisów produktów
        /// </summary>
        /// <param name="items"></param>
        private void SynchronizeProductsLanguages(List<Model.produkty> items)
        {

            try
            {
                var l = items.FirstOrDefault(p => p.widoczny).Languages;
                if (items.FirstOrDefault(p => p.widoczny) != null && items.First(p => p.widoczny).Languages != null && items.Any(p => p.Languages.Count > 0))
                {
                    string s_desc_update = Config.Settings.GetSettingString("aktualizuj_opisy_produktow", "opis1;opis2;opis3;opis4;opis5");
                    bool b_desc_update = s_desc_update == null ? false : s_desc_update.Contains("opis1");


                    // pobranie tlumaczen
                    MainDataContext db = MainDAO.GetContext();
                    SlownikiSearchCriteria criteria = new SlownikiSearchCriteria();
                    criteria.obiekt_id.Add(Config.GetSystemTypeId("produkt"));
                    List<Model.slowniki> items_b2b = SlownikDAL.Get(criteria);

                
                
                    foreach (Model.produkty i in items.Where(p => p.Languages.Count > 0))
                    {
                        List<Model.slowniki> toChange = new List<Model.slowniki>();
                        foreach (Model.slowniki lang in i.Languages) 
                        {
                            var v = items_b2b.FirstOrDefault(p => p.id == i.zrodlo_id && p.pole == lang.pole && p.jezyk_id == lang.jezyk_id);
                            if (v == null)
                            {
                                toChange.Add(lang);
                            }
                            else
                            {
                                if (((lang.pole != "desc" && lang.pole != "desc-short") || (b_desc_update || v == null)))
                                {
                                    toChange.Add(lang);
                                }
                            }
                        }
                        SlownikDAL.UpdateSlowniki(toChange);
                    }
                }
            }
            catch (Exception ex)
            {
              log.Error( "synchronizowanie wersji językowych ",ex);
            }

        }

        internal void SynchronizeAttributes(List<Model.cechy> items)
        {

            /*
            if (items == null || items.Count < 1)
                return;

            log.Info ("start - cechy");

            MainDataContext db = null;
            try
            {
                bool attr_default_visibility = Config.Settings.GetSettingBool("attr_default_visibility", true);
                db = MainDAO.GetContext();
                var ce = MainDAO.db.Select<cechy>();

                // usuniecie cech
                List<int> ids = items.Select(w => w.cecha_id).ToList<int>();
                List<string> symbols = items.Select(w => w.symbol).ToList<string>();
                int size = 900;
                var ces = ce.Where(p => (p.zrodlo_id < 10000 && p.zrodlo_id>-1) && (p.zrodlo_id == 0 ? !symbols.Contains(p.symbol) : !ids.Contains(p.zrodlo_id)));

                for (int i = 0; i < 100; ++i)
                {
                    var v = ces.Skip(i * size).Take(size);
                    if (v.Count() == 0)
                        break;
                    MainDAO.db.DeleteAll<cechy>(v);
                }
                
                // usuniecie atrybutow
                ids = items.Select(w => w.AttributeId).ToList<int>().Distinct<int>().ToList<int>();
                symbols = items.Select(w => w.AttributeName).ToList<string>().Distinct<string>().ToList<string>();

                var atrs = db.atrybuties.Where(p => p.zrodlo_id >-1 && (p.zrodlo_id > 5000 ? !symbols.Contains(p.symbol)
                        : !ids.Contains(p.zrodlo_id.Value)));

                for (int i = 0; i < 100; ++i)
                {
                    var v = atrs.Skip(i * size).Take(size);
                    if (v.Count() == 0)
                        break;
                    db.atrybuties.DeleteAllOnSubmit(v);
                    db.SubmitChanges();
                }      
          
                var q = db.cechies.ToList();

                // atrybuty
                string s = Config.Settings.GetSettingString("attributes_traits_auto", "");
                bool dot = Config.Settings.GetSettingBool("attributes_replace_coma", false);
                if (s == null) s = "";
                string[] sp = s.Split(';'); 
                var ats = db.atrybuties.Where(p => s == "" ? true : sp.Contains(p.nazwa)).ToList();
                foreach (cechy i in items)
                {
                    if (dot && i.symbol != null)
                        i.symbol = i.symbol.Replace(",", ".");

                    var v = q.SingleOrDefault(p => i.cecha_id < 1 ? p.symbol == i.symbol : p.zrodlo_id == i.id);
                    if (v == null)
                    {
                        v = new cechy();
                        v.widoczna = attr_default_visibility;
                    }
                    v.symbol = i.symbol;
                
                    v.zrodlo_id = i.id;
                    
                    if (v.cecha_id == 0)
                    {
                        if (!string.IsNullOrEmpty(i.nazwa))
                            v.nazwa = i.nazwa;
                        if (v.nazwa == null) v.nazwa = "";
                        db.cechies.InsertOnSubmit(v);
                        db.SubmitChanges();
                    }

                    if (i.AttributeId > 0 || !string.IsNullOrEmpty(i.AttributeName))
                    {
                        atrybuty a = i.AttributeId > 5000 ? ats.FirstOrDefault(p => p.symbol == i.AttributeName)
                            : ats.FirstOrDefault(p => p.zrodlo_id == i.AttributeId);
                  
                        if (a == null)
                        {                            
                            a = new atrybuty();
                            a.widoczny = attr_default_visibility;
                            a.szerokosc = "\'auto\'";
                            if (!string.IsNullOrEmpty(i.AttributeName))
                            {
                                a.symbol = i.AttributeName;
                                a.nazwa = i.AttributeName;
                            }
                        }
                                               
                        a.zrodlo_id = i.AttributeId;
                        a.szerokosc = "";
                        
                        if (a.atrybut_id < 1)
                        {
                            db.atrybuties.InsertOnSubmit(a);
                            db.SubmitChanges();
                            ats.Add(a);
                        }

                        v.atrybut_id = a.atrybut_id;
                        db.SubmitChanges();
                    }
                    
                    foreach (var a in ats)
                    {
                        if(v.symbol.StartsWith(a.nazwa, StringComparison.OrdinalIgnoreCase) && v.atrybut_id == null) // !db.cechy_atrybuties.Any(p => p.atrybut_id == a.atrybut_id && p.cecha_id == v.cecha_id))
                        {
                            v.atrybut_id = a.atrybut_id;
                        }
                    }
                    db.SubmitChanges();
                }

                db.SubmitChanges();
            }
            catch (Exception ex)
            {
               log.Error( "synchronizowanie cech",ex);
            }
            finally
            {
                if(db != null) db.Dispose();
            }

           log.Error( "czas",new Exception("koniec - cechy"));
             *
             * */
            //TODO: synchro cech
        }
        public void SetOrdersState(historia_dokumenty[] docs) //int[] ids)
        {
            MainDataContext db = MainDAO.GetContext();

            try
            {
                int vat = Config.Settings.GetSettingInt("vat", 23);
                int[] ids = docs.Select(p => p.id).ToArray();

                var q = db.zamowienias.Where(p => ids.Contains(p.zamowienie_id));
                var w = db.zamowienia_statusies.FirstOrDefault(p => p.nazwa == "Zapisane");

                if (w == null)
                    throw new Exception("Brak zdefiniowanego statusu zamowienia: Zapisane.");

                foreach (var v in q)
                {
                    historia_dokumenty doc = docs.FirstOrDefault(p => p.id == v.zamowienie_id);

                    v.pobrane = true;
                    v.status_id = w.id;
                  //  v.nazwa_dokumentu = doc.SourceID;
                    db.SubmitChanges();

                    try
                    {
                        Order item = SolEx.Hurt.Core.CoreManager.GetOrderById(v.zamowienie_id, null, 0, Config.MainCS);
                     //   item.DocumentName = doc.Name;
                        item.Customer = SolEx.Hurt.Core.CoreManager.GetCustomerByLogin(item.Customer.email, null);
                        List<object> data = new List<object>();
                        data.AddRange(SolEx.Hurt.Core.CoreManager.ConvertCustomerToObjects(item.Customer));
                        data.Add(item);
                        object dt = SolEx.Hurt.Core.BLL.DynamicType.Generate(data);
                        SolEx.Hurt.Core.CoreManager.SendRaport(dt, item.Customer, "order_imported", Config.Settings,null,null);

                        //if (File.Exists(doc.FilePath))
                        //    File.Delete(doc.FilePath);
                    }
                    catch (Exception ex)
                    {
                       log.Error( "synchronizowanie stanów zamówień, mail",ex);
                    }
                }                
            }
            catch (Exception ex)
            {
               log.Error( "synchronizowanie stanów zamówień",ex);
            }
            finally
            {
                if(db != null) db.Dispose();
            }
        }

       

        internal void ClearCache()
        {
            MainDataContext db = MainDAO.GetContext();

            try
            {
                db.CommandTimeout = Config.Settings.TimeoutMedium;

                db.ExecuteCommand("Delete from logi where data < '" + DateTime.Now.AddDays(-1).ToString() + "';");
            }
            catch (Exception ex)
            {
               log.Error( "usuwanie cache",ex);
            }
            finally
            {
                if (db != null) db.Dispose();
            }
        }

      



        internal void ImportDocsLiteCSV(string docPath, string docProducsPath, string docTable, string docProductTable , int firstRow, string fieldTerminator, string rowTerminator)
        {

            int d = Config.Settings.GetSettingInt("b2b_mini_dokumenty_dni", 0);
            DateTime date = DateTime.Now;

            if (d > 0)
                date = DateTime.Today.AddDays(-d);
            date = date.Date;

            MainDataContext db = MainDAO.GetContext();
            string template = "";
            string sql = "";

            try
            {
                
                sql = string.Format( @"delete from historia_dokumenty_produkty where  dokument_id in(
                    select id from historia_dokumenty where data_utworzenia>='{0}');
                    delete from historia_dokumenty where data_utworzenia >= '{0}';", date);

                if (Config.Settings.GetSettingBool("mini_zk_clear_all", false))
                    sql = string.Format(@"delete from historia_dokumenty_produkty where  dokument_id in(
                        select id from historia_dokumenty where data_utworzenia >= '{0}' or rodzaj_id = 2);
                        delete from historia_dokumenty where data_utworzenia >= '{0}' or rodzaj_id = 2;", date);

                db.ExecuteCommand(sql);

                template = "Exec proc_BULK_INSERT @csv_path = '{0}', @table_name = '{1}', @first_row = {2} , @field_terminator = '{3}', @row_terminator = '{4}' ;";

                sql = string.Format(template, docPath, docTable, firstRow, fieldTerminator, rowTerminator);
                db.ExecuteCommand(sql);
                sql = string.Format(template, docProducsPath, docProductTable, firstRow, fieldTerminator, rowTerminator);
                db.ExecuteCommand(sql);
                
            }
            catch (Exception ex)
            {
               log.Error( "ImportDocsLiteCSV",ex);
            }
            finally
            {
                if (db != null) db.Dispose();                
            }
        }
        internal void ImportDocsLiteCSV(string docPath, string docTable, int firstRow, string fieldTerminator, string rowTerminator)
        {
            int d = Config.Settings.GetSettingInt("b2b_mini_dokumenty_dni", 0);
            DateTime date = DateTime.Now;

            if (d > 0)
                date = DateTime.Today.AddDays(-d);
            date = date.Date;

            MainDataContext db = MainDAO.GetContext();

            string sql = "";

            try
            {
                sql = string.Format(@"delete from historia_dokumenty_listy_przewozowe where  nazwa_dokumentu in(
                    select nazwa_dokumentu from historia_dokumenty where data_utworzenia>='{0}' and rodzaj_id=1) or nazwa_dokumentu not in (select nazwa_dokumentu from historia_dokumenty);
                ;", date);

                db.ExecuteCommand(sql);

                string template = "Exec proc_BULK_INSERT @csv_path = '{0}', @table_name = '{1}', @first_row = {2} , @field_terminator = '{3}', @row_terminator = '{4}' ;";

                sql = string.Format(template, docPath, docTable, firstRow, fieldTerminator, rowTerminator);
                db.ExecuteCommand(sql);
            }
            catch (Exception ex)
            {
               log.Error( "ImportDocsLiteCSV",ex);
            }
            finally
            {
                if (db != null) db.Dispose();
            }
        }
        internal void UpdateStates()
        {
            MainDataContext db = MainDAO.GetContext();
            try
            {
                db.proc_produkty_PrzetworzStany();

            }
            catch (Exception ex) { log.Error( "uaktualnienie stanów",ex); }
            finally { if (db != null) db.Dispose(); }
        }

        internal int GetSourceCategoriesCount()
        {
            MainDataContext db = MainDAO.GetContext();
            int a = 0;
            try
            {
                a = db.kategorie_zrodlowes.Count();

            }
            catch (Exception ex) {log.Error( "kategorie_zrodlowe",ex); }
            finally { if (db != null) db.Dispose(); }
            return a;
        }

        internal List<SolEx.Hurt.Model.klienci> GetDocsToSend()
        {
            const string SELECT_DATA = "select dokument=hd.id,k.klient_id,k.zrodlo_id,k.jezyk_id,k.alternatywny_email from historia_dokumenty hd join klienci k on hd.klient_id=k.zrodlo_id where  hd.id not in(  select dokument_id from klienci_sfera_dokumenty_wyslane where  status=1)  and hd.rodzaj_id=1 and k.alternatywny_email is not null and k.alternatywny_email<>''";
            const string DOC_EXT = "pdf";
            SqlConnection conn = null;
            SqlCommand cmd = null;
            SqlDataReader r = null;
            List<klienci> result = new List<klienci>();
            try
            {
                conn = new SqlConnection(Config.MainCS);
                conn.Open();

                cmd = new SqlCommand(SELECT_DATA, conn);
                r = cmd.ExecuteReader();
                while (r.Read())
                {
                    string email =DataHelper.dbs("alternatywny_email", r);
                    int id = DataHelper.dbi("klient_id", r);
                    int source_id = DataHelper.dbi("zrodlo_id", r);
                    int? lang_id =DataHelper.dbin("jezyk_id", r);
                    int doc_id = DataHelper.dbi("dokument", r);
                    if (!result.Any(p => p.klient_id == id))
                    {
                        klienci tmp = new klienci()
                        {
                            alternatywny_email = email,
                            klient_id = id,
                            zrodlo_id = source_id,
                            jezyk_id = lang_id
                        };
                        result.Add(tmp);
                    }
                    klienci c=result.First(p => p.klient_id == id);
                    string fileName = Tools.GetPathToFile(SolEx.Hurt.Core.CoreManager.SferaGetDocDirectory(""), c.zrodlo_id, doc_id, DOC_EXT);
                    if (File.Exists(fileName))
                    {
                        c.FilesToSendIDs.Add(doc_id);
                        c.FilesToSend.Add(fileName);
                    }
                }
                r.Close();
                r.Dispose();
            }
            catch (Exception ex)
            {
              log.Error( "sfera_pliki_do_wyslania",ex);
            }
            finally
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
                if (r != null)
                {
                    r.Close();
                    r.Dispose();
                }
            }
            return result;
        }
     
        internal void SetDocSendResult(int customerID, int documentId, bool status,string name)
        {
            bool newRow = false;
            MainDataContext db = MainDAO.GetContext();
            try
            {
                klienci_sfera_dokumenty_wyslane r = db.klienci_sfera_dokumenty_wyslanes.FirstOrDefault<klienci_sfera_dokumenty_wyslane>(p => (p.klient_id == customerID) && (p.dokument_id == documentId));
                if (r == null)
                {
                    newRow = true;
                    r = new klienci_sfera_dokumenty_wyslane { klient_id = customerID, nazwa_pliku = name, dokument_id = documentId };
                  //  int documentID = int.Parse(documentId);
                //    r.dokument_id = documentId;
                }
                r.status = status ? 1 : 0;
                r.data_wyslania = DateTime.Now;
                if (newRow)
                {
                    db.klienci_sfera_dokumenty_wyslanes.InsertOnSubmit(r);
                }
                db.SubmitChanges();
            }
            catch (Exception ex)
            {
              log.Error( "kategorie_zrodlowe",ex);
            }
            finally
            {
                if (db != null)
                {
                    db.Dispose();
                }
            }
        }
    }
}
