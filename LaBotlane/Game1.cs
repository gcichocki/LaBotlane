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

namespace Labotlane
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class LaBotlane : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager m_graphics;
        SpriteBatch m_spriteBatch;
        Scenes.Scene m_scene;
        public static LaBotlane Instance;
        

        /// <summary>
        /// Obtient une r�f�rence vers une collection de ressources communes � tout le jeu.
        /// </summary>
        public GameComponents.Ressources Ressources
        {
            get;
            private set;
        }
        /// <summary>
        /// Retourne une r�f�rence vers la Sc�ne.
        /// </summary>
        public Scenes.Scene Scene
        {
            get { return m_scene; }
        }
        /// <summary>
        /// Cr�e une nouvelle instance du jeu.
        /// </summary>
        public LaBotlane()
        {
            m_graphics = new GraphicsDeviceManager(this);
            m_graphics.PreferredBackBufferWidth = Scenes.Scene.ResolutionWidth;
            m_graphics.PreferredBackBufferHeight = Scenes.Scene.ResolutionHeight;
            GameComponents.GameMap.ScreenWidth = Scenes.Scene.ResolutionWidth / GameComponents.GameMap.TileSize + 2;
            GameComponents.GameMap.ScreenHeight = Scenes.Scene.ResolutionHeight / GameComponents.GameMap.TileSize + 2;

            m_graphics.IsFullScreen = false;
            Input.ModuleInit();
            Content.RootDirectory = "Content";
            Instance = this;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            m_spriteBatch = new SpriteBatch(GraphicsDevice);
            Ressources = new GameComponents.Ressources();
            m_scene = new Scenes.Scene();
            m_scene.Initialize();
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // Update de l'input
            Input.Update();

            // Update de la sc�ne
            m_scene.Update(gameTime);

            // TODO: Add your update logic here
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            m_scene.Draw(m_spriteBatch, gameTime);
            base.Draw(gameTime);
        }
    }
}
