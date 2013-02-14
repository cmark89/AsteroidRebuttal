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
        protected static Texture2D boss2Texture;

        protected float currentGameTime;

        public Boss(GameScene newScene, Vector2 newPos = new Vector2()) : base(newScene, newPos)
        {
        }

        public override void Initialize()
        {
            Color = Color.Transparent;
            CollisionLayer = 1;
            CollidesWithLayers = new int[] { 2 };
            Origin = new Vector2(127.5f, 101.5f);
            Hitbox = new Circle(Center, 12.5f);

            base.Initialize();
        }

        public static void LoadContent(ContentManager content)
        {
            if (testBossTexture == null)
            {
                testBossTexture = content.Load<Texture2D>("Graphics/Ships/Enemy2");
            }
            if (boss2Texture == null)
            {
                boss2Texture = content.Load<Texture2D>("Graphics/Ships/Boss2");
            }
        }

        public override void Update(GameTime gameTime)
        {
            currentGameTime = (float)gameTime.TotalGameTime.TotalSeconds;

            base.Update(gameTime);
        }
    }
}
