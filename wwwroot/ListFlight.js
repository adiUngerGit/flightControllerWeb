"use strict";

async function addFligthList() {
    let request = new XMLHttpRequest();
    let url = await null;
    if (sync) {
        url = "/api/Flights?relative_to=" + realtive_to + "&sync_all";
    }
    else {
        url = "/api/Flights?relative_to=" + realtive_to;
    }
    request.onreadystatechange = function () {
        try {
            //get info from server
            if (this.readyState == 4 && this.status == 200) {
                let myArr = JSON.parse(this.responseText).data;
                addTabel(myArr);
            }
        }
        catch (e) {
            console.log("eror: " + e);
        }
    }
    request.open("get", url, true);
    request.send();
}

async function deleteFlight(id) {
    id = new String(id);
    let request = new XMLHttpRequest();
    //call server
    let url = "api/Flights/" + id;
    for (var i = 0; i < markerArr.length; i++) {
        if (markerArr[i].id == id) {
            markerArr[i].setMap(null);
        }
    }
    await (request.open("Delete", url, true));
    await (request.send());
    if (markerArr.length == 1) {
        addFligthList();
    }
}

function showFlightDittels(_id) {
    let request = new XMLHttpRequest();
    let url = null;
    if (sync) {
        url = "/api/Flights?relative_to=" + realtive_to + "&sync_all";
    }
    else {
        url = "/api/Flights?relative_to=" + realtive_to;
    }
    request.onreadystatechange = function () {
        try {
            //if ststs ok
            if (this.readyState == 4 && this.status == 200) {
                let myArr = JSON.parse(this.responseText).data;
                if (myArr) {
                    for (var index = 0; index < myArr.length; index++) {
                        if (myArr[index].flight_id == (_id)) {
                            addTheDIttelsToTabel(myArr[index]);
                        }
                    }
                }
            }
        }
        catch (e) {
            console.log("eror " + e);
        }
    }
    request.open("get", url, true);
    request.send();
}

function addTheDIttelsToTabel(flight) {
    //delet all tabel
    var Table = document.getElementById("flightDataTabel");
    Table.innerHTML = "";

    let tr2 = document.createElement("tr");
    let tr1 = document.createElement("tr");

    let id = document.createElement("th");
    let company = document.createElement("th");
    let isExternal = document.createElement("th");
    let longitude = document.createElement("th");
    let latitude = document.createElement("th");
    let numberOfPassenger = document.createElement("th");
    let dateAndTime = document.createElement("th");

    let s1 = document.createElement("td")
    let s2 = document.createElement("td")
    let s3 = document.createElement("td")
    let s4 = document.createElement("td")
    let s5 = document.createElement("td")
    let s6 = document.createElement("td")

    let id2 = document.createElement("td");
    let company2 = document.createElement("td");
    let isExternal2 = document.createElement("td");
    let longitude2 = document.createElement("td");
    let latitude2 = document.createElement("td");
    let numberOfPassenger2 = document.createElement("td");
    let dateAndTime2 = document.createElement("td");


    id.textContent = "id: ";
    s1.textContent = "  ";
    company.textContent = "companyName:    ";
    s2.textContent = "  ";
    isExternal.textContent = "isExternal:    ";
    s3.textContent = "  ";
    longitude.textContent = "longitude:    ";
    s4.textContent = "  ";
    latitude.textContent = "latitude:   ";
    s5.textContent = "  ";
    numberOfPassenger.textContent = "numberOfPassenger:   ";
    s6.textContent = "  ";
    dateAndTime.textContent = "dateAndTime:   ";

    id2.textContent = flight.flight_id;
    company2.textContent = flight.company_name;
    isExternal2.textContent = flight.is_external;
    longitude2.textContent = Math.round(flight.longitude * 100) / 100;
    latitude2.textContent = Math.round(flight.latitude * 100) / 100;
    numberOfPassenger2.textContent = flight.passengers;
    dateAndTime2.textContent = flight.date_time;

    tr1.appendChild(id);
    tr1.appendChild(company);
    tr1.appendChild(isExternal);
    tr1.appendChild(longitude);
    tr1.appendChild(latitude);
    tr1.appendChild(numberOfPassenger);
    tr1.appendChild(dateAndTime);

    tr2.appendChild(id2);
    tr2.appendChild(company2);
    tr2.appendChild(isExternal2);
    tr2.appendChild(longitude2);
    tr2.appendChild(latitude2);
    tr2.appendChild(numberOfPassenger2);
    tr2.appendChild(dateAndTime2);

    let _element = document.getElementById("flightDataTabel");

    _element.appendChild(tr1);
    _element.appendChild(tr2);
}
function addTabel(json) {

    var tabelIsExternal = document.getElementById("myFlightList");
    tabelIsExternal.innerHTML = "";
    var tabelNotExternal = document.getElementById("myFlightListSyncAll");
    tabelNotExternal.innerHTML = "";

    let thead = document.createElement("thead");
    let trH = document.createElement("tr")
    let th1 = document.createElement("th")
    th1.textContent = "flight ID";
    let th2 = document.createElement("th")
    th2.textContent = "company name";

    trH.appendChild(th1);
    trH.appendChild(th2);
    thead.appendChild(trH);
    tabelIsExternal.appendChild(thead);

    let thead1 = document.createElement("thead");
    let trH1 = document.createElement("tr")
    let th11 = document.createElement("th")
    th11.textContent = "flight ID";
    let th21 = document.createElement("th")
    th21.textContent = "company name";

    trH1.appendChild(th11);
    trH1.appendChild(th21);
    thead1.appendChild(trH1);
    tabelNotExternal.appendChild(thead1);

    json.forEach((row) => {

        let tr = document.createElement("tr");
        tr.id = row.flight_id;
        let companyName = document.createElement("td");
        let ID = document.createElement("td");
        ID.textContent = row.flight_id;
        ID.onclick = async function () {
            await clickOnIconOrID({ id: row.flight_id });
        };
        companyName.textContent = row.company_name;
        tr.appendChild(ID);
        tr.appendChild(companyName);
        if (!row.is_external) {
            let delet = document.createElement("td");
            let delet1 = document.createElement("img");
            delet1.src = "garbge.png";
            delet1.width = "30";
            delet1.height = "30";
            delet1.id = row.flight_id;
            delet1.onclick = function () {
                deleteFlight(row.flight_id);
            };
            delet.appendChild(delet1);
            tr.appendChild(delet);
        }

        if (!row.is_external) {
            tabelIsExternal.appendChild(tr);
        }
        if (row.is_external) {
            tabelNotExternal.appendChild(tr);
        }
    });

    getListServer();
}

function getListServer() {

    var tabelServer = document.getElementById("serverList");
    tabelServer.innerHTML = "";

    let thead3 = document.createElement("thead");
    let trH3 = document.createElement("tr")
    let th13 = document.createElement("th")
    th13.textContent = "Server ID";
    let th23 = document.createElement("th")
    th23.textContent = "Url";

    trH3.appendChild(th13);
    trH3.appendChild(th23);
    thead3.appendChild(trH3);
    tabelServer.appendChild(thead3);

    let request = new XMLHttpRequest();
    let url = null;
    url = "/api/servers";

    request.onreadystatechange = function () {
        try {
            //if ststs ok
            if (this.readyState == 4 && this.status == 200) {
                let myArr = JSON.parse(this.responseText).data;
                if (myArr) {
                    myArr.forEach((row) => {
                        let tr = document.createElement("tr");
                        let td = document.createElement("td");
                        td.textContent = row.serverID;
                        let td1 = document.createElement("td");
                        td1.textContent = row.serverUrl;
                        tr.appendChild(td);
                        tr.appendChild(td1);
                        tabelServer.appendChild(tr);
                    });
                }
            }
            else {
            }
        }
        catch (e) {
            console.log("eror " + e);
        }
        
    }
    request.open("get", url, true);
    request.send();
}