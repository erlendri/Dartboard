namespace Dart.GameManager
{
    public interface IGameManager
    {
        event GameManager.ScoreUpdated GameScoreUpdated;
        void AddThrowManually(int points);
        void RemoveLastThrow();
        void StartNewGame();
    }
}