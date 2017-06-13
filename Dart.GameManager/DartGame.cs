using System;
using System.Collections.Generic;
using System.Linq;

namespace Dart.GameManager
{
    public class DartGame : IDartGame
    {
        private List<int> dartThrows = new List<int>();

        public int ThrowsCounter { get; set; } = 0;


        public int GetDartThrow(int throwNumber)
        {
            return dartThrows.ElementAtOrDefault(throwNumber - 1);
        }

        public void AddThrow(int points)
        {
            ThrowsCounter++;
            Console.WriteLine(ThrowsCounter);

            dartThrows.Add(points);
        }

        public bool IsComplete => ThrowsCounter >= 3;
        public int FirstThrow => GetDartThrow(1);
        public int SecondThrow => GetDartThrow(2);
        public int ThirdThrow => GetDartThrow(3);

        public int TotalScore => FirstThrow + SecondThrow + ThirdThrow;

        public void RemoveLastThrow()
        {
            if(dartThrows.Count > 0)
                dartThrows.Remove(dartThrows.Last());
        }

        public void Reset()
        {
            dartThrows.Clear();
            ThrowsCounter = 0;
        }

    }
}