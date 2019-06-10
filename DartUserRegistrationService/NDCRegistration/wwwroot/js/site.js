let app = {
  startClass: 'player-button-start',
  deleteClass: 'player-button-delete',
  playerClass: 'player',
  playerName: 'player-name',
  playerScore: 'player-score',
  nextAreaId: 'player-next',
  currentAreaId: 'player-current',
  templatePlayerId: 'player-template',
  Init: () => {
    let nextArea = document.getElementById(app.nextAreaId);
    nextArea.addEventListener('click', v => {
      let target = v.target;
      if (target.classList.contains(app.startClass) || target.closest(`.${app.startClass}`)) {
        var gameStartId = target.closest(`.${app.playerClass}`).id;
        hub.StartGame(gameStartId);
      }
      else if (target.classList.contains(app.deleteClass) || target.closest(`.${app.deleteClass}`)) {
        var gameDeleteId = target.closest(`.${app.playerClass}`).id;
        hub.DeleteGame(gameDeleteId);
      }
    });
  },
  GetExistingPlayer: (gameId) => {
    var players = Array.from(document.getElementsByClassName(app.playerClass));
    var match = players.filter(f => f.id === gameId);
    if (match.length > 0)
      return match[0];
    return null;
  },
  AddPlayerNext: (game) => {
    app.RemovePlayer(game.id);
    var tmpl = document.getElementById(app.templatePlayerId);
    var nextArea = document.getElementById(app.nextAreaId);
    nextArea.appendChild(tmpl.content.cloneNode(true));
    var existing = nextArea.lastElementChild;
    app.UpdatePlayerNode(existing, game);
  },
  RemovePlayer: (gameId) => {
    var existing = app.GetExistingPlayer(gameId);
    if (existing !== undefined && existing !== null)
      existing.remove();
  },
  UpdatePlayerCurrent: (game) => {
    app.RemovePlayer(game.id);
    var existing = app.GetExistingPlayer(game.id);
    if (existing === undefined || existing === null) {
      app.ClearPlayerCurrent();
      var tmpl = document.getElementById(app.templatePlayerId);
      var currentArea = document.getElementById(app.currentAreaId);
      currentArea.appendChild(tmpl.content.cloneNode(true));
      existing = currentArea.lastElementChild;
    }
    app.UpdatePlayerNode(existing, game);
  },
  ClearPlayerNext: () => {
    var nextArea = document.getElementById(app.nextAreaId);
    nextArea.innerHTML = '';
  },
  ClearPlayerCurrent: () => {
    var currentArea = document.getElementById(app.currentAreaId);
    currentArea.innerHTML = '';
  },
  UpdatePlayerNode: (node, game) => {
    node.id = game.id;
    node.getElementsByClassName(app.playerScore)[0].innerHTML = game.score;
    if (game.name !== undefined && game.name !== null)
      node.getElementsByClassName(app.playerName)[0].innerHTML = game.name;
  },
  Test: () => {
    alert('hello world');
  }
}