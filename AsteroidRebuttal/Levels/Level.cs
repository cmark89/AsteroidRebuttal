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
        ScrollingBackground[] scrollingBackground = new ScrollingBackground[2];

        LevelManager manager;

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
            foreach (ScrollingBackground sb in scrollingBackground)
            {
                sb.Update(gameTime);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (ScrollingBackground sb in scrollingBackground)
            {
                spriteBatch.Draw(sb.texture, new Rectangle((int)sb.position.X, (int)sb.position.Y, sb.texture.Width, sb.texture.Height), Color.White);
            }
        }

        public virtual IEnumerator<float> LevelScript()
        {
            // This is the script for the level; controls enemy spawning and events.
            yield return 0f;
        }

        // Sets up the background to begin scrolling.
        public void SetupBackground(Texture2D bg, float bgSpeed)
        {
            Console.WriteLine("Set up the background");

            scrollingBackground = new ScrollingBackground[2];
            for (int i = 0; i < 2; i++)
            {
                scrollingBackground[i] = new ScrollingBackground(bg, bgSpeed);

                scrollingBackground[i].position = new Vector2(0, manager.thisScene.ScreenArea.Height - (scrollingBackground[i].texture.Height * i));
                scrollingBackground[i].screenArea = manager.thisScene.ScreenArea;
            }
        }
    }

    public class ScrollingBackground
    {
        public Texture2D texture;
        public Vector2 position;
        float scrollSpeed;
        public Rectangle screenArea;

        public ScrollingBackground(Texture2D newTexture, float scrollingSpeed)
        {
            texture = newTexture;
            scrollSpeed = scrollingSpeed;
        }

        public void Update(GameTime gameTime)
        {
            position.Y += (float)gameTime.ElapsedGameTime.TotalSeconds * scrollSpeed;

            if (position.Y >= screenArea.Height)
            {
                position.Y = screenArea.Y - texture.Height;
            }
        }
    }
}
