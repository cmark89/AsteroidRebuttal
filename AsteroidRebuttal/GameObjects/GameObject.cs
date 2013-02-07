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
        // The parent object, if it exists
        public GameObject Parent { get; set; }

        // The position of the game object in world space
        private Vector2 _position;
        public virtual Vector2 Position 
        {
            get { return _position; }

            set
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
            set
            {
                Position = value - Origin;
            }
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

        // Stores the object's position on the last frame.
        public Vector2 LastPosition { get; protected set; }

        // Stores the vector of movement
        public Vector2 MovementVector { get; protected set; }

        // Phasing prevents collision occuring with this object.
        public bool Phasing;

        // This is the layer on which this object will send collision.
        public int CollisionLayer;

        public bool LockedToParentPosition { get; set; }
        public Vector2 LockPositionOffset { get; set; }
        public bool LockedToParentRotation { get; set; }

        public Vector2 DeletionBoundary { get; protected set; }

        #endregion

        #region Game Physics
        // Angle of rotation in radians
        private float _rotation;
        public float Rotation 
        {
            get
            {
                return _rotation;
            }
            set
            {
                _rotation = value;
                if (DrawAtTrueRotation)
                    DrawRotation = value;
            }
        }

        // Angle of the object's draw rotation.
        public virtual float DrawRotation { get; set; }

        // If this is true, DrawRotation will be updated to match Rotation.  Otherwise it must be set separately.
        public bool DrawAtTrueRotation { get; set; }

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

        #region Miscellaneous
        public float CustomValue1 { get; set; }
        public float CustomValue2 { get; set; }
        public float CustomValue3 { get; set; }
        public float CustomValue4 { get; set; }
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

            if (DeletionBoundary == null)
            {
                DeletionBoundary = new Vector2(Hitbox.Radius, Hitbox.Radius);
            }
        }


        public virtual void Update(GameTime gameTime)
        {
            // Check if object is off screen and remove it if it is.
            CheckIfOffScreen();

            // Update the last position no matter what.
            LastPosition = Position;

            if (Parent != null && LockedToParentRotation)
            {
                Console.WriteLine("LOCKED ROTATION SET");
                Rotation = Parent.Rotation;
            }
            else
            {
                // If angular acceleration is not 0, change the angular velocity of the object
                if (AngularAcceleration != 0)
                {
                    AngularVelocity += AngularAcceleration * (float)gameTime.ElapsedGameTime.TotalSeconds;
                }

                // If angular velocity is not 0, change the rotation of the object
                if (AngularVelocity != 0)
                {
                    Rotation += AngularVelocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
            }

            // If the object has a parent and its position is locked to it...
            if (Parent != null && LockedToParentPosition)
            {
                // Set the position to the parent.
                Center = Parent.Center + LockPositionOffset;
            }
            else
            {
                // If acceleration exists, modify velocity by it
                if (Acceleration != 0)
                {
                    Velocity += Acceleration * (float)gameTime.ElapsedGameTime.TotalSeconds;
                }

                // Move the object forward by its velocity if it is not equal to 0
                if (Velocity != 0)
                {
                    // Create a new vector to store movement
                    Vector2 movement = new Vector2();

                    movement.X = (float)Math.Cos(Rotation) * (Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds);
                    movement.Y = (float)Math.Sin(Rotation) * (Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds);

                    // Set the movement vector
                    MovementVector = movement;

                    // Actually move the object
                    Position += MovementVector;
                }
            }
            

            if (lerpingPosition || lerpingRotation || lerpingVelocity || lerpingAngularVelocity)
                LerpUpdate(gameTime);
        }


        private void CheckIfOffScreen()
        {
            // Check if object is off screen and remove it if it is.
            if (Hitbox.Center.X + Hitbox.Radius < thisScene.ScreenArea.X - DeletionBoundary.X ||
                Hitbox.Center.X - Hitbox.Radius > thisScene.ScreenArea.Width + DeletionBoundary.X ||
                Hitbox.Center.Y + Hitbox.Radius < thisScene.ScreenArea.Y - DeletionBoundary.Y ||
                Hitbox.Center.Y - Hitbox.Radius > thisScene.ScreenArea.Height + DeletionBoundary.Y)
            {
                Destroy();
            }
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

                Center = newPos;

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
            // Draw the sprite simply.
            
            //float drawnRotation;
            //if (DrawAtTrueRotation) 
                //drawnRotation = Rotation;
            //else 
                //drawnRotation = DrawRotation;

            spriteBatch.Draw(Texture, Position, Color.White);
        }

        public void LerpPosition(Vector2 newPosition, float duration)
        {
            lerpingPosition = true;
            startPosition = Center;
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
