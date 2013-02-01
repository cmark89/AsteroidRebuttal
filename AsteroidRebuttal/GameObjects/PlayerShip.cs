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

namespace AsteroidRebuttal.GameObjects
{
    public class PlayerShip : GameObject
    {
        static Texture2D playerShipTexture;
        public static Texture2D hitboxTexture;

        public ScriptManager scriptManager;
        List<Bullet> bullets;

        float speed = 250;

        public PlayerShip(GameScene newScene = null, Vector2 position = new Vector2())
        {
            thisScene = newScene;
            Position = position;

            Initialize();
        }

        public override void Initialize()
        {
            Console.WriteLine("Player initialized!");
            Texture = playerShipTexture;
            scriptManager = new ScriptManager();
            bullets = new List<Bullet>();

            Origin = new Vector2(23.5f, 23.5f);
            Hitbox = new Circle(Center, 17.5f);
            UsesInnerHitbox = true;
            InnerHitbox = new Circle(new Vector2(23.5f, 19.5f), 4f);

            scriptManager.Execute(FireBullet, true);

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
            scriptManager.Update(gameTime);

            Console.Clear();

            Console.WriteLine(string.Format("Main Hitbox: {0}, {1} ... Radius: {2}\nInner Hitbox: {3}, {4} ... Radius: {5}", Hitbox.Center.X, Hitbox.Center.Y, Hitbox.Radius, InnerHitbox.Center.X, InnerHitbox.Center.Y, InnerHitbox.Radius));

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

            Position += movement;

            //Test a hoolio
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Center, Texture.Bounds, Color.White, Rotation, Origin, 1f, SpriteEffects.None, 0f);

            //HITBOX
            spriteBatch.Draw(hitboxTexture, new Rectangle((int)(Hitbox.Center.X - Hitbox.Radius), (int)(Hitbox.Center.Y - Hitbox.Radius), (int)(Hitbox.Radius) * 2, (int)(Hitbox.Radius*2)), new Color(.3f, .7f, .5f, .5f));
            spriteBatch.Draw(hitboxTexture, new Vector2(InnerHitbox.Center.X - InnerHitbox.Radius, InnerHitbox.Center.Y - InnerHitbox.Radius), Color.White);
            spriteBatch.Draw(QuadTree.texture, new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height), Color.White);
        }

        // THESE ARE ALL FOR TESTING BULLET SCRIPTING
        public IEnumerator<float> FireBullet()
        {
            BulletEmitter emitter = new BulletEmitter(this, Position);
            ChildObjects.Add(emitter);
            emitter.AngularVelocity = 2f;
            int times = 0;

            while(true)
            {
                while (times < 35)
                {
                    int numBullets;
                    float spread;

                    Bullet b = emitter.FireBullet(VectorMathHelper.GetAngleTo(emitter.Center, this.Center, .3f), 200f, Color.White);
                    bullets.Add(b);
                    scriptManager.Execute(RandomCoolChargeBullet, b);
                    
                    times++;
                    yield return .03f;
                }

                yield return 5f;
                emitter.AngularVelocity *= -1;
                times = 0;
            }
        }

        public IEnumerator<float> BulletsWackyCurve(GameObject go)
        {
            Bullet thisBullet = (Bullet)go;
            float angleMultiplier = 1;

            while (true)
            {
                if (thisBullet.Rotation < 6.5)
                {
                    angleMultiplier = 1;
                }
                else if(thisBullet.Rotation > 7.5)
                {
                    angleMultiplier = -1;
                }

                thisBullet.Rotation += .1f * angleMultiplier;
                yield return 0;
            }
        }

        public IEnumerator<float> RandomChargeBullet(GameObject go)
        {
            Bullet thisBullet = (Bullet)go;
            Random rand = new Random();

            yield return .8f;
            thisBullet.Rotation = (float)rand.NextDouble() * 5f;
        }

        public IEnumerator<float> RandomCoolChargeBullet(GameObject go)
        {
            Bullet thisBullet = (Bullet)go;
            Random rand = new Random();

            thisBullet.LerpVelocity(100f, 1.5f);
            yield return 2f;

            foreach (Bullet b in (new BulletEmitter(this, thisBullet.Position, true).BulletExplosion(22, 150f, Color.Lerp(Color.White, Color.Magenta, 0.2f))))
            {
                bullets.Add(b);
                scriptManager.Execute(SwitchDirections, b);
            }

            thisBullet.Destroy();
        }

        public IEnumerator<float> SwitchDirections(GameObject go)
        {
            Bullet thisBullet = (Bullet)go;
            thisBullet.LerpVelocity(0f, 2f);
            yield return 2f;

            thisBullet.Rotation += (float)Math.PI;
            thisBullet.LerpVelocity(300f, 3f);
        }
    }
}
