using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using AsteroidRebuttal;
using AsteroidRebuttal.GameObjects;
using AsteroidRebuttal.Core;
using AsteroidRebuttal.Scenes;
using AsteroidRebuttal.Scripting;

namespace AsteroidRebuttal.Enemies
{
    public class Boss : Enemy
    {
        protected static Texture2D testBossTexture;

        protected float currentGameTime;

        public Boss(GameScene newScene, Vector2 newPos = new Vector2()) : base(newScene, newPos)
        {
        }

        public override void Initialize()
        {
            CollisionLayer = 1;
            CollidesWithLayers = new int[] { 2 };

            base.Initialize();
        }

        public static void LoadContent(ContentManager content)
        {
            if (testBossTexture == null)
            {
                testBossTexture = content.Load<Texture2D>("Graphics/SHIP03");
            }
        }

        public override void Update(GameTime gameTime)
        {
            currentGameTime = (float)gameTime.TotalGameTime.TotalSeconds;
            scriptManager.Update(gameTime);

            base.Update(gameTime);
        }

        //public override void Draw()
        //{
        //}
    }
}
