-------------------------------------------------
-- STWORZYŁ: BARTEK
-- POWÓD: poprawka zeby widocznosc typow mogla miec NULL w kategorie klientów - odtychczas bez sensu byla zapisywana pusta lista []. Tabela nie jest CDC więc prosty SQL
--------------------------------------------------

ALTER TABLE WidocznosciTypow ALTER COLUMN KategoriaKlientaIdWszystkie varchar(400) NULL;
ALTER TABLE WidocznosciTypow ALTER COLUMN KategoriaKlientaIdKtorakolwiek varchar(400) NULL