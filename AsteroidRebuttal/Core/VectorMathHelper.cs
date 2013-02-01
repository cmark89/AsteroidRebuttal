using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AsteroidRebuttal.Scenes;
using Microsoft.Xna.Framework;

namespace AsteroidRebuttal.Core
{
    public class VectorMathHelper
    {
        static Random rand = new Random();
        public static float GetAngleTo(Vector2 from, Vector2 to)
        {
            if (from == to)
                return 0f;

            // First, normalize the two vectors.
            to -= from;
            from = new Vector2(1, 0);
            to.Normalize();

            float angle = (float)Math.Atan2(to.Y, to.X) - (float)Math.Atan2(from.Y, from.X);
            return angle;
        }

        public static float GetAngleTo(Vector2 from, Vector2 to, float randomSpread)
        {
            // Gets a random between 1 and -1
            float randomAngle = (((float)rand.NextDouble() * 2) - 1f) * randomSpread / 2;
        
            return GetAngleTo(from, to) + randomAngle;
        }

        public static float DegreesToRadians(float degrees)
        {
            return (degrees / 360f) * (2 * (float)Math.PI);
        }
    }
}
