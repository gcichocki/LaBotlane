﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace Labotlane.GameComponents.Particles
{
    /// <summary>
    /// Classe représentant une particule gérant les effets :
    ///     - Le déplacement des particules.
    ///     - Durée de vie, effets de fade in / fade out.
    /// </summary>
    public abstract class ParticleBase : Particle
    {
        #region Static
        #region Création de mouvements
        /// <summary>
        /// Retourne une fonction de mouvement rectiligne crée à partir de la position de fin de la particule.
        /// </summary>
        /// <param name="endPos"></param>
        /// <returns></returns>
        public static MoveDelegate MoveLine(GameMap.Position endPos)
        {
            MoveDelegate del = delegate(ParticleBase thisParticle, GameTime time)
            {
                TimeSpan start = thisParticle.CreationTime.TotalGameTime;
                TimeSpan end = time.TotalGameTime;
                TimeSpan span = end - start;
                float percentLifetime = (float)span.TotalSeconds / thisParticle.DurationSeconds;
                GameMap.Position pos = thisParticle.StartPosition * (1 - percentLifetime) + endPos * percentLifetime;
                return pos;
            };
            return del;
        }
        #endregion
        #endregion
        #region Delegates
        /// <summary>
        /// Prototype de fonction permettant le déplacement d'une particule.
        /// </summary>
        /// <param name="thisParticle">Particule devant être déplacée.</param>
        /// <param name="time">Collection de données temporelles sur le jeu.</param>
        /// <returns>La nouvelle position de la particule.</returns>
        public delegate GameMap.Position MoveDelegate(ParticleBase thisParticle, GameTime time);


        #endregion

        #region Variables

        #region Fade In / Out
        /// <summary>
        /// Temps auquel la particule a été créée.
        /// </summary>
        protected GameTime m_creationTime;


        /// <summary>
        /// Couleur au début du fade in.
        /// </summary>
        protected Color m_fadeInStartColor;

        /// <summary>
        /// Couleur à la fin du fade in.
        /// </summary>
        protected Color m_fadeBaseColor;

        /// <summary>
        /// Couleur à la fin du fade out.
        /// </summary>
        protected Color m_fadeOutEndColor;

        /// <summary>
        /// Délai en % du temps de vie avant le début du fade-in.
        /// </summary>
        protected float m_fadeInDelay = 0.0f;

        /// <summary>
        /// Durée en % du temps de vie du fade in.
        /// </summary>
        protected float m_fadeInDuration = 0.1f;

        /// <summary>
        /// Durée en % du temps de vie du fade out.
        /// </summary>
        protected float m_fadeOutDuration = 0.0f;

        /// <summary>
        /// Durée de vie de la particule en secondes.
        /// </summary>
        protected float m_durationSeconds;

        #endregion

        #region Position / Déplacement
        /// <summary>
        /// Position de départ de la particule.
        /// </summary>
        protected GameMap.Position m_startPosition;
 
        /// <summary>
        /// Position courante de la particule.
        /// </summary>
        protected GameMap.Position m_currentPosition;

        /// <summary>
        /// Fonction permettant le déplacement de la particule.
        /// </summary>
        protected MoveDelegate m_moveFunction;

        #endregion

        #endregion

        #region Properties
        /// <summary>
        /// Obtient ou définit la date de création de cette particule.
        /// </summary>
        public GameTime CreationTime
        {
            get { return m_creationTime; }
            set { m_creationTime = value; }
        }
        /// <summary>
        /// Obtient ou définit la couleur au début du fade-in.
        /// </summary>
        public Color FadeInStartColor
        {
            get { return m_fadeInStartColor; }
            set { m_fadeInStartColor = value; }
        }
        /// <summary>
        /// Obtient ou définit la couleur de fin du fade-in.
        /// </summary>
        public Color FadeBaseColor
        {
            get { return m_fadeBaseColor; }
            set { m_fadeBaseColor = value; }
        }
        /// <summary>
        /// Obtient ou définit la couleur de fin du fade-out.
        /// </summary>
        public Color FadeOutEndColor
        {
            get { return m_fadeOutEndColor; }
            set { m_fadeOutEndColor = value; }
        }
        /// <summary>
        /// Obtient ou définit le délai en % du temps de vie avant le début du fade-in.
        /// </summary>
        public float FadeInDelay
        {
            get { return m_fadeInDelay; }
            set { m_fadeInDelay = value; }
        }
        /// <summary>
        /// Obtient ou définit la durée en % du temps de vie du fade in.
        /// </summary>
        public float FadeInDuration
        {
            get { return m_fadeInDuration; }
            set { m_fadeInDuration = value; }
        }
        /// <summary>
        /// Obtient ou définit la durée en % du temps de vie du fade out.
        /// </summary>
        public float FadeOutDuration
        {
            get { return m_fadeOutDuration; }
            set { m_fadeOutDuration = value; }
        }
        /// <summary>
        /// Obtient ou définit la durée de vie de la particule en secondes.
        /// </summary>
        public float DurationSeconds
        {
            get { return m_durationSeconds; }
            set { m_durationSeconds = value; }
        }
        /// <summary>
        /// Obtient ou définit la position de départ de la particule.
        /// </summary>
        public GameMap.Position StartPosition
        {
            get { return m_startPosition; }
            set { m_startPosition = value; }
        }

        /// <summary>
        /// Obtient ou définit la fonction permettant le déplacement de la particule.
        /// </summary>
        public MoveDelegate MoveFunction
        {
            get { return m_moveFunction; }
            set { m_moveFunction = value; }
        }
        /// <summary>
        /// Obtient ou définit la position courante de la particule.
        /// </summary>
        public GameMap.Position CurrentPosition
        {
            get { return m_currentPosition; }
            set { m_currentPosition = value; }
        }
        /// <summary>
        /// Obtient ou définit la couleur courante.
        /// </summary>
        public Color CurrentColor
        {
            get;
            set;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Crée une nouvelle instance de ParticleText.
        /// </summary>
        public ParticleBase(GameTime time)
        {
            CreationTime = new GameTime(time.TotalGameTime, time.ElapsedGameTime, time.IsRunningSlowly);
            DurationSeconds = 2.0f;
            FadeBaseColor = new Color(255, 125, 0, 255);
            FadeInStartColor = new Color(255, 125, 0, 0);
            FadeOutEndColor = new Color(0, 0, 0, 0);
            FadeInDelay = 0.0f;
            FadeInDuration = 0.3f;
            FadeOutDuration = 0.3f;
        }
        /// <summary>
        /// Mets à jour la particule.
        /// </summary>
        public override void Update(GameTime time)
        {
            // Déplace la particule.
            if(MoveFunction != null)
            {
                CurrentPosition = MoveFunction(this, time);
            }
            // Mise à jour du fade out.
            UpdateFade(time);
        }
        /// <summary>
        /// Mets à jour le fade-in / fade-out.
        /// </summary>
        /// <param name="time"></param>
        protected void UpdateFade(GameTime time)
        {
            TimeSpan start = CreationTime.TotalGameTime;
            TimeSpan end = time.TotalGameTime;
            TimeSpan span = end - start;
            float percentLifetime = (float)span.TotalSeconds / DurationSeconds;

            if(percentLifetime < FadeInDelay)
            {
                CurrentColor = FadeInStartColor;
            }
            else if(percentLifetime < FadeInDuration+FadeInDelay)
            {
                float interpFactor = (percentLifetime - FadeInDelay)/FadeInDuration;
                CurrentColor = new Color((int)(FadeInStartColor.R * (1-interpFactor) + FadeBaseColor.R*interpFactor),
                    (int)(FadeInStartColor.G * (1-interpFactor) + FadeBaseColor.G*interpFactor),
                    (int)(FadeInStartColor.B * (1-interpFactor) + FadeBaseColor.B*interpFactor),
                    (int)(FadeInStartColor.A * (1-interpFactor) + FadeBaseColor.A*interpFactor));
            }
            else if (percentLifetime < 1 - FadeOutDuration)
            {
                CurrentColor = FadeBaseColor;
            }
            else
            {
                float interpFactor = 1 - ((percentLifetime - (1 - FadeOutDuration))/FadeOutDuration);
                CurrentColor = new Color((int)(FadeOutEndColor.R * (1 - interpFactor) + FadeBaseColor.R * interpFactor),
                    (int)(FadeOutEndColor.G * (1 - interpFactor) + FadeBaseColor.G * interpFactor),
                    (int)(FadeOutEndColor.B * (1 - interpFactor) + FadeBaseColor.B * interpFactor),
                    (int)(FadeOutEndColor.A * (1 - interpFactor) + FadeBaseColor.A * interpFactor));
            }
        }
        /// <summary>
        /// Dessine la particule à l'écran.
        /// </summary>
        /// <param name="batch"></param>
        public override void Draw(SpriteBatch batch)
        {

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
