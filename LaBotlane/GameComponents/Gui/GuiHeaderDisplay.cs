using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace Labotlane.GameComponents.Gui
{
    /// <summary>
    /// Widget permettant l'affichage du HUD sur la map.
    /// </summary>
    public class GuiHeaderDisplay : GuiWidget
    {
        #region Variables

        #endregion

        #region Properties

        #endregion

        #region Methods
        /// <summary>
        /// Mets à jour la logique du Widget.
        /// </summary>
        /// <param name="time"></param>
        public override void Update(GameTime time)
        {

        }
        /// <summary>
        /// Dessine le widget.
        /// </summary>
        /// <param name="batch"></param>
        public override void Draw(SpriteBatch batch)
        {
            // Calcul de la hauteur d'une "box" de la GUI.
            int guiHeight = 60;
            int guiWidth = Scenes.Scene.ResolutionWidth / LaBotlane.Instance.Scene.Controlers.Count;
            int marginSize = 5;

            // Récupération de ressources
            SpriteFont font = LaBotlane.Instance.Ressources.Font;
            // Dessin des interfaces.
            List<GameComponents.Controlers.PlayerControler> controlers = LaBotlane.Instance.Scene.Controlers;
            int playerId = 0;
            foreach(GameComponents.Controlers.PlayerControler controler in controlers)
            {
                GameComponents.Player player = controler.Player;
                int baseAlpha = 75;
                int textAlpha = 255;

                // Dessine le rectangle contenant les données sur le joueur.
                Rectangle box = new Rectangle(playerId*guiWidth+marginSize, marginSize, guiWidth-2*marginSize, guiHeight-2*marginSize);
                Drawing.DrawRectBox(batch, 
                    LaBotlane.Instance.Ressources.Menu, 
                    box,
                    new Color(255, 255, 255, baseAlpha),
                    Ressources.GUI_DEPTH-0.01f);

                // Dessine les informations sous forme de texte.
                // ---
                Color baseColor = Color.Black;
                Color color = new Color(baseColor.R, baseColor.G, baseColor.B, textAlpha);
                // Nom du joueur.
                string playerName = player.Nickname;
                Vector2 nameSize = font.MeasureString(playerName);
                Vector2 position = new Vector2(box.X + (box.Width - nameSize.X) / 2, box.Y + marginSize);
                batch.DrawString(font, playerName, position, color);

                // Gold du joueur.
                position.X = box.X + marginSize;
                position.Y += nameSize.Y;
                batch.DrawString(font, "G : " + player.Gold.ToString(), position, color);

                // Points du joueur.
                position.X += box.Width / 2;
                batch.DrawString(font, "Score : " + player.Score.ToString(), position, color);

                playerId++;
            }
        }
        #endregion
    }
}
