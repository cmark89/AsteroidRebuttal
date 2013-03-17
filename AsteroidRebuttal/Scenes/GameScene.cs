using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ObjectivelyRadical;
using ObjectivelyRadical.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;
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

        float currentGameTime;

        // This is the actual GAME window; the UI will appear to the side.
        public Rectangle ScreenArea { get; private set; }

        // This is the area of the UI.
        public Rectangle GUIArea { get; private set; }

        public PlayerShip player;

        CollisionDetection collisionDetection;
        bool DrawQuadtree;

        LevelManager levelManager;

        //Content 
        public static Texture2D BossHealthbarFrameTexture;
        public static Texture2D BossHealthbarBackgroundTexture;
        public static Texture2D BossHealthbarTexture;
        public static Texture2D BossHealthbarDividerTexture;

        public static Texture2D GUITexture;
        public static Texture2D GUITextureExperienceFrame;
        public static Texture2D GUIRankOrb;

        public static Texture2D GUIBossWarning;
        public bool BossWarningShown;

        public static SoundEffect Rank1;
        public static SoundEffect Rank2;
        public static SoundEffect Rank3;
        public static SoundEffect Rank4;

        public static SoundEffect BossWarning;
        public static SoundEffect BossAlarm;

        public int Score = 0;
        public int ScoreMultiplier = 1;
        public int GrazeCount = 0;
        public int GrazeValue
        {
            get
            {
                return (1 + (GrazeCount / 6)) * ScoreMultiplier;
            }
        }

        public int Lives = 2;
        public float Experience = 0f;
        public float NextRankUp = 20f;
        public float ExperienceDecay = 1f;
        public bool ExperienceDecayPaused = false;
        public float ExperienceDecayPauseDuration = 0f;
        public int Rank = 0;

        SpriteFont scoreFont;
        SpriteFont extendFont;
        SpriteFont livesFont;


        bool bossHealthbarAnimationComplete = false;

        public Boss levelBoss;
        bool bossHealthbarFrameShown = false;
        Color bossHealthbarFrameColor = Color.Transparent;
        Color bossHealthbarColor = Color.Transparent;
        float bossHealthbarWidth = 0;
        float bossHealthbarMaxWidth = 675f;

        Color BossWarningColor = Color.Transparent;
        Color bossWarningStartColor;
        Color bossWarningEndColor;
        float bossWarningColorLerpStartTime;
        float bossWarningColorLerpEndTime;

        public override void Initialize()
        {
            scriptManager = new ScriptManager();
            gameObjects = new List<GameObject>();

            // Set the game area to 700 x 650.
            ScreenArea = new Rectangle(0, 0, 700, 650);
            
            // Set the UI window to 150 x 650, beginning after the ScreenArea.
            GUIArea = new Rectangle(700, 0, 225, 650);

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
            //TEST
            Console.WriteLine("Loaded level content!");
            BossHealthbarBackgroundTexture = content.Load<Texture2D>("Graphics/GUI/HealthBarBackground");
            BossHealthbarFrameTexture = content.Load<Texture2D>("Graphics/GUI/HealthBarFrame");
            BossHealthbarDividerTexture = content.Load<Texture2D>("Graphics/GUI/HealthBarDivision");
            BossHealthbarTexture = content.Load<Texture2D>("Graphics/GUI/HealthBar");

            GUITexture = content.Load<Texture2D>("Graphics/GUI/GUI");
            GUITextureExperienceFrame = content.Load<Texture2D>("Graphics/GUI/GUIExperienceFrame");
            GUIRankOrb = content.Load<Texture2D>("Graphics/GUI/RankOrb");
            scoreFont = content.Load<SpriteFont>("Fonts/ScoreFont");
            extendFont = content.Load<SpriteFont>("Fonts/ExtendFont");
            livesFont = content.Load<SpriteFont>("Fonts/LivesFont");

            GUIBossWarning = content.Load<Texture2D>("Graphics/GUI/BossWarning");

            Rank1 = content.Load<SoundEffect>("Audio/AI/LockAndLoad");
            Rank2 = content.Load<SoundEffect>("Audio/AI/BadBoy");
            Rank3 = content.Load<SoundEffect>("Audio/AI/BulletRider");
            Rank4 = content.Load<SoundEffect>("Audio/AI/HeckYeah");

            BossWarning = content.Load<SoundEffect>("Audio/AI/BossWarning");
            BossAlarm = content.Load<SoundEffect>("Audio/AI/WarningAlarm");
        }


        public override void Update(GameTime gameTime)
        {
            // Get rid of unneeded objects.
            RemoveObjectsOutsideScreen();

            currentGameTime = (float)gameTime.TotalGameTime.TotalSeconds;

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

            if (Experience > 0)
            {
                if (!ExperienceDecayPaused)
                {
                    Experience -= ExperienceDecay * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (Experience <= 0)
                        ResetRank();
                }
                else if (ExperienceDecayPaused && ExperienceDecayPauseDuration > 0)
                {
                    ExperienceDecayPauseDuration -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if(ExperienceDecayPauseDuration <= 0)
                        ExperienceDecayPaused = false;
                }
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
            levelManager.Draw(spriteBatch);

            foreach (GameObject go in gameObjects)
            {
                go.Draw(spriteBatch);
            }

            if(DrawQuadtree)
                quadtree.Draw(spriteBatch);

            if (bossHealthbarFrameShown)
            {
                if(bossHealthbarAnimationComplete)
                    bossHealthbarWidth = (levelBoss.Health / levelBoss.MaxHealth) * bossHealthbarMaxWidth;

                spriteBatch.Draw(BossHealthbarBackgroundTexture, Vector2.Zero, bossHealthbarFrameColor);

                spriteBatch.Draw(BossHealthbarTexture, new Rectangle(20, 0, (int)bossHealthbarWidth, 20), bossHealthbarColor);

                spriteBatch.Draw(BossHealthbarFrameTexture, Vector2.Zero, bossHealthbarFrameColor);

                foreach (int i in levelBoss.PhaseChangeValues)
                {
                    spriteBatch.Draw(BossHealthbarDividerTexture, new Vector2(20 + (((float)i / (float)levelBoss.MaxHealth) * bossHealthbarMaxWidth) - 2, 0), bossHealthbarFrameColor);
                }
            }

            // Draw the GUI!
            spriteBatch.Draw(GUITexture, GUIArea, Color.White);

            //draw experience bar
            spriteBatch.Draw(BossHealthbarTexture, new Rectangle(GUIArea.X + 36, 166, (int)(150 * (Experience / NextRankUp)), 22), Color.PaleGreen);
            spriteBatch.Draw(GUITextureExperienceFrame, new Vector2(GUIArea.X, 163), Color.White);

            if (Rank > 0)
            {
                for (int i = 0; i < Rank; i++)
                { 
                    spriteBatch.Draw(GUIRankOrb, new Vector2(GUIArea.X + 57 + (28 * i), 138), Color.White);
                }
            }

            if (ScoreMultiplier > 1)
            {
                spriteBatch.DrawString(scoreFont, "X" + ScoreMultiplier.ToString(), new Vector2(GUIArea.X + 176, 193), Color.Gold);
            }

            spriteBatch.DrawString(scoreFont, String.Format("{0:000,000,000}",Score), new Vector2(GUIArea.X + 42, 38), Color.White);
            spriteBatch.DrawString(extendFont, "Next Extend: 1,000,000", new Vector2(GUIArea.X + 42, 74), Color.Gold);
            spriteBatch.DrawString(scoreFont, GrazeCount.ToString(), new Vector2(GUIArea.X + 108, 234), Color.Teal);
            spriteBatch.DrawString(scoreFont, GrazeValue.ToString(), new Vector2(GUIArea.X + 108, 276), Color.Teal);
            spriteBatch.DrawString(livesFont, Lives.ToString(), new Vector2(GUIArea.X + 46, 602), Color.White);

            if (BossWarningShown)
            {
                spriteBatch.Draw(GUIBossWarning, new Vector2((ScreenArea.Width / 2f) - (GUIBossWarning.Width / 2f), (ScreenArea.Height / 2f) - (GUIBossWarning.Height / 2f)), BossWarningColor);
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
                bossHealthbarFrameColor = Color.Lerp(Color.Transparent, Color.White, elapsedTime);
                elapsedTime += .03f;
                yield return .03f;
            }

            bossHealthbarFrameColor = Color.White;
            elapsedTime = 0f;
            bossHealthbarColor = Color.Yellow;

            while (elapsedTime < .5f)
            {
                bossHealthbarWidth = (elapsedTime / .5f) * bossHealthbarMaxWidth;
                elapsedTime += .01f;
                yield return .01f;
            }

            bossHealthbarAnimationComplete = true;
            bossHealthbarWidth = bossHealthbarMaxWidth;
        }

        public void BossPhaseChange()
        {
            // Maybe play a sound or something here.
            bossHealthbarColor = Color.Lerp(Color.Red, Color.Yellow, ((float)levelBoss.Health / (float)levelBoss.MaxHealth));
        }

        public void GainExperience(float xpGain)
        {
            Experience += xpGain;
            if (Experience >= NextRankUp)
                RankUp();
        }

        public void RankUp()
        {
            Rank++;
            Experience = 5;
            NextRankUp += 5;
            ExperienceDecay *= 1.5f;
            ScoreMultiplier *= 2;
            PauseExperienceDecay(5f);

            switch (Rank)
            {
                case (1):
                    Rank1.Play(.8f, 0f, 0f);
                    break;
                case (2):
                    Rank2.Play(.8f, 0f, 0f);
                    break;
                case (3):
                    Rank3.Play(.8f, 0f, 0f);
                    break;
                case (4):
                    Rank4.Play(.8f, 0f, 0f);
                    break;
                default:
                    break;
            }
        }

        public void ResetRank()
        {
            Rank = 0;
            Experience = 0;
            ExperienceDecay = .5f;
            ScoreMultiplier = 1;
            NextRankUp = 20f;
        }

        public void PauseExperienceDecay()
        {
            ExperienceDecayPaused = true;
        }

        public void PauseExperienceDecay(float duration)
        {
            ExperienceDecayPaused = true;
            ExperienceDecayPauseDuration = duration;
        }

        public void ResumeExperienceDecay()
        {
            ExperienceDecayPaused = false;
        }

        public void PlayBossWarning()
        {
            scriptManager.Execute(ShowBossWarning);
        }

        //6.5 second total including final fade out
        public IEnumerator<float> ShowBossWarning()
        {
            PauseExperienceDecay(10f);
            BossWarningShown = true;
            LerpBossWarningColor(Color.White, 1.5f);

            BossAlarm.Play(.8f, 0f, 0f);
            yield return .75f;

            BossWarning.Play(.6f, 0f, 0f);
            yield return .75f;


            for (int i = 0; i < 2; i++)
            {
                BossAlarm.Play(.8f, 0f, 0f);
                LerpBossWarningColor(new Color(.5f, .5f, .5f, .5f), 1f);
                yield return 1f;
                LerpBossWarningColor(Color.White, 1f);
                yield return 1f;
            }

            LerpBossWarningColor(Color.Transparent, 1f);
            yield return 1f;
            BossWarningShown = false;
        }

        public void LerpBossWarningColor(Color targetColor, float duration)
        {
            bossWarningStartColor = BossWarningColor;
            bossWarningEndColor = targetColor;

            bossWarningColorLerpStartTime = currentGameTime;
            bossWarningColorLerpEndTime = currentGameTime + duration;

            scriptManager.Execute(BossWarningColorLerpScript);
        }

        public IEnumerator<float> BossWarningColorLerpScript()
        {
            while (currentGameTime < bossWarningColorLerpEndTime)
            {
                Console.WriteLine("LERP BOSS COLOR!");
                BossWarningColor = Color.Lerp(bossWarningStartColor, bossWarningEndColor, (currentGameTime - bossWarningColorLerpStartTime) / (bossWarningColorLerpEndTime - bossWarningColorLerpStartTime));
                yield return 0.00f;
            }

            BossWarningColor = bossWarningEndColor;
        }
    }
}
