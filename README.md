# 🚀 **Kartverket Prosjektet for Gruppe 14!** 🚀
## **For å sette opp prosjektet og koble opp databasen, følg disse trinnene:**

### 🌿 **Trinn 1: Sørg for å være i riktig branch**  
- Sjekk at du er på **dev** branch inntil videre. 
- Dev branch er somregel 20-50 commits foran main.

### 🛠️ **Trinn 2: Kjør prosjektet i Visual Studio**  
Start prosjektet i dockerfile modus slik at docker containeren som kjører applikasjonen kjører.

### 🌐 **Trinn 3: Opprett et Docker-nettverk**  
For å opprette et nettverk, kjør denne kommandoen:

    docker network create kartverket-network

### 🔗 **Trinn 4: Koble applikasjonscontaineren til nettverket**  
Koble webapplikasjonen til nettverket med:

    docker network connect kartverket-network kartverketprosjekt

### 🚀 **Trinn 5: Start databasen**  
Bygg og start databasen med:

    docker-compose up --build

### 🎊 **Da var alt klart!**  
Nå har du en databasecontainer som kjører i Docker. Du kan starte applikasjonen og teste all funksjonalitet.

> [!WARNING]
> Hvis det er gjort endringer i database struktur vil du måtte bygge databasen på nytt.
> Dette kan gjøres ved å kjøre:
>
> `docker-compose down`deretter:   `docker-volume prune`
>
> **Merk deg at dette fjerner *alle* tidligere oppføringer i databasen**


---

## **Funksjonalitet**

### 🔍 **Brukerfunksjoner**
- **Registrering og innlogging**: Brukere kan opprette en konto og logge inn.
- **Rapportering av kartfeil**: Brukere kan markere feil i kartet med, legge til beskrivelse og sende inn.
- **Visning av innmeldte saker**: Brukere kan se egne innmeldte saker med statusoppdateringer og kommentarer fra saksbehandlere.
- **Sletting av saker**: Brukere kan administrere sine egene saker.
- **Oppdatering av Navn og Passord**: Brukere kan oppdatere navnet sitt, eller endre passordet sitt.
- **Notifikasjoner**: Brukere som har fått en endring på saken sin vil få en varsel og en notifikasjon på meny knappen.

### 🛠️ **Saksbehandlerfunksjoner**
- **Administrering av saker**: Saksbehandlere kan se, endre status, deligere og slette saker.
- **Tilbakemelding**: Saksbehandlere kan gi brukere tilbakemelding i et kommentarfelt.
- **Automatisk sakstildeling**: Saker blir automatisk tildelt saksbehandler med færrest saker, og kan videre deligeres derfra.
- **Visning av vedlegg**: Saksbehandler kan enkelt se vedlegg som ligger knyttet til en sak.

### 🛠️ **Administratorfunksjoner**
- **Saksbehandling**: Administratorer har all funksjonalitet en saksbehandler har.
- **Brukeradministrasjon**: Administrator kan oppdatere tilgangsnivå til bruker, og slette brukere.
- **Oppretting av brukere**: Administrator kan opprette brukere.
- **System stats**: Administrator har tilgang på et stats-bord som innholder all statistikk for systemet.

### 🌐 **Geofunksjoner**
- **Kartintegrasjon med Leaflet[^1]**: Alle saker vises på et interaktivt kart.
- **GeoJSON-støtte**: Brukerinnsendte data konverteres og vises som GeoJSON på kartet for saksbehandlere.
- **Eiendomsinndeling[^2]**: Eiendomsgrenser kan toggles i kartet.
- **Oppmerking av veier[^3]**: Veier kan toggles i kartet.
- **Kartlag**[^4]: vekslbart kartlag: Topografisk kart som standard, gråtone, turkart og sjøkart.
> [!NOTE]
> **Alle Kartlagstjenester er fra kartverkets datasett og oppdateres jevnlig.**

### 🤖 **Ekstra funksjonalitet**
- **Slack/Discord Bot**: Automatiske oppdateringer sendes til en kanal når nye saker blir rapportert.
- **Dashboard**: Oversikt over alle rapporterte saker, med søk og filtrering
---
## **System arkitektur**
### **MVC-modellen i .NET**:

  I dette prosjektet bruker vi *Model-View-Controller (MVC)* mønsteret, som er et designmønster ofte brukt i .NET-applikasjoner. MVC-modellen hjelper til med å skille ansvar innen applikasjonen:

- **Model**: Representerer applikasjonens data og forretningslogikk. Modellen er ansvarlig for å hente data fra databasen, behandle dem og sende dem til kontrolleren.
- **View**: Representerer brukergrensesnittet. Visningen viser data fra modellen til brukeren og sender brukerinput til kontrolleren.
- **Controller**: Fungerer som en mellommann mellom modellen og visningen. Kontrolleren mottar input fra visningen, behandler den, og returnerer den passende visningen som svar.
  
  Denne separasjonen av bekymringer gjør applikasjonen mer modulær, enklere å teste og vedlikeholde. Det lar utviklere jobbe med forskjellige deler av applikasjonen samtidig uten å forstyrre hverandre.
---
## **Testing**
### **Unit Testing:**
  *Ikke enda implementert:*
- [ ] Kontrollere
- [ ] API modeller
- [ ] Javascript funskjoner
- [ ] Database initialisering
- [ ] Docker
      
### **Testing scenarioer:**
Se egen mappe for dette:
[Testing scenarioer](docs/)

---
## **Forhåndsvisning av prosjektet**

### **1. Index-side**  
![image](https://github.com/user-attachments/assets/c4890ea1-9d3a-492d-9b81-b1982cf50445)


### **2. Innmeldingsside**  
![Innmeldingsside preview](https://github.com/user-attachments/assets/8e177b23-2729-4c4f-baca-30d7b2c3bee4)

### **3. Saksbehandler Dashboard**  
![Dashboard preview](https://github.com/user-attachments/assets/8ffa36b5-b8a8-493d-91fc-c891d851a5ab)

### **4. Discord/Slack Bot**  
![Bot preview](https://github.com/user-attachments/assets/e1738455-0a17-4ef2-bbef-2113d2fc8618)


[^1]: Leaflet javascript bilbiotek: [Leaflet](https://leafletjs.com/)
[^2]: WMS-kartoverleggs-tjeneste (vegnett) [geonorge](https://www.geonorge.no/)
[^3]: WMS-kartoverleggs-tjeneste (matrikkel) [geonorge](https://kartkatalog.geonorge.no/metadata/matrikkelkart-wms/30dda4c6-2cba-4378-b2e7-26f644df9d99)
[^4]: WMS-kartlag-tjeneste [Kartverket](https://kartkatalog.geonorge.no/metadata/vegnett2-wms/302fcb0e-a7dc-44f4-a336-8c9ee9709d73?search=vegnett)


