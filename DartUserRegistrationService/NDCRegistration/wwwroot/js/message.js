"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/messageHub").build();


connection.on("GameStarted", function (gamer) {
  console.log(gamer);
  alert(`Game started for id ${gamer.id}`);
});

connection.start().then(function () {
  console.log('Ready to receive SignalR');
  app.Init();

}).catch(function (err) {
  return console.error(err.toString());
  });

let hub = {
  HelloServer: () => {
    connection.invoke('HelloServer');
  },
  StartGame: (id) => {
    connection.invoke('StartGame', id);
  }
}