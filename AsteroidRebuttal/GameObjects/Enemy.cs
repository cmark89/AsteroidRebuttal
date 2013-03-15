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
using Microsoft.Xna.Framework.Content;

namespace AsteroidRebuttal.Enemies
{
    public class Enemy : GameObject, ICollidable
    {
        protected List<Bullet> bullets;

        public float Health { get; set; }

        public List<GameObject> CollidedObjects { get; set; }
        public int[] CollidesWithLayers { get; set; }
        public event CollisionEventHandler OnOuterCollision;
        public event CollisionEventHandler OnInnerCollision;

        protected ScriptManager scriptManager;

        protected static Texture2D slicerTexture;
        protected static Texture2D tortoiseTexture;

        public Enemy(GameScene newScene, Vector2 position = new Vector2())
        {
            thisScene = newScene;
            Center = position;

            Initialize();
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

            Rotation = (float)Math.PI / 2;

            base.Initialize();
        }

        public static void LoadContent(ContentManager content)
        {
            if (slicerTexture == null)
            {
                slicerTexture = content.Load<Texture2D>("Graphics/Ships/Enemy1");
            }
            if (tortoiseTexture == null)
            {
                tortoiseTexture = content.Load<Texture2D>("Graphics/Ships/Enemy2");
            }
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

        // Put explosion here
        public void CheckForDeath()
        {
            if (Health < 1)
            {
                // Explosion
                Destroy();
            }
        }
    }

    public enum EnemyType
    {
        Slicer,
        Tortoise,
        Dragon,
        Komodo
    }
}
