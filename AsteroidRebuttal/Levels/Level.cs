﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using AsteroidRebuttal.Enemies;
using AsteroidRebuttal.Scripting;

namespace AsteroidRebuttal.Levels
{
    public class Level
    {
        protected List<ScrollingBackgroundLayer> scrollingBackground;
        protected LevelManager manager;
        protected ScriptManager scriptManager;

        protected Boss levelBoss;

        // Content
        public static Song BossTheme;

        public Level(LevelManager thisManager)
        {
            manager = thisManager;
            scriptManager = manager.thisScene.scriptManager;
            Initialize();
        }

        public virtual void Initialize()
        {
        }

        public void Update(GameTime gameTime)
        {
            foreach (ScrollingBackgroundLayer sbl in scrollingBackground)
            {
                sbl.Update(gameTime);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (ScrollingBackgroundLayer sbl in scrollingBackground)
            {
                sbl.Draw(spriteBatch);
            }
        }

        public virtual IEnumerator<float> LevelScript()
        {
            // This is the script for the level; controls enemy spawning and events.
            yield return 0f;
        }

        // Sets up the background to begin scrolling.
        public virtual void SetupBackground()
        {
            Console.WriteLine("Set up the background");

            scrollingBackground = new List<ScrollingBackgroundLayer>();
            // Individually add each layer to the scrolling background...
            //scrollingBackground.Add(new ScrollingBackgroundLayer(thisScene, texture, scrollSpeed, color);
        }

        public Enemy SpawnEnemy(EnemyType newType, Vector2 position)
        {
            Enemy newEnemy = null;
            switch (newType)
            {
                case(EnemyType.Slicer):
                    newEnemy = new Slicer(manager.thisScene, position);
                    break;
                case (EnemyType.Tortoise):
                    newEnemy = new Tortoise(manager.thisScene, position);
                    break;
                case (EnemyType.Dragon):
                    //newEnemy = new Slicer(manager.thisScene, position);
                    break;
                case (EnemyType.Komodo):
                    //newEnemy = new Slicer(manager.thisScene, position);
                    break;
                default:
                    break;
            }

            return newEnemy;
        }

        public Enemy SpawnEnemyAtAngle(EnemyType type, Vector2 position, float angle, float velocity)
        {
            Enemy e;
            e = SpawnEnemy(type, position);
            e.Rotation = angle;
            e.DrawAtTrueRotation = true;
            e.Velocity = velocity;

            return e;
        }

        public void BeginBossBattle(Boss thisBoss)
        {
            manager.thisScene.levelBoss = thisBoss;
            scriptManager.Execute(manager.thisScene.ShowBossHealthBar);
        }
    }
}
