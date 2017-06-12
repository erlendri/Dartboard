namespace Dart.GameManager
{
    public interface IDartGame
    {
        int FirstThrow { get; }
        int SecondThrow { get; }
        int ThirdThrow { get; }
        int TotalScore { get; }
        int ThrowsCounter { get; }
        bool IsComplete { get; }
    }
}