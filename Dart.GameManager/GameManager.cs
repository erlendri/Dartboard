using System;
using System.CodeDom;
using System.Text;
using System.Threading.Tasks;


namespace Dart.GameManager
{
    public class GameManager : IGameManager
    {
        
        public IDartboardListener MyDartboardListener { get; set; }
        public IBeerPublisher MyBeerPublisher { get; set; }
        public DartGame CurrentGame { get; set; }

        public int BeerScoreLimit { get; set; } = 50;

        public event ScoreUpdated GameScoreUpdated;
        
        public delegate void ScoreUpdated(object sender, OnScoreUpdatedArgs args);

        public GameManager(IDartboardListener myDartboardListener, IBeerPublisher myBeerPublisher)
        {
            MyDartboardListener = myDartboardListener;
            MyBeerPublisher = myBeerPublisher;
            StartNewGame();
            MyDartboardListener.ThrowReceivedEvent += MyDartboardListener_ThrowReceivedEvent;
        }

        public void StartNewGame()
        {
            CurrentGame = new DartGame();
        }

        public void RemoveLastThrow()
        {
            CurrentGame.RemoveLastThrow();
        }

        public void AddThrowManually(int points)
        {
            CurrentGame.AddThrow(points);
        }

        private void MyDartboardListener_ThrowReceivedEvent(object sender, OnThrowReceivedArgs args)
        {
            CurrentGame.AddThrow(args.Points);

            if (CurrentGame.ThrowsCounter <= 3)
                GameScoreUpdated?.Invoke(sender, new OnScoreUpdatedArgs(CurrentGame));
           
            if (CurrentGame.IsComplete && CurrentGame.TotalScore >= BeerScoreLimit)
            {
                Console.WriteLine("Beer poured!");
                MyBeerPublisher.PourBeer();
                CurrentGame.Reset();
                Console.WriteLine("Game complete.");
            }
            else if(CurrentGame.IsComplete)
            {
                CurrentGame.Reset();
                Console.WriteLine("Game complete.");
            }
           
           
        }

       
    }
}
