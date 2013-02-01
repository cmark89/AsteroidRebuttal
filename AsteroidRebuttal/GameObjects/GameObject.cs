using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using AsteroidRebuttal.Scenes;
using AsteroidRebuttal.Core;

namespace AsteroidRebuttal.GameObjects
{
    // The basic game object class from which all objects inherit
    public abstract class GameObject
    {

        #region Core Fields
        // The texture of the game object
        public Texture2D Texture { get; protected set; }
        public GameScene thisScene;
        public List<GameObject> ChildObjects { get; protected set; }
        #endregion

        #region Position, Hitboxes, Collision

        // The position of the game object in world space
        private Vector2 _position;
        public virtual Vector2 Position 
        {
            get { return _position; }

            protected set
            {
                _position = value;
                if (Hitbox != null)
                {
                    Hitbox.Center = Center;
                }
                if (InnerHitbox != null)
                {
                    InnerHitbox.Center = Position + _innerHitboxOffset;
                }
            }
        }

        public Vector2 Origin { get; protected set; }

        public Vector2 Center
        {
            get { return Position + Origin; }
        }

        public Circle Hitbox { get; protected set; }

        public bool UsesInnerHitbox { get; protected set; }
        protected Vector2 _innerHitboxOffset;
        private Circle _innerHitbox;
        public Circle InnerHitbox 
        {
            get
            {
                return _innerHitbox;
            }
            protected set
            {
                _innerHitbox = value;
                _innerHitboxOffset = value.Center;
            }
        }

        // Stores the vector of movement
        public Vector2 MovementVector { get; protected set; }

        // Phasing prevents collision occuring with this object.
        public bool Phasing;

        // This is the layer on which this object will send collision.
        public int CollisionLayer;

        #endregion

        #region Game Physics
        // Angle of rotation in radians
        public float Rotation { get; set; }

        // How fast the object is moving
        public float Velocity { get; set; }
        // How fast the object is spinning
        public float AngularVelocity { get; set; }
        // How much Velocity is changing over time
        public float Acceleration { get; set; }
        // How much AngularVelocity is changing over time
        public float AngularAcceleration { get; set; }
        #endregion

        #region Private Lerping Properties
        // I know there's a better way to do this, but for now this will suffice
        bool lerpingPosition;
        float positionLerpElapsedTime;
        float positionLerpDuration;
        Vector2 startPosition;
        Vector2 targetPosition;

        bool lerpingRotation;
        float rotationLerpElapsedTime;
        float rotationLerpDuration;
        float startRotation;
        float targetRotation;

        bool lerpingVelocity;
        float velocityLerpElapsedTime;
        float velocityLerpDuration;
        float startVelocity;
        float targetVelocity;

        bool lerpingAngularVelocity;
        float angularVelocityLerpElapsedTime;
        float angularVelocityLerpDuration;
        float startAngularVelocity;
        float targetAngularVelocity;

        #endregion

        #region Management
        // Fields for Add/Remove management
        public bool FlaggedForRemoval { get; set; }
        public bool IsNewObject = true;
        #endregion


        public virtual void Initialize()
        {
            ChildObjects = new List<GameObject>();

            // If an outerhitbox does not yet exist, set it equal to the "true" hitbox.
            if (UsesInnerHitbox && InnerHitbox == null)
            {
                InnerHitbox = Hitbox;
            }

            if (thisScene != null)
                thisScene.AddGameObject(this);
        }

        public virtual void Update(GameTime gameTime)
        {
            // If acceleration exists, modify velocity by it
            if(Acceleration != 0)
            {
                Velocity += Acceleration * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            // If angular velocity is not 0, change the angular velocity of the object
            if (AngularAcceleration != 0)
            {
                AngularVelocity += AngularAcceleration * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            
            // If angular velocity is not 0, change the rotation of the object
            if (AngularVelocity != 0)
            {
                Rotation += AngularVelocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            // Move the object forward by its velocity if it is not equal to 0
            if (Velocity != 0)
            {
                // Create a new vector to store movement
                Vector2 movement = new Vector2();

                movement.X = (float)Math.Cos(Rotation) * (Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds);
                movement.Y = (float)Math.Sin(Rotation) * (Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds);

                // Actually move the object
                MovementVector = movement;
                Position += MovementVector;
            }

            if (lerpingPosition || lerpingRotation || lerpingVelocity || lerpingAngularVelocity)
                LerpUpdate(gameTime);
        }

        // Lerp values that are changing over time
        public void LerpUpdate(GameTime gameTime)
        {
            if (lerpingPosition)
            {
                positionLerpElapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                Vector2 newPos = new Vector2();
                newPos.X = MathHelper.Lerp(startPosition.X, targetPosition.X, positionLerpElapsedTime / positionLerpDuration);
                newPos.Y = MathHelper.Lerp(startPosition.Y, targetPosition.Y, positionLerpElapsedTime / positionLerpDuration);

                if (positionLerpElapsedTime >= positionLerpDuration)
                    lerpingPosition = false;
            }

            if (lerpingRotation)
            {
                rotationLerpElapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                Rotation = MathHelper.Lerp(startRotation, targetRotation, rotationLerpElapsedTime / rotationLerpDuration);
                

                if (rotationLerpElapsedTime >= rotationLerpDuration)
                    lerpingRotation = false;
            }

            if (lerpingVelocity)
            {
                velocityLerpElapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                Velocity = MathHelper.Lerp(startVelocity, targetVelocity, velocityLerpElapsedTime / velocityLerpDuration);


                if (velocityLerpElapsedTime >= velocityLerpDuration)
                    lerpingVelocity = false;
            }

            if (lerpingAngularVelocity)
            {
                angularVelocityLerpElapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                AngularVelocity = MathHelper.Lerp(startAngularVelocity, targetAngularVelocity, angularVelocityLerpElapsedTime / angularVelocityLerpDuration);


                if (angularVelocityLerpElapsedTime >= angularVelocityLerpDuration)
                    lerpingAngularVelocity = false;
            }
        }


        public virtual void Draw(SpriteBatch spriteBatch)
        {
            // Take into account the rotation...
            spriteBatch.Draw(Texture, Position, Color.White);
        }

        public void LerpPosition(Vector2 newPosition, float duration)
        {
            lerpingPosition = true;
            startPosition = Position;
            targetPosition = newPosition;
            positionLerpDuration = duration;
            positionLerpElapsedTime = 0f;
        }

        public void LerpRotation(float newRotation, float duration)
        {
            lerpingRotation = true;
            startRotation = Rotation;
            targetRotation = newRotation;
            rotationLerpDuration = duration;
            rotationLerpElapsedTime = 0f;
        }

        public void LerpVelocity(float newVelocity, float duration)
        {
            lerpingVelocity = true;
            startVelocity = Velocity;
            targetVelocity = newVelocity;
            velocityLerpDuration = duration;
            velocityLerpElapsedTime = 0f;
        }

        public void LerpAngularVelocity(float newAngularVelocity, float duration)
        {
            lerpingAngularVelocity = true;
            startAngularVelocity = AngularVelocity;
            targetAngularVelocity = newAngularVelocity;
            angularVelocityLerpDuration = duration;
            angularVelocityLerpElapsedTime = 0f;
        }

        //public void LerpAcceleration(float newAcceleration, float duration)
        //{
        //}

        //public void LerpAngularAcceleration(float newAngularAcceleration, float duration)
        //{
        //}

        // Lerp color
        public void SetCollisionLayer(int newLayer)
        {
            CollisionLayer = newLayer;
        }


        public virtual void Destroy()
        {
            FlaggedForRemoval = true;
            foreach (GameObject go in ChildObjects)
                go.Destroy();
        }
    }
}
