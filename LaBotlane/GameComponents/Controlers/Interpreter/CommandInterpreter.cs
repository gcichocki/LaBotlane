using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
namespace Labotlane.GameComponents.Controlers.Interpreter
{
    /// <summary>
    /// Classe permettant d'interpréter du code afin de stocker des scénarios de test.
    /// 
    /// 
    /// Commandes possibles :
    ///     - spawn ownerName x y entityType entityName # Fait spawner une entité
    ///     - attack srcEntityName x y                  # Fait attaquer une entité
    ///     - move entityName x y                       # Déplace une entité.
    ///     
    ///     - spawnfactory ownerId x y entityName       # Fait spawner une usine.
    ///     - resetmp entityName                        # Effectue un reset des move points.
    ///     - @control entityName                       # prend le contrôle d'une entité.
    ///     - @clearlocal all                           # Supprime le contexte local entier
    ///     - @clearlocal name                          # Supprime l'entité name du contexte local.
    ///     - @display entityName PropertyName          # Affiche dans les systèmes de sorties la valeur de la 
    ///                                                 # propriété PropertyName de l'entité
    ///     - @track entityName PropertyName            # Affiche de manière permanente une propriété jusqu'à l'appel de
    ///     - @untrack entityName PropertyName          # untrack
    ///     - @untrackall entityName                    # Untraque toutes les propriétés de l'entité donnée. Mot clef all : toutes les entités.
    ///     
    ///  Remarque : pour les commandes comme move, il est possible de spécifier des entiers absolus à la position d'une entité grâce au
    ///  préfixe @.
    /// </summary>
    public class CommandInterpreter
    {
        const bool ABORT_ON_ERROR = false;

        #region Output Systems
        /// <summary>
        /// Représente un objet contenant des informations permettant de tracker une variable.
        /// </summary>
        public class Tracker
        {
            /// <summary>
            /// Obtient ou définit une valeur indiquant si ce tracker à pour but la suppression d'une variable trackée.
            /// (si false, ce tracker a pour but de tracker une variable).
            /// </summary>
            public bool Delete
            {
                get;
                set;
            }
            /// <summary>
            /// Obtient ou définit le nom de l'entité trackée.
            /// </summary>
            public string OwnerName { get; set; }
            /// <summary>
            /// Obtient ou définit l'objet sur lequel tracker la propriété.
            /// </summary>
            public object Owner { get; set; }
            /// <summary>
            /// Obtient ou définit la propriété trackée.
            /// </summary>
            public PropertyInfo Property { get; set; }
            /// <summary>
            /// Retourne la valeur de la propriété trackée.
            /// </summary>
            /// <returns></returns>
            public object GetValue()
            {
                if (Owner == null || Property == null)
                    throw new InvalidOperationException("Objet tracker mal initialisé");

                return Property.GetValue(Owner, new object[] { });
            }
            
            /// <summary>
            /// Crée une nouvelle instance de Tracker.
            /// </summary>
            public Tracker()
            {

            }

            /// <summary>
            /// Crée une nouvelle instance de Tracker, en spécifiat un Owner et une Property à tracker.
            /// </summary>
            public Tracker(object owner, string ownerName, PropertyInfo property, bool delete)
            {
                Owner = owner;
                Property = property;
                Delete = delete;
                OwnerName = ownerName;
            }


            /// <summary>
            /// Retourne vrai si deux trackers trackent la même propriété.
            /// </summary>
            /// <returns></returns>
            public bool Match(Tracker tracker)
            {
                return ((tracker.Owner == Owner) || (OwnerName == "all") || (tracker.OwnerName == "all")) && 
                    (tracker.Property == Property || Property == null || tracker.Property == null);
            }
        }
        #endregion

        /// <summary>
        /// Représente une commande de l'interpréteur.
        /// </summary>
        public struct Command
        {
            /// <summary>
            /// Delegate permettant la transformation d'une commande en Interaction.
            /// arguments : arguments de la commande
            /// interpreter : ref vers l'interpréteur qui exécute la commande
            /// inter       : intéraction résultant de la commande
            /// error       : erreur éventuelle.
            /// return     : valeur indiquant si la commande s'est bien passée.
            /// </summary>
            public delegate bool  ExecuteCommandDelegate(string[] arguments, CommandInterpreter interpreter,
                out Entities.Interaction inter, out string error);

            /// <summary>
            /// Représente un argument.
            /// </summary>
            public struct Arg
            {
                public string Name;
                public Arg(string name)
                {
                    Name = name;
                }
            }
            /// <summary>
            /// Liste des arguments de la commande.
            /// </summary>
            public List<Arg> Arguments;
            /// <summary>
            /// Fonction exécutée lorsque la commande doit être exécutée.
            /// </summary>
            public ExecuteCommandDelegate Function;
        }

        #region Variables
        /// <summary>
        /// Représente les entités créées par cet interpréteur, indexées par leur nom.
        /// </summary>
        Dictionary<string, GameComponents.Entities.Entity> m_createdEntities;
        /// <summary>
        /// Set de commandes gérées par l'interpréteur indexées par leurs nom.
        /// </summary>
        Dictionary<string, Command> m_commandSet;
        #endregion

        #region Properties
        /// <summary>
        /// Obtient ou définit l'id du propriétaire des entités qui vont 
        /// </summary>
        public int CurrentOwnerId
        {
            get;
            set;
        }
        /// <summary>
        /// Ligne entrain d'être exécutée par l'interpréteur.
        /// (utilisée pour la récupération de l'erreur).
        /// </summary>
        public int CurrentLine
        {
            get;
            private set;
        }
        /// <summary>
        /// Obtient ou définit la liste des Gestionnaires de sortie utilisés par l'interpréteur.
        /// </summary>
        public List<OutputSystem> OutputSystems
        {
            get;
            set;
        }
        #endregion


        #region Methods
        /// <summary>
        /// Crée une nouvelle instance de CommandInterpreter.
        /// </summary>
        public CommandInterpreter()
        {
            m_createdEntities = new Dictionary<string, Entities.Entity>();
            m_commandSet = new Dictionary<string, Command>();
            OutputSystems = new List<OutputSystem>();
            CreateCommands();
        }

        /// <summary>
        /// Vide le contexte de cet interpréteur, y compris toutes les entités mémorisées.
        /// </summary>
        public void ClearContext()
        {
            m_createdEntities.Clear();
        }
        /// <summary>
        /// Ajoute une entité créée par l'interpréteur.
        /// </summary>
        public void AddEntity(Entities.Entity entity, string name)
        {
            m_createdEntities.Add(name, entity);
        }
        /// <summary>
        /// Ajoute une entité créée par l'interpréteur, si celle-ci n'existe déja pas.
        /// </summary>
        public void AddEntityIfDoesntExist(Entities.Entity entity, string name)
        {
            if (!m_createdEntities.ContainsKey(name))
                m_createdEntities.Add(name, entity);
        }
        /// <summary>
        /// Retourne l'entité crée par l'interpréteur dont le nom est 'name'.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Entities.Entity GetEntity(string name)
        {
            if (m_createdEntities.ContainsKey(name))
                return m_createdEntities[name];
            else
                return null;
        }

        /// <summary>
        /// Compile le script passé en paramètre.
        /// Un rapport d'erreurs sera automatiquement rempli et retourné en paramètre out.
        /// </summary>
        /// <param name="script">Script à compiler</param>
        /// <param name="report">Rapport d'erreur éventuel suite à la compilation de la commande.</param>
        /// <returns>Retourne une valeur indiquant si la commande s'est bien passée. (true = pas d'erreur)</returns>
        public bool CompileScriptWithReport(string script, out ErrorReport report, out List<Entities.Interaction> interactions, out bool hasWarnings)
        {
            // Crée une error report et l'ajoute à la liste des systèmes de sortie.
            report = new ErrorReport(); report.OriginalScript = script;
            OutputSystems.Add(report);

            bool isOK = CompileScript(script, out interactions, out hasWarnings);

            OutputSystems.Remove(report);
            return isOK;
        }
        /// <summary>
        /// Compile le script passé en paramètre.
        /// Un rapport d'erreurs sera automatiquement rempli et retourné en paramètre out.
        /// </summary>
        /// <param name="script">Script à compiler</param>
        /// <param name="report">Rapport d'erreur éventuel suite à la compilation de la commande.</param>
        /// <returns>Retourne une valeur indiquant si la commande s'est bien passée. (true = pas d'erreur)</returns>
        public bool CompileScript(string script, out List<Entities.Interaction> interactions, out bool hasWarnings)
        {
            string[] lines = script.Replace("\r", "").Split('\n');
            bool hasErrors = false;
            hasWarnings = false;
            interactions = new List<Entities.Interaction>();
            int errorCount = 0;
            int warningCount = 0;
            for (int i = 0; i < lines.Length; i++)
            {
                string msg;
                Entities.Interaction inter;
                CurrentLine = i + 1;
                string command = lines[i].Trim(' ', '\t', '\r');
                // Saute la ligne si elle est vide ou si c'est un commentaire.
                if (command == "" || command.StartsWith("#"))
                    continue;

                // Exécute la commande et loggue les éventuels messages d'erreur.
                if (!CompileCommand(command, out msg, out inter))
                {
                    PrintError(msg);


                    // Erreur ne pouvant être ignorée.
                    if (!msg.Contains("#warning"))
                    {
                        hasErrors = true;
                        errorCount++;
                        // Si l'erreur est fatale et qu'on a définit l'arrêt sur erreur.
                        if (ABORT_ON_ERROR)
                        {
                            PrintStatus("Erreur fatale. Arrêt de la compilation du script à la ligne " + CurrentLine.ToString());
                            break;
                        }
                    }
                    else
                    {
                        hasWarnings = true;
                        warningCount++;
                    }
                }
                else
                {
                    interactions.Add(inter);
                }
            }

            // Ajout d'un message donnant le statut de la compilation.
            if (hasErrors)
                PrintStatus("Echec de la compilation : " + errorCount.ToString() + " erreurs, " + warningCount.ToString() + " warnings.");
            else
                PrintStatus("Compilation réussie : " + errorCount.ToString() + " erreurs, " + warningCount.ToString() + " warnings.");

            return !hasErrors;
        }
        #region Private
        /// <summary>
        /// Compile la commande passée en paramètre.
        /// 
        /// <param name="command">Commande à compiler.</param>
        /// <param name="lineNumber">Numéro de ligne à laquelle est compilée la commande.</param>
        /// <param name="errorMsg">Message d'erreur éventuel.</param>
        /// <returns>Retourne une valeur indiquant si la compilaiton de la commande s'est bien passée. (true = pas d'erreur)</returns>
        /// </summary>
        bool CompileCommand(string command, out string errorMsg, out Entities.Interaction interaction)
        {
            string[] tokens = command.Split(' ');
            interaction = Entities.Interaction.NullInteraction();

            // Vérification du nombre de jetons.
            if (tokens.Length < 1)
            {
                errorMsg = "Commande : '" + command + "' invalide. (ligne " + CurrentLine.ToString() + ")";
                return false;
            }

            // Sélection de la commande.
            string commandName = tokens[0];
            errorMsg = "";
            bool hasErrors = false;

            // Si la commande existe.
            if (m_commandSet.ContainsKey(commandName))
            {
                Command cmd = m_commandSet[commandName];
                if (cmd.Arguments.Count() == tokens.Length - 1)
                {
                    // Collecte les arguments de la fonction.
                    bool hasNamedArguments = tokens[1].Contains('='); // indique si la fonction est appelée avec des arguments nommés.
                    string[] arguments = new string[tokens.Length - 1];
                    if (hasNamedArguments)
                    {
                        // Arguments nommés :
                        List<int> argsFound = new List<int>(); // liste des n° d'arguments trouvés : évite les doublons.
                        for(int i = 0; i < arguments.Length; i++)
                        {
                            // Erreur : argument non nommé
                            if(!tokens[i+1].Contains("="))
                            {
                                hasErrors = true;
                                errorMsg = "[Ligne " + CurrentLine.ToString() + "] Le paramètre n°" + (i+1).ToString() + " doit être nommé.";
                                return !hasErrors;
                            }

                            // Obtiention des left / right values.
                            string[] lrValues = tokens[i + 1].Split('=');

                            // Recherche de l'argument ayant ce nom
                            bool found = false;
                            int argId = 0;
                            for(int j = 0; j < cmd.Arguments.Count; j++)
                            {
                                string argName = lrValues[0];
                                if(cmd.Arguments[j].Name == argName)
                                {
                                    // Vérification : existe-t-il des doublons ? (arguments spécifiés 2 fois).
                                    if(argsFound.Contains(j))
                                    {
                                        hasErrors = true;
                                        errorMsg = "[Ligne " + CurrentLine.ToString() + "] Le paramètre nommé '" + lrValues[0] + "' est en double.";
                                        return !hasErrors;
                                    }
                                    // Argument trouvé
                                    found = true;
                                    argId = j;
                                    argsFound.Add(j);
                                    break;
                                }
                            }

                            // Si l'argument est trouvé, on le rentre dans la liste des arguments.
                            if(found)
                            {
                                arguments[argId] = lrValues[1];
                            }
                            else
                            {
                                hasErrors = true;
                                errorMsg = "[Ligne " + CurrentLine.ToString() + "] Le paramètre nommé '" + lrValues[0] + "' n'existe pas pour la commande "
                                    + commandName + ".";

                                // Vérification : y a-t-il une commande proche ?
                                var argsAsString = cmd.Arguments.Select(delegate(Command.Arg arg) { return arg.Name; }).ToList<string>();
                                string nearest;
                                if(Tools.StringUtils.FindNearest(lrValues[0], argsAsString, 33, out nearest))
                                {
                                    errorMsg += " (Vouliez-vous dire '" + nearest + "' ?)";
                                }
                                return !hasErrors;
                            }
                        }
                    }
                    else
                    {
                        // Arguments non nommés : mis dans l'ordre de la fonction.
                        for (int i = 0; i < arguments.Length; i++)
                        {
                            // Erreur : argument nommé
                            if (tokens[i + 1].Contains("="))
                            {
                                hasErrors = true;
                                errorMsg = "[Ligne " + CurrentLine.ToString() + "] Le paramètre n°" + (i + 1).ToString() + " ne peut pas être nommé.";
                                return !hasErrors;
                            }
                            arguments[i] = tokens[i + 1];
                        }
                    }

                    // Exécute la commande si les arguments sont corrects.
                    if(!hasErrors)
                    {
                        hasErrors = !cmd.Function(arguments, this, out interaction, out errorMsg);
                    }
                }
                else
                {
                    // Montre l'erreur à l'utilisateur.
                    StringBuilder msg = new StringBuilder();
                    msg.AppendLine("[Ligne " + CurrentLine.ToString() + "] Erreur : Nombre d'arguments passés à la commande " + command.ToString() + " sont invalides.");
                    msg.Append("Attendus : ");
                    foreach (Command.Arg arg in cmd.Arguments)
                    {
                        msg.Append(arg.Name + " ");
                    }
                    errorMsg = msg.ToString();
                    hasErrors = true;
                }
            }
            else
            {
                // Si la commande n'existe pas, on le signale à l'utilisateur.
                errorMsg = "[Ligne " + CurrentLine.ToString() + "] La commande '" + commandName + "' n'existe pas.";

                // On peut aussi lui proposer une commande + appropriée.
                // Vérification : y a-t-il une commande proche ?
                var argsAsString = m_commandSet.Keys.Select(delegate(string arg) { return arg; }).ToList<string>();
                string nearest;
                if (Tools.StringUtils.FindNearest(commandName, argsAsString, 50, out nearest))
                {
                    errorMsg += " (Vouliez-vous dire '" + nearest + "' ?)";
                }

                hasErrors = true;
            }


            return !hasErrors;
        }

        #region Create Commands
        /// <summary>
        /// Crée la liste des commandes.
        /// </summary>
        void CreateCommands()
        {
            CreateCommandAttack();
            CreateCommandMove();
            CreateCommandSpawn();
            CreateCommandControl();
            CreateCommandResetMp();
            CreateCommandSpawnFactory();
            CreateCommandScript();
            CreateCommandDisplay();
            CreateCommandDisplay(true);
            CreateCommandDisplay(true, true);
            CreateCommandUntrackAll();
            CreateCommandClearLocal();
            CreateCommandImportFactory();
        }

        void CreateCommandSpawnFactory()
        {
            // Spawn factory
            Command commandSpawn = new Command();
            commandSpawn.Arguments = new List<Command.Arg>() { new Command.Arg("ownerId"), // nom de l'entité faisant spawn l'entité cible.
                new Command.Arg("x"),
                new Command.Arg("y"),
                new Command.Arg("name") };
            commandSpawn.Function = new Command.ExecuteCommandDelegate(delegate(string[] args,
                CommandInterpreter interpreter,
                out Entities.Interaction inter,
                out string error)
            {

                error = "";
                inter = Entities.Interaction.NullInteraction();
                try
                {
                    // -- Crée l'entité.
                    Entities.Entity entity;
                    entity = new Entities.FactoryEntity() { OwnerId = ParseIntWithError(args[0], commandSpawn, 0) };

                    // -- Crée l'intéraction.
                    int x = ParseIntWithError(args[1], commandSpawn, 1);
                    int y = ParseIntWithError(args[2], commandSpawn, 2);
                    inter = Entities.Interaction.SpawnInteraction(
                        entity,
                        entity,
                        GameMap.Position.FromMapSpacePoint(x, y),
                        args[3]

                        );
                    inter.Debug_LineNumber = CurrentLine;

                    // Ajoute l'entité.
                    AddEntity(entity, args[3]);

                   
                }
                catch (FormatException fe)
                {
                    error = fe.Message;
                    return false;
                }
                catch (Exception e)
                {
                    error = "[Ligne " + CurrentLine.ToString() + "] Exception lors de la création de la commande : " + e.Message;
                    return false;
                }

                return true;
            });
            m_commandSet.Add("spawnfactory", commandSpawn);
        }

        void CreateCommandResetMp()
        {
            // Permet de prendre le contrôle d'une entité.
            Command commandResetMp = new Command();
            commandResetMp.Arguments = new List<Command.Arg>() { new Command.Arg("src") };
            commandResetMp.Function = new Command.ExecuteCommandDelegate(delegate(string[] args,
                CommandInterpreter interpreter,
                out Entities.Interaction inter,
                out string error)
            {

                error = "";
                inter = Entities.Interaction.NullInteraction();
                try
                {
                    Entities.Entity srcEntity = interpreter.GetEntity(args[0]);
                    if (srcEntity == null)
                    {
                        error = "[Ligne " + CurrentLine.ToString() + "] L'entité " + args[0] + " n'existe pas.";
                        return false;
                    }
                    srcEntity.ResetMovePoints();
                }
                catch (Exception e)
                {
                    error = "[Ligne " + CurrentLine.ToString() + "] Exception lors de la création de la commande : " + e.Message;
                    return false;
                }

                return true;
            });
            m_commandSet.Add("resetmp", commandResetMp);
        }

        void CreateCommandControl()
        {
            // Permet de prendre le contrôle d'une entité.
            Command commandControl = new Command();
            commandControl.Arguments = new List<Command.Arg>() { new Command.Arg("dst") };
            commandControl.Function = new Command.ExecuteCommandDelegate(delegate(string[] args,
                CommandInterpreter interpreter,
                out Entities.Interaction inter,
                out string error)
            {
                
                error = "";
                inter = Entities.Interaction.NullInteraction();
                try
                {
                    Entities.Entity srcEntity = interpreter.GetEntity(args[0]);
                    if (srcEntity == null)
                    {
                        error = "[Ligne " + CurrentLine.ToString() + "] L'entité " + args[0] + " n'existe pas.";
                        return false;
                    }
                    Labotlane.Scenes.Scene.Instance.Controlers[interpreter.CurrentOwnerId].ControledEntity = srcEntity;
                }
                catch (Exception e)
                {
                    error = "[Ligne " + CurrentLine.ToString() + "] Exception lors de la création de la commande : " + e.Message;
                    return false;
                }

                return true;
            });
            m_commandSet.Add("@control", commandControl);
        }

        void CreateCommandScript()
        {
            // Permet de prendre le contrôle d'une entité.
            Command commandLdScript = new Command();
            commandLdScript.Arguments = new List<Command.Arg>() { new Command.Arg("filename") };
            commandLdScript.Function = new Command.ExecuteCommandDelegate(delegate(string[] args,
                CommandInterpreter interpreter,
                out Entities.Interaction inter,
                out string error)
            {

                error = "";
                inter = Entities.Interaction.NullInteraction();
                ErrorReport report;
                bool isOK = false;
                try
                {
                    isOK = Labotlane.Scenes.Scene.Instance.Controlers[interpreter.CurrentOwnerId].ExecuteScriptFile(args[0], this, out report);
                    error = "[Ligne " + CurrentLine.ToString() + "] Exception lors de l'exécution du script " + args[0] + " : \n\r" +
                            Tools.StringUtils.Indent(report.GetErrorReportText(), 1); // TODO
                    // Affiche le rapport d'erreurs.
                    if (!isOK)
                        report.Display();
                }
                catch(System.IO.FileNotFoundException e)
                {
                    error = "[Ligne " + CurrentLine.ToString() + "] Script '" + args[0] + "' introuvable.";
                }

                return isOK;
            });
            m_commandSet.Add("@ldscript", commandLdScript);
        }

        void CreateCommandClearLocal()
        {
            // Permet de prendre le contrôle d'une entité.
            Command commandLdScript = new Command();
            commandLdScript.Arguments = new List<Command.Arg>() { new Command.Arg("entity") };
            commandLdScript.Function = new Command.ExecuteCommandDelegate(delegate(string[] args,
                CommandInterpreter interpreter,
                out Entities.Interaction inter,
                out string error)
            {

                error = "";
                inter = Entities.Interaction.NullInteraction();

                // Clear tout le contexte si l'argument vaut all.
                if (args[0] == "all")
                    ClearContext();
                else
                {
                    if(m_createdEntities.ContainsKey(args[0]))
                        m_createdEntities.Remove(args[0]);

                }

                return true;
            });
            m_commandSet.Add("@clearlocal", commandLdScript);
        }

        void CreateCommandSpawn()
        {
            // Attaque
            Command commandSpawn = new Command();
            commandSpawn.Arguments = new List<Command.Arg>() { new Command.Arg("src"), // nom de l'entité faisant spawn l'entité cible.
                new Command.Arg("x"),
                new Command.Arg("y"),
                new Command.Arg("type"),
                new Command.Arg("name") };
            commandSpawn.Function = new Command.ExecuteCommandDelegate(delegate(string[] args,
                CommandInterpreter interpreter,
                out Entities.Interaction inter,
                out string error)
            {
                
                error = "";
                inter = Entities.Interaction.NullInteraction();
                try
                {
                    Entities.Entity entity;
                    Entities.Entity srcEntity = interpreter.GetEntity(args[0]);
                    if (srcEntity == null)
                    {
                        error = "[Ligne " + CurrentLine.ToString() + "] L'entité " + args[0] + " n'existe pas.";
                        return false;
                    }

                    // -- Crée l'entité.
                    Type type = Assembly.GetExecutingAssembly().GetType("Labotlane.GameComponents.Entities." + args[3], true);
                    try { entity = (Entities.Entity)Activator.CreateInstance(type); }
                    catch { throw new Exception("[Ligne " + CurrentLine.ToString() + "] le type d'entité '" + args[3] + "' n'existe pas." ); }

                    // -- Crée l'intéraction.
                    entity.OwnerId = srcEntity.OwnerId;
                    int x = ParseIntWithError(args[1], commandSpawn, 1);
                    int y = ParseIntWithError(args[2], commandSpawn, 2);

                    // -- Update l'état de l'entité.
                    entity.EndOfQueuePosition = GameMap.Position.FromMapSpacePoint(x, y);
                    

                    inter = Entities.Interaction.SpawnInteraction(
                        srcEntity,
                        entity,
                        GameMap.Position.FromMapSpacePoint(x, y),
                        args[4]

                        );
                    inter.Debug_LineNumber = CurrentLine;
                    // Ajoute l'entité.
                    AddEntity(entity, args[4]);
                }
                catch (FormatException fe)
                {
                    error = fe.Message;
                    return false;
                }
                catch (Exception e)
                {
                    error = "[Ligne " + CurrentLine.ToString() + "] Exception lors de la création de la commande : " + e.Message;
                    return false;
                }

                return true;
            });
            m_commandSet.Add("spawn", commandSpawn);
        }

        void CreateCommandMove()
        {
            // Attaque
            Command commandMove = new Command();
            commandMove.Arguments = new List<Command.Arg>() { new Command.Arg("src"), // nom de l'entité à déplacer
                new Command.Arg("x"),
                new Command.Arg("y") };
            commandMove.Function = new Command.ExecuteCommandDelegate(delegate(string[] args,
                CommandInterpreter interpreter,
                out Entities.Interaction inter,
                out string error)
            {
                error = "";
                inter = Entities.Interaction.NullInteraction();
                try
                {
                    Entities.Entity srcEntity = interpreter.GetEntity(args[0]);
                    if (srcEntity == null)
                    {
                        error = "[Ligne " + CurrentLine.ToString() + "] L'entité '" + args[0] + "' n'existe pas.";
                        return false;
                    }

                    bool relativeX = !args[1].StartsWith("@");
                    bool relativeY = !args[2].StartsWith("@");
                    int x = ParseIntWithError(args[1], commandMove, 1) + (relativeX ? srcEntity.EndOfQueuePosition.MapSpacePoint.X : 0);
                    int y = ParseIntWithError(args[2], commandMove, 2) + (relativeY ? srcEntity.EndOfQueuePosition.MapSpacePoint.Y : 0);
                    inter = Entities.Interaction.MoveInteraction(
                        srcEntity,
                        new GameComponents.Trajectory(LaBotlane.Instance.Scene.Map.GetTrajectoryTo(
                            srcEntity.EndOfQueuePosition,
                            GameMap.Position.FromMapSpacePoint(x, y)))

                        );
                    inter.Debug_LineNumber = CurrentLine;
                }
                catch (FormatException fe)
                {
                    error = fe.Message;
                    return false;
                }
                /*catch(Exception e)
                {
                    error = "[Ligne " + CurrentLine.ToString() + "] Exception lors de la création de la commande : " + e.Message;
                    return false;
                }*/
 
                return true;
            });
            m_commandSet.Add("move", commandMove);
        }

        void CreateCommandAttack()
        {
            // Attaque
            Command commandAttack = new Command();
            commandAttack.Arguments = new List<Command.Arg>() { new Command.Arg("src"), // nom de l'entité source
                new Command.Arg("x"),
                new Command.Arg("y") };
            commandAttack.Function = new Command.ExecuteCommandDelegate(delegate(string[] args,
                CommandInterpreter interpreter,
                out Entities.Interaction inter,
                out string error)
            {
                error = "";
                inter = Entities.Interaction.NullInteraction();
                try
                {
                    // Récupération de l'entité
                    Entities.Entity srcEntity = interpreter.GetEntity(args[0]);
                    if (srcEntity == null)
                    {
                        error = "[Ligne " + CurrentLine.ToString() + "] L'entité " + args[0] + " n'existe pas.";
                        return false;
                    }

                    // Création de l'intéraction.
                    int x = ParseIntWithError(args[1], commandAttack, 1);
                    int y = ParseIntWithError(args[2], commandAttack, 2);
                    
                    inter = Entities.Interaction.AttackInteraction(
                        interpreter.GetEntity(args[0]),
                        GameMap.Position.FromMapSpacePoint(x, y));
                    inter.Debug_LineNumber = CurrentLine;
                }
                catch (FormatException fe)
                {
                    error = fe.Message;
                    return false;
                }
                catch (Exception e)
                {
                    error = "[Ligne " + CurrentLine.ToString() + "] Exception lors de la création de la commande : " + e.Message;
                    return false;
                }

                return true;
            });
            m_commandSet.Add("attack", commandAttack);
        }

        void CreateCommandDisplay(bool tracker=false, bool track=false)
        {
            // Attaque
            Command commandDisplay = new Command();
            commandDisplay.Arguments = new List<Command.Arg>() { new Command.Arg("src"), // nom de l'entité source
                new Command.Arg("property") };
            commandDisplay.Function = new Command.ExecuteCommandDelegate(delegate(string[] args,
                CommandInterpreter interpreter,
                out Entities.Interaction inter,
                out string error)
            {
                error = "";
                inter = Entities.Interaction.NullInteraction();
                try
                {
                    // Récupération de l'entité
                    Entities.Entity srcEntity = interpreter.GetEntity(args[0]);
                    if (srcEntity == null)
                    {
                        error = "[Ligne " + CurrentLine.ToString() + "] L'entité " + args[0] + " n'existe pas.";
                        return false;
                    }

                    // Récupération de la propriété.
                    Type t = srcEntity.GetType();
                    string propertyName = args[1];

                    try
                    {
                        // Récupération de la valeur de la propriété 
                        PropertyInfo prop = t.GetProperty(propertyName);
                        string s = prop.GetValue(srcEntity, new object[] { }).ToString();
                        if(tracker)
                        {
                            // Envoie un tracker à la sortie.
                            PrintObject(new Tracker(srcEntity, args[0], prop, !track));
                        }
                        else
                        {
                            Print(s);
                        }
                    }
                    catch
                    {
                        error = "[Ligne " + CurrentLine.ToString() + "] L'entité '" + args[0] + "' de type '" + t.Name +
                            "' ne contient pas de propriété nommée " + args[1] + ".";

                        // Vérification : y a-t-il une propriété proche ?
                        var argsAsString = t.GetProperties().Select(delegate(PropertyInfo inf) { return inf.Name; }).ToList<string>();
                        string nearest;
                        if (Tools.StringUtils.FindNearest(args[1], argsAsString, 33, out nearest))
                        {
                            error += " (Vouliez-vous dire '" + nearest + "' ?).";
                        }
                        return false;
                    }
                    

                    inter.Debug_LineNumber = CurrentLine;
                }
                catch (FormatException fe)
                {
                    error = fe.Message;
                    return false;
                }
                catch (Exception e)
                {
                    error = "[Ligne " + CurrentLine.ToString() + "] Exception lors de la création de la commande : " + e.Message;
                    return false;
                }

                return true;
            });

            m_commandSet.Add(tracker ? (track ? "@track" : "@untrack") : "@display", commandDisplay);
        }

        void CreateCommandImportFactory()
        {
            // Spawn factory
            Command commandSpawn = new Command();
            commandSpawn.Arguments = new List<Command.Arg>() { new Command.Arg("ownerId"), // nom de l'entité faisant spawn l'entité cible.
                new Command.Arg("name") };
            commandSpawn.Function = new Command.ExecuteCommandDelegate(delegate(string[] args,
                CommandInterpreter interpreter,
                out Entities.Interaction inter,
                out string error)
            {

                error = "";
                inter = Entities.Interaction.NullInteraction();
                try
                {
                    // -- Récupère l'entité.
                    int ownerId = ParseIntWithError(args[0], commandSpawn, 0);
                    var factories = LaBotlane.Instance.Scene.Map.FindEntityByType(ownerId, Entities.TargetType.Factory);
                    if(factories.Count == 0)
                    {
                        error = "[Ligne " + CurrentLine + "] Import Factory : aucune factory n'existe pour l'owner id " + ownerId.ToString() + ".";
                        return false;
                    }

                    Entities.Entity entity = factories.First();
                    inter.Debug_LineNumber = CurrentLine;

                    // Ajoute l'entité.
                    AddEntity(entity, args[1]);
                }
                catch (FormatException fe)
                {
                    error = fe.Message;
                    return false;
                }
                catch (Exception e)
                {
                    error = "[Ligne " + CurrentLine.ToString() + "] Exception lors de la création de la commande : " + e.Message;
                    return false;
                }

                return true;
            });
            m_commandSet.Add("@importfactory", commandSpawn);
        }
        /// <summary>
        /// Demande d'untrack toutes les propriétés d'une entité.
        /// Si src=all, doit untrack toutes les entités.
        /// </summary>
        void CreateCommandUntrackAll()
        {
            // Attaque
            Command commandDisplay = new Command();
            commandDisplay.Arguments = new List<Command.Arg>() { new Command.Arg("src") };
            commandDisplay.Function = new Command.ExecuteCommandDelegate(delegate(string[] args,
                CommandInterpreter interpreter,
                out Entities.Interaction inter,
                out string error)
            {
                error = "";
                inter = Entities.Interaction.NullInteraction();
                try
                {
                    if (args[0] == "all")
                    {
                        // Demande de supprimer toutes les propriétés de toutes les entités
                        PrintObject(new Tracker(null, "all", null, true));
                    }
                    else
                    {
                        // Récupération de l'entité
                        Entities.Entity srcEntity = interpreter.GetEntity(args[0]);
                        if (srcEntity == null)
                        {
                            error = "[Ligne " + CurrentLine.ToString() + "] L'entité " + args[0] + " n'existe pas.";
                            return false;
                        }


                        PrintObject(new Tracker(srcEntity, args[0], null, true));
                    }


                    inter.Debug_LineNumber = CurrentLine;
                }
                catch (FormatException fe)
                {
                    error = fe.Message;
                    return false;
                }
                catch (Exception e)
                {
                    error = "[Ligne " + CurrentLine.ToString() + "] Exception lors de la création de la commande : " + e.Message;
                    return false;
                }

                return true;
            });

            m_commandSet.Add("@untrackall", commandDisplay);
        }
        /// <summary>
        /// Tente de parser un string afin d'y trouver un int.
        /// Si le parsing échoue, renvoie une exception contenant un message approprié.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="argName"></param>
        /// <returns></returns>
        int ParseIntWithError(string s, Command cmd, int argNumber)
        {
            int val;
            if(Int32.TryParse(s.TrimStart('@'), out val))
            {
                return val;
            }
            else
            {
                throw new FormatException("[Ligne " + CurrentLine.ToString() + "]" +
                    " L'argument n° " + argNumber.ToString() + " (" + cmd.Arguments[argNumber].Name + ") doit être un entier. " +
                    "Reçu : '" + s + "'.");
            }
        }


        #endregion

        #region Output
        /// <summary>
        /// Affiche le string s dans tous les systèmes de sortie.
        /// </summary>
        /// <param name="s"></param>
        void Print(string s)
        {
            foreach(OutputSystem system in OutputSystems)
            {
                system.Print(s);
            }
        }
        /// <summary>
        /// Affiche le string s dans tous les systèmes de sortie d'erreur.
        /// </summary>
        /// <param name="s"></param>
        void PrintError(string s)
        {
            foreach (OutputSystem system in OutputSystems)
            {
                system.PrintError(s);
            }
        }

        /// <summary>
        /// Affiche le message de statut s dans tous les systèmes de sortie de statut.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="report"></param>
        void PrintStatus(string s)
        {
            foreach(OutputSystem system in OutputSystems)
            {
                system.PrintStatus(s);
            }
        }

        /// <summary>
        /// Envoie l'objet o à tous les systèmes de sortie.
        /// </summary>
        /// <param name="o"></param>
        void PrintObject(object o)
        {
            foreach(OutputSystem system in OutputSystems)
            {
                system.PrintObject(o);
            }
        }
        #endregion

        #endregion
        #endregion
    }
}