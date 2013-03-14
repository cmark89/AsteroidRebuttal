using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AsteroidRebuttal.Levels
{
    public class Level1 : Level
    {
        public static Texture2D Level1Texture;

        public Level1(LevelManager thisManager) : base(thisManager) { }

        public override void Initialize()
        {
            SetupBackground(Level1Texture, 100);
        }

        public override void SetupBackground(Texture2D bg, float bgSpeed)
        {
            Console.WriteLine("Set up the background for level 1!");

            scrollingBackground = new List<ScrollingBackgroundLayer>();

            // Individually add each layer to the scrolling background...
            scrollingBackground.Add(new ScrollingBackgroundLayer(manager.thisScene, Level1Texture, 100f, Color.White));
        }

        public override IEnumerator<float> LevelScript()
        {
            scrollingBackground[0].LerpSpeed(10f, 4f);
            yield return 1f;
        }
    }
}
