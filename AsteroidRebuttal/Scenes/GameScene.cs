﻿using System;
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
using AsteroidRebuttal.Levels;

namespace AsteroidRebuttal.Scenes
{
    public class GameScene : Scene
    {
        public QuadTree quadtree {get; protected set;}
        public List<GameObject> gameObjects { get; private set; }
        public ScriptManager scriptManager;

        // This is the actual GAME window; the UI will appear to the side.
        public Rectangle ScreenArea { get; private set; }

        // This is the area of the UI.
        public Rectangle UIArea { get; private set; }

        public PlayerShip player;

        CollisionDetection collisionDetection;
        bool DrawQuadtree;

        LevelManager levelManager;

        //Content 
        Texture2D BossHealthbarFrameTexture;
        Texture2D BossHealthbarBackgroundTexture;
        Texture2D BossHealthbarTexture;
        Texture2D BossHealthbarDividerTexture;

        public Boss levelBoss;
        bool bossHealthbarFrameShown = false;
        Color bossHealthbarFrameColor = Color.Transparent;
        Color bossHealthbarColor = Color.Transparent;
        float bossHealthbarWidth = 0;
        float bossHealthbarMaxWidth = 675f;

        public override void Initialize()
        {
            scriptManager = new ScriptManager();
            gameObjects = new List<GameObject>();

            // Set the game area to 700 x 650.
            ScreenArea = new Rectangle(0, 0, 700, 650);
            
            // Set the UI window to 150 x 650, beginning after the ScreenArea.
            UIArea = new Rectangle(700, 0, 150, 650);

            quadtree = new QuadTree(0, ScreenArea);
            collisionDetection = new CollisionDetection(this);

            levelManager = new LevelManager(this);

            // Test
            levelManager.SetLevel(2);

            //new FinalBoss(this, new Vector2(350, -300));
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

            levelManager.Update(gameTime);
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
            levelManager.Draw(spriteBatch);

            foreach (GameObject go in gameObjects)
            {
                go.Draw(spriteBatch);
            }

            if(DrawQuadtree)
                quadtree.Draw(spriteBatch);

            if (bossHealthbarFrameShown)
            {
                spriteBatch.Draw(BossHealthbarBackgroundTexture, Vector2.Zero, bossHealthbarFrameColor);
                spriteBatch.Draw(BossHealthbarTexture, new Rectangle(20, 0, (int)bossHealthbarWidth, 20), bossHealthbarColor);
                spriteBatch.Draw(BossHealthbarFrameTexture, Vector2.Zero, bossHealthbarFrameColor);
                foreach (int i in levelBoss.PhaseChangeValues)
                {
                    spriteBatch.Draw(BossHealthbarDividerTexture, new Vector2(20 + ((i / levelBoss.MaxHealth) * bossHealthbarMaxWidth) - 2, 0), bossHealthbarColor);
                }
            }
            
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

        public IEnumerator<float> ShowBossHealthBar()
        {
            bossHealthbarFrameShown = true;
            bossHealthbarFrameColor = Color.Transparent;

            float elapsedTime = 0f;
            while (elapsedTime < 1f)
            {
                bossHealthbarColor = Color.Lerp(Color.Transparent, Color.White, elapsedTime);
                elapsedTime += .03f;
                yield return .03f;
            }

            bossHealthbarColor = Color.White;
            yield return .5f;
            elapsedTime = 0f;
            while (elapsedTime < 2f)
            {
                elapsedTime += .03f;

                bossHealthbarWidth = (elapsedTime / 2f) * bossHealthbarMaxWidth;
            }

            bossHealthbarWidth = bossHealthbarMaxWidth;
        }

        public void BossPhaseChange()
        {
            // Maybe play a sound or something here.
            bossHealthbarColor = Color.Lerp(Color.Yellow, Color.Red, levelBoss.Health / levelBoss.MaxHealth);
        }
    }
}
