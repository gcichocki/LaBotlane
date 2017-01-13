using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace Labotlane.GameComponents.Entities
{
    /// <summary>
    /// Classe de base de toutes les entités du jeu.
    /// Chaque entité a à sa charge :
    ///     - Gestion dynamique de ses stats.
    ///     - Dessin
    /// L'entité ne connaît que sa position d'affichage, mais
    /// pas sa position dans le jeu. Elle sera automatiquement
    /// mise à jour par le moteur de jeu.
    /// </summary>
    public class SoldierEntity : Entity
    {

        #region Properties

        public int Debug_Variant
        {
            get;
            set;
        }
        #endregion

        #region Overrides
        /// <summary>
        /// Obtient le type de l'entité.
        /// </summary>
        public override TargetType Type
        {
            get
            {
                return TargetType.Soldier;
            }
        }
        /// <summary>
        /// Obtient le nombre de points de mouvement de base.
        /// </summary>
        public override int BaseMovePoints
        {
            get
            {
                return 20;
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
            return 2;
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
            Point srcTile = new Point(4 * 32, 0);
            switch(Debug_Variant)
            {
                case 0:
                    srcTile = new Point(4 * 32, 0);
                    break;
                case 1:
                    srcTile = new Point(4 * 32, 3 * 32);
                    break;
                case 2:
                    srcTile = new Point(4 * 32, 6 * 32);
                    break;
                case 3:
                    srcTile = new Point(2 * 32, 0);
                    break;
                case 4:
                    srcTile = new Point(2 * 32, 3 * 32);
                    break;
                case 5:
                    srcTile = new Point(2 * 32, 6 * 32);
                    break;
            }

            // Dessine l'unité.
            LaBotlane.Instance.Scene.GraphicsEngine.DrawEntity(map.TilesetTexture,
                new Rectangle(srcTile.X, srcTile.Y, 32, 32),
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
        #endregion

        #region Virtual
        /// <summary>
        /// Crée une nouvelle instance de SoldierEntity avec les paramètres par défaut.
        /// </summary>
        public SoldierEntity()
        {
            HP = 4;
            ResetMovePoints();
            
        }

        /// <summary>
        /// Retourne la liste des intéractions possibles avec le tile passé en paramètre.
        /// </summary>
        /// <param name="dstEntity"></param>
        /// <returns></returns>
        public override List<Interaction> GetInteractions(Tile dstTile)
        {
            // Indique si le tile passé en paramètre contient une unité dynamique empêchant
            // la fabrication d'une unité.
            bool hasNonCrossableUnit = false;
            bool hasEnnemyUnit = false;
            foreach (Entity entity in dstTile.Entities)
            {
                if (entity.IsDynamic || !(entity.IsCrossable))
                    hasNonCrossableUnit = true;
                if ((entity.Type & TargetType.Targetable) != 0 && entity.OwnerId != OwnerId)
                    hasEnnemyUnit = true;
            }
            List<Interaction> interactions = new List<Interaction>();

            if (!hasNonCrossableUnit)
            {
                // Si pas d'unité, ajoute la possibilité de se déplacer.
                Interaction moveInteraction = Interaction.MoveInteraction(this, new Trajectory(LaBotlane.Instance.Scene.Map.GetTrajectoryTo(EndOfQueuePosition, dstTile.Position)));
                interactions.Add(moveInteraction);
            }
            if (hasEnnemyUnit)
            {
                Interaction attackInteraction = Interaction.AttackInteraction(this, dstTile.Position);
                interactions.Add(attackInteraction);
            }
            return interactions;

            
        }

        #endregion

        #endregion
    }
}
