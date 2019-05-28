let app = {
  startClass: 'player-button-start',
  deleteClass: 'player-button-delete',
  playerClass: 'player',
  nextAreaId: 'player-next',
  Init: () => {
    let nextArea = document.getElementById(app.nextAreaId);
    nextArea.addEventListener('click', v => {
      let target = v.target;
      if (target.classList.contains(app.startClass) || target.closest(`.${app.startClass}`)) {
        var playerId = target.closest(`.${app.playerClass}`).id;
        hub.StartGame(playerId);
      }
    });
  },
  Test: () => {
    alert('hello world');
  }
}