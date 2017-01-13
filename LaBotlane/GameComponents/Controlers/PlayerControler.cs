using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
namespace Labotlane.GameComponents.Controlers
{
    /// <summary>
    /// Classe de base permettant le contrôle d'un joueur.
    /// 
    /// Le contrôleur :
    ///     - Détermine possibilité des intéractions.
    ///     - Contient les méthodes permettant d'effectuer les intéractions demandées.
    /// </summary>
    public abstract class PlayerControler
    {

        #region Variables
        /// <summary>
        /// Joueur contrôlé par ce contrôleur.
        /// </summary>
        private Player m_player;
        /// <summary>
        /// Id du joueur contrôlé par ce contrôleur.
        /// </summary>
        protected int m_playerId;
        #endregion

        #region Properties
        /// <summary>
        /// Obtient ou définit l'id du joueur contrôlé par ce contrôleur.
        /// </summary>
        public int PlayerId
        {
            get { return m_playerId; }
            protected set { m_playerId = value; }
        }

        /// <summary>
        /// Obtient une référence vers le joueur contrôlé par ce contrôleur.
        /// </summary>
        public Player Player
        {
            get { return m_player; }
            protected set { m_player = value; }
        }

        /// <summary>
        /// Obtient une valeur indiquant si le tour actuel est terminé.
        /// </summary>
        public bool TurnEnded
        {
            get;
            protected set;
        }

        /// <summary>
        /// Obtient ou définit une référence vers l'entité contrôlée.
        /// Null si aucune.
        /// </summary>
        public Entities.Entity ControledEntity
        {
            get;
            set;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Crée une nouvelle instance de PlayerControler.
        /// </summary>
        /// <param name="playerId"></param>
        public PlayerControler(int playerId)
        {
            PlayerId = playerId;
            Player = new Player();
        }

        #region Logique de jeu tour par tout
        /// <summary>
        /// Mets à jour le contrôleur du joueur.
        /// Appelé lorsque c'est au tour du joueur contrôlé par ce contrôleur.
        /// </summary>
        /// <param name="time"></param>
        public abstract void Update(GameTime time);

        /// <summary>
        /// Appelé au démarrage du tour de ce joueur.
        /// </summary>
        public virtual void StartTurn()
        {
            TurnEnded = false;
        }

        /// <summary>
        /// Appelé à la fin du tour de ce joueur.
        /// </summary>
        public virtual void EndTurn()
        {
            TurnEnded = true;
        }
        #endregion

        #region Actions sur la map

        #region Movement
        /// <summary>
        /// Déplace l'entité désignée par entity à la position indiquée par position.
        /// 
        /// LE PLUS COURT CHEMIN est automatiquement choisi.
        /// 
        /// Si le déplacement est impossible, lève une exception.
        /// </summary>
        /// <param name="position"></param>
        public void MoveTo(Entities.Entity entity, GameMap.Position position)
        {
            // Vérification de la paternité de l'entité.
            if (entity.OwnerId != m_playerId)
                throw new Exception("Tentative de déplacement d'une entité n'appartenant pas à ce joueur.");

            // TODO
            throw new NotImplementedException();
        }
        #endregion

        #region Interactions
        /// <summary>
        /// Retourne une valeur indiquant si l'intéraction passée en argument est réalisable.
        /// </summary>
        /// <param name="interaction"></param>
        public bool IsInteractionPossible(Entities.Interaction interaction)
        {
            //return true; // DEBUG
            //if (interaction.SrcEntity.ActionDone)
            //    return false;
            switch(interaction.InteractionType)
            {
                case Entities.InteractionType.Move:
                    return IsMovePossible(interaction);
                case Entities.InteractionType.SpawnEntity:
                    return IsSpawnPossible(interaction);
                case Entities.InteractionType.Attack:
                    return IsAttackPossible(interaction);

            }
            return true;
        }

        /// <summary>
        /// Retourne une valeur indiquant si l'action de spawn est possible.
        /// L'entité doit être à proximité immédiate de la factory source, l'emplacement
        /// de destination doit être libre et le joueur doit avoir assez d'argent.
        /// </summary>
        /// <param name="interaction"></param>
        /// <returns></returns>
        bool IsSpawnPossible(Entities.Interaction interaction)
        {
            var dstEntity = ((Entities.Entity)interaction.InteractionState["EntityToSpawn"]);
            var spawnPosition = ((GameMap.Position)interaction.InteractionState["SpawnPosition"]);
            bool hasMoney = dstEntity.GoldCost <= LaBotlane.Instance.Scene.Controlers[LaBotlane.Instance.Scene.CurrentPlayerId].Player.Gold;
            bool factoryActionDone = interaction.SrcEntity.ActionDone;
            bool isNear = GameMap.Position.DistanceSquared(spawnPosition, interaction.SrcEntity.TilemapPosition) <= 1;
            bool isFree = LaBotlane.Instance.Scene.Map.GetTileAt(spawnPosition).IsCrossable;
            return hasMoney && isNear && isFree && !factoryActionDone;
        }
        /// <summary>
        /// Retourne une valeur indiquant si l'intéraction d'attaque passée en paramètre est réalisable.
        /// Une unité doit être présente, ou l'action échouera.
        /// La position ciblée doit être à une distance inférieure ou égale à la portée de l'attaque.
        /// L'entité source doit être en mesure d'attaquer.
        /// </summary>
        /// <param name="interaction"></param>
        /// <returns></returns>
        bool IsAttackPossible(Entities.Interaction interaction)
        {
            // Vérification de la faisabilité de l'action.
            if (interaction.DstEntity == null || interaction.SrcEntity.ActionDone)
                return false;

            Point dstEntityPos = interaction.DstEntity.EndOfQueuePosition.MapSpacePoint;
            Point srcEntityPos = interaction.SrcEntity.EndOfQueuePosition.MapSpacePoint;
            bool cond = true;

            // Distance entre les entités.
            float distance = (float)Math.Sqrt(Math.Pow(srcEntityPos.X - dstEntityPos.X, 2) + Math.Pow(srcEntityPos.Y - dstEntityPos.Y, 2));//new Point(srcEntityPos.X - dstEntityPos.X, srcEntityPos.Y - dstEntityPos.Y);

            // Vérification de la portée :
            cond = cond && interaction.SrcEntity.Portance.X <= distance;
            cond = cond && interaction.SrcEntity.Portance.Y >= distance;


            return cond;
        }
        /// <summary>
        /// Retourne une valeur indiquant si l'intéraction de mouvement passée en paramètre est réalisable.
        /// </summary>
        /// <param name="interaction"></param>
        /// <returns></returns>
        bool IsMovePossible(Entities.Interaction interaction)
        {
            return LaBotlane.Instance.Scene.Map.CanMoveTo(interaction.SrcEntity, ((Trajectory)interaction.InteractionState["Trajectory"]).LastPosition());
        }

        #region Apply Interaction
        /// <summary>
        /// Applique l'intéraction passée en paramètre.
        /// 
        /// -- Sécurité 
        /// Lève une exception si cette interaction n'est pas autorisée.
        /// </summary>
        /// <param name="interaction"></param>
        public void ApplyInteraction(Entities.Interaction interaction)
        {
            switch(interaction.InteractionType)
            {
                case Entities.InteractionType.Move:
                    ApplyMoveInteraction(interaction);
                    break;
                case Entities.InteractionType.SpawnEntity:
                    ApplySpawnInteraction(interaction);
                    break;
                case Entities.InteractionType.Attack:
                    ApplyAttackInteraction(interaction);
                    break;
            }
        }
        /// <summary>
        /// Applique une interaction de type Attaque.
        /// </summary>
        /// <param name="interaction"></param>
        void ApplyAttackInteraction(Entities.Interaction interaction)
        {
            // Validation de l'intéraction.
            if(interaction.DstEntity == null || interaction.DstEntity.IsDead)
            {
                new Entities.InvalidInteractionException("Impossible d'attaquer une entité morte ou inexistante.");
            }

            // Indique à l'entité attaquante qu'elle devra attaquer l'entité de destination.
            interaction.SrcEntity.ApplyInteraction(interaction);
        }
        /// <summary>
        /// Applique une interaction de type Move.
        /// </summary>
        /// <param name="interaction"></param>
        void ApplyMoveInteraction(Entities.Interaction interaction)
        {
            // La validité de cette intéraction est effectuée par la map.
            LaBotlane.Instance.Scene.Map.MoveEntity(interaction.SrcEntity, ((Trajectory)interaction.InteractionState["Trajectory"]).LastPosition());
        }
        /// <summary>
        /// Applique une interaction de type Spawn.
        /// </summary>
        /// <param name="interaction"></param>
        void ApplySpawnInteraction(Entities.Interaction interaction)
        {
            // La validité de cette intéraction est vérifiée par la map.
            Entities.Entity entityToSpawn = (Entities.Entity)interaction.InteractionState["EntityToSpawn"];
            GameMap.Position spawnPosition = (GameMap.Position)interaction.InteractionState["SpawnPosition"];
            LaBotlane.Instance.Scene.Map.SpawnEntity(entityToSpawn, spawnPosition);

            // Décompte du gold si l'action est faite par une factory.
            if(interaction.SrcEntity is Entities.FactoryEntity)
            {
                Entities.FactoryEntity facto = (Entities.FactoryEntity)interaction.SrcEntity;
                LaBotlane.Instance.Scene.Controlers[LaBotlane.Instance.Scene.CurrentPlayerId].Player.Gold -= entityToSpawn.GoldCost;
                facto.ActionDone = true;
            }
        }
        #endregion
        /// <summary>
        /// Retourne l'icone représentant l'intéraction passée en paramètre.
        /// </summary>
        /// <param name="interaction"></param>
        /// <returns></returns>
        public Texture2D GetInteractionIcon(Entities.Interaction interaction)
        {
            switch(interaction.InteractionType)
            {
                case Entities.InteractionType.Attack:
                    return LaBotlane.Instance.Ressources.IconAttack;
                case Entities.InteractionType.Move:
                    return LaBotlane.Instance.Ressources.IconMove;
                default:
                    return LaBotlane.Instance.Ressources.IconMove;
            }
        }
        #endregion

        #endregion

        #region Scripting
        /// <summary>
        /// Exécute un script de manière sécurisée en affichant un rapport d'erreur indiquant les erreurs
        /// de compilation et d'exécution.
        /// </summary>
        /// <param name="script">script à exécuter.</param>
        /// <param name="interpreter">interpréteur à utiliser pour exécuter le script.</param>
        /// <param name="displayReport">indique si le rapport doit être affiché.</param>
        /// <returns>Vrai si aucune erreur ne s'est produite.</returns>
        public bool SafeExecuteScript(string script, Interpreter.CommandInterpreter interpreter, bool displayReport=false)
        {
            Interpreter.ErrorReport report;
            List<Entities.Interaction> interactions;
            bool hasWarnings;
            bool hasCompilationErrors;
            bool hasExecutionErrors = false;

            // Compilation du script.
            hasCompilationErrors = !interpreter.CompileScriptWithReport(script.ToString(), out report, out interactions, out hasWarnings);
            
            // Exécution du script si pas d'erreur.
            if (!hasCompilationErrors)
            {
                // On effectue les intéractions, et on regarde si tout a bien marché.
                for (int i = 0; i < interactions.Count; i++)
                {
                    Entities.Interaction interaction = interactions[i];
                    try
                    {
                        // Si des intéractions compilées par le script ne sont pas valides, une exception sera levée.
                        ApplyInteraction(interaction);
                    }
                    catch (Entities.InvalidInteractionException e)
                    {
                        hasExecutionErrors = true;
                        report.Messages.Add("[Ligne " + interaction.Debug_LineNumber + "] Erreur fatale : exécution de l'intéraction n°" + (i + 1).ToString() + " : " + e.Message);
                        report.Messages.Add("Arrêt de l'exécution du script.");
                    }
                }

                // Affiche un rapport s'il y a des erreurs d'exécution ou des warnings de compilation.
                if ((hasExecutionErrors || hasWarnings) && displayReport)
                    report.Display();
            }
            else if(displayReport)
            {
                report.Display();
            }

            return !(hasCompilationErrors | hasExecutionErrors);
        }
        /// <summary>
        /// Exécute un script de manière sécurisée, et renvoie en paramètre out le rapport d'erreur.
        /// </summary>
        /// <param name="script">script à exécuter.</param>
        /// <param name="interpreter">interpréteur à utiliser pour exécuter le script.</param>
        /// <returns>Vrai si aucune erreur ne s'est produite.</returns>
        public bool SafeExecuteScript(string script, Interpreter.CommandInterpreter interpreter, out Interpreter.ErrorReport report)
        {
            List<Entities.Interaction> interactions;
            bool hasWarnings;
            bool hasCompilationErrors;
            bool hasExecutionErrors = false;

            // Compilation du script.
            hasCompilationErrors = !interpreter.CompileScriptWithReport(script.ToString(), out report, out interactions, out hasWarnings);

            // Exécution du script si pas d'erreur.
            if (!hasCompilationErrors)
            {
                // On effectue les intéractions, et on regarde si tout a bien marché.
                for (int i = 0; i < interactions.Count; i++)
                {
                    Entities.Interaction interaction = interactions[i];
                    try
                    {
                        // Si des intéractions compilées par le script ne sont pas valides, une exception sera levée.
                        ApplyInteraction(interaction);
                    }
                    catch (Entities.InvalidInteractionException e)
                    {
                        hasExecutionErrors = true;
                        report.Messages.Add("[Ligne " + interaction.Debug_LineNumber + "] Erreur fatale : exécution de l'intéraction n°" + (i + 1).ToString() + " : " + e.Message);
                        report.Messages.Add("Arrêt de l'exécution du script.");
                    }
                }
            }

            return !(hasCompilationErrors | hasExecutionErrors);
        }

        /// <summary>
        /// Exécute le script contenu dans le fichier passé en paramètre.
        /// Affiche le rapport d'erreur si des erreurs existent.
        /// </summary>
        /// <param name="filename"></param>
        public void ExecuteScriptFile(string filename, Interpreter.CommandInterpreter interpreter, bool displayReport=false)
        {
            GameMap map = LaBotlane.Instance.Scene.Map;

            // Position de départ de l'entité à faire spawn.
            Entities.FactoryEntity facto = (Entities.FactoryEntity)map.FindEntityByType(PlayerId, Entities.TargetType.Factory).First();
            interpreter.AddEntity(facto, "factory");
            ControledEntity = facto;

            // Chargement du script
            filename = filename.Contains(".txt") ? filename : filename + ".txt";
            var stream = System.IO.File.Open("Content\\scripts\\" + filename, System.IO.FileMode.Open);
            System.IO.StreamReader reader = new System.IO.StreamReader(stream);
            string script = reader.ReadToEnd();
            reader.Dispose();
            stream.Dispose();

            // Exécute le script, et si tout a fonctionné, sélectionne l'entité soldier (pour tester).
            SafeExecuteScript(script.ToString(), interpreter, displayReport);
        }

        /// <summary>
        /// Exécute le script contenu dans le fichier passé en paramètre.
        /// Renvoie le rapport d'erreur généré par la compilation.
        /// Le rapport d'erreur ne sera pas affiché.
        /// </summary>
        /// <returns>True s'il n'y a pas eu d'erreurs.</returns>
        /// <param name="filename"></param>
        public bool ExecuteScriptFile(string filename, Interpreter.CommandInterpreter interpreter, out Interpreter.ErrorReport report)
        {
            GameMap map = LaBotlane.Instance.Scene.Map;

            // Position de départ de l'entité à faire spawn.
            Entities.FactoryEntity facto = (Entities.FactoryEntity)map.FindEntityByType(PlayerId, Entities.TargetType.Factory).First();
            interpreter.AddEntityIfDoesntExist(facto, "factory");
            ControledEntity = facto;

            // Chargement du script
            filename = filename.Contains(".txt") ? filename : filename + ".txt";
            var stream = System.IO.File.Open("Content\\scripts\\" + filename, System.IO.FileMode.Open);
            System.IO.StreamReader reader = new System.IO.StreamReader(stream);
            string script = reader.ReadToEnd();
            reader.Dispose();
            stream.Dispose();

            // Exécute le script, et si tout a fonctionné, sélectionne l'entité soldier (pour tester).
            return SafeExecuteScript(script.ToString(), interpreter, out report);
        }

        /// <summary>
        /// Exécute le script passé en paramètre.
        /// </summary>
        /// <returns>True s'il n'y a pas eu d'erreur.</returns>
        /// <param name="filename"></param>
        public bool ExecuteScript(string script, Interpreter.CommandInterpreter interpreter, bool displayReport=false)
        {
            GameMap map = LaBotlane.Instance.Scene.Map;

            // Position de départ de l'entité à faire spawn.
            Entities.FactoryEntity facto = (Entities.FactoryEntity)map.FindEntityByType(PlayerId, Entities.TargetType.Factory).First();
            interpreter.AddEntityIfDoesntExist(facto, "factory");

            // Exécute le script, et si tout a fonctionné, sélectionne l'entité soldier (pour tester).
            return SafeExecuteScript(script.ToString(), interpreter, displayReport);
        }
        #endregion
        #endregion
    }
}
