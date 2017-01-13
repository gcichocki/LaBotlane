using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace Labotlane.GameComponents.Entities
{
    /// <summary>
    /// Sol de base.
    /// </summary>
    public class WaterEntity : Entity
    {
        #region Properties

        #region Overrides
        /// <summary>
        /// Obtient le type de l'entité.
        /// </summary>
        public override TargetType Type
        {
            get
            {
                return TargetType.Invincible | TargetType.Neutral | TargetType.Water;
            }
        }        
        /// <summary>
        /// Obtient ou définit l'id du joueur à qui appartient cette entité.
        /// -1 : entité neutre.
        /// </summary>
        public override int OwnerId
        {
            get
            {
                return -1;
            }
            set
            {
                throw new Exception();
            }
        }

        /// <summary>
        /// Obtient ou définit la portée d'une attaque.
        /// La valeur X du point correspond à la distance minimum, 
        /// La valeur Y du point correspond à la distance maximum.
        /// Unité : la case.
        /// </summary>
        public override Point Portance
        {
            get { return new Point(1, 5); }
        }
        #endregion

        #endregion

        #region Methods

        #region Abstract Implementation
        /// <summary>
        /// Obtient les dégâts infligés par l'unité à une unité dont le type
        /// est passé en paramètre.
        /// 
        /// Prends en compte les bonus/malus offensifs de l'unité.
        /// </summary>
        /// <param name="type">Type de l'unité attaquée.</param>
        /// <returns></returns>
        public override int GetDealtDamage(TargetType type)
        {
            return 0;
        }
        /// <summary>
        /// Effectue le traitement des dégâts infligés par l'unité dont le type est renseigné.
        /// 
        /// Prend en compte les bonus/malus défensifs de l'unité.
        /// </summary>
        /// <param name="amount">Nombre de dégâts bruts infligés.</param>
        /// <param name="source">Type source de l'attaque.</param>
        public override int ReceiveDamage(int amount, TargetType source)
        {
            return 0;
        }
        /// <summary>
        /// Dessine l'entité à l'écran.
        /// </summary>
        /// <param name="batch"></param>
        /// <param name="gameScreen">Map sur laquelle est dessinée l'entité.</param>
        public override void Draw(GameMap map)
        {
            LaBotlane.Instance.Scene.GraphicsEngine.DrawEntity(map.TilesetTexture,
                new Rectangle(272, 144, 32, 32), 
                Position,
                new Vector2(GameMap.TileSize, GameMap.TileSize),
                Ressources.GROUND_DEPTH,
                false);
        }

        /// <summary>
        /// Mets à jour la logique de l'entité.
        /// </summary>
        /// <param name="time"></param>
        public override void Update(GameTime time)
        {
            base.Update(time);
        }
        #endregion

        #region Virtual

        #endregion

        #endregion
    }
}
