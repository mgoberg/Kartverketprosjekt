document.addEventListener('DOMContentLoaded', function () {
    // Initial kartposisjon og zoom-nivå
    var map = L.map('map').setView([65, 12], 4);

    // Legger til bakgrunnskart ved å bruke layerUrl
    L.tileLayer(layerUrl, { attribution: '&copy; <a href="https://www.kartverket.no/">Kartverket</a>' }).addTo(map);

    // Legger GeoJSON-dataen til kartet
    var layer = L.geoJSON(geoJsonData).bindPopup(description);
    layer.addTo(map);

    // Finner senteret av GeoJSON-dataen ved å bruke Leaflet's innebygde funksjoner
    var bounds = layer.getBounds();

    // Flytter kartet til å passe til GeoJSON-dataens størrelse
    map.fitBounds(bounds);
});
