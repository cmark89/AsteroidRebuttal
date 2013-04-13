using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ObjectivelyRadical.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using AsteroidRebuttal.Core;
using AsteroidRebuttal.Scenes;
using AsteroidRebuttal.Scripting;
using AsteroidRebuttal.GameObjects;
using AsteroidRebuttal.Enemies;
using ObjectivelyRadical;

namespace AsteroidRebuttal.GameObjects
{
    public class PlayerShip : GameObject, ICollidable
    {
        static Texture2D playerShipTexture;
        public static Texture2D hitboxTexture;

        public ScriptManager scriptManager;
        List<Bullet> bullets;

        float speed = 250;
        float currentGameTime;

        public int[] CollidesWithLayers { get; set; }
        public event CollisionEventHandler OnOuterCollision;
        public event CollisionEventHandler OnInnerCollision;
        public List<GameObject> CollidedObjects { get; set;  }

        // Placeholder until cool weapons come
        private BulletEmitter mainEmitter;
        private float nextFireTime;

        public bool CanFire { get; set; }

        public PlayerShip(GameScene newScene = null, Vector2 position = new Vector2())
        {
            thisScene = newScene;
            Center = position;

            Initialize();
        }

        public override void Initialize()
        {
            Console.WriteLine("Player initialized!");
            Texture = playerShipTexture;

            DrawLayer = .4f;

            if (thisScene == null)
                Console.WriteLine("This scene is null!?");
            scriptManager = thisScene.scriptManager;

            bullets = new List<Bullet>();

            Origin = new Vector2(23.5f, 23.5f);
            Hitbox = new Circle(Center, 20f);
            UsesInnerHitbox = true;
            InnerHitbox = new Circle(new Vector2(23.5f, 19.5f), 4f);

            mainEmitter = new BulletEmitter(this, Center, false);
            mainEmitter.LockedToParentPosition = true;

            CollidesWithLayers = new int[] { 0, 1 };
            CollisionLayer = 3;

            Color = Color.White;

            CanFire = true;

            OnOuterCollision += ObjectGrazed;
            OnInnerCollision += ObjectCollidedWith;

            base.Initialize();
        }
        
        public static void LoadContent(ContentManager content)
        {
            if (playerShipTexture == null)
            {
                playerShipTexture = content.Load<Texture2D>("Graphics/playerShip");
                Console.WriteLine("Player texture loaded!");
            }

            if (hitboxTexture == null)
            {
                hitboxTexture = content.Load<Texture2D>("Graphics/hitbox");
            }
        }

        public override void Update(GameTime gameTime)
        {
            currentGameTime = (float)gameTime.TotalGameTime.TotalSeconds;

            //Console.Clear();
            //Console.WriteLine(string.Format("Main Hitbox: {0}, {1} ... Radius: {2}\nInner Hitbox: {3}, {4} ... Radius: {5}", Hitbox.Center.X, Hitbox.Center.Y, Hitbox.Radius, InnerHitbox.Center.X, InnerHitbox.Center.Y, InnerHitbox.Radius));

            Vector2 movement = new Vector2();
            // Update controls
            float moveSpeed;

            if (KeyboardManager.KeyDown(Keys.LeftShift))
            {
                moveSpeed = speed / 2f;
            }
            else
            {
                moveSpeed = speed;
            }

         
            if(KeyboardManager.KeyDown(Keys.Left))
            {
                movement.X = -1 * (moveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds);
            }
            if(KeyboardManager.KeyDown(Keys.Right))
            {
                movement.X = (moveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds);
            }
            if(KeyboardManager.KeyDown(Keys.Up))
            {
                movement.Y = -1 * (moveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds);
            }
            if(KeyboardManager.KeyDown(Keys.Down))
            {
                movement.Y = (moveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds);
            }

            if (KeyboardManager.KeyDown(Keys.Space) && gameTime.TotalGameTime.TotalSeconds > nextFireTime && CanFire)
            {
                // Fire!
                AudioManager.PlaySoundEffect(GameScene.Shot3Sound, .2f);
                Bullet newBullet = mainEmitter.FireBullet(((float)Math.PI / 2) * 3, 500f, Color.GreenYellow, BulletType.Diamond);
                newBullet.SetCollisionLayer(2);
                newBullet.DrawLayer = .1f;

                nextFireTime = (float)gameTime.TotalGameTime.TotalSeconds + .1f;
            }

            Position += movement;

            // Now clamp the position...

            if (Center.X - Origin.X < thisScene.ScreenArea.X)
                Center = new Vector2(thisScene.ScreenArea.X + Origin.X, Center.Y);
            else if (Center.X + Origin.X > thisScene.ScreenArea.X + thisScene.ScreenArea.Width)
                Center = new Vector2(thisScene.ScreenArea.X + thisScene.ScreenArea.Width - Origin.X, Center.Y);

            if (Center.Y - Origin.Y < thisScene.ScreenArea.Y)
                Center = new Vector2(Center.X, thisScene.ScreenArea.Y + Origin.Y);
            else if (Center.Y + Origin.Y > thisScene.ScreenArea.Y + thisScene.ScreenArea.Height)
                Center = new Vector2(Center.X, thisScene.ScreenArea.Y + thisScene.ScreenArea.Height - Origin.Y);

            //Test a hoolio
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Center, Texture.Bounds, Color, Rotation, Origin, 1f, SpriteEffects.None, DrawLayer);

            //HITBOX
            spriteBatch.Draw(hitboxTexture, new Vector2(InnerHitbox.Center.X - InnerHitbox.Radius, InnerHitbox.Center.Y - InnerHitbox.Radius), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, DrawLayer - .01f);
        }


        public void OuterCollision(GameObject sender, CollisionEventArgs e)
        {
            if (OnOuterCollision != null)
                OnOuterCollision(sender, e);
        }

        public void InnerCollision(GameObject sender, CollisionEventArgs e)
        {
            if (OnInnerCollision != null)
                OnInnerCollision(sender, e);
        }


        public void ObjectGrazed(GameObject sender, CollisionEventArgs e)
        {
            // If the object grazed was a bullet, graze it and report the graze.
            if (sender is Bullet)
            {
                Bullet thisBullet = (Bullet)sender;
                if (!thisBullet.Grazed)
                {
                    thisBullet.Grazed = true;
                    thisScene.GrazeCount++;
                    thisScene.Score += thisScene.GrazeValue;

                    thisScene.GainExperience(1f);
                    thisScene.PauseExperienceDecay(1f);
                }
                return;
            }
            
        }

        public void ObjectCollidedWith(GameObject sender, CollisionEventArgs e)
        {
            if (!Phasing && (sender is Bullet || sender is Enemy || sender is Boss))
            {
                Phasing = true;
                thisScene.PlayAnimation(new Animation(GameScene.PlayerExplosionTexture, new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }, 48, 20f, new Vector2(Center.X - 24, Center.Y - 25), false));
                AudioManager.PlaySoundEffect(GameScene.Explosion3Sound, .8f);

                thisScene.scriptManager.Execute(thisScene.TryRespawn);
                
                Destroy();
            }
        }
    }
}
