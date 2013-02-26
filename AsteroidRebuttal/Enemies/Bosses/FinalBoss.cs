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

namespace AsteroidRebuttal.Enemies.Bosses
{
    public class FinalBoss : Boss
    {
        BulletEmitter mainEmitter;
        List<BulletEmitter> ring1Emitters;
        List<BulletEmitter> ring2Emitters;

        Enemy ring1;
        Enemy ring2;
        Enemy topLayer;

        int phase = 1;

        float nextSpreadShotTime = 0f;

        public FinalBoss(GameScene newScene, Vector2 newPos = new Vector2()) : base(newScene, newPos)
        {
            thisScene = newScene;
            Center = newPos;

            Initialize();
        }

        public override void Initialize()
        {
            Console.WriteLine("Initialize me!");
            Health = 2050;

            // Get the actual origin.
            Origin = new Vector2(255.5f, 234.5f);
            Hitbox = new Circle(Center, 140f);
            Texture = boss5BaseTexture;
            DeletionBoundary = new Vector2(1500, 1500);

            Color = Color.White;

            CollisionLayer = 1;
            CollidesWithLayers = new int[] { 2, 3 };

            InitializeParts();

            scriptManager = thisScene.scriptManager;
            scriptManager.Execute(Phase1Script, this);

            OnOuterCollision += CollisionHandling;

            base.Initialize();
        }

        public void InitializeParts()
        {
            mainEmitter = new BulletEmitter(this, Origin, false);
            mainEmitter.LockedToParentPosition = true;
            mainEmitter.LockPositionOffset = Vector2.Zero;

            ring1 = new Enemy(thisScene, Origin);
            ring1.Initialize();
            ring1.DeletionBoundary = DeletionBoundary;
            ring1.DrawAtTrueRotation = true;
            ring1.Color = Color.White;
            ring1.Origin = Origin;
            ring1.SetParent(this);
            ring1.SetTexture(boss5Ring1Texture);
            ring1.LockedToParentPosition = true;

            ring2 = new Enemy(thisScene, Origin);
            ring2.Initialize();
            ring2.DeletionBoundary = DeletionBoundary;
            ring2.DrawAtTrueRotation = true;
            ring2.Color = Color.White;
            ring2.Origin = Origin;
            ring2.SetParent(this);
            ring2.SetTexture(boss5Ring2Texture);
            ring2.LockedToParentPosition = true;

            ring1Emitters = new List<BulletEmitter>()
            {
                new BulletEmitter(ring1, new Vector2(130, 175)),
                new BulletEmitter(ring1, new Vector2(130, 294)),
                new BulletEmitter(ring1, new Vector2(381, 175)),
                new BulletEmitter(ring1, new Vector2(381, 294))
            };

            ring2Emitters = new List<BulletEmitter>()
            {
                new BulletEmitter(ring2, new Vector2(195, 110)),
                new BulletEmitter(ring2, new Vector2(117, 234.5f)),
                new BulletEmitter(ring2, new Vector2(195, 359)),
                new BulletEmitter(ring2, new Vector2(316, 110)),
                new BulletEmitter(ring2, new Vector2(394, 234.5f)),
                new BulletEmitter(ring2, new Vector2(316, 359))
            };

            // Setup the ring emitter properties.
            foreach (BulletEmitter be in ring1Emitters)
            {
                be.Center = Center + (be.Center - Origin);
                be.DeletionBoundary = new Vector2(99999, 99999);
                be.CustomValue1 = Vector2.Distance(Center, be.Center);
                be.Rotation = VectorMathHelper.GetAngleTo(Center, be.Center);
            }

            // Setup the ring emitter properties.
            foreach (BulletEmitter be in ring2Emitters)
            {
                be.Center = Center + (be.Center - Origin);
                be.DeletionBoundary = new Vector2(99999, 99999);
                be.CustomValue1 = Vector2.Distance(Center, be.Center);
                be.Rotation = VectorMathHelper.GetAngleTo(Center, be.Center);
            }
            
            topLayer = new Enemy(thisScene, Origin);
            topLayer.Initialize();
            topLayer.DeletionBoundary = DeletionBoundary;
            topLayer.Color = Color.White;
            topLayer.Origin = Origin;
            topLayer.SetTexture(boss5TopTexture);
            topLayer.SetParent(this);
            topLayer.LockedToParentPosition = true;
        }

        public override void Update(GameTime gameTime)
        {
            Console.Clear();
            Console.WriteLine("Health: " + Health + " / 2050");
            // Move the subemitters with the rotation of the rings, and ensure they are facing outwards.
            foreach (BulletEmitter be in ring1Emitters)
            {
                // Update rotation.
                be.Rotation += ring1.AngularVelocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

                Vector2 newPos = new Vector2();
                newPos.X = (float)Math.Cos(be.Rotation) * be.CustomValue1;
                newPos.Y = (float)Math.Sin(be.Rotation) * be.CustomValue1;

                be.Center = Center + newPos;
            }

            foreach (BulletEmitter be in ring2Emitters)
            {
                // Update rotation.
                be.Rotation += ring2.AngularVelocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

                Vector2 newPos = new Vector2();
                newPos.X = (float)Math.Cos(be.Rotation) * be.CustomValue1;
                newPos.Y = (float)Math.Sin(be.Rotation) * be.CustomValue1;

                be.Center = Center + newPos;
            }


            if (phase == 1 && Health < 1600)
            {
                phase = 2;
                scriptManager.AbortObjectScripts(this);
                scriptManager.Execute(Phase2Script, this);
            }

            if (phase == 2 && Health < 1350)
            {
                phase = 3;
                scriptManager.AbortObjectScripts(this);
                scriptManager.Execute(Phase3Script, this);
            }

            if (phase == 3 && Health < 1000)
            {
                phase = 4;
                scriptManager.AbortObjectScripts(this);
                //scriptManager.Execute(Phase4Script, this);
            }

            base.Update(gameTime);
        }

        public IEnumerator<float> Phase1Script (GameObject thisObject)
        {
            Console.WriteLine("MOVE FINAL BOSS");
            LerpPosition(new Vector2(350f, 220f), 10f);
            yield return 5f;

            int cycles = 0;
            while (cycles < 4)
            {
                float rotateSpeed = 0f;
                if (cycles % 2 == 0)
                    rotateSpeed = .1f;
                else
                    rotateSpeed = -.1f;

                mainEmitter.Rotation = 0f;
                foreach (Bullet b in mainEmitter.FireBulletExplosion(40, 120f, Color.DeepSkyBlue))
                {
                    b.LerpVelocity(90f, 1f);
                }

                yield return .4f;

                mainEmitter.Rotation += rotateSpeed;
                foreach (Bullet b in mainEmitter.FireBulletExplosion(40, 120f, Color.DeepSkyBlue))
                {
                    b.LerpVelocity(90f, 1f);
                }

                yield return .4f;

                mainEmitter.Rotation += rotateSpeed;
                foreach (Bullet b in mainEmitter.FireBulletExplosion(40, 120f, Color.DeepSkyBlue))
                {
                    b.LerpVelocity(90f, 1f);
                }

                yield return .4f;

                mainEmitter.Rotation += rotateSpeed;
                foreach (Bullet b in mainEmitter.FireBulletExplosion(40, 120f, Color.DeepSkyBlue))
                {
                    b.LerpVelocity(90f, 1f);
                }

                yield return .4f;

                mainEmitter.Rotation += rotateSpeed;
                foreach (Bullet b in mainEmitter.FireBulletExplosion(40, 120f, Color.DeepSkyBlue))
                {
                    b.LerpVelocity(90f, 1f);
                }

                cycles++;
                yield return 4f;
            }

            LerpPosition(new Vector2(350f, 125f), 10f);
            yield return 1f;

            ring1.LerpAngularVelocity(.3f, 3f);
            yield return 1f;
            ring2.LerpAngularVelocity(-.45f, 3f);
            yield return 2f;

            int moveCount = 0;
            Vector2[] movePositions = new Vector2[]
            {
                new Vector2(200, 125f),
                new Vector2(350f, 125f),
                new Vector2(500f, 125f),
                new Vector2(350f, 125f)
            };

            // Cycle the power!
            while (true)
            {
                
                float nextMoveTime = currentGameTime + 7f;

                while (currentGameTime < nextMoveTime)
                {
                    foreach (BulletEmitter be in ring1Emitters)
                    {
                        be.FireBullet(180, Color.Red);
                    }
                    foreach (BulletEmitter be in ring2Emitters)
                    {
                        be.FireBullet(140, Color.DeepSkyBlue);
                    }
                    yield return .2f;
                }

                LerpPosition(movePositions[moveCount], 2f);
                scriptManager.Execute(Phase1MoveVolley, this);
                moveCount++;

                if (moveCount >= movePositions.Length)
                    moveCount = 0;
            }
        }

        public IEnumerator<float> Phase1MoveVolley(GameObject go)
        {
            mainEmitter.Rotation = 0;
            mainEmitter.FireBulletExplosion(40, 160, Color.Lerp(Color.White, Color.Orange, .4f));
            yield return .2f;

            mainEmitter.Rotation += .2f;
            mainEmitter.FireBulletExplosion(40, 160, Color.Lerp(Color.White, Color.Orange, .4f));
            yield return .2f;

            mainEmitter.Rotation += .2f;
            mainEmitter.FireBulletExplosion(40, 160, Color.Lerp(Color.White, Color.Orange, .4f));
            yield return .2f;
        }

        public IEnumerator<float> Phase2Script(GameObject go)
        {
            LerpPosition(new Vector2(350f, 220f), 5f);
            ring1.LerpAngularVelocity(0, 3f);
            ring2.LerpAngularVelocity(0, 3f);
            yield return 3f;
            ring1.LerpAngularVelocity(-1f, 3f);
            ring2.LerpAngularVelocity(1f, 3f);
            yield return 3f;

            // Do the cool pattern!!
            int cycles = 0;
            while (true)
            {
                int shots = 0;

                while (shots < 40)
                {
                    foreach (BulletEmitter be in ring1Emitters)
                    {
                        Bullet b = be.FireBullet(20f, Color.Red);
                        b.CustomValue1 = -250f;
                        scriptManager.Execute(Phase2BulletSwarm, b);
                    }

                    foreach (BulletEmitter be in ring2Emitters)
                    {
                        Bullet b = be.FireBullet(20f, Color.DeepSkyBlue);
                        b.CustomValue1 = 150f;
                        scriptManager.Execute(Phase2BulletSwarm, b);
                    }

                    shots++;
                    yield return .09f;
                }

                yield return 4f;
                ring1.LerpAngularVelocity(ring1.AngularVelocity * -1f, 5f);
                ring2.LerpAngularVelocity(ring2.AngularVelocity * -1f, 5f);
                yield return 2.5f;
                
                // If this is the 5th time the cycle has repeated or more, fire radial bullets.
                if (cycles >= 2)
                    scriptManager.Execute(Phase1MoveVolley, this);

                cycles++;
                yield return 2.5f;
            }
        }

        public IEnumerator<float> Phase2BulletSwarm(GameObject go)
        {
            Bullet thisBullet = (Bullet)go;
            yield return 3f;

            thisBullet.LerpVelocity(thisBullet.CustomValue1, 3f);
        }

        public IEnumerator<float> Phase3Script(GameObject go)
        {
            LerpPosition(new Vector2(350f, 140f), 5f);
            ring1.LerpAngularVelocity(0, 3f);
            ring2.LerpAngularVelocity(0, 3f);
            yield return 3f;
            ring1.LerpAngularVelocity(-.35f, 3f);
            ring2.LerpAngularVelocity(-.2f, 3f);
            yield return 3f;


            nextSpreadShotTime = currentGameTime + 1f;
            // Execute the movement script.
            scriptManager.Execute(Phase3MovementScript, this);

            // Execute the ring 1 script
            scriptManager.Execute(Phase3Ring1Script, this);

            // Execute the ring 2 script.
            scriptManager.Execute(Phase3Ring2Script, this);

            while(true)
            {
                if (nextSpreadShotTime < currentGameTime)
                {
                    int shots = 0;
                    
                    // Fire a slow moving, rotating spread shot from the center emitter.
                    foreach (Bullet b in mainEmitter.FireBulletExplosion(40, 50f, Color.Lerp(Color.White, Color.Orange, .4f)))
                    {
                        if (shots % 2 == 0)
                            b.AngularVelocity = .2f;
                        else
                            b.AngularVelocity = -.2f;
                        
                        b.LerpVelocity(100f, 3f);
                    }

                    shots++;
                    nextSpreadShotTime += 1.5f;
                }

                yield return .1f;
            }

        }

        public IEnumerator<float> Phase3Ring1Script(GameObject go)
        {
            int inrangeof = 0;
            float angleToPlayer;
            float thisRotation;

            while (true)
            {
                inrangeof = 0;

                foreach (BulletEmitter be in ring1Emitters)
                {
                    if (currentGameTime < be.CustomValue2)
                    {
                        continue;
                    }

                    angleToPlayer = VectorMathHelper.GetAngleTo(Center, thisScene.player.InnerHitbox.Center);
                    thisRotation = VectorMathHelper.GetAngleTo(Center, be.Center);

                    if (thisRotation < (angleToPlayer + .1f) && thisRotation > (angleToPlayer - .1f))
                    {
                        be.FireBulletCluster(be.Rotation, 10, 20f, 100f, 200f, Color.Red);

                        // Stop it from firing for .2 seconds
                        be.CustomValue2 = currentGameTime + .2f;
                        inrangeof++;
                    }
                }

                if (inrangeof > 0)
                {
                    // The player is in range of at least one cannon, so delay the next spreading shot from the center by .7 seconds.
                    nextSpreadShotTime += .7f;
                }

                yield return .03f;
            }
        }

        public IEnumerator<float> Phase3Ring2Script(GameObject go)
        {
            while (true)
            {
                foreach (BulletEmitter be in ring2Emitters)
                {
                    float angleToPlayer = VectorMathHelper.GetAngleTo(Center, thisScene.player.InnerHitbox.Center);
                    float thisRotation = VectorMathHelper.GetAngleTo(Center, be.Center);

                    if (thisRotation < (angleToPlayer + .1f) && thisRotation > (angleToPlayer - .1f))
                    {
                        // Fire a bomb that will explode after a few moments.
                        Bullet bomb = be.FireBullet(140f, Color.Orange);
                        bomb.LerpVelocity(0f, 2f);
                        scriptManager.Execute(BombExplosion, bomb);
                        
                        // Delay the spread.
                        nextSpreadShotTime += 1.5f;

                    }
                    else
                    {
                        foreach (Bullet b in be.FireBulletSpread(be.Rotation, 4, 25f, 130f, Color.DeepSkyBlue))
                        {
                            b.LerpVelocity(60f, 3f);
                        }
                    }
                }

                yield return 1.2f;
            }
        }

        public IEnumerator<float> BombExplosion(GameObject go)
        {
            yield return 2f;
            BulletEmitter explosion = new BulletEmitter(this, go.Center, true);
            explosion.FireBulletExplosion(20, 110f, Color.Lerp(Color.White, Color.Orange, .7f));
            explosion.FireBulletExplosion(30, 130f, Color.Lerp(Color.White, Color.Orange, .7f), BulletType.CircleSmall);
            go.Destroy();
        }

        public IEnumerator<float> Phase3MovementScript(GameObject go)
        {
            while (true)
            {
                yield return 6f;
                LerpPosition(new Vector2(100f, 140f), 5f);
                yield return 10f;
                LerpPosition(new Vector2(350f, 140f), 5f);
                yield return 10f;
                LerpPosition(new Vector2(500f, 140f), 5f);
                yield return 10f;
                LerpPosition(new Vector2(350f, 140f), 5f);
                yield return 10f;

                LerpPosition(new Vector2(500f, 400f), 15f);
                yield return 20f;
                LerpPosition(new Vector2(100f, 400f), 15f);
                yield return 20f;
                LerpPosition(new Vector2(350f, 140f), 15f);
                
                yield return 10f;
                ring1.LerpAngularVelocity(ring1.AngularVelocity * -1, 10f);
                ring2.LerpAngularVelocity(ring2.AngularVelocity * -1, 10f);
                yield return 10f;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            ring1.Draw(spriteBatch);
            ring2.Draw(spriteBatch);
            topLayer.Draw(spriteBatch);
        }


        public void CollisionHandling(GameObject sender, CollisionEventArgs e)
        {
            if (CollidedObjects.Contains(sender))
                return;
            else
                CollidedObjects.Add(sender);

            // If collision occured with a player bullet...
            if (e.collisionLayer == 2)
            {
                sender.Destroy();
                // Flash the thing here.
                Health--;
            }

            // Collided with player; kill!
            if (e.collisionLayer == 3)
            {
                PlayerShip thisShip = (PlayerShip)e.otherObject;
                thisShip.ObjectCollidedWith(this, new CollisionEventArgs(this, 1));
            }
        }
    }
}
