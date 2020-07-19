"use strict";

var markerArr = [];
var realtive_to;
var date;
var _map;
let flightPath;
var icon;
let sync;

function callRequest() {
    markerArr = [];
    //the date from the user
    realtive_to = document.getElementById("relative_to").value;
    let relative = realtive_to;
    sync = relative.includes("sync_all");
    //if include external server
    if (sync) {
        relative = relative.split("&sync_all");
        relative = relative[0];
    }
    date = new Date(relative);

    let options = {
        zoom: 4,
        center: { lat: 33.771959, lng: 35.217018 }
    }
    _map = new google.maps.Map(document.getElementById('map'), options);
    //run all 1 second
    setInterval(function () {
        getListOfFlights(sync, _map)
    }, 1000)
}
//show the flight on the screem
async function getListOfFlights(sync, map) {
    let request = new XMLHttpRequest();
    try {
        var n = date.toISOString();
        realtive_to = n.split('.')[0] + "Z";
    } catch {
        document.getElementById("error_relativeToNotCorrect").style.display = 'block';
    }

    date.setSeconds(date.getSeconds() + 1);

    let url = await null;
    if (sync) {
        url = "/api/Flights?relative_to=" + realtive_to + "&sync_all";
    }
    else {
        url = "/api/Flights?relative_to=" + realtive_to;
    }
    request.onreadystatechange = async function () {
        try {
            if (this.readyState == 4 && this.status == 200) {
                //my arr = list of flights
                let myArr_ = JSON.parse(this.responseText);
                //error from server
                if (myArr_.errorId == 300) {
                    document.getElementById("error_DbEmpty").style.display = 'block';
                }
                let myArr = myArr_.data;

                markerArr = updateIcon(myArr);

                //to update the list of flights
                if (myArr.length != markerArr.length) {
                    addFligthList();
                }

                //all the info from data base
                myArr.forEach((row) => {
                    addMarker({ coords: { lat: row.latitude, lng: row.longitude }, id: row.flight_id });
                });

                function addMarker(props) {
                    var k, enter = false;

                    //check if need to update the marker or change lon and lat
                    for (k = 0; k < markerArr.length; k++) {
                        //the marker ecsist
                        if ((markerArr[k].id) == (props.id)) {
                            var latlng = new google.maps.LatLng(props.coords.lat, props.coords.lng);
                            markerArr[k].setPosition(latlng);
                            enter = true;
                        }
                    }
                    //need to make new marke
                    if (!enter) {

                        icon = {
                            url: 'http://maps.google.com/mapfiles/kml/shapes/airports.png',
                            //http://kml4earth.appspot.com/icons.html
                            scaledSize: new google.maps.Size(30, 30),
                        };

                        var marker = new google.maps.Marker({
                            id: props.id,
                            position: props.coords,
                            map: map,
                            icon: icon,
                        });

                        marker.addListener('click', async function () {
                            clickOnIconOrID({ id: props.id, marker: marker });
                        });
                        markerArr.push(marker);
                    }
                }
            }
        }
        catch (e) {

        }
    }
    request.open("get", url, true);
    request.send();
}

//statr whith Boid on list of flights and return normal and icons
function restartBold() {
    //resturt icons
    for (var j = 0; j < markerArr.length; j++) {
        markerArr[j].setIcon(icon);
    }
    //restars list
    var listFlight = document.getElementsByTagName("tr")
    for (var i = 0; i < listFlight.length; i++) {
        listFlight[i].style.fontWeight = "normal";
    }
}

//to bold the icon and list
async function clickOnIconOrID(props) {
    await restartBold();
    showFlightDittels(props.id);
    boldID(props.id);
    getFlightPlan(props.id, _map);
    //if its a marker
    if (props.marker) {
        props.marker.setIcon('http://maps.google.com/mapfiles/kml/pal2/icon48.png');
    }
    //else its Id from list
    else {
        for (var k = 0; k < markerArr.length; k++) {
            if ((markerArr[k].id) == (props.id)) {
                markerArr[k].setIcon('http://maps.google.com/mapfiles/kml/pal2/icon48.png');
            }
        }
    }
}

//check if need to move the marker on the map or add a new one
//ckeck ehith the new list = my arr , and the last list = marker arr
function updateIcon(myArr) {
    let ditels = false;
    let newMarkerArr = [];
    let s = 0;
    for (var t = 0; t < markerArr.length; t++) {
        ditels = false;
        for (var k = 0; k < myArr.length; k++) {
            if (markerArr[t].id == myArr[k].flight_id) {
                ditels = true;
                newMarkerArr[s] = markerArr[t];
                s++;
                break;
            }
        }
        if (ditels == false) {
            if (flightPath) {
                flightPath.setMap(null);
            }
            markerArr[t].setMap(null)
            ditels = false;
            document.getElementById(markerArr[t].id).remove();
        }
    }
    return newMarkerArr;
}
