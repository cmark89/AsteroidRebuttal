using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AsteroidRebuttal.Levels
{
    public class Level
    {
        protected List<ScrollingBackgroundLayer> scrollingBackground;
        protected LevelManager manager;

        public Level(LevelManager thisManager)
        {
            manager = thisManager;
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
        public virtual void SetupBackground(Texture2D bg, float bgSpeed)
        {
            Console.WriteLine("Set up the background");

            scrollingBackground = new List<ScrollingBackgroundLayer>();
            // Individually add each layer to the scrolling background...
            //scrollingBackground.Add(new ScrollingBackgroundLayer(thisScene, texture, scrollSpeed, color);
        }
    }
}
