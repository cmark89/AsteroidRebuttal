using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ObjectivelyRadical;
using ObjectivelyRadical.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using AsteroidRebuttal.Scenes;
using AsteroidRebuttal.GameObjects;
using AsteroidRebuttal.Enemies;
using AsteroidRebuttal.Enemies.Bosses;
using AsteroidRebuttal.Scripting;
using AsteroidRebuttal.Core;
using Microsoft.Xna.Framework.Media;

namespace AsteroidRebuttal.Levels
{
    public class LevelManager
    {
        public GameScene thisScene;
        ScriptManager scriptManager;
        Level currentLevel;

        public LevelManager(GameScene newScene)
        {
            thisScene = newScene;
            scriptManager = newScene.scriptManager;
        }

        public static void LoadContent(ContentManager content)
        {
            //TEST
            Console.WriteLine("Loaded level content!");
            Level1.Level1GroundTexture = content.Load<Texture2D>("Graphics/Backgrounds/level1");
            //Level2.Level2Texture = content.Load<Texture2D>("Graphics/Backgrounds/level2");
            //Level3Texture = content.Load<Texture2D>("Graphics/Backgrounds/level3");
            //Level4Texture = content.Load<Texture2D>("Graphics/Backgrounds/level4");
            //Level5Texture = content.Load<Texture2D>("Graphics/Backgrounds/level5");

            Level1.Level1Theme = content.Load<Song>("Audio/Music/ThisWarTornMote");
            //Level2.Level1Theme = content.Load<Song>("Audio/Music/PaleBlueBattlefield");
            //Level3.Level1Theme = content.Load<Song>("Audio/Music/ApolloGraveyard");
            //Level4.Level1Theme = content.Load<Song>("Audio/Music/TechnoFuture");
            //Level5.Level1Theme = content.Load<Song>("Audio/Music/Showdown");

            Level.BossTheme = content.Load<Song>("Audio/Music/BossTheme");
        }

        public void Update(GameTime gameTime)
        {
            if (currentLevel != null)
                currentLevel.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (currentLevel != null)
                currentLevel.Draw(spriteBatch);
        }

        public void SetLevel(int levelNumber)
        {
            Level newLevel = new Level(this);
            switch (levelNumber)
            {
                case(1):
                    newLevel = new Level1(this);
                    break;
                case (2):
                    //Set level = 2
                    break;
                case (3):
                    //Set level = 3
                    break;
                case (4):
                    //Set level = 4
                    break;
                case (5):
                    //Set level = 5
                    break;
                default:
                    break;
            }

            currentLevel = newLevel;
            scriptManager.Execute(currentLevel.LevelScript);
        }
    }
}
