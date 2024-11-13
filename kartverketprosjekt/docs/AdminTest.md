# **Admin test scenario**
1. **Logg inn** som admin (bruk: **testadmin@example.com, Test**).
2. **Gå til administrator side** ved å trykke på **Admin** i navigasjonsbaren.
3. **Bekreft** at statistikk tabell og bruker tabell samsvarer med databasen (dette kan gjøres ved å gå inn i containeren som kjører databasen _**(db-1**)_ og kjøre følgene kommandoer i **exec** tab: `mariadb -u root -p` (passord er root), `use kartverketdb` , `select * from Bruker;` , `select * from Sak;`.)
4. **Endre på tilgansnivå til en bruker** og bekreft dette i databasen (`select * from Bruker;`).
5. **Slett en bruker** _(merk at for å kunne slette en bruker må du først **slette** saken som er tilknyttet, dette kan gjøres i saksbehandler view)_ og **bekreft** i databasen (`select * from Bruker;`).
6. **Opprett en prioritert bruker**, dette gjøres ved å fylle ut skjemaet nederst på admin siden og velge **tilgangsnivå 2** (prioritert bruker). **Bekreft** i databasen at brukeren er opprettet og at **tilgansnivå = 2**:
```sql
SELECT *
FROM Bruker
WHERE tilgangsnivaa_id = 2;
```

 **Hvis alle punkter kan gjennomføres er test-scenarioet: *Bestått***
 ---

> [!IMPORTANT]
> Denne testen bygger videre på **saksbehandler testen** og vi går _**ikke**_ gjennom funskjoner som blir gjort der.
