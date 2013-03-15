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
        protected static Texture2D boss3Texture;
        protected static Texture2D boss3EggTexture;
        protected static Texture2D boss4Texture;
        protected static Texture2D boss4OrbTexture;
        protected static Texture2D boss5BaseTexture;
        protected static Texture2D boss5Ring1Texture;
        protected static Texture2D boss5Ring2Texture;
        protected static Texture2D boss5TopTexture;

        protected float currentGameTime;

        public List<int> PhaseChangeValues {get; protected set;}
        public int MaxHealth { get; protected set; }
        private bool _vulnerable = false;
        public bool Vulnerable
        {
            get
            {
                return _vulnerable;
            }
            protected set
            {
                _vulnerable = value;
            }
        }

        public Boss(GameScene newScene, Vector2 newPos = new Vector2()) : base(newScene, newPos)
        {
        }

        public override void Initialize()
        {
            if (Color == null)
                Color = Color.White;

            CollisionLayer = 1;

            if(Origin == null)
                Origin = new Vector2(127.5f, 101.5f);

            if (DeletionBoundary == null)
                DeletionBoundary = new Vector2(9999, 9999);

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
            if (boss3Texture == null)
            {
                boss3Texture = content.Load<Texture2D>("Graphics/Ships/Boss3");
            }
            if (boss3EggTexture == null)
            {
                boss3EggTexture = content.Load<Texture2D>("Graphics/Ships/Boss3Egg");
            }
            if (boss4Texture == null)
            {
                boss4Texture = content.Load<Texture2D>("Graphics/Ships/Boss4");
            }
            if (boss4OrbTexture == null)
            {
                boss4OrbTexture = content.Load<Texture2D>("Graphics/Ships/Boss4Orb");
            }
            if (boss5BaseTexture == null)
            {
                boss5BaseTexture = content.Load<Texture2D>("Graphics/Ships/Boss5Base");
            }
            if (boss5Ring1Texture == null)
            {
                boss5Ring1Texture = content.Load<Texture2D>("Graphics/Ships/Boss5Ring1");
            }
            if (boss5Ring2Texture == null)
            {
                boss5Ring2Texture = content.Load<Texture2D>("Graphics/Ships/Boss5Ring2");
            }
            if (boss5TopTexture == null)
            {
                boss5TopTexture = content.Load<Texture2D>("Graphics/Ships/Boss5Top");
            }

        }

        public override void Update(GameTime gameTime)
        {
            currentGameTime = (float)gameTime.TotalGameTime.TotalSeconds;

            base.Update(gameTime);
        }


    }
}
