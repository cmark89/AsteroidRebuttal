using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using AsteroidRebuttal;
using AsteroidRebuttal.GameObjects;
using AsteroidRebuttal.Core;
using AsteroidRebuttal.Scenes;
using AsteroidRebuttal.Scripting;

namespace AsteroidRebuttal.Enemies
{
    public class Enemy : GameObject, ICollidable
    {
        protected List<Bullet> bullets;

        public float Health { get; protected set; }

        public List<GameObject> CollidedObjects { get; set; }
        public int[] CollidesWithLayers { get; set; }
        public event CollisionEventHandler OnOuterCollision;
        public event CollisionEventHandler OnInnerCollision;

        protected ScriptManager scriptManager;

        public Enemy(GameScene newScene, Vector2 position = new Vector2())
        {
            thisScene = newScene;
            Center = position;

            Initialize();
        }

        public override void Initialize()
        {
            Hitbox = new Circle(Center, 15f);
            CollisionLayer = 1;
            scriptManager = thisScene.scriptManager;
            CollidedObjects = new List<GameObject>();

            if (DeletionBoundary == null)
                DeletionBoundary = new Vector2(Hitbox.Radius + 20, Hitbox.Radius + 20);

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            CollidedObjects.Clear();
            base.Update(gameTime);
        }

        //public override void Draw()
        //{
        //}

        public void OuterCollision(GameObject sender, CollisionEventArgs e)
        {
            if (OnOuterCollision != null)
                OnOuterCollision(sender, e);
        }
        public void InnerCollision(GameObject sender, CollisionEventArgs e)
        {
            if (OnOuterCollision != null)
                OnOuterCollision(sender, e);
        }
    }

    public enum EnemyType
    {
    }
}
