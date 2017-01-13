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
    /// Classe abstraite de base des moteurs graphiques.
    /// Contient des wrappers vers des méthodes de dessin.
    /// </summary>
    public abstract class GraphicsEngine : IDisposable
    {
  

        #region Properties
        /// <summary>
        /// Obtient ou définit une référence vers le Gestionnaire de Particule.
        /// </summary>
        public abstract GameComponents.Particles.ParticleManager Particles { get; set; }
        /// <summary>
        /// Obtient ou définit une référence vers la GUI.
        /// </summary>
        public abstract GameComponents.Gui.GuiManager Gui { get; set; }
        #endregion

        #region Constructeur
        #endregion

        #region Update
        /// <summary>
        /// Mets à jour les composants du moteur graphique.
        /// </summary>
        /// <param name="time"></param>
        public abstract void Update(GameTime time);
        #endregion

        #region Draw

        #region Public API
        /// <summary>
        /// Dessine la GUI.
        /// </summary>
        /// <param name="time"></param>
        public abstract void DrawGUI(GameTime time);
        /// <summary>
        /// Dessine les entités présentes sur la map.
        /// </summary>
        /// <param name="map"></param>
        public abstract void DrawMap(GameMap map, GameTime time);
        /// <summary>
        /// Dessine un élément mettant en surbrillance le tile donné.
        /// Doit être appelé entre StartDrawEntities et StartDrawEntities.
        /// </summary>
        /// <param name="tile"></param>
        public abstract void DrawTileHighlight(Tile tile);
        /// Indique au moteur qu'il doit dessiner une entité.
        /// </summary>
        public abstract void DrawEntity(Texture2D texture, Rectangle srcRect, GameMap.Position position, Vector2 size, float layerDepth, bool selected);
        #endregion

        #region IDisposable
        public void Dispose() { }
        public virtual void Dispose(bool disposed = true) { }

        #endregion
        #endregion
    }
}
