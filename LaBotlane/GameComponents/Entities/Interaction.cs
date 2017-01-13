using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Labotlane.GameComponents.Entities
{
    public enum InteractionType
    {
        None,
        Move,
        Teleport,
        Attack,
        SpawnEntity,
    }
    /// <summary>
    /// Exception permettant d'identifier des erreurs lors de l'exécution d'intéractions invalides.
    /// </summary>
    public class InvalidInteractionException : Exception
    {
        public InvalidInteractionException() : base() { }
        public InvalidInteractionException(string msg) : base(msg) { }
    }
    /// <summary>
    /// Représente une intéraction entre deux entités.
    /// </summary>
    public class Interaction
    {
        #region Debug Utils
        /// <summary>
        /// Si cette intéraction a été générée à partir d'un script, retourne le
        /// numéro de ligne du script à partir duquel cette intéraction a été générée.
        /// </summary>
        public int Debug_LineNumber { get; set; }
        #endregion
        /// <summary>
        /// Obtient ou définit l'entité source de l'intéraction. 
        /// </summary>
        public Entities.Entity SrcEntity { get; set; }

        /// <summary>
        /// Obtient ou définit l'entité cible de l'intéraction.
        /// </summary>
        public Entities.Entity DstEntity { get; set; }

        /// <summary>
        /// Obtient ou définit la description de l'intéraction.
        /// Cette description sera affichée au joueur.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Obtient ou définit le type d'intéraction entre les deux entités.
        /// </summary>
        public InteractionType InteractionType { get; set; }

        /// <summary>
        /// Obtient ou définit des paramètres additionnels d'intéraction, ainsi
        /// que des paramètres d'état de l'intéraction si celle ci se déroule
        /// sur une période de temps supérieure à une frame.
        /// </summary>
        public Dictionary<string, object> InteractionState { get; set; }

        /// <summary>
        /// Indique si l'intéraction est terminée.
        /// </summary>
        public bool Ended { get; set; }

        /// <summary>
        /// Crée une nouvelle instance de Interaction.
        /// </summary>
        private Interaction()
        {
            InteractionState = new Dictionary<string, object>();
            InteractionType = Entities.InteractionType.None;
            Ended = false;
        }

        #region Factory
        /// <summary>
        /// Crée une intéraction nulle.
        /// </summary>
        /// <returns></returns>
        public static Interaction NullInteraction()
        {
            return new Interaction();
        }
        /// <summary>
        /// Crée une nouvelle intéraction de type Move.
        /// </summary>
        /// <param name="entity">Entité à déplacer.</param>
        /// <param name="trajectory">Trajectoire sur laquelle déplacer l'entité.</param>
        /// <returns></returns>
        public static Interaction MoveInteraction(Entity entity, Trajectory trajectory)
        {
            if (entity == null | trajectory == null)
                throw new ArgumentNullException("[MoveInteraction] : arguments nulls");
            Interaction interaction = new Interaction();
            interaction.SrcEntity = entity;
            interaction.Description = "Aller à";
            interaction.InteractionType = InteractionType.Move;
            interaction.InteractionState["Trajectory"] = trajectory;
            return interaction;
        }

        /// <summary>
        /// Crée une nouvelle intéraction de type Spawn.
        /// Fait spawn une entité passée en paramètre sur la map.
        /// </summary>
        /// <param name="toSpawn">Entity à faire spawn.</param>
        /// <param name="entityName">Nom de l'entité.</param>
        /// <returns></returns>
        public static Interaction SpawnInteraction(Entity srcEntity, Entity toSpawn, GameMap.Position position, string entityName)
        {
            if (srcEntity == null | toSpawn == null)
                throw new ArgumentNullException("[SpawnInteraction] : arguments nulls");
            Interaction interaction = new Interaction();
            interaction.SrcEntity = srcEntity;
            interaction.Description = entityName;
            interaction.InteractionType = InteractionType.SpawnEntity;
            interaction.InteractionState["EntityToSpawn"] = toSpawn;
            interaction.InteractionState["SpawnPosition"] = position;
            return interaction;
        }

        /// <summary>
        /// Crée une nouvelle intéraction de type attaque.
        /// 
        /// Envoie un signal d'attaque à l'unité ennemie à la position ciblée.
        /// 
        /// Une unité doit être présente, ou l'action échouera.
        /// La position ciblée doit être à une distance inférieure ou égale à la portée de l'attaque.
        /// L'entité source doit être en mesure d'attaquer.
        /// </summary>
        /// <param name="srcEntity"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public static Interaction AttackInteraction(Entity srcEntity, GameMap.Position position)
        {
            if (srcEntity == null)
                throw new ArgumentNullException("[AttackInteraction] : arguments nulls");

            Interaction interaction = new Interaction();
            interaction.SrcEntity = srcEntity;
            interaction.DstEntity = null;
            foreach(Entity entity in LaBotlane.Instance.Scene.Map.GetTileAt(position).Entities)
            {
                if((entity.Type & TargetType.Targetable) != 0 && entity.OwnerId!=srcEntity.OwnerId)
                {
                    interaction.DstEntity = entity;
                    break;
                }
            }

            if (interaction.DstEntity == null)
                throw new InvalidInteractionException("Il n'y a pas d'entité à l'endroit sélectionné");

            interaction.Description = "Attaquer";
            interaction.InteractionType = InteractionType.Attack;
            return interaction;
        }
        #endregion
    }
}
