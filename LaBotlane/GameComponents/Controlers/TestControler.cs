using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
namespace Labotlane.GameComponents.Controlers
{
    /// <summary>
    /// Classe de base permettant le contrôle d'un joueur.
    /// </summary>
    public class TestControler : PlayerControler
    {
        #region Variables
        /// <summary>
        /// Interpréteur de commandes.
        /// </summary>
        Interpreter.CommandInterpreter m_interpreter;

        /// <summary>
        /// Dispatcher de messages de l'interpréteur.
        /// </summary>
        Interpreter.MessageDispatcher m_dispatcher;

        /// <summary>
        /// Console permettant d'entrer des commandes.
        /// </summary>
        Gui.GuiTextInput m_consoleInput;

        /// <summary>
        /// Sortie de certains messages de la console.
        /// </summary>
        Gui.GuiMultilineTextDisplay m_consoleOutput;

        /// <summary>
        /// Sortie des messages de type track de l'interpréteur.
        /// </summary>
        Gui.GuiMultilineTextDisplay m_trackerOutput;

        /// <summary>
        /// Liste de traqueurs de variables à afficher sur la console.
        /// </summary>
        List<Interpreter.CommandInterpreter.Tracker> m_trackers;
        #endregion

        #region Properties

        #endregion

        #region Methods
        /// <summary>
        /// Crée une nouvelle instance de PlayerControler.
        /// </summary>
        /// <param name="playerId"></param>
        public TestControler(int playerId) : base(playerId)
        {
            Player.Gold = 1000;
            m_interpreter = new Interpreter.CommandInterpreter();
            m_consoleInput = new Gui.GuiTextInput();
            m_dispatcher = new Interpreter.MessageDispatcher(m_interpreter);
            m_consoleOutput = new Gui.GuiMultilineTextDisplay();
            m_trackerOutput = new Gui.GuiMultilineTextDisplay();
            m_trackers = new List<Interpreter.CommandInterpreter.Tracker>();
            InitializeConsoleEvents();
            LaBotlane.Instance.Scene.GraphicsEngine.Gui.AddWidget(m_consoleInput);
            LaBotlane.Instance.Scene.GraphicsEngine.Gui.AddWidget(m_consoleOutput);
            LaBotlane.Instance.Scene.GraphicsEngine.Gui.AddWidget(m_trackerOutput);
        }

        /// <summary>
        /// Initialise les évènements liés à la console.
        /// </summary>
        void InitializeConsoleEvents()
        {
            m_consoleInput.TextValidated += delegate(Gui.GuiTextInput console)
            {
                string scriptTmp = console.Text;
                console.Text = "";
                ExecuteScript(scriptTmp, m_interpreter, false);
            };

            m_dispatcher.OnErrorMessage += delegate(string s) { m_consoleOutput.AppendLine(s.Replace("[Ligne 1]", "")); };
            m_dispatcher.OnMessage += delegate(string s) { m_consoleOutput.AppendLine(s.Replace("[Ligne 1]", "")); ; };
            m_dispatcher.OnObject += delegate(object o)
            {
                if (o is Interpreter.CommandInterpreter.Tracker)
                    ProcessTracker((Interpreter.CommandInterpreter.Tracker)o);
            };
        }

        #region Trackers
        /// <summary>
        /// Traite le message de traqueur envoyé par l'interpréteur.
        /// </summary>
        /// <param name="tracker"></param>
        void ProcessTracker(Interpreter.CommandInterpreter.Tracker tracker)
        {
            if(tracker.Delete)
            {
                // Recherche du tracker à supprimer.
                List<Interpreter.CommandInterpreter.Tracker> toDelete = new List<Interpreter.CommandInterpreter.Tracker>(); 
                foreach(Interpreter.CommandInterpreter.Tracker t in m_trackers)
                {
                    if (t.Match(tracker))
                    {
                        toDelete.Add(t);
                    }
                }

                // Suppression.
                foreach(Interpreter.CommandInterpreter.Tracker d in toDelete)
                    m_trackers.Remove(d);
            }
            else
            {
                m_trackers.Add(tracker);
            }
        }

        /// <summary>
        /// Mets à jour la console des traqueurs.
        /// </summary>
        void UpdateTrackers()
        {
            StringBuilder builder = new StringBuilder();
            foreach(Interpreter.CommandInterpreter.Tracker tracker in m_trackers)
            {
                builder.AppendLine(tracker.OwnerName + "." + tracker.Property.Name + " = " + tracker.GetValue().ToString());
            }
            m_trackerOutput.Clear();
            m_trackerOutput.Append(builder.ToString());
        }
        #endregion
        /// <summary>
        /// Mets à jour la caméra.
        /// Corrige la position de la souris pour qu'elle ne sorte pas de l'écran et
        /// fait scroller la map si la souris est sur le bord.
        /// </summary>
        /// <param name="time"></param>
        void UpdateCamera(GameTime time)
        {
            // Centre la caméra sur l'entité contrôlée.
            if(ControledEntity != null)
                LaBotlane.Instance.Scene.Map.CenterOnPosition(ControledEntity.Position);
        }

        bool m_hasStarted = false;
        /// <summary>
        /// Mets à jour le contrôleur du joueur.
        /// Appelé lorsque c'est au tour du joueur contrôlé par ce contrôleur.
        /// </summary>
        /// <param name="time"></param>
        public override void Update(GameTime time)
        {
            // Gestion de la fin du tour.
            if(Input.IsTrigger(Microsoft.Xna.Framework.Input.Keys.Tab) )
            {
                m_hasStarted = false;
                EndTurn();
                return;
            }

            // Mets à jour la caméra.
            UpdateCamera(time);
            UpdateConsole();
            UpdateTrackers();

            if(!m_hasStarted)
            {
                GameMap map = LaBotlane.Instance.Scene.Map;
                m_hasStarted = true;

                // utilisation de l'interpréteur
                m_interpreter.CurrentOwnerId = 1;

                // ExecuteScriptFile("test1.txt", m_interpreter);
            }
        }

        /// <summary>
        /// Mets à jour la console.
        /// </summary>
        void UpdateConsole()
        {
            m_consoleInput.Position = GameMap.Position.FromScreenSpace(0, Scenes.Scene.ResolutionHeight - 20);
            m_consoleInput.Size = new Point(Scenes.Scene.ResolutionWidth, 20);

            m_consoleOutput.Position = GameMap.Position.FromScreenSpace(0, Scenes.Scene.ResolutionHeight - 220);
            m_consoleOutput.Size = new Point(2*Scenes.Scene.ResolutionWidth/3, 200);

            m_trackerOutput.Position = GameMap.Position.FromScreenSpace(2 * Scenes.Scene.ResolutionWidth / 3, Scenes.Scene.ResolutionHeight - 220);
            m_trackerOutput.Size = new Point(Scenes.Scene.ResolutionWidth/3, 200);
        }

        #endregion

        #region Start / End Turn
        /// <summary>
        /// Effectue les actions de fin de tour.
        /// </summary>
        public override void EndTurn()
        {
            base.EndTurn();
            m_consoleInput.IsVisible = false;
            m_consoleInput.HasFocus = false;
            m_consoleOutput.IsVisible = false;
            m_trackerOutput.IsVisible = false;
        }

        /// <summary>
        /// Effectue les actions de commencement de tour.
        /// </summary>
        public override void StartTurn()
        {
            base.StartTurn();
            m_consoleInput.IsVisible = true;
            m_consoleInput.HasFocus = true;
            m_consoleOutput.IsVisible = true;
            m_trackerOutput.IsVisible = true;
        }
        #endregion
    }
}
