﻿using System;
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
    public class TestBoss : Boss
    {
        public bool phase2;

        public TestBoss(GameScene newScene, Vector2 newPos = new Vector2()) : base(newScene, newPos)
        {
            thisScene = newScene;
            Center = newPos;

            Initialize();
        }

        public override void Initialize()
        {
            Health = 200;

            // Get the actual origin.
            Origin = new Vector2(23.5f, 23.5f);
            Hitbox = new Circle(Center, 15f);
            Texture = testBossTexture;

            DeletionBoundary = new Vector2(1500, 1500);
            Color = Color.White;

            CollisionLayer = 1;
            CollidesWithLayers = new int[] { 2 };

            scriptManager = thisScene.scriptManager;
            scriptManager.Execute(MainScriptPhase1, this);

            OnOuterCollision += CollisionHandling;

            // Test
            Health = 200;

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            Console.Clear();
            Console.WriteLine("Boss Health: " + Health + " / " + 200 + " ... " + ((float)Health / 200f) + "%");
            if(!phase2 && Health < 100)
            {
                phase2 = true;
                Console.WriteLine("GO TO BOSS PHASE 2!!");
                scriptManager.AbortObjectScripts(this);
                scriptManager.Execute(MainScriptPhase2, this);
            }

            if (phase2)
            {
                CustomValue1 += (((float)Math.PI * 2) / 2) * (float)gameTime.ElapsedGameTime.TotalSeconds * 1.5f;
                CustomValue2 -= (((float)Math.PI * 2) / 2) * (float)gameTime.ElapsedGameTime.TotalSeconds * 1.5f;
            }

            base.Update(gameTime);
        }

        // Main script
        public IEnumerator<float> MainScriptPhase1(GameObject thisShip)
        {
            BulletEmitter mainEmitter = new BulletEmitter(this, this.Center, false);
            mainEmitter.LockedToParentPosition = true;
            
            int bossPhase = 1;
            LerpPosition(new Vector2(300f, 25f), 2f);
            yield return 2f;

            bool alive = true;
            while (alive)
            {
                int cycles = 0;

                while (bossPhase == 1)
                {
                    int shots = 0;

                    while (shots < 15)
                    {
                        mainEmitter.FireBulletSpread(((float)Math.PI / 2), 7, 90f, 140f, Color.Lerp(Color.White, Color.Orange, .45f), BulletType.Circle);
                        shots++;

                        yield return .5f;
                        mainEmitter.FireBulletSpread(((float)Math.PI / 2), 6, 70f, 140f, Color.Lerp(Color.White, Color.Orange, .45f), BulletType.Circle);
                        shots++;
                        yield return .5f;
                    }

                    shots = 0;
                    LerpPosition(new Vector2(25, 25), 1.5f);
                    yield return 1.5f;

                    while (shots < 20)
                    {
                        mainEmitter.FireBulletSpread(((float)Math.PI / 2 * .35f) + (float)Math.Sin(currentGameTime * 3) / 4, 3, 25f, 220f, Color.Lerp(Color.White, Color.DeepSkyBlue, .45f), BulletType.Circle);
                        shots++;
                        yield return .15f;
                    }

                    shots = 0;
                    LerpPosition(new Vector2(600, 25), 2.5f);
                    yield return 2.5f;

                    while (shots < 20)
                    {
                        mainEmitter.FireBulletSpread(((float)Math.PI / 2 * 1.65f) + (float)Math.Sin(currentGameTime * 3) / 4, 3, 25f, 220f, Color.Lerp(Color.White, Color.DeepSkyBlue, .45f), BulletType.Circle);
                        shots++;
                        yield return .15f;
                    }

                    LerpPosition(new Vector2(300f, 25f), 1.5f);
                    yield return 2.2f;
                    cycles++;
                    if (cycles > 3)
                        bossPhase++;
                }

                BulletEmitter subEmitter1 = new BulletEmitter(this, this.Center, false);
                subEmitter1.LockedToParentPosition = true;
                subEmitter1.LockPositionOffset = new Vector2(-28f, -10f);
                BulletEmitter subEmitter2 = new BulletEmitter(this, this.Center, false);
                subEmitter2.LockedToParentPosition = true;
                subEmitter2.LockPositionOffset = new Vector2(28f, -10f);

                yield return 1.5f;
                scriptManager.Execute(Subemitter1Script, subEmitter1);
                scriptManager.Execute(Subemitter2Script, subEmitter2);
                scriptManager.Execute(SubemitterFirePhase2, subEmitter1);
                scriptManager.Execute(SubemitterFirePhase2, subEmitter2);

                yield return 1.5f;

                cycles = 0;
                while (bossPhase == 2)
                {
                    mainEmitter.FireBulletExplosion(15, 100f, Color.Lerp(Color.White, Color.DeepSkyBlue, .45f), BulletType.Circle);
                    yield return 1f;
                    LerpPosition(new Vector2(50, 300), 3f);
                    yield return 3f;
                    mainEmitter.FireBulletExplosion(15, 100f, Color.Lerp(Color.White, Color.DeepSkyBlue, .45f), BulletType.Circle);
                    yield return 1f;
                    LerpPosition(new Vector2(550, 300), 3f);
                    yield return 3f;
                    LerpPosition(new Vector2(300f, 150f), 2f);
                    mainEmitter.FireBulletExplosion(15, 100f, Color.Lerp(Color.White, Color.DeepSkyBlue, .45f), BulletType.Circle);
                    yield return 1f;
                    LerpPosition(new Vector2(300f, 25f), 4f);
                    yield return .5f;
                    mainEmitter.FireBulletExplosion(15, 150f, Color.Lerp(Color.White, Color.DeepSkyBlue, .45f), BulletType.Circle);
                    yield return .5f;
                    mainEmitter.FireBulletExplosion(13, 150f, Color.Lerp(Color.White, Color.DeepSkyBlue, .45f), BulletType.Circle);
                    yield return .5f;
                    mainEmitter.FireBulletExplosion(11, 150f, Color.Lerp(Color.White, Color.DeepSkyBlue, .45f), BulletType.Circle);
                    yield return .5f;
                    mainEmitter.FireBulletExplosion(15, 150f, Color.Lerp(Color.White, Color.DeepSkyBlue, .45f), BulletType.Circle);
                    yield return .5f;
                    mainEmitter.FireBulletExplosion(13, 150f, Color.Lerp(Color.White, Color.DeepSkyBlue, .45f), BulletType.Circle);
                    yield return .5f;
                    mainEmitter.FireBulletExplosion(11, 150f, Color.Lerp(Color.White, Color.DeepSkyBlue, .45f), BulletType.Circle);
                    yield return 1f;

                    cycles++;
                    if (cycles > 2)
                    {
                        bossPhase = 3;
                    }
                }

                cycles = 0;
                subEmitter1.Destroy();
                subEmitter2.Destroy();

                LerpPosition(new Vector2(300f, 25f), 2f);
                while (bossPhase == 3)
                {
                    int shots = 0;
                    while (shots < 25)
                    {
                        shots++;
                        mainEmitter.FireBulletSpread(VectorMathHelper.GetAngleTo(this.Hitbox.Center, thisScene.player.InnerHitbox.Center), 5, 80f, 200f, Color.Lerp(Color.White, Color.Orange, .4f), BulletType.Circle);
                        yield return .35f;
                    }

                    int i = 1;
                    foreach (Bullet b in mainEmitter.FireBulletWave(0f, 4, 200f, 50f, Color.Lerp(Color.White, Color.Orange, .3f)))
                    {
                        b.CustomValue1 = i++;
                        scriptManager.Execute(ClusterBombs, b);
                    }

                    i = 1;
                    foreach (Bullet b in mainEmitter.FireBulletWave((float)Math.PI, 4, 200f, 50f, Color.Lerp(Color.White, Color.Orange, .3f)))
                    {
                        b.CustomValue1 = i++;
                        scriptManager.Execute(ClusterBombs, b);
                    }

                    yield return 7f;
                    cycles++;

                    if (cycles > 3)
                    {
                        bossPhase = 1;
                    }
                }
            }
        }


        public IEnumerator<float> MainScriptPhase2(GameObject thisShip)
        {
            BulletEmitter mainEmitter1 = new BulletEmitter(this, this.Center, false);
            mainEmitter1.CustomValue1 = 1;       // Rotate this emitter counter clockwise
            mainEmitter1.CustomValue2 = 35;       // Rotate at this distance.
            BulletEmitter mainEmitter2 = new BulletEmitter(this, this.Center, false);
            mainEmitter2.CustomValue2 = 35f;        // This is the distance to orbit.

            scriptManager.Execute(RotatingEmitters, mainEmitter1);
            scriptManager.Execute(RotatingEmitters, mainEmitter2);

            

            while (true)
            {
                LerpPosition(new Vector2(335, 155), 2f);
                yield return 1.85f;
                CustomValue1 = (float)Math.PI / 2f * 3;
                CustomValue2 = CustomValue1;
                yield return .15f;

                int shots = 0;

                while (shots < 120)
                {
                    int subshots = 0;

                    while (subshots < 4)
                    {
                        mainEmitter1.FireBullet(VectorMathHelper.GetAngleTo(this.Center, mainEmitter1.Center), 140f, Color.Lerp(Color.White, Color.DeepSkyBlue, .6f));
                        mainEmitter2.FireBullet(VectorMathHelper.GetAngleTo(this.Center, mainEmitter2.Center), 140f, Color.Lerp(Color.White, Color.DeepSkyBlue, .6f));
                        subshots++;
                        shots++;
                        yield return .03f;
                    }
                    subshots = 0;
                    yield return .09f;
                }

                LerpPosition(new Vector2(335, 35), 1.5f);
                shots = 0;
                // Randomize the rotation
                CustomValue1 = (float)(new Random().NextDouble() * ((float)Math.PI*2));
                CustomValue2 = CustomValue1;

                while (shots < 10)
                {
                    
                    foreach (Bullet b in (mainEmitter1.FireBulletExplosion(15, 170f, Color.Lerp(Color.White, Color.Orange, .4f))))
                    {
                        b.LerpVelocity(90f, 3f);
                    }
                    foreach (Bullet b in (mainEmitter2.FireBulletExplosion(15, 190f, Color.Lerp(Color.White, Color.Red, .4f), BulletType.CircleSmall)))
                    {
                        b.LerpVelocity(90f, 3f);
                    }
                    shots++;
                    yield return .15f;
                }

                yield return 3f;
            }
        }

        public IEnumerator<float> RotatingEmitters(GameObject go)
        {
            BulletEmitter thisEmitter = (BulletEmitter) go;
            while(true)
            {
                float angle = 0;

                if (thisEmitter.CustomValue1 == 1)
                {
                    angle = thisEmitter.Parent.CustomValue2;
                }
                else
                {
                    angle = thisEmitter.Parent.CustomValue1; 
                }

                float x = (float)Math.Cos(angle) * thisEmitter.CustomValue2;
                float y = (float)Math.Sin(angle) * thisEmitter.CustomValue2;
                go.Position = go.Parent.Center + new Vector2(x, y);

                yield return 0f;
            }
        }

        public IEnumerator<float> Subemitter1Script(GameObject go)
        {
            BulletEmitter thisEmitter = (BulletEmitter)go;
            thisEmitter.Rotation = (float)Math.PI;
            while (thisEmitter != null)
            {
                thisEmitter.LerpRotation(((float)Math.PI / 2) * 1.5f, 5f);
                yield return 7f;
                thisEmitter.LerpRotation((float)Math.PI, 5f);
                yield return 7f;
            }
        }

        public IEnumerator<float> Subemitter2Script(GameObject go)
        {
            BulletEmitter thisEmitter = (BulletEmitter)go;

            thisEmitter.Rotation = 0f;

            while (thisEmitter != null)
            {
                thisEmitter.LerpRotation(((float)Math.PI / 2) * .5f, 5f);
                yield return 7f;
                thisEmitter.LerpRotation(0f, 5f);
                yield return 7f;
            }

            yield return 0f;
        }

        public IEnumerator<float> SubemitterFirePhase2(GameObject go)
        {
            BulletEmitter thisEmitter = (BulletEmitter)go;
            while (true)
            {
                thisEmitter.FireBullet(thisEmitter.Rotation, 350f, Color.Lerp(Color.White, Color.Orange, .7f), BulletType.DiamondSmall);
                yield return .03f;
            }
        }

        public IEnumerator<float> ClusterBombs(GameObject go)
        {
            Bullet thisBullet = (Bullet)go;
            thisBullet.LerpVelocity(0f, 3f);
            yield return 3f + (thisBullet.CustomValue1 * .6f);

            BulletEmitter explosion = new BulletEmitter(this, thisBullet.Center);
            explosion.FireBulletExplosion(8, 160f, Color.Lerp(Color.White, Color.DeepSkyBlue, .4f));
            explosion.FireBulletCluster(VectorMathHelper.GetAngleTo(thisBullet.Center, thisScene.player.InnerHitbox.Center), 5, 40f, 160f, 40f, Color.Lerp(Color.White, Color.DeepSkyBlue, .4f), BulletType.Circle);
            explosion.FireBulletCluster(VectorMathHelper.GetAngleTo(thisBullet.Center, thisScene.player.InnerHitbox.Center), 6, 50f, 140f, 50f, Color.Lerp(Color.White, Color.DeepSkyBlue, .4f), BulletType.CircleSmall);
            explosion.Destroy();

            thisBullet.Destroy();
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
        }
    }
}
