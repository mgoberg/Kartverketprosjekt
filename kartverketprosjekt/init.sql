-- Opprette tabell for tilgangsnivåer
CREATE TABLE Tilgangsnivaa (
    id INT AUTO_INCREMENT PRIMARY KEY,
    level_name VARCHAR(50) NOT NULL -- Navn på tilgangsnivå
);

-- Opprette tabell for kommuner
CREATE TABLE Kommune (
    id INT AUTO_INCREMENT PRIMARY KEY,
    navn VARCHAR(100) NOT NULL
);

-- Opprette tabell for brukere
CREATE TABLE Bruker (
    epost VARCHAR(100) PRIMARY KEY NULL,
    navn VARCHAR(100) NULL,
    passord VARCHAR(255) NOT NULL,
    tilgangsnivaa_id INT,
    organisasjon VARCHAR(100), -- Organisasjon gjelder kun hvis tilgangsnivå = 2 (Prioritert Bruker)
    kommune_id INT, -- Kommune gjelder kun hvis tilgangsnivå = 3 (Saksbehandler)
    FOREIGN KEY (tilgangsnivaa_id) REFERENCES Tilgangsnivaa(id),
    FOREIGN KEY (kommune_id) REFERENCES Kommune(id)
);


-- Opprette tabell for saker
CREATE TABLE Sak (
    id INT AUTO_INCREMENT PRIMARY KEY,
    epost_bruker VARCHAR(100) NULL, -- Referanse til brukeren som rapporterer saken
    beskrivelse TEXT NOT NULL,
    vedlegg VARCHAR(255) NULL, -- Binærdata for vedlegg
    geojson_data JSON, -- Inneholder kartdata i JSON-format
    kommune_id INT, -- Referanse til kommunen
    type_feil VARCHAR(50) NOT NULL,
    status_endret BOOLEAN DEFAULT FALSE, -- Indikerer om status er endret
    status ENUM('Ubehandlet', 'Under Behandling', 'Løst', 'Avvist', 'Arkivert') NOT NULL,
    opprettet_dato TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    layerurl VARCHAR(255) NULL, -- URL til kartlaget
    Kommunenavn VARCHAR(255),      -- Feltene for kommuneinfo
    Kommunenummer VARCHAR(50),      -- Kan være en string, avhengig av hvordan nummeret lagres
    Fylkesnavn VARCHAR(255),
    Fylkesnummer VARCHAR(50),
    IsPriority BOOLEAN DEFAULT FALSE, -- Nytt felt for å indikere om saken er prioritert
    saksbehandler_id VARCHAR(100), -- Nytt felt for å lagre epost til saksbehandler
    FOREIGN KEY (epost_bruker) REFERENCES Bruker(epost),
    FOREIGN KEY (kommune_id) REFERENCES Kommune(id), -- Relasjon til Kommune-tabellen
    FOREIGN KEY (saksbehandler_id) REFERENCES Bruker(epost)
);  

CREATE TABLE Kommentar (
    Id INT AUTO_INCREMENT PRIMARY KEY, -- Bruk AUTO_INCREMENT i stedet for IDENTITY
    Tekst TEXT NOT NULL, -- Tekst datatype i MySQL
    Dato DATETIME NOT NULL, -- Datoen kommentaren ble opprettet
    SakId INT NOT NULL, -- Fremmednøkkel til Sak-tabellen
    FOREIGN KEY (SakId) REFERENCES Sak(id) -- Relasjon til Sak-tabellen
);
ALTER TABLE Kommentar
ADD epost VARCHAR(50);

ALTER TABLE Kommentar
ADD CONSTRAINT FK_Kommentar_Saksbehandler
FOREIGN KEY (epost) REFERENCES Bruker(epost);


-- Opprette tabell for tilbakemeldinger fra brukere (IKKE I BRUK)
CREATE TABLE Tilbakemelding (
    id INT AUTO_INCREMENT PRIMARY KEY,
    epost_bruker VARCHAR(100), -- Relasjon til brukeren som sender tilbakemelding
    emne VARCHAR(255) NOT NULL,
    beskrivelse TEXT NOT NULL,
    vedlegg LONGBLOB, -- Binærdata for vedlegg
    opprettet_dato TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (epost_bruker) REFERENCES Bruker(epost) -- Relasjon til Bruker-tabellen via epost
);

INSERT INTO Kommune (navn) VALUES
('Agder'),
('Akershus'),
('Aust-Agder'),
('Buskerud'),
('Finnmark'),
('Hedmark'),
('Hedmark'),
('Hordaland'),
('Møre og Romsdal'),
('Nord-Trøndelag'),
('Nordland'),
('Oppland'),
('Oslo'),
('Rogaland'),
('Sogn og Fjordane'),
('Sør-Trøndelag'),
('Telemark'),
('Vest-Agder'),
('Vestfold'),
('Østfold'),
('Bergen'),
('Stavanger'),
('Kristiansand'),
('Drammen'),
('Bodø'),
('Arendal'),
('Tromsø'),
('Ålesund'),
('Sandnes'),
('Haugesund'),
('Skien'),
('Lillehammer'),
('Larvik'),
('Hamar'),
('Gjøvik'),
('Mo i Rana'),
('Namsos'),
('Horten'),
('Notodden'),
('Lillestrøm'),
('Porsgrunn'),
('Ås'),
('Kongsberg'),
('Halden'),
('Fredrikstad'),
('Moss'),
('Nettuno'),
('Hønefoss'),
('Bærum'),
('Asker'),
('Nittedal'),
('Bodø'),
('Alta'),
('Kirkenes'),
('Narvik'),
('Vadsø'),
('Harstad'),
('Kongsvinger'),
('Stjørdal'),
('Rana'),
('Volda'),
('Fauske'),
('Lillesand'),
('Bø'),
('Lærdal'),
('Os'),
('Suldal'),
('Sogndal'),
('Stryn'),
('Førde'),
('Midsund'),
('Vigrestad'),
('Voss'),
('Sæbø'),
('Vang'),
('Sunde'),
('Søgne'),
('Stord'),
('Eigersund'),
('Holmestrand'),
('Haugesund'),
('Porsgrunn'),
('Sandefjord'),
('Sarpsborg'),
('Fredrikstad'),
('Rygge'),
('Halden'),
('Moss'),
('Lier'),
('Kongsberg'),
('Asker'),
('Nordre Follo'),
('Nesodden'),
('Vestby'),
('Ski'),
('Ås'),
('Frogn'),
('Lørenskog'),
('Nittedal'),
('Bærum'),
('Nes'),
('Hurdal'),
('Aurskog-Høland'),
('Gjerdrum'),
('Rælingen'),
('Enebakk'),
('Lørenskog'),
('Skedsmo'),
('Moss'),
('Rygge'),
('Nordre Follo'),
('Oslo'),
('Lillehammer'),
('Ås'),
('Drammen'),
('Bærum'),
('Asker'),
('Gjøvik'),
('Hamar'),
('Haugesund'),
('Lillehammer'),
('Stavanger'),
('Bergen'),
('Tromsø'),
('Bodø'),
('Kristiansand'),
('Porsgrunn'),
('Skien'),
('Færder'),
('Vardø'),
('Ringerike'),
('Vestre Toten'),
('Nord-Fron'),
('Lesja'),
('Sel'),
('Gausdal'),
('Lillehammer'),
('Nord-Aurdal'),
('Øyer'),
('Sogndal'),
('Vik'),
('Holmestrand'),
('Larvik'),
('Horten'),
('Sandefjord'),
('Rygge'),
('Halden'),
('Moss'),
('Sarpsborg'),
('Fredrikstad'),
('Oslo');

-- Insert predefined tilgangsnivåer (Bruker, Prioritert Bruker, Saksbehandler, Administrator)
INSERT INTO Tilgangsnivaa (level_name)
VALUES 
    ('Bruker'),          -- ID = 1
    ('Prioritert Bruker'),-- ID = 2
    ('Saksbehandler'),    -- ID = 3
    ('Administrator');    -- ID = 4

-- Insert a test user for each access level

-- Testbruker med tilgangsnivå 'Bruker'
INSERT INTO Bruker (epost, navn, passord, tilgangsnivaa_id)
VALUES ('testbruker@example.com', 'TestBruker', 'Test', 1);

-- Testbruker med tilgangsnivå 'Prioritert Bruker'
INSERT INTO Bruker (epost, navn, passord, tilgangsnivaa_id, organisasjon)
VALUES ('testprio@example.com', 'TestPrioritertBruker', 'Test', 2, 'Statens Vegvesen');

-- Testbruker med tilgangsnivå 'Saksbehandler'
INSERT INTO Bruker (epost, navn, passord, tilgangsnivaa_id, kommune_id)
VALUES ('testsaksbehandler@example.com', 'TestSaksbehandler', 'Test', 3, 1);

-- Testbruker med tilgangsnivå 'Saksbehandler'
INSERT INTO Bruker (epost, navn, passord, tilgangsnivaa_id, kommune_id)
VALUES ('testsaksbehandler2@example.com', 'TestSaksbehandler2', 'Test', 3, 1);

-- Testbruker med tilgangsnivå 'Administrator'
INSERT INTO Bruker (epost, navn, passord, tilgangsnivaa_id)
VALUES ('testadmin@example.com', 'TestAdministrator', 'Test', 4);


