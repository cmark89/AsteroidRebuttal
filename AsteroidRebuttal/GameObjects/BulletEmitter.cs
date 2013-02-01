using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using AsteroidRebuttal.Scenes;
using AsteroidRebuttal.Core;

namespace AsteroidRebuttal.GameObjects
{
    public class BulletEmitter : GameObject
    {
        // The parent of this bulletemitter
        GameObject parent;
        bool visible;

        public BulletEmitter(GameObject newParent, Vector2 newPosition, bool oneshot = false)
        {
            parent = newParent;
            thisScene = newParent.thisScene;
            Position = newPosition;

            if (oneshot)
                FlaggedForRemoval = true;

            Initialize();
        }

        public override void Initialize()
        {
            Hitbox = new Circle(Center, 0f);

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
            if(visible)
                base.Draw(spriteBatch);
        }

        public override void Destroy()
        {
        }

        // Simple method that creates a bullet and shoots it in the direction the emitter is facing
        public Bullet FireBullet(float velocity, Color newColor)
        {
            return new Bullet(parent, Position, Rotation, velocity, newColor);
        }

        // Method that creates a bullet and shoots it in any direction
        public Bullet FireBullet(float rotation, float velocity, Color newColor)
        {
            return new Bullet(parent, Position, rotation, velocity, newColor);
        }

        // Method that creates a bullet and shoots it in any direction
        public Bullet[] BulletExplosion(int numberOfBullets, float velocity, Color newColor)
        {
            List<Bullet> bullets = new List<Bullet>();
            
            
            float spread = ((float)(Math.PI * 2) / (float)numberOfBullets);

            for (int i = 0; i < numberOfBullets; i++ )
            {
                bullets.Add(FireBullet(Rotation + spread * i, velocity, newColor));
            }

            return bullets.ToArray();
        }


        // Method that creates a bullet and shoots it in any direction
        public Bullet[] BulletSpread(float rotation, int numberOfBullets, float spread, float velocity, Color newColor)
        {
            List<Bullet> bullets = new List<Bullet>();

            float angleDifference = spread / (float)numberOfBullets;
            float startAngle = rotation - (spread / 2);

            for (int i = 0; i < numberOfBullets; i++)
            {
                bullets.Add(FireBullet(startAngle + (angleDifference * i), velocity, newColor));
            }

            return bullets.ToArray();
        }
    }
}
