using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
        }
        
        public override void Initialize()
        {
            if(Hitbox == null)
                Hitbox = new Circle(Center, 15f);

            if(scriptManager == null)
                scriptManager = thisScene.scriptManager;

            if (CollidesWithLayers == null)
                CollidesWithLayers = new int[0];

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

        public void SetTexture(Texture2D newTexture)
        {
            Texture = newTexture;
        }
    }

    public enum EnemyType
    {
    }
}
