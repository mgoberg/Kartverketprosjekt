//SETUP

// Initial map position and zoom-level
var map = L.map('map').setView([65, 12], 4);

// Initial tile layer
L.tileLayer('https://tile.openstreetmap.org/{z}/{x}/{y}.png', { attribution: '&copy; <a href="http://www.openstreetmap.org/">OpenStreetMap</a>' }).addTo(map);

//adds scale to map
L.control.scale().addTo(map);



//var popup = L.popup();




//FUNCTIONS

// Function to change the tile layer and update button styles
function changeTileLayer(layerUrl, attribution, buttonId) {
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
}
// functions called by pressing the map-layer buttons
function changeToLand() {
    changeTileLayer('https://cache.kartverket.no/v1/wmts/1.0.0/topo/default/webmercator/{z}/{y}/{x}.png', '&copy; <a href="http://www.kartverket.no/">Kartverket</a>', 'btn-changeToLand');
}

function changeToAerial() {
    changeTileLayer('https://tiles.stadiamaps.com/tiles/alidade_satellite/{z}/{x}/{y}{r}.png', '&copy; CNES, Distribution Airbus DS, © Airbus DS, © PlanetObserver (Contains Copernicus Data) | &copy; <a href="https://www.stadiamaps.com/" target="_blank">Stadia Maps</a> &copy; <a href="https://openmaptiles.org/" target="_blank">OpenMapTiles</a> &copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> ', 'btn-changeToAerial');
}

function changeToGrey() {
    changeTileLayer('https://cache.kartverket.no/v1/wmts/1.0.0/topograatone/default/webmercator/{z}/{y}/{x}.png', '&copy; <a href="http://www.kartverket.no/">Kartverket</a>', 'btn-changeToGrey');
}

function changeToRaster() {
    changeTileLayer('https://cache.kartverket.no/v1/wmts/1.0.0/toporaster/default/webmercator/{z}/{y}/{x}.png', '&copy; <a href="http://www.kartverket.no/">Kartverket</a>', 'btn-changeToRaster');
}
function changeToSea() {
    changeTileLayer('https://cache.kartverket.no/v1/wmts/1.0.0/sjokartraster/default/webmercator/{z}/{y}/{x}.png', '&copy; <a href="http://www.kartverket.no/">Kartverket</a>', 'btn-changeToSea');
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



/**
 * Event listener for the form submission.
 * Prevents the default form submission behavior, captures the URL of the current TileLayer,
 * creates a hidden input to store the TileLayer URL, appends the hidden input to the form,
 * and submits the form.
 */
document.getElementById('areaChangeForm').addEventListener('submit', function (event) {
    // Prevent the default form submission
    event.preventDefault();

    // Capture the URL of the current TileLayer
    var tileLayerUrl = map._layers[Object.keys(map._layers)[1]]._url;

    // Create a hidden input to store the TileLayer URL
    var mapStateInput = document.createElement('input');
    mapStateInput.type = 'hidden';
    mapStateInput.name = 'mapUrl';
    mapStateInput.value = tileLayerUrl;

    // Append the hidden input to the form
    this.appendChild(mapStateInput);

    // Submit the form
    this.submit();
});
