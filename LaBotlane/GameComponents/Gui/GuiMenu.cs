using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
namespace Labotlane.GameComponents.Gui
{
    /// <summary>
    /// Représente un menu dans la GUI.
    /// </summary>
    public class GuiMenu : GuiWidget
    {
        #region Delegate / Events / Classes
        public delegate void ItemSelectedDelegate();

 
        /// <summary>
        /// Représente un item du menu.
        /// </summary>
        public class GuiMenuItem
        {
            /// <summary>
            /// Texte affiché par cet item.
            /// </summary>
            public string Text { get; set; }
            /// <summary>
            /// Icône affiché sur cet item.
            /// </summary>
            public Texture2D Icon { get; set; }
            /// <summary>
            /// Id de l'event déclenché par cet item.
            /// </summary>
            public event ItemSelectedDelegate ItemSelected;
            /// <summary>
            /// Retourne vrai si l'item est activé (cliquable).
            /// </summary>
            public bool IsEnabled { get; set; }
            /// <summary>
            /// Dispatche l'event ItemSelected.
            /// </summary>
            public void DispatchItemSelectedEvent()
            {
                if (ItemSelected != null && IsEnabled)
                    ItemSelected();
            }

            public GuiMenuItem()
            {
                Text = "";
                Icon = null;
                IsEnabled = true;
            }

            public GuiMenuItem(string text)
            {
                Text = text;
                Icon = null;
                IsEnabled = true;
            }

            public GuiMenuItem(string text, Texture2D icon)
            {
                Text = text;
                Icon = icon;
                IsEnabled = true;
            }

            public GuiMenuItem(string text, Texture2D icon, bool isEnabled)
            {
                Text = text;
                Icon = icon;
                IsEnabled = isEnabled;
            }

            public GuiMenuItem(string text, Texture2D icon, bool isEnabled, ItemSelectedDelegate onItemSelected)
            {
                Text = text;
                Icon = icon;
                ItemSelected += onItemSelected;
                IsEnabled = isEnabled;
            }
        }
        #endregion

        #region Variables
        /// <summary>
        /// Id de l'item sur lequel est positionnée la souris.
        /// -1 pour aucun.
        /// </summary>
        protected int m_hoverItemId;
        bool firstFrame = true;
        /// <summary>
        /// Taille de la barre de titre.
        /// </summary>
        int TitleBarSize = 35;
        /// <summary>
        /// Taille de la marge globale du menu.
        /// </summary>
        int MainMarginSize = 8;
        /// <summary>
        /// Taille des marges des items en pixels.
        /// </summary>
        int ItemMarginSize = 3;
        /// <summary>
        /// Largeur d'un item en pixels.
        /// </summary>
        int m_itemWidth;
        /// <summary>
        /// Hauteur d'un item en pixels.
        /// </summary>
        int m_itemHeight;
        /// <summary>
        /// Taille des icones en pixels.
        /// Les icones sont des textures carrées.
        /// </summary>
        int m_iconSize = 16;
        /// <summary>
        /// Liste des items du menu.
        /// </summary>
        List<GuiMenuItem> m_items;
        #endregion

        #region Properties
        /// <summary>
        /// Définit la liste des items de ce menu.
        /// 
        /// /!\ Ecriture seule.
        /// </summary>
        public List<GuiMenuItem> Items
        {
            private get { return m_items; }
            set { m_items = value; ComputeItemsSize(); }
        }

        /// <summary>
        /// Ajoute l'item donné à la liste des items.
        /// </summary>
        /// <param name="item"></param>
        public void AddItem(GuiMenuItem item)
        {
            m_items.Add(item);
            ComputeItemsSize();
        }

        /// <summary>
        /// Obtient ou définit le titre du menu.
        /// </summary>
        public string Title
        {
            get;
            set;
        }
        #endregion 

        #region Methods
        /// <summary>
        /// Crée une nouvelle instance de GuiMenu.
        /// </summary>
        public GuiMenu() : base()
        {
            Items = new List<GuiMenuItem>();
            m_hoverItemId = -1;
            Title = "Sans titre";
        }

        /// <summary>
        /// Calcule la taille des items du menu.
        /// </summary>
        void ComputeItemsSize()
        {
            // Détermine la largeur du menu en fonction
            // de l'élément à la plus grande taille.
            int maxStringWidth = 120;
            int maxStringHeight = 20;

            int maxIconWidth = 0;
            for (int y = 0; y < Items.Count; y++)
            {
                SpriteFont font = LaBotlane.Instance.Ressources.Font;
                Vector2 size = font.MeasureString(Items[y].Text);
                
                if(Items[y].Icon != null)
                {
                    int w = Items[y].Icon.Width;
                    if (w > maxIconWidth)
                        maxIconWidth = w;
                }

                if (size.X > maxStringWidth)
                {
                    maxStringWidth = (int)size.X;
                }

                if (size.Y > maxStringHeight)
                {
                    maxStringHeight = (int)size.Y;
                }
            }

            
            m_itemWidth = maxStringWidth + ItemMarginSize*2 + m_iconSize + ItemMarginSize;
            m_itemHeight = maxStringHeight + ItemMarginSize*2;
        }
        /// <summary>
        /// Mets à jour le menu.
        /// </summary>
        /// <param name="time"></param>
        public override void Update(Microsoft.Xna.Framework.GameTime time)
        {
            // Retourne si première frame : évite certains artifacts de clic.
            if(firstFrame)
            {
                firstFrame = false;
                return;
            }

            m_hoverItemId = -1;
            MouseState state = Mouse.GetState();
            for(int y = 0; y < Items.Count; y++)
            {
                GameMap.Position pos = Position + GameMap.Position.FromPixels(0, y * m_itemHeight);
                Rectangle pxRect = new Rectangle((int)pos.ScreenSpace.X, (int)pos.ScreenSpace.Y, m_itemWidth, m_itemHeight);
                Point mousePos = new Point(state.X, state.Y);
                if (pxRect.Contains(mousePos))
                {
                    m_hoverItemId = y;

                }
            }

            if (Input.IsLeftClickTrigger())
            {
                // Envoie un signal indiquant que la sélection a changé si le menu est cliqué.
                if (m_hoverItemId != -1)
                {
                    Items[m_hoverItemId].DispatchItemSelectedEvent();
                }

                Input.Update();
                // Supprime le menu.
                Dispose();
            }
        }

        /// <summary>
        /// Dessine les items de ce menu.
        /// </summary>
        /// <param name="batch"></param>
        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch batch)
        {
            
            GameMap.Position pos = Position + GameMap.Position.FromPixels(-MainMarginSize, -TitleBarSize);
            Rectangle menuBox = new Rectangle((int)pos.ScreenSpace.X, (int)pos.ScreenSpace.Y, m_itemWidth + MainMarginSize * 2, m_itemHeight * Items.Count + TitleBarSize + MainMarginSize);
            
            // Dessine la boite principale du menu.
            Drawing.DrawRectBox(batch, LaBotlane.Instance.Ressources.Menu,
                menuBox,
                Color.White,
                Ressources.GUI_DEPTH+0.05f);

            Vector2 tSize = LaBotlane.Instance.Ressources.Font.MeasureString(Title);
            pos = (GameMap.Position.FromScreenSpace(menuBox.X, menuBox.Y) 
                + GameMap.Position.FromPixels(menuBox.Width, TitleBarSize) / 2
                - GameMap.Position.FromPixels(tSize.X/2, tSize.Y/2));

            // Dessine le titre du menu.
            batch.DrawString(LaBotlane.Instance.Ressources.Font, Title, 
                pos.ScreenSpace,
                Color.White,
                0.0f,
                Vector2.Zero, 
                1.0f,
                SpriteEffects.None,
                Ressources.GUI_DEPTH);

            for (int y = 0; y < Items.Count; y++)
            {
                pos = Position + GameMap.Position.FromPixels(0, y * m_itemHeight);
                tSize = LaBotlane.Instance.Ressources.Font.MeasureString(Items[y].Text);
                Rectangle pxRect = new Rectangle((int)pos.ScreenSpace.X, (int)pos.ScreenSpace.Y, m_itemWidth, m_itemHeight);
                Texture2D t = m_hoverItemId == y && Items[y].IsEnabled ? LaBotlane.Instance.Ressources.MenuItemHover : LaBotlane.Instance.Ressources.MenuItem;
                Drawing.DrawRectBox(batch, t, pxRect, Color.White, Ressources.GUI_DEPTH+0.01f);
                //batch.Draw(t, pxRect, null, Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 0.1f);

                if(Items[y].Icon != null)
                {
                    Rectangle dstRect = new Rectangle(pxRect.X + ItemMarginSize, pxRect.Y + (pxRect.Height - m_iconSize) / 2, m_iconSize, m_iconSize);
                    Color color = Items[y].IsEnabled ? Color.White : new Color(125, 125, 125, 125);
                    batch.Draw(Items[y].Icon, dstRect, null, color, 0.0f, Vector2.Zero, SpriteEffects.None, Ressources.GUI_DEPTH);
                }

                pos = GameMap.Position.FromScreenSpace(pxRect.X + 2*ItemMarginSize + m_iconSize, pxRect.Y + pxRect.Height/2 - tSize.Y/2);
                Color textColor = Items[y].IsEnabled ? Color.White : Color.Gray;
                batch.DrawString(LaBotlane.Instance.Ressources.Font, Items[y].Text, pos.ScreenSpace, textColor, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, Ressources.GUI_DEPTH);
            }
        }
        #endregion
    }
}
