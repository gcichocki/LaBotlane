﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace Labotlane.GameComponents.Particles
{
    /// <summary>
    /// Classe représentant une particule gérant les effets et affichant uniquement du texte.
    /// </summary>
    public class ParticleText : ParticleBase
    {

        #region Variables
        
        #endregion

        #region Properties
        /// <summary>
        /// Obtient ou définit le texte affiché par la particule.
        /// </summary>
        public string Text
        {
            get;
            set;
        }

        /// <summary>
        /// Obtient ou définit la police utilisée pour afficher le texte.
        /// </summary>
        public SpriteFont Font
        {
            get;
            set;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Crée une nouvelle instance de ParticleText.
        /// </summary>
        public ParticleText(GameTime time) : base(time)
        {
            Font = LaBotlane.Instance.Ressources.Font;
            Text = "";
        }
        /// <summary>
        /// Mets à jour la particule.
        /// </summary>
        public override void Update(GameTime time)
        {
            base.Update(time);
        }
        /// <summary>
        /// Dessine la particule à l'écran.
        /// </summary>
        /// <param name="batch"></param>
        public override void Draw(SpriteBatch batch)
        {
            batch.DrawString(Font, Text, CurrentPosition.ScreenSpace, CurrentColor, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, Ressources.PARTICLES_DEPTH);
        }
        /// <summary>
        /// Libère la mémoire utilisée par cette particule.
        /// </summary>
        public override void Dispose()
        {

        }
        /// <summary>
        /// Variable indiquant si la particule a été supprimée.
        /// </summary>
        public override bool IsDisposed
        {
            get;
            set;
        }
        #endregion
    }
}
