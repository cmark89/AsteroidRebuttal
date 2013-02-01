using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using AsteroidRebuttal.Scenes;
using AsteroidRebuttal.GameObjects;

namespace AsteroidRebuttal.Core
{
    public class CollisionDetection
    {
        GameScene thisScene;

        // Performs a broad sweep of objects in the game.
        public void DetectCollisions()
        {
            int checks = 0;
            // Enumerate through ICollidable objects that are not phasing.
            foreach (ICollidable ic in thisScene.gameObjects.FindAll(x => !x.Phasing))
            {
                // Next, get all the objects from that object's sector of the quadtree.
                List<GameObject> collisionCandidates = thisScene.quadtree.Retrieve((GameObject)ic);

                // For each object returned that is not phasing and that the ICollidable is able to collide with...
                foreach(GameObject go in collisionCandidates.FindAll(x => !x.Phasing && ic.CollidesWithLayers.Contains(x.CollisionLayer)))
                {
                    checks++;
                    // Perform narrow phase collision detection on the object.
                }

                Console.WriteLine(checks + " collisions checked this frame.");
            }
        }
    }



    public class Circle
    {
        public float Radius;
        public Vector2 Center;

        public Circle()
        {
            Center = Vector2.Zero;
            Radius = 0f;
        }

        public Circle(Vector2 newCenter, float radius)
        {
            Radius = radius;
            Center = newCenter;
        }
    }
}
