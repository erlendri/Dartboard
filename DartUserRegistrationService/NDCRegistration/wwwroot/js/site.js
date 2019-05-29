let app = {
  startClass: 'player-button-start',
  deleteClass: 'player-button-delete',
  playerClass: 'player',
  nextAreaId: 'player-next',
  templatePlayerId: 'player-template',
  Init: () => {
    let nextArea = document.getElementById(app.nextAreaId);
    nextArea.addEventListener('click', v => {
      let target = v.target;
      if (target.classList.contains(app.startClass) || target.closest(`.${app.startClass}`)) {
        var gameId = target.closest(`.${app.playerClass}`).id;
        hub.StartGame(gameId);
      }
    });
  },
  AddPlayerNext: (game) => {

  },

  Test: () => {
    alert('hello world');
  }
}