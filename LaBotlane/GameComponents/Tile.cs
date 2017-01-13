using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace Labotlane.GameComponents
{
    /// <summary>
    /// Représente une case de la map.
    /// </summary>
    public sealed class Tile
    {
        #region Variables
        /// <summary>
        /// Contient une référence vers toutes les entités contenues dans ce tile.
        /// </summary>
        private List<Entities.Entity> m_entities;
        /// <summary>
        /// Obtient ou définit une valeur indiquant si le tile doit être mis en valeur par un 
        /// procédé graphique.
        /// 
        /// Cette valeur est remise à false à chaque fin de frame, afin d'éviter aux objets utilisant
        /// la fonctionnalité de devoir manuellement "éteindre" la highlight et ainsi d'éviter les conflits.
        /// </summary>
        private bool m_isHighlighted;
        #endregion

        #region Properties
        /// <summary>
        /// Obtient une référence vers la référence vers toutes les entités 
        /// contenues dans ce tile.
        /// </summary>
        public List<Entities.Entity> Entities
        {
            get { return m_entities; }
            set { m_entities = value; }
        }
        /// <summary>
        /// Obtient ou définit la position du tile dans la tilemap.
        /// (information dupliquée).
        /// </summary>
        public GameMap.Position Position
        {
            get;
            set;
        }

        /// <summary>
        /// Obtient une valeur indiquant si ce tile est traversable.
        /// 
        /// Le tile est traversable si et seulement si toutes les entités
        /// qu'il contient sont traversables.
        /// </summary>
        public bool IsCrossable
        {
            get
            {
                foreach(Entities.Entity entity in Entities)
                {
                    if (!entity.IsCrossable)
                        return false;
                }
                return true;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Crée une nouvelle instance de Tile.
        /// </summary>
        public Tile()
        {
            m_entities = new List<Entities.Entity>();
            
        }

        #region API
        /// <summary>
        /// Mets en valeur ce tile pendant une frame.
        /// </summary>
        public void Highlight()
        {
            m_isHighlighted = true;
        }
        #endregion

        #region Drawing
        /// <summary>
        /// Dessine toutes les entités STATIQUES présentes dans cette case.
        /// </summary>
        /// <param name="batch"></param>
        /// <param name="map"></param>
        public void Draw(GameMap map)
        {
            // Dessine les entités.
            foreach (Entities.Entity entity in m_entities)
            {
                if (!entity.IsDynamic)
                    entity.Draw(map);
            }

            // Dessine un effet graphique sur le tile.
            if (m_isHighlighted)
                LaBotlane.Instance.Scene.GraphicsEngine.DrawTileHighlight(this);

            m_isHighlighted = false;
        }
        #endregion
        #endregion
    }
}
