using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using AsteroidRebuttal;
using AsteroidRebuttal.Core;
using AsteroidRebuttal.GameObjects;
using AsteroidRebuttal.Scripting;
using AsteroidRebuttal.Scenes;
using ObjectivelyRadical;
using ObjectivelyRadical.Controls;



namespace AsteroidRebuttal
{
    public class AsteroidRebuttal : Microsoft.Xna.Framework.Game
    {
        public static GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // The current scene of the game.
        public Scene CurrentScene { get; private set; }

        public AsteroidRebuttal()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

       
        protected override void Initialize()
        {
            base.Initialize();
            ChangeScene(new GameScene());
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            PlayerShip.LoadContent(Content);
            Bullet.LoadContent(Content);
            QuadTree.LoadContent(Content);
        }


        protected override void UnloadContent()
        {
            //
        }

        
        protected override void Update(GameTime gameTime)
        {
            KeyboardManager.Update(gameTime);

            if (CurrentScene != null)
                CurrentScene.Update(gameTime);

            base.Update(gameTime);
        }

        
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            if (CurrentScene != null)
                CurrentScene.Draw(spriteBatch);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        public void ChangeScene(Scene newScene)
        {
            if(CurrentScene != null)
                CurrentScene.Unload();

            CurrentScene = newScene;
            CurrentScene.Initialize();
            CurrentScene.LoadContent(Content);
        }
    }
}
