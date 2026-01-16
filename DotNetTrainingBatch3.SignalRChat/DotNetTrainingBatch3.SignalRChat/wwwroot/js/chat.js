"use strict";

// Program.cs မှာ map လုပ်ထားတဲ့ hub URL နဲ့ တူရပါမယ်
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chatHub")
    .build();

// Connection မစတင်ခင် Send button ကိုပိတ်ထားမယ်
document.getElementById("sendButton").disabled = true;

// ✅ Server က broadcast လုပ်တဲ့ message ကို လက်ခံဖို့ event register
connection.on("ClientReceiveMessage", (user, message) => {
    const li = document.createElement("li");
    li.textContent = `${user} says: ${message}`;
    document.getElementById("messagesList").appendChild(li);
});

// Hub connection စတင်
connection.start()
    .then(() => {
        document.getElementById("sendButton").disabled = false;
    })
    .catch(err => console.error(err.toString()));

// Send button click → Server method ကိုခေါ်
document.getElementById("sendButton")
    .addEventListener("click", event => {

        const user = document.getElementById("userInput").value;
        const message = document.getElementById("messageInput").value;

        connection.invoke("ServerReceiveMessage", user, message)
            .catch(err => console.error(err.toString()));

        event.preventDefault();
    });
