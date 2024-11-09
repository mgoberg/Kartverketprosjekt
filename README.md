# 🚀 **Kartverket Prosjektet for Gruppe 14!** 🚀

## **For å sette opp prosjektet og koble opp databasen, følg disse trinnene:**

### 🌿 **Trinn 1: Sørg for å være i riktig branch**  
- Sjekk at du er på **dev** branch inntil videre. 
- Dev branch er somregel 20-50 commits foran main.

### 🛠️ **Trinn 2: Kjør prosjektet i Visual Studio**  
Start prosjektet i dockerfile modus slik at docker containeren som kjører applikasjonen kjører.

### 🌐 **Trinn 3: Opprett et Docker-nettverk**  
For å opprette et nettverk, kjør denne kommandoen:  
`docker network create kartverket-network`

### 🔗 **Trinn 4: Koble applikasjonscontaineren til nettverket**  
Koble webapplikasjonen til nettverket med:  
`docker network connect kartverket-network kartverketprosjekt`

### 🚀 **Trinn 5: Start databasen**  
Bygg og start databasen med:  
`docker-compose up --build`

### 🎊 **Da var alt klart!**  
Nå har du en databasecontainer som kjører i Docker. Du kan starte applikasjonen og teste all funksjonalitet.

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
- **Kartintegrasjon med Leaflet**: Alle saker vises på et interaktivt kart.
- **GeoJSON-støtte**: Brukerinnsendte data konverteres og vises som GeoJSON på kartet for saksbehandlere.
- **Eiendomsinndeling**: Eiendomsgrenser kan toggles i kartet.
- **Oppmerking av veier**: Veier kan toggles i kartet.
- **Kartlag**: Kartlag: Topografisk kart som standard, gråtone, turkart og sjøkart.


### 🤖 **Ekstra funksjonalitet**
- **Slack/Discord Bot**: Automatiske oppdateringer sendes til en kanal når nye saker blir rapportert.
- **Dashboard**: Oversikt over alle rapporterte saker, med søk og filtrering.

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
