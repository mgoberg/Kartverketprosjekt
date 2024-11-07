//SETUP

// Initial map position and zoom-level
var map = L.map('map').setView([65, 12], 4);

// Initial tile layer
L.tileLayer('https://cache.kartverket.no/v1/wmts/1.0.0/topo/default/webmercator/{z}/{y}/{x}.png', { attribution: '&copy; <a href="http://www.kartverket.no/">Kartverket</a>' }).addTo(map);

//adds scale to map
L.control.scale().addTo(map);

const layerErrorTypes = {
    'land': ['Annet', 'Veg', 'Skogbilveg', 'Privatveg', 'Kommunalveg', 'Fylkesveg', 'Europaveg', 'Riksveg', 'Bom', 'Betgongkjegle', 'Bilsperre', 'Låst bom', 'New jersey', 'Rørgelender', 'Steinblokk', 'Bom med automatisk åpner', 'Trafikkavviser', 'Adressefeil'],
    'grey': ['Annet', 'Veg', 'Skogbilveg', 'Privatveg', 'Kommunalveg', 'Fylkesveg', 'Europaveg', 'Riksveg', 'Bom', 'Betgongkjegle', 'Bilsperre', 'Låst bom', 'New jersey', 'Rørgelender', 'Steinblokk', 'Bom med automatisk åpner', 'Trafikkavviser', 'Adressefeil'],
    'raster': ['Annet', 'Fotrute', 'Skiløype', 'Sykkelrute', 'Annen rute', 'Hytte', 'Skilt', 'Parkering'],
    'sea': ['Annet', 'Grunnstøtning', 'Grunne, skjær eller holme', 'Bro, kai eller konstruksjon i sjø', 'Fylling eller molo', 'Havbruk eller oppdrettsanlegg', 'Rørledning, luftspenn eller undervannskabel', 'Lykt eller merke']
};

//var popup = L.popup();

//FUNCTIONS

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

// Function to change the tile layer and update button styles
function changeTileLayer(layerUrl, attribution, buttonId, layerName) {
    // Remove all existing layers
    map.eachLayer(function (layer) {
        if (!(layer instanceof L.FeatureGroup)) {
            map.removeLayer(layer);
        }
    });


    // Add the new layer
    L.tileLayer(layerUrl, { attribution }, { maxZoom: 18 }).addTo(map);

    // Update button styles when selected
    var buttons = document.querySelectorAll('button');
    buttons.forEach(function (button) {
        button.classList.remove('selected');
    });
    document.getElementById(buttonId).classList.add('selected');

    updateErrorTypeDropdown(layerName);
}


// functions called by pressing the map-layer buttons
function changeToLand() {
    changeTileLayer('https://cache.kartverket.no/v1/wmts/1.0.0/topo/default/webmercator/{z}/{y}/{x}.png', '&copy; <a href="http://www.kartverket.no/">Kartverket</a>', 'btn-changeToLand', 'land');
}

function changeToGrey() {
    changeTileLayer('https://cache.kartverket.no/v1/wmts/1.0.0/topograatone/default/webmercator/{z}/{y}/{x}.png', '&copy; <a href="http://www.kartverket.no/">Kartverket</a>', 'btn-changeToGrey', 'grey');
}

function changeToRaster() {
    changeTileLayer('https://cache.kartverket.no/v1/wmts/1.0.0/toporaster/default/webmercator/{z}/{y}/{x}.png', '&copy; <a href="http://www.kartverket.no/">Kartverket</a>', 'btn-changeToRaster', 'raster');
}
function changeToSea() {
    changeTileLayer('https://cache.kartverket.no/v1/wmts/1.0.0/sjokartraster/default/webmercator/{z}/{y}/{x}.png', '&copy; <a href="http://www.kartverket.no/">Kartverket</a>', 'btn-changeToSea', 'sea');
}

document.getElementById('toggleButton').addEventListener('click', function () {
    var buttonContainer = document.getElementById('buttonContainer');
    if (buttonContainer.style.display === 'none' || buttonContainer.style.display === '') {
        buttonContainer.style.display = 'flex';
    } else {
        buttonContainer.style.display = 'none';
    }
});

document.getElementById('geolocateButton').addEventListener('click', function () {

    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(function (position) {
            var lat = position.coords.latitude;
            var lng = position.coords.longitude;
            map.flyTo([lat, lng], 13); // Center the map on the user's location
        }, function (error) {
            console.error("Geolocation error: " + error.message);
        });
    } else {
        alert("Geolocation is not supported by this browser.");
    }
});
// Fullscreen support for popular browsers, trigger with fullscreen button
document.getElementById('fullscreenButton').addEventListener('click', function () {
    var mapElement = document.getElementById('map');
    if (!document.fullscreenElement) {
        if (mapElement.requestFullscreen) {
            mapElement.requestFullscreen();
        } else if (mapElement.mozRequestFullScreen) { // Firefox
            mapElement.mozRequestFullScreen();
        } else if (mapElement.webkitRequestFullscreen) { // Chrome, Safari and Opera
            mapElement.webkitRequestFullscreen();
        } else if (mapElement.msRequestFullscreen) { // IE/Edge
            mapElement.msRequestFullscreen();
        }
    } else {
        if (document.exitFullscreen) {
            document.exitFullscreen();
        } else if (document.mozCancelFullScreen) { // Firefox
            document.mozCancelFullScreen();
        } else if (document.webkitExitFullscreen) { // Chrome, Safari and Opera
            document.webkitExitFullscreen();
        } else if (document.msExitFullscreen) { // IE/Edge
            document.msExitFullscreen();
        }
    }
});

// Ensure elements with higher z-index receive pointer events and those with lower isn't also interacted with
document.querySelectorAll('#verticalButtonContainer button, #buttonContainer button').forEach(function (button) {
    button.addEventListener('click', function (event) {
        event.stopPropagation(); // Prevent the map from receiving the click event
    });
});




