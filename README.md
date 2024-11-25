# 🚀 **Kartverket Prosjektet for Gruppe 14!** 🚀
## **For å sette opp prosjektet og koble opp databasen, følg disse trinnene:**

## 🌿 **Trinn 1: Sørg for å være i riktig branch**  
Sjekk at du er på **Main** branch 

## 🛠️ **Trinn 2: Kjør prosjektet i Visual Studio**  
Start prosjektet i dockerfile modus slik at docker containeren som kjører applikasjonen kjører.

## 🌐 **Trinn 3: Opprett et Docker-nettverk**  
For å opprette et nettverk, kjør denne kommandoen:

```ruby
docker network create kartverket-network
```

## 🔗 **Trinn 4: Koble applikasjonscontaineren til nettverket**  
Koble webapplikasjonen til nettverket med:'

```ruby
docker network connect kartverket-network kartverketprosjekt
```
## 🚀 **Trinn 5: Start databasen**  
Bygg og start databasen med:

```ruby
docker-compose up --build
```

## 🎊 **Da var alt klart!**  
Nå har du en databasecontainer som kjører i Docker. Du kan starte applikasjonen og teste all funksjonalitet.
Det finnes 4 test-brukere i systemet.
- **testbruker@example.com** med passord: **Test**
- **testadmin@example.com** med passord: **Test**
- **testsaksbehandler@example.com** med passord: **Test**
- **testprio@example.com** med passord: **Test**

---

> [!WARNING]
> Hvis det er gjort endringer i database struktur vil du måtte bygge databasen på nytt.
> Dette kan gjøres ved å kjøre:
>
> `docker-compose down`deretter:   `docker-volume prune`
>
> **Merk deg at dette fjerner *alle* tidligere oppføringer i databasen**

---

# **Funksjonalitet**

## 🔍 **Brukerfunksjoner**
- **Registrering og innlogging**: Brukere kan opprette en konto og logge inn.
- **Rapportering av kartfeil**: Brukere kan markere feil i kartet med **Linjer**, **Polygons**, **Flater**, **Punkter** og legge til beskrivelse.
- **Visning av innmeldte saker**: Brukere kan se egne innmeldte saker med statusoppdateringer og kommentarer fra saksbehandlere.
- **Sletting av saker**: Brukere kan administrere sine egene saker.
- **Oppdatering av Navn og Passord**: Brukere kan oppdatere navnet sitt, eller endre passordet sitt.
- **Notifikasjoner**: Brukere som har fått en endring på saken sin vil få en varsel og en notifikasjon på meny knappen.

## 🛠️ **Saksbehandlerfunksjoner**
- **Administrering av saker**: Saksbehandlere kan se, endre status, deligere og slette saker.
- **Tilbakemelding**: Saksbehandlere kan gi brukere tilbakemelding i et kommentarfelt.
- **Automatisk sakstildeling**: Saker blir automatisk tildelt saksbehandler med færrest saker, og kan videre deligeres derfra.
- **Visning av vedlegg**: Saksbehandler kan enkelt se vedlegg som ligger knyttet til en sak.

## 🛠️ **Administratorfunksjoner**
- **Saksbehandling**: Administratorer har all funksjonalitet en saksbehandler har.
- **Brukeradministrasjon**: Administrator kan oppdatere tilgangsnivå til bruker, og slette brukere.
- **Oppretting av brukere**: Administrator kan opprette brukere.
- **System stats**: Administrator har tilgang på et stats-bord som innholder all statistikk for systemet.

## 🌐 **Geofunksjoner**
- **Kartintegrasjon med Leaflet[^1]**: Alle saker vises på et interaktivt kart.
- **GeoJSON-støtte**: Brukerinnsendte data konverteres og sendes som GeoJSON for lagring i database
- **Eiendomsinndeling[^2]**: Matrikkel data kan toggles i kartet.
- **Oppmerking av veier[^3]**: Vegnett data kan toggles i kartet.
- **Kartlag**[^4]: vekslbart kartlag: Kartverkets offisielle kartlag som vekslbare lag _(Topografisk kart som standard, gråtone, turkart og sjøkart)_.
> [!NOTE]
> **Alle Kartlagstjenester er fra kartverkets datasett og oppdateres jevnlig.**

## 🤖 **Ekstra funksjonalitet**
- **Slack/Discord Bot**: Automatiske oppdateringer sendes til en kanal når nye saker blir rapportert.
- **Dashboard**: Oversikt over alle rapporterte saker, med søk og filtrering
---
# **System arkitektur**
## **MVC-modellen i .NET**:

I dette prosjektet bruker vi *Model-View-Controller (MVC)* mønsteret, som er et designmønster ofte brukt i .NET-applikasjoner. MVC-modellen hjelper til med å skille ansvar innen applikasjonen:

- **Model**: Representerer applikasjonens data. Modellen er ansvarlig for å hente data fra databasen, behandle dem og sende dem til kontrolleren.
- **View**: Representerer brukergrensesnittet. Visningen viser data fra modellen til brukeren og sender brukerinput til kontrolleren.
- **Controller**: Fungerer som en mellommann mellom modellen og visningen. Kontrolleren mottar input fra visningen, behandler den, og returnerer den passende visningen som svar.
  
# Controller-Service-Repository mønster
## Controller _(Ruting)_
I vårt prosjekt håndterer controlleren HTTP-forespørsler og **kobler sammen brukerens handlinger med applikasjonens forretningslogikk**. En controller er ansvarlig for å motta forespørsler fra brukeren, bearbeide forespørselen ved hjelp av en **service**, og deretter returnere et passende svar.

**Eksempel på controller metode:**

```c#
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> UpdateAccess(string userId, int newAccessLevel)
{
    var (success, message) = await _adminService.UpdateUserAccessAsync(userId, newAccessLevel);
    if (success)
    {
        TempData["SuccessMessage"] = message;
    }
    else
    {
        TempData["ErrorMessage"] = message;
    }

    return RedirectToAction("AdminView");
}
```

## Service _(Forretningslogikk)_
Service-laget er **ansvarlig for applikasjonens forretningslogikk**. Dette laget utfører operasjoner som er nødvendige for å oppfylle kravene i applikasjonen, som å **hente**, **oppdatere**, eller **slette data**, samt **interagere med eksterne tjenester eller systemer**. Service-laget er der forretningsreglene implementeres.

**Hva gjør en Service?**
- **Utføre** forretningslogikk knyttet til applikasjonens spesifikke krav.
- **Kommunisere** med repository-laget for å hente, lagre eller oppdatere data i databasen.
- **Integrere** med andre systemer eller tjenester (f.eks. API-er, tredjepartsbiblioteker).
- **Håndtere** eventuelle feil som kan oppstå under forretningslogikkprosessen.
- **Returnere** resultater eller feilmeldinger til controlleren.

Service-laget gjør at controlleren kan være enklere og mer fokusert på HTTP-håndtering, samtidig som det holder applikasjonens forretningslogikk samlet og lettere å vedlikeholde og teste.

**Eksempel på Service metode:**

```c#
public async Task<(bool Success, string Message)> UpdateUserAccessAsync(string userId, int newAccessLevel)
{
    var user = await _brukerRepository.GetUserByIdAsync(userId);
    if (user != null)
    {
        user.tilgangsnivaa_id = newAccessLevel;
        await _brukerRepository.SaveChangesAsync();
        return (true, $"Endret tilgangsnivå for {userId} til: {newAccessLevel}");
    }

    return (false, "Bruker ikke funnet.");
}
```
## Repository _(Databaseoperasjoner)_
I dette prosjektet bruker vi **Repository Pattern** for å **isolere data-adgangslaget fra applikasjonens forretningslogikk**. Dette gir flere fordeler, som **lettere testing**, **bedre struktur** og muligheten til å **bytte ut lagringsmekanismer** 

Repository-laget fungerer som et **mellomledd mellom applikasjonen og datalaget** (f.eks. en database). Det håndterer all interaksjon med databasen, som å **hente, lagre, oppdatere eller slette** data. Ved å bruke repositories kan vi gjøre applikasjonen vår **mer modulær**, noe som gjør det **enklere å teste**, **vedlikeholde** og **endre datatilgangslaget** uten å påvirke resten av applikasjonen.

**Eksempel på Repository metoder:**
```c#
public async Task<BrukerModel?> GetUserByIdAsync(string userId)
{
    return await _context.Bruker.FindAsync(userId);
}

public async Task SaveChangesAsync()
{
    await _context.SaveChangesAsync();
}
```

> [!NOTE]
> **Controller**-**Service**-**Repository** mønsteret er _**ikke**_ istedet for MVC, men heller en utvidelse av controllerene for å **øke modularitet**, **skalerbarhet**, **testbarhet** og for å **løsne tette koblinger**.

# ORM og Entity Framework i Prosjektet

## Hva er en ORM?
**Object-Relational Mapping (ORM)** er en teknikk som brukes til å koble objektorientert programmering med relasjonsdatabaser. Ved hjelp av en ORM kan utviklere arbeide med databaser ved å bruke objekter i stedet for rå SQL-spørringer. ORM-verktøy forenkler prosesser som:

- Opprettelse, lesing, oppdatering og sletting av data (CRUD-operasjoner).
- Konvertering mellom database-tabeller og objektmodeller.
- Håndtering av relasjoner mellom tabeller.

## Hva er Entity Framework?
**Entity Framework (EF)** er en ORM for .NET-applikasjoner som lar utviklere arbeide med databasen ved å bruke C#-klasser. EF gir følgende fordeler:

- **Produktivitet**: Reduserer behovet for å skrive SQL manuelt.
- **Type-sikkerhet**: Data håndteres som sterke typer i C#.
- **Relasjonsstyring**: Håndterer relasjoner (f.eks. én-til-mange, mange-til-mange) automatisk.

### Modeller i EF
Entity Framework representerer tabeller som C#-klasser, hvor kolonner blir egenskaper og rader blir objekter. For eksempel:

```csharp
public class BrukerModel
{
    public string epost { get; set; }
    
    public string? navn { get; set; }
   
    public string passord { get; set; }

    public int tilgangsnivaa_id { get; set; }
}

```

### Entity Framework uten Migrations
I prosjektet vårt bruker vi **Entity Framework uten migrations**. Dette betyr at vi ikke lar EF automatisk opprette eller oppdatere databasen. I stedet håndterer vi databaseendringer manuelt. 

#### Begrunnelser for å unngå migrations:
1. **Kontroll over databasestruktur**:
   - Ved å bruke scriptede databaseendringer har vi full kontroll over hvordan databasen endres i produksjon.
   - Migrations kan introdusere uforutsette feil ved komplekse databaseendringer.

2. **Kompatibilitet med eksisterende databaser**:
   - Hvis databasen allerede er i bruk, unngår vi risikoen for at migrations forårsaker uforenlige endringer.

3. **Kodebase og database synkronisering**:
   - Vi sikrer at endringer i datamodellene alltid er nøye vurdert og implementert eksplisitt i både kode og database.

4. **Bedre forståelse av databasen**:
   - Manuell styring av databasen gir utviklere bedre innsikt i hvordan databasen fungerer.

#### Ulemper ved å ikke bruke migrations:
- Krever mer arbeid ved opprettelse og endring av databasen.
- Feil kan oppstå dersom scripts og koden ikke holdes i synkronisering.

### Eksempel på opprettelse av tabeller uten migrations
I stedet for migrations bruker vi SQL-skript for å opprette og oppdatere tabeller. For eksempel:

```sql
CREATE TABLE Bruker (
    epost VARCHAR(100) PRIMARY KEY,
    navn VARCHAR(100) NULL,
    passord VARCHAR(255) NOT NULL,
    tilgangsnivaa_id INT,
    FOREIGN KEY (tilgangsnivaa_id) REFERENCES Tilgangsnivaa(id),
);

```

### Fordeler med Entity Framework i prosjektet
- **Enklere dataoperasjoner**: Reduserer kompleksiteten ved CRUD-operasjoner.
- **Lesbarhet**: Koden er enklere å lese sammenlignet med rå SQL.
- **Skalerbarhet**: Kan enkelt tilpasses nye datamodeller.

---

## Sikkerhet: CSRF og XSS-beskyttelse
I prosjektet håndteres beskyttelse mot Cross-Site Request Forgery (CSRF) ved å bruke ASP.NET Core sitt innebygde CSRF-beskyttelsessystem, som automatisk genererer og validerer CSRF-tokens for alle sensitive POST-forespørsler. Dette sikrer at kun legitime brukere kan sende inn data til applikasjonen.

```c#
[ValidateAntiForgeryToken]
```
```js
headers: {
    'RequestVerificationToken': token
}
```
```js
const token = $('input[name="__RequestVerificationToken"]').val();
```
For å forhindre Cross-Site Scripting (XSS) benyttes ASP.NET Core sitt sanitizing-system, som automatisk rømmer farlige HTML-tegntokens i brukerinndata før de vises i nettleseren. I tillegg brukes "Content Security Policy" (CSP) for å beskytte mot ondsinnet scriptkjøring fra eksterne kilder.
```c#
@Html.Encode(TempData["Message"])
```
---
# **Testing**
## **Enhetstesting (Unit Testing):**

Enhetstesting er en viktig del av utviklingsprosessen som sikrer at individuelle deler av applikasjonen fungerer som forventet. I dette prosjektet har vi brukt enhetstester til å validere logikken i våre tjenester, inkludert API-integrasjoner. Dette hjelper oss å identifisere feil tidlig i utviklingssyklusen og sikrer pålitelighet.

## Teknologier som brukes
- **xUnit**: For å strukturere og kjøre testene.
- **Moq**: For mocking av avhengigheter
- **RichardSzalay.MockHttp**: For å simulere HTTP-forespørsler og -svar uten å koble til eksterne APIer.

## Eksempel: KommuneInfoService
En av tjenestene vi tester er `KommuneInfoService`, som henter informasjon om kommune og fylke fra et Kartverkets API basert på et geografisk punkt.

### Positivt Scenario
Vi tester at `GetKommuneInfoAsync` returnerer riktig informasjon når API-svaret er vellykket. Ved hjelp av `MockHttpMessageHandler` simulerer vi et gyldig JSON-svar fra APIet og verifiserer at tjenesten parser dette korrekt.

```csharp
[Fact]
public async Task GetKommuneInfoAsync_ReturnsKommuneInfo_WhenApiResponseIsSuccessful()
{
    // Arrange
    var expectedKommuneInfo = new KommuneInfo
    {
        KommuneNavn = "Oslo",
        Fylkesnavn = "Oslo",
        Kommunenummer = "0301",
        Fylkesnummer = "03"
    };

    var jsonResponse = JsonSerializer.Serialize(expectedKommuneInfo);

    _httpMessageHandlerMock.When("http://example.com/punkt*")
                           .Respond("application/json", jsonResponse);

    // Act
    var result = await _kommuneInfoService.GetKommuneInfoAsync(1000, 2000, 1);

    // Assert
    Assert.NotNull(result);
    Assert.Equal(expectedKommuneInfo.KommuneNavn, result.KommuneNavn);
    Assert.Equal(expectedKommuneInfo.Fylkesnavn, result.Fylkesnavn);
    Assert.Equal(expectedKommuneInfo.Kommunenummer, result.Kommunenummer);
    Assert.Equal(expectedKommuneInfo.Fylkesnummer, result.Fylkesnummer);
}
```

### Negativt Scenario
Vi tester at `GetKommuneInfoAsync` returnerer `null` og logger en feil når API-svaret mislykkes (f.eks. HTTP 500).

```csharp
[Fact]
public async Task GetKommuneInfoAsync_ReturnsNull_WhenApiResponseFails()
{
    // Arrange
    _httpMessageHandlerMock.When("http://example.com/punkt*")
                           .Respond(HttpStatusCode.InternalServerError);

    // Act
    var result = await _kommuneInfoService.GetKommuneInfoAsync(1000, 2000, 1);

    // Assert
    Assert.Null(result);
    _loggerMock.Verify(
        log => log.Log(
            LogLevel.Error,
            It.IsAny<EventId>(),
            It.IsAny<It.IsAnyType>(),
            It.IsAny<Exception>(),
            (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
        Times.Once);
}
```

## Hvordan kjøre testene
For å kjøre testene kan du bruke følgende kommando:

```bash
> dotnet test
```

Dette vil kjøre alle enhetstestene i prosjektet og gi deg en rapport om resultatene. Eventuelle feil vil bli logget slik at du kan feilsøke dem.
      
### **Testing scenarioer:**
Vi har også lagd flere testing scenarioer. Dette er trinnvise tester som gjennomføres manuelt for å sikre forventet oppførsel av systemets funskjoner.
#### Se egen mappe for dette:
[Testing scenarioer](kartverketprosjekt/docs/)

---
## **Forhåndsvisning av prosjektet**

### **1. Video Demo**
https://github.com/user-attachments/assets/0218f524-23d1-44e4-9b53-32aff07958f9

### **2. Index-side**  
![image](https://github.com/user-attachments/assets/c4890ea1-9d3a-492d-9b81-b1982cf50445)

### **3. Innmeldingsside**  
![Innmeldingsside preview](https://github.com/user-attachments/assets/8e177b23-2729-4c4f-baca-30d7b2c3bee4)

### **4. Saksbehandler Dashboard**  
![Dashboard preview](https://github.com/user-attachments/assets/8ffa36b5-b8a8-493d-91fc-c891d851a5ab)

### **5. Discord/Slack Bot**  
![Bot preview](https://github.com/user-attachments/assets/e1738455-0a17-4ef2-bbef-2113d2fc8618)

[^1]: Leaflet javascript bilbiotek: [Leaflet](https://leafletjs.com/)
[^2]: WMS-kartoverleggs-tjeneste (vegnett) [geonorge](https://www.geonorge.no/)
[^3]: WMS-kartoverleggs-tjeneste (matrikkel) [geonorge](https://kartkatalog.geonorge.no/metadata/matrikkelkart-wms/30dda4c6-2cba-4378-b2e7-26f644df9d99)
[^4]: WMS-kartlag-tjeneste [Kartverket](https://kartkatalog.geonorge.no/metadata/vegnett2-wms/302fcb0e-a7dc-44f4-a336-8c9ee9709d73?search=vegnett)
