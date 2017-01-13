using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
namespace Labotlane.GameComponents.Graphics
{
    /// <summary>
    /// Gestionnaire de graphisme.
    /// Contient des wrappers vers des méthodes de dessin.
    /// </summary>
    public class GraphicsEngine2D : GraphicsEngine, IDisposable
    {
        #region Variables
        #region Engines
        /// <summary>
        /// Référence vers le Gestionnaire de Particules.
        /// </summary>
        private GameComponents.Particles.ParticleManager m_particleManager;
        /// <summary>
        /// Référence vers le Gui Manager.
        /// </summary>
        private GameComponents.Gui.GuiManager m_guiManager;
        #endregion
        #region Render
        SpriteBatch m_batch;
        GraphicsDevice m_device;
        private RenderTarget2D m_mapRenderTarget;
        private RenderTarget2D m_shapesRenderTarget;
        private RenderTarget2D m_lightMapRenderTarget;
        private Effect m_lightningEffect;
        private Texture2D m_fogTexture;
        #endregion
        #region Autres
        /// <summary>
        /// Indique si StartDrawEntities a été appelé.
        /// </summary>
        bool m_startDrawEntities;
        #endregion
        #endregion

        #region Properties
        /// <summary>
        /// Obtient ou définit une référence vers le Gestionnaire de Particule.
        /// </summary>
        public override GameComponents.Particles.ParticleManager Particles
        {
            get { return m_particleManager; }
            set { m_particleManager = value; }
        }
        /// <summary>
        /// Obtient ou définit une référence vers la GUI.
        /// </summary>
        public override GameComponents.Gui.GuiManager Gui
        {
            get { return m_guiManager; }
            set { m_guiManager = value; }
        }
        #endregion

        #region Constructeur
        /// <summary>
        /// Crée une nouvelle instance de GraphicsEngine lié à au matériel graphique passé en paramètre.
        /// </summary>
        /// <param name="device"></param>
        public GraphicsEngine2D(GraphicsDevice device)
        {
            m_device = device;
            m_batch = new SpriteBatch(device);
            Particles = new GameComponents.Particles.ParticleManager();
            // Charge les effets et textures.
            m_lightningEffect = LaBotlane.Instance.Content.Load<Effect>("shaders/lightning");
            m_fogTexture = LaBotlane.Instance.Content.Load<Texture2D>("textures/fog");

            // Initialisation des render targets
            int w = Labotlane.Scenes.Scene.ResolutionWidth;
            int h = Labotlane.Scenes.Scene.ResolutionHeight;
            m_lightMapRenderTarget = new RenderTarget2D(LaBotlane.Instance.GraphicsDevice, w, h, true, SurfaceFormat.Color, DepthFormat.None);
            m_mapRenderTarget = new RenderTarget2D(LaBotlane.Instance.GraphicsDevice, w, h, false, SurfaceFormat.Color, DepthFormat.None);
            m_shapesRenderTarget = new RenderTarget2D(LaBotlane.Instance.GraphicsDevice, w, h, false, SurfaceFormat.Color, DepthFormat.None);
            
            // Initialisation de la GUI.
            Gui = new GameComponents.Gui.GuiManager();
            Gui.AddWidget(new GameComponents.Gui.GuiHeaderDisplay());
        }
        #endregion

        #region Update
        /// <summary>
        /// Mets à jour les composants du moteur graphique.
        /// </summary>
        /// <param name="time"></param>
        public override void Update(GameTime time)
        {
            // Mets à jour le moteur de particules.
            Particles.Update(time);
            // Mets à jour l'interface
            Gui.Update(time);
        }
        #endregion
        
        #region Draw

        #region Public API
        /// <summary>
        /// Dessine la GUI.
        /// </summary>
        /// <param name="time"></param>
        public override void DrawGUI(GameTime time)
        {
            MouseState mouseState = Mouse.GetState();
            Rectangle bounds = LaBotlane.Instance.Window.ClientBounds;
            int playerId = Labotlane.Scenes.Scene.Instance.CurrentPlayerId;
            // Dessine le curseur.
            LaBotlane.Instance.GraphicsDevice.SetRenderTarget(null);
            m_batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            // Affiche le numéro du joueur à qui c'est le tour.
            m_batch.DrawString(LaBotlane.Instance.Ressources.Font, "Joueur : " + (playerId + 1).ToString(), Vector2.Zero, Color.White);

            // Dessine l'interface
            Gui.Draw(m_batch);

            m_batch.Draw(LaBotlane.Instance.Ressources.Cursor,
                        new Rectangle(mouseState.X, mouseState.Y, LaBotlane.Instance.Ressources.Cursor.Width, LaBotlane.Instance.Ressources.Cursor.Height),
                        null,
                        Color.White,
                        0.0f,
                        Vector2.Zero,
                        SpriteEffects.None,
                        0.0f
                        );
            m_batch.End();
        }
        /// <summary>
        /// Dessine les entités présentes sur la map.
        /// </summary>
        /// <param name="map"></param>
        public override void DrawMap(GameMap map, GameTime time)
        {
            // Récupération de variables importantes.
            MouseState mouseState = Mouse.GetState();
            Rectangle bounds = LaBotlane.Instance.Window.ClientBounds;
            int w = Labotlane.Scenes.Scene.ResolutionWidth;
            int h = Labotlane.Scenes.Scene.ResolutionHeight;
            int playerId = Labotlane.Scenes.Scene.Instance.CurrentPlayerId;
            // Dessine la map.
            LaBotlane.Instance.GraphicsDevice.SetRenderTarget(m_mapRenderTarget);
            StartDrawEntities();
            map.Draw(m_batch);
            EndDrawEntities();

            // Dessine les formes blocantes de la lumière.
            LaBotlane.Instance.GraphicsDevice.SetRenderTarget(m_shapesRenderTarget);
            LaBotlane.Instance.GraphicsDevice.Clear(Color.Transparent);
            DEBUG_DRAW_DYNAMIC_ENTITIES(map);


            // Crée les lumières à partir des entités appartenant au joueur.
            List<Vector2> lights = new List<Vector2>();

            lights.Add(new Vector2((float)mouseState.X / w, (float)mouseState.Y / Labotlane.Scenes.Scene.ResolutionHeight));
            foreach (GameComponents.Entities.Entity entity in map.DynamicEntities)
            {
                if (entity.IsDynamic && entity.OwnerId == playerId)
                {
                    var gameMapPos = entity.Position.ScreenSpace;
                    lights.Add(new Vector2(
                        (gameMapPos.X + GameComponents.GameMap.TileSize / 2) / w,
                        (gameMapPos.Y + GameComponents.GameMap.TileSize / 2) / Labotlane.Scenes.Scene.ResolutionHeight));
                }
            }


            // Dessine la map de luminosité.
            LaBotlane.Instance.GraphicsDevice.SetRenderTarget(m_lightMapRenderTarget);
            LaBotlane.Instance.GraphicsDevice.Clear(new Color(0, 0, 0, 255));
            foreach (Vector2 light in lights)
            {
                m_batch.Begin(SpriteSortMode.Immediate, BlendState.Additive);

                m_lightningEffect.Parameters["xPosition"].SetValue(light);
                m_lightningEffect.Parameters["xRadius"].SetValue(0.2f);
                m_lightningEffect.Parameters["xObstacleMapTexture"].SetValue(m_shapesRenderTarget);
                m_lightningEffect.Parameters["xMapTexture"].SetValue(m_mapRenderTarget);
                m_lightningEffect.Parameters["xScreenSize"].SetValue(new Vector2(w, h));
                m_lightningEffect.Parameters["MatrixTransform"].SetValue(LaBotlane.Instance.Ressources.PlaneTransform2D);
                m_lightningEffect.CurrentTechnique = m_lightningEffect.Techniques["LightCalc"];
                m_lightningEffect.CurrentTechnique.Passes[0].Apply();
                m_batch.Draw(m_mapRenderTarget, new Rectangle(0, 0, m_lightMapRenderTarget.Width, m_lightMapRenderTarget.Height), Color.White);
                m_batch.End();
            }

            // Combinaison des lumières -> dessin sur le backbuffer.
            LaBotlane.Instance.GraphicsDevice.SetRenderTarget(null);
            LaBotlane.Instance.GraphicsDevice.Clear(Color.Black);
            m_batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            m_lightningEffect.Parameters["xLightMap"].SetValue(m_lightMapRenderTarget);
            m_lightningEffect.Parameters["xFogMap"].SetValue(m_fogTexture);
            m_lightningEffect.Parameters["xElapsedTime"].SetValue((float)time.TotalGameTime.TotalSeconds);
            m_lightningEffect.Parameters["xFogSpeed"].SetValue(0.05f);
            m_lightningEffect.Parameters["xScrolling"].SetValue(new Vector2(map.ScrollingPixels.X / w, map.ScrollingPixels.Y / h));
            m_lightningEffect.Parameters["xLightMapRatio"].SetValue(m_mapRenderTarget.Width / m_lightMapRenderTarget.Width);
            m_lightningEffect.Parameters["MatrixTransform"].SetValue(LaBotlane.Instance.Ressources.PlaneTransform2D);
            m_lightningEffect.CurrentTechnique = m_lightningEffect.Techniques["LightCombine"];
            m_lightningEffect.CurrentTechnique.Passes[0].Apply();

            m_batch.Draw(m_mapRenderTarget, new Rectangle(0, 0, bounds.Width, bounds.Height), Color.White);


            // Dessin des particules
            Particles.Draw(m_batch);
            m_batch.End();
        }
        /// <summary>
        /// Dessine un élément mettant en surbrillance le tile donné.
        /// Doit être appelé entre StartDrawEntities et StartDrawEntities.
        /// </summary>
        /// <param name="tile"></param>
        public override void DrawTileHighlight(Tile tile)
        {
            if (!m_startDrawEntities)
                throw new InvalidOperationException("StartDrawEntities doit être appelé avant de dessiner des entités.");

            m_batch.Draw(LaBotlane.Instance.Ressources.HighlightMark,
                new Rectangle((int)tile.Position.ScreenSpace.X, (int)tile.Position.ScreenSpace.Y, GameMap.TileSize, GameMap.TileSize),
                null,
                Color.White,
                0.0f,
                Vector2.Zero,
                SpriteEffects.None,
                Ressources.GROUND_DEPTH - 0.01f
                );
        }
        /// <summary>
        /// Indique au moteur qu'il doit dessiner une entité.
        /// </summary>
        public override void DrawEntity(Texture2D texture, Rectangle srcRect, GameMap.Position position, Vector2 size, float layerDepth, bool selected)
        {
            if (!m_startDrawEntities)
                throw new InvalidOperationException("StartDrawEntities doit être appelé avant de dessiner des entités.");

            Color color = selected ? Color.Red : Color.White;
            m_batch.Draw(texture,
                new Rectangle((int)(position.ScreenSpace.X), (int)(position.ScreenSpace.Y),
                    (int)size.X,
                    (int)size.Y),
                srcRect,
                color,
                0.0f,
                Vector2.Zero,
                SpriteEffects.None,
                layerDepth);
        }
        #endregion

        #region Private
        /// <summary>
        /// Indique au moteur qu'il commence à dessiner des entités.
        /// </summary>
        void StartDrawEntities()
        {
            if (m_startDrawEntities)
                throw new InvalidOperationException("EndDrawEntities doit être appelé avant un autre appel à StartDrawEntities.");

            m_batch.Begin(SpriteSortMode.BackToFront, BlendState.NonPremultiplied);
            m_startDrawEntities = true;
        }
        /// <summary>
        /// Indique au moteur qu'il a fini de dessiner des entités.
        /// </summary>
        void EndDrawEntities()
        {
            if (!m_startDrawEntities)
                throw new InvalidOperationException("StartDrawEntities doit être appelé avant un autre appel à EndDrawEntities.");

            m_batch.End();
            m_startDrawEntities = false;
        }
        /// <summary>
        /// Appelle les fonctions Draw de chacune des cellules.
        /// </summary>
        /// <param name="batch"></param>
        void DEBUG_DRAW_DYNAMIC_ENTITIES(GameMap map)
        {
            StartDrawEntities();
            foreach (Entities.Entity entity in map.DynamicEntities)
            {
                entity.Draw(map);
            }
            EndDrawEntities();
        }
        #endregion
        #endregion

        #region IDisposable
        /// <summary>
        /// Supprime toutes les ressources non managées allouées par la Scene.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Supprime toutes les ressources non managées allouées par la Scene.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                m_mapRenderTarget.Dispose();
                m_shapesRenderTarget.Dispose();
                m_lightMapRenderTarget.Dispose();
                m_lightningEffect.Dispose();
            }
        }
        #endregion
    }
}
