using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using AsteroidRebuttal.Scripting;
using AsteroidRebuttal.Core;

namespace AsteroidRebuttal.GameObjects
{
    public class Bullet : GameObject
    {
        #region Bullet Graphics
        public static Texture2D SimpleBulletTexture;

        #endregion

        // Stores the color of the bullet.
        Color bulletColor;

        // Stores the GameObject that produced this bullet.
        GameObject parent;

        // Stores the script manager that belongs to the parent if it exists
        
        public Bullet(GameObject newParent, Vector2 newPosition, float newRotation, float newVelocity, GameObjectScript birthEvent = null, GameObjectScript deathEvent = null)
        {
            parent = newParent; 

            Position = newPosition;
            Rotation = newRotation;
            Velocity = newVelocity;

            bulletColor = Color.White;

            Initialize();
        }

        public Bullet(GameObject newParent, Vector2 newPosition, float newRotation, float newVelocity, Color newColor, GameObjectScript birthEvent = null, GameObjectScript deathEvent = null)
        {
            parent = newParent;

            Position = newPosition;
            Rotation = newRotation;
            Velocity = newVelocity;

            bulletColor = newColor;

            Initialize();
        }

        public override void Initialize()
        {
            // Switch on the bullet type
            Texture = SimpleBulletTexture;
            thisScene = parent.thisScene;

            Origin = new Vector2(15f, 8f);

            // For test purposes.
            Hitbox = new Circle(Center, 5f);

            base.Initialize();
        }

        public static void LoadContent(ContentManager content)
        {
            if (SimpleBulletTexture == null)
            {
                //SimpleBulletTexture = content.Load<Texture2D>("Graphics/simpleBullet");
                SimpleBulletTexture = content.Load<Texture2D>("Graphics/arrowBullet");
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Center, Texture.Bounds, bulletColor, Rotation + ((float)Math.PI / 2), Origin, 1f, SpriteEffects.None, 0f);
            //spriteBatch.Draw(PlayerShip.hitboxTexture, new Vector2(Center.X - 4, Center.Y - 4), Color.White);
        }
    }


    public enum BulletType
    {
        Simple
    }
}
