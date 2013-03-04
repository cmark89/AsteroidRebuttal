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
    public class ParasiteBoss : Boss
    {
        BulletEmitter mainEmitter;
        BulletEmitter arm1Emitter;
        BulletEmitter arm2Emitter;
        BulletEmitter arm3Emitter;
        BulletEmitter arm4Emitter;
        BulletEmitter arm5Emitter;
        BulletEmitter arm6Emitter;

        int phase = 1;
        Random rand = new Random();

        // Used for holding things
        Vector2[] arms;

        List<GameObject> eggs = new List<GameObject>();

        public ParasiteBoss(GameScene newScene, Vector2 newPos = new Vector2()) 
            : base(newScene, newPos)
        {
            thisScene = newScene;
            Center = newPos;

            Initialize();
        }


        public override void Initialize()
        {
            Console.WriteLine("Initialize me!");
            Health = 450;

            // Get the actual origin.
            Origin = new Vector2(127.5f, 61.5f);
            Hitbox = new Circle(Center, 25f);
            Texture = boss4Texture;
            DeletionBoundary = new Vector2(1500, 1500);

            Color = Color.White;

            CollisionLayer = 1;
            CollidesWithLayers = new int[] { 2 };

            InitializeParts();

            scriptManager = thisScene.scriptManager;
            scriptManager.Execute(Phase1Script, this);

            OnOuterCollision += CollisionHandling;


            base.Initialize();
        }


        public override void Update(GameTime gameTime)
        {
            //Console.WriteLine(Hitbox.Radius);
            base.Update(gameTime);
        }

        public void InitializeParts()
        {
            Console.WriteLine("INIT PARTS");
            arms = new Vector2[6]
            {
                new Vector2(31, 22),
                new Vector2(25, 65),
                new Vector2(55, 121),
                new Vector2(224, 22),
                new Vector2(231, 65),
                new Vector2(200, 121)
            };

            Hitbox = new Circle(Center, 25f);

            mainEmitter = new BulletEmitter(this, Center, false);
            mainEmitter.LockedToParentPosition = true;
            mainEmitter.LockPositionOffset = new Vector2(0f, 0f);

            arm1Emitter = new BulletEmitter(this, Center, false);
            arm1Emitter.LockedToParentPosition = true;
            arm1Emitter.LockPositionOffset = arms[0] - Origin;

            arm2Emitter = new BulletEmitter(this, Center, false);
            arm2Emitter.LockedToParentPosition = true;
            arm2Emitter.LockPositionOffset = arms[1] - Origin;

            arm3Emitter = new BulletEmitter(this, Center, false);
            arm3Emitter.LockedToParentPosition = true;
            arm3Emitter.LockPositionOffset = arms[2] - Origin;

            arm4Emitter = new BulletEmitter(this, Center, false);
            arm4Emitter.LockedToParentPosition = true;
            arm4Emitter.LockPositionOffset = arms[3] - Origin;

            arm5Emitter = new BulletEmitter(this, Center, false);
            arm5Emitter.LockedToParentPosition = true;
            arm5Emitter.LockPositionOffset = arms[4] - Origin;

            arm6Emitter = new BulletEmitter(this, Center, false);
            arm6Emitter.LockedToParentPosition = true;
            arm6Emitter.LockPositionOffset = arms[5] - Origin;
        }

        public IEnumerator<float> Phase1Script(GameObject thisShip)
        {
            LerpPosition(new Vector2(375, 50f), 4f);
            yield return .03f;
        }


        public void CollisionHandling(GameObject sender, CollisionEventArgs e)
        {
            if (CollidedObjects.Contains(sender))
            {
                return;
            }
            else
                CollidedObjects.Add(sender);

            // If collision occured with a player bullet...
            //if (e.collisionLayer == 2)
            //{
                //sender.Destroy();
                // Flash the thing here.
                //Health--;
            //}
        }
    }
}