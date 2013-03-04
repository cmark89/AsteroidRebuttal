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

        public PlayerShip player;

        CollisionDetection collisionDetection;
        bool DrawQuadtree;

        public override void Initialize()
        {
            scriptManager = new ScriptManager();
            gameObjects = new List<GameObject>();
            ScreenArea = new Rectangle(0, 0, AsteroidRebuttal.graphics.GraphicsDevice.Viewport.Width, AsteroidRebuttal.graphics.GraphicsDevice.Viewport.Height);
            quadtree = new QuadTree(0, ScreenArea);
            collisionDetection = new CollisionDetection(this);

            
            new ParasiteBoss(this, new Vector2(350, -300));
            player = new PlayerShip(this, new Vector2(100, 200));
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
            }

            foreach (GameObject go in gameObjects.FindAll(x => !x.IsNewObject))
            {
                quadtree.Insert(go);
            }

            scriptManager.Update(gameTime);
            collisionDetection.BroadphaseCollisionDetection();
            
            // Respawn function
            if (KeyboardManager.KeyPressedUp(Microsoft.Xna.Framework.Input.Keys.F1))
            {
                player.Destroy();
                player = new PlayerShip(this, new Vector2(350, 450));
            }
            if (KeyboardManager.KeyPressedUp(Microsoft.Xna.Framework.Input.Keys.Q))
            {
                DrawQuadtree = !DrawQuadtree;
            }
        }

        public void RemoveObjectsOutsideScreen()
        {
            foreach (GameObject go in gameObjects)
            {
                if (go.Center.X < ScreenArea.X - go.DeletionBoundary.X ||
                    go.Center.X > ScreenArea.Width + go.DeletionBoundary.X ||
                    go.Center.Y < ScreenArea.Y - go.DeletionBoundary.Y ||
                    go.Center.Y > ScreenArea.Height + go.DeletionBoundary.Y)
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

            if(DrawQuadtree)
                quadtree.Draw(spriteBatch);
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
