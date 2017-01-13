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
    public class HumanPlayerControler : PlayerControler
    {
        #region Variables
        /// <summary>
        /// Représente le menu de GUI utilisé pour afficher les intéractions.
        /// Seule la dernière instance utilisée est mémorisée afin de connaître son état.
        /// </summary>
        Gui.GuiMenu m_interactionsMenu;
        #endregion

        #region Properties

        #endregion

        #region Methods
        /// <summary>
        /// Crée une nouvelle instance de PlayerControler.
        /// </summary>
        /// <param name="playerId"></param>
        public HumanPlayerControler(int playerId) : base(playerId)
        {
            Player.Gold = 1000;
        }

        /// <summary>
        /// Mets à jour la caméra.
        /// Corrige la position de la souris pour qu'elle ne sorte pas de l'écran et
        /// fait scroller la map si la souris est sur le bord.
        /// </summary>
        /// <param name="time"></param>
        void UpdateCamera(GameTime time)
        {

            MouseState state = Mouse.GetState();
            GameMap.Position newMousePos = GameMap.Position.FromScreenSpace(state.X, state.Y).InScreenRange();
            Mouse.SetPosition((int)newMousePos.ScreenSpace.X, (int)newMousePos.ScreenSpace.Y);
            if (newMousePos.ScreenSpace.X <= 1)
                LaBotlane.Instance.Scene.Map.ScrollLeft(time);
            else if (newMousePos.ScreenSpace.X >= Scenes.Scene.ResolutionWidth - 1)
                LaBotlane.Instance.Scene.Map.ScrollRight(time);
            if (newMousePos.ScreenSpace.Y <= 1)
                LaBotlane.Instance.Scene.Map.ScrollUp(time);
            else if (newMousePos.ScreenSpace.Y >= Scenes.Scene.ResolutionHeight - 1)
                LaBotlane.Instance.Scene.Map.ScrollDown(time);

        }
        /// <summary>
        /// Mets à jour le contrôleur du joueur.
        /// Appelé lorsque c'est au tour du joueur contrôlé par ce contrôleur.
        /// </summary>
        /// <param name="time"></param>
        public override void Update(GameTime time)
        {
            // Gestion de la fin du tour.
            if(Input.IsTrigger(Microsoft.Xna.Framework.Input.Keys.Tab))
                EndTurn();

            
            // Mets à jour la caméra.
            UpdateCamera(time);

            // Gestion de la sélection d'une entité.
            MouseState state = Mouse.GetState();
            if(Input.IsLeftClickTrigger())
            {
                GameMap.Position position = GameMap.Position.FromScreenSpace(state.X, state.Y);
                if (!position.IsInMap())
                    return;

                Entities.Entity clickedEntity = LaBotlane.Instance.Scene.Map.GetEntityAt(position, PlayerId);
                ControledEntity = clickedEntity;
            }

            // Gestion d'une action sur une entité.
            if(Input.IsRightClickTrigger() && ControledEntity != null && (m_interactionsMenu == null || m_interactionsMenu.IsDisposed) )
            {
                GameMap.Position clickPosition = GameMap.Position.FromScreenSpace(state.X, state.Y);

                // Ajoute toutes les intéractions au menu.
                List<Entities.Interaction> interactions = ControledEntity.GetInteractions(LaBotlane.Instance.Scene.Map.GetTileAt(clickPosition));
                if (interactions.Count != 0)
                {
                    m_interactionsMenu = new Gui.GuiMenu();
                    m_interactionsMenu.Title = ControledEntity.GetType().Name.ToString();

                    foreach (Entities.Interaction interaction in interactions)
                    {
                        m_interactionsMenu.AddItem(new Gui.GuiMenu.GuiMenuItem(interaction.Description,
                            GetInteractionIcon(interaction),
                            IsInteractionPossible(interaction),
                            delegate() { ApplyInteraction(interaction); }));
                    }

                    // Menu
                    m_interactionsMenu.Position = clickPosition;
                    Entities.Entity entityRef = ControledEntity;
                    LaBotlane.Instance.Scene.GraphicsEngine.Gui.AddWidget(m_interactionsMenu);
                }
            }
        }

        #endregion
    }
}
