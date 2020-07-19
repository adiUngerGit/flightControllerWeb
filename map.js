"use strict";
function load() {

    let options = {
        zoom: 8,
        center: { lat: 31.771959, lng: 35.217018 }
    }
    var map = new google.maps.Map(document.getElementById('map'), options);
}