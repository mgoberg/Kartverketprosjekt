🚀 Kartverket Prosjektet! 🚀

For å kunne benytte deg av funksjonalitet som krever database på prosjektet, må du følge disse enkle trinnene:

🛠️ Trinn 1: Start prosjektet  
Start prosjektet og stopp det. (I **DOCKERFILE MODUS**)

🌿 Trinn 2: Sørg for å være i dev branch  
Sjekk at du er på **dev** branch (inntil videre).

🌐 Trinn 3: Opprett et Docker-nettverk  
Kjør følgende kommando for å opprette et nettverk:  
`docker network create kartverket-network`

🔗 Trinn 4: Koble applikasjonscontaineren til nettverket  
Kjør denne kommandoen:  
`docker network connect kartverket-network kartverketprosjekt`

🚀 Trinn 5: Start databasen  
Kjør denne kommandoen for å bygge og starte databasen:  
`docker-compose up --build`

🎊 Gratulerer!  
Da skal du ha fått en container i Docker som hoster databasen. Nå kan du starte applikasjonen og prøve å registrere en bruker.

For å sjekke at brukeren er opprettet i databasen, kan du gå inn på Docker Desktop, trykke på pila under den andre containeren, trykke på db-1, gå til exec, og skrive:  
`mariadb -u root -p` (passord: root).

Deretter må du skrive:  
`USE kartverketdb;`  
Og til slutt:  
`SELECT * FROM Bruker;`

Preview av index:
![image](https://github.com/user-attachments/assets/78baa97f-8d91-4853-b8d1-2d132f1a034a)

Preview av saksbehandler dashboard:
![image](https://github.com/user-attachments/assets/8ffa36b5-b8a8-493d-91fc-c891d851a5ab)


