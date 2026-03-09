using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SolEx.Hurt.Model;
using System.Data.SqlClient;
using SolEx.Hurt.Sync.Core;
using SolEx.Hurt.Helpers;

namespace SolEx.Hurt.Sync.App.Modules_.Provider
{
	public class StaleCenyPromocjeProvider : IImportDataModule
	{
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		string SQL_VIEW = @"if not exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[solex_produkty_poziomy_cen]'))  exec('create view solex_produkty_poziomy_cen as

select produkt_id=tc_idtowar,Cena= ceny.wartosc,Poziom=poziomy.wartosc from
(SELECT 
		Wartosc,Pole
  FROM [tw_Parametr]
  unpivot
  (Wartosc for Pole in ( twp_NazwaCeny1,twp_NazwaCeny2,twp_NazwaCeny3,twp_NazwaCeny4,
  twp_NazwaCeny5,twp_NazwaCeny6,twp_NazwaCeny7,twp_NazwaCeny8,twp_NazwaCeny9,twp_NazwaCeny10)
   ) as Amount) poziomy
   
   join
 (select 
	 tc_idTowar,
	   Wartosc,Pole
from (select tc_idTowar,  tc_cenanetto1 as twp_NazwaCeny1
	  , tc_cenanetto2 as twp_NazwaCeny2
	  , tc_cenanetto3 as twp_NazwaCeny3
	  , tc_cenanetto4 as twp_NazwaCeny4
	  , tc_cenanetto5 as twp_NazwaCeny5
	  , tc_cenanetto6 as twp_NazwaCeny6
	  , tc_cenanetto7 as twp_NazwaCeny7
	  , tc_cenanetto8 as twp_NazwaCeny8
	  , tc_cenanetto9 as twp_NazwaCeny9
	  , tc_cenanetto10 as twp_NazwaCeny10 from tw_cena) k  
unpivot
  (Wartosc for Pole in (
	  [twp_NazwaCeny1]
	  ,[twp_NazwaCeny2]
	  ,[twp_NazwaCeny3]
	  ,[twp_NazwaCeny4]
	  ,[twp_NazwaCeny5]
	  ,[twp_NazwaCeny6]
	  ,[twp_NazwaCeny7]
	  ,[twp_NazwaCeny8]
	  ,[twp_NazwaCeny9]
	  ,[twp_NazwaCeny10]
	  )
   ) as Amount ) ceny on poziomy.pole=ceny.pole where poziomy.wartosc<>'''' ')";
		  string SQL=@"select * from solex_produkty_poziomy_cen pc join tw_CechaTw  tc 
on pc.produkt_id=tc.cht_idtowar join sl_cechatw ctw on tc.cht_idcecha=ctw.ctw_id
where ctw_nazwa=@trait and pc.poziom=@priceLevel";    
		#region IImportDataModule Members

		public void DoWork(System.Collections.Specialized.NameValueCollection configuration, SourceDB db)
		{
			if (this.ProgresChanged != null)
			{
				ProgresChanged(this, new ProgressChangedEventArgs("Początek pobierania promocji"));
			}
			if (this.ProgresChanged != null)
			{
				ProgresChanged(this, new ProgressChangedEventArgs("Ilość rabatów przed " + db.Discounts.Count.ToString()));
			}
				string set = configuration["cecha_stale_ceny"];
				List<DiscountItem> tmp = new List<DiscountItem>();
	  
				string[] items = set.Split(new string[] { ";;" }, StringSplitOptions.RemoveEmptyEntries);
				var customerCatIds = db.CustomerCategories.Select(p => p.id).ToList();
				SqlConnection conn = null;
				SqlCommand cmd = null;
				SqlDataReader rd = null;
				try
				{
				 
					conn = new SqlConnection(configuration["erp_cs"]);
					conn.Open();
					//log.Error("Dokumenty: " + startDate + ";" + startDate2);
					cmd = new SqlCommand(string.Format(SQL_VIEW), conn);
					cmd.CommandTimeout = 600;
					cmd.ExecuteNonQuery();
					cmd.Dispose();
					log.Error(string.Format("Początek stałe ceny {0}", DateTime.Now));
					foreach (string item in items)
					{
						string[] data = item.Split(new string[] { "^^" }, StringSplitOptions.RemoveEmptyEntries);
						cmd = new SqlCommand(string.Format(SQL), conn);
						cmd.Parameters.AddWithValue("@trait", data[0]);
						cmd.Parameters.AddWithValue("@priceLevel", data[1]);

						rd = cmd.ExecuteReader();
					  
						while (rd.Read())
						{
							int id = 0;
							int? price_level = 1;
							int? customerID = (int?)null;
							int? customerCategory = (int?)null;
							DateTime? dateTo = (DateTime?)null;
							DateTime? dateFrom = (DateTime?)null;
							decimal discount = DataHelper.dbd("cena", rd);
							int? productCategory = (int?)null;
							int? product = DataHelper.dbin("produkt_id", rd);
						
							foreach (var cc in customerCatIds)
							{
								int source = -(product.GetValueOrDefault(0) + 1000000 + (100000 + (cc * 50)));
								  if (!tmp.Any(p => p.Id == source))
								  {
									  DiscountItem d = new DiscountItem(source, price_level, customerID, cc,
									   dateFrom, dateTo, discount, productCategory, product, DiscountItemType.Advance, DiscountItemValueType.Numeric);
									  tmp.Add(d);
								  }
							}
						}
						rd.Close();
						rd.Dispose();

					}
					log.Error(string.Format("Koniec stałe ceny {0}", DateTime.Now));
				}
				finally
				{

					if (conn != null) { conn.Close(); conn.Dispose(); }
					if (cmd != null) { cmd.Dispose(); }
					if (rd != null) { rd.Dispose(); }
				}
				db.Discounts.AddRange(tmp);
			if (this.ProgresChanged != null)
			{
				ProgresChanged(this, new ProgressChangedEventArgs("Ilość rabatów po " + db.Discounts.Count.ToString()));
			}
			//cecha_stale_ceny || s_Promocja^^3;;s_Wyprzedaz^^3
			if (this.ProgresChanged != null)
			{
				ProgresChanged(this, new ProgressChangedEventArgs("Zakończenie pobierania promocji"));
			}
		}


		public event ProgressChangedEventHandler ProgresChanged;

		#endregion
	}
}
