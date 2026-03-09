UPDATE Slajd SET LinkUrl = '{Url:'+LinkUrl+',Tryb:ObecneOkno}' WHERE LinkUrl IS NOT NULL AND LinkUrl NOT LIKE '{"Url":"%'

UPDATE KategoriaProduktu SET LinkUrl = '{Url:'+LinkUrl+',Tryb:ObecneOkno}' WHERE LinkUrl IS NOT NULL AND LinkUrl NOT LIKE '{"Url":"%'

UPDATE Sklep SET LinkUrl = '{Url:'+LinkUrl+',Tryb:ObecneOkno}' WHERE LinkUrl IS NOT NULL AND LinkUrl NOT LIKE '{"Url":"%'