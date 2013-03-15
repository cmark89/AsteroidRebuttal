using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using ObjectivelyRadical;
using AsteroidRebuttal.GameObjects;
using AsteroidRebuttal.Enemies;
using ObjectivelyRadical.Scripting;
using AsteroidRebuttal.Core;
using AsteroidRebuttal.Enemies.Bosses;

namespace AsteroidRebuttal.Levels
{
    public class Level2 : Level
    {
        // Content
        public static Texture2D Level2GroundTexture;
        public static Song Level2Theme;


        public Level2(LevelManager thisManager) : base(thisManager) { }

        public override void Initialize()
        {
            SetupBackground();
        }

        public override void SetupBackground()
        {
            Console.WriteLine("Set up the background for level 2!");

            scrollingBackground = new List<ScrollingBackgroundLayer>();

            // Individually add each layer to the scrolling background...
            scrollingBackground.Add(new ScrollingBackgroundLayer(manager.thisScene, Level2GroundTexture, 100f, Color.White));
        }

        public override IEnumerator<float> LevelScript()
        {
            Enemy e;

            //AudioManager.PlaySong(Level2Theme, false, .5f);
            yield return 4f;

            //SPAWN THE BOSS
            Boss boss = new PhantomBoss(manager.thisScene, new Vector2(350f, -20f));
            BeginBossBattle(boss);
            yield return 3f;
            AudioManager.PlaySong(BossTheme);


        }


    }
}
