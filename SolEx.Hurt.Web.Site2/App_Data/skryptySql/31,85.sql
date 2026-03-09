
delete from [dbo].[Tlumaczenie] where typ  = 'SolEx.Hurt.Model.TlumaczeniePole,SolEx.Hurt.Model' AND   ObiektId in ( 
	select id from [TlumaczeniePole] where nazwa in ( select nazwa  from [dbo].[TlumaczeniePole] group by nazwa having count(nazwa ) >1 )
);

delete from [TlumaczeniePole] where nazwa in (  select nazwa  from [dbo].[TlumaczeniePole] group by nazwa having count(nazwa ) >1  );