using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace Labotlane.GameComponents
{
    public sealed class Ressources
    {
        public const float GUI_DEPTH = 0.18f;
        public const float PARTICLES_DEPTH = 0.21f;
        public const float UNITS_DEPTH = 0.5f;
        public const float GROUND_DEPTH = 1.0f;
        public const float DEPTH_STEP_Y = 0.01f;
        /// <summary>
        /// Obtient une police de caractère utilisable pour l'affichage de texte.
        /// </summary>
        public SpriteFont Font
        {
            get;
            private set;
        }

        public SpriteFont CourrierFont
        {
            get;
            private set;
        }
        /// <summary>
        /// Obtient une texture représentant une marque de sélection.
        /// </summary>
        public Texture2D SelectMark
        {
            get;
            private set;
        }

        public Texture2D HighlightMark
        {
            get;
            private set;
        }
        public Texture2D MenuItem
        {
            get;
            private set;
        }

        public Texture2D MenuItemHover
        {
            get;
            private set;
        }

        public Texture2D TextBox
        {
            get;
            set;
        }

        public Texture2D Menu
        {
            get;
            private set;
        }

        public Texture2D Cursor
        {
            get;
            private set;
        }

        public Texture2D IconAttack
        {
            get;
            private set;
        }

        public Texture2D IconMove
        {
            get;
            private set;
        }

        /// <summary>
        /// Matrix de transformation permettant la projection de sprites 2D.
        /// </summary>
        public Matrix PlaneTransform2D
        {
            get;
            set;
        }
        /// <summary>
        /// Matrix de transformation permettant la projection en mode 7.
        /// </summary>
        public Matrix Mode7Transform2D
        {
            get;
            set;
        }
        /// <summary>
        /// Charge des ressources communes.
        /// </summary>
        public Ressources()
        {
            Font = LaBotlane.Instance.Content.Load<SpriteFont>("segoe_ui_16");
            SelectMark = LaBotlane.Instance.Content.Load<Texture2D>("textures/select_mark");
            MenuItem = LaBotlane.Instance.Content.Load<Texture2D>("textures/gui/menu_item");
            MenuItemHover = LaBotlane.Instance.Content.Load<Texture2D>("textures/gui/menu_item_hover");
            Menu = LaBotlane.Instance.Content.Load<Texture2D>("textures/gui/menu");
            Cursor = LaBotlane.Instance.Content.Load<Texture2D>("textures/gui/cursor");
            IconAttack = LaBotlane.Instance.Content.Load<Texture2D>("textures/icons/attack");
            IconMove = LaBotlane.Instance.Content.Load<Texture2D>("textures/icons/move");
            HighlightMark = LaBotlane.Instance.Content.Load<Texture2D>("textures/highlight_mark");
            TextBox = LaBotlane.Instance.Content.Load<Texture2D>("textures/gui/textbox");
            CourrierFont = LaBotlane.Instance.Content.Load<SpriteFont>("courrier-16pt");
            // Matrice de transformation pour les planes 2D.
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Scenes.Scene.ResolutionWidth, Scenes.Scene.ResolutionHeight, 0, 0, 1);
            Matrix halfPixelOffset = Matrix.CreateTranslation(-0.5f, -0.5f, -1);
            PlaneTransform2D = halfPixelOffset * projection;

            // Projection
            float aspectRatio = (float)Scenes.Scene.ResolutionWidth / (float)Scenes.Scene.ResolutionHeight;
            float fov = MathHelper.PiOver4 * aspectRatio * 0.75f;//0.75f;//3 / 4;
            var proj = Matrix.CreatePerspectiveFieldOfView(fov, aspectRatio, 0.01f, 100);
            Mode7Transform2D =  Matrix.CreateLookAt(new Vector3(-1.8f, -0.8f, -10), new Vector3(0, 0, -5), new Vector3(0, 0, -1)) * proj;
            // Mode7TransformD = PlaneTransform2D;
        }
    }
}
