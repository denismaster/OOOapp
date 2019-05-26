"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

//Disable send button until connection is established
document.getElementById("sendButton").disabled = true;

connection.on("ReceiveMessage", function (user, message) {
    var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    var encodedMsg = user + ": " + msg;
    var dt = document.createElement("dt");
    dt.textContent = user;
    var dd = document.createElement("dd");
    dd.textContent = msg;
    document.getElementById("messagesList").appendChild(dt);
    document.getElementById("messagesList").appendChild(dd);
});

connection.start().then(function () {
    var group = document.getElementById("groupInput").value;
    connection.invoke("AddToGroup", group).catch(function (err) {
        return console.error(err.toString());
    });
    document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    var group = document.getElementById("groupInput").value;
    var message = document.getElementById("messageInput").value;
    connection.invoke("SendMessage", group, message).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});
