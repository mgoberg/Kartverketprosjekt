﻿// Initial map position and zoom-level
var map = L.map('map').setView([65, 12], 4);

// Henter layer URL fra modellen
var layerUrl = '@Model.layerurl';

// Fjerner eksisterende lag
map.eachLayer(function (layer) {
    if (!(layer instanceof L.FeatureGroup)) {
        map.removeLayer(layer);
    }
});

// Legger til bakgrunnskart
L.tileLayer(layerUrl, { attribution: '&copy; <a href="https://www.kartverket.no/">Kartverket</a> ' }).addTo(map);

// Henter GeoJSON-dataen fra modellen
var geoJsonData = JSON.parse('@Html.Raw(Model.geojson_data)');

// Legger GeoJSON-dataen til kartet
var layer = L.geoJSON(geoJsonData).bindPopup('@Model.beskrivelse');
layer.addTo(map);

var coordinates = geoJsonData.geometry.coordinates;

if (geoJsonData.geometry.type === 'LineString') {
    var lineCenter = calculateLineCenter(coordinates);
    map.setView([lineCenter[1], lineCenter[0]], 15); // Fly til linjens sentrum
} else if (geoJsonData.geometry.type === 'Polygon') {
    var polygonCenter = calculatePolygonCenter(coordinates[0]);
    map.setView([polygonCenter[1], polygonCenter[0]], 15); // Fly til polygonets sentrum
} else {
    map.setView([coordinates[1], coordinates[0]], 15); // Fly til punktet
}

// Funksjon for å beregne sentrum av en linje
function calculateLineCenter(coordinates) {
    var totalPoints = coordinates.length;
    var sumLat = 0;
    var sumLng = 0;

    for (var i = 0; i < totalPoints; i++) {
        sumLat += coordinates[i][1];
        sumLng += coordinates[i][0];
    }

    var centerLat = sumLat / totalPoints;
    var centerLng = sumLng / totalPoints;

    return [centerLng, centerLat];
}

// Funksjon for å beregne sentrum av et polygon
function calculatePolygonCenter(coordinates) {
    var totalPoints = coordinates.length;
    var sumLat = 0;
    var sumLng = 0;

    for (var i = 0; i < totalPoints; i++) {
        sumLat += coordinates[i][1];
        sumLng += coordinates[i][0];
    }

    var centerLat = sumLat / totalPoints;
    var centerLng = sumLng / totalPoints;

    return [centerLng, centerLat];
}