//OPPSETT

// Initial kartposisjon og zoom-nivå
var map = L.map('map').setView([65, 12], 4);

// Initial flislag
L.tileLayer('https://cache.kartverket.no/v1/wmts/1.0.0/topo/default/webmercator/{z}/{y}/{x}.png', { attribution: '&copy; <a href="http://www.kartverket.no/">Kartverket</a>' }).addTo(map);

//legger til skala på kartet
L.control.scale().addTo(map);

const layerErrorTypes = {
    'Topografisk': ['Annet', 'Veg', 'Skogbilveg', 'Privatveg', 'Kommunalveg', 'Fylkesveg', 'Europaveg', 'Riksveg', 'Bom', 'Betgongkjegle', 'Bilsperre', 'Låst bom', 'New jersey', 'Rørgelender', 'Steinblokk', 'Bom med automatisk åpner', 'Trafikkavviser', 'Adressefeil'],
    'Topografisk gråtone': ['Annet', 'Veg', 'Skogbilveg', 'Privatveg', 'Kommunalveg', 'Fylkesveg', 'Europaveg', 'Riksveg', 'Bom', 'Betgongkjegle', 'Bilsperre', 'Låst bom', 'New jersey', 'Rørgelender', 'Steinblokk', 'Bom med automatisk åpner', 'Trafikkavviser', 'Adressefeil'],
    'Turkart': ['Annet', 'Fotrute', 'Skiløype', 'Sykkelrute', 'Annen rute', 'Hytte', 'Skilt', 'Parkering'],
    'Sjøkart': ['Annet', 'Grunnstøtning', 'Grunne, skjær eller holme', 'Bro, kai eller konstruksjon i sjø', 'Fylling eller molo', 'Havbruk eller oppdrettsanlegg', 'Rørledning, luftspenn eller undervannskabel', 'Lykt eller merke']
};


//FUNKSJONER

// Funksjon for å oppdatere feiltype i dropdown
function updateErrorTypeDropdown(layerName) {
    const errorTypeSelect = document.getElementById('typeFeil');

    // Tøm eksisterende alternativer
    errorTypeSelect.innerHTML = '<option value="" disabled selected>...</option>';

    // Hent de nye feiltypene basert på kartlaget
    const errorTypes = layerErrorTypes[layerName] || [];

    // Legg til de nye alternativene i dropdown
    errorTypes.forEach(function (type) {
        const option = document.createElement('option');
        option.value = type;
        option.textContent = type;
        errorTypeSelect.appendChild(option);
    });
}


// global variabel for å holde Matrikkelkart og vegnett WMS lag
var matrikkelkartWMSLayer;
var vegnettWMSLayer;


// Funksjon for å endre kartlag og oppdatere knappestiler
function changeTileLayer(layerUrl, attribution, buttonId, layerName) {
    // Fjern alle eksisterende lag bortsett fra Matrikkelkart WMS-laget (hvis det er aktivt)
    map.eachLayer(function (layer) {
        if (!(layer instanceof L.FeatureGroup) && layer !== matrikkelkartWMSLayer) {
            map.removeLayer(layer);
        }
    });

    // Legg til det nye kartlaget
    L.tileLayer(layerUrl, { attribution: attribution, maxZoom: 18 }).addTo(map);

    // Oppdater knappestiler når valgt
    var buttons = document.querySelectorAll('button');
    buttons.forEach(function (button) {
        button.classList.remove('selected');
    });
    document.getElementById(buttonId).classList.add('selected');

    // Oppdater det viste lagnavnet
    document.getElementById('layerName').textContent = layerName;

    updateErrorTypeDropdown(layerName);
}

// Funksjon for å veksle Matrikkelkart WMS-lagets synlighet
function toggleMatrikkelkartLayer() {
    // Sjekk om laget allerede eksisterer
    if (!matrikkelkartWMSLayer) {
        // Opprett og legg til Matrikkelkart WMS-laget hvis det ikke eksisterer
        matrikkelkartWMSLayer = L.tileLayer.wms("https://wms.geonorge.no/skwms1/wms.matrikkelkart", {
            layers: 'matrikkelkart',
            format: 'image/png',
            transparent: true,
            version: '1.3.0',
            attribution: "&copy; <a href='https://www.kartverket.no/'>Kartverket</a>"
        }).addTo(map);

        matrikkelkartWMSLayer.bringToFront();
    } else {
        // Veksle synligheten til Matrikkelkart WMS-laget
        if (map.hasLayer(matrikkelkartWMSLayer)) {
            map.removeLayer(matrikkelkartWMSLayer); // Fjern laget hvis det allerede er synlig
        } else {
            map.addLayer(matrikkelkartWMSLayer); // Legg til laget hvis det ikke er synlig
        }
    }
    document.getElementById('matrikkelkartToggleButton').classList.toggle('active');
}

// Knapp event-listener for veksling
document.getElementById('matrikkelkartToggleButton').addEventListener('click', toggleMatrikkelkartLayer);

function toggleVegnettLayer() {
    // Sjekk om Vegnett WMS-laget allerede eksisterer
    if (!vegnettWMSLayer) {
        // Opprett og legg til Vegnett WMS-laget hvis det ikke eksisterer
        vegnettWMSLayer = L.tileLayer.wms("https://openwms.statkart.no/skwms1/wms.vegnett2", {
            layers: 'vegnett2', 
            format: 'image/png',
            transparent: true,
            version: '1.3.0',
            attribution: "&copy; <a href='https://www.statkart.no/'>Statkart</a>"
        }).addTo(map);

        vegnettWMSLayer.bringToFront();
    } else {
        // Veksle synligheten til Vegnett WMS-laget
        if (map.hasLayer(vegnettWMSLayer)) {
            map.removeLayer(vegnettWMSLayer); // Fjern laget hvis det allerede er synlig
        } else {
            map.addLayer(vegnettWMSLayer); // Legg til laget hvis det ikke er synlig
            vegnettWMSLayer.bringToFront(); // Sørg for at det er på toppen hvis det legges til igjen
        }
    }
    document.getElementById('vegnettToggleButton').classList.toggle('active');
}

document.getElementById('vegnettToggleButton').addEventListener('click', toggleVegnettLayer);
// funksjoner kalt ved å trykke på kartlagsknappene
function changeToLand() {
    changeTileLayer('https://cache.kartverket.no/v1/wmts/1.0.0/topo/default/webmercator/{z}/{y}/{x}.png', '&copy; <a href="http://www.kartverket.no/">Kartverket</a>', 'btn-changeToLand', 'Topografisk');
}

function changeToGrey() {
    changeTileLayer('https://cache.kartverket.no/v1/wmts/1.0.0/topograatone/default/webmercator/{z}/{y}/{x}.png', '&copy; <a href="http://www.kartverket.no/">Kartverket</a>', 'btn-changeToGrey', 'Topografisk gråtone');
}

function changeToRaster() {
    changeTileLayer('https://cache.kartverket.no/v1/wmts/1.0.0/toporaster/default/webmercator/{z}/{y}/{x}.png', '&copy; <a href="http://www.kartverket.no/">Kartverket</a>', 'btn-changeToRaster', 'Turkart');
}
function changeToSea() {
    changeTileLayer('https://cache.kartverket.no/v1/wmts/1.0.0/sjokartraster/default/webmercator/{z}/{y}/{x}.png', '&copy; <a href="http://www.kartverket.no/">Kartverket</a>', 'btn-changeToSea', 'Sjøkart');
}

document.getElementById('toggleButton').addEventListener('click', function () {
    var buttonContainer = document.getElementById('buttonContainer');
    if (buttonContainer.style.display === 'none' || buttonContainer.style.display === '') {
        buttonContainer.style.display = 'flex';
    } else {
        buttonContainer.style.display = 'none';
    }
    document.getElementById('toggleButton').classList.toggle('active');
});

// geolokator knapp som sentrerer kartet på brukerens posisjon
document.getElementById('geolocateButton').addEventListener('click', function () {

    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(function (position) {
            var lat = position.coords.latitude;
            var lng = position.coords.longitude;
            map.flyTo([lat, lng], 13);
        }, function (error) {
            console.error("Geolokasjonsfeil: " + error.message);
        });
    } else {
        alert("Geolokasjon støttes ikke av denne nettleseren.");
    }
});
// Fullskjermstøtte for populære nettlesere, utløses med fullskjermknapp
document.getElementById('fullscreenButton').addEventListener('click', function () {
    var mapElement = document.getElementById('map');
    if (!document.fullscreenElement) {
        if (mapElement.requestFullscreen) {
            mapElement.requestFullscreen();
        } else if (mapElement.mozRequestFullScreen) { // Firefox
            mapElement.mozRequestFullScreen();
        } else if (mapElement.webkitRequestFullscreen) { // Chrome, Safari og Opera
            mapElement.webkitRequestFullscreen();
        } else if (mapElement.msRequestFullscreen) { // IE/Edge
            mapElement.msRequestFullscreen();
        }
    } else {
        if (document.exitFullscreen) {
            document.exitFullscreen();
        } else if (document.mozCancelFullScreen) { // Firefox
            document.mozCancelFullScreen();
        } else if (document.webkitExitFullscreen) { // Chrome, Safari og Opera
            document.webkitExitFullscreen();
        } else if (document.msExitFullscreen) { // IE/Edge
            document.msExitFullscreen();
        }
    }
});

// Sørg for at elementer med høyere z-indeks mottar pekerehendelser og de med lavere ikke også blir interagert med
document.querySelectorAll('#verticalButtonContainer button, #buttonContainer button').forEach(function (button) {
    button.addEventListener('click', function (event) {
        event.stopPropagation(); // Forhindre at kartet mottar klikkhendelsen
    });
});
