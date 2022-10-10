using Darkorbit.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darkorbit.Game.Movements
{
    class Position
    {
        //public static Position MMOPosition = new Position(1800, 7300);
        //public static Position EICPosition = new Position(9500, 2800);
        //public static Position EICPosition = new Position(10400, 1800);
        //public static Position VRUPosition = new Position(18500, 7300);
        //public static Position VRUPosition = new Position(19100, 6500);

        public static Position MMOPosition = new Position(2000, 6400);
        public static Position EICPosition = new Position(18900, 2000);
        public static Position VRUPosition = new Position(18900, 6400);
        public static Position TDMEIC = new Position(18900, 2000);
        public static Position TDMVRU = new Position(18900, 6400);
        public static Position TDMMMO = new Position(2000, 6400);
        public static Position newMMOPosition = new Position(2000, 6400);
        public static Position newEICPosition = new Position(18900, 2000);
        public static Position newVRUPosition = new Position(18900, 6400);

        public int X { get; set; }
        public int Y { get; set; }

        public Position(int x, int y)
        {
            X = x;
            Y = y;
        }

        public double DistanceTo(Position point)
        {
            return Math.Sqrt(Math.Pow(point.X - X, 2) + Math.Pow(point.Y - Y, 2));
        }

        public static Position Random(Spacemap map, int minX = 0, int maxX = 0, int minY = 0, int maxY = 0)
        {
            if (minX == 0) minX = map.Limits[0].X;
            if (minY == 0) minY = map.Limits[0].Y;
            if (maxX == 0) maxX = map.Limits[1].X;
            if (maxY == 0) maxY = map.Limits[1].Y;

            var posX = Randoms.random.Next(minX, maxX);
            var posY = Randoms.random.Next(minY, maxY);
            return new Position(posX, posY);
        }

        public static Position GetPosOnCircle(Position circleCenter, int radius)
        {
            var a = Randoms.random.Next(0, 360);
            var calculateX = circleCenter.X + Convert.ToInt32(radius * Math.Cos(a));
            var calculateY = circleCenter.Y + Convert.ToInt32(radius * Math.Sin(a));

            return new Position(calculateX, calculateY);
        }
    }
}
