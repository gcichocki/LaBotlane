using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Labotlane.Scenes
{
    /// <summary>
    /// Singleton comportant les éléments du jeu et gérant leur coordination.
    /// </summary>
    public class Scene : IDisposable
    {
        public const int ResolutionWidth = 800;
        public const int ResolutionHeight = 600;

        #region Static
        /// <summary>
        /// Instance du Singleton de la scène.
        /// </summary>
        public static Scene Instance;
        #endregion

        #region Variables
        /// <summary>
        /// Liste des contrôleurs des joueurs.
        /// </summary>
        private List<GameComponents.Controlers.PlayerControler> m_controlers;
        /// <summary>
        /// Id du player à qui c'est le tour.
        /// </summary>
        private int m_currentPlayerId;
        /// <summary>
        /// Référence vers le moteur graphique du jeu.
        /// </summary>
        private GameComponents.Graphics.GraphicsEngine m_graphicsEngine;

        #endregion


        #region Properties
        /// <summary>
        /// Obtient ou définit la map de jeu.
        /// </summary>
        public GameComponents.GameMap Map
        {
            get;
            set;
        }

        /// <summary>
        /// Obtient ou définit une référence vers le moteur graphique de jeu utilisé.
        /// </summary>
        public GameComponents.Graphics.GraphicsEngine GraphicsEngine
        {
            get { return m_graphicsEngine; }
            private set { m_graphicsEngine = value; }
        }
        /// <summary>
        /// Obtient la liste des contrôleurs de joueur.
        /// </summary>
        public List<GameComponents.Controlers.PlayerControler> Controlers
        {
            get { return m_controlers; }
            protected set { m_controlers = value; }
        }

        /// <summary>
        /// Obtient l'id du joueur entrain de jouer.
        /// </summary>
        public int CurrentPlayerId
        {
            get { return m_currentPlayerId; }
            private set { m_currentPlayerId = value; }
        }

        /// <summary>
        /// Obtient ou définit le temps de jeu.
        /// Permet d'accéder à cette variable sans surcharger le nombre d'arguments passés aux fonctions.
        /// </summary>
        public GameTime Time
        {
            get;
            private set;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Crée une nouvelle instance de Scene.
        /// </summary>
        public Scene()
        {
            // On n'appelle pas initialize ici car certains objets ont besoin d'une instance de Scene pour s'initialiser.
        }

        /// <summary>
        /// Initialise la Scene.
        /// </summary>
        public void Initialize()
        {
            Instance = this;
            Map = new GameComponents.GameMap();

            // Moteur graphique
            GraphicsEngine = new GameComponents.Graphics.GraphicsEngine2D(LaBotlane.Instance.GraphicsDevice);

            // Ajout de controleurs pour les joueurs.
            Controlers = new List<GameComponents.Controlers.PlayerControler>();
            Controlers.Add(new GameComponents.Controlers.HumanPlayerControler(0));
            Controlers.Add(new GameComponents.Controlers.TestControler(1));

            // Charge une map
            Map.LoadFromFile(@"Content\maps\battlefield.txt");
        }

        /// <summary>
        /// Mets à jour la scène.
        /// </summary>
        /// <param name="batch"></param>
        public void Update(GameTime time)
        {
            Time = time;

            // Mets à jour les composants du moteur graphique
            GraphicsEngine.Update(time);

            // Mets à jour le contrôleur du joueur qui est entrain de jouer.
            Controlers[CurrentPlayerId].Update(time);

            // Termine le tour du joueur.
            if (Controlers[CurrentPlayerId].TurnEnded)
            {
                CurrentPlayerId = (CurrentPlayerId + 1) % Controlers.Count;
                Controlers[CurrentPlayerId].StartTurn();
                Map.TurnStarted(CurrentPlayerId);
            }

            // Mise à jour de la map et de ses éléments.
            Map.Update(time);
        }

        /// <summary>
        /// Dessine la scène.
        /// </summary>
        /// <param name="batch"></param>
        public void Draw(SpriteBatch batch, GameTime time)
        {
            // Dessine la map.
            GraphicsEngine.DrawMap(Map, time);

            // Dessine la GUI.
            GraphicsEngine.DrawGUI(time);
        }

        #endregion

        #region IDisposable
        /// <summary>
        /// Supprime toutes les ressources non managées allouées par la Scene.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Supprime toutes les ressources non managées allouées par la Scene.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                GraphicsEngine.Dispose();
            }
        }
        #endregion
    }
}
