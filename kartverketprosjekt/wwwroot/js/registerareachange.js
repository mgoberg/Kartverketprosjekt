// steg for progressbar
const totalSteps = 2;
let currentStep = 0;

// array med de ulike stegenes tekst
const stepTitles = [
    "Bruk tegneverktøyet i kartet for å markere feil",
    "Fyll ut skjemaet for å fullføre",
    "Send inn"
];

// Simulerer at brukeren trykker på knappen for å endre til land for å ha det som initielle kartlag. Hvis ikke blir ikke kart URL'en sendt med form dataen
document.addEventListener('DOMContentLoaded', function () {
    changeToLand();
});




var searchControl = new L.esri.Controls.Geosearch().addTo(map); // Add the geocoding searchbar
document.querySelector('.geocoder-control').style.zIndex = '2000';

var drawnItems = new L.FeatureGroup();
map.addLayer(drawnItems);

var drawControl = new L.Control.Draw({
    draw: {
        polygon: true,
        polyline: true,
        rectangle: true,
        circle: false,
        marker: true,
        circlemarker: false
    },
    edit: {
        featureGroup: drawnItems
    }
});
map.addControl(drawControl);

map.on(L.Draw.Event.CREATED, function (e) {
    var type = e.layerType,
        layer = e.layer;

    // Fjern alle tidligere tegnede elementer
    drawnItems.clearLayers();

    // Legg til det nye elementet
    drawnItems.addLayer(layer);

    if (currentStep == 0) {
        goToNextStep();

    }

    var geoJsonData = layer.toGeoJSON();
    var geoJsonString = JSON.stringify(geoJsonData);

    document.getElementById('geoJsonInput').value = geoJsonString;

    // Hent koordinater basert på type
    if (type === 'marker') {
        var coordinates = layer.getLatLng();
        document.getElementById('Nord').value = coordinates.lat;
        document.getElementById('Ost').value = coordinates.lng;
    } else if (type === 'polyline') {
        var coordinatesArray = layer.getLatLngs();
        var firstPoint = Array.isArray(coordinatesArray[0]) ? coordinatesArray[0][0] : coordinatesArray[0];
        document.getElementById('Nord').value = firstPoint.lat;
        document.getElementById('Ost').value = firstPoint.lng;
    } else if (type === 'polygon') {
        var coordinatesArray = layer.getLatLngs();
        var firstPoint = coordinatesArray[0][0];
        document.getElementById('Nord').value = firstPoint.lat;
        document.getElementById('Ost').value = firstPoint.lng;
    } else if (type === 'rectangle') {
        // Hent koordinatene fra rektangelet (det første hjørnet)
        var bounds = layer.getBounds();
        var firstPoint = bounds.getNorthWest(); // Hent nordvestlige hjørnet
        document.getElementById('Nord').value = firstPoint.lat;
        document.getElementById('Ost').value = firstPoint.lng;
    }
});


// Håndterer redigering av tegnet element
map.on(L.Draw.Event.EDITED, function (e) {
    var layers = e.layers;
    layers.eachLayer(function (layer) {
        var geoJsonData = layer.toGeoJSON();
        var geoJsonString = JSON.stringify(geoJsonData);
        document.getElementById('geoJsonInput').value = geoJsonString;

        // Hent koordinater basert på type
        if (layer instanceof L.Marker) {
            var coordinates = layer.getLatLng();
            document.getElementById('Nord').value = coordinates.lat;
            document.getElementById('Ost').value = coordinates.lng;
        } else if (layer instanceof L.Polyline) {
            var coordinatesArray = layer.getLatLngs();

            // Sjekk om det er flere segmenter (array av arrays)
            if (Array.isArray(coordinatesArray[0])) {
                var firstPoint = coordinatesArray[0][0];
            } else {
                var firstPoint = coordinatesArray[0];
            }

            document.getElementById('Nord').value = firstPoint.lat;
            document.getElementById('Ost').value = firstPoint.lng;
        } else if (layer instanceof L.Polygon) {
            // Hent koordinatene fra polygonen
            var coordinatesArray = layer.getLatLngs();

            // Hvis det er en polygon med flere ringer (ytre og indre ringer), henter vi første punkt fra første ytre ring
            var firstPoint = coordinatesArray[0][0];

            document.getElementById('Nord').value = firstPoint.lat;
            document.getElementById('Ost').value = firstPoint.lng;
        }
    });
});



// Håndterer sletting av tegnet element
map.on(L.Draw.Event.DELETED, function (e) {
    currentStep = 0;
    updateStepTitle();
    updateProgressBar();
    var layers = e.layers;
    layers.eachLayer(function (layer) {
        // Tøm skjemaet når et element blir slettet
        document.getElementById('geoJsonInput').value = '';
        document.getElementById('Nord').value = '';
        document.getElementById('Ost').value = '';

    });
});


// Henter bekreftelses modal og knapper
var modal = document.getElementById("confirmationModal");
var confirmBtn = document.getElementById("confirmSubmit");
var areaChangeForm = document.getElementById("areaChangeForm");
var sendBtn = document.getElementById("openConfirmationModal");
var closeBtn = document.querySelector(".close");

// Forhindre standard innsending av skjemaet og åpne modalen
areaChangeForm.addEventListener('submit', function (event) {
    event.preventDefault(); // Stopper skjemaet fra å bli sendt automatisk

    // Sjekker om det er noen lag i drawnItems før innsending
    if (drawnItems.getLayers().length === 0) {
        alert("Vennligst marker noe på kartet før du sender inn skjemaet."); // Viser en advarsel
        return; // Avbryt innsending
    }

    modal.style.display = "block"; // Viser modalen
});

// Funksjon for å sende skjemaet
confirmBtn.addEventListener("click", function () {
    // Fanger opp URL-en til nåværende TileLayer

    var tileLayerUrl = map._layers[Object.keys(map._layers)[1]]._url;

    // Lager en skjult input for å lagre TileLayer URL-en
    var mapStateInput = document.createElement('input');
    mapStateInput.type = 'hidden';
    mapStateInput.name = 'layerurl';
    mapStateInput.value = tileLayerUrl;

    // Legger til det skjulte inputet i skjemaet
    areaChangeForm.appendChild(mapStateInput);

    // legger inn loader ikon
    confirmBtn.innerHTML = "<i class='bx bx-loader bx-spin' id='loadIcon'></i>";

    modal.style.display = "none"; // Skjul modalen
    areaChangeForm.submit(); // Send inn skjemaet manuelt

    
});

// Funksjon for å lukke modalen
closeBtn.addEventListener("click", function () {
    modal.style.display = "none"; // Skjul modalen
});



function updateProgressBar() {
    let progress = (currentStep / totalSteps) * 100;
    document.getElementById("progressBar").style.width = progress + "%";
    updateStepTitle();
}

// Call updateProgressBar() when the user moves to the next step.
function goToNextStep() {
    if (currentStep < totalSteps) {
        currentStep++;
        updateProgressBar();
    }
}
function goToPreviousStep() {
    if (currentStep > 0) {
        currentStep--;
        updateProgressBar();
    }
}

function updateStepTitle() {
    const stepTitleElement = document.getElementById("stepTitle");
    stepTitleElement.textContent = stepTitles[currentStep];
}

document.addEventListener("DOMContentLoaded", function () {
    // Select the form and required fields
    const form = document.getElementById("areaChangeForm");
    const requiredFields = form.querySelectorAll("[required]");
    let debounceTimeout;

    // Check if all required fields are filled
    function areAllFieldsFilled() {
        return Array.from(requiredFields).every(field => field.value.trim() !== "");
    }

    // Function to check fields and navigate steps
    function checkRequiredFields() {
        if (currentStep === 1) { // Ensure we're on step 2
            if (areAllFieldsFilled()) {
                goToNextStep(); // Advance to the next step if all fields are filled
            }
        } else if (currentStep > 1 && !areAllFieldsFilled()) {
            goToPreviousStep(); // Go back to the previous step if fields are erased
        }
    }

    // Monitor each required field for changes
    requiredFields.forEach(field => {
        field.addEventListener("input", function () {
            // Clear the previous timeout and set a new one
            clearTimeout(debounceTimeout);
            debounceTimeout = setTimeout(checkRequiredFields, 300); // Adjust delay as needed
        });
    });
});


document.addEventListener("DOMContentLoaded", () => {
    updateStepTitle();
});

function validateFile() {
    const fileInput = document.getElementById('vedlegg');
    const filePath = fileInput.value;
    const allowedExtensions = /(\.jpg|\.jpeg|\.png|\.gif)$/i;

    if (!allowedExtensions.exec(filePath)) {
        alert('Ugyldig filtype. Vennligst last opp et bilde (JPG, JPEG, PNG, GIF).');
        fileInput.value = ''; // Clear the input
        return false;
    }
}