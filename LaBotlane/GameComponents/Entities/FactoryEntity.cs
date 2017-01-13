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
    public class FactoryEntity : Entity
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
                return TargetType.Factory;
            }
        }

        /// <summary>
        /// Obtient le nombre de points de mouvements de base.
        /// </summary>
        public override int BaseMovePoints
        {
            get
            {
                return 200;
            }
        }
        #endregion

        #endregion

        #region Methods
        /// <summary>
        /// Crée une nouvelle instance de FactoryEntity.
        /// </summary>
        public FactoryEntity() : base()
        {

        }

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
            HP -= amount;
            return amount;
        }
        /// <summary>
        /// Dessine l'entité à l'écran.
        /// </summary>
        /// <param name="batch"></param>
        /// <param name="gameScreen">Map sur laquelle est dessinée l'entité.</param>
        public override void Draw(GameMap map)
        {
            if (map == null)
                throw new ArgumentNullException();

            // Dessine l'unité.
            LaBotlane.Instance.Scene.GraphicsEngine.DrawEntity(map.TilesetTexture,
                new Rectangle(288, 0, 32, 32), 
                Position,
                new Vector2(GameMap.TileSize, GameMap.TileSize),
                Ressources.UNITS_DEPTH,
                LaBotlane.Instance.Scene.Controlers[LaBotlane.Instance.Scene.CurrentPlayerId].ControledEntity == this);
        }

        /// <summary>
        /// Mets à jour la logique de l'entité.
        /// </summary>
        /// <param name="time"></param>
        public override void Update(GameTime time)
        {
            base.Update(time);
        }

        /// <summary>
        /// Retourne la liste des intéractions possibles avec le tile passé en paramètre.
        /// </summary>
        /// <param name="dstEntity"></param>
        /// <returns></returns>
        public override List<Interaction> GetInteractions(Tile dstTile)
        {
            
            // Affiche l'action de spawn si la destination se trouve immédiatement à proximité de l'Usine.
            float dst = GameMap.Position.DistanceSquared(dstTile.Position, TilemapPosition);
            if(dst <= 1)
            {
                SoldierEntity entity = new SoldierEntity() { OwnerId = OwnerId };
                Interaction spawnInteraction = Interaction.SpawnInteraction(this, entity, dstTile.Position, "Soldat (" + entity.GoldCost.ToString() + "G)");
                List<Interaction> interactions = new List<Interaction>();
                interactions.Add(spawnInteraction);
                return interactions;
            }
            
            return new List<Interaction>();
        }
        #endregion

        #region Virtual

        #endregion

        #endregion
    }
}
