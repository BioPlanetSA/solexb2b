-- Usuwanie zamowień które nie mają powiązania z dokumentami 

delete Zamowienie where Id not in (select IdZamowienia from ZamowienieDokumenty)