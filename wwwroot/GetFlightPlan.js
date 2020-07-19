"use strict";

function allHelp() {
    document.getElementById("help").style.display = "block"
}
 
let PathID = null;
 async function getFlightPlan(id, map) {

    let request = new XMLHttpRequest();
    let url = "/api/FlightPlan/" + id;
     request.onreadystatechange = async function () {
        try {
            if (this.readyState == 4 && this.status == 200) {
                let myArr = JSON.parse(this.responseText);
                var segments = myArr.segments;
                //flightPath.setMap(null);
                let flightPlanCoordinates = new Array();
                var initial = myArr.initial_location;
                let lng = parseFloat(initial.longitude);
                let lat = parseFloat(initial.latitude);
                PathID = id;
                flightPlanCoordinates.push({ lat, lng });
                segments.forEach((row) => {
                     lat = row.latitude;
                     lng = row.longitude;
                    flightPlanCoordinates.push({ lat, lng });
                });
                if (flightPath) {
                    flightPath.setPath(flightPlanCoordinates);
                } else {
                    flightPath = await new google.maps.Polyline({
                        path: flightPlanCoordinates,
                        geodesic: true,
                        strokeColor: '#FF0000',
                        strokeOpacity: 1.0,
                        strokeWeight: 2
                    });
                    
                }
                flightPath.setMap(map);
           
            }
         
        }
        catch {

        }
    }
    await request.open("get", url, true);
    request.send();
    
}