using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ways
{
    class Program
    {
        static void Main(string[] args)
        {
            const string fill = "@";
            const string entity = "  ";
            var rnd = new Random();
            const int length = 40;
            const int wayCount = 10;
            const int wayWidth = 2;
            var colors = new Dictionary<int, ConsoleColor>();

            Way.Build(length);
            var ways = new List<Way>();
            for (int i = 0; i < wayCount; i++)
            {
                ways.Add(new Way(rnd.Next(length - wayWidth), rnd.Next(2) == 0, wayWidth));
                colors.Add(ways.Last().Id, (ConsoleColor)rnd.Next((int)ConsoleColor.DarkBlue, (int)ConsoleColor.White + 1));
            }

            while (true)
            {
                foreach (var keyValuePair in Way.PositionsDictionary)
                {
                    if (keyValuePair.Value != 0)
                    {
                        Console.ForegroundColor = colors[keyValuePair.Value];
                        Console.Write(fill);
                        Console.ResetColor();
                    }
                    Console.Write(entity);
                    //strBild.Append(keyValuePair.Value ? fill : entity);
                }
                foreach (var way in ways)
                {
                    way.Move();
                }
                Console.WriteLine();
                Thread.Sleep(5);
            }
        }
    }

    class Way
    {
        public static Dictionary<int, int> PositionsDictionary;
        public static int PlaceCount { get; set; }
        private static readonly Random Rnd = new Random();
        private static int _ids;
        private static readonly int MaxRouter = 1;
        private static readonly int MaxFronter = 2;

        public static void Build(int placeCount)
        {
            PlaceCount = placeCount;
            _ids = 1;
            PositionsDictionary = new Dictionary<int, int>();
            for (int i = 0; i < PlaceCount; i++)
            {
                PositionsDictionary.Add(i, 0);
            }
        }

        public readonly int Id;

        public int Position
        {
            get { return _position; }
            private set
            {
                if (_rounder == 0)
                {
                    //if (PositionsDictionary[_position] == Id) PositionsDictionary[_position] = 0;
                    //_position = value;
                    //PositionsDictionary[_position] = Id;
                    MoveInDictionary(value);
                    _position = value;
                }
                else _rounder--;
            }
        }

        private bool _arrow;
        private double _probably;
        private int _jorny;
        private int _position;
        private int _rounder;
        private int _fronter;
        private readonly int _width;
        public void MoveInDictionary(int value) //TODO оптимизировать!
        {
            var board = _position + _width;
            for (int i = _position; i < board; i++)
            {
                if (PositionsDictionary[i] == Id) PositionsDictionary[i] = 0;
            }
            board = value + _width;
            for (int i = value; i < board; i++)
            {
                PositionsDictionary[i] = Id;
            }
        }

        public Way(int position, bool arrow, int width)
        {
            Id = _ids++;
            _width = width;
            _position = position;
            _arrow = arrow;
            _jorny = 0;
            _probably = 0.0;
            _rounder = 0;
            _fronter = 0;
        }

        private void ChangePosition()
        {
            if (_fronter < MaxFronter)
            {
                _fronter++;
                return;
            }
            _fronter = 0;
            Position += _arrow ? 1 : -1;
            ++_jorny;
        }

        private void ChangeProbably()
        {
            _probably = _jorny / (double)PlaceCount;
        }

        private void TryChangeArrow()
        {
            if (_probably >= Rnd.NextDouble())
            {
                ChangeArrow();
            }
        }

        private void ChangeArrow()
        {
            if (_rounder == 0)
            {
                _arrow = !_arrow;
                _jorny = 0;
                _rounder = MaxRouter;
            }
        }

        public void Move()
        {
            if (_arrow && Position == PlaceCount - _width || !_arrow && Position == 0)
            {
                ChangeArrow();
            }
            ChangePosition();
            ChangeProbably();
            TryChangeArrow();
        }
    }
}
