 IF OBJECT_ID ( 'dbo.[DzialaniaUzytkownikowProcedura]', 'P' ) IS NOT NULL   
 DROP PROCEDURE DzialaniaUzytkownikowProcedura

 Exec('CREATE PROCEDURE [DzialaniaUzytkownikowProcedura] @data datetime, @zdarzenieGlowne varchar(100), @parametr varchar(100)
	AS
	BEGIN
	  select distinct CASE IsNumeric(Replace(Split.a.value(''.'', ''varchar(10)''),CHAR(10),'''')) when 1 then Cast(Replace(Split.a.value(''.'', ''varchar(10)''),CHAR(10),'''') as bigint) Else Replace(Split.a.value(''.'', ''varchar(10)''),CHAR(10),'''')End  from (SELECT dp.[Id],du.ZdarzenieGlowne,dp.NazwaParametru,du.Data, CAST (''<M>'' + REPLACE([Wartosc], '','', ''</M><M>'') + ''</M>'' AS XML) AS String 
	   FROM [DzialaniaUzytkwonikowParametry] dp join DzialaniaUzytkownikow du on dp.IdDzialania=du.Id where du.ZdarzenieGlowne = @zdarzenieGlowne 
	    and dp.NazwaParametru=@parametr and du.Data>=@data) AS A CROSS APPLY String.nodes (''/M'') AS Split(a)
  END')