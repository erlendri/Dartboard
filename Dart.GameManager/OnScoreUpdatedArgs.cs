namespace Dart.GameManager
{
    public class OnScoreUpdatedArgs
    {
        public OnScoreUpdatedArgs(IDartGame currentGame)
        {
            CurrentGame = currentGame;
        }

        public IDartGame CurrentGame { get; set; }
    }
}