using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ObjectivelyRadical;
using ObjectivelyRadical.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using AsteroidRebuttal.GameObjects;
using AsteroidRebuttal.Enemies;
using AsteroidRebuttal.Enemies.Bosses;
using AsteroidRebuttal.Scripting;
using AsteroidRebuttal.Core;

namespace AsteroidRebuttal.Scenes
{
    public class GameScene : Scene
    {
        public QuadTree quadtree {get; protected set;}
        public List<GameObject> gameObjects { get; private set; }
        public ScriptManager scriptManager;

        public Rectangle ScreenArea { get; private set; }
        float gameAreaBoundary = 100f;

        public PlayerShip player;

        CollisionDetection collisionDetection;

        public override void Initialize()
        {

            scriptManager = new ScriptManager();
            gameObjects = new List<GameObject>();
            ScreenArea = new Rectangle(0, 0, AsteroidRebuttal.graphics.GraphicsDevice.Viewport.Width, AsteroidRebuttal.graphics.GraphicsDevice.Viewport.Height);
            quadtree = new QuadTree(0, ScreenArea);
            collisionDetection = new CollisionDetection(this);

            player = new PlayerShip(this, new Vector2(100, 200));
            new TestBoss(this, new Vector2(100, 50));
        }


        public override void LoadContent(ContentManager content)
        {
        }


        public override void Update(GameTime gameTime)
        {
            // Get rid of unneeded objects.
            RemoveObjectsOutsideScreen();

            foreach (GameObject go in gameObjects.FindAll(x => x.FlaggedForRemoval))
            {
                scriptManager.AbortObjectScripts(go);
                gameObjects.Remove(go);
            }

            foreach (GameObject go in gameObjects.FindAll(x => x.IsNewObject))
            {
                go.IsNewObject = false;
            }

            // Populate the quadtree in preparation for collision checking.
            quadtree.Clear();

            foreach (GameObject go in gameObjects.FindAll(x => !x.IsNewObject))
            {
                go.Update(gameTime);
                quadtree.Insert(go);
            }

            collisionDetection.BroadphaseCollisionDetection();
        }

        public void RemoveObjectsOutsideScreen()
        {
            foreach (GameObject go in gameObjects)
            {
                if (go.Position.X < ScreenArea.X - gameAreaBoundary ||
                    go.Position.X > ScreenArea.Width + gameAreaBoundary ||
                    go.Position.Y < ScreenArea.Y - gameAreaBoundary ||
                    go.Position.Y > ScreenArea.Height + gameAreaBoundary)
                {
                    go.Destroy();
                }
                        
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            foreach (GameObject go in gameObjects)
            {
                go.Draw(spriteBatch);
            }

            //quadtree.Draw(spriteBatch);
        }

        public void AddGameObject(GameObject newObject)
        {
            gameObjects.Add(newObject);
        }


        public override void Unload()
        {
        }


        // Sets the game's level
        public void SetLevel()
        {
        }
    }
}
