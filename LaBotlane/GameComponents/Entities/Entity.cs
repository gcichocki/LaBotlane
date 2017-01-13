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
    /// </summary>
    public abstract class Entity
    {
        #region Variables
        /// <summary>
        /// Représente une file d'intéractions à effectuer par l'entité.
        /// </summary>
        protected Queue<Interaction> m_interactions = new Queue<Interaction>();
        /// <summary>
        /// Id du joueur à qui appartient cette entité.
        /// 
        /// Si inférieur à 0 : entité neutre.
        /// </summary>
        protected int m_ownerId = -1;
        /// <summary>
        /// Valeur indiquant si l'unité a effectué son action durant son tour.
        /// </summary>
        protected bool m_actionDone;
        /// <summary>
        /// Position de l'entité.
        /// </summary>
        protected GameMap.Position m_position;
        /// <summary>
        /// Points de vie de l'unité.
        /// </summary>
        protected int m_hp;
        /// <summary>
        /// Nombre de points de mouvement de base de l'unité.
        /// 
        /// Ce nombre de points correspond au nombre de points de mouvement
        /// disponibles à chaque début de tour.
        /// </summary>
        protected int m_baseMovePoints;
        /// <summary>
        /// Nombre de points de mouvement disponibles actuellement pendant le tour.
        /// 
        /// Ce nombre de points correspond au nombre de points de mouvement
        /// disponibles à actuellement pendant le tour.
        /// </summary>
        protected int m_remainingMovePoints;

        /// <summary>
        /// Valeur indiquant l'armure de base de l'unité.
        /// </summary>
        protected int m_armor;

        /// <summary>
        /// Indique la position de l'unité DANS LA TILEMAP.
        /// </summary>
        protected GameMap.Position m_tilemapPosition;
        #endregion
        
        #region Properties
        /// <summary>
        /// Obtient ou définit le montant de Gold que le joueur doit payer pour créer une unité de ce type.
        /// </summary>
        public int GoldCost
        {
            get;
            set;
        }
        /// <summary>
        /// Obtient ou définit la position de l'unité dans la tilemap.
        /// 
        /// Cette valeur garantit les propriétés suivantes :
        ///     - L'entité courante est à la position indiquée par TilemapPosition dans
        ///       la Tilemap.
        /// Elle ne garantit pas :
        ///     - Que la position est celle qui doit être affichée. (utiliser Position).
        ///     - Que la position est celle de l'entité une fois qu'elle 
        ///       aura fini toutes ses intéractions. (utiliser EndOfQueuePosition).
        /// </summary>
        public GameMap.Position TilemapPosition
        {
            get;
            set;
        }
        /// <summary>
        /// Obtient ou définit la position de l'unité une fois toutes les actions dans la queue effectuées.
        /// Cette position est à considérer pour la prévision des actions futures.
        /// 
        /// Cette valeur garantit les propriétés suivantes :
        ///     - Que la position est celle de l'entité une fois qu'elle 
        ///       aura fini toutes ses intéractions.
        /// Elle ne garantit pas :
        ///     - L'entité courante est à la position indiquée par TilemapPosition dans
        ///       la Tilemap (utiliser TilemapPosition).
        ///     - Que la position est celle qui doit être affichée. (utiliser Position).
        /// </summary>
        public GameMap.Position EndOfQueuePosition
        {
            get { return TilemapPosition; }
            set { TilemapPosition = value; }
        }

        /// <summary>
        /// Obtient ou définit la position d'une entité à l'instant présent.
        /// Cette position sert principalement pour l'affichage de l'unité et des effets graphiques.
        /// 
        /// Cette valeur garantit les propriétés suivantes :
        ///     - Que la position est celle qui doit être affichée. (utiliser Position).
        /// Elle ne garantit pas :
        ///     - L'entité courante est à la position indiquée par TilemapPosition dans
        ///       la Tilemap (utiliser TilemapPosition).
        ///     - Que la position est celle de l'entité une fois qu'elle 
        ///       aura fini toutes ses intéractions.
        /// </summary>
        public virtual GameMap.Position Position
        {
            get { return m_position; }
            set { m_position = value; }
        }
        /// <summary>
        /// Obtient une valeur indiquant l'armure de base l'unité.
        /// </summary>
        public virtual int Armor
        {
            get { return m_armor; }
            protected set { m_armor = value; }
        }

        /// <summary>
        /// Indique si l'entité est traversable.
        /// </summary>
        public bool IsCrossable
        {
            get { return (Type & TargetType.Crossable) == TargetType.Crossable; }
        }
        /// <summary>
        /// Retourne une valeur indiquant si l'entité est dynamique et doit être régulièrement mise à jour.
        /// 
        /// Cette propriété doit être à vrai pour toutes les unités contrôlées par les joueurs,
        /// et fausse pour les décors et autres objets immobiles.
        /// </summary>
        public bool IsDynamic
        {
            get { return (Type & TargetType.Dynamic) == TargetType.Dynamic; }
        }
        /// <summary>
        /// Obtient le type de cette unité.
        /// </summary>
        public abstract TargetType Type
        {
            get;
        }
        /// <summary>
        /// Obtient ou définit une valeur indiquant si l'unité a effectué son action durant son
        /// tour.
        /// </summary>
        public bool ActionDone
        {
            get { return m_actionDone; }
            set { m_actionDone = value; }
        }
        /// <summary>
        /// Obtient ou définit l'id du joueur à qui appartient cette entité.
        /// -1 : entité neutre.
        /// </summary>
        public virtual int OwnerId
        {
            get 
            {
                // Retourne -1 si l'entité est neutre.
                if ((Type & TargetType.Neutral) == TargetType.Neutral)
                    return -1;
                else
                    return m_ownerId; 
            }
            set
            {
                // Lève une exception si on essaye de changer l'owner id d'une entité neutre.
                if((Type & TargetType.Neutral) == TargetType.Neutral)
                    throw new InvalidOperationException("Impossible de modifier l'Owner Id d'une entité neutre");

                m_ownerId = value; 
            }
        }
        /// <summary>
        /// Nombre de points de mouvement de base de l'unité.
        /// 
        /// Ce nombre de points correspond au nombre de points de mouvement
        /// disponibles à chaque début de tour.
        /// </summary>
        public virtual int BaseMovePoints
        {
            get { return m_baseMovePoints; }
        }
        /// <summary>
        /// Nombre de points de mouvement disponibles actuellement pendant le tour.
        /// 
        /// Ce nombre de points correspond au nombre de points de mouvement
        /// disponibles à actuellement pendant le tour.
        /// </summary>
        public int RemainingMovePoints
        {
            get { return m_remainingMovePoints; }
            set { m_remainingMovePoints = value; }
        }
        /// <summary>
        /// Retourne une valeur indiquant si l'unité est décédée.
        /// </summary>
        public bool IsDead
        {
            get { return HP <= 0; }
        }

        /// <summary>
        /// Obtient ou définit les points de vie de l'unité.
        /// </summary>
        public int HP
        {
            get { return m_hp; }
            protected set 
            {
                m_hp = value;
                if(m_hp<=0)
                {
                    LaBotlane.Instance.Scene.Map.RemoveEntity(this);
                }
            }
        }

        /// <summary>
        /// Obtient la portée d'une attaque.
        /// La valeur X du point correspond à la distance minimum, 
        /// La valeur Y du point correspond à la distance maximum.
        /// Unité : la case.
        /// </summary>
        public virtual Point Portance
        {
            get { return new Point(0, 0); }
        }
        
        #endregion

        #region Methods

        #region Abstract
        /// <summary>
        /// Obtient les dégâts infligés par l'unité à une unité dont le type
        /// est passé en paramètre.
        /// 
        /// Prends en compte les bonus/malus offensifs de l'unité.
        /// </summary>
        /// <param name="type">Type de l'unité attaquée.</param>
        /// <returns></returns>
        public abstract int GetDealtDamage(TargetType type);
        /// <summary>
        /// Effectue le traitement des dégâts infligés par l'unité dont le type est renseigné.
        /// 
        /// Prend en compte les bonus/malus défensifs de l'unité.
        /// </summary>
        /// <param name="amount">Nombre de dégâts bruts infligés.</param>
        /// <param name="source">Type source de l'attaque.</param>
        /// <return>Le nombre de dégats infligés au final.</return>
        public abstract int ReceiveDamage(int amount, TargetType source);
        /// <summary>
        /// Dessine l'entité à l'écran.
        /// </summary>
        /// <param name="batch"></param>
        /// <param name="map">Map sur laquelle est dessinée l'entité.</param>
        public abstract void Draw(GameMap map);
        /// <summary>
        /// Mets à jour la logique de l'entité.
        /// </summary>
        /// <param name="time"></param>
        public virtual void Update(GameTime time)
        {
            UpdateInteractionQueue(time);
        }
        #endregion

        #region Virtual
        /// <summary>
        /// Réinitialise les points de mouvement.
        /// </summary>
        public virtual void ResetMovePoints()
        {
            RemainingMovePoints = BaseMovePoints;
        }

        #region Interactions
        /// <summary>
        /// Mets l'intéraction passée en paramètre dans la queue des intéractions à effectuer par
        /// cette unité.
        /// </summary>
        /// <param name="interaction"></param>
        public virtual void ApplyInteraction(Interaction interaction)
        {
            m_interactions.Enqueue(interaction);
        }

        /// <summary>
        /// Mets à jour le traitement des intéractions.
        /// </summary>
        protected virtual void UpdateInteractionQueue(GameTime time)
        {
            if(m_interactions.Count != 0)
            {
                UpdateInteraction(m_interactions.Peek(), time);

                // Si l'intéraction est terminée, la supprime de la file.
                if (m_interactions.Peek().Ended)
                    m_interactions.Dequeue();
            }
        }

        /// <summary>
        /// Mets à jour l'intéraction passée en paramètre.
        /// 
        /// Contient la logique et la partie graphique des intéractions.
        /// 
        /// Si l'intéraction est terminée, mets la propriété Ended de l'intéraction à
        /// true.
        /// 
        /// -- Sécurité.
        /// Si une action n'est pas possible à cet endroit, lever une exception !
        /// 
        /// <returns>Retourne une valeur indiquant si l'intéraction a été traitée par cette méthode.</returns>
        /// </summary>
        protected virtual bool UpdateInteraction(Interaction interaction, GameTime time)
        {
            if (interaction == null)
                throw new ArgumentNullException();
            // Indique si une intéraction a été traitée.
            bool interactionProcessed = false;

            switch(interaction.InteractionType)
            {
                // Permet le déplacement d'une unité.
                case InteractionType.Move:
                    // Mise à jour de la trajectoire.
                    Trajectory trajectory = ((Trajectory)interaction.InteractionState["Trajectory"]);
                    trajectory.NextStep((float)time.ElapsedGameTime.TotalSeconds*5);

                    // Si on atteint une nouvelle case, envoie un signal à la map pour dire que l'on vient de traverser 
                    // la case.
                    if (Position.MapSpacePoint != trajectory.Position.MapSpacePoint)
                        LaBotlane.Instance.Scene.Map.SendEntityCrossSignal(trajectory.Position, this);

                    // Mise à jour de la position.
                    Position = trajectory.Position;
                    interaction.Ended = trajectory.IsEnded;
                    interactionProcessed = true;
                    break;
                case InteractionType.Attack:
                    if (interaction.DstEntity == null || interaction.SrcEntity == null)
                        throw new InvalidOperationException();
                    
                    // Effectue l'action d'attaque et récupère le nombre de dégâts
                    // déjà encaissés par la cible.
                    int dealtDamage = interaction.DstEntity.ReceiveDamage(interaction.SrcEntity.GetDealtDamage(interaction.SrcEntity.Type), interaction.SrcEntity.Type);
                    
                    // Ajoute un effet de particule
                    {
                        Particles.ParticleText part = new Particles.ParticleText(LaBotlane.Instance.Scene.Time)
                        {
                            MoveFunction = Particles.ParticleText.MoveLine(interaction.DstEntity.Position + GameMap.Position.FromMapSpacePoint(0, -4)),
                            StartPosition = interaction.DstEntity.Position,
                            Text = "-" + dealtDamage.ToString(),
                            DurationSeconds = 1.0f
                        };
                        LaBotlane.Instance.Scene.GraphicsEngine.Particles.Add(part);
                    }

                    // Termine l'intéraction.
                    interaction.Ended = true;
                    interactionProcessed = true;
                    ActionDone = true;
                    break;
                case InteractionType.Teleport:
                    interactionProcessed = true;
                    break;
            }

            return interactionProcessed;
        }

        /// <summary>
        /// Retourne la liste des intéractions possibles avec le tile passé en paramètre.
        /// </summary>
        /// <param name="dstEntity"></param>
        /// <returns></returns>
        public virtual List<Interaction> GetInteractions(Tile dstTile)
        {
            return new List<Interaction>();
        }

        /// <summary>
        /// Fonction appelée lorsque cette entité est traversée par l'entité passée en argument.
        /// </summary>
        public virtual void OnCrossedBy(Entity entity)
        {

        }
        #endregion

        #endregion

        #endregion
    }
}